﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example
{
    public class OrderProcessing
    {
        private readonly IStoreOrders orderStorage;
        private readonly IOrderValueStrategy valueStrategy;

        public OrderProcessing(IStoreOrders orderStorage, IOrderValueStrategy valueStrategy)
        {
            this.orderStorage = orderStorage;
            this.valueStrategy = valueStrategy;
        }

        public async Task PrioritizeLargeOrders()
        {
            IEnumerable<Order> orders = await orderStorage.GetIncompleted();

            foreach (var order in orders.Where(o => valueStrategy.IsHighValuedOrder(o)))
            {
                order.Complete();

                await orderStorage.Store(order);
            }
        }

        public async Task<Order> AcceptOrder(string id, decimal totalPrice)
        {
            var order = new Order
            {
                Id = id,
                TotalPrice = totalPrice
            };

            await orderStorage.Store(order);

            return order;
        }
    }
}