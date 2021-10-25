using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace PlcInterface.Tests.Extension;

internal static class MethodInfoExtensions
{
    [ExcludeFromCodeCoverage]
    public static object InvokeUnwrappedException(this MethodInfo memberInfo, object obj, object[] parameters)
    {
        try
        {
            return memberInfo.Invoke(obj, parameters);
        }
        catch (TargetInvocationException ex)
        {
            ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            throw;
        }
    }
}