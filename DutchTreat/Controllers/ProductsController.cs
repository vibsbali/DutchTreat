using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DutchTreat.Controllers
{
   [Route("api/[Controller]")]
   public class ProductsController : Controller
   {
      private readonly IDutchRepository _repo;
      private readonly ILogger<ProductsController> _logger;

      public ProductsController(IDutchRepository repo, ILogger<ProductsController> logger)
      {
         _repo = repo;
         _logger = logger;
      }

      [HttpGet]
      public IActionResult Get()
      {
         try
         {
            return Ok(_repo.GetAllProducts());
         }
         catch (Exception e)
         {
            _logger.LogError($"Failed to get products: {e}");
            return BadRequest("Failed to get products");
         }
      }

   }
}
