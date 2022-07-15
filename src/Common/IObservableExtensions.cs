using System;
using System.Reactive.Linq;

namespace PlcInterface;

/// <summary>
/// Extension methods for <see cref="IObservable{T}"/>.
/// </summary>
internal static class IObservableExtensions
{
    /// <summary>
    /// Filters the elements of an observable sequence based on if they are <see langword="null"/>.
    /// </summary>
    /// <typeparam name="TResult">The type of the elements in the produced sequence.</typeparam>
    /// <param name="source">An observable sequence whose elements to filter.</param>
    /// <returns>An observable sequence that contains elements from the input sequence that satisfy the condition.</returns>
    public static IObservable<TResult> WhereNotNull<TResult>(this IObservable<TResult?> source)
        => Observable.Create<TResult>(o =>
            source.Subscribe(
                x =>
                {
                    if (x != null)
                    {
                        o.OnNext(x);
                    }
                },
                o.OnError,
                o.OnCompleted));
}