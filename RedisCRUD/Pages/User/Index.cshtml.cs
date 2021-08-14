using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NRediSearch;
using RedisCRUD.Models;
using StackExchange.Redis;

namespace RedisCRUD.Pages.User
{
    public class IndexModel : PageModel
    {
        private readonly IConnectionMultiplexer muxer;

        public IndexModel(IConnectionMultiplexer multiplexer)
        {
            muxer = multiplexer;
        }

        public IList<UserModel> Users { get; set; }

        public void OnGet()
        {
            Client client = new("idx_users", muxer.GetDatabase());
            Query query = new("*");

            SearchResult res = client.Search(query);

            Users = new List<UserModel>();
            foreach (Document doc in res.Documents)
            {
                UserModel item = new()
                {
                    ID = (long)doc["id"],
                    FullName = doc["name"].ToString()
                };

                Users.Add(item);
            }
        }
    }
}
