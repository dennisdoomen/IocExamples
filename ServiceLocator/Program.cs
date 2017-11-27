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
            IContainer container = SetupContainer();

            var orderProcessing = container.Resolve<OrderProcessing>();

            var order = await orderProcessing.AcceptOrder("myOrder", 1000);

            await orderProcessing.PrioritizeLargeOrders(new TotalPriceBasedOrderValueStrategy());

            Console.WriteLine($"Status of order {order.Id} is " + (order.IsCompleted ? "completed" : "incomplete"));
        }

        private static IContainer SetupContainer()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());

            containerBuilder.RegisterType<PersistedStoreOrders>().Keyed<IStoreOrders>(StorageLevel.Cold);
            containerBuilder.RegisterType<PersistedStoreOrders>().Keyed<IStoreOrders>(StorageLevel.Hot);
            
            containerBuilder.Register<GetStorage>(ctx =>
            {
                var cc = ctx.Resolve<IComponentContext>();
                
                return level => cc.ResolveKeyed<IStoreOrders>(level);
            });

            IContainer container = containerBuilder.Build();

            return container;
        }
    }
}