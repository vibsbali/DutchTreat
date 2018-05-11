using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DutchTreat.Data.Entities;
using Microsoft.Extensions.Logging;

namespace DutchTreat.Data
{
    public class DutchRepository : IDutchRepository
    {
        private readonly DutchContext ctx;
        private readonly ILogger<DutchRepository> logger;

        public DutchRepository(DutchContext ctx, ILogger<DutchRepository> logger)
        {
            this.ctx = ctx;
            this.logger = logger;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            logger.LogInformation("Calling Get All Products");
            return ctx.Products.OrderBy(p => p.Id).ToList();
        }

        public IEnumerable<Product> GetProductsByCategory(string category)
        {
            return ctx.Products.Where(p => p.Category == category).ToList();
        }

        public bool SaveAll()
        {
            return ctx.SaveChanges() > 0;
        }
    }
}
