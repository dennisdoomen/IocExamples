using System.Threading.Tasks;

using FluentAssertions;
using Xunit;

namespace Example.Specs
{
    public class OrderProcessingSpecs
    {
        [Fact]
        public async Task When_only_high_valued_orders_must_be_processed_it_should_ignore_others()
        {
            // Arrange
            var store = new MemoryOrderStore();

            var cheapOrder = new Order
            {
                Id = "Small Order",
                TotalPrice = 999
            };

            var largeOrder = new Order
            {
                Id = "Large Order",
                TotalPrice = 1001
            };

            await store.Store(cheapOrder);
            await store.Store(largeOrder);
            
            // Act
            var processing = new OrderProcessing(store);
            await processing.PrioritizeLargeOrders(new TotalPriceBasedOrderValueStrategy());

            // Assert
            cheapOrder.IsCompleted.Should().BeFalse();
            largeOrder.IsCompleted.Should().BeTrue();
        }
    }
}
