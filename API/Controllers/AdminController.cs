using API.Dtos;
using API.Extensions;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController(IUnitOfWork _unitOfWork, IPaymentService _paymentService) : BaseApiController
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

        [HttpPost("orders/refund/{id:int}")]
        public async Task<ActionResult<OrderDto>> RefundOrder(int id)
        {
            var spec = new OrderSpecification(id);

            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);

            if (order == null) return BadRequest("No order with that id!");

            if (order.Status == OrderStatus.Pending) return BadRequest("Payment not received for this order!");

            var result = await _paymentService.RefundPayment(order.PaymentIntentId);

            if (result == "succeeded")
            {
                order.Status = OrderStatus.Refunded;
                await _unitOfWork.Complete();

                return order.ToDto();
            }

            return BadRequest("Problem refunding order!");
        } 
    }
}