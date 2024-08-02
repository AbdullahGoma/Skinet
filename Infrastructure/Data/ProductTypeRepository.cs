using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data
{
    public class ProductTypeRepository : BaseRepository<ProductType>, IProductTypeRepository 
    {
        public ProductTypeRepository(StoreContext context) : base(context)
        {
        }
    }
}