using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Autofac.Features.ResolveAnything;
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


            var map = new Dictionary<StorageLevel, IStoreOrders>
            {
                { StorageLevel.Cold, store },
                { StorageLevel.Hot, store}
            };
            
            // Act
            var processing = new OrderProcessing(level => map[level]);
            await processing.PrioritizeLargeOrders(new TotalPriceBasedOrderValueStrategy());

            // Assert
            cheapOrder.IsCompleted.Should().BeFalse();
            largeOrder.IsCompleted.Should().BeTrue();
        }

        [Fact]
        public async Task When_an_order_is_processed_it_should_move_to_cold_storage()
        {
            // Arrange
            var coldStorage = new MemoryOrderStore();
            var hotStorage = new MemoryOrderStore();

            var theOrder = new Order
            {
                Id = "Large Order",
                TotalPrice = 1001
            };

            await hotStorage.Store(theOrder);

            var map = new Dictionary<StorageLevel, IStoreOrders>
            {
                { StorageLevel.Cold, coldStorage },
                { StorageLevel.Hot, hotStorage }
            };

            // Act
            var processing = new OrderProcessing(level => map[level]);
            await processing.PrioritizeLargeOrders(new TotalPriceBasedOrderValueStrategy());

            // Assert
            hotStorage.Contains(theOrder.Id).Should().BeFalse();
            coldStorage.Contains(theOrder.Id).Should().BeTrue();
        }
    }
}
