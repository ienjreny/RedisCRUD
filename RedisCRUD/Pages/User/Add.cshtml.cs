using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RedisCRUD.Models;
using StackExchange.Redis;

namespace RedisCRUD.Pages.User
{
    public class AddModel : PageModel
    {
        private readonly IConnectionMultiplexer muxer;

        public AddModel(IConnectionMultiplexer multiplexer)
        {
            muxer = multiplexer;
        }

        [BindProperty]
        public UserModel MyUser { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            List<HashEntry> hashFields = new();

            // get new user ID
            MyUser.ID = muxer.GetDatabase().StringIncrement("user_id", 1);

            HashEntry id = new("id", MyUser.ID);
            hashFields.Add(id);

            HashEntry name = new("name", MyUser.FullName);
            hashFields.Add(name);

            muxer.GetDatabase().HashSet("user:" + MyUser.ID.ToString(), hashFields.ToArray());

            return RedirectToPage("./Index");
        }
    }
}
