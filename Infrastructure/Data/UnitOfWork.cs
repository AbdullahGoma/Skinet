using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data
{
    public class UnitOfWork(StoreContext _context) : IUnitOfWork
    {
        private readonly ConcurrentDictionary<string, object> _repositories = new();
        public IDeliveryMethodRepository DeliveryMethods => new DeliveryMethodRepository(_context);
        public IProductRepository Products => new ProductRepository(_context);
        public IProductTypeRepository ProductTypes => new ProductTypeRepository(_context);
        public IProductBrandRepository ProductBrands => new ProductBrandRepository(_context);

        public async Task<bool> Complete() => await _context.SaveChangesAsync() > 0;
        public void Dispose() => _context.Dispose();

        public IBaseRepository<T> Repository<T>() where T : BaseEntity
        {
            var type = typeof(T).Name;

            return (IBaseRepository<T>)_repositories.GetOrAdd(type, t => 
            {
                var repositoryType = typeof(BaseRepository<>).MakeGenericType(typeof(T));
                return Activator.CreateInstance(repositoryType, _context) 
                        ?? throw new InvalidOperationException($"Could not create repository instance for {t}");
            });
        }
    }
}