using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Talabat.APIS.Errors;
using Talabat.APIS.Helper;
using Talabat.Core;
using Talabat.Core.Interfaces;
using Talabat.Core.Services.interfaces;
using Talabat.Repository;
using Talabat.Repository.Data;
using Talabat.Repository.Repositories;
using Talabat.Service;

namespace Talabat.APIS.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddControllers(); // Register Built-In Apis Service at the Container

          services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();


            services.AddDbContext<StoreDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();


            services.AddAutoMapper(typeof(MappingProfiles));
            //services.AddScoped<IOrderService, OrderService>();


            services.Configure<ApiBehaviorOptions>(options =>
                options.InvalidModelStateResponseFactory = (ActionContext context) =>
                {
                    var errors = context.ModelState
                        .Where(p => p.Value.Errors.Any())
                        .SelectMany(p => p.Value.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToArray();

                    var validateErrorResponse = new ApiValidationErrorResponse
                    {
                        Errors = errors
                    };

                    return new BadRequestObjectResult(validateErrorResponse);
                });

            return services;  // Missing semicolon added here
        }
    }
}
