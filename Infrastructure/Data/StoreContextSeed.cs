using System.Reflection;
using System.Text.Json;
using Core.Entities;

namespace Infrastructure.Data
{
    public class StoreContextSeed
    {
        public static async Task SeedAsync(StoreContext context)
        {
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

            if(!context.Products.Any())
            {
                var productsData = await File.ReadAllTextAsync(path + @"/Data/SeedData/products.json");
                var products = JsonSerializer.Deserialize<List<Product>>(productsData);
                if(products is null) return;
                context.Products.AddRange(products);
            }

            if(!context.DeliveryMethods.Any())
            {
                var deliveryMethods = await File.ReadAllTextAsync(path + @"/Data/SeedData/delivery.json");
                var deliveries = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryMethods);
                if(deliveries is null) return;
                context.DeliveryMethods.AddRange(deliveries);
            }

            if(context.ChangeTracker.HasChanges()) await context.SaveChangesAsync();
        }
    }
}