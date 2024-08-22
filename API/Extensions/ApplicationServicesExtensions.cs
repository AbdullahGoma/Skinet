using API.Errors;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace API.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection ApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            // Register MVC controllers
            services.AddControllers();


            // Add services to the container.
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();


            services.AddDbContext<StoreContext>(option => 
            {
                option.UseSqlServer(config.GetConnectionString("DefaultConnection"));
            });

            // Services

            // Unit Of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //Auto Mapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Configures the behavior of API controllers when model validation fails
            services.Configure<ApiBehaviorOptions>(options => 
            {
                // Customizes the response returned for invalid model state
                options.InvalidModelStateResponseFactory = actionContext => 
                {
                    // Extracts validation errors from the ModelState where there are any errors
                    var errors = actionContext.ModelState
                        .Where(e => e.Value.Errors.Count > 0)   // Filters out entries with no errors
                        .SelectMany(x => x.Value.Errors)        // Flattens the list of errors
                        .Select(x => x.ErrorMessage)            // Selects the error messages
                        .ToArray();                             // Converts to an array of strings

                    // Creates a custom error response object with the extracted errors
                    var errorResponse = new ApiValidationErrorResponse
                    {
                        Errors = errors
                    };

                    // Returns a 400 Bad Request response with the custom error response in the body
                    return new BadRequestObjectResult(errorResponse);
                };
            });

            // Configure Redis
            services.AddSingleton<IConnectionMultiplexer>(c => 
            {
                var connectionString = config.GetConnectionString("Redis") ?? throw new Exception("Cannot get redis connection string");
                var configuration = ConfigurationOptions.Parse(connectionString, true);
                return ConnectionMultiplexer.Connect(configuration);
            });

            // CartService
            services.AddSingleton<ICartService, CartService>();

            // Identity Congfiguration
            services.AddAuthorization(options =>
            {
                // Configure authorization policies if needed
            });
            services.AddIdentityApiEndpoints<ApplicationUser>().AddEntityFrameworkStores<StoreContext>();

            // Cors Config
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200");
                });
            });

            return services;
        }
    }
}