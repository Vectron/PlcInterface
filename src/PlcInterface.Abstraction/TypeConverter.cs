using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Globalization;
using System.Linq;

namespace PlcInterface;

/// <summary>
/// A base implementation of <see cref="ITypeConverter"/>.
/// </summary>
public abstract class TypeConverter : ITypeConverter
{
    /// <inheritdoc/>
    public virtual T Convert<T>(object value)
    {
        if (value is T targetValue)
        {
            return targetValue;
        }

        return (T)Convert(value, typeof(T));
    }

    /// <inheritdoc/>
    public virtual object Convert(object value, Type targetType)
    {
        if (value.GetType() == targetType)
        {
            return value;
        }

        if (value is DateTime dateTime && targetType == typeof(DateTimeOffset))
        {
            return new DateTimeOffset(dateTime);
        }

        if (targetType.IsEnum)
        {
            return Enum.ToObject(targetType, value);
        }

        if (targetType.IsArray && value is Array expandArray)
        {
            return ConvertArray(expandArray, targetType);
        }

        if (value is ExpandoObject expandoObject)
        {
            return ConvertExpando(expandoObject, targetType);
        }

        if (value is DynamicObject dynamicObject)
        {
            return ConvertDynamic(dynamicObject, targetType);
        }

        try
        {
            return System.Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
        }
        catch (InvalidCastException ex)
        {
            throw new SymbolException(ex.Message);
        }
    }

    private object Convert(IEnumerable<(string Name, object? Value)> members, Type targetType)
    {
        var membersLookup = members.ToDictionary(x => x.Name, x => x.Value, StringComparer.OrdinalIgnoreCase);
        var constructors = targetType.GetConstructors()
            .Select(constructor =>
            {
                var parameters = constructor.GetParameters();
                return (constructor, parameters);
            })
            .Where(cp => cp.parameters.Length == membersLookup.Count
                        && cp.parameters.All(parameter => parameter.Name != null && membersLookup.ContainsKey(parameter.Name)));

        foreach (var (constructor, parameters) in constructors)
        {
            var arguments = parameters.Select(x =>
            {
                _ = x.Name ?? throw new InvalidOperationException("Parameter name is null");
                var memberValue = membersLookup.GetValueOrDefault(x.Name)
                    ?? throw new SymbolException($"Member: {x.Name} was null");
                return Convert(memberValue, x.ParameterType);
            }).ToArray();

            var instance = Activator.CreateInstance(targetType, arguments);
            if (instance != null)
            {
                return instance;
            }
        }

        var destination = Activator.CreateInstance(targetType)
            ?? throw new NotSupportedException($"Unable to create a instance for type: {targetType.Name}");

        foreach (var (memberName, memberValue) in members)
        {
            var property = targetType.GetProperty(memberName)
                ?? throw new InvalidOperationException($"{memberName} not found as a property");

            if (!property.CanWrite)
            {
                throw new InvalidOperationException($"{property.Name} is not writable");
            }

            if (memberValue == null)
            {
                throw new SymbolException($"Member: {property.Name} was null");
            }

            var convertedValue = Convert(memberValue, property.PropertyType);
            property.SetValue(destination, convertedValue);
        }

        return destination;
    }

    private object ConvertArray(Array expandArray, Type targetType)
    {
        var elementType = targetType.GetElementType() ?? throw new NotSupportedException($"Unable to retrieve element type");
        var dimensionLengts = new int[expandArray.Rank];
        for (var i = 0; i < expandArray.Rank; i++)
        {
            dimensionLengts[i] = expandArray.GetLength(i);
        }

        var destination = Array.CreateInstance(elementType, dimensionLengts);
        foreach (var indices in IndicesHelper.GetIndices(destination))
        {
            var dynamicValue = expandArray.GetValue(indices)
                ?? throw new SymbolException($"No value found at index: {indices}");
            destination.SetValue(Convert(dynamicValue, elementType), indices);
        }

        return destination;
    }

    private object ConvertDynamic(DynamicObject dynamicObject, Type targetType)
    {
        if (targetType.IsArray)
        {
            throw new NotSupportedException($"Can't convert from {typeof(DynamicObject)} to {targetType}");
        }

        var data = dynamicObject.GetDynamicMemberNames().Select(x =>
         {
             if (!dynamicObject.TryGetMember(new MemberBinder(x, true), out var memberValue))
             {
                 throw new InvalidOperationException($"{x} is not found in the PLC type");
             }

             return (x, memberValue);
         });

        return Convert(data, targetType);
    }

    private object ConvertExpando(ExpandoObject expandoObject, Type targetType)
    {
        var data = expandoObject.Select(x => (x.Key, x.Value));
        return Convert(data, targetType);
    }

    /// <summary>
    /// A simple implementation of <see cref="GetMemberBinder"/>.
    /// </summary>
    protected sealed class MemberBinder : GetMemberBinder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberBinder"/> class.
        /// </summary>
        /// <param name="name">The name of the member to get.</param>
        /// <param name="ignoreCase">true if the name should be matched ignoring case; false otherwise.</param>
        public MemberBinder(string name, bool ignoreCase)
            : base(name, ignoreCase)
        {
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject? errorSuggestion)
            => throw new NotSupportedException();
    }
}