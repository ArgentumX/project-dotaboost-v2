using Application.BoosterApplications;
using Application.Boosters.Commands.ApproveBoosterApplication;
using Application.Boosters.Commands.RejectBoosterApplication;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Common.Enum;
using Domain.Entities;
using Moq;
using Moq.EntityFrameworkCore;

namespace Application.UnitTests.BoosterApplications;

[TestFixture]
public class RejectBoosterApplicationHandlerTests : CommandTestsBase
{
        private Mock<IApplicationDbContext> _mockContext = null!;
        private RejectBoosterApplicationHandler _handler = null!;

        [SetUp]
        public void SetUp()
        {
            _mockContext = new Mock<IApplicationDbContext>();

            _mockContext
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _handler = new RejectBoosterApplicationHandler(_mockContext.Object, RealMapper);
        }

        [Test]
        public async Task Handle_ValidApplicationId_RejectsApplicationAndReturnsDto()
        {
            // Arrange
            var applicationId = Guid.NewGuid();
            var entity = new BoosterApplication
            {
                Id = applicationId,
                Status = ApplicationStatus.Pending
            };
            var applications = new List<BoosterApplication> { entity };
            _mockContext.Setup(x => x.BoosterApplications).ReturnsDbSet(applications);

            var command = new RejectBoosterApplicationCommand { ApplicationId = applicationId };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(entity.Status, Is.EqualTo(ApplicationStatus.Rejected));
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<BoosterApplicationDto>());
            Assert.That(result.Status, Is.EqualTo(ApplicationStatus.Rejected));
            Assert.That(result.Id, Is.EqualTo(applicationId));

            _mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void Handle_InvalidApplicationId_ThrowsNotFoundException()
        {
            // Arrange
            var applications = new List<BoosterApplication>(); 
            _mockContext.Setup(x => x.BoosterApplications).ReturnsDbSet(applications);

            var command = new RejectBoosterApplicationCommand { ApplicationId = Guid.NewGuid() };

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () => await _handler.Handle(command, CancellationToken.None));

            _mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public void Handle_WhenStatusNotPending_ThrowsBadRequestException()
        {
            // Arrange
            var existing = new BoosterApplication
            {
                Id = Guid.NewGuid(),
                Status = ApplicationStatus.Approved // не Pending
            };
            var applications = new List<BoosterApplication> { existing };
            _mockContext.Setup(x => x.BoosterApplications).ReturnsDbSet(applications);

            var command = new RejectBoosterApplicationCommand { ApplicationId = existing.Id };

            // Act & Assert
            Assert.ThrowsAsync<BadRequestException>(async () => await _handler.Handle(command, CancellationToken.None));
            _mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
}