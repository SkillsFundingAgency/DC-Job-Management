using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Builder;

namespace ESFA.DC.Job.WebApi.Extensions
{
    public static class AutoFacExtension
    {
        public static IRegistrationBuilder<IOrderedEnumerable<TService>, SimpleActivatorData, SingleRegistrationStyle> RegisterOrdered<TService>(this ContainerBuilder builder, Action<IOrderedScope> setter)
        {
            var scope = new OrderedScope<TService>(builder);
            setter(scope);
            return builder
                .Register(ctx => ctx.Resolve<IEnumerable<TService>>()
                .Where(x => scope.OrderedTypes.ContainsKey(x.GetType()))
                .OrderBy(x => scope.OrderedTypes[x.GetType()]));
        }
    }

    public class OrderedScope<TService> : IOrderedScope
    {
        private readonly ContainerBuilder _builder;
        private int _order = 1;

        public OrderedScope(ContainerBuilder builder)
        {
            _builder = builder;
            OrderedTypes = new Dictionary<Type, int>();
        }

        public Dictionary<Type, int> OrderedTypes { get; }

        public IRegistrationBuilder<T, ConcreteReflectionActivatorData, SingleRegistrationStyle> Register<T>()
        {
            OrderedTypes.Add(typeof(T), _order++);
            return _builder.RegisterType<T>().As<TService>();
        }
    }
}