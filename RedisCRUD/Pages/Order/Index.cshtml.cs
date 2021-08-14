using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NRediSearch;
using RedisCRUD.Models;
using StackExchange.Redis;

namespace RedisCRUD.Pages.Order
{
    public class IndexModel : PageModel
    {
        private readonly IConnectionMultiplexer muxer;

        public IndexModel(IConnectionMultiplexer multiplexer)
        {
            muxer = multiplexer;
        }

        public IList<OrderItemModel> Items { get; set; }

        public void OnGet()
        {
            Client client = new("idx_orders", muxer.GetDatabase());
            Query query = new("*");

            SearchResult res = client.Search(query);

            Items = new List<OrderItemModel>();
            foreach (Document doc in res.Documents)
            {
                OrderItemModel item = new()
                {
                    ID = doc["id"].ToString(),
                    ProductName = doc["product_name"].ToString(),
                    ProductID = doc["product_id"].ToString(),
                    ProductPrice = (int)doc["product_price"],
                    Qty = (int)doc["qty"]
                };

                Items.Add(item);
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            RedisKey key = new("order:" + id);
            await muxer.GetDatabase().KeyDeleteAsync(key);

            return RedirectToPage("Index");
        }

        public async Task<IActionResult> OnPostDoneAsync(string id)
        {
            List<HashEntry> hashFields = new();

            HashEntry status = new("status", "Done");
            hashFields.Add(status);

            await muxer.GetDatabase().HashSetAsync("order:" + id, hashFields.ToArray());

            return RedirectToPage("Index");
        }
    }
}
