using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Core.Interfaces;
using Core.Specifications;
using API.Dtos;
using AutoMapper;
using API.Errors;
using API.Helpers;

namespace API.Controllers
{
    public class ProductsController(IUnitOfWork unitOfWork, IMapper mapper) : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        public async Task<ActionResult<Pagination<ProductDto>>> GetProducts( [FromQuery] ProductSpecParams productParams) 
        {
            var spec = new ProductSpecification(productParams);
            var countSpec = new ProductWithFiltersForCountSpecification(productParams);
            var totalItems = await _unitOfWork.Products.CountAsync(countSpec);
            var products = await _unitOfWork.Products.ListAsync(spec);

            var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductDto>>(products);

            return Ok(new Pagination<ProductDto>(productParams.PageIndex, productParams.PageSize, totalItems, data));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>> GetProduct(int id) 
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);

            if (product == null) 
               return NotFound(new ApiResponse(404));

            return _mapper.Map<Product, ProductDto>(product);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateProduct(int id, Product product)
        {
            if (product.Id != id || !ProductExists(id))
                return BadRequest("Cannot update this product");

            _unitOfWork.Repository<Product>().Update(product);

            if (await _unitOfWork.Complete())
            {
                return NoContent();
            }

            return BadRequest("Problem updating the product");
        }

    
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            _unitOfWork.Repository<Product>().Add(product);

            if (await _unitOfWork.Complete())
            {
                return CreatedAtAction("GetProduct", new { id = product.Id }, product);
            }

            return BadRequest("Problem with creating a product!");
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);

            if (product == null) return NotFound();

            _unitOfWork.Repository<Product>().Remove(product);

            if (await _unitOfWork.Complete()) return NoContent();

            return BadRequest("Problem with deleting the product!");
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
        {
            var spec = new BrandListSpecification();
            return Ok(await _unitOfWork.Products.ListAsync(spec));
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
        {
            var spec = new TypeListSpecification();
            return Ok(await _unitOfWork.Products.ListAsync(spec));
        }

        private bool ProductExists(int id)
        {
            return _unitOfWork.Repository<Product>().Exists(id);
        }

    }
}