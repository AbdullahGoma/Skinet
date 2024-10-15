using Core.Entities;

namespace Core.Interfaces
{
    public interface ICouponService
    {
        Task<Coupon?> GetCouponFromPromoCode(string code);
    }
}