using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Features.Indexed;

namespace Example
{
    public class OrderProcessing
    {
        private readonly IIndex<StorageLevel, IStoreOrders> orderStorages;

        public OrderProcessing(IIndex<StorageLevel, IStoreOrders> orderStorages)
        {
            this.orderStorages = orderStorages;
        }

        public async Task PrioritizeLargeOrders(IOrderValueStrategy valueStrategy)
        {
            IEnumerable<Order> orders = await orderStorages[StorageLevel.Hot].GetIncompleted();

            foreach (var order in orders.Where(o => valueStrategy.IsHighValuedOrder(o)))
            {
                order.Complete();

                await orderStorages[StorageLevel.Hot].Delete(order.Id);
                await orderStorages[StorageLevel.Cold].Store(order);
            }
        }

        public async Task<Order> AcceptOrder(string id, decimal totalPrice)
        {
            var order = new Order
            {
                Id = id,
                TotalPrice = totalPrice
            };

            await orderStorages[StorageLevel.Hot].Store(order);

            return order;
        }
    }
}