using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NorthwindInlV1.Models
{
    public class ProductsListModel
    {
        public List<Product> Products { get; set; }
        public int OrderId { get; set; }
        public short Amount { get; set; }
    }
}