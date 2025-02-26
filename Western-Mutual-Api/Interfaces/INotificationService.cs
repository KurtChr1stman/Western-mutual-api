namespace Western_Mutual_Api.Interfaces
{
    public interface INotificationService
    {
        Task NotifyBuyerAsync(string buyerEmail, string message);
    }
}
