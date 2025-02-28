using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
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
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                var products = await _productDapperService.GetProducts();
                if (products == null || !products.Any())
                {
                    return NotFound("No Products found.");
                }

                return Ok(products);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving products: {ex.Message}");
                return StatusCode(500, "An Error occured while fetching the products.");
            }
        }

        [HttpGet("{sku}")]
        public async Task<IActionResult> GetProductBySku(string sku)
        {
            try
            {
                var product = await _productDapperService.GetProductBySku(sku);

                if (product == null)
                {
                    return NotFound($"No product found with the SKU: {sku}");
                }

                return Ok(product);
            }
            catch (SqlException sqlex)
            {
                Console.WriteLine($"SQL Error fetching product with SKU: {sku}: {sqlex.Message}");
                return StatusCode(500, $"An error occurred while retrieving the product with SKU {sku}. Please try again later.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retieving information for product: {sku}");
                return StatusCode(500, $"An error occured while fetching Product: {sku}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            try
            {
                var newProduct = await _productDapperService.CreateProduct(product);
                return Ok(newProduct);
            }
            catch (SqlException sqlex)
            {
                Console.WriteLine($"SQL Error creating product: {sqlex.Message}");
                return StatusCode(500, "An error occurred while creating the product. Please try again later.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating for product");
                return StatusCode(500, "An error occured while creating Product");
            }
        }

        [HttpDelete("{sku}")]
        public async Task<IActionResult> DeleteProductBySku(string sku)
        {
            try
            {
                var result = await _productDapperService.DeleteProductBySku(sku);
                return Ok(result);

            }
            catch (SqlException sqlex)
            {
                Console.WriteLine($"SQL Error deleting product: {sqlex.Message}");
                return StatusCode(500, "An error occurred while deleting the product. Please try again later.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting for product");
                return StatusCode(500, "An error occured while deleting Product");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] Product updatedProduct)
        {
            try
            {
                var updateSuccess = await _productDapperService.UpdateProduct(updatedProduct);
                return Ok(updateSuccess);
            }
            catch (SqlException sqlex)
            {
                Console.WriteLine($"SQL Error updating product: {sqlex.Message}");
                return StatusCode(500, "An error occurred while updating the product. Please try again later.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating for product");
                return StatusCode(500, "An error occured while updating Product");
            }

        }
    }
}
