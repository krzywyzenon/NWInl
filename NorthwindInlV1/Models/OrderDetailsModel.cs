using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NorthwindInlV1.Models
{
    public class OrderDetailsModel
    {
        public Order Order { get; set; }
        public List<Order_Detail> OrderDetails { get; set; }
        public Customer Customer { get; set; }
        public List<Product> Products { get; set; }
        public IEnumerable<SelectListItem> categories { get; set; }
        public Product Product { get; set; }
        public int MaxPrice { get; set; }
        public int MinPrice { get; set; }
    }
}