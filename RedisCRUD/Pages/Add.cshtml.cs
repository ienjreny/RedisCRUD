using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RedisCRUD.Models;
using StackExchange.Redis;

namespace RedisCRUD.Pages
{
    public class AddModel : PageModel
    {
        private readonly IConnectionMultiplexer muxer;

        public AddModel(IConnectionMultiplexer multiplexer)
        {
            muxer = multiplexer;
        }

        [BindProperty]
        public ProductModel Product { get; set; }

        public void OnGet()
        {
            Product = new ProductModel();
            Product.ID = Guid.NewGuid().ToString();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            List<HashEntry> hashFields = new List<HashEntry>();

            HashEntry id = new HashEntry("id", Product.ID);
            hashFields.Add(id);

            HashEntry name = new HashEntry("name", Product.Name);
            hashFields.Add(name);

            HashEntry age = new HashEntry("price", Product.Price);
            hashFields.Add(age);

            await muxer.GetDatabase().HashSetAsync("product:" + Product.ID, hashFields.ToArray());

            return RedirectToPage("./Index");
        }
    }
}
