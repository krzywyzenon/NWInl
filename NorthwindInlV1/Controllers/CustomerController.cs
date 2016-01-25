using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NorthwindInlV1.Models;

namespace NorthwindInlV1.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Customer
        [Authorize(Roles = "Manager, SalesManager")]
        public ActionResult FindCustomer()
        {
            //List<Customer> customers = new List<Customer>();
            //Customer c = new Customer();
            CustomerModel customerModel = new CustomerModel();
            return View(customerModel);
        }

        [HttpPost]
        [Authorize(Roles = "Manager, SalesManager")]
        public ActionResult FindCustomer(CustomerModel c)
        {
            List<Customer> customers;
            using (NorthwindConnection db = new NorthwindConnection())
            {
                if (c.Customer.CompanyName != null && c.Customer.City != null)
                {
                    customers =
                        db.Customers.Where(x => x.CompanyName.Contains(c.Customer.CompanyName) && x.City == c.Customer.City).ToList();
                }
                else if (c.Customer.CompanyName != null)
                {
                    customers = db.Customers.Where(x => x.CompanyName.Contains(c.Customer.CompanyName)).ToList();
                }
                else
                {
                    customers = db.Customers.Where(x => x.City == c.Customer.City).ToList();
                }
            }

            CustomerModel cm = new CustomerModel();
            cm.Customers = customers;
            return View(cm);
        }

    }
}