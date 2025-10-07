using Castle.DynamicProxy;
using System;
using System.Linq;
using System.Reflection;

namespace LCW.Core.Utilities.Interceptors.Autofac
{
    public class AspectInterceptorSelector : IInterceptorSelector
    {
        public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
        {
            var classAttribute = type.GetCustomAttributes<MethodInterceptionBaseAttribute>(true).ToList();

            var methodAttributes = type.GetMethod(method.Name).GetCustomAttributes<MethodInterceptionBaseAttribute>(true);

            classAttribute.AddRange(methodAttributes);

            return classAttribute.OrderBy(x => x.Priority).ToArray();
        }
    }
}
