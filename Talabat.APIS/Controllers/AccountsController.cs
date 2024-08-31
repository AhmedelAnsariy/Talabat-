using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIS.DTO;
using Talabat.APIS.Errors;
using Talabat.APIS.Extensions;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services.interfaces;

namespace Talabat.APIS.Controllers
{
 
    public class AccountsController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountsController(UserManager<AppUser> userManager , SignInManager<AppUser> signInManager , ITokenService tokenService , IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }


        [HttpPost("Register")]
        public async Task<ActionResult<UserDtoResponse>> Register(RegisterDto model)
        {
            if(CheckEmailExists(model.Email).Result.Value) // To make it Synchronous
            {
                return BadRequest(new ApiResponse(400, "Email Already Exists"));
            }



            var user = new AppUser()
            {
                Email = model.Email,
                DisplayName = model.DisplayName,
                PhoneNumber = model.phone,
                UserName = model.DisplayName
            };

            var result = await _userManager.CreateAsync(user , model.Password);

            if(result.Succeeded)
            {
                var ReturnUser = new UserDtoResponse()
                {
                    DisplayName = model.DisplayName,
                    Email = model.Email,
                    Token = await _tokenService.CreateTokenAsync(user, _userManager)
                };

                return Ok(ReturnUser);
            }
            else
            {
                return BadRequest(new ApiResponse(400));
            }

        }



        [HttpPost("Login")]
        public async Task<ActionResult<UserDtoResponse>> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null)
            {
                return Unauthorized(new ApiResponse(401));
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (!result.Succeeded)
            {
                //return Unauthorized(new ApiResponse(401));
                return BadRequest(new ApiResponse(400, "Invalid Email Or Password"));
            }

            var ReturnUser = new UserDtoResponse()
            {
                DisplayName = user.DisplayName,
                Email = model.Email,
                Token = await _tokenService.CreateTokenAsync(user, _userManager)
            };

            return Ok(ReturnUser);


        }

        [Authorize]
        [HttpGet("GetCurrentUser")]
        public async Task<ActionResult<UserDtoResponse>> GetCurrentUser()
        {
            var EmailUser = User.FindFirstValue(ClaimTypes.Email); // Cannot return null because he must be login and have token

            var userData = await _userManager.FindByEmailAsync(EmailUser);

            return Ok(new UserDtoResponse()
            {
                DisplayName= userData.DisplayName,
                Email = userData.Email,
                Token = await _tokenService.CreateTokenAsync(userData, _userManager)
            }
            );


        }

        [Authorize]
        [HttpGet("GetCurrentUserAddress")]
        public async Task<ActionResult<AddressDto>> GetCurrentUserAddress()
        {
            var user = await _userManager.FindUserAddressAsync(User);
            var mappedAddress = _mapper.Map<Address, AddressDto>(user.Address);

            if(mappedAddress != null)
            {

            return Ok(mappedAddress);
            }
            else
            {
                return BadRequest(new ApiResponse(204,"User  is Authorized but don't Have Address Yet"));
            }
            
        }



        [Authorize]
        [HttpPut("Address")]
        public async Task<ActionResult<AddressDto>> UpdateCurrentUserAddress(AddressDto model)
        {
            var user = await _userManager.FindUserAddressAsync(User);
            var address = _mapper.Map<AddressDto, Address>(model);

            user.Address = address;
            var result = await _userManager.UpdateAsync(user);

            if(!result.Succeeded)
            {
                return BadRequest(new ApiResponse(400));
            }

            return Ok(model);

        }




        [HttpGet("emailExisits")]
        public async Task<ActionResult<bool>> CheckEmailExists(string email)
        {
            return await _userManager.FindByEmailAsync(email) is not null;
        }

    }
}
