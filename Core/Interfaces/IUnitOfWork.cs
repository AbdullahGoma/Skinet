using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IUnitOfWork
    {
        IProductRepository Products { get; }
        IProductTypeRepository ProductTypes { get; }
        IProductBrandRepository ProductBrands { get; }
        int Complete();
    }
}