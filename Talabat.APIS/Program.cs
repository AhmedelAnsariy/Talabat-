using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Talabat.APIS.Errors;
using Talabat.APIS.Extensions;
using Talabat.APIS.Helper;
using Talabat.APIS.MiddleWares;
using Talabat.Core.Interfaces;
using Talabat.Repository.Data;
using Talabat.Repository.Repositories;
using Talabat.APIS.Extensions;
using StackExchange.Redis;
using Talabat.Repository.Identity;
using Microsoft.AspNetCore.Identity;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services.interfaces;
using Talabat.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace Talabat.APIS
{
    public class Program
    {


        // Main Entry Point
        public static async Task Main(string[] args)
        {
            var webApplicationBuilder = WebApplication.CreateBuilder(args);

     

            #region Configure Services
            webApplicationBuilder.Services.AddApplicationService(webApplicationBuilder.Configuration);


            // For Redis
            webApplicationBuilder.Services.AddSingleton<IConnectionMultiplexer>((serviceprovider) =>
            {
                var connection = webApplicationBuilder.Configuration.GetConnectionString("Redis");
                return ConnectionMultiplexer.Connect(connection);
            });





            webApplicationBuilder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            { }).AddEntityFrameworkStores<AppIdentityDbContext>();

            #endregion



            webApplicationBuilder.Services.AddScoped(typeof(IBasketRepository) , typeof(BasketRepository));

            webApplicationBuilder.Services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseSqlServer(webApplicationBuilder.Configuration.GetConnectionString("IdentityConnection"));
            });




            webApplicationBuilder.Services.AddScoped<ITokenService, TokenService>();
            webApplicationBuilder.Services.AddScoped<IOrderService, OrderService>();
            webApplicationBuilder.Services.AddScoped<IPaymentService,PaymentService>();




            webApplicationBuilder.Services.AddAuthentication(optoins =>
            {
                optoins.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                optoins.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                                                             .AddJwtBearer(options =>
                                                             {
                                                                 options.TokenValidationParameters = new TokenValidationParameters()
                                                                 {
                                                                     ValidateIssuer = true,
                                                                     ValidIssuer = webApplicationBuilder.Configuration["JWT:ValidIssure"],
                                                                     ValidateAudience = true,
                                                                     ValidAudience = webApplicationBuilder.Configuration["JWT:ValidAudience"],
                                                                     ValidateLifetime = true,
                                                                     ValidateIssuerSigningKey = true,
                                                                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(webApplicationBuilder.Configuration["JWT:Key"]))

                                                                 };


                                                             } );












            var app = webApplicationBuilder.Build();




            #region For Update Database
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;


            var _Context = services.GetRequiredService<StoreDbContext>();
            var _IdentityDbContext = services.GetRequiredService<AppIdentityDbContext>();

            var LogggerFactory = services.GetRequiredService<ILoggerFactory>();


            try
            {
                await _Context.Database.MigrateAsync();
                await StoreDbContextSeed.SeedAsync(_Context);
                await _IdentityDbContext.Database.MigrateAsync();


                var _userManager = services.GetRequiredService<UserManager<AppUser>>();
                await IdentityDbContextSeed.SeedUserAsync(_userManager);


            }
            catch (Exception ex)
            {
                var loogger = LogggerFactory.CreateLogger<Program>();
                loogger.LogError(ex, "Error in Update Database");
            } 
            #endregion



            //Middelwares

            #region Configure
            app.UseMiddleware<ExceptionMiddleware>();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            app.UseStaticFiles();

            app.UseHttpsRedirection();
            
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers(); 
            #endregion



            app.Run();
        }
    }
}
