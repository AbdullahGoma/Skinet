using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class PaymentsController(IPaymentService _paymentService, IUnitOfWork _unitOfWork) : BaseApiController
    {
        [HttpPost("{cartId}")]
        [Authorize]
        public async Task<ActionResult<ShoppingCart>> CreateOrUpdatePaymentIntent(string cartId)
        {
            var cart = await _paymentService.CreateOrUpdatePaymentIntent(cartId);

            if(cart == null) return BadRequest("Problem with your cart!");

            return Ok(cart);
        }

        [HttpGet("delivery-method")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
        {
            return Ok(await _unitOfWork.DeliveryMethods.ListAllAsync());
        }
    }
}