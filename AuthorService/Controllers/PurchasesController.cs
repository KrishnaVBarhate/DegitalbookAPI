using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthorService.Models;
using System.Security.Cryptography;

namespace AuthorService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchasesController : ControllerBase
    {
        private readonly DigitalBooksContext _context;

        public PurchasesController(DigitalBooksContext context)
        {
            _context = context;
        }

        // GET: api/Purchases
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Purchase>>> GetPurchases()
        {
          if (_context.Purchases == null)
          {
              return NotFound();
          }
            return await _context.Purchases.ToListAsync();
        }

        // GET: api/Purchases/5
        [HttpGet]
        [Route("Getpurchaseeml")]
        public async Task<ActionResult<IEnumerable<Purchase>>> GetPurchase(string eml)
        {
          if (_context.Purchases == null)
          {
                return NotFound();
          }
            
            var purchase = await _context.Purchases.Where(x => x.EmailId== eml).ToListAsync();

            if (purchase == null)
            {
                return NotFound();

            }
            else
            {
                foreach (var p in purchase)
                {
                    p.Book = await _context.Books.Where(x => x.BookId == p.BookId).SingleOrDefaultAsync();
                    p.Book.Purchases = null;

                }
            }
            

               return purchase;
            
        }

        [HttpGet]
        [Route("Getpurchasedbook")]
        public async Task<ActionResult<IEnumerable<Purchase>>> GetPurchaseReader(string eml)
        {
            if (_context.Purchases == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchases.Where(x => x.EmailId == eml).ToListAsync();

            if (purchase == null)
            {
                return NotFound();

            }
            else
            {
                foreach (var p in purchase)
                {
                    p.Book = await _context.Books.Where(x => x.BookId == p.BookId).SingleOrDefaultAsync();
                    p.Book.Purchases = null;

                }
            }


            return purchase;

        }

        // PUT: api/Purchases/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPurchase(int id, Purchase purchase)
        {
            if (id != purchase.PurchaseId)
            {
                return BadRequest();
            }

            _context.Entry(purchase).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PurchaseExists(id))
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

        // POST: api/Purchases
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Purchase>> PostPurchase(Purchase purchase)
        {
          if (_context.Purchases == null)
          {
              return Problem("Entity set 'DigitalBooksContext.Purchases'  is null.");
          }

            var count = _context.Purchases.Where(x=>x.EmailId == purchase.EmailId && x.BookId == purchase.BookId).Count();
            if (count == 0)
            {
                _context.Purchases.Add(purchase);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetPurchase", new { id = purchase.PurchaseId }, purchase);
            }
            else
            {
                return Problem("Purchase Already Exists");
            }
        }

        // DELETE: api/Purchases/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchase(int id)
        {
            if (_context.Purchases == null)
            {
                return NotFound();
            }
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
            {
                return NotFound();
            }

            _context.Purchases.Remove(purchase);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PurchaseExists(int id)
        {
            return (_context.Purchases?.Any(e => e.PurchaseId == id)).GetValueOrDefault();
        }
    }
}
