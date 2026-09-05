using SoftnetManager.Modules.Products.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SoftnetManager.Modules.Products.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        [Authorize(Policy = "Permission.Product.Create")]
        [HttpPost("createProduct")]
        public IActionResult Create()
        {
            return Ok(new { Message = "Product Created Successfully" });
        }

        [Authorize(Policy = "Permission.Product.Read")]
        [HttpGet("GetProduct/{id}")]
        public IActionResult getProduct(int id)
        {
            return Ok(new Product { Id = id, Name = "Chair", Price = 1500, Stock = 150 });
        }

        [Authorize(Policy = "Permission.Product.Update")]
        [HttpPut("Update-Product/{id}")]
        public IActionResult Update(int id)
        {
            var updatedProduct = new Product { Id = id, Name = "Chair", Price = 1500, Stock = 150 };
            return RedirectToAction(nameof(getProduct), new { id = updatedProduct.Id });

        }

        [Authorize(Policy = "Permission.Product.Delete")]
        [HttpDelete("deleteProduct")]
        public IActionResult Delete()
        {
            return Ok(new { Message = "Product Deleted Successfully" });
        }
    }
}
