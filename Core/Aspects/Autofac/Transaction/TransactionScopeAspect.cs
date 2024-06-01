using Castle.DynamicProxy;
using System.Transactions;
using Core.Utilities.Results;
using Core.Utilities.Messages;
using Core.Utilities.Interceptors;

namespace Core.Aspects.Autofac.Transaction
{
    public class TransactionScopeAspect : MethodInterception
    {
        public override void Intercept(IInvocation invocation)
        {
            TransactionScope transactionScope = new();
            try
            {
                invocation.Proceed();
                transactionScope.Complete();
            }
            catch (System.Exception)
            {
                transactionScope.Dispose();
                string errorMessage = AspectMessages.TransactionError;
                invocation.ReturnValue = new ErrorResult(errorMessage);
                return;
            }
        }
    }
}