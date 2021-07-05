using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.Tests.DataTypes;

namespace PlcInterface.Tests
{
    public abstract class IReadValueTestBase : ConnectionBase
    {
        [TestMethod]
        public void ReadDynamic()
        {
            // Arrange
            var readWrite = GetReadWrite();
            var expected = DUT_TestStruct.Default;

            // Act
            var structValue = readWrite.ReadDynamic("ReadTestData.StructValue");

            // Assert
            AssertExtension.DUT_TestStructEquals(expected, structValue);
        }

        [TestMethod]
        public async Task ReadDynamicAsync()
        {
            // Arrange
            var readWrite = GetReadWrite();
            var expected = DUT_TestStruct.Default;

            // Act
            var structValue = await readWrite.ReadDynamicAsync("ReadTestData.StructValue");

            // Assert
            AssertExtension.DUT_TestStructEquals(expected, structValue);
        }

        [DataTestMethod]
        [DynamicData(nameof(Settings.GetReadData), typeof(Settings), DynamicDataSourceType.Method)]
        [DataRow("ReadTestData.EnumValue", TestEnum.Second)]
        public void ReadGeneric(string ioName, object value)
        {
            var instanceType = value.GetType();
            var method = typeof(IReadValueTestBase)
                .GetMethod(nameof(ReadValueGenericHelper), BindingFlags.NonPublic | BindingFlags.Instance)
                .MakeGenericMethod(instanceType);
            _ = method.Invoke(this, new object[] { ioName, value });
        }

        [DataTestMethod]
        [DynamicData(nameof(Settings.GetReadData), typeof(Settings), DynamicDataSourceType.Method)]
        [DataRow("ReadTestData.EnumValue", TestEnum.Second)]
        public async Task ReadGenericAsync(string ioName, object value)
        {
            var instanceType = value.GetType();
            var method = typeof(IReadValueTestBase)
                .GetMethod(nameof(ReadValueGenericHelperAsync), BindingFlags.NonPublic | BindingFlags.Instance)
                .MakeGenericMethod(instanceType);
            await (Task)method.Invoke(this, new object[] { ioName, value });
        }

        [TestMethod]
        public void ReadMultiple()
        {
            // Arrange
            var data = Settings.ReadMultipleData;
            var readWrite = GetReadWrite();

            // Act
            var value = readWrite.Read(data.Keys);

            // Assert
            using var dataEnumerator = data.GetEnumerator();
            using var valueEnumerator = value.GetEnumerator();

            Assert.AreEqual(data.Count, value.Count);
            var multiAssert = new MultiAssert();

            while (dataEnumerator.MoveNext()
                && valueEnumerator.MoveNext())
            {
                multiAssert.Check(() => Assert.AreEqual(dataEnumerator.Current.Key, valueEnumerator.Current.Key));
                multiAssert.Check(() => Assert.That.ObjectEquals(dataEnumerator.Current.Value, valueEnumerator.Current.Value, dataEnumerator.Current.Key));
            }

            multiAssert.Assert();
        }

        [TestMethod]
        public async Task ReadMultipleAsync()
        {
            // Arrange
            var data = Settings.ReadMultipleData;
            var readWrite = GetReadWrite();

            // Act
            var value = await readWrite.ReadAsync(data.Keys);

            // Assert
            using var dataEnumerator = data.GetEnumerator();
            using var valueEnumerator = value.GetEnumerator();

            Assert.AreEqual(data.Count, value.Count);
            var multiAssert = new MultiAssert();

            while (dataEnumerator.MoveNext()
                && valueEnumerator.MoveNext())
            {
                multiAssert.Check(() => Assert.AreEqual(dataEnumerator.Current.Key, valueEnumerator.Current.Key));
                multiAssert.Check(() => Assert.That.ObjectEquals(dataEnumerator.Current.Value, valueEnumerator.Current.Value, dataEnumerator.Current.Key));
            }

            multiAssert.Assert();
        }

        [DataTestMethod]
        [DynamicData(nameof(Settings.GetReadData), typeof(Settings), DynamicDataSourceType.Method)]
        public void ReadValue(string ioName, object expectedValue)
        {
            // Arrange
            var readWrite = GetReadWrite();

            // Act
            var value = readWrite.Read(ioName);

            // Assert
            Assert.That.ObjectEquals(expectedValue, value, ioName);
        }

        [DataTestMethod]
        [DynamicData(nameof(Settings.GetReadData), typeof(Settings), DynamicDataSourceType.Method)]
        public async Task ReadValueAsync(string ioName, object expectedValue)
        {
            // Arrange
            var readWrite = GetReadWrite();

            // Act
            var value = await readWrite.ReadAsync(ioName);

            // Assert
            Assert.That.ObjectEquals(expectedValue, value, ioName);
        }

        protected override IMonitor GetMonitor()
            => throw new NotSupportedException();

        protected void ReadValueGenericHelper<T>(string ioName, T expectedValue)
        {
            // Arrange
            var readWrite = GetReadWrite();

            // Act
            var value = readWrite.Read<T>(ioName);

            // Assert
            Assert.IsNotNull(expectedValue);
            Assert.IsNotNull(value);
            Assert.That.ObjectEquals(expectedValue, value, ioName);
        }

        protected async Task ReadValueGenericHelperAsync<T>(string ioName, T expectedValue)
        {
            // Arrange
            var readWrite = GetReadWrite();

            // Act
            var value = await readWrite.ReadAsync<T>(ioName).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(expectedValue);
            Assert.IsNotNull(value);
            Assert.That.ObjectEquals(expectedValue, value, ioName);
        }
    }
}