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

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
            containerBuilder.RegisterInstance(store).Keyed<IStoreOrders>(StorageLevel.Cold);
            containerBuilder.RegisterInstance(store).Keyed<IStoreOrders>(StorageLevel.Hot);
            var container = containerBuilder.Build();

            // Act
            var processing = container.Resolve<OrderProcessing>();
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

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
            containerBuilder.RegisterInstance(coldStorage).Keyed<IStoreOrders>(StorageLevel.Cold);
            containerBuilder.RegisterInstance(hotStorage).Keyed<IStoreOrders>(StorageLevel.Hot);
            var container = containerBuilder.Build();

            // Act
            var processing = container.Resolve<OrderProcessing>();
            await processing.PrioritizeLargeOrders(new TotalPriceBasedOrderValueStrategy());

            // Assert
            hotStorage.Contains(theOrder.Id).Should().BeFalse();
            coldStorage.Contains(theOrder.Id).Should().BeTrue();
        }
    }
}
