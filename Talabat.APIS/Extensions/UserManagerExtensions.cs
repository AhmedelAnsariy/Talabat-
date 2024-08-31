using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Talabat.Core.Entities.Identity;

namespace Talabat.APIS.Extensions
{
    public  static class UserManagerExtensions
    {
        public static async Task<AppUser?> FindUserAddressAsync(this UserManager<AppUser> manager, ClaimsPrincipal user)
        {
            var userEmail = user.FindFirstValue(ClaimTypes.Email);
            var userWithAddress = await manager.Users.Include(u => u.Address).FirstOrDefaultAsync(u => u.Email == userEmail);
            return userWithAddress;
        }
    }
}
