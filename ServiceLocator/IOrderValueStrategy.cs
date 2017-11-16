namespace Example
{
    public interface IOrderValueStrategy
    {
        bool IsHighValuedOrder(Order order);
    }
}