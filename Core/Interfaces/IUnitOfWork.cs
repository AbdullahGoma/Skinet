using Core.Entities;

namespace Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        public IBaseRepository<T> Repository<T>() where T : BaseEntity;
        IDeliveryMethodRepository DeliveryMethods { get; }
        IProductRepository Products { get; }
        IProductTypeRepository ProductTypes { get; }
        IProductBrandRepository ProductBrands { get; }
        Task<bool> Complete();
    }
}