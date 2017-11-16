using System;

namespace Example
{
    public class Order
    {
        public string Id { get; set; }

        public decimal TotalPrice { get; set; }

        public void Complete()
        {
            CompletedAt = DateTime.Now;
        }

        public DateTime? CompletedAt { get; private set; }

        public bool IsCompleted => CompletedAt.HasValue;
    }
}