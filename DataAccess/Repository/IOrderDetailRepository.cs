using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject;

namespace DataAccess.Repository
{
    public interface IOrderDetailRepository
    {
        public IEnumerable<OrderDetail> GetOrderDetails();
        public IEnumerable<OrderDetail> GetOrderDetailsByOrderID(int OrderID);
        public void Insert(OrderDetail orderDetail);
        public void Delete(int orderID, int productId);
        public void Update(OrderDetail orderDetail);
    }
}
