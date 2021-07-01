using Autofac.Builder;

namespace ESFA.DC.Job.WebApi.Extensions
{
    public interface IOrderedScope
    {
        IRegistrationBuilder<T, ConcreteReflectionActivatorData, SingleRegistrationStyle> Register<T>();
    }
}