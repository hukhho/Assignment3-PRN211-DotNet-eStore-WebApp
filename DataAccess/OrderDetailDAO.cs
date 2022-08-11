using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    class OrderDetailDAO
    {
        private static OrderDetailDAO instance = null;

        private static readonly object instanceLock = new object();

        public static OrderDetailDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new OrderDetailDAO();
                    }
                    return instance;
                }
            }
        }

        public IEnumerable<OrderDetail> GetOrderDetailsList()
        {
            var orderdetails = new List<OrderDetail>();
            try
            {
                using var context = new FStoreDBContext();
                orderdetails = context.OrderDetails
                    .Include(c => c.Product)
                    .Include(d => d.Order)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return orderdetails;
        }


        public IEnumerable<OrderDetail> GetOrderDetailsByOrderID(int orderID)
        {
            var orderdetails = new List<OrderDetail>();
            try
            {
                using var context = new FStoreDBContext();
                orderdetails = (List<OrderDetail>) context.OrderDetails
                    .Include(c => c.Product)
                    .Include(d => d.Order)
                    .Where(t => t.OrderId == orderID).ToList();
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return orderdetails;
        }

        public OrderDetail GetOrderDetailsByOrderIDAndProductID(int orderID, int productID)
        {
            OrderDetail orderdetail = new OrderDetail();
            try
            {
                using var context = new FStoreDBContext();
                orderdetail = context.OrderDetails
                    .Include(c => c.Product)
                    .Include(d => d.Order)
                    .FirstOrDefault(t => t.OrderId == orderID && t.ProductId == productID);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return orderdetail;
        }

        public void AddNew(OrderDetail orderDetail)
        {
            try
            {
                OrderDetail _orderDetail = GetOrderDetailsByOrderIDAndProductID(orderDetail.OrderId, orderDetail.ProductId);
                if (_orderDetail == null)
                {
                    using var context = new FStoreDBContext();
                    context.OrderDetails.Add(orderDetail);
                    context.SaveChanges();
                }
                else
                {
                    using var context = new FStoreDBContext();
                    _orderDetail.Quantity += orderDetail.Quantity;
                    context.OrderDetails.Update(_orderDetail);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Update(OrderDetail orderDetail)
        {
            try
            {       
                using var context = new FStoreDBContext();
                context.OrderDetails.Update(orderDetail);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public void Remove(int orderId, int productId)
        {
            try
            {
                OrderDetail _orderDetail = GetOrderDetailsByOrderIDAndProductID(orderId, productId);
                if (_orderDetail != null)
                {
                    using var context = new FStoreDBContext();
                    context.OrderDetails.Remove(_orderDetail);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("Order detail does not already exists.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }



}
