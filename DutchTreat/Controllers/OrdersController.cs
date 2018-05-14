using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using DutchTreat.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DutchTreat.Controllers
{
   [Route("api/[Controller]")]
   [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
   public class OrdersController : Controller
   {
      private readonly IDutchRepository _repo;
      private readonly ILogger<OrdersController> _logger;
      private readonly IMapper _mapper;
      private readonly UserManager<StoreUser> _userManager;

      public OrdersController(IDutchRepository repo, ILogger<OrdersController> logger, IMapper mapper, UserManager<StoreUser> userManager)
      {
         _repo = repo;
         _logger = logger;
         _mapper = mapper;
         _userManager = userManager;
      }

      [HttpGet]
      public IActionResult Get(bool includeProducts = true)
      {
         try
         {
            //this is username
            var username = User.Identity.Name;

            var results = _repo.GetAllOrdersByUser(username, includeProducts);

            return Ok(_mapper.Map<IEnumerable<Order>, IEnumerable<OrderViewModel>>(results));
         }
         catch (Exception e)
         {
            _logger.LogError($"Failed to get orders: {e}");
            return BadRequest("Failed to get orders");
         }
      }

      [HttpGet("{id:int}")]
      public IActionResult Get(int id)
      {
         try
         {
            //this is username
            var username = User.Identity.Name;
            var order = _repo.GetOrderById(id, username);

            if (order != null)
            {
               return Ok(_mapper.Map<Order, OrderViewModel>(order));
            }

            return NotFound();
         }
         catch (Exception e)
         {
            _logger.LogError($"Unable to get the order: {e}");
            return BadRequest("Failed to get the order");
         }
      }

      [HttpPost]
      public async Task<IActionResult> Post([FromBody]OrderViewModel model)
      {
         try
         {
            //We can use the same methodology to test for validity of the model as we did in web
            if (ModelState.IsValid)
            {
               Order newOrder = _mapper.Map<OrderViewModel, Order>(model);

               if (newOrder.OrderDate == DateTime.MinValue)
               {
                  newOrder.OrderDate = DateTime.Now;
               }

               newOrder.User = await _userManager.FindByNameAsync(User.Identity.Name);
               _repo.AddEntity(newOrder);

               if (_repo.SaveAll())
               {
                  OrderViewModel vm = _mapper.Map<Order, OrderViewModel>(newOrder);

                  return Created($"/api/orders/{vm.OrderId}", vm);
               }

               return BadRequest($"Unable to save the order");
            }
            else
            {
               return BadRequest(ModelState);
            }
         }
         catch (Exception e)
         {
            _logger.LogError($"Failed to save a new order: {e}");
            return BadRequest("Failed to get the order");
         }
      }
   }
}
