using Application.BoosterApplications;
using Application.BoosterApplications.Commands.CreateBoosterApplication;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Common.Enum;
using Domain.Entities;
using Moq;
using Moq.EntityFrameworkCore;


namespace Application.UnitTests.BoosterApplications;

[TestFixture]
public class CreateBoosterApplicationHandlerTests : CommandTestsBase
{
    private Mock<IApplicationDbContext> _mockContext = null!;
    private CreateBoosterApplicationHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _mockContext = new Mock<IApplicationDbContext>();

        _mockContext
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        
        _handler = new CreateBoosterApplicationHandler(_mockContext.Object, RealMapper);
    }

    [Test]
    public async Task Handle_ValidRequest_AddsApplicationAndReturnsDto()
    {
        // Arrange
        var senderId = Guid.NewGuid();
        var applications = new List<BoosterApplication>(); 
        _mockContext.Setup(x => x.BoosterApplications).ReturnsDbSet(applications);

        var command = new CreateBoosterApplicationCommand
        {
            SenderId = senderId,
            Motivation = "Motivation",
            Contact = "discord#1234",
            SteamAccountLink = "https://steamcommunity.com/id/someone"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert 
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<BoosterApplicationDto>());
        
        _mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        
        Assert.That(result.UserId, Is.EqualTo(senderId));
        Assert.That(result.Motivation, Is.EqualTo(command.Motivation));
        Assert.That(result.Contact, Is.EqualTo(command.Contact));
    }

    [Test]
    public void Handle_WhenUserHasActivePendingApplication_ThrowsBadRequestException()
    {
        // Arrange
        var senderId = Guid.NewGuid();
        var existing = new BoosterApplication
        {
            Id = Guid.NewGuid(),
            UserId = senderId,
            Status = ApplicationStatus.Pending
        };
        var applications = new List<BoosterApplication> { existing };
        _mockContext.Setup(x => x.BoosterApplications).ReturnsDbSet(applications);

        var command = new CreateBoosterApplicationCommand
        {
            SenderId = senderId,
            Motivation = "Motivation",
            Contact = "contact",
            SteamAccountLink = "https://steamcommunity.com/id/xxx"
        };

        // Act & Assert
        Assert.ThrowsAsync<BadRequestException>(async () => await _handler.Handle(command, CancellationToken.None));
        _mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
