using System.Reflection;
using System.Reflection.Emit;

namespace PlcInterface.Abstraction.Tests;

public static class MyTypeBuilder
{
    public static Type CompileResultType()
    {
        var tb = GetTypeBuilder();
        var constructor = tb.DefineConstructor(
            MethodAttributes.Public
            | MethodAttributes.SpecialName
            | MethodAttributes.RTSpecialName,
            CallingConventions.Standard,
            [typeof(int)]);

        // generate the code to call the parent's default constructor
        var il = constructor.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ret);

        return tb.CreateType()!;
    }

    private static TypeBuilder GetTypeBuilder()
    {
        var typeSignature = "MyDynamicType";
        var an = new AssemblyName(typeSignature);
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
        var tb = moduleBuilder.DefineType(
            typeSignature,
            TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.AutoLayout,
            parent: null);
        return tb;
    }
}
