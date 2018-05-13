using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DutchTreat.Data.Entities;
using DutchTreat.ViewModels;

namespace DutchTreat.Data
{
    public class DutchMappingProfile : Profile
    {
       public DutchMappingProfile()
       {
          CreateMap<Order, OrderViewModel>()
             .ForMember(o => o.OrderId, ex => ex.MapFrom(o => o.Id))
             //here we are saying that hey automapper when you want to populate OrderId of OrderViewModel
             //look at order's Id

             .ReverseMap();
             //the ReserveMap is changed to create an opposite mapping


          CreateMap<OrderItem, OrderItemViewModel>()
             .ReverseMap();
       }
    }
}
