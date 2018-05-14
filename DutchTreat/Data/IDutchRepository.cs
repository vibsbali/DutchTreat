using System.Collections.Generic;
using DutchTreat.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DutchTreat.Data
{
   public interface IDutchRepository
   {
      IEnumerable<Product> GetAllProducts();
      IEnumerable<Product> GetProductsByCategory(string category);
      bool SaveAll();

      IEnumerable<Order> GetAllOrders(bool includeItems);
      Order GetOrderById(int id, string username);
      void AddEntity(object model);
      IEnumerable<Order> GetAllOrdersByUser(string userName, bool includeProducts);
     
   }
}