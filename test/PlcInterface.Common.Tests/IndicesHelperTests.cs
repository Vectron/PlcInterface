using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlcInterface.Common.Tests;

[TestClass]
public class IndicesHelperTests
{
    private static int[,,] Data
        => new int[,,]
        {
            {
                { 00, 01, 02 },
                { 03, 04, 05 },
                { 06, 07, 08 },
            },
            {
                { 10, 11, 12 },
                { 13, 14, 15 },
                { 16, 17, 18 },
            },
            {
                { 20, 21, 22 },
                { 23, 24, 25 },
                { 26, 27, 28 },
            },
        };

    [TestMethod]
    public void GetIndicesReturnsTheValueFromString()
    {
        // Arrange
        var name = "ArrayObject[5,3]";
        var expected = new[] { 5, 3 };

        // Act
        var indices1 = IndicesHelper.GetIndices(name);
        var indices2 = IndicesHelper.GetIndices(name.AsSpan());

        // Assert
        CollectionAssert.AreEqual(expected, indices1);
        CollectionAssert.AreEqual(indices1, indices2);
    }

    [TestMethod]
    public void IndicesOnlyCreatesOneArray()
    {
        // Arrange
        var data = Data;
        Array? first = null;

        // Act Assert
        foreach (var indices in IndicesHelper.GetIndices(data))
        {
            first ??= indices;
            Assert.AreSame(first, indices);
        }
    }

    [TestMethod]
    public void IndicesReturnsAllArrayDimensions()
    {
        // Arrange
        var data = Data;
        var expected = new int[][]
        {
            [0, 0, 0],
            [0, 0, 1],
            [0, 0, 2],
            [0, 1, 0],
            [0, 1, 1],
            [0, 1, 2],
            [0, 2, 0],
            [0, 2, 1],
            [0, 2, 2],
            [1, 0, 0],
            [1, 0, 1],
            [1, 0, 2],
            [1, 1, 0],
            [1, 1, 1],
            [1, 1, 2],
            [1, 2, 0],
            [1, 2, 1],
            [1, 2, 2],
            [2, 0, 0],
            [2, 0, 1],
            [2, 0, 2],
            [2, 1, 0],
            [2, 1, 1],
            [2, 1, 2],
            [2, 2, 0],
            [2, 2, 1],
            [2, 2, 2],
        };
        using var expectedEnumerator = expected.AsEnumerable().GetEnumerator();

        // Act
        var actual = IndicesHelper.GetIndices(data);
        using var actualEnumerator = actual.GetEnumerator();

        // Assert
        while (actualEnumerator.MoveNext() && expectedEnumerator.MoveNext())
        {
            CollectionAssert.AreEqual(expectedEnumerator.Current, actualEnumerator.Current);
        }
    }
}
