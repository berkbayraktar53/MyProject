using Castle.DynamicProxy;
using Core.Utilities.Results;

namespace Core.Utilities.Interceptors
{
    public abstract class MethodInterception : MethodInterceptionBaseAttribute
    {
        protected virtual void OnBefore(IInvocation invocation) { }

        protected virtual void OnAfter(IInvocation invocation) { }

        protected virtual void OnException(IInvocation invocation, Exception e) { }

        protected virtual void OnSuccess(IInvocation invocation) { }

        public override void Intercept(IInvocation invocation)
        {
            var isSuccess = true;
            OnBefore(invocation);
            try
            {
                if (invocation.ReturnValue != null && (invocation.ReturnValue.GetType() == typeof(ErrorResult) || (invocation.ReturnValue.GetType().IsGenericType && invocation.ReturnValue.GetType().GetGenericTypeDefinition() == typeof(ErrorDataResult<>))))
                {
                    isSuccess = false;
                    return;
                }
                invocation.Proceed();
            }
            catch (System.Exception e)
            {
                isSuccess = false;
                OnException(invocation, e);
                throw;
            }
            finally
            {
                if (isSuccess)
                {
                    OnSuccess(invocation);
                }
            }
            OnAfter(invocation);
        }
    }
}