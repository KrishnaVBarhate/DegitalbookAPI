using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthorService.Models;
using Microsoft.AspNetCore.Authorization;


namespace AuthorService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize]
    //MTIzNDU=
    public class UsersController : ControllerBase
    {
        private readonly DigitalBooksContext _context;

        public UsersController(DigitalBooksContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpGet]
        [Route("Userbyname")]
        public async Task<ActionResult<IEnumerable<User>>> GetUserbyname(string uname)
        {
            
            if (_context.Users == null)
            {
                return NotFound();
            }
            var ulist = await _context.Users.Where(x => x.UserName.Equals(uname)).ToListAsync();
            

            return ulist;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User userTable)
        {
            if (id != userTable.UserId)
            {
                return BadRequest();
            }

            _context.Entry(userTable).State = EntityState.Modified;

            try
            {
                var DbUser = _context.Users.FirstOrDefault(x => x.UserId.Equals(id));



                DbUser.UserName = userTable.UserName;
                DbUser.EmailId = userTable.EmailId;
                DbUser.FirstName = userTable.FirstName;
                DbUser.LastName = userTable.LastName;
                DbUser.Active = userTable.Active;
                DbUser.UserPassword = EncryptionDecryption.EncodePasswordToBase64(userTable.UserPassword);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            var uncount = _context.Users.Where(x => x.UserName == user.UserName).Count();
            var emcount = _context.Users.Where(x => x.EmailId == user.EmailId).Count();
            if (_context.Users == null)
          {
              return Problem("Entity set 'DigitalBooksContext.Users'  is null.");
          }
            if(uncount== 0)
            {
                if(emcount == 0)
                {
                    user.UserPassword=EncryptionDecryption.EncodePasswordToBase64(user.UserPassword);
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                    return CreatedAtAction("GetUser", new { id = user.UserId }, user);
                }
                else
                {
                    return Problem("Duplicate email id");
                }
            }
            else
            {
                return Problem("Duplicate user name");
            }


            return CreatedAtAction("GetUser", new { id = user.UserId }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}
