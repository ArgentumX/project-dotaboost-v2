using Application.BoosterApplications;
using Application.Boosters.Commands.ApproveBoosterApplication;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Common.Enum;
using Domain.Entities;
using Moq;
using Moq.EntityFrameworkCore;

namespace Application.UnitTests.BoosterApplications;

    [TestFixture]
    public class ApproveBoosterApplicationHandlerTests : CommandTestsBase
    {
        private Mock<IApplicationDbContext> _mockContext;
        private ApproveBoosterApplicationHandler _handler;

        [SetUp]
        public void Setup()
        {
            _mockContext = new Mock<IApplicationDbContext>();
            _handler = new ApproveBoosterApplicationHandler(_mockContext.Object, RealMapper);
        }

        [Test]
        public async Task Handle_ValidApplicationId_ApprovesApplication()
        {
            // Arrange
            var applicationId = Guid.NewGuid();
            var boosterApplication = new BoosterApplication() { Id = applicationId };
            var applications = new List<BoosterApplication> { boosterApplication };
            
            _mockContext
                .Setup(x => x.BoosterApplications)
                .ReturnsDbSet(applications);
            
            _mockContext
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var command = new ApproveBoosterApplicationCommand { ApplicationId = applicationId };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(boosterApplication.Status, Is.EqualTo(ApplicationStatus.Approved));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Status, Is.EqualTo(ApplicationStatus.Approved));
            Assert.That(result.Id, Is.EqualTo(applicationId));
            _mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.That(result, Is.TypeOf<BoosterApplicationDto>());
        }
    

        [Test]
        public void Handle_InvalidApplicationId_ThrowsNotFoundException()
        {
            // Arrange
            var invalidId = Guid.NewGuid();
            var emptyApplications = new List<BoosterApplication>();
            
            _mockContext
                .Setup(x => x.BoosterApplications)
                .ReturnsDbSet(emptyApplications);

            var command = new ApproveBoosterApplicationCommand { ApplicationId = invalidId };

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(() => 
                _handler.Handle(command, CancellationToken.None));
        }
    }