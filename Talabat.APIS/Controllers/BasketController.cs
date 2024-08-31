using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIS.DTO;
using Talabat.APIS.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Interfaces;

namespace Talabat.APIS.Controllers
{
    
    public class BasketController : BaseApiController
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        public BasketController(IBasketRepository basketRepository , IMapper mapper)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
        }



        [HttpGet]
        public async Task<ActionResult<CustomerBasket>> GetBasket (string id)
        {
            var basket = await _basketRepository.GetBasketAsync (id);

            if(basket is null)
            {
                return Ok(new CustomerBasket { Id = id });
            }

            return Ok(basket);
        }

        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdateBasket (CustomerBasketDto basket)
        {

            var MappedBasket = _mapper.Map<CustomerBasket>(basket);

            var createorupdate = await _basketRepository.UpdateBasketAsync (MappedBasket);
            if(createorupdate is null)
            {
                return BadRequest(new ApiResponse(400));
            }

            return Ok(createorupdate);
        }


        [HttpDelete]
        public async Task DeleteBasket(string id)
        {
            await _basketRepository.DeleteBasket(id);
        }






    }
}
