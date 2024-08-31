using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services.interfaces;

namespace Talabat.Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public async Task<string> CreateTokenAsync(AppUser appUser, UserManager<AppUser> userManager)
        {
             // Payload : 
             // Private Claims (Name , Id , Email )
             var AuthClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.GivenName , appUser.DisplayName),
                new Claim(ClaimTypes.Email, appUser.Email)
            };

            var userRolrs = await userManager.GetRolesAsync(appUser);

            foreach (var role in userRolrs)
            {
                AuthClaims.Add(new Claim(ClaimTypes.Role, role));
            }


            var AuthKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));


            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssure"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddDays(2),
                claims: AuthClaims,
                signingCredentials: new SigningCredentials(AuthKey, SecurityAlgorithms.HmacSha256Signature)
                );


                   
            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
