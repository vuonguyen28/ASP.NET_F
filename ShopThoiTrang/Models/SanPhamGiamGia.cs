//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShopThoiTrang.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SanPhamGiamGia
    {
        public int MaSanPhamGiamGia { get; set; }
        public Nullable<int> MaSanPham { get; set; }
        public Nullable<int> MaGiamGia { get; set; }
    
        public virtual GiamGia GiamGia { get; set; }
        public virtual SanPham SanPham { get; set; }
    }
}