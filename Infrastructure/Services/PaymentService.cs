using Core.Entities;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Infrastructure.Services
{
    public class PaymentService(IConfiguration _config, 
                                ICartService _cartService, 
                                IUnitOfWork _unitOfWork) : IPaymentService
    {
        public async Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string cartId)
        {
            StripeConfiguration.ApiKey = _config["StripeSettings:SecretKey"];

            var cart = await _cartService.GetCartAsync(cartId);

            if (cart == null) return null;

            var shippingPrice = 0m;

            if (cart.DeliveryMethodId.HasValue) 
            {
                var deliveryMethod = await _unitOfWork.DeliveryMethods.GetByIdAsync((int)cart.DeliveryMethodId);
                
                if (deliveryMethod == null) return null; 
                
                shippingPrice = deliveryMethod.Price;
            }

            foreach (var item in cart.Items)
            {
                var productItem = await _unitOfWork.Products.GetByIdAsync(item.ProductId);

                if (productItem == null) return null;

                if (item.Price != productItem.Price) 
                {
                    item.Price = productItem.Price;
                }
            }

            var service = new PaymentIntentService();
            PaymentIntent? intent = null;

            if (string.IsNullOrEmpty(cart.PaymentIntentId)) // && (cart.DeliveryMethodId.HasValue)
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long) cart.Items.Sum(x => x.Quantity * (x.Price * 100)) + (long) shippingPrice * 100,
                    Currency = "usd",
                    PaymentMethodTypes = ["card"]
                };

                intent = await service.CreateAsync(options);
                cart.PaymentIntentId = intent.Id;
                cart.ClientSecret = intent.ClientSecret;
            }
            else //if (cart.DeliveryMethodId.HasValue)
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = (long) cart.Items.Sum(x => x.Quantity * (x.Price * 100)) + (long) shippingPrice * 100
                };
                intent = await service.UpdateAsync(cart.PaymentIntentId, options);
            }

            await _cartService.SetCartAsync(cart);

            return cart;
        }
    }
}