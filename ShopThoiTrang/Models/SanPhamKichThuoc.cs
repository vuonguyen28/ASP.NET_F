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
    
    public partial class SanPhamKichThuoc
    {
        public int MaSanPhamKichThuoc { get; set; }
        public Nullable<int> MaSanPham { get; set; }
        public Nullable<int> MaKichThuoc { get; set; }
        public int SoLuongTheoKichThuoc { get; set; }
    
        public virtual KichThuoc KichThuoc { get; set; }
        public virtual SanPham SanPham { get; set; }
    }
}
