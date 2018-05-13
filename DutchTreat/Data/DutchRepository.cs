using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DutchTreat.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

      public IEnumerable<Order> GetAllOrders(bool includeItems = true)
      {
         IQueryable<Order> orders = ctx.Orders.Include(o => o.Items).Include(o => o.User);

         if (includeItems)
         {
            orders = ctx.Orders.Include(o => o.Items).ThenInclude(o => o.Product).Include(o => o.User);
         }

         return orders.ToList();
      }

      public Order GetOrderById(int id)
      {
         return ctx.Orders.Where(o => o.Id == id)
            .Include(o => o.Items)
            .ThenInclude(o => o.Product)
            .Include(o => o.User)
            .FirstOrDefault();
      }

      public void AddEntity(object model)
      {
         ctx.Add(model);
      }
   }
}
