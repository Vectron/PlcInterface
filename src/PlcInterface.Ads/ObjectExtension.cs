using System;
using System.Collections.Generic;

namespace PlcInterface.Ads;

/// <summary>
/// Extension methods for any object.
/// </summary>
internal static class ObjectExtension
{
    /// <summary>
    /// Do a depth first traversal of a tree.
    /// </summary>
    /// <typeparam name="T">The type of the objects in the tree.</typeparam>
    /// <param name="root">The first item to traverse.</param>
    /// <param name="children">A <see cref="Func{T, TResult}"/> for getting the children.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing all children.</returns>
    public static IEnumerable<T> DepthFirstTreeTraversal<T>(this T root, Func<T, IEnumerable<T>> children)
    {
        var stack = new Stack<T>();
        stack.Push(root);
        while (stack.Count != 0)
        {
            var current = stack.Pop();
            foreach (var child in children(current))
            {
                stack.Push(child);
            }

            yield return current;
        }
    }

    /// <summary>
    /// Do a depth first traversal of a tree.
    /// </summary>
    /// <typeparam name="T">The type of the objects in the tree.</typeparam>
    /// <param name="roots">The root items to traverse.</param>
    /// <param name="children">A <see cref="Func{T, TResult}"/> for getting the children.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing all children.</returns>
    public static IEnumerable<T> DepthFirstTreeTraversal<T>(this IEnumerable<T> roots, Func<T, IEnumerable<T>> children)
    {
        var stack = new Stack<T>(roots);

        while (stack.Count != 0)
        {
            var current = stack.Pop();
            foreach (var child in children(current))
            {
                stack.Push(child);
            }

            yield return current;
        }
    }
}
