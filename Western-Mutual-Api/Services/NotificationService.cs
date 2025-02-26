using Western_Mutual_Api.Interfaces;

namespace Western_Mutual_Api.Services
{
    public class NotificationService : INotificationService
    {
        public async Task NotifyBuyerAsync(string buyerEmail, string message)
        {
            Console.WriteLine($"Notifying {buyerEmail}: {message}");
            await Task.CompletedTask;
        }
    }
}
