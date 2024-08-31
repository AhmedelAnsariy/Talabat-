using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications.ProductsSpec
{
    public class ProductWithcCountSpec : BaseSpecifications<Product>
    {
        public ProductWithcCountSpec(ProductSpecParams productSpec) : base(p =>
            (!productSpec.BrandId.HasValue || p.BrandId == productSpec.BrandId.Value) &&
            (!productSpec.CategoryId.HasValue || p.CategoryId == productSpec.CategoryId.Value)
        )

        {
            
        }
    }
}
