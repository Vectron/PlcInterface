using System.Collections;

namespace PlcInterface;

/// <summary>
/// A wrapper around a Array with non zero indexing.
/// </summary>
/// <typeparam name="T">The Type of the Array to create.</typeparam>
/// <param name="lengths">A one-dimensional array that contains the size of each dimension of the Array to create.</param>
/// <param name="lowerBounds">A one-dimensional array that contains the lower bound (starting index) of each dimension of the Array to create.</param>
public readonly struct NonZeroBasedArray<T>(int[] lengths, int[] lowerBounds) : IEnumerable<T>, IStructuralComparable, IStructuralEquatable, IArrayWrapper
{
    private readonly Array backingArray = Array.CreateInstance(typeof(T), lengths, lowerBounds);

    /// <summary>
    /// Gets the backing array storage.
    /// </summary>
    Array IArrayWrapper.BackingArray => backingArray;

    /// <inheritdoc/>
    Type IArrayWrapper.ElementType => typeof(T);

    /// <summary>
    /// Gets all indices of this array.
    /// </summary>
    public IEnumerable<int[]> Indices
        => IndicesHelper.GetIndices(backingArray);

    /// <inheritdoc cref="Array.Length"/>
    public int Length => backingArray.Length;

    /// <inheritdoc cref="Array.LongLength"/>
    public long LongLength => backingArray.LongLength;

    /// <inheritdoc cref="Array.Rank"/>
    public int Rank => backingArray.Rank;

    /// <summary>
    /// Gets or sets the element at the specified index.
    /// </summary>
    /// <param name="index">The index of the element to get or set.</param>
    /// <returns>The element at the specified index.</returns>
    /// <exception cref="ArgumentException">The current Array does not have exactly one dimension.</exception>
    /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is outside the range of valid indexes for the current Array.</exception>
    public T this[int index]
    {
        get => (T)backingArray.GetValue(index)!;
        set => backingArray.SetValue(value, index);
    }

    /// <summary>
    /// Gets or sets the element at the specified index.
    /// </summary>
    /// <param name="indices">The index of the element to get or set.</param>
    /// <returns>The element at the specified index.</returns>
    /// <exception cref="ArgumentException">The current Array does not have exactly one dimension.</exception>
    /// <exception cref="IndexOutOfRangeException"><paramref name="indices"/> is outside the range of valid indexes for the current Array.</exception>
    public T this[int[] indices]
    {
        get => (T)backingArray.GetValue(indices)!;
        set => backingArray.SetValue(value, indices);
    }

    /// <summary>
    /// Check if the two items are not equal.
    /// </summary>
    /// <param name="left">The left item.</param>
    /// <param name="right">The right item.</param>
    /// <returns><see langword="true"/> when both items are not equal, otherwise <see langword="false"/>.</returns>
    public static bool operator !=(NonZeroBasedArray<T> left, NonZeroBasedArray<T> right)
        => !(left == right);

    /// <summary>
    /// Check if the two items are equal.
    /// </summary>
    /// <param name="left">The left item.</param>
    /// <param name="right">The right item.</param>
    /// <returns><see langword="true"/> when both items are equal, otherwise <see langword="false"/>.</returns>
    public static bool operator ==(NonZeroBasedArray<T> left, NonZeroBasedArray<T> right)
        => left.Equals(right);

    /// <inheritdoc/>
    int IStructuralComparable.CompareTo(object? other, IComparer comparer) => ((IStructuralComparable)backingArray).CompareTo(other, comparer);

    /// <inheritdoc/>
    bool IStructuralEquatable.Equals(object? other, IEqualityComparer comparer) => ((IStructuralEquatable)backingArray).Equals(other, comparer);

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => backingArray.Equals(obj);

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => new NonZeroBasedArrayEnumerator(backingArray);

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => backingArray.GetEnumerator();

    /// <inheritdoc/>
    int IStructuralEquatable.GetHashCode(IEqualityComparer comparer) => ((IStructuralEquatable)backingArray).GetHashCode(comparer);

    /// <inheritdoc/>
    public override int GetHashCode()
        => backingArray.GetHashCode();

    /// <inheritdoc cref="Array.GetLength(int)"/>
    public int GetLength(int dimension)
        => backingArray.GetLength(dimension);

    /// <inheritdoc cref="Array.GetLowerBound(int)"/>
    public int GetLowerBound(int dimension)
        => backingArray.GetLowerBound(dimension);

    /// <inheritdoc cref="Array.GetUpperBound(int)"/>
    public int GetUpperBound(int dimension)
        => backingArray.GetUpperBound(dimension);

    /// <inheritdoc cref="Array.GetValue(int[])"/>
    public T GetValue(params int[] indices)
        => (T)backingArray.GetValue(indices)!;

    /// <inheritdoc cref="Array.GetValue(int)"/>
    public T GetValue(int index)
        => (T)backingArray.GetValue(index)!;

    /// <inheritdoc cref="Array.GetValue(int, int)"/>
    public T GetValue(int index1, int index2)
        => (T)backingArray.GetValue(index1, index2)!;

    /// <inheritdoc cref="Array.GetValue(int, int)"/>
    public T GetValue(int index1, int index2, int index3)
        => (T)backingArray.GetValue(index1, index2, index3)!;

    /// <inheritdoc cref="Array.SetValue(object?, int[])"/>
    public void SetValue(T value, params int[] indices)
        => backingArray.SetValue(value, indices);

    /// <inheritdoc cref="Array.SetValue(object?,int)"/>
    public void SetValue(T value, int index)
        => backingArray.SetValue(value, index);

    /// <inheritdoc cref="Array.SetValue(object?,int, int)"/>
    public void SetValue(T value, int index1, int index2)
        => backingArray.SetValue(value, index1, index2);

    /// <inheritdoc cref="Array.SetValue(object?,int, int)"/>
    public void SetValue(T value, int index1, int index2, int index3)
        => backingArray.SetValue(value, index1, index2, index3);

    private readonly struct NonZeroBasedArrayEnumerator(Array array) : IEnumerator<T>
    {
        private readonly IEnumerator enumerator = array.GetEnumerator();

        public T Current => (T)enumerator.Current;

        object IEnumerator.Current => enumerator.Current;

        public void Dispose()
        {
            if (enumerator is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        public bool MoveNext() => enumerator.MoveNext();

        public void Reset() => enumerator.Reset();
    }
}
