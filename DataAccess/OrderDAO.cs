using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository
{
    class OrderDAO
    {
        private static OrderDAO instance = null;

        private static readonly object instanceLock = new object();

        public static OrderDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new OrderDAO();
                    }
                    return instance;
                }
            }
        }

        public IEnumerable<Order> GetOrderList()
        {
            var Orders = new List<Order>();
            try
            {
                using var context = new FStoreDBContext();
                Orders = context.Orders
                    .Include(c => c.Member)
                    .Include(c => c.OrderDetails)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Orders;
        }


        public Order GetOrderByID(int OrderID)
        {
            Order Order = null;
            try
            {
                using var context = new FStoreDBContext();
                Order = context.Orders
                    .Include(c => c.Member)
                    .Include(c => c.OrderDetails)
                    .SingleOrDefault(m => m.OrderId == OrderID);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Order;
        }


        public void AddNew(Order Order)
        {
            try
            {
                Order _Order = GetOrderByID(Order.OrderId);
                if (_Order == null)
                {
                    using var context = new FStoreDBContext();
                    context.Orders.Add(Order);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("Order is already exist!!!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Update(Order Order)
        {
            try
            {
                Order _Order = GetOrderByID(Order.OrderId);
                if (Order != null)
                {
                    using var context = new FStoreDBContext();
                    context.Orders.Update(Order);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("Order does not already exists.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public void Remove(int OrderID)
        {
            try
            {
                Order Order = GetOrderByID(OrderID);
                if (Order != null)
                {
                    using var context = new FStoreDBContext();
                    context.Orders.Remove(Order);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("Order does not already exists.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }

}
