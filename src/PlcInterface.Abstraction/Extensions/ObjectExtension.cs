using System;
using System.Collections.Generic;

namespace PlcInterface.Extensions;

/// <summary>
/// Extension methods for all classes and structs.
/// </summary>
public static class ObjectExtension
{
    /// <summary>
    /// Throw a exception when the type is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to check.</typeparam>
    /// <param name="obj">The object to check.</param>
    /// <param name="parameterName">The name of the parameter to check.</param>
    /// <returns>The non <see langword="null"/> <typeparamref name="T"/>.</returns>
    /// <exception cref="ArgumentNullException">When the <paramref name="obj"/> is <see langword="null"/>.</exception>
    public static T ThrowIfNull<T>(this T? obj, string parameterName = "")
    {
        if (obj == null)
        {
            throw new ArgumentNullException(parameterName);
        }

        return obj;
    }

    /// <summary>
    /// Wraps this object instance into an IEnumerable&lt;T&gt;
    /// consisting of a single item.
    /// </summary>
    /// <typeparam name="T"> Type of the object. </typeparam>
    /// <param name="item"> The instance that will be wrapped. </param>
    /// <returns> An IEnumerable&lt;T&gt; consisting of a single item. </returns>
    public static IEnumerable<T> Yield<T>(this T item)
    {
        yield return item;
    }
}