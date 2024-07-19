using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopThoiTrang.Models
{
    public class ProductView
    {
        public SanPham SanPham { get; set; }
        public List<ChiTietSanPham> ChiTietSanPhams { get; set; }
        public List<HinhAnh> HinhAnhs { get; set; }
    }
}