using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthorService.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Policy;
using System.Xml.Linq;
using System.Security.Cryptography;
using AuthorService.NewFolder;

namespace AuthorService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class BooksController : ControllerBase
    {
        private readonly DigitalBooksContext _context;

        public BooksController(DigitalBooksContext context)
        {
            _context = context;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
          if (_context.Books == null)
          {
                return NotFound();
          }
            var b= _context.Books.Where(x =>x.Active==true).ToList();
            if(b==null)
            {
                return NotFound();
            }
            return b;
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public  string GetBook(int id)
        {
          if (_context.Books == null)
          {
              return "NA";
          }
            var book = _context.Books.Where(x => x.BookId.Equals(id)).FirstOrDefault();
            string bname = book.BookName;
            if (book == null)
            {
                return "NA";
            }

            return bname;
        }
        
        [HttpGet]
        [Route("GetauthorBook")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Book>>> GetauthorBook(string uid)
        {
            if (_context.Books == null)
            {
                return NotFound();
            }
            
            var book = await _context.Books.Where(x => x.UserId.ToString() == uid).ToListAsync();
            if (book == null)
            {
                return NotFound();
            }
            //else
            //{
            //    foreach (var item in book)
            //    {
            //        item.User.Books = null;
            //    }
            //}
            return book;
        }

        [HttpGet]
        [Route("SerachBook")]
        public async Task<ActionResult<IEnumerable<Book>>> SearchBook(string BName,string Author,string Publisher,DateTime Pd )
        {
            if (_context.Books == null)
            {
                return NotFound();
            }
            var userlist = await _context.Users.Where(x => x.UserName.Equals(Author)).ToListAsync();
            int userId = 0;
            if (userlist.Count() > 0)
                userId = userlist.Select(x => x.UserId).FirstOrDefault();
            var book = await _context.Books.Where(x => x.BookName.Equals(BName) || x.UserId == userId || x.Publisher == Publisher || x.PublishedDate == Pd || x.Active == true).ToListAsync();
            //var book1 = await _context.Books.Where(x => x.UserId == userId && x.Active == true).ToListAsync();
           // List<Book> bookall = new List<Book>();
            //foreach (var item in book)
            //{
            //    bookall.Add(item);
            //}
            //foreach (var item in book1)
            //{
            //    bookall.Add(item);
            //}
            if (book == null)
            {
                return NotFound();
            }
            else
            {
                foreach (var item in book)
                {
                    if(item.User!= null)
                    {
                        item.User.Books = null;
                    }
                    else
                    {
                        item.User=await _context.Users.Where(x => x.UserId == item.UserId).SingleOrDefaultAsync();
                        item.User.Books = null;
                    }
                }
            }
            return book;


        }

        [HttpGet]
        [Route("SearchBook")]
        public List<ViewBook> SearchBooks(string BName, string Author, string Publisher, DateTime Pd)
        {
            List<ViewBook> lsBookMaster = new List<ViewBook>();
            if (_context.Books == null)
            {
                return lsBookMaster;
            }

            try
            {
                lsBookMaster = (from b in _context.Books
                                join users in _context.Users on b.UserId equals users.UserId
                                where b.BookName == BName || users.UserName == Author
                                && b.Publisher == Publisher || b.PublishedDate == Pd
                                && b.Active == true
                                select new
                                {
                                    BookId = b.BookId,
                                    BookName = b.BookName,
                                    Author = users.FirstName + " " + users.LastName,
                                    Publisher = b.Publisher,
                                    Price = b.Price,
                                    PublishedDate = b.PublishedDate

                                }).ToList()
                                .Select(x => new ViewBook()
                                {
                                    BookId = x.BookId,
                                    Title = x.BookName,
                                    Author = x.Author,
                                    Publisher = x.Publisher,
                                    Price = Convert.ToDouble(x.Price),
                                    PublishedDate = (DateTime)x.PublishedDate
                                }).ToList();
            }
            catch (Exception ex)
            {
                return lsBookMaster;
            }

            return lsBookMaster;
        }

        [HttpPost]
        [Route("AddBook")]
        public async  Task<ActionResult<IEnumerable<Book>>> AddBook(string un,Book book)
        { 
            var ulist = await _context.Users.Where(x => x.UserName.Equals(un)).ToListAsync();
            var authorid = _context.Users.Where(x => x.UserName == un).FirstOrDefault().UserId;
            var count = _context.Books.Where(x => x.BookName == book.BookName).Count();
            //var bookauthorid = _context.Books.Where(x => x.BookName == bookname).FirstOrDefault().UserId;
            if (ulist.Count() > 0)
            {
                if (count == 0)
                {
                    book.UserId = ulist.Select(x => x.UserId).FirstOrDefault();
                    book.User = null;
                }
                else
                {
                    return Problem("Book with this name already exists");
                }
            }
            else { return Problem("Author not available"); }

            if (_context.Books == null)
            {
                return Problem("Entity set 'DigitalBooksContext.Books'  is null.");
                //return NotFound();
                
            }
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBook", new { id = book.BookId }, book);          
            
        }

        // PUT: api/Books/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.BookId)
            {
                return BadRequest();
            }

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
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
        [HttpPut]
        [Route("Deactivebook")]
        public async Task<IActionResult> Deactivebook(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            if (!BookExists(id))
            {
                return NotFound();
            }

            try
            {
                Book b= await _context.Books.Where(x => x.BookId== id).SingleOrDefaultAsync();
               
                if(b != null)
                {
                    b.Active =!b.Active;
                    _context.Entry(b).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                return CreatedAtAction("GetBook", new { id = b.BookId }, b);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }
        [HttpPut]
        [Route("ubook")]
        public async Task<IActionResult> ubook(int id,string content)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            if (!BookExists(id))
            {
                return NotFound();
            }

            try
            {
                Book b = await _context.Books.Where(x => x.BookId == id).SingleOrDefaultAsync();

                if (b != null)
                {
                    b.Content = content;
                    _context.Entry(b).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                return CreatedAtAction("GetBook", new { id = b.BookId }, b);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }
        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            if (_context.Books == null)
            {
                return Problem("Entity set 'DigitalBooksContext.Books'  is null.");
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBook", new { id = book.BookId }, book);
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            if (_context.Books == null)
            {
                return NotFound();
            }
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookExists(int id)
        {
            return (_context.Books?.Any(e => e.BookId == id)).GetValueOrDefault();
        }
    }
}
