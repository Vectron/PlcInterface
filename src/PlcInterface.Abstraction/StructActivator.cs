using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace PlcInterface;

/// <summary>
/// Encapsuls the logic to create a type by constructor with parameters, or default constructor with
/// property setters.
/// </summary>
internal sealed class StructActivator : ITypeActivator
{
    private readonly Activator activator;
    private readonly PropertyInfo[] properties;

    /// <summary>
    /// Initializes a new instance of the <see cref="StructActivator"/> class.
    /// </summary>
    /// <param name="type">The type of the value type.</param>
    public StructActivator(Type type)
    {
        activator = GetActivator(type);
        properties = type
            .GetProperties()
            .Where(x => x.CanWrite)
            .ToArray();
    }

    private delegate object Activator();

    /// <summary>
    /// Try to create a instance.
    /// </summary>
    /// <param name="memberValueGetter">
    /// A <see cref="Func{T, TResult}"/> for getting the value of the member with the given name.
    /// </param>
    /// <param name="memberCount">The number of members.</param>
    /// <param name="instance">The created instance.</param>
    /// <returns><see langword="true"/> if creation was succesfull, otherwise false.</returns>
    /// <exception cref="SymbolException">is thrown when the data is invallid.</exception>
    public bool TryCreateInstance(Func<string, Type, object?> memberValueGetter, int memberCount, [MaybeNullWhen(false)] out object instance)
    {
        instance = default;
        if (properties.Length >= memberCount)
        {
            instance = activator.Invoke();
            foreach (var property in properties)
            {
                var memberValue = memberValueGetter.Invoke(property.Name, property.PropertyType)
                    ?? throw new SymbolException($"Member: {property.Name} was null");
                property.SetValue(instance, memberValue);
            }

            return true;
        }

        return false;
    }

    private static Activator GetActivator(Type type)
    {
        // make a NewExpression that calls the ctor
        var newExp = Expression.New(type);
        var cast = Expression.Convert(newExp, typeof(object));

        // create a lambda with the New Expression as body
        var lambda = Expression.Lambda<Activator>(cast);

        // compile it
        var compiled = lambda.Compile();
        return compiled;
    }
}