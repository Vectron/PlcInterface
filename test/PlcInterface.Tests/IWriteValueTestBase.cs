using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlcInterface.Tests
{
    public abstract class IWriteValueTestBase : ConnectionBase
    {
        [TestInitialize]
        public void ResetPLCValues()
        {
            var readWrite = GetReadWrite();
            readWrite.Write("MAIN.Reset", true);
            while (readWrite.Read<bool>("MAIN.Reset"))
            {
            }
        }

        [TestMethod]
        public void ToggleBool()
        {
            // Arrange
            var readWrite = GetReadWrite();
            var ioName = "WriteTestData.BoolValue";

            // Act
            var original = readWrite.Read<bool>(ioName);
            readWrite.ToggleBool(ioName);
            var newValue = readWrite.Read<bool>(ioName);

            // Assert
            Assert.AreNotEqual(original, newValue);
        }

        [DataTestMethod]
        [DynamicData(nameof(Settings.GetWriteData), typeof(Settings), DynamicDataSourceType.Method)]
        public void Write(string ioName, object newValue, object readValue)
        {
            // Arrange
            var readWrite = GetReadWrite();

            // Act
            var original = readWrite.Read(ioName);
            readWrite.Write(ioName, newValue);
            var newValueRead = readWrite.Read(ioName);

            // Assert
            Assert.That.ObjectNotEquals(readValue, original, "Reset values in PLC");
            Assert.That.ObjectEquals(readValue, newValueRead, ioName);
        }

        [DataTestMethod]
        [DynamicData(nameof(Settings.GetWriteData), typeof(Settings), DynamicDataSourceType.Method)]
        public async Task WriteAsync(string ioName, object newValue, object readValue)
        {
            var readWrite = GetReadWrite();

            // Arrange

            // Act
            var original = await readWrite.ReadAsync(ioName);
            await readWrite.WriteAsync(ioName, newValue);
            var newValueRead = await readWrite.ReadAsync(ioName);

            // Assert
            Assert.That.ObjectNotEquals(readValue, original, "Reset values in PLC");
            Assert.That.ObjectEquals(readValue, newValueRead, ioName);
        }

        [DataTestMethod]
        [DynamicData(nameof(Settings.GetWriteData), typeof(Settings), DynamicDataSourceType.Method)]
        public void WriteGeneric(string ioName, object newValue, object readValue)
        {
            var methodInfo = GetType().GetMethod(nameof(WriteValueGenericHelper), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var method = methodInfo.MakeGenericMethod(newValue.GetType());
            _ = method.Invoke(this, new[] { ioName, newValue, readValue });
        }

        [DataTestMethod]
        [DynamicData(nameof(Settings.GetWriteData), typeof(Settings), DynamicDataSourceType.Method)]
        public async Task WriteGenericAsync(string ioName, object newValue, object readValue)
        {
            var methodInfo = GetType().GetMethod(nameof(WriteValueGenericHelperAsync), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var method = methodInfo.MakeGenericMethod(newValue.GetType());
            await (Task)method.Invoke(this, new[] { ioName, newValue, readValue });
        }

        [TestMethod]
        public void WriteMultiple()
        {
            // Arrange
            var data = Settings.WriteMultipleData;
            var readWrite = GetReadWrite();

            // Act
            var original = readWrite.Read(data.Keys);
            readWrite.Write(data);
            var newValueRead = readWrite.Read(data.Keys);

            // Assert
            using var dataEnumerator = data.GetEnumerator();
            using var originalEnumerator = original.GetEnumerator();
            using var newValueEnumerator = newValueRead.GetEnumerator();

            Assert.AreEqual(data.Count, original.Count);
            Assert.AreEqual(data.Count, newValueRead.Count);

            while (true)
            {
                if (!dataEnumerator.MoveNext()
                    || !newValueEnumerator.MoveNext()
                    || originalEnumerator.MoveNext())
                {
                    break;
                }

                Assert.AreEqual(dataEnumerator.Current.Key, newValueEnumerator.Current.Key);
                Assert.AreEqual(dataEnumerator.Current.Key, originalEnumerator.Current.Key);
                Assert.That.ObjectNotEquals(dataEnumerator.Current.Value, originalEnumerator.Current.Value, "Reset values in PLC");
                Assert.That.ObjectEquals(dataEnumerator.Current.Value, newValueEnumerator.Current.Value, dataEnumerator.Current.Key);
            }
        }

        [TestMethod]
        public async Task WriteMultipleAsync()
        {
            // Arrange
            var data = Settings.WriteMultipleData;
            var readWrite = GetReadWrite();

            // Act
            var original = await readWrite.ReadAsync(data.Keys);
            await readWrite.WriteAsync(data);
            var newValueRead = await readWrite.ReadAsync(data.Keys);

            // Assert
            using var dataEnumerator = data.GetEnumerator();
            using var originalEnumerator = original.GetEnumerator();
            using var newValueEnumerator = newValueRead.GetEnumerator();

            Assert.AreEqual(data.Count, original.Count);
            Assert.AreEqual(data.Count, newValueRead.Count);

            while (true)
            {
                if (!dataEnumerator.MoveNext()
                    || !newValueEnumerator.MoveNext()
                    || originalEnumerator.MoveNext())
                {
                    break;
                }

                Assert.AreEqual(dataEnumerator.Current.Key, newValueEnumerator.Current.Key);
                Assert.AreEqual(dataEnumerator.Current.Key, originalEnumerator.Current.Key);
                Assert.That.ObjectNotEquals(dataEnumerator.Current.Value, originalEnumerator.Current.Value, "Reset values in PLC");
                Assert.That.ObjectEquals(dataEnumerator.Current.Value, newValueEnumerator.Current.Value, dataEnumerator.Current.Key);
            }
        }

        protected override IMonitor GetMonitor()
            => throw new NotSupportedException();

        protected void WriteValueGenericHelper<T>(string ioName, T newValue, object readValue)
            where T : notnull
        {
            // Arrange
            var readWrite = GetReadWrite();

            // Act
            var original = readWrite.Read(ioName);
            readWrite.Write(ioName, newValue);
            var newValueRead = readWrite.Read(ioName);

            // Assert
            Assert.That.ObjectNotEquals(readValue, original, "Reset values in PLC");
            Assert.That.ObjectEquals(readValue, newValueRead, ioName);
        }

        protected async Task WriteValueGenericHelperAsync<T>(string ioName, T newValue, object readValue)
            where T : notnull
        {
            // Arrange
            var readWrite = GetReadWrite();

            // Act
            var original = await readWrite.ReadAsync(ioName).ConfigureAwait(false);
            await readWrite.WriteAsync(ioName, newValue).ConfigureAwait(false);
            var newValueRead = await readWrite.ReadAsync(ioName).ConfigureAwait(false);

            // Assert
            Assert.That.ObjectNotEquals(readValue, original, "Reset values in PLC");
            Assert.That.ObjectEquals(readValue, newValueRead, ioName);
        }
    }
}