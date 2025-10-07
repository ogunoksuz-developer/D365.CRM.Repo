using LCW.Core.CrossCuttingConcerns.Caching;
using LCW.Core.Utilities.Interceptors.Autofac;
using Microsoft.Extensions.DependencyInjection;
using LCW.Core.Utilities.IoC;
using System;
using System.Collections.Generic;
using System.Text;
using Castle.DynamicProxy;

namespace LCW.Core.Aspects.Autofac.Caching
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class CacheRemoveAspectAttribute : MethodInterceptionAttribute
    {
        private readonly string _pattern;
        private readonly ICacheManager _cacheManager;

        public CacheRemoveAspectAttribute(string pattern)
        {
            _pattern = pattern;
            _cacheManager = ServiceTool.ServiceProvider.GetService<ICacheManager>();
        }

        protected override void OnSuccess(IInvocation invocation)
        {
            _cacheManager.RemoveByPattern(_pattern);
        }
    }
}
