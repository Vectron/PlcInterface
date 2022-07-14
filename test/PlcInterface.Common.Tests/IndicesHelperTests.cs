using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlcInterface.Abstraction.Tests;

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
        var indeces1 = IndicesHelper.GetIndices(name);
        var indeces2 = IndicesHelper.GetIndices(name.AsSpan());

        // Assert
        CollectionAssert.AreEqual(expected, indeces1);
        CollectionAssert.AreEqual(indeces1, indeces2);
    }

    [TestMethod]
    public void IndicesOnlyCreatesOneArray()
    {
        // Arrange
        var data = Data;
        Array? first = null;

        // Act
        // Assert
        foreach (var indices in IndicesHelper.GetIndices(data))
        {
            if (first == null)
            {
                first = indices;
            }

            Assert.AreSame(first, indices);
        }
    }

    [TestMethod]
    public void IndicesReturnsAllArrayDimensions()
    {
        // Arrange
        var data = Data;
        IEnumerable<int[]> expected = new int[][]
        {
                new int[] { 0, 0, 0 },
                new int[] { 0, 0, 1 },
                new int[] { 0, 0, 2 },
                new int[] { 0, 1, 0 },
                new int[] { 0, 1, 1 },
                new int[] { 0, 1, 2 },
                new int[] { 0, 2, 0 },
                new int[] { 0, 2, 1 },
                new int[] { 0, 2, 2 },
                new int[] { 1, 0, 0 },
                new int[] { 1, 0, 1 },
                new int[] { 1, 0, 2 },
                new int[] { 1, 1, 0 },
                new int[] { 1, 1, 1 },
                new int[] { 1, 1, 2 },
                new int[] { 1, 2, 0 },
                new int[] { 1, 2, 1 },
                new int[] { 1, 2, 2 },
                new int[] { 2, 0, 0 },
                new int[] { 2, 0, 1 },
                new int[] { 2, 0, 2 },
                new int[] { 2, 1, 0 },
                new int[] { 2, 1, 1 },
                new int[] { 2, 1, 2 },
                new int[] { 2, 2, 0 },
                new int[] { 2, 2, 1 },
                new int[] { 2, 2, 2 },
        };
        using var expectedEnumerator = expected.GetEnumerator();

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