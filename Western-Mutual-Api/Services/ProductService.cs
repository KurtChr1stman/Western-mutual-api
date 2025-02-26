using Microsoft.EntityFrameworkCore;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using Western_Mutual_Api.Data;
using Western_Mutual_Api.Interfaces;
using Western_Mutual_Api.Models;
using Western_Mutual_Api.Services;
using DbUpdateException = Microsoft.EntityFrameworkCore.DbUpdateException;

namespace Western_Mutual_Api.Repository
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;
        public ProductService(ApplicationDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public ICollection<Product> GetProducts()
        {
            // call to the database and return the full list of products ordered by SKU
            return _context.Products.OrderBy(p => p.SKU).ToList();
        }

        public Product GetProductBySku(string productSku)
        {
            try
            {
                // call to the database and find the first result that matches the SKU
                // since skus are unique there will only be one item matching that sku
                return _context.Products.FirstOrDefault(p => p.SKU == productSku);
            } 
            catch (Exception ex)
            {
                // write an error message in the console and return null if any error happens
                Console.WriteLine($"Error fetching product: {ex.Message}");
                return null;
            }
        }

        public async Task<Product> CreateProduct(Product product)
        {
            // validate product is not null, if it is throw an arguement null exception
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            try
            {
                // add the product to the products table
                _context.Products.Add(product);
                // save the changes
                await _context.SaveChangesAsync();
                // Return the newly created product
                return product; 
            }
            catch (DbUpdateException dbEx)
            {
                // Log detailed information about the database update exception
                Console.WriteLine($"Database update error: {dbEx.InnerException?.Message}");
                throw new Exception("An error occurred while saving the product to the database.", dbEx);
            }
            catch (Exception ex)
            {
                // Log general errors
                Console.WriteLine($"Error adding product: {ex.Message}");
                throw new Exception("An error occurred while saving the product.", ex);
            }
        }


        public Product GetProductTrimToUpper(Product productCreate)
        {
            // checks the database for a product by the name exist
            return GetProducts()
                .Where(c => c.Title.Trim().ToUpper() == productCreate.Title.Trim().ToUpper())
                .FirstOrDefault();
        }

        public async Task<bool> DeleteProductBySku(string productSku)
        {
            // check for valid request
            if (string.IsNullOrWhiteSpace(productSku))
            {
                throw new ArgumentException(nameof(productSku), "Product SKU cannot be null or empty");
            }

            try
            {
                // validate that the product exists and return that product into a variable
                var product = await _context.Products.FindAsync(productSku);

                if (product == null)
                {
                    // if it is empty, the product with that SKU doesnt exist
                    throw new KeyNotFoundException($"Product with SKU {productSku} not found.");
                }

                // call for the delete and save the changes
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                // if everything was successful, return truw
                return true;
            }
            catch (DbUpdateException dbEx)
            {
                // return false any database errors.
                Console.WriteLine($"Database update error: {dbEx.InnerException?.Message}");
                return false;
            }
            catch (Exception ex)
            {
                // Log general errors and return false
                Console.WriteLine($"Error deleting product: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateProduct(Product updatedProduct)
        {
            // validate proper request
            if (updatedProduct == null)
            {
                throw new ArgumentNullException(nameof(updatedProduct));
            }

            try
            {
                // get the existing product you are about to update
                var existingProduct = await _context.Products.FindAsync(updatedProduct.SKU);
                // check if the product exists, if it doesnt, theres no product to update and return error
                if (existingProduct == null)
                {
                    throw new KeyNotFoundException($"Product with SKU {updatedProduct.SKU} not found");
                }

                // put the oldbuyerId from the existing product into a variable for verification
                string oldBuyerId = existingProduct.BuyerId;
                Buyer oldBuyer = await _context.Buyers.FindAsync(oldBuyerId);
                // put the new buyer Id into a newbuyer variable for validation
                string newBuyerId = updatedProduct.BuyerId;

                // if the exisitng product active doesnt match the new update
                if (existingProduct.Active != updatedProduct.Active)
                {
                    if (oldBuyer != null)
                    {
                        // Notify the buyer of deactivation if the product is being set to inactive
                        if (!updatedProduct.Active)
                        {
                            await _notificationService.NotifyBuyerAsync(oldBuyer.Email, "Your product has been deactivated.");
                        }
                    }
                }

                // check if the old and new buyer dont match
                if (oldBuyerId != newBuyerId)
                {
                    // Notify the old buyer they have been unassigned
                    if (oldBuyer != null)
                    {
                        await _notificationService.NotifyBuyerAsync(oldBuyer.Email, "You have been unassigned from a product.");
                    }

                    // Notify the new buyer
                    var newBuyer = await _context.Buyers.FindAsync(newBuyerId);
                    if (newBuyer != null)
                    {
                        await _notificationService.NotifyBuyerAsync(newBuyer.Email, "You have been assigned to a product.");
                    }
                }

                // update the existing product with the new information
                existingProduct.Title = updatedProduct.Title;
                existingProduct.Description = updatedProduct.Description;
                existingProduct.Active = updatedProduct.Active;
                existingProduct.BuyerId = updatedProduct.BuyerId;

                // save changes and return true
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating product: {ex.Message}");
                return false;
            }
        }
    }
}
