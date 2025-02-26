using System;
using System.Threading.Tasks;
using Western_Mutual_Api.Services;
using Xunit;

namespace Western_Mutual_Api.Tests
{
    public class NotificationServiceTests
    {
        private readonly NotificationService _notificationService;

        public NotificationServiceTests()
        {
            _notificationService = new NotificationService();
        }

        [Fact]
        public async Task NotifyBuyerAsync_ShouldExecuteWithoutException()
        {
            // Arrange
            string testEmail = "testbuyer@email.com";
            string testMessage = "This is a notification test.";

            // Act
            var exception = await Record.ExceptionAsync(() => _notificationService.NotifyBuyerAsync(testEmail, testMessage));

            // Assert
            Assert.Null(exception); // Ensure no exceptions were thrown
        }
    }
}
