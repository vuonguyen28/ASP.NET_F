﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class FShopEntities : DbContext
    {
        public FShopEntities()
            : base("name=FShopEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ChucNang> ChucNang { get; set; }
        public virtual DbSet<DanhGiaSanPham> DanhGiaSanPham { get; set; }
        public virtual DbSet<DanhMuc> DanhMuc { get; set; }
        public virtual DbSet<GiamGia> GiamGia { get; set; }
        public virtual DbSet<HinhAnhSanPham> HinhAnhSanPham { get; set; }
        public virtual DbSet<KichThuoc> KichThuoc { get; set; }
        public virtual DbSet<KhuyenMai> KhuyenMai { get; set; }
        public virtual DbSet<MauSac> MauSac { get; set; }
        public virtual DbSet<NhaCungCap> NhaCungCap { get; set; }
        public virtual DbSet<NHANVIEN> NHANVIEN { get; set; }
        public virtual DbSet<PhanQuyen> PhanQuyen { get; set; }
        public virtual DbSet<SanPham> SanPham { get; set; }
        public virtual DbSet<SanPhamDanhMuc> SanPhamDanhMuc { get; set; }
        public virtual DbSet<SanPhamGiamGia> SanPhamGiamGia { get; set; }
        public virtual DbSet<SanPhamKichThuoc> SanPhamKichThuoc { get; set; }
        public virtual DbSet<SanPhamKhuyenMai> SanPhamKhuyenMai { get; set; }
        public virtual DbSet<SanPhamMauSac> SanPhamMauSac { get; set; }
        public virtual DbSet<SanPhamNhaCungCap> SanPhamNhaCungCap { get; set; }
        public virtual DbSet<SanPhamThuongHieu> SanPhamThuongHieu { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<ThuongHieu> ThuongHieu { get; set; }
    }
}