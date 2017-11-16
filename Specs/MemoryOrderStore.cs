using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example.Specs
{
    public class MemoryOrderStore : IStoreOrders
    {
        private readonly List<Order> orders = new List<Order>();

        public Task<IEnumerable<Order>> GetIncompleted()
        {
            return Task.FromResult(orders.Where(o => !o.CompletedAt.HasValue).ToArray().AsEnumerable());
        }

        public Task Store(Order order)
        {
            orders.Add(order);

            return Task.FromResult(0);
        }

        public Task Deleteorder(string orderId)
        {
            orders.RemoveAll(o => o.Id == orderId);

            return Task.FromResult(0);
        }
    }
}