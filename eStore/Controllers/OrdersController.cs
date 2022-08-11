using BusinessObject;
using DataAccess.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eStore.Controllers
{
    public class OrdersController : Controller
    {
        IOrderRepository orderRepository = null;
        IOrderDetailRepository orderDetailRepository = null;
        IProductRepository productRepository = null;
        IMemberRepository memberRepository = null;
        public OrdersController()
        {
            orderRepository = new OrderRepository();
            orderDetailRepository = new OrderDetailRepository();
            memberRepository = new MemberRepository();
            productRepository = new ProductRepository();
        }



        // GET: OrdersController
        public ActionResult Index()
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") == "Admin")
            {
                var orderList = orderRepository.GetOrders();
                return View(orderList);
            }
            else if (session.GetString("Role") == "Member") {
                var memberId = session.GetInt32("Id");
           
                var orderList = orderRepository.GetOrders().Where(o => o.MemberId == memberId).ToList();                  
                return View(orderList);               
            } else{
                return StatusCode(403);
            }
        }

        // GET: OrdersController/Details/5
        public ActionResult Details(int id)
        {
            IEnumerable<OrderDetail> orderDetails;
            var order = orderRepository.GetOrderByID(id);
            if (order == null)
            {
                return NotFound();
            }

            var session = HttpContext.Session;
            if (session.GetString("Role") != "Admin" && session.GetInt32("Id") != order.MemberId)
            {
                return StatusCode(403);
            }
     
            orderDetails = orderDetailRepository.GetOrderDetailsByOrderID(id);
            
            ViewData["OrderDetailList"] = orderDetails;

            return View(order);
        }

        // GET: OrdersController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OrdersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") != "Admin")
            {
                return StatusCode(403);
            }

            var query = memberRepository.GetMembers().Select(c => new { c.MemberId, c.Email });

            ViewBag.MemberId = new SelectList(query.AsEnumerable(), "MemberId", "Email", 1);

            return View();
        }

        // GET: OrdersController/Edit/5
        public ActionResult Edit(int id)
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") != "Admin")
            {
                return StatusCode(403);
            }
            Order order = orderRepository.GetOrderByID(id);
            if (order == null)
            {
                return NotFound();
            }
            var query = memberRepository.GetMembers().Select(c => new { c.MemberId, c.Email });

            ViewBag.MemberId = new SelectList(query.AsEnumerable(), "MemberId", "Email", order.MemberId);

            return View(order);
        }

        // POST: OrdersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Order order)
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") != "Admin")
            {
                return StatusCode(403);
            }
            Member member = memberRepository.GetMemberByID((int) order.MemberId);         
            if (member == null) {
                ViewBag.Message = "Not found user";
                return View(order);
            }
            

            if (order.OrderDate == null || order.RequiredDate == null || order.ShippedDate == null) 
            {
                ViewBag.Message = "Date cant be null!";
                return View(order);
            }

            else if (order.OrderId <= 0)
            {
                ViewBag.Message = "Order id must vaild";
                return View(order);
            }
            else if (order.Freight < 0)
            {
                ViewBag.Message = "Freight must >= 0 ";
                return View(order);
            }

            try
            {         
                if (ModelState.IsValid)
                {
                    orderRepository.UpdateOrder(order);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }

        // GET: OrdersController/Delete/5
        public ActionResult Delete(int? id)
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") != "Admin")
            {
                return StatusCode(403);
            }

            var order = orderRepository.GetOrderByID(id.Value);

            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: OrdersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") != "Admin")
            {
                return StatusCode(403);
            }

            try
            {
                orderRepository.DeleteOrder(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }
    }
}
