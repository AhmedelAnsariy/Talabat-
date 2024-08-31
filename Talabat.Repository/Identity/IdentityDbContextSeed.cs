using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Repository.Identity
{
    public static class IdentityDbContextSeed
    {

        public static async Task SeedUserAsync(UserManager<AppUser> _userManager)
        {

            if(_userManager.Users.Count() == 0)
            {
                var user = new AppUser()
                {
                    DisplayName = "Ahmed Mohamed",
                    Email = "Ahmed@gmail.com",
                    UserName = "AhmedAnsary",
                    PhoneNumber = "01022079104"
                };

                await _userManager.CreateAsync(user ,"Ahmed@123");
            }


        }






    }
}
