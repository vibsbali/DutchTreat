using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using DutchTreat.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DutchTreat.Controllers
{
   [Route("/api/orders/{orderid}/items")]
   [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
   public class OrderItemsController : Controller
   {
      private readonly IDutchRepository _repo;
      private readonly ILogger<OrderItemsController> _logger;
      private readonly IMapper _mapper;


      public OrderItemsController(IDutchRepository repo, ILogger<OrderItemsController> logger, IMapper mapper)
      {
         _repo = repo;
         _logger = logger;
         _mapper = mapper;
      }

      [HttpGet]
      public IActionResult Get(int orderId)
      {
         //this is username
         var username = User.Identity.Name;
         var order = _repo.GetOrderById(orderId, username);
         if (order != null)
            return Ok(_mapper.Map<IEnumerable<OrderItem>, IEnumerable<OrderItemViewModel>>(order.Items));

         return NotFound(); 
      }

      [HttpGet("{id}")]
      public IActionResult Get(int orderId, int id)
      {
         //this is username
         var username = User.Identity.Name;
        

         var order = _repo.GetOrderById(orderId, username);
         // ReSharper disable once UseNullPropagation
         if (order != null)
         {
            var item = order.Items.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
               return Ok(_mapper.Map<OrderItem, OrderItemViewModel>(item));
            }
         }
         return NotFound();

      }

   }
}
