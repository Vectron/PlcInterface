using System.Collections;
using System.Dynamic;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlcInterface.IntegrationTests.Extension;

internal static class AssertObjectValue
{
    public static void ValuesAreEqual(object? expected, object? actual, string message = "")
    {
        var fullMessage = GetFullMessage(expected, message);
        ValidateObject(expected, actual, fullMessage, equals: true);
    }

    public static void ValuesAreNotEqual(object? expected, object? actual, string message = "")
    {
        var fullMessage = GetFullMessage(expected, message);
        ValidateObject(expected, actual, fullMessage, equals: false);
    }

    private static object GetDynamicValue(dynamic o, string name)
    {
        if (o is IDictionary<string, object> expando)
        {
            return expando[name];
        }

        var arguments = new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, name: null) };
        var binder = Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, name, context: null, arguments);
        var site = CallSite<Func<CallSite, object, object>>.Create(binder);
        return site.Target(site, o);
    }

    private static string GetFullMessage(object? expected, string message)
    {
        var messageBuilder = new StringBuilder();
        if (!string.IsNullOrEmpty(message))
        {
            _ = messageBuilder.Append(message)
                .Append(':')
                .AppendLine();
        }

        if (expected is not null)
        {
            _ = messageBuilder
                .Append(expected.GetType().Name);
        }

        return messageBuilder.ToString();
    }

    private static void ValidateArray(Array expected, Array actual, string message, bool equals)
    {
        Assert.AreEqual(expected.Rank, actual.Rank, $"{message} -> Dimensions differ");
        Assert.AreEqual(expected.Length, actual.Length, $"{message} -> Number of elements differ");
        for (var dimension = 0; dimension < expected.Rank; dimension++)
        {
            var expectedDimensionLength = expected.GetLength(dimension);
            var actualDimensionLength = expected.GetLength(dimension);
            Assert.AreEqual(expectedDimensionLength, actualDimensionLength, string.Format(CultureInfo.InvariantCulture, "{0} -> dimension {1} have different lower bounds", message, dimension));

            var expectedDimensionLowerBound = expected.GetLength(dimension);
            var actualDimensionLowerBound = expected.GetLength(dimension);
            Assert.AreEqual(expectedDimensionLowerBound, actualDimensionLowerBound, string.Format(CultureInfo.InvariantCulture, "{0} -> dimension {1} have different lower bounds", message, dimension));
        }

        ValidateEnumerable(expected, actual, message, equals);
    }

    private static void ValidateEnumerable(IEnumerable expected, IEnumerable actual, string message, bool equals)
    {
        var expectedEnumerator = expected.GetEnumerator();
        var actualEnumerator = actual.GetEnumerator();
        var index = 0;
        while (expectedEnumerator.MoveNext())
        {
            if (!actualEnumerator.MoveNext())
            {
                Assert.Fail($"{message} -> Number of elements differ");
                return;
            }

            var currentExpected = expectedEnumerator.Current;
            var currentActual = actualEnumerator.Current;
            ValidateObject(currentExpected, currentActual, string.Format(CultureInfo.InvariantCulture, "{0}[{1}]", message, index), equals);
            index++;
        }

        if (actualEnumerator.MoveNext() && !expectedEnumerator.MoveNext())
        {
            Assert.Fail($"{message} -> Number of elements differ");
            return;
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MA0051:Method is too long", Justification = "Method length should no be that inportant")]
    private static void ValidateObject(object? expected, object? actual, string message, bool equals)
    {
        if (expected == null)
        {
            Assert.IsNull(actual, message);
            return;
        }

        Assert.IsNotNull(expected, message);

        if (expected is DateTime expectedDateTime
            && actual is DateTimeOffset actualDateTimeOffset)
        {
            if (equals)
            {
                Assert.AreEqual(expectedDateTime, actualDateTimeOffset.DateTime, message);
            }
            else
            {
                Assert.AreNotEqual(expectedDateTime, actualDateTimeOffset.DateTime, message);
            }

            return;
        }

        if (expected is DateTimeOffset expectedDateTimeOffset
            && actual is DateTime actualDateTime)
        {
            if (equals)
            {
                Assert.AreEqual(expectedDateTimeOffset, new DateTimeOffset(actualDateTime), message);
            }
            else
            {
                Assert.AreNotEqual(expectedDateTimeOffset, new DateTimeOffset(actualDateTime), message);
            }

            return;
        }

        if (expected
            is bool
            or byte
            or sbyte
            or short
            or ushort
            or int
            or uint
            or ulong
            or long
            or float
            or double
            or string
            or TimeSpan
            or DateTimeOffset
            or DateTime)
        {
            if (equals)
            {
                Assert.AreEqual(expected, actual, message);
            }
            else
            {
                Assert.AreNotEqual(expected, actual, message);
            }

            return;
        }

        if (expected is Array expectedArray)
        {
            if (actual is not Array actualArray)
            {
                Assert.Fail("Actual is not an array");
                return;
            }

            ValidateArray(expectedArray, actualArray, message, equals);
            return;
        }

        if (expected is IEnumerable expectedEnumerable)
        {
            if (actual is not IEnumerable actualEnumerable)
            {
                Assert.Fail($"{message} -> Actual is not a IEnumerable");
                return;
            }

            ValidateEnumerable(expectedEnumerable, actualEnumerable, message, equals);
            return;
        }

        var expectedType = expected.GetType();
        var properties = expectedType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var property in properties)
        {
            var expectedValue = property.GetValue(expected);
            var actualValue = actual is IDynamicMetaObjectProvider ? GetDynamicValue(actual, property.Name) : property.GetValue(actual);
            ValidateObject(expectedValue, actualValue, $"{message}.{property.Name}", equals);
        }
    }
}
