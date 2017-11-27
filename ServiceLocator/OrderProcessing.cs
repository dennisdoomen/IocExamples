using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example
{
    public delegate IStoreOrders GetStorage(StorageLevel level);
    
    public class OrderProcessing
    {
        private readonly GetStorage orderStorages;

        public OrderProcessing(GetStorage orderStorages)
        {
            this.orderStorages = orderStorages;
        }

        public async Task PrioritizeLargeOrders(IOrderValueStrategy valueStrategy)
        {
            IEnumerable<Order> orders = await orderStorages(StorageLevel.Hot).GetIncompleted();

            foreach (var order in orders.Where(valueStrategy.IsHighValuedOrder))
            {
                order.Complete();

                await orderStorages(StorageLevel.Hot).Delete(order.Id);
                await orderStorages(StorageLevel.Cold).Store(order);
            }
        }

        public async Task<Order> AcceptOrder(string id, decimal totalPrice)
        {
            var order = new Order
            {
                Id = id,
                TotalPrice = totalPrice
            };

            await orderStorages(StorageLevel.Hot).Store(order);

            return order;
        }
    }
}