using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp1.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp1.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class Table_1Controller : ControllerBase
    {
        private readonly AppDbContext _context;

        public Table_1Controller(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Table_1
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tables_1>>> GetTables_1()
        {
            if (_context.Table_1 == null)
            {
                return NotFound();
            }
            return await _context.Table_1.ToListAsync();
        }

        // GET: api/Table_1/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tables_1>> GetTable_1(int id)
        {
            if (_context.Table_1 == null)
            {
                return NotFound();
            }
            var table_1 = await _context.Table_1.FindAsync(id);

            if (table_1 == null)
            {
                return NotFound();
            }

            return table_1;
        }

        // PUT: api/Table_1/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTable_1(int id, Tables_1 table_1)
        {
            if (id != table_1.Id)
            {
                return BadRequest();
            }

            _context.Entry(table_1).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Table_1Exists(id))
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

        // POST: api/Table_1
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Tables_1>> PostTable_1(Tables_1 table_1)
        {
            if (_context.Table_1 == null)
            {
                return Problem("Entity set 'AppDbContext.Tables_1'  is null.");
            }
            _context.Table_1.Add(table_1);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTable_1", new { id = table_1.Id }, table_1);
        }

        // DELETE: api/Table_1/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTable_1(int id)
        {
            if (_context.Table_1 == null)
            {
                return NotFound();
            }
            var table_1 = await _context.Table_1.FindAsync(id);
            if (table_1 == null)
            {
                return NotFound();
            }

            _context.Table_1.Remove(table_1);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool Table_1Exists(int id)
        {
            return (_context.Table_1?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
