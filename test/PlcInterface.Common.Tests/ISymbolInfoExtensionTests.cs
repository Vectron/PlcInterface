using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace PlcInterface.Common.Tests;

[TestClass]
public class ISymbolInfoExtensionTests
{
    [TestMethod]
    public void FlattenReturnsAIEnumerableOfISymbolInfoWithAllChildSymbols()
    {
        // Arrange
        var symbolInfo = new Mock<ISymbolInfo>();
        _ = symbolInfo.SetupGet(x => x.Name).Returns("FlattenObject");
        _ = symbolInfo.SetupGet(x => x.ChildSymbols).Returns(new List<string>() { "FlattenObject.BoolValue", "FlattenObject.IntValue", "FlattenObject.ChildArray" });
        var symbolHandler = new Mock<ISymbolHandler>();
        _ = symbolHandler.Setup(x => x.GetSymbolInfo(It.IsAny<string>())).Returns<string>(s =>
        {
            var symbolInfo = new Mock<ISymbolInfo>();
            _ = symbolInfo.SetupGet(x => x.ChildSymbols).Returns(new List<string>());
            _ = symbolInfo.SetupGet(x => x.Name).Returns(s);
            var splitName = s.Split('.');
            _ = symbolInfo.SetupGet(x => x.ShortName).Returns(splitName[splitName.Length - 1]);

            if (s.Equals("FlattenObject.ChildArray", StringComparison.Ordinal))
            {
                _ = symbolInfo.SetupGet(x => x.ChildSymbols).Returns(new List<string>() { "FlattenObject.ChildArray[0]", "FlattenObject.ChildArray[1]", "FlattenObject.ChildArray[2]", "FlattenObject.ChildArray[3]" });
            }

            return symbolInfo.Object;
        });

        // Act
        var linked = symbolInfo.Object.Flatten(symbolHandler.Object).ToList();

        // Assert
        Assert.AreEqual(6, linked.Count);
        Assert.AreEqual("FlattenObject.BoolValue", linked[0].Name);
        Assert.AreEqual("FlattenObject.IntValue", linked[1].Name);
        Assert.AreEqual("FlattenObject.ChildArray[0]", linked[2].Name);
        Assert.AreEqual("FlattenObject.ChildArray[1]", linked[3].Name);
        Assert.AreEqual("FlattenObject.ChildArray[2]", linked[4].Name);
        Assert.AreEqual("FlattenObject.ChildArray[3]", linked[5].Name);
    }

    [TestMethod]
    public void FlattenReturnsSourceISymbolInfoWhenNoChildren()
    {
        // Arrange
        var symbolInfo = new Mock<ISymbolInfo>();
        _ = symbolInfo.SetupGet(x => x.Name).Returns("FlattenObject");
        _ = symbolInfo.SetupGet(x => x.ChildSymbols).Returns(new List<string>());
        var symbolHandler = new Mock<ISymbolHandler>();

        // Act
        var linked = symbolInfo.Object.Flatten(symbolHandler.Object).ToList();

        // Assert
        Assert.AreEqual(1, linked.Count);
        Assert.AreEqual(symbolInfo.Object, linked[0]);
    }

    [TestMethod]
    public void FlattenWithValueFlattensAISymbolInfoChildHierarchyAndAddsValues()
    {
        // Arrange
        var symbolInfo = new Mock<ISymbolInfo>();
        _ = symbolInfo.SetupGet(x => x.Name).Returns("FlattenObject");
        _ = symbolInfo.SetupGet(x => x.ChildSymbols).Returns(new List<string>() { "FlattenObject.BoolValue", "FlattenObject.IntValue", "FlattenObject.ChildArray" });
        var symbolHandler = new Mock<ISymbolHandler>();
        _ = symbolHandler.Setup(x => x.GetSymbolInfo(It.IsAny<string>())).Returns<string>(s =>
        {
            var symbolInfo = new Mock<ISymbolInfo>();
            _ = symbolInfo.SetupGet(x => x.ChildSymbols).Returns(new List<string>());
            _ = symbolInfo.SetupGet(x => x.Name).Returns(s);
            var splitName = s.Split('.');
            _ = symbolInfo.SetupGet(x => x.ShortName).Returns(splitName[splitName.Length - 1]);

            if (s.Equals("FlattenObject.ChildArray", StringComparison.Ordinal))
            {
                _ = symbolInfo.SetupGet(x => x.ChildSymbols).Returns(new List<string>() { "FlattenObject.ChildArray[0]", "FlattenObject.ChildArray[1]", "FlattenObject.ChildArray[2]", "FlattenObject.ChildArray[3]" });
            }

            return symbolInfo.Object;
        });

        var data = new FlattenObject() { BoolValue = true, IntValue = 5, ChildArray = [9, 8, 7, 6] };

        // Act
        var linked = symbolInfo.Object.FlattenWithValue(symbolHandler.Object, data).ToList();

        // Assert
        Assert.AreEqual(6, linked.Count);
        Assert.AreEqual("FlattenObject.BoolValue", linked[0].SymbolInfo.Name);
        Assert.AreEqual(data.BoolValue, linked[0].Value);
        Assert.AreEqual("FlattenObject.IntValue", linked[1].SymbolInfo.Name);
        Assert.AreEqual(data.IntValue, linked[1].Value);
        Assert.AreEqual("FlattenObject.ChildArray[0]", linked[2].SymbolInfo.Name);
        Assert.AreEqual(data.ChildArray[0], linked[2].Value);
        Assert.AreEqual("FlattenObject.ChildArray[1]", linked[3].SymbolInfo.Name);
        Assert.AreEqual(data.ChildArray[1], linked[3].Value);
        Assert.AreEqual("FlattenObject.ChildArray[2]", linked[4].SymbolInfo.Name);
        Assert.AreEqual(data.ChildArray[2], linked[4].Value);
        Assert.AreEqual("FlattenObject.ChildArray[3]", linked[5].SymbolInfo.Name);
        Assert.AreEqual(data.ChildArray[3], linked[5].Value);
    }

    [TestMethod]
    public void FlattenWithValueReturnsTheObjectWhenNoChildren()
    {
        // Arrange
        var ioName = "FlattenObject";
        var symbolInfo = new Mock<ISymbolInfo>();
        _ = symbolInfo.SetupGet(x => x.Name).Returns(ioName);
        _ = symbolInfo.SetupGet(x => x.ChildSymbols).Returns(new List<string>());
        var symbolHandler = new Mock<ISymbolHandler>();
        var data = new FlattenObject() { BoolValue = true, IntValue = 5 };

        // Act
        var linked = symbolInfo.Object.FlattenWithValue(symbolHandler.Object, data).ToList();

        // Assert
        Assert.AreEqual(1, linked.Count);
        Assert.AreEqual(data, linked[0].Value);
        Assert.AreSame(data, linked[0].Value);
        Assert.AreEqual(symbolInfo.Object, linked[0].SymbolInfo);
    }

    private sealed class FlattenObject
    {
        public FlattenObject()
            => ChildArray = [];

        public bool BoolValue
        {
            get;
            set;
        }

        public int[] ChildArray
        {
            get;
            set;
        }

        public int IntValue
        {
            get;
            set;
        }
    }
}
