using System.ComponentModel.DataAnnotations;
using DutchTreat.Data.Entities;

namespace DutchTreat.ViewModels
{
   public class OrderItemViewModel
   {
      public int Id { get; set; }

      [Required]
      public int Quantity { get; set; }
      [Required]
      public decimal UnitPrice { get; set; }

      [Required]
      public int ProductId { get; set; }
      //Following properties are from Product and are prefixed with Product so that automapper can fill the values
      //Without us having to create a specific mapping
      public string ProductCategory { get; set; }
      public string ProductSize { get; set; }
      public string ProductTitle { get; set; }
      public string ProductArtist { get; set; }
      public string ProductArtId { get; set; }
   }
}