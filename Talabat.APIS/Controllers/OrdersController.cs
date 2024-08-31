using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIS.DTO;
using Talabat.APIS.Errors;
using Talabat.Core;
using Talabat.Core.Entities.Order;
using Talabat.Core.Services.interfaces;

namespace Talabat.APIS.Controllers
{
    
    public class OrdersController : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OrdersController(IOrderService orderService , IMapper mapper , IUnitOfWork unitOfWork)
        {
            _orderService = orderService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }




        [ProducesResponseType(typeof(OrderToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<OrderToReturnDto>> CreateOrder(OrderDto model)
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var address = _mapper.Map<AddressDto, Address>(model.ShippingAddress);
            var order =  await _orderService.CreateOrderAsync(buyerEmail, model.BasketId, model.DeliveryMethodId, address);

            if (order is  null)
            {
                return BadRequest(new ApiResponse(400, "An Error happened in Creating Order"));
            }


            var Result =  _mapper.Map<Order, OrderToReturnDto > (order);

            return Ok(Result);
        }


        [ProducesResponseType(typeof(OrderToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetAllOrdersForUser()
        {

            var email = User.FindFirstValue(ClaimTypes.Email);
            var orders = await _orderService.GetOrdersForSpecificUserAsync(email);

            if(orders is null || orders.Count() ==0)
            {
                return NotFound(new ApiResponse(404, "You Don't Have any Orders"));
            }


            var result = _mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(orders);
            return Ok(result);

        }


        [ProducesResponseType(typeof(OrderToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderToReturnDto>> GetOneOrderForUser(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var order = await _orderService.GetOneOrderForSpecificUserAsync(email, id);
            if(order  is null)
            {
                return NotFound(new ApiResponse(404, "There is No Order with this Id For You"));
            }

            var result = _mapper.Map<Order, OrderToReturnDto>(order);

            return Ok(result);

        }



        [HttpGet("deliverymethod")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethods>>> GetAllDeliverymethods()
        {
            var methods = await _unitOfWork.Repository<DeliveryMethods>().GetAllAsync();
            return Ok(methods);
        }






    }
}



