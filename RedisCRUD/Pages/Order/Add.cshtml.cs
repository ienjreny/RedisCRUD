using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NRediSearch;
using RedisCRUD.Models;
using StackExchange.Redis;

namespace RedisCRUD.Pages.Order
{
    public class AddModel : PageModel
    {
        private readonly IConnectionMultiplexer muxer;

        public AddModel(IConnectionMultiplexer multiplexer)
        {
            muxer = multiplexer;
        }

        public IList<SelectListItem> Products { get; set; }

        [BindProperty]
        public string ProductID { get; set; }

        [BindProperty]
        public int Qty { get; set; }

        public void OnGet()
        {
            Client client = new("idx_products", muxer.GetDatabase());
            SearchResult result = client.Search(new Query("*"));

            Products = new List<SelectListItem>();
            foreach (Document doc in result.Documents)
            {
                SelectListItem item = new(doc["name"].ToString(), doc["id"].ToString());

                Products.Add(item);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            string productID = ProductID.Replace("-", "\\-");

            Client client = new("idx_products", muxer.GetDatabase());
            SearchResult res = client.Search(new Query("@id:{" + productID + "}"));

            List<HashEntry> hashFields = new();

            HashEntry id = new("id", Guid.NewGuid().ToString());
            hashFields.Add(id);

            HashEntry product_name = new("product_name", res.Documents[0]["name"].ToString());
            hashFields.Add(product_name);

            HashEntry product_id = new("product_id", ProductID);
            hashFields.Add(product_id);

            HashEntry product_price = new("product_price", (int)res.Documents[0]["price"]);
            hashFields.Add(product_price);

            HashEntry qty = new("qty", Qty);
            hashFields.Add(qty);

            HashEntry status = new("status", "New");
            hashFields.Add(status);

            // ref: https://stackoverflow.com/questions/25976231/stackexchange-redis-transaction-methods-freezes
            var tran = muxer.GetDatabase().CreateTransaction();

            // create a new order
            var orderTask = tran.HashSetAsync("order:" + id.Value.ToString(), hashFields.ToArray());

            // decrease the selected product QTY
            var productTask = tran.HashDecrementAsync("product:" + ProductID, "qty", Qty);

            // execute all commands
            await tran.ExecuteAsync();

            return RedirectToPage("Index");
        }
    }
}
