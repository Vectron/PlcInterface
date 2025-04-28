using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace PlcInterface;

/// <summary>
/// Encapsules the logic to create a type by constructor with parameters, or default constructor
/// with property setters.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ObjectActivator"/> class.
/// </remarks>
internal sealed class ObjectActivator(Type type) : ITypeActivator
{
    private readonly Dictionary<int, ParametersAndActivator> constructors = GetConstructors(type);

    private readonly Dictionary<PropertyInfo, PropertySetter> propertySetters = type
            .GetProperties()
            .Where(x => x.CanWrite)
            .ToDictionary(x => x, GetPropertySetter);

    private delegate object Activator(params object[] args);

    private delegate void PropertySetter(object instance, object? value);

    /// <inheritdoc/>
    public bool TryCreateInstance(Func<string, Type, object?> memberValueGetter, int memberCount, [MaybeNullWhen(false)] out object instance)
    {
        for (var i = memberCount; i >= 0; i--)
        {
            if (!constructors.TryGetValue(i, out var data))
            {
                continue;
            }

            var aguments = data.ParameterInfos
                .Where(p => !string.IsNullOrEmpty(p.Name))
                .Select(p => memberValueGetter.Invoke(p.Name!, p.ParameterType))
                .Where(v => v != null)
                .Cast<object>()
                .ToArray();

            if (aguments.Length != i)
            {
                continue;
            }

            instance = data.Activator.Invoke(aguments);
            if (aguments.Length == memberCount)
            {
                return true;
            }

            foreach (var (propertyInfo, setter) in propertySetters)
            {
                var memberValue = memberValueGetter.Invoke(propertyInfo.Name, propertyInfo.PropertyType);

                // Setting properties on value types with the expression tree make hidden copies and thus not setting the values.
                if (type.IsValueType)
                {
                    propertyInfo.SetValue(instance, memberValue);
                    continue;
                }

                setter(instance, memberValue);
            }

            return true;
        }

        instance = null;
        return false;
    }

    private static Activator GetActivator(ConstructorInfo constructorInfo)
    {
        var parameters = constructorInfo.GetParameters();

        // create a single param of type object[]
        var param = Expression.Parameter(typeof(object[]), "args");
        var argsExp = new Expression[parameters.Length];

        // pick each arg from the params array and create a typed expression of them
        for (var i = 0; i < parameters.Length; i++)
        {
            Expression index = Expression.Constant(i);
            var paramType = parameters[i].ParameterType;
            Expression paramAccessorExp = Expression.ArrayIndex(param, index);
            Expression paramCastExp = Expression.Convert(paramAccessorExp, paramType);
            argsExp[i] = paramCastExp;
        }

        // make a NewExpression that calls the ctor with the args we just created
        var newExp = Expression.New(constructorInfo, argsExp);
        var cast = Expression.Convert(newExp, typeof(object));

        // create a lambda with the New Expression as body and our param object[] as arg
        var lambda = Expression.Lambda<Activator>(cast, param);

        // compile it
        var compiled = lambda.Compile();
        return compiled;
    }

    private static Dictionary<int, ParametersAndActivator> GetConstructors(Type type)
    {
        var constructorInfos = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public);

        // Value types might not return a constructor.
        if (constructorInfos.Length == 0)
        {
            object CreateInstance(object[] args) => System.Activator.CreateInstance(type)!;
            return new Dictionary<int, ParametersAndActivator>
            {
                { 0, new ParametersAndActivator([], CreateInstance) },
            };
        }

        return constructorInfos
            .Select(c => new ParametersAndActivator(c.GetParameters(), GetActivator(c)))
            .ToDictionary(p => p.ParameterInfos.Length);
    }

    private static PropertySetter GetPropertySetter(PropertyInfo propertyInfo)
    {
        var instanceParam = Expression.Parameter(typeof(object));
        var instanceParamCast = Expression.Convert(instanceParam, propertyInfo.DeclaringType!);
        var propertyParam = Expression.Parameter(typeof(object));
        var propertyParamCast = Expression.Convert(propertyParam, propertyInfo.PropertyType);
        var propertyGetterExpression = Expression.Property(instanceParamCast, propertyInfo.Name);
        var assignExpression = Expression.Assign(propertyGetterExpression, propertyParamCast);
        var lambda = Expression.Lambda<PropertySetter>(assignExpression, instanceParam, propertyParam);
        var compiled = lambda.Compile();
        return compiled;
    }

    private record ParametersAndActivator(ParameterInfo[] ParameterInfos, Activator Activator);
}
