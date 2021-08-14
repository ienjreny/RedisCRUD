using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RedisCRUD.Models
{
    public class ProductModel
    {
        [Required]
        public string ID { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; }

        [Required]
        public int Price { get; set; }

    }
}
