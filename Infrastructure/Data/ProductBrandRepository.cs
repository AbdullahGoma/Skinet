using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data
{
    public class ProductBrandRepository : BaseRepository<ProductBrand>, IProductBrandRepository
    {
        public ProductBrandRepository(StoreContext context) : base(context)
        {
        }
    }
}