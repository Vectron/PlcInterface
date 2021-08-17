using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TwinCAT.Ads;

namespace PlcInterface.Ads.Tests
{
    [TestClass]
    public class TcAdsClientExtensionTests
    {
        [TestMethod]
        public void ValidateConnectionReturnsTheConnectionWhenSuccess()
        {
            // Arrange
            var stateInfo = new StateInfo(AdsState.Run, 0);
            var clientMock = new Mock<IAdsConnection>();
            _ = clientMock.SetupGet(x => x.IsConnected).Returns(true);
            _ = clientMock.Setup(x => x.TryReadState(out stateInfo)).Returns(AdsErrorCode.NoError);

            // Act
            var client = clientMock.Object.ValidateConnection();

            // Assert
            Assert.AreEqual(clientMock.Object, client);
        }

        [TestMethod]
        public void ValidateConnectionThrowsArgumentNullExceptionWhenPassedANull() =>
            _ = Assert.ThrowsException<ArgumentNullException>(() => TcAdsClientExtension.ValidateConnection(null!));

        [TestMethod]
        public void ValidateConnectionThrowsInvalidOperationExceptionWhenItCantReadState()
        {
            // Arrange
            var stateInfo = default(StateInfo);
            var clientMock = new Mock<IAdsConnection>();
            _ = clientMock.SetupGet(x => x.IsConnected).Returns(true);
            _ = clientMock.Setup(x => x.TryReadState(out stateInfo)).Returns(AdsErrorCode.InternalError);

            // Act
            _ = Assert.ThrowsException<InvalidOperationException>(clientMock.Object.ValidateConnection);

            // Assert
        }

        [TestMethod]
        public void ValidateConnectionThrowsInvalidOperationExceptionWhenNotConnected()
        {
            // Arrange
            var stateInfo = default(StateInfo);
            var clientMock = new Mock<IAdsConnection>();
            _ = clientMock.SetupGet(x => x.IsConnected).Returns(false);
            _ = clientMock.Setup(x => x.TryReadState(out stateInfo)).Returns(AdsErrorCode.InternalError);

            // Act
            _ = Assert.ThrowsException<InvalidOperationException>(clientMock.Object.ValidateConnection);

            // Assert
        }

        [TestMethod]
        public void ValidateConnectionThrowsInvalidOperationExceptionWhenStateIsNotAdsStateRun()
        {
            // Arrange
            var stateInfo = new StateInfo(AdsState.Stop, 0);
            var clientMock = new Mock<IAdsConnection>();
            _ = clientMock.SetupGet(x => x.IsConnected).Returns(true);
            _ = clientMock.Setup(x => x.TryReadState(out stateInfo)).Returns(AdsErrorCode.NoError);

            // Act
            _ = Assert.ThrowsException<InvalidOperationException>(clientMock.Object.ValidateConnection);

            // Assert
        }
    }
}