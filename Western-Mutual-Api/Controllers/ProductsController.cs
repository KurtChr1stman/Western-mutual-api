using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Western_Mutual_Api.Interfaces;
using Western_Mutual_Api.Models;

namespace Western_Mutual_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService, IBuyerService buyerService)
        {
            // Inject the needed project service to make calls to this service
            _productService = productService;
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            try
            {
                // call the GetProduct function and if it is successful, return OK and the list of Products
                var results = _productService.GetProducts();
                return Ok(results);
            }
            catch (Exception ex)
            {
                // If anything fails during the try, return error message with the status code 500
                return StatusCode(500, new { message = "An error occurred while fetching products", error = ex.Message });
            }
        }

        [HttpGet("productSKU")]
        public IActionResult GetProductBySku(string productSku)
        {
            // The SKU of the product, extracted from the URL path.
            try
            {
                var result = _productService.GetProductBySku(productSku);

                // check if the reult is empty. They could be using a SKU of a product that doesnt exist
                if (result == null)
                {
                    // if it doesnt exist, return a message with the status code of 500
                    return NotFound(new { message = $"Product with SKU: {productSku} not found." });
                }

                // if we get a product back, return OK with the result
                return Ok(result);
            }
            catch (Exception ex)
            {
                // if antyhing goes wrong, return error message with 500 status code
                return StatusCode(500, new { message = "An error occured while fetching Product by SKU", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            // Check if the request object has data
            if (product == null)
            {
                // If it's empty, return an error of incomplete
                return BadRequest("Product is not complete");
            }

            // Check if the product with that name exists
            var existingProduct = _productService.GetProductTrimToUpper(product);

            // If it does exist, return an error message
            if (existingProduct != null)
            {
                return StatusCode(422, "Product with that name already exists");
            }

            // If we got to this point, that means the product does not already exist
            await _productService.CreateProduct(product); // Call asynchronously

            // Assuming the product is successfully added, return 201 created response
            // along with the URI of the newly created product and the product details.
            return CreatedAtAction(nameof(CreateProduct), new { id = product.SKU }, product);
        }

        [HttpDelete("productSku")]
        public async Task<IActionResult> DeleteProductBySku(string productSku)
        {
            // The SKU of the product, extracted from the URL path.
            // check if it is a vaild request
            if (string.IsNullOrWhiteSpace(productSku))
            {
                return BadRequest("Product SKU cannot be empty.");
            }

            try
            {
                bool deletionResult = await _productService.DeleteProductBySku(productSku);

                if (deletionResult)
                {
                    // if deleteResult is true, return 204 No Content response
                    return NoContent();
                }
                else
                {
                    // if delete was unsuccessful, return Not Found
                    return NotFound($"Product with SKU {productSku} not found.");
                }
            }
            catch (KeyNotFoundException ex)
            {
                // this catch is to handle exceptions of not finding a product that matches that SKU
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // thihs catch is to handle any general errors that might happen
                Console.WriteLine($"Error deleting product: {ex.Message}");
                return StatusCode(500, "An error occurred while deleting the product.");
            }
        }

        [HttpPut("sku")]
        public async Task<IActionResult> UpdateProduct(string sku, [FromBody] Product updatedProduct)
        {
            // check if it a valid request
            if (sku != updatedProduct.SKU)
            {
                return BadRequest("SKU mismatch.");
            }

            try
            {
                // call for the update, if it is successful, return 204 no content response
                await _productService.UpdateProduct(updatedProduct);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                // this catch is to handle exceptions of not finding a product that matches that SKU
                return NotFound();
            }
            catch (Exception ex)
            {
                // thihs catch is to handle any general errors that might happen
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
