using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Specifications
{
    public class ProductWithFiltersForCountSpecification : BaseSpecification<Product>
    {
        public ProductWithFiltersForCountSpecification(ProductSpecParams productParams) : base(x => 
                                    (string.IsNullOrEmpty(productParams.Search) || x.Name.ToLower().Contains(productParams.Search)) 
                                    && (productParams.Brands.Count == 0 || productParams.Brands.Contains(x.ProductBrand.Name)) &&
                                    (productParams.Types.Count == 0 || productParams.Types.Contains(x.ProductType.Name)))
        {

        }
    }
}