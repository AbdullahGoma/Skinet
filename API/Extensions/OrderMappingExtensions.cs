using API.Dtos;
using Core.Entities.OrderAggregate;

namespace API.Extensions
{
    public static class OrderMappingExtensions
    {
        public static OrderDto ToDto(this Order order) 
        {
            return new OrderDto
            {
                Id = order.Id,
                BuyerEmail = order.BuyerEmail,
                OrderDate = order.OrderDate,
                ShippingAddress = order.ShippingAddress,
                PaymentSummary = order.PaymentSummary,
                DeliveryMethod = order.DeliveryMethod.Description,
                ShippingPrice = order.DeliveryMethod.Price,
                OrderItems = order.OrderItems.Select(oi => oi.ToDto()).ToList(),
                Subtotal = order.Subtotal,
                Discount = order.Discount,
                Total = order.GetTotal(),
                Status = order.Status.ToString(),
                PaymentIntentId = order.PaymentIntentId,
            };
        }

        public static OrderItemDto ToDto(this OrderItem item)
        {
            return new OrderItemDto
            {
                ProductId = item.ItemOrdered.ProductId,
                ProductName = item.ItemOrdered.ProductName,
                PictureUrl = item.ItemOrdered.PictureUrl,
                Price = item.Price,
                Quantity = item.Quantity,
            };
        }
    }
}