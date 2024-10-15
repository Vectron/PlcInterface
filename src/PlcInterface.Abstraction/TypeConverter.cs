using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Globalization;
using System.Reflection;

namespace PlcInterface;

/// <summary>
/// A base implementation of <see cref="ITypeConverter"/>.
/// </summary>
public abstract class TypeConverter : ITypeConverter
{
    private readonly ConcurrentDictionary<string, ITypeActivator[]> activatorCache = new(StringComparer.OrdinalIgnoreCase);

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

        if (targetType.IsEnum
            && value.GetType() == typeof(string))
        {
            var enumNumber = Convert(value, typeof(long));
            return Enum.ToObject(targetType, enumNumber);
        }

        if (targetType.IsEnum)
        {
            return Enum.ToObject(targetType, value);
        }

        if (targetType.IsArray && value is Array expandArray)
        {
            return ConvertArray(expandArray, targetType);
        }

        if (targetType.IsGenericType
            && targetType.GetGenericTypeDefinition() == typeof(NonZeroBasedArray<>)
            && value is Array nonZeroBoundArray)
        {
            return ConvertNonZeroBoundArray(nonZeroBoundArray, targetType);
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

    private object Convert(Func<string, Type, object?> memberValueGetter, int memberCount, Type targetType)
    {
        var parameterInfo = activatorCache.GetOrAdd(
            targetType.Name,
            (key, targetType) =>
            {
                var constructors = targetType.GetConstructors();
                var activators = new List<ITypeActivator>(constructors.Length);
                if (constructors.Length == 0
                    && targetType.IsValueType)
                {
                    activators.Add(new StructActivator(targetType));
                }

                foreach (var constructor in constructors)
                {
                    activators.Add(new ObjectActivator(constructor));
                }

                return [.. activators];
            },
            targetType);

        foreach (var typeMapperInfo in parameterInfo)
        {
            if (!typeMapperInfo.TryCreateInstance(memberValueGetter, memberCount, out var instance))
            {
                continue;
            }

            return instance;
        }

        throw new NotSupportedException($"Failed to create an instance of {targetType.Name}");
    }

    private Array ConvertArray(Array source, Type targetType)
    {
        var elementType = targetType.GetElementType() ?? throw new NotSupportedException($"Unable to retrieve element type");
        var dimensionLengths = new int[source.Rank];
        var dimensionLowerBound = new int[source.Rank];
        for (var i = 0; i < source.Rank; i++)
        {
            dimensionLengths[i] = source.GetLength(i);
            dimensionLowerBound[i] = source.GetLowerBound(i);
        }

        var destination = Array.CreateInstance(elementType, dimensionLengths);

        using var destinationEnumerator = IndicesHelper.GetIndices(destination).GetEnumerator();
        using var sourceEnumerator = IndicesHelper.GetIndices(source).GetEnumerator();

        while (destinationEnumerator.MoveNext() && sourceEnumerator.MoveNext())
        {
            var sourceIndices = sourceEnumerator.Current;
            var destinationIndices = destinationEnumerator.Current;

            var dynamicValue = source.GetValue(sourceIndices)
                ?? throw new SymbolException($"No value found at index: {string.Join(';', sourceIndices)}");
            destination.SetValue(Convert(dynamicValue, elementType), destinationIndices);
        }

        return destination;
    }

    private object ConvertDynamic(DynamicObject dynamicObject, Type targetType)
    {
        if (targetType.IsArray)
        {
            throw new NotSupportedException($"Can't convert from {typeof(DynamicObject)} to {targetType}");
        }

        object? GetValue(string name, Type targetType)
        {
            if (dynamicObject.TryGetMember(new MemberBinder(name, ignoreCase: true), out var value)
                && value != null)
            {
                return Convert(value, targetType);
            }

            return null;
        }

        return Convert(GetValue, dynamicObject.GetDynamicMemberNames().Count(), targetType);
    }

    private object ConvertExpando(ExpandoObject expandoObject, Type targetType)
    {
        var expando = expandoObject as IDictionary<string, object>;
        object? GetValue(string name, Type targetType)
        {
            if (expando.TryGetValue(name, out var value)
                && value != null)
            {
                return Convert(value, targetType);
            }

            return null;
        }

        return Convert(GetValue, expando.Count, targetType);
    }

    private object ConvertNonZeroBoundArray(Array source, Type targetType)
    {
        var elementType = targetType.GenericTypeArguments[0];

        var method = typeof(TypeConverter).GetMethod(nameof(CreateNonZeroBasedArray), BindingFlags.Instance | BindingFlags.NonPublic);
        var genericMethod = method?.MakeGenericMethod(elementType);
        var array = genericMethod?.Invoke(this, [source])
            ?? throw new InvalidProgramException("Unable to call CreateNonZeroBasedArray");
        return array;
    }

    private NonZeroBasedArray<T> CreateNonZeroBasedArray<T>(Array source)
    {
        var dimensionLengths = new int[source.Rank];
        var dimensionLowerBound = new int[source.Rank];
        for (var i = 0; i < source.Rank; i++)
        {
            dimensionLengths[i] = source.GetLength(i);
            dimensionLowerBound[i] = source.GetLowerBound(i);
        }

        var destination = new NonZeroBasedArray<T>(dimensionLengths, dimensionLowerBound);
        foreach (var indices in destination.Indices)
        {
            var dynamicValue = source.GetValue(indices)
                ?? throw new SymbolException($"No value found at index: {string.Join(';', indices)}");
            var convertedValue = Convert<T>(dynamicValue);
            destination.SetValue(convertedValue, indices);
        }

        return destination;
    }

    /// <summary>
    /// A simple implementation of <see cref="GetMemberBinder"/>.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="MemberBinder"/> class.
    /// </remarks>
    /// <param name="name">The name of the member to get.</param>
    /// <param name="ignoreCase">true if the name should be matched ignoring case; false otherwise.</param>
    protected sealed class MemberBinder(string name, bool ignoreCase) : GetMemberBinder(name, ignoreCase)
    {
        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject? errorSuggestion)
            => throw new NotSupportedException();
    }
}
