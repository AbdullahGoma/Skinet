using API.Dtos;
using API.Extensions;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class OrdersController(ICartService _cartService, IUnitOfWork _unitOfWork) : BaseApiController
    {
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(CreateOrderDto dto) 
        {
            var email = User.GetEmail();

            var cart = await _cartService.GetCartAsync(dto.CartId);

            if (cart == null) return BadRequest("Cart not Found!");

            if (cart.PaymentIntentId == null) return BadRequest("No payment intent for this order!");

            var items = new List<OrderItem>();

            foreach (var item in cart.Items) 
            {
                var productItem = await _unitOfWork.Repository<Product>().GetByIdAsync(item.ProductId);

                if (productItem == null) return BadRequest("Problem with the order!");

                var itemOrdered = new ProductItemOrdered
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    PictureUrl = item.PictureUrl,
                };

                var orderItem = new OrderItem
                {
                    ItemOrdered = itemOrdered,
                    Price = productItem.Price,
                    Quantity = item.Quantity,
                };
                items.Add(orderItem);
            }

            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(dto.DeliveryMethodId);
        
            if (deliveryMethod == null) return BadRequest("No Delivery Method Selected!");

            var order = new Order
            {
                OrderItems = items,
                DeliveryMethod = deliveryMethod,
                ShippingAddress = dto.ShippingAddress,
                Subtotal = items.Sum(x => x.Price * x.Quantity),
                Discount = dto.Discount,
                PaymentSummary = dto.PaymentSummary,
                PaymentIntentId = cart.PaymentIntentId,
                BuyerEmail = email,
            };

            _unitOfWork.Repository<Order>().Add(order);
            
            if (await _unitOfWork.Complete()) return order;

            return BadRequest("Problem happened when creating the order!");
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrdersForUser() 
        {
            var spec = new OrderSpecification(User.GetEmail());

            var orders = await _unitOfWork.Repository<Order>().ListAsync(spec);

            var ordersToReturn = orders.Select(o => o.ToDto()).ToList();

            return Ok(ordersToReturn);
        } 

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
        {
            var spec = new OrderSpecification(User.GetEmail(), id);

            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);

            if (order == null) return NotFound();

            return order.ToDto();
        }

    }
}