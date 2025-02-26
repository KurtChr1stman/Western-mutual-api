using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Western_Mutual_Api.Data;
using Western_Mutual_Api.Interfaces;
using Western_Mutual_Api.Models;
using Western_Mutual_Api.Repository;
using Xunit;

namespace Western_Mutual_Api.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly ApplicationDbContext _context;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _notificationServiceMock = new Mock<INotificationService>();
            _productService = new ProductService(_context, _notificationServiceMock.Object);
        }


        [Fact]
        public async Task CreateProduct_ShouldAddProductToDatabase()
        {
            // Arrange
            var product = new Product { SKU = "SKU001", Title = "Product 1", Active = true, BuyerId = "BuyerId001" };

            // Act
            var createdProduct = await _productService.CreateProduct(product);

            // Assert
            Assert.NotNull(createdProduct);
            Assert.Equal(product.SKU, createdProduct.SKU);
            Assert.Equal(1, _context.Products.Count());
        }

        [Fact]
        public async Task DeleteProductBySku_ShouldRemoveProductFromDatabase()
        {
            // Arrange
            var product = new Product { SKU = "SKU001", Title = "Product 1", Active = true, BuyerId = "BuyerId001" };
            await _productService.CreateProduct(product);

            // Act
            var result = await _productService.DeleteProductBySku("SKU001");

            // Assert
            Assert.True(result);
            Assert.Empty(_context.Products);
        }

        [Fact]
        public async Task UpdateProduct_ShouldNotifyBuyerWhenActiveStatusChanges()
        {
            // Arrange
            var product = new Product { SKU = "SKU001", Title = "Product 1", Active = true, BuyerId = "BuyerId001" };
            var buyer = new Buyer { Id = "BuyerId001", Name = "Mr/Mrs Buyer", Email = "buyer@email.com" };
            _context.Buyers.Add(buyer);
            await _context.SaveChangesAsync();
            await _productService.CreateProduct(product);

            var updatedProduct = new Product { SKU = "SKU001", Title = "Product 1", Active = false, BuyerId = "BuyerId001" };

            // Act
            var result = await _productService.UpdateProduct(updatedProduct);

            // Assert
            Assert.True(result);
            _notificationServiceMock.Verify(ns => ns.NotifyBuyerAsync(buyer.Email, "Your product has been deactivated."), Times.Once);
        }

        [Fact]
        public async Task UpdateProduct_ShouldNotifyOldAndNewBuyerWhenBuyerChanges()
        {
            // Arrange
            var oldBuyer = new Buyer { Id = "OldBuyerId", Name = "Old Buyer", Email = "oldbuyer@email.com" };
            var newBuyer = new Buyer { Id = "NewBuyerId", Name = "New Buyer", Email = "newbuyer@email.com" };
            _context.Buyers.Add(oldBuyer);
            _context.Buyers.Add(newBuyer);
            await _context.SaveChangesAsync();

            var product = new Product { SKU = "SKU001", Title = "Product 1", Active = true, BuyerId = "OldBuyerId" };
            await _productService.CreateProduct(product);

            var updatedProduct = new Product { SKU = "SKU001", Title = "Product 1", Active = true, BuyerId = "NewBuyerId" };

            // Act
            var result = await _productService.UpdateProduct(updatedProduct);

            // Assert
            Assert.True(result);
            _notificationServiceMock.Verify(ns => ns.NotifyBuyerAsync(oldBuyer.Email, "You have been unassigned from a product."), Times.Once);
            _notificationServiceMock.Verify(ns => ns.NotifyBuyerAsync(newBuyer.Email, "You have been assigned to a product."), Times.Once);
        }

        [Fact]
        public async Task GetProductBySku_ShouldReturnProduct_WhenExists()
        {
            // Arrange
            var product = new Product { SKU = "SKU001", Title = "Product 1", Active = true, BuyerId = "BuyerId001" };
            await _productService.CreateProduct(product);

            // Act
            var result = _productService.GetProductBySku("SKU001");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(product.SKU, result.SKU);
        }

        [Fact]
        public async Task GetProducts_ShouldReturnAllProducts()
        {
            // Arrange
            var product1 = new Product { SKU = "SKU001", Title = "Product 1", Active = true, BuyerId = "BuyerId001" };
            var product2 = new Product { SKU = "SKU002", Title = "Product 2", Active = true, BuyerId = "BuyerId002" };
            await _productService.CreateProduct(product1);
            await _productService.CreateProduct(product2);

            // Act
            var products = _productService.GetProducts();

            // Assert
            Assert.Equal(2, products.Count);
        }
    }
}
