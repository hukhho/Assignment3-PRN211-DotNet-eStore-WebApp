
using BusinessObject;
using DataAccess.Repository;
using eStore.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace eStore.Controllers
{
    public class CartController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        IMemberRepository memberRepository = null;
        IProductRepository productRepository = null;
        IOrderRepository orderRepository = null;
        IOrderDetailRepository orderDetailRepository = null;
        private readonly IConfiguration _config;

        public CartController(IWebHostEnvironment webHostEnvironment, IConfiguration config)
        {
            _webHostEnvironment = webHostEnvironment;
            productRepository = new ProductRepository();
            memberRepository = new MemberRepository();
            orderRepository = new OrderRepository();
            orderDetailRepository = new OrderDetailRepository();
            _config = config;
        }
        
        public IActionResult Index()
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") == null || session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Members");
            }

            return View();
        }

        [Route("/addCart/{productId:int}", Name = "addCart")]
        public IActionResult AddToCart([FromRoute] int productId)
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") != "Admin")
            {
                return StatusCode(403);
            }

            var product = productRepository.GetProductByID(productId);
            if (product == null)
            {
                return NotFound();
            }

            var cart = GetCartItems();

            var cartItem = cart.Find(p => p.productId == productId);
            if (cartItem != null)
            {
                cartItem.quantity++;
            }
            else
            {              
                cart.Add(new CartItem()
                {
                    quantity = 1,
                    discount = 0,
                    productId = productId
                });          
            }
        
            SaveCartSession(cart);
            return Redirect("/Products");
        }

        [HttpPost]
        [Route("/updatecart", Name = "updateCart")]
        public IActionResult UpdateCartItem(int discount, int quantity, int productId)
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") != "Admin")
            {
                return StatusCode(403);
            }

            if (discount < 0)
            {
                string messageStr = "Discount not vail!";
                return RedirectToRoute("cart", new { message = messageStr });
            }

            var cart = GetCartItems();
            var cartItem = cart.Find(p => p.productId == productId);
            if (cartItem != null)
            {              
                if (quantity <= 0)
                {
                    RemoveFromCart(cartItem.productId);
                    return RedirectToAction(nameof(Cart));
                }
                else if (quantity > productRepository.GetProductByID(cartItem.productId).UnitsInStock)
                {
                    var name = productRepository.GetProductByID(cartItem.productId).ProductName;
                    string messageStr = "Not enough";
                    return RedirectToRoute("cart", new { message = messageStr });
                }
                cartItem.quantity = quantity;
                cartItem.discount = discount;
            }

            SaveCartSession(cart);
            return RedirectToAction(nameof(Cart));
        }

        [Route("/removeItem/{productId:int}", Name = "removeItem")]
        public IActionResult RemoveFromCart([FromRoute] int productId)
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") != "Admin")
            {
                return StatusCode(403);
            }

            var cart = GetCartItems();
            var cartitem = cart.Find(p => p.productId == productId);
            if (cartitem != null)
            {
                cart.Remove(cartitem);
            }

            SaveCartSession(cart);
            return RedirectToAction(nameof(Cart));
        }


        [Route("/cart", Name = "cart")]
        public IActionResult Cart(string message)
        {          
           var session = HttpContext.Session;
            if (session.GetString("Role") != "Admin")
            {
                return StatusCode(403);
            }

            ViewData["CustomerList"] = new SelectList(memberRepository.GetMembers(), "MemberId", "Email");
            if (message != null)
            {
                ViewData["OutStockMess"] = message;
            }
            return View(GetCartItems());
        }

        void ClearCart()
        {
            var session = HttpContext.Session;
            session.Remove("cart");

        }

        [HttpPost]
        [Route("/addorder", Name = "addorder")]
        public IActionResult AddOrder([FromForm] decimal freight,
            [FromForm] DateTime requiredDate,
            [FromForm] DateTime orderDate,
            [FromForm] DateTime shippedDate,
            [FromForm] int customerId)
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") != "Admin")
            {
                return StatusCode(403);
            }

            try
            {
                Int32 unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                int orderId = (int) unixTimestamp;

                if (orderRepository.GetOrderByID(orderId) != null)
                {
                    return RedirectToAction(nameof(Cart));
                }
                Order order = new Order
                {
                    OrderId = orderId,
                    MemberId = customerId,
                    Freight = freight,
                    OrderDate = orderDate,
                    RequiredDate = requiredDate,
                    ShippedDate = shippedDate
                };

                orderRepository.InsertOrder(order);

                List<CartItem> cart = GetCartItems();
                foreach (CartItem item in cart)
                {
                    Product product = productRepository.GetProductByID(item.productId);
                    if (item.quantity <= 0 || item.quantity > product.UnitsInStock)
                    {
                        orderRepository.DeleteOrder(orderId);
                        RemoveFromCart(item.productId);
                        return RedirectToAction(nameof(Cart));
                    }
                    if (item.discount < 0)
                    {            
                        orderRepository.DeleteOrder(orderId);
                        RemoveFromCart(item.productId);
                        return RedirectToAction(nameof(Cart));
                    }

                   
                    OrderDetail orderDetail = new OrderDetail
                    {
                        OrderId = orderId,
                        ProductId = item.productId,
                        UnitPrice = product.UnitPrice,
                        Discount = item.discount,
                        Quantity = item.quantity
                    };
                    orderDetailRepository.Insert(orderDetail);

                    Product productPruchased = productRepository.GetProductByID(product.ProductId);
                    productPruchased.UnitsInStock -= item.quantity;
                    productRepository.UpdateProduct(productPruchased);
                }

                ClearCart();
                return RedirectToAction("Index", "Orders");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction(nameof(Cart));
            }
        }

       
        List<CartItem> GetCartItems()
        {
            var session = HttpContext.Session;
            string jsonCart = session.GetString("cart");
            if (jsonCart != null && session.GetString("Role") == "Admin")
            {
                var settings = new Newtonsoft.Json.JsonSerializerSettings();
                settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                List<CartItem> listCart = JsonConvert.DeserializeObject<List<CartItem>>(jsonCart);
                foreach (CartItem item in listCart)
                {
                    item.product = productRepository.GetProductByID(item.productId);
                }
                return listCart;
            }

            return new List<CartItem>();
        }

      
        void SaveCartSession(List<CartItem> list)
        {
            var session = HttpContext.Session;

            string jsonCart = JsonConvert.SerializeObject(list);

            if (jsonCart != null && session.GetString("Role") == "Admin")
            {
                session.SetString("cart", jsonCart);
            }
        }


    }
}
