using Western_Mutual_Api.Models;

namespace Western_Mutual_Api.Interfaces.Dapper
{
    public interface IProductDapperService
    {
        Task<IEnumerable<Product>> GetProducts();
        Task<Product> CreateProduct(Product product);
        Task<bool> DeleteProductBySku(string sku);
        Task<Product> GetProductBySku(string sku);
        Task<bool> UpdateProduct(Product updatedProduct);
    }
}
