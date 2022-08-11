using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject;

namespace DataAccess.Repository
{
    public interface IProductRepository
    {
        public IEnumerable<Product> GetProducts() => ProductDAO.Instance.GetProductList();
        public Product GetProductByID(int ProductID) => ProductDAO.Instance.GetProductByID(ProductID);
        public void InsertProduct(Product Product) => ProductDAO.Instance.AddNew(Product);
        public void DeleteProduct(int ProductID) => ProductDAO.Instance.Remove(ProductID);
        public void UpdateProduct(Product Product) => ProductDAO.Instance.Update(Product);
    }
}
