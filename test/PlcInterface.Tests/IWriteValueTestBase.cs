using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlcInterface.Tests
{
    public abstract class IWriteValueTestBase : ConnectionBase
    {
        protected abstract string BoolValue
        {
            get;
        }

        protected abstract IEnumerable<object[]> Data
        {
            get;
        }

        [TestMethod]
        public void ToggleBool()
        {
            // Arrange
            var readWrite = GetReadWrite();

            // Act
            var original = readWrite.Read<bool>(BoolValue);
            readWrite.ToggleBool(BoolValue);
            var newValue = readWrite.Read<bool>(BoolValue);

            // Assert
            Assert.AreNotEqual(original, newValue);
        }

        [TestMethod]
        public void Write()
        {
            var readWrite = GetReadWrite();

            foreach (var item in Data)
            {
                // Arrange
                var ioName = item[0] as string;
                var newValue = item[1];

                // Act
                var original = readWrite.Read(ioName);
                readWrite.Write(ioName, newValue);
                var newValueRead = readWrite.Read(ioName);

                // Assert
                Assert.That.ObjectNotEquals(newValue, original, "Reset values in PLC");
                Assert.That.ObjectEquals(newValue, newValueRead);
            }
        }

        [TestMethod]
        public async Task WriteAsync()
        {
            var readWrite = GetReadWrite();

            foreach (var item in Data)
            {
                // Arrange
                var ioName = item[0] as string;
                var newValue = item[1];

                // Act
                var original = await readWrite.ReadAsync(ioName);
                await readWrite.WriteAsync(ioName, newValue);
                var newValueRead = await readWrite.ReadAsync(ioName);

                // Assert
                Assert.That.ObjectNotEquals(newValue, original, "Reset values in PLC");
                Assert.That.ObjectEquals(newValue, newValueRead);
            }
        }

        [TestMethod]
        public abstract void WriteGeneric();

        [TestMethod]
        public abstract Task WriteGenericAsync();

        [TestMethod]
        public void WriteMultiple()
        {
            // Arrange
            var data = Data.ToDictionary(x => (string)x[0], x => x[1]);
            var readWrite = GetReadWrite();

            // Act
            var original = readWrite.Read(data.Keys);
            readWrite.Write(data);
            var newValueRead = readWrite.Read(data.Keys);

            // Assert
            var dataEnumerator = data.GetEnumerator();
            var originalEnumerator = original.GetEnumerator();
            var newValueEnumerator = newValueRead.GetEnumerator();

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

                Assert.That.ObjectEquals(dataEnumerator.Current.Key, newValueEnumerator.Current.Key);
                Assert.That.ObjectEquals(dataEnumerator.Current.Key, originalEnumerator.Current.Key);
                Assert.That.ObjectNotEquals(dataEnumerator.Current.Value, originalEnumerator.Current.Value, "Reset values in PLC");
                Assert.That.ObjectEquals(dataEnumerator.Current.Value, newValueEnumerator.Current.Value);
            }
        }

        [TestMethod]
        public async Task WriteMultipleAsync()
        {
            // Arrange
            var data = Data.ToDictionary(x => (string)x[0], x => x[1]);
            var readWrite = GetReadWrite();

            // Act
            var original = await readWrite.ReadAsync(data.Keys);
            await readWrite.WriteAsync(data);
            var newValueRead = await readWrite.ReadAsync(data.Keys);

            // Assert
            var dataEnumerator = data.GetEnumerator();
            var originalEnumerator = original.GetEnumerator();
            var newValueEnumerator = newValueRead.GetEnumerator();

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

                Assert.That.ObjectEquals(dataEnumerator.Current.Key, newValueEnumerator.Current.Key);
                Assert.That.ObjectEquals(dataEnumerator.Current.Key, originalEnumerator.Current.Key);
                Assert.That.ObjectNotEquals(dataEnumerator.Current.Value, originalEnumerator.Current.Value, "Reset values in PLC");
                Assert.That.ObjectEquals(dataEnumerator.Current.Value, newValueEnumerator.Current.Value);
            }
        }

        protected override IMonitor GetMonitor()
        {
            throw new System.NotImplementedException();
        }

        protected void WriteValueGenericHelper<T>(string ioName, T newValue)
        {
            // Arrange
            var readWrite = GetReadWrite();

            // Act
            var original = readWrite.Read(ioName);
            readWrite.Write(ioName, newValue);
            var newValueRead = readWrite.Read(ioName);

            // Assert
            Assert.That.ObjectNotEquals(newValue, original, "Reset values in PLC");
            Assert.That.ObjectEquals(newValue, newValueRead);
        }

        protected async Task WriteValueGenericHelperAsync<T>(string ioName, T newValue)
        {
            // Arrange
            var readWrite = GetReadWrite();

            // Act
            var original = await readWrite.ReadAsync(ioName);
            await readWrite.WriteAsync(ioName, newValue);
            var newValueRead = await readWrite.ReadAsync(ioName);

            // Assert
            Assert.That.ObjectNotEquals(newValue, original, "Reset values in PLC");
            Assert.That.ObjectEquals(newValue, newValueRead);
        }
    }
}