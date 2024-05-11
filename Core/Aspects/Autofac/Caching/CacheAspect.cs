using Core.Utilities.IoC;
using Castle.DynamicProxy;
using Core.Utilities.Interceptors;
using Core.CrossCuttingConcerns.Caching;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Aspects.Autofac.Caching
{
    public class CacheAspect(int duration = 60) : MethodInterception
    {
        private readonly int _duration = duration;
        private readonly ICacheManager _cacheManager = ServiceTool.ServiceProvider.GetService<ICacheManager>();

        public override void Intercept(IInvocation invocation)
        {
            var methodName = string.Format($"{invocation.Method.ReflectedType.FullName}.{invocation.Method.Name}");
            var arguments = invocation.Arguments.ToList();
            var key = $"{methodName}({string.Join(",", arguments.Select(x => x?.ToString() ?? "<Null>"))})";
            if (_cacheManager.IsAdd(key))
            {
                invocation.ReturnValue = _cacheManager.Get(key);
                return;
            }
            invocation.Proceed();
            _cacheManager.Add(key, invocation.ReturnValue, _duration);
        }
    }
}