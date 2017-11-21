using System.Threading.Tasks;
using Autofac;
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

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<TotalPriceBasedOrderValueStrategy>().AsImplementedInterfaces();
            containerBuilder.RegisterInstance(store).AsImplementedInterfaces();
            var container = containerBuilder.Build();
            
            // Act
            var processing = new OrderProcessing(container);
            await processing.PrioritizeLargeOrders();

            // Assert
            cheapOrder.IsCompleted.Should().BeFalse();
            largeOrder.IsCompleted.Should().BeTrue();
        }
    }
}
