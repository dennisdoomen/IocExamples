using Autofac;
using Autofac.Features.ResolveAnything;

namespace Example
{
    public class OrderProcessingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());

            builder.RegisterType<PersistedStoreOrders>().Keyed<IStoreOrders>(StorageLevel.Cold).SingleInstance();
            builder.RegisterType<PersistedStoreOrders>().Keyed<IStoreOrders>(StorageLevel.Hot).SingleInstance();
            
            builder.Register<GetStorage>(ctx =>
            {
                var cc = ctx.Resolve<IComponentContext>();
                
                return level => cc.ResolveKeyed<IStoreOrders>(level);
            });
        }
    }
}