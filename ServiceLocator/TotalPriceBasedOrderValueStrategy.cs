namespace Example
{
    public class TotalPriceBasedOrderValueStrategy : IOrderValueStrategy
    {
        public bool IsHighValuedOrder(Order order)
        {
            return order.TotalPrice > 1000;
        }
    }
}