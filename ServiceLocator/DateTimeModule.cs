using System;
using Autofac;

namespace Example
{
    public class DateTimeModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register<GetNow>(ctx => () => DateTime.Now);
        }
    }
}