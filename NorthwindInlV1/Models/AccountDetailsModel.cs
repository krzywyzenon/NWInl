using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NorthwindInlV1.Models
{
    public class AccountDetailsModel
    {
        public Employee Employee { get; set; }
        public Customer Customer { get; set; }
        public string UserType { get; set; }
    }
}