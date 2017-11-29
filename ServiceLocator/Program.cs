using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Autofac;
using Autofac.Features.ResolveAnything;
using IContainer = Autofac.IContainer;

namespace Example
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            using (IContainer container = SetupContainer())
            {
                var orderProcessing = container.Resolve<OrderProcessing>();

                var order = await orderProcessing.AcceptOrder("myOrder", 1000);

                await orderProcessing.PrioritizeLargeOrders(new TotalPriceBasedOrderValueStrategy());
                await orderProcessing.PrioritizeLargeOrders(new TotalPriceBasedOrderValueStrategy());
                await orderProcessing.PrioritizeLargeOrders(new TotalPriceBasedOrderValueStrategy());

                Console.WriteLine($"Status of order {order.Id} is " + (order.IsCompleted ? "completed" : "incomplete"));
            }
        }

        private static IContainer SetupContainer()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<OrderProcessingModule>();
            containerBuilder.RegisterModule<DateTimeModule>();

            IContainer container = containerBuilder.Build();

            return container;
        }
    }
}