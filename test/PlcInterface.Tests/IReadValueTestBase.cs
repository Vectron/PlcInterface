using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlcInterface.Tests
{
    public abstract class IReadValueTestBase : ConnectionBase
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
        public abstract void ReadDynamic();

        [TestMethod]
        public abstract Task ReadDynamicAsync();

        [TestMethod]
        public abstract void ReadGeneric();

        [TestMethod]
        public abstract Task ReadGenericAsync();

        [TestMethod]
        public void ReadMultiple()
        {
            // Arrange
            var data = Data.ToDictionary(x => (string)x[0], x => x[1]);
            var readWrite = GetReadWrite();

            // Act
            var value = readWrite.Read(data.Keys);

            // Assert
            var dataEnumerator = data.GetEnumerator();
            var valueEnumerator = value.GetEnumerator();

            Assert.AreEqual(data.Count, value.Count);

            while (true)
            {
                if (!dataEnumerator.MoveNext() || !valueEnumerator.MoveNext())
                {
                    break;
                }

                Assert.That.ObjectEquals(dataEnumerator.Current.Key, valueEnumerator.Current.Key);
                Assert.That.ObjectEquals(dataEnumerator.Current.Value, valueEnumerator.Current.Value);
            }
        }

        [TestMethod]
        public async Task ReadMultipleAsync()
        {
            // Arrange
            var data = Data.ToDictionary(x => (string)x[0], x => x[1]);
            var readWrite = GetReadWrite();

            // Act
            var value = await readWrite.ReadAsync(data.Keys);

            // Assert
            var dataEnumerator = data.GetEnumerator();
            var valueEnumerator = value.GetEnumerator();

            Assert.AreEqual(data.Count, value.Count);

            while (true)
            {
                if (!dataEnumerator.MoveNext() || !valueEnumerator.MoveNext())
                {
                    break;
                }

                Assert.That.ObjectEquals(dataEnumerator.Current.Key, valueEnumerator.Current.Key);
                Assert.That.ObjectEquals(dataEnumerator.Current.Value, valueEnumerator.Current.Value);
            }
        }

        [TestMethod]
        public void ReadValue()
        {
            var readWrite = GetReadWrite();

            foreach (var item in Data)
            {
                // Arrange
                var ioName = item[0] as string;
                var expectedValue = item[1];

                // Act
                var value = readWrite.Read(ioName);

                // Assert
                Assert.That.ObjectEquals(expectedValue, value);
            }
        }

        [TestMethod]
        public async Task ReadValueAsync()
        {
            var readWrite = GetReadWrite();

            foreach (var item in Data)
            {
                // Arrange
                var ioName = item[0] as string;
                var expectedValue = item[1];

                // Act
                var value = await readWrite.ReadAsync(ioName);

                // Assert
                Assert.That.ObjectEquals(expectedValue, value);
            }
        }

        protected override IMonitor GetMonitor()
        {
            throw new System.NotImplementedException();
        }

        protected void ReadValueGenericHelper<T>(string ioName, T expectedValue)
        {
            // Arrange
            var readWrite = GetReadWrite();

            // Act
            var value = readWrite.Read<T>(ioName);

            // Assert
            Assert.That.ObjectEquals(expectedValue, value);
        }

        protected async Task ReadValueGenericHelperAsync<T>(string ioName, T expectedValue)
        {
            // Arrange
            var readWrite = GetReadWrite();

            // Act
            var value = await readWrite.ReadAsync<T>(ioName);

            // Assert
            Assert.That.ObjectEquals(expectedValue, value);
        }
    }
}