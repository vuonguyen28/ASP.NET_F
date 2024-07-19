using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopThoiTrang.Models
{
    public class DonHangViewModel
    {
        public DonHang DonHang { get; set; } // Thông tin của đơn hàng

        public List<ChiTietDonHang> ChiTietDonHangs { get; set; } // Danh sách các chi tiết đơn hàng
    }
}