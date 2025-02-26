using Western_Mutual_Api.Data;
using Western_Mutual_Api.Interfaces;
using Western_Mutual_Api.Models;

namespace Western_Mutual_Api.Services
{
    public class BuyerService : IBuyerService
    {
        private readonly ApplicationDbContext _context;
        public BuyerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public ICollection<Buyer> GetBuyers()
        {
            return _context.Buyers.OrderBy(b => b.Id).ToList();
        }
    }
}
