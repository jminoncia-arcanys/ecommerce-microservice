﻿namespace ProductCatalogMicroservice.Model
{
    public class CartItem
    {
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int UserId { get; set; } 
    }
}
