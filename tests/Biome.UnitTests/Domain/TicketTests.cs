using Biome.Domain.Support;
using Biome.Domain.Support.Enums;
using FluentAssertions;

namespace Biome.UnitTests.Domain;

public class TicketTests
{
    [Fact]
    public void Create_ShouldInitializeTicket_WhenArgumentsAreValid()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var subject = "Test Ticket";
        var initialMessage = "I have a problem";

        // Act
        var result = Ticket.Create(customerId, subject, initialMessage);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.CustomerId.Should().Be(customerId);
        result.Value.Subject.Should().Be(subject);
        result.Value.Status.Should().Be(SupportTicketStatus.Open);
        result.Value.Messages.Should().HaveCount(1);
    }
}
