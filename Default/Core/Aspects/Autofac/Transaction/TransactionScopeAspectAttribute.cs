using Castle.DynamicProxy;
using LCW.Core.Utilities.Interceptors.Autofac;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace LCW.Core.Aspects.Autofac.Transaction
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class TransactionScopeAspectAttribute : MethodInterceptionAttribute
    {
        public override void Intercept(IInvocation invocation)
        {
            using TransactionScope transactionScope = new TransactionScope();
            try
            {
                invocation.Proceed();
                transactionScope.Complete();
            }
            catch (System.Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }

        }
    }
}
