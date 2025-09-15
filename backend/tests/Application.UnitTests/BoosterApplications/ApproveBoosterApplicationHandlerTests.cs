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
    public class ApproveBoosterApplicationHandlerTests
    {
        private Mock<IApplicationDbContext> _mockContext;
        private Mock<IMapper> _mockMapper;
        private ApproveBoosterApplicationHandler _handler;

        [SetUp]
        public void Setup()
        {
            _mockContext = new Mock<IApplicationDbContext>();
            _mockMapper = new Mock<IMapper>();
            _handler = new ApproveBoosterApplicationHandler(_mockContext.Object, _mockMapper.Object);
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
            
            _mockMapper
                .Setup(m => m.Map<BoosterApplicationDto>(It.IsAny<BoosterApplication>()))
                .Returns<BoosterApplication>(b => new BoosterApplicationDto
                {
                    Id = b.Id,
                    Status = b.Status
                });

            var command = new ApproveBoosterApplicationCommand { ApplicationId = applicationId };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(boosterApplication.Status, Is.EqualTo(ApplicationStatus.Approved));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Status, Is.EqualTo(ApplicationStatus.Approved));
            Assert.That(result.Id, Is.EqualTo(applicationId));
            _mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockMapper.Verify(m => m.Map<BoosterApplicationDto>(It.Is<BoosterApplication>(b => b == boosterApplication)), Times.Once);
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