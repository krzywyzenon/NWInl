using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using NorthwindInlV1.Models;

namespace NorthwindInlV1.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User u)
        {
            User user;
            using (NorthwindConnection db = new NorthwindConnection())
            {
                user = db.Users.SingleOrDefault(x => x.UserName == u.UserName && x.Password == u.Password);
            }

            if (user != null)
            {
                FormsAuthentication.SetAuthCookie(user.UserName, true);
                if (user.EmployeeID != null)
                {
                    Session["EmployeeId"] = user.EmployeeID;
                }
                else if (user.CustomerID != null)
                {
                    Session["CustomerId"] = user.CustomerID;
                }
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }

        [Authorize]
        public ActionResult AccountDetails()
        {
            Employee employee;
            Customer customer;
            AccountDetailsModel model = new AccountDetailsModel();
            string type;
            if (Session["EmployeeId"] != null)
            {
                int id = (int) Session["EmployeeId"];
                using (NorthwindConnection db = new NorthwindConnection())
                {
                    employee = db.Employees.SingleOrDefault(p => p.EmployeeID == id);
                }
                type = "employee";
                model.Employee = employee;
            }
            else
            {
                string id = (string) Session["CustomerId"];
                using (NorthwindConnection db = new NorthwindConnection())
                {
                    customer = db.Customers.SingleOrDefault(c => c.CustomerID == id);
                }
                model.Customer = customer;
                type = "customer";
            }
            model.UserType = type;

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateProfile(AccountDetailsModel m)
        {
            AccountDetailsModel model = m;
            if (m.UserType == "employee")
            {
                using (NorthwindConnection db = new NorthwindConnection())
                {
                    Employee e = db.Employees.SingleOrDefault(x => x.EmployeeID == m.Employee.EmployeeID);
                    
                    db.Employees.SingleOrDefault(x => x.EmployeeID == m.Employee.EmployeeID).FirstName =
                        m.Employee.FirstName;

                    db.Employees.SingleOrDefault(x => x.EmployeeID == m.Employee.EmployeeID).LastName =
                        m.Employee.LastName;
                    db.SaveChanges();

                }
            }
            else
            {
                using (NorthwindConnection db = new NorthwindConnection())
                {

                    db.Customers.SingleOrDefault(x => x.CustomerID == m.Customer.CustomerID).ContactName =
                        m.Customer.ContactName;

                    db.Customers.SingleOrDefault(x => x.CustomerID == m.Customer.CustomerID).ContactTitle =
                        m.Customer.ContactTitle;
                    db.SaveChanges();
                }
            }
            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}