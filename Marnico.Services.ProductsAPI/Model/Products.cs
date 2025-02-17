﻿using System.ComponentModel.DataAnnotations;

namespace Marnico.Services.ProductsAPI.Model
{
    public class Products
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; }
        
        public double Price { get; set; }
        
        public string Description { get; set; }

        public string CategoryName { get; set; }

        public string ImageUrl { get; set; }
    }

}
