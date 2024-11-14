using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BlazorApp1.Models;

namespace BlazorApp1.Pages
{
    public class Table_1Model : PageModel
    {
        private readonly BlazorApp1.Models.TesteContext _context;

        public Table_1Model(BlazorApp1.Models.TesteContext context)
        {
            _context = context;
        }

        public IList<Table1> Table1 { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Table1s != null)
            {
                Table1 = await _context.Table1s.ToListAsync();
            }
        }
    }
}
