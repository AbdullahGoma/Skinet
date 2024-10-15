using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Infrastructure.Services
{
    public class CouponService : ICouponService
    {
        public CouponService(IConfiguration config)
        {
            StripeConfiguration.ApiKey = config["StripeSettings:SecretKey"];
        }
        public async Task<Core.Entities.Coupon?> GetCouponFromPromoCode(string code)
        {
            var promotionService = new PromotionCodeService();
            var options = new PromotionCodeListOptions
            {
                Code = code
            };
            var promotionCodes = await promotionService.ListAsync(options);
            var promotionCode = promotionCodes.FirstOrDefault();
            if (promotionCode != null && promotionCode.Coupon != null)
            {
                return new Core.Entities.Coupon
                {
                    Name = promotionCode.Coupon.Name,
                    AmountOff = promotionCode.Coupon.AmountOff,
                    PercentOff = promotionCode.Coupon.PercentOff,
                    CouponId = promotionCode.Coupon.Id,
                    PromotionCode = promotionCode.Code
                };
            }
            return null;
        }
    }
}