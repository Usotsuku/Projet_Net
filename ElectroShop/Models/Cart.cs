﻿using System.ComponentModel.DataAnnotations;

namespace ElectroShop.Models
{
    public class Cart
    {
        public int Id { get; set; }

        [Required]

        public string? UserId { get; set; }
        public List<CartItem> CartItems { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
