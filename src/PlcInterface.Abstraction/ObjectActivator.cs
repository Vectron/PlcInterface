using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace PlcInterface;

/// <summary>
/// Encapsules the logic to create a type by constructor with parameters, or default constructor
/// with property setters.
/// </summary>
internal sealed class ObjectActivator : ITypeActivator
{
    private readonly Activator activator;
    private readonly ParameterInfo[] parameters;
    private readonly PropertySetterHelper[] properties;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectActivator"/> class.
    /// </summary>
    /// <param name="constructorInfo">The <see cref="ConstructorInfo"/> used to create the type.</param>
    public ObjectActivator(ConstructorInfo constructorInfo)
    {
        if (constructorInfo.DeclaringType == null)
        {
            throw new ArgumentException("No declaring type defined.", nameof(constructorInfo));
        }

        parameters = constructorInfo.GetParameters();
        activator = GetActivator(constructorInfo);
        var query = constructorInfo.DeclaringType
            .GetProperties()
            .Where(x => x.CanWrite)
            .Select(x => new PropertySetterHelper(x));
        properties = [.. query];
    }

    private delegate object Activator(params object[] args);

    /// <inheritdoc/>
    public bool TryCreateInstance(Func<string, Type, object?> memberValueGetter, int memberCount, [MaybeNullWhen(false)] out object instance)
    {
        instance = default;
        if (parameters.Length == memberCount)
        {
            var arguments = new object[parameters.Length];
            var index = 0;
            foreach (var parameter in parameters)
            {
                if (string.IsNullOrEmpty(parameter.Name))
                {
                    return false;
                }

                arguments[index++] = memberValueGetter.Invoke(parameter.Name, parameter.ParameterType)
                    ?? throw new SymbolException($"Member: {parameter.Name} was null");
            }

            var argumentsArray = arguments.ToArray();
            instance = activator.Invoke(argumentsArray);
            return true;
        }

        if (properties.Length >= memberCount)
        {
            instance = activator.Invoke();
            foreach (var property in properties)
            {
                var memberValue = memberValueGetter.Invoke(property.Name, property.PropertyType)
                    ?? throw new SymbolException($"Member: {property.Name} was null");
                property.Set(instance, memberValue);
            }

            return true;
        }

        return false;
    }

    private Activator GetActivator(ConstructorInfo constructorInfo)
    {
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
}
