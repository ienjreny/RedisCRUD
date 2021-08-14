using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisCRUD.Models
{
    public class OrderItemModel
    {
        public string ID { get; set; }
        public string ProductName { get; set; }
        public string ProductID { get; set; }
        public int Qty { get; set; }
        public int ProductPrice { get; set; }
    }
}
