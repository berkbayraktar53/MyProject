using Core.Extensions;
using Business.Constants;
using Core.Utilities.IoC;
using Castle.DynamicProxy;
using Core.Utilities.Results;
using Microsoft.AspNetCore.Http;
using Core.Utilities.Interceptors;
using Microsoft.Extensions.DependencyInjection;

namespace Business.BusinessAspects.Autofac
{
    public class SecuredOperation(string roles) : MethodInterception
    {
        private readonly string[] _roles = roles.Split(',');
        private readonly IHttpContextAccessor? _httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();

        protected override void OnBefore(IInvocation invocation)
        {
            var roleClaims = _httpContextAccessor?.HttpContext.User.ClaimRoles();
            if (roleClaims?.Count > 0)
            {
                foreach (var role in _roles)
                {
                    if (roleClaims.Contains(role))
                    {
                        if (invocation.Method.Name.StartsWith("Get"))
                        {
                            var returnType = invocation.Method.ReturnType;
                            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(IDataResult<>))
                            {
                                var genericArgument = returnType.GetGenericArguments()[0];
                                if (genericArgument.IsGenericType && genericArgument.GetGenericTypeDefinition() == typeof(List<>))
                                {
                                    var listType = genericArgument;
                                    var emptyList = Activator.CreateInstance(listType);
                                    var successDataResultType = typeof(SuccessDataResult<>).MakeGenericType(listType);
                                    invocation.ReturnValue = Activator.CreateInstance(successDataResultType, emptyList);
                                }
                                else
                                {
                                    var successDataResultType = typeof(SuccessDataResult<>).MakeGenericType(genericArgument);
                                    invocation.ReturnValue = Activator.CreateInstance(successDataResultType, null);
                                }
                            }
                        }
                        else
                        {
                            invocation.ReturnValue = new SuccessResult();
                        }
                    }
                }
            }
            else
            {
                if (invocation.Method.Name.StartsWith("Get"))
                {
                    var returnType = invocation.Method.ReturnType;
                    if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(IDataResult<>))
                    {
                        var genericArgument = returnType.GetGenericArguments()[0];
                        if (genericArgument.IsGenericType && genericArgument.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            var listType = genericArgument;
                            var emptyList = Activator.CreateInstance(listType);
                            var errorDataResultType = typeof(ErrorDataResult<>).MakeGenericType(listType);
                            invocation.ReturnValue = Activator.CreateInstance(errorDataResultType, emptyList, Messages.AuthorizationDenied);
                        }
                        else
                        {
                            var errorDataResultType = typeof(ErrorDataResult<>).MakeGenericType(genericArgument);
                            invocation.ReturnValue = Activator.CreateInstance(errorDataResultType, Messages.AuthorizationDenied);
                        }
                    }
                }
                else
                {
                    invocation.ReturnValue = new ErrorResult(Messages.AuthorizationDenied);
                }
            }
        }
    }
}