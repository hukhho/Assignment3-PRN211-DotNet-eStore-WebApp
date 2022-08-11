using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessObject;
using DataAccess;
using DataAccess.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eStore.Controllers
{
    public class ProductController : Controller
    {
        IProductRepository productRepository = null;
        ICategoryRepository categoryRepository = null;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IWebHostEnvironment webHostEnvironment)
        {
            productRepository = new ProductRepository();
            categoryRepository = new CategoryRepository();
            _webHostEnvironment = webHostEnvironment;
        }
        //public ProductController() => productRepository = new ProductRepository();

        // GET: ProductController
        [Route("/products", Name = "products")]
        public ActionResult Index(string searchString, int priceFilter)
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") != "Admin")
            {
                return StatusCode(403);
            }

            var productList = productRepository.GetProducts();

            if (!String.IsNullOrEmpty(searchString))
            {
                productList = productList.Where(p => p.ProductName.ToLower().Contains(searchString.ToLower().Trim())).ToList();
            }

            if (priceFilter > 0)
            {
                if (priceFilter == 1)
                {
                    productList = productList.Where(p => p.UnitPrice >= 0 && p.UnitPrice <= 50).ToList();
                } else if (priceFilter == 2)
                {
                    productList = productList.Where(p => p.UnitPrice > 50 && p.UnitPrice <= 100).ToList();
                }
                else if (priceFilter == 3)
                {
                    productList = productList.Where(p => p.UnitPrice > 100 && p.UnitPrice <= 150).ToList();
                }
                else if (priceFilter == 4)
                {
                    productList = productList.Where(p => p.UnitPrice > 150).ToList();
                }
            }

            return View(productList);
        }

        // GET: ProductController/Details/5
        public ActionResult Details(int id)
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") != "Admin")
            {
                return StatusCode(403);
            }

            Product product = productRepository.GetProductByID(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // GET: ProductController/Create
        public ActionResult Create()
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") != "Admin")
            {
                return StatusCode(403);
            }

            var query = categoryRepository.GetCategorys().Select(c => new { c.CategoryId, c.CategoryName });

            ViewBag.CategoryId = new SelectList(query.AsEnumerable(), "CategoryId", "CategoryName", 1);
            
            return View();
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product)
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") != "Admin")
            {
                return StatusCode(403);
            }

            if (product.ProductName == null || product.Weight == null)
            {
                ViewBag.Message = "Product name or weight can't be emty!";
                return View(product);
            }
            else if (product.UnitPrice <= 0)
            {
                ViewBag.Message = "Price must >= 0";
                return View(product);
            }
            else if (product.UnitsInStock < 0)
            {
                ViewBag.Message = "Unit stock must >= 0 ";
                return View(product);
            }

            try
            {
                if (ModelState.IsValid)
                {
                    productRepository.InsertProduct(product);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(product);
            }
        }

        // GET: ProductController/Edit/5
        public ActionResult Edit(int id)
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") != "Admin")
            {
                return StatusCode(403);
            }

            var product = productRepository.GetProductByID(id);

            var query = categoryRepository.GetCategorys().Select(c => new { c.CategoryId, c.CategoryName });

            ViewBag.CategoryId = new SelectList(query.AsEnumerable(), "CategoryId", "CategoryName", product.CategoryId);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: ProductController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Product product)
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") != "Admin")
            {
                return StatusCode(403);
            }

            if (product.ProductName == null || product.Weight == null)
            {
                ViewBag.Message = "Product name or weight can't be emty!";
                return View(product);
            }
            else if (product.UnitPrice <= 0)
            {
                ViewBag.Message = "Price must >= 0";
                return View(product);
            }
            else if (product.UnitsInStock < 0)
            {
                ViewBag.Message = "Unit stock must >= 0 ";
                return View(product);
            }

            try
            {
                if (id != product.ProductId)
                {
                    return NotFound();
                }
                if (ModelState.IsValid)
                {
                    productRepository.UpdateProduct(product);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }

        // GET: ProductController/Delete/5
        public ActionResult Delete(int? id)
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") != "Admin")
            {
                return StatusCode(403);
            }

            var product = productRepository.GetProductByID(id.Value);

            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: ProductController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") != "Admin")
            {
                return StatusCode(403);
            }

            try
            {
                productRepository.DeleteProduct(id);
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
