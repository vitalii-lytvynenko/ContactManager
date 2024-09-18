using ContactManager.Context;
using ContactManager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ContactManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadCSV(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty");

            using (var stream = new StreamReader(file.OpenReadStream()))
            {
                while (!stream.EndOfStream)
                {
                    var line = await stream.ReadLineAsync();
                    var values = line.Split(',');

                    var contact = new User
                    {
                        Name = values[0],
                        DateOfBirth = DateTime.Parse(values[1]),
                        Married = bool.Parse(values[2]),
                        Phone = values[3],
                        Salary = decimal.Parse(values[4])
                    };

                    _context.Users.Add(contact);
                }
            }

            await _context.SaveChangesAsync();
            return Ok("Data uploaded successfully");
        }

        [HttpGet]
        public IActionResult GetContacts()
        {
            return Ok(_context.Users.ToList());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContact(int id, User updatedContact)
        {
            var contact = await _context.Users.FindAsync(id);
            if (contact == null) return NotFound();

            contact.Name = updatedContact.Name;
            contact.DateOfBirth = updatedContact.DateOfBirth;
            contact.Married = updatedContact.Married;
            contact.Phone = updatedContact.Phone;
            contact.Salary = updatedContact.Salary;

            await _context.SaveChangesAsync();
            return Ok(contact);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var contact = await _context.Users.FindAsync(id);
            if (contact == null) return NotFound();

            _context.Users.Remove(contact);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
