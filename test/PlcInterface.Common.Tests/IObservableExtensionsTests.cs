using System.Reactive.Subjects;
using Moq;

namespace PlcInterface.Common.Tests;

[TestClass]
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
        subject.OnNext(value: null);
        subject.OnNext(value: null);
        subject.OnNext(value: true);
        subject.OnNext(value: null);
        subject.OnNext(value: null);
        subject.OnNext(value: null);
        subject.OnNext(value: null);

        // Assert
        observerMock.Verify(x => x.OnNext(It.IsNotNull<object>()), Times.Once);
    }

    [TestMethod]
    public void WhereNotNullPassesOnCompletedThrough()
    {
        // Arrange
        using var subject = new Subject<object?>();
        var observerMock = new Mock<IObserver<object?>>();
        var expectedException = new InvalidOperationException();

        // Act
        using var subscription = subject.WhereNotNull().Subscribe(observerMock.Object);
        subject.OnNext(value: null);
        subject.OnNext(value: true);
        subject.OnNext(value: null);
        subject.OnError(expectedException);

        // Assert
        observerMock.Verify(x => x.OnError(It.Is<InvalidOperationException>(x => x == expectedException)), Times.Once);
    }

    [TestMethod]
    public void WhereNotNullPassesOnErrorsThrough()
    {
        // Arrange
        using var subject = new Subject<object?>();
        var observerMock = new Mock<IObserver<object?>>();

        // Act
        using var subscription = subject.WhereNotNull().Subscribe(observerMock.Object);
        subject.OnNext(value: null);
        subject.OnNext(value: true);
        subject.OnNext(value: null);
        subject.OnCompleted();

        // Assert
        observerMock.Verify(x => x.OnCompleted(), Times.Once);
    }
}
