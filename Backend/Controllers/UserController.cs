using Backend.Data;
using Backend.Models;
using Backend.Models.DTOs;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController: ControllerBase {
    private readonly AppDbContext _context; 

    public UserController(AppDbContext context) {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers() {
        return await _context.Users.ToListAsync(); 
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<User>> GetUser(int id) {
        var user = await _context.Users.FindAsync(id); 

        if(user == null) return NotFound(); 
        return user; 
    }

    [HttpGet("find")]
    public async Task<ActionResult<User>> FindUser([FromQuery]string username, [FromQuery]string password) {

        try {
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Username == username && user.Password == password); 

            if(user == null) return NotFound(); 

            return user; 
        } catch (Exception e) {
            return BadRequest(e.Message); 
        }
    }

    [HttpPost]
    public async Task<ActionResult<User>> AddUser([FromBody] RegisterFormValues formValues) {

        if(!ModelState.IsValid) {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage); 
            foreach(var error in errors) {
                Console.WriteLine(error); 
            }
            return BadRequest(ModelState); 
        }

        User user = new User {
            Email = formValues.Email,
            Username = formValues.Username,
            Password = formValues.Password,
        };

        _context.Users.Add(user); 
        await _context.SaveChangesAsync(); 

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user); 
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}