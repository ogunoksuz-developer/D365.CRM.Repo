
using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace LCW.Core.Utilities.Interceptors.Autofac
{
    public abstract class MethodInterceptionAttribute : MethodInterceptionBaseAttribute
    {
        protected virtual void OnBefore(IInvocation invocation)
        {

        }

        protected virtual void OnAfter(IInvocation invocation)
        {

        }

        protected virtual void OnException(IInvocation invocation, System.Exception ex)
        {

        }

        protected virtual void OnSuccess(IInvocation invocation)
        {

        }


        public override void Intercept(IInvocation invocation)
        {
            var isSuccess = true;

            OnBefore(invocation);

            try
            {
                invocation.Proceed();
            }
            catch (Exception ex)
            {
                isSuccess = false;
                OnException(invocation, ex);
                throw;
            }
            finally
            {
                if (isSuccess)
                    OnSuccess(invocation);
            }

            OnAfter(invocation);
        }
    }

}
