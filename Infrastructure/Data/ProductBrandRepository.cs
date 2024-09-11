using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data
{
    public class ProductBrandRepository(StoreContext context) : BaseRepository<ProductBrand>(context), IProductBrandRepository
    {
    }
}