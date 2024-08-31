using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order;

namespace Talabat.Repository.Data
{
    public static class StoreDbContextSeed
    {

        public static async Task SeedAsync(StoreDbContext _context)
        {



            if(_context.ProductBrands.Count() ==0 )
            {
                // 1- Read Data From Files 
                var Brands = File.ReadAllText("../Talabat.Repository/Data/DataSeed/brands.json");
                var BrandsData = JsonSerializer.Deserialize<List<ProductBrand>>(Brands);

                if (BrandsData?.Count > 0)
                {
                    foreach (var Brand in BrandsData)
                    {
                        _context.Set<ProductBrand>().Add(Brand);
                    }
                    await _context.SaveChangesAsync();
                }

            }



            if(_context.ProductCategories.Count() ==0 )
            {
                var categories = File.ReadAllText("../Talabat.Repository/Data/DataSeed/categories.json");
                var CategoriesData = JsonSerializer.Deserialize<List<ProductCategory>>(categories);
                if(CategoriesData?.Count > 0)
                {
                    foreach (var Category in CategoriesData)
                    {
                        _context.ProductCategories.Add(Category);
                    }
                    await _context.SaveChangesAsync();
                }
            }


            if(_context.Products.Count() ==0)
            {
                var Products = File.ReadAllText("../Talabat.Repository/Data/DataSeed/products.json");
                var ProductsData = JsonSerializer.Deserialize<List<Product>>(Products);
                if(ProductsData?.Count > 0)
                {
                    foreach (var product in ProductsData)
                    {
                        _context.Products.Add(product);
                    }
                    await _context.SaveChangesAsync();
                }
            }








            if (_context.DeliveryMethods.Count() == 0)
            {
                var deliveryMethods = File.ReadAllText("../Talabat.Repository/Data/DataSeed/delivery.json");

                var deliveryMethodsData = JsonSerializer.Deserialize<List<DeliveryMethods>>(deliveryMethods);


                if (deliveryMethodsData?.Count > 0)
                {
                    foreach (var delverymethod in deliveryMethodsData)
                    {
                        _context.DeliveryMethods.Add(delverymethod);
                    }
                    await _context.SaveChangesAsync();
                }
            }







        }

    }
}
