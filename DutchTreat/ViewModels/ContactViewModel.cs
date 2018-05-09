﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault.Models;

namespace DutchTreat.ViewModels
{
    public class ContactViewModel
    {
        [Required]
        [MinLength(5)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        [MaxLength(20, ErrorMessage ="Too Long")]
        public string Message { get; set; }
    }
}
