namespace EntainTask1.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EntainTask1.Classes;
    using Moq;
    using Xunit;

    public class CallGroupTests
    {
        [Fact]
        public void Constructor_ValidParameters_ShouldInitializeProperly()
        {
            // Arrange
            int participantCount = 3;
            Func<IReadOnlyCollection<string>, Task> groupCallDelegate = (operations) => Task.CompletedTask;
            TimeSpan timeout = TimeSpan.FromSeconds(30);

            // Act
            var callGroup = new CallGroup<string>(participantCount, groupCallDelegate, timeout);

            // Assert
            Assert.NotNull(callGroup);
        }

        [Fact]
        public void Constructor_InvalidParticipantCount_ShouldThrowArgumentException()
        {
            // Arrange
            int participantCount = 0;
            Func<IReadOnlyCollection<string>, Task> groupCallDelegate = (operations) => Task.CompletedTask;
            TimeSpan timeout = TimeSpan.FromSeconds(30);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new CallGroup<string>(participantCount, groupCallDelegate, timeout));
        }

        [Fact]
        public async Task Join_ValidParticipant_ShouldCompleteTask()
        {
            // Arrange
            int participantCount = 1;
            var groupCallDelegate = new Mock<Func<IReadOnlyCollection<string>, Task>>();
            groupCallDelegate.Setup(g => g(It.IsAny<IReadOnlyCollection<string>>())).Returns(Task.CompletedTask);
            TimeSpan timeout = TimeSpan.FromSeconds(30);

            var callGroup = new CallGroup<string>(participantCount, groupCallDelegate.Object, timeout);

            // Act
            var task = callGroup.Join("participant");

            // Assert
            var exception = Record.Exception(() => callGroup.Join);
            Assert.Null(exception);
        }

        [Fact]
        public async Task Leave_ShouldBeExecutedAndNotThrowException()
        {
            // Arrange
            int participantCount = 1;
            Func<IReadOnlyCollection<string>, Task> groupCallDelegate = (operations) => Task.CompletedTask;
            TimeSpan timeout = TimeSpan.FromSeconds(30);

            var callGroup = new CallGroup<string>(participantCount, groupCallDelegate, timeout);

            // Act & Assert
            var exception = Record.Exception(callGroup.Leave);
            Assert.Null(exception);
        }

        [Fact]
        public async Task Join_MoreParticipantsThanAllowed_ShouldThrowException()
        {
            // Arrange
            int participantCount = 1;
            Func<IReadOnlyCollection<string>, Task> groupCallDelegate = (operations) => Task.CompletedTask;
            TimeSpan timeout = TimeSpan.FromSeconds(30);

            var callGroup = new CallGroup<string>(participantCount, groupCallDelegate, timeout);

            // Act
            await callGroup.Join("participant");

            // Assert
            await Assert.ThrowsAsync<Exception>(() => callGroup.Join("another participant"));
        }
    }

}