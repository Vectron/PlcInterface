﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Subjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace PlcInterface.Abstraction.Tests
{
    [TestClass]
    [SuppressMessage("Usage", "MA0032:Use an overload with a CancellationToken argument", Justification = "We dont want to use the overload with cancelation token")]
    public class IObservableExtensionsTests
    {
        [TestMethod]
        public void WhereNotNullOnlyReturnsItemsThatAreNotNull()
        {
            // Arrange
            using var subject = new Subject<object?>();
            var observerMock = new Mock<IObserver<object?>>();

            // Act
            using var subscription = subject.WhereNotNull().Subscribe(observerMock.Object);
            subject.OnNext(null);
            subject.OnNext(null);
            subject.OnNext(true);
            subject.OnNext(null);
            subject.OnNext(null);
            subject.OnNext(null);
            subject.OnNext(null);

            // Assert
            observerMock.Verify(x => x.OnNext(It.IsNotNull<object>()), Times.Once);
        }

        [TestMethod]
        public void WhereNotNullPassesOnCompletedThroug()
        {
            // Arrange
            using var subject = new Subject<object?>();
            var observerMock = new Mock<IObserver<object?>>();
            var expectedException = new InvalidOperationException();

            // Act
            using var subscription = subject.WhereNotNull().Subscribe(observerMock.Object);
            subject.OnNext(null);
            subject.OnNext(true);
            subject.OnNext(null);
            subject.OnError(expectedException);

            // Assert
            observerMock.Verify(x => x.OnError(It.Is<InvalidOperationException>(x => x == expectedException)), Times.Once);
        }

        [TestMethod]
        public void WhereNotNullPassesOnErrorsThroug()
        {
            // Arrange
            using var subject = new Subject<object?>();
            var observerMock = new Mock<IObserver<object?>>();

            // Act
            using var subscription = subject.WhereNotNull().Subscribe(observerMock.Object);
            subject.OnNext(null);
            subject.OnNext(true);
            subject.OnNext(null);
            subject.OnCompleted();

            // Assert
            observerMock.Verify(x => x.OnCompleted(), Times.Once);
        }
    }
}