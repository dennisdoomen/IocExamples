using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.CommonServiceLocator;
using Autofac.Features.ResolveAnything;
using CommonServiceLocator;

namespace Example
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            IContainer container = SetupContainer();

            var orderProcessing = container.Resolve<OrderProcessing>();

            var order = await orderProcessing.AcceptOrder("myOrder", 1000);

            await orderProcessing.PrioritizeLargeOrders();

            Console.WriteLine($"Status of order {order.Id} is " + (order.IsCompleted ? "completed" : "incomplete"));
        }

        private static IContainer SetupContainer()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
            containerBuilder.RegisterType<PersistedStoreOrders>().AsImplementedInterfaces().SingleInstance();
            containerBuilder.RegisterType<TotalPriceBasedOrderValueStrategy>().AsImplementedInterfaces().SingleInstance();

            IContainer container = containerBuilder.Build();

            ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(container));
            return container;
        }
    }
}