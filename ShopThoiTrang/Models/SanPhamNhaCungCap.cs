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
    
    public partial class SanPhamNhaCungCap
    {
        public int MaSanPhamNhaCungCap { get; set; }
        public Nullable<int> MaSanPham { get; set; }
        public Nullable<int> MaNhaCungCap { get; set; }
        public decimal GiaNhaCungCap { get; set; }
    
        public virtual NhaCungCap NhaCungCap { get; set; }
        public virtual SanPham SanPham { get; set; }
    }
}