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
        public async Task<ActionResult<Order>> GetOrders([FromQuery]OrderSpecParams specParams)
        {
            var spec = new OrderSpecification(specParams);
            
            return await CreatePagedResult(_unitOfWork.Repository<Order>(), spec, specParams.PageIndex, specParams.PageSize);
        } 
    }
}