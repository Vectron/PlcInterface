namespace PlcInterface;

/// <summary>
/// Represents a type used to read and write values from the PLC.
/// </summary>
public interface IReadWrite
{
    /// <summary>
    /// Read multiple values from the PLC.
    /// </summary>
    /// <param name="ioNames">The tag names to read.</param>
    /// <returns>A <see cref="IDictionary{TKey, TValue}"/> containing the tag name and it's value.</returns>
    public IDictionary<string, object> Read(IEnumerable<string> ioNames);

    /// <summary>
    /// Read a value from the PLC.
    /// </summary>
    /// <param name="ioName">The tag name to read.</param>
    /// <returns>The value of the tag.</returns>
    public object Read(string ioName);

    /// <summary>
    /// Read a value from the PLC.
    /// </summary>
    /// <typeparam name="T">The type to read.</typeparam>
    /// <param name="ioName">The tag name to read.</param>
    /// <returns>The value of the tag.</returns>
    public T Read<T>(string ioName);

    /// <summary>
    /// Read multiple values asynchronously from the PLC.
    /// </summary>
    /// <param name="ioNames">The tag names to read.</param>
    /// <returns>A <see cref="Task"/> for the read.</returns>
    public Task<IDictionary<string, object>> ReadAsync(IEnumerable<string> ioNames);

    /// <summary>
    /// Read a value asynchronously from the PLC.
    /// </summary>
    /// <param name="ioName">The tag name to read.</param>
    /// <returns>A <see cref="Task"/> for the read.</returns>
    public Task<object> ReadAsync(string ioName);

    /// <summary>
    /// Read a value asynchronously from the PLC.
    /// </summary>
    /// <typeparam name="T">The type to read.</typeparam>
    /// <param name="ioName">The tag name to read.</param>
    /// <returns>A <see cref="Task"/> for the read.</returns>
    public Task<T> ReadAsync<T>(string ioName);

    /// <summary>
    /// Read a value from the PLC.
    /// </summary>
    /// <param name="ioName">The tag name to read.</param>
    /// <returns>The value of the tag as a dynamic object.</returns>
    public dynamic ReadDynamic(string ioName);

    /// <summary>
    /// Read a value from the PLC.
    /// </summary>
    /// <param name="ioName">The tag name to read.</param>
    /// <returns>A <see cref="Task"/> for the read.</returns>
    public Task<dynamic> ReadDynamicAsync(string ioName);

    /// <summary>
    /// Toggles a boolean value.
    /// </summary>
    /// <param name="ioName">The tag name to read.</param>
    public void ToggleBool(string ioName);

    /// <summary>
    /// Write multiple values to the PLC.
    /// </summary>
    /// <param name="namesValues">A <see cref="IDictionary{TKey, TValue}"/> containing the tag and it's new value.</param>
    public void Write(IDictionary<string, object> namesValues);

    /// <summary>
    /// Write a values to the PLC.
    /// </summary>
    /// <typeparam name="T">The type to write.</typeparam>
    /// <param name="ioName">The tag name to write.</param>
    /// <param name="value">The value to write.</param>
    public void Write<T>(string ioName, T value)
        where T : notnull;

    /// <summary>
    /// Write multiple values asynchronously to the PLC.
    /// </summary>
    /// <param name="namesValues">A <see cref="IDictionary{TKey, TValue}"/> containing the tag and it's new value.</param>
    /// <returns>A <see cref="Task"/> for the write.</returns>
    public Task WriteAsync(IDictionary<string, object> namesValues);

    /// <summary>
    /// Write a values asynchronously to the PLC.
    /// </summary>
    /// <typeparam name="T">The type to write.</typeparam>
    /// <param name="ioName">The tag name to write.</param>
    /// <param name="value">The value to write.</param>
    /// <returns>A <see cref="Task"/> for the write.</returns>
    public Task WriteAsync<T>(string ioName, T value)
        where T : notnull;
}
