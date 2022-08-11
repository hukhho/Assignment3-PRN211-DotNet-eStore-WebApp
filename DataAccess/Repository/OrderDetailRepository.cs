using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject;

namespace DataAccess.Repository
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        public IEnumerable<OrderDetail> GetOrderDetails() => OrderDetailDAO.Instance.GetOrderDetailsList();
        public IEnumerable<OrderDetail> GetOrderDetailsByOrderID(int OrderID) => OrderDetailDAO.Instance.GetOrderDetailsByOrderID(OrderID);
        public void Insert(OrderDetail orderDetail) => OrderDetailDAO.Instance.AddNew(orderDetail);
        public void Delete(int orderID, int productId) => OrderDetailDAO.Instance.Remove(orderID, productId);
        public void Update(OrderDetail orderDetail) => OrderDetailDAO.Instance.Update(orderDetail);
    }
}
