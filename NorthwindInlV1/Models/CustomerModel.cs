using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NorthwindInlV1.Models
{
    public class CustomerModel
    {
        public Customer Customer { get; set; }
        public List<Customer> Customers { get; set; }
    }
}