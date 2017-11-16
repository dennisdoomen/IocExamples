using System.Collections.Generic;
using System.Threading.Tasks;

namespace Example
{
    public interface IStoreOrders
    {
        Task<IEnumerable<Order>> GetIncompleted();
        Task Store(Order order);
        Task Deleteorder(string orderId);
    }
}