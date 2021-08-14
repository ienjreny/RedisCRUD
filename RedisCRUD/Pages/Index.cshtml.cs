using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NRediSearch;
using RedisCRUD.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisCRUD.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IConnectionMultiplexer muxer;

        [BindProperty]
        public ProductModel Product { get; set; }

        //public async Task<IActionResult> OnPostAsync()
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Page();
        //    }
        //}

        public IList<ProductModel> Products { get; set; }
        public long PagesCount { get; set; }

        [FromQuery(Name = "page")]
        public int CurrentPage { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IConnectionMultiplexer multiplexer)
        {
            _logger = logger;
            muxer = multiplexer;
        }

        public void OnGet(string name)
        {
            Product = new()
            {
                Name = name
            };

            string queryString = "*";
            if (!string.IsNullOrWhiteSpace(name))
            {
                queryString = $"@name:{name}*";
            }

            Client client = new("idx_products", muxer.GetDatabase());
            Query query = new(queryString);
            
            // pagination
            query.Limit(CurrentPage, 2);

            // sort by
            query.SortBy = "name";

            SearchResult res = client.Search(query);
            PagesCount = res.TotalResults / 2;

            Products = new List<ProductModel>();
            foreach (Document doc in res.Documents)
            {
                ProductModel product = new()
                {
                    ID = doc["id"].ToString(),
                    Name = doc["name"].ToString(),
                    Price = (int)doc["price"]
                };

                Products.Add(product);
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            RedisKey key = new("product:" + id);
            await muxer.GetDatabase().KeyDeleteAsync(key);

            return RedirectToPage("./Index");
        }
    }
}
