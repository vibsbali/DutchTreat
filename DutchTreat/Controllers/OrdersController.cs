using System;
using System.Collections.Generic;
using AutoMapper;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using DutchTreat.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DutchTreat.Controllers
{
   [Route("api/[Controller]")]
   public class OrdersController : Controller
   {
      private readonly IDutchRepository _repo;
      private readonly ILogger<OrdersController> _logger;
      private readonly IMapper _mapper;

      public OrdersController(IDutchRepository repo, ILogger<OrdersController> logger, IMapper mapper)
      {
         _repo = repo;
         _logger = logger;
         _mapper = mapper;
      }

      [HttpGet]
      public IActionResult Get(bool includeProducts = true)
      {
         try
         {
            return Ok(_mapper.Map<IEnumerable<Order>, IEnumerable<OrderViewModel>>(_repo.GetAllOrders(includeProducts)));
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
            var order = _repo.GetOrderById(id);

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
      public IActionResult Post([FromBody]OrderViewModel model)
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
