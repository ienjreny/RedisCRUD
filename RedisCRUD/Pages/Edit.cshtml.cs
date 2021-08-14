using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NRediSearch;
using RedisCRUD.Models;
using StackExchange.Redis;

namespace RedisCRUD.Pages
{
    public class EditModel : PageModel
    {
        private readonly IConnectionMultiplexer muxer;

        public EditModel(IConnectionMultiplexer multiplexer)
        {
            muxer = multiplexer;
        }

        [BindProperty]
        public ProductModel Product { get; set; }

        public void OnGet(string id)
        {
            string productID = id.Replace("-", "\\-");

            Client client = new("idx_products", muxer.GetDatabase());
            SearchResult res = client.Search(new Query("@id:{" + productID + "}") { WithPayloads = true });

            Product = new()
            {
                ID = res.Documents[0]["id"].ToString(),
                Name = res.Documents[0]["name"].ToString(),
                Price = (int)res.Documents[0]["price"]
            };
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
