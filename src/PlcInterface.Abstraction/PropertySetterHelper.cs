using System.Linq.Expressions;
using System.Reflection;

namespace PlcInterface;

/// <summary>
/// Helper for setting property values.
/// </summary>
internal sealed class PropertySetterHelper
{
    private readonly PropertyInfo propertyInfo;
    private readonly PropertySetter setter;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertySetterHelper"/> class.
    /// </summary>
    /// <param name="propertyInfo">The <see cref="PropertyInfo"/> to create a setter binding for.</param>
    public PropertySetterHelper(PropertyInfo propertyInfo)
    {
        this.propertyInfo = propertyInfo;
        setter = GetSetter();
    }

    private delegate void PropertySetter(object instance, object value);

    /// <summary>
    /// Gets the name of the property.
    /// </summary>
    public string Name => propertyInfo.Name;

    /// <summary>
    /// Gets the <see cref="Type"/> of the property.
    /// </summary>
    public Type PropertyType => propertyInfo.PropertyType;

    /// <summary>
    /// Set the given value to the given instance.
    /// </summary>
    /// <param name="instance">The instance containing the property.</param>
    /// <param name="value">The value to set.</param>
    public void Set(object instance, object value) => setter.Invoke(instance, value);

    private PropertySetter GetSetter()
    {
        if (propertyInfo.DeclaringType == null)
        {
            throw new NotSupportedException($"{propertyInfo.Name} has no declaring type");
        }

        var instanceParam = Expression.Parameter(typeof(object));
        var instanceParamCast = Expression.Convert(instanceParam, propertyInfo.DeclaringType);
        var propertyParam = Expression.Parameter(typeof(object));
        var propertyParamCast = Expression.Convert(propertyParam, propertyInfo.PropertyType);
        var propertyGetterExpression = Expression.Property(instanceParamCast, propertyInfo.Name);
        var assignExpression = Expression.Assign(propertyGetterExpression, propertyParamCast);
        var lambda = Expression.Lambda<PropertySetter>(assignExpression, instanceParam, propertyParam);
        var compiled = lambda.Compile();
        return compiled;
    }
}
