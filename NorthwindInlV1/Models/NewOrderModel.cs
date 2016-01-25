using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NorthwindInlV1.Models
{
    public class NewOrderModel
    {
        public IEnumerable<SelectListItem> CustomerNamesForEmployee { get; set; }
        public IEnumerable<SelectListItem> CustomerNames { get; set; }
        public Customer Customer { get; set; }
        public IEnumerable<SelectListItem> Shippers { get; set; }
        public Order Order { get; set; }
        
    }
}