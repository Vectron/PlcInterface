using System;
using System.Dynamic;

namespace PlcInterface.Ads
{
    /// <summary>
    /// Represents a type used to convert <see cref="DynamicObject"/>.
    /// </summary>
    public interface IDynamicValueConverter
    {
        /// <summary>
        /// Converts a <see cref="DynamicObject"/> to the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="dynamicObject">The <see cref="DynamicObject"/> to convert.</param>
        /// <param name="type">The <see cref="Type"/> to convert to.</param>
        /// <returns>The constructed type.</returns>
        object ConvertFrom(DynamicObject dynamicObject, Type type);
    }
}