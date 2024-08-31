using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIS.DTO;
using Talabat.APIS.Errors;
using Talabat.APIS.Helper;
using Talabat.Core.Entities;
using Talabat.Core.Interfaces;
using Talabat.Core.Specifications.ProductsSpec;

namespace Talabat.APIS.Controllers
{
   
    public class ProductsController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IGenericRepository<ProductBrand> _brandRepo;
        private readonly IGenericRepository<ProductCategory> _categoryRepo;

        public ProductsController(IGenericRepository<Product> ProductRepo , 
            IMapper mapper,
            IGenericRepository<ProductBrand> BrandRepo , 
            IGenericRepository<ProductCategory> CategoryRepo 
            )

        {
            _productRepo = ProductRepo;
            _mapper = mapper;
            _brandRepo = BrandRepo;
            _categoryRepo = CategoryRepo;
        }



        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>>  GetProducts(  [FromQuery]ProductSpecParams productSpec)
        {
            var spec = new ProductWithBrandAndCategorySpec(productSpec);

            var products = await _productRepo.GetAllWithSpecAsync( spec );



            var result = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);

            var countSpec = new ProductWithcCountSpec(productSpec); 
            var count = await _productRepo.GetCountAsync( countSpec );


            return Ok(new Pagination<ProductToReturnDto>(productSpec.PageIndex, productSpec.PageSize, count , result)); 


        }


        [ProducesResponseType(typeof(ProductToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductToReturnDto>> GetProductById(int id)
        {
            var spec = new ProductWithBrandAndCategorySpec(id);

            var product = await _productRepo.GetWithSpecByIdAsync(spec);

            if (product is null)
            {
                return NotFound(new ApiResponse(StatusCodes.Status404NotFound, "Not Found"));
            }

            var result = _mapper.Map<Product, ProductToReturnDto>(product);

            return Ok(result);
        }




        [HttpGet("brands")]   // get : /api / products / brands 
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
        {
            var brands = await _brandRepo.GetAllAsync();
            return Ok(brands);
        }


        [HttpGet("categories")]   // get : /api / products / categories 
        public async Task<ActionResult<IReadOnlyList<ProductCategory>>> GetCategories()
        {
            var brands = await _categoryRepo.GetAllAsync();
            return Ok(brands);
        }





    }




}
