using Dapper;
using Microsoft.Data.SqlClient;
using NuGet.Protocol.Plugins;
using System.Data;
using System.Drawing.Text;
using Western_Mutual_Api.Interfaces;
using Western_Mutual_Api.Interfaces.Dapper;
using Western_Mutual_Api.Models;

namespace Western_Mutual_Api.Services.Dapper
{
    public class ProductDapperService : IProductDapperService
    {
        private readonly string _connectionString;
        private readonly INotificationService _notificationService;

        public ProductDapperService(IConfiguration configuration, INotificationService notificationService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _notificationService = notificationService;
        }
        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<Product>> GetProducts()
        {
            using var connection = CreateConnection();

            try
            {
                var query = "SELECT * FROM Products";
                return await connection.QueryAsync<Product>(query);
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"SQL Error: {sqlEx.Message}");
                throw new Exception("An error occured while fetching products from the databbase");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw new Exception("An error ocured while fetching products.");
            }
            
        }

        public async Task<Product> GetProductBySku(string sku)
        {
            using var connection = CreateConnection();

            try
            {
                var query = @"SELECT * FROM Products WHERE Products.SKU = @SKU";
                return await connection.QueryFirstOrDefaultAsync<Product>(query, new { SKU = sku });
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"SQL Error: {sqlEx.Message}");
                throw new Exception("An error occured while fetching product from the databbase", sqlEx);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw new Exception("An error ocured while fetching product.", ex);
            }
        }

        public async Task<Product> CreateProduct(Product product)
        {
            using var connection = CreateConnection();
            try
            {
                var query = @"INSERT INTO Products (SKU, Title, Description, Active, BuyerId)
                          Values (@SKU, @Title, @Description, @Active, @BuyerId);
                          Select * from Products WHERE SKU = @SKU;";
                return await connection.QuerySingleAsync<Product>(query, product);
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"SQL Error: {sqlEx.Message}");
                throw new Exception("An error occured while creating product in the databbase", sqlEx);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw new Exception("An error ocured while creating product.", ex);
            }
        }

        public async Task<bool> DeleteProductBySku(string sku)
        {
            try
            {
                using var connection = CreateConnection();
                var query = "DELETE FROM Products WHERE SKU = @SKU";
                var rowsAffected = await connection.ExecuteAsync(query, new { SKU = sku });
                return rowsAffected > 0;
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"SQL Error: {sqlEx.Message}");
                throw new Exception("An error occured while deleting product in the databbase", sqlEx);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw new Exception("An error ocured while deleting product.", ex);
            }

        }

        public async Task<bool> UpdateProduct(Product updatedProduct)
        {
            using var connection = CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                var existingProduct = await connection.QueryFirstOrDefaultAsync<Product>(
                    "SELECT * from Products WHERE SKU = @SKU",
                    new { updatedProduct.SKU },
                    transaction);

                if (existingProduct == null)
                {
                    throw new KeyNotFoundException($"Product with SKU {updatedProduct.SKU} not found.");
                }

                string oldBuyerId = existingProduct.BuyerId;
                string newBuyerId = updatedProduct.BuyerId;
                bool wasActive = existingProduct.Active;
                bool isActiveNow = updatedProduct.Active;

                var query = @"UPDATE Products
                            SET Title = @Title,
                                Description = @Description,
                                Active = @Active,
                                BuyerId = @BuyerId
                            WHERE SKU = @SKU";

                int rowsAffected = await connection.ExecuteAsync(query, updatedProduct, transaction);

                if(rowsAffected > 0)
                {
                    var notificationTasks = new List<Task>();

                    if (wasActive && !isActiveNow)
                    {
                        notificationTasks.Add(_notificationService.NotifyBuyerAsync(oldBuyerId, "Your product has been deactivated."));
                    }

                    if (oldBuyerId != newBuyerId)
                    {
                        notificationTasks.Add(_notificationService.NotifyBuyerAsync(oldBuyerId, "You have been unassinged from a product."));
                        notificationTasks.Add(_notificationService.NotifyBuyerAsync(newBuyerId, "You have been assigned to a product"));
                    }

                    await Task.WhenAll(notificationTasks);
                }

                transaction.Commit();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"Error updating product: {updatedProduct.SKU} - {ex.Message}");
                return false;
            }
        }
    }
}
