using System;

namespace Example
{

    public delegate DateTime GetNow();
    
    public class Order
    {
        public string Id { get; set; }

        public decimal TotalPrice { get; set; }

        public void Complete(GetNow getNow)
        {
            CompletedAt = getNow();
        }

        public DateTime? CompletedAt { get; private set; }

        public bool IsCompleted => CompletedAt.HasValue;
    }
}