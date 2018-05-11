using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DutchTreat.Data.Entities;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace DutchTreat.Data
{
    public class DutchSeeder
    {
        private readonly DutchContext ctx;
        private readonly IHostingEnvironment hostingEnvironment;

        public DutchSeeder(DutchContext ctx, IHostingEnvironment hostingEnvironment)
        {
            this.ctx = ctx;
            this.hostingEnvironment = hostingEnvironment;
        }

        public void Seed()
        {
            ctx.Database.EnsureCreated();

            if (!ctx.Products.Any())
            {
                var filePath = Path.Combine(hostingEnvironment.ContentRootPath, "Data/art.json");
                var json = File.ReadAllText(filePath);
                var products = JsonConvert.DeserializeObject<IEnumerable<Product>>(json);
                ctx.Products.AddRange(products);

                var order = new Order
                {
                    OrderDate = DateTime.Now,
                    OrderNumber = "12345",
                    Items = new List<OrderItem>
                    {
                        new OrderItem
                        {
                            Product = products.First(),
                            Quantity = 5,
                            UnitPrice = products.First().Price
                        }
                    }
                };

                ctx.Orders.Add(order);
                ctx.SaveChanges();
            }
        }
    }
}
