using System;
using System.Collections.Generic;
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

            var map = new Dictionary<StorageLevel, IStoreOrders>
            {
                { StorageLevel.Cold, store },
                { StorageLevel.Hot, store}
            };
            
            // Act
            DateTime now = 29.November(2017).At(14, 33);
            
            var processing = new OrderProcessing(level => map[level], () => now);
            await processing.PrioritizeLargeOrders(new TotalPriceBasedOrderValueStrategy());

            // Assert
            cheapOrder.CompletedAt.Should().BeNull();
            largeOrder.CompletedAt.Should().Be(now);
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
            var processing = new OrderProcessing(level => map[level], () => DateTime.Now);
            await processing.PrioritizeLargeOrders(new TotalPriceBasedOrderValueStrategy());

            // Assert
            hotStorage.Contains(theOrder.Id).Should().BeFalse();
            coldStorage.Contains(theOrder.Id).Should().BeTrue();
        }
    }
}