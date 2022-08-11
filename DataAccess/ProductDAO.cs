using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    class ProductDAO
    {
        private static ProductDAO instance = null;

        private static readonly object instanceLock = new object();

        public static ProductDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new ProductDAO();
                    }
                    return instance;
                }
            }
        }

        public IEnumerable<Product> GetProductList()
        {
            var Products = new List<Product>();
            try
            {
                using var context = new FStoreDBContext();
                Products = context.Products
                                .Include(c => c.Category)
                                .Include(d => d.OrderDetails)
                                .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Products;
        }


        public Product GetProductByID(int ProductID)
        {
            Product Product = null;
            try
            {
                using var context = new FStoreDBContext();
                Product = context.Products
                                       .Include(c => c.Category)
                                       .Include(d => d.OrderDetails)
                                       .SingleOrDefault(m => m.ProductId == ProductID);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Product;
        }


        public void AddNew(Product Product)
        {
            try
            {
                Product _Product = GetProductByID(Product.ProductId);
                if (_Product == null)
                {
                    using var context = new FStoreDBContext();
                    context.Products.Add(Product);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("Product is already exist!!!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Update(Product Product)
        {
            try
            {
                Product _Product = GetProductByID(Product.ProductId);
                if (Product != null)
                {
                    using var context = new FStoreDBContext();
                    context.Products.Update(Product);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("Product does not already exists.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public void Remove(int ProductID)
        {
            try
            {
                Product Product = GetProductByID(ProductID);
                if (Product != null)
                {
                    using var context = new FStoreDBContext();
                    context.Products.Remove(Product);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("Product does not already exists.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


    }

}
