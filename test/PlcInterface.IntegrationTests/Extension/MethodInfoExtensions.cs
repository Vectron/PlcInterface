using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace PlcInterface.IntegrationTests.Extension;

internal static class MethodInfoExtensions
{
    [ExcludeFromCodeCoverage]
    public static Task InvokeAsyncUnwrappedException(this MethodInfo memberInfo, object obj, object[] parameters)
    {
        try
        {
            return memberInfo.Invoke(obj, parameters) is not Task result
                ? Task.CompletedTask
                : result;
        }
        catch (TargetInvocationException ex)
        {
            ExceptionDispatchInfo.Capture(ex.InnerException ?? ex).Throw();
            throw;
        }
    }

    [ExcludeFromCodeCoverage]
    public static object? InvokeUnwrappedException(this MethodInfo memberInfo, object obj, object[] parameters)
    {
        try
        {
            return memberInfo.Invoke(obj, parameters);
        }
        catch (TargetInvocationException ex)
        {
            ExceptionDispatchInfo.Capture(ex.InnerException ?? ex).Throw();
            throw;
        }
    }
}