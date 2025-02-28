using Microsoft.AspNetCore.Mvc;
using Western_Mutual_Api.Interfaces.Dapper;
using Western_Mutual_Api.Models;
using Western_Mutual_Api.Services.Dapper;

namespace Western_Mutual_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductDapperController : ControllerBase
    {
        private readonly IProductDapperService _productDapperService;
        public ProductDapperController(IProductDapperService productDapperService)
        {
            _productDapperService = productDapperService;
        }

        [HttpGet]
        public Task<IEnumerable<Product>> GetProducts()
        {
            var products = _productDapperService.GetProducts();
            return products;
        }

        [HttpGet("{sku}")]
        public async Task<Product> GetProductBySku(string sku)
        {
            var product = await _productDapperService.GetProductBySku(sku);
            return product;
        }

        [HttpPost]
        public async Task<Product> CreateProduct([FromBody] Product product)
        {
            var newProduct = await _productDapperService.CreateProduct(product);
            return newProduct;
        }

        [HttpDelete("{sku}")]
        public async Task<bool> DeleteProductBySku(string sku)
        {
            return await _productDapperService.DeleteProductBySku(sku);
        }

        [HttpPut]
        public async Task<bool> UpdateProduct([FromBody] Product updatedProduct)
        {
            var updateSuccess = await _productDapperService.UpdateProduct(updatedProduct);
            return updateSuccess;
        }
    }
}
