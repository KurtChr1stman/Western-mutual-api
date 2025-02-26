using Western_Mutual_Api.Models;

namespace Western_Mutual_Api.Interfaces
{
    public interface IProductService
    {
        ICollection<Product> GetProducts();
        Product GetProductBySku(string productSku);
        Task<Product> CreateProduct(Product product);
        Product GetProductTrimToUpper(Product productCreate);
        Task<bool> DeleteProductBySku(string productSku);
        Task<bool> UpdateProduct(Product updatedProduct);
    }
}
