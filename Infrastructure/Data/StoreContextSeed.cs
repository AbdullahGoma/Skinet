using System.Reflection;
using System.Text.Json;
using Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data
{
    public class StoreContextSeed
    {
        public static async Task SeedAsync(StoreContext _context, UserManager<ApplicationUser> _userManager)
        {
            if (!_userManager.Users.Any(x => x.UserName == "admin1@test.com")) 
            {
                var user = new ApplicationUser 
                {
                    UserName = "admin1@test.com",
                    Email = "admin1@test.com",
                };

                await _userManager.CreateAsync(user, "Pa$$w0rd");
                await _userManager.AddToRoleAsync(user, "Admin");
            }


            // if(!context.ProductBrands.Any())
            // {
            //     var brandData = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/brands.json");
            //     var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandData);
            //     context.ProductBrands.AddRange(brands);
            // }

            // if(!context.ProductTypes.Any())
            // {
            //     var typesData = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/types.json");
            //     var types = JsonSerializer.Deserialize<List<ProductType>>(typesData);
            //     context.ProductTypes.AddRange(types);
            // }

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if(!_context.Products.Any())
            {
                var productsData = await File.ReadAllTextAsync(path + @"/Data/SeedData/products.json");
                var products = JsonSerializer.Deserialize<List<Product>>(productsData);
                if(products is null) return;
                _context.Products.AddRange(products);
            }

            if(!_context.DeliveryMethods.Any())
            {
                var deliveryMethods = await File.ReadAllTextAsync(path + @"/Data/SeedData/delivery.json");
                var deliveries = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryMethods);
                if(deliveries is null) return;
                _context.DeliveryMethods.AddRange(deliveries);
            }

            if(_context.ChangeTracker.HasChanges()) await _context.SaveChangesAsync();
        }
    }
}