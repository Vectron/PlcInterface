using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace PlcInterface;

/// <summary>
/// Encapsules the logic to create a type by constructor with parameters, or default constructor
/// with property setters.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="StructActivator"/> class.
/// </remarks>
/// <param name="type">The type of the value type.</param>
internal sealed class StructActivator(Type type) : ITypeActivator
{
    private readonly Activator activator = GetActivator(type);

    private readonly PropertyInfo[] properties = type
            .GetProperties()
            .Where(x => x.CanWrite)
            .ToArray();

    private delegate object Activator();

    /// <summary>
    /// Try to create a instance.
    /// </summary>
    /// <param name="memberValueGetter">
    /// A <see cref="Func{T, TResult}"/> for getting the value of the member with the given name.
    /// </param>
    /// <param name="memberCount">The number of members.</param>
    /// <param name="instance">The created instance.</param>
    /// <returns><see langword="true"/> if creation was successful, otherwise false.</returns>
    /// <exception cref="SymbolException">is thrown when the data is invalid.</exception>
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
