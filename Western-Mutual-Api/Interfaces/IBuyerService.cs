using Western_Mutual_Api.Models;

namespace Western_Mutual_Api.Interfaces
{
    public interface IBuyerService
    {
        ICollection<Buyer> GetBuyers();
    }
}
