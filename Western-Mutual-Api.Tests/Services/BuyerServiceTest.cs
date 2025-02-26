using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Western_Mutual_Api.Data;
using Western_Mutual_Api.Models;
using Western_Mutual_Api.Services;
using Xunit;

namespace Western_Mutual_Api.Tests
{
    public class BuyerServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly BuyerService _buyerService;

        public BuyerServiceTests()
        {
            // Set up in-memory database for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _buyerService = new BuyerService(_context);
        }

        [Fact]
        public async Task GetBuyers_ShouldReturnAllBuyers()
        {
            // Arrange
            var buyers = new List<Buyer>
            {
                new Buyer { Id = "Buyer1", Name = "Alice", Email = "alice@email.com" },
                new Buyer { Id = "Buyer2", Name = "Bob", Email = "bob@email.com" }
            };

            _context.Buyers.AddRange(buyers);
            await _context.SaveChangesAsync();

            // Act
            var result = _buyerService.GetBuyers();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, b => b.Name == "Alice");
            Assert.Contains(result, b => b.Name == "Bob");
        }
    }
}
