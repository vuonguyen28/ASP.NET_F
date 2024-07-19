using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopThoiTrang.Models
{
    public class ShoppingCartItemViewModel
    {
        public int CartItemId { get; set; }
        public string ProductName { get; set; }
        public double? Price { get; set; }
        public double? Sale { get; set; }

        public List<string> Images { get; set; }
        public int? Quantity { get; set; }
        public string ColorName { get; set; }
        public string SizeName { get; set; }
        public double? TotalPrice { get; set; }
        public string ship { get; set; }

    }

    public class ShoppingCartViewModel
    {
        public List<ShoppingCartItemViewModel> CartItems { get; set; }
        public double? TotalCartPrice { get; set; }

    }


}