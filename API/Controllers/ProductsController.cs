using Infrastructure.Data;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Core.Interfaces;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repo;

        public ProductsController(IProductRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts() 
        {
            var products = await _repo.GetProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id) 
        {
            if(await _repo.GetProductAsync(id) == null) 
                return NotFound();
            return await _repo.GetProductAsync(id);
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
        {
            return Ok(await _repo.GetProductBrandsAsync());
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetTypes()
        {
            return Ok(await _repo.GetProductTypesAsync());
        }

    }
}