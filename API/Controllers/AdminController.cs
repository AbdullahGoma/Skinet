using API.Dtos;
using API.Extensions;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController(IUnitOfWork _unitOfWork) : BaseApiController
    {
        [HttpGet("orders")]
        public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrders([FromQuery]OrderSpecParams specParams)
        {
            var spec = new OrderSpecification(specParams);
            
            return await CreatePagedResult(_unitOfWork.Repository<Order>(), spec, specParams.PageIndex, specParams.PageSize, o => o.ToDto());
        } 

        [HttpGet("orders/{id:int}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
        {
            var spec = new OrderSpecification(id);

            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);

            if (order == null) return BadRequest("No order with bad id!");

            return order.ToDto();
        } 
    }
}