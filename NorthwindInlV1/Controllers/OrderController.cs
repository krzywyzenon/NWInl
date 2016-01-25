using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NorthwindInlV1.Models;

namespace NorthwindInlV1.Controllers
{
    public class OrderController : Controller
    {
        // GET: Order
        [Authorize(Roles = "Manager")]
        public ActionResult ViewAllOrders()
        {
            List<Order> orders;
            using (NorthwindConnection db = new NorthwindConnection())
            {
                orders = db.Orders.Select(p=>p).Include(p=>p.Customer).ToList();
            }
            TempData["orders"] = orders;
            return RedirectToAction("ViewOrders");
        }


        [Authorize]
        public ActionResult ViewOrders()
        {
            List<Order> orders;
            if (TempData["orders"] != null)
            {
                orders = (List<Order>)TempData["orders"];
            }
            else
            {
                using (NorthwindConnection db = new NorthwindConnection())
                {
                    if (Session["EmployeeId"] != null)
                    {
                        int id = (int) Session["EmployeeId"];
                        orders = db.Orders.Select(p => p)
                            .Include(p => p.Customer)
                            .Where(p => p.EmployeeID == id).ToList();
                    }
                    else
                    {
                        string id = (string) Session["CustomerId"];
                        orders = db.Orders.Select(p => p)
                            .Include(p => p.Customer)
                            .Where(p => p.CustomerID == id)
                            .ToList();
                    }
                    
                }
            }
            return View(orders);
        }

        [Authorize]
        public ActionResult OrderDetails(int id)
        {
            Order order;
            List<Order_Detail> orderDetail;
            Customer customer;
            List<Product> products;
            List<Category> categories;
            using (NorthwindConnection db = new NorthwindConnection())
            {
                order = db.Orders.SingleOrDefault(o => o.OrderID == id);
                orderDetail = GetOrderDetails(db, order.OrderID);
                customer = db.Customers.SingleOrDefault(c => c.CustomerID == order.CustomerID);
                products = db.Products.ToList();
                categories = db.Categories.ToList();
            }

            OrderDetailsModel model = new OrderDetailsModel();
            model.Order = order;
            model.OrderDetails = orderDetail;
            model.Customer = customer;
            model.Products = products;
            model.categories = categories.Select(x => new SelectListItem()
            {
                Value = x.CategoryID.ToString(),
                Text = x.CategoryName
            });

            return PartialView("_OrderDetails", model);
        }

        [Authorize(Roles = "Manager, SalesManager")]
        public ActionResult DeleteOrder(int id)
        {
            
            Order order;
            List<Order_Detail> orderDetails;
            List<Order> orders;
            using (NorthwindConnection db = new NorthwindConnection())
            {
                order = db.Orders.SingleOrDefault(x => x.OrderID == id);
                if (order.ShippedDate > DateTime.Now)
                {
                    
                    orderDetails = db.Order_Details.Where(x => x.OrderID == id).ToList();
                    List<int> prodIds = new List<int>();
                    foreach (var orderDetail in orderDetails)
                    {
                        prodIds.Add(orderDetail.ProductID);
                    }
                    foreach (var prodId in prodIds)
                    {
                        db.DeleteOrderDetail(id, prodId);
                    }

                    db.DeleteOrder(id);
                    orders = db.Orders.ToList();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View();
                }
                
            }
        }

        [Authorize(Roles = "Manager, SalesManager")]
        public ActionResult ChangeAmount(int orderId, int productId, string change, short amount)
        {
            Order order;
            List<Order_Detail> orderDetail;
            Customer customer;
            List<Product> products;
            List<Category> categories;
            using (NorthwindConnection db = new NorthwindConnection())
            {
                categories = db.Categories.ToList();
                products = db.Products.ToList();
                order = db.Orders.SingleOrDefault(o => o.OrderID == orderId);
                orderDetail = GetOrderDetails(db, order.OrderID);
                    
                short quantity = orderDetail.SingleOrDefault(x => x.ProductID == productId).Quantity;
                if (change == "add")
                {
                    quantity += amount;
                }
                else
                {
                    if ((quantity - amount) > 0)
                    {
                        quantity -= amount;
                    }
                    else
                    {
                        quantity = 0;
                    }
                }
                if (quantity > 0)
                {
                    orderDetail.SingleOrDefault(x => x.ProductID == productId).Quantity = quantity;
                }
                else
                {
                    Order_Detail rowToRemove = orderDetail.SingleOrDefault(x => x.ProductID == productId);
                    orderDetail.Remove(rowToRemove);
                    db.DeleteOrderDetail(order.OrderID, productId);
                }
                customer = db.Customers.SingleOrDefault(c => c.CustomerID == order.CustomerID);
                db.SaveChanges();
            }

            OrderDetailsModel model = new OrderDetailsModel();
            model.Order = order;
            model.OrderDetails = orderDetail;
            model.Customer = customer;
            model.Products = products;
            model.categories = categories.Select(x => new SelectListItem()
            {
                Value = x.CategoryID.ToString(),
                Text = x.CategoryName
            });

            return PartialView("_OrderDetails", model);
        }

        [Authorize]
        private List<Order_Detail> GetOrderDetails(NorthwindConnection db, int orderId)
        {
            List<Order_Detail> orderDetail;

            
                orderDetail =
                    db.Order_Details.Select(p => p)
                        .Where(p => p.OrderID == orderId)
                        .Include(p => p.Product)
                        .ToList();
            

            return orderDetail;

        }

        [Authorize]
        public ActionResult FindProducts(OrderDetailsModel model, int id)
        {
            List<Product> products;
            using (NorthwindConnection db = new NorthwindConnection())
            {
                products = db.Products.Where(x => x.CategoryID == model.Product.CategoryID && x.UnitPrice>=model.MinPrice).ToList();
                if (model.MaxPrice > 0)
                {
                    products = products.Where(x => x.UnitPrice <= model.MaxPrice).ToList();
                }
                if (model.Product.ProductName != null && model.Product.ProductName!="")
                {
                    products = products.Where(x=>x.ProductName.Contains(model.Product.ProductName)).ToList();
                }
            }
            ProductsListModel plModel = new ProductsListModel
            {
                Products = products,
                OrderId = id
            };
            return PartialView("_ProductList", plModel);
        }

        [Authorize(Roles = "Manager, SalesManager")]
        public ActionResult AddNewProduct(int productId, int orderId, short amount)
        {
            List<Order_Detail> orderDetails;
            using (NorthwindConnection db = new NorthwindConnection())
            {
                orderDetails = GetOrderDetails(db, orderId);
                bool isNew = true;
                foreach (var orderDetail in orderDetails)
                {
                    if (orderDetail.ProductID == productId)
                    {
                        isNew = false;
                    }
                }

                if (isNew)
                {
                    Product prod = db.Products.SingleOrDefault(x => x.ProductID == productId);
                    //Order order = db.Orders.SingleOrDefault(p => p.OrderID == orderId);
                    db.UpdateOrderDetails(orderId, productId, prod.UnitPrice, amount, 0);
                    db.SaveChanges();
                }
                else
                {
                    return RedirectToAction("ChangeAmount", new{orderId = orderId, productId = productId, change = "add", amount = amount});
                }
            }
            return RedirectToAction("OrderDetails", new{id = orderId});
        }

        [Authorize(Roles = "Manager, SalesManager")]
        public ActionResult AddNewOrder()
        {
            int employeeId = (int) Session["EmployeeId"];
            List<Customer> customers;
            List<Customer> customersForEmployee = new List<Customer>();
            List<Shipper> shippers;
            using (NorthwindConnection db = new NorthwindConnection())
            {
                shippers = db.Shippers.ToList();
                customers = db.Customers.ToList();
                var orders = db.Orders.Where(x=>x.EmployeeID == employeeId).ToList().GroupBy(x=>x.CustomerID);
                foreach (IGrouping<string, Order> grouping in orders)
                {
                    Customer customer = db.Customers.SingleOrDefault(x => x.CustomerID == grouping.Key);
                    customersForEmployee.Add(customer);
                }
            }
            NewOrderModel model = new NewOrderModel();
            model.CustomerNames = customers.Select(x => new SelectListItem()
            {
                Value = x.CustomerID,
                Text = x.CompanyName
            });

            model.CustomerNamesForEmployee = customersForEmployee.Select(x => new SelectListItem()
            {
                Value = x.CustomerID,
                Text = x.CompanyName
            });
            model.Shippers = shippers.Select(x => new SelectListItem()
            {
                Value = x.ShipperID.ToString(),
                Text = x.CompanyName
            });
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Manager, SalesManager")]
        public ActionResult AddNewOrder(NewOrderModel model)
        {
            Customer customer;
            int id = (int) Session["EmployeeId"];

            using (NorthwindConnection db = new NorthwindConnection())
            {
                customer = db.Customers.SingleOrDefault(x => x.CustomerID == model.Order.CustomerID);

                db.AddOrderTemporary(customer.CustomerID, id, model.Order.OrderDate, model.Order.RequiredDate,
                    model.Order.ShippedDate, model.Order.ShipVia, customer.CompanyName);
                db.SaveChanges();
            }


            return RedirectToAction("Index", "Home");
        }
    }
}