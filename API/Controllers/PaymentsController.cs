using API.Extensions;
using API.SignalR;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Stripe;

namespace API.Controllers
{
    public class PaymentsController(IPaymentService _paymentService, 
    IUnitOfWork _unitOfWork, 
    ILogger<PaymentsController> _logger, 
    IConfiguration _config,
    IHubContext<NotificationHub> _hubContext) : BaseApiController
    {
        private readonly string _webhookSecret = _config["StripeSettings:WhSecret"]!;

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

        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook() 
        {
            var json = await new StreamReader(Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = ConstructStripeEvent(json);
                
                if (stripeEvent.Data.Object is not PaymentIntent intent)
                {
                    return BadRequest("Invalid event data");
                }

                await HandlePaymentIntentSucceeded(intent);

                return Ok();
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe webhook error!");
                return StatusCode(StatusCodes.Status500InternalServerError, "Stripe webhook error!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occured!");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occured!");
            }
        }

        private async Task HandlePaymentIntentSucceeded(PaymentIntent intent)
        {
            if (intent.Status == "succeeded")
            {
                var spec = new OrderSpecification(intent.Id, true);

                var order = await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec) ?? throw new Exception("Order not found!");

                var orderTotalInCents = (long)Math.Round(order.GetTotal() * 100, MidpointRounding.AwayFromZero);
                if (orderTotalInCents != intent.Amount)
                {
                    order.Status = OrderStatus.PaymentMismatch;
                }
                else 
                {
                    order.Status = OrderStatus.PaymentRecieved;
                }

                await _unitOfWork.Complete();

                var connectionId = NotificationHub.GetConnectionIdByEmail(order.BuyerEmail);

                if (!string.IsNullOrEmpty(connectionId)) 
                {
                    await _hubContext.Clients.Client(connectionId).SendAsync("OrderCompleteNotification", order.ToDto());
                }
            }
        }

        private Event ConstructStripeEvent(string json)
        {
            try
            {
                return EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _webhookSecret);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Failed to construct stripe event!");
                throw new StripeException("Invalid signature");
            }
        }
    }
}