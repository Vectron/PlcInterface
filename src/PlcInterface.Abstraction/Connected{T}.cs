namespace PlcInterface;

/// <summary>
/// A <see cref="IConnected{T}"/> implementation.
/// </summary>
/// <typeparam name="T">The type that is connected.</typeparam>
public sealed class Connected<T> : IConnected<T>
{
    private readonly T? value;

    /// <summary>
    /// Initializes a new instance of the <see cref="Connected{T}"/> class.
    /// </summary>
    /// <param name="value">A <typeparamref name="T"/> containing the connection.</param>
    internal Connected(T value)
    {
        this.value = value;
        IsConnected = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Connected{T}"/> class.
    /// </summary>
    internal Connected()
    {
    }

    /// <inheritdoc/>
    public bool IsConnected
    {
        get;
    }

    /// <inheritdoc/>
    public T Value => value ?? throw new InvalidOperationException($"There is no value when {nameof(IsConnected)} returns false");
}
