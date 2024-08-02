using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Interfaces;

namespace Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext _context;

        public UnitOfWork(StoreContext context)
        {
            _context = context;
        }
        public IProductRepository Products => new ProductRepository(_context);

        public IProductTypeRepository ProductTypes => new ProductTypeRepository(_context);

        public IProductBrandRepository ProductBrands => new ProductBrandRepository(_context);

        public int Complete()
        {
            return _context.SaveChanges();
        }
    }
}