using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopThoiTrang.Models;

namespace ShopThoiTrang.Controllers
{
    public class MainCustomerController : Controller
    {
        // GET: MainCustomer
        QL_SHOPTHOITRANG_DOANEntities db = new QL_SHOPTHOITRANG_DOANEntities();

        public ActionResult NotFound()
        {
            return View();
        }

        //trang chủ
        public ActionResult IndexProduct()
        {
            //var productList = db.SanPham.Take(8).ToList(); // cái này là xuất thông tin sản phẩm -- info product. kh co fix t
            var productList = db.LaySanPhamDuocDatNhieuNhat_top8().ToList();
            // Lấy ds 2 hình ảnh đầu tiên của mỗi sản phẩm và gán vào ViewBag
            var imagesPerProduct = db.HinhAnh.GroupBy(h => h.MaSp)
                                            .ToDictionary(
                                                g => g.Key,
                                                g => g.Select(h => h).AsEnumerable().Take(2).ToList()
                                            );
            ViewBag.Images = imagesPerProduct;

            

            return View(productList);
        }
       
        //Chi tiết sản phẩm
        public ActionResult DetailProduct(int MaSP)
        {
            SanPham product = db.SanPham
                                .Include("DanhMuc")
                                .Include("HinhAnh")
                                .FirstOrDefault(p => p.MaSP == MaSP);

            if (product == null)
            {
                return HttpNotFound();
            }

            var images = product.HinhAnh.ToList();

            ViewBag.ProductImages = images;

            var chiTietSanPham = db.ChiTietSanPham
                                   .Where(ctsp => ctsp.MaSP == MaSP)
                                   .ToList();

            ViewBag.ProductDetails = chiTietSanPham;

            return View(product);
        }

        //Chi tiết sản phẩm part2
        public ActionResult ProductDetailItem(int MaSP)
        {
            var chiTietSanPhams = db.ChiTietSanPham.Where(ctsp => ctsp.MaSP == MaSP).ToList();
            var sanPham = db.SanPham
                                .Include("DanhMuc")
                                .Include("NhaCungCap")
                                .FirstOrDefault(p => p.MaSP == MaSP);

            var hinhAnhs = db.HinhAnh.Where(ha => ha.MaSp == MaSP).ToList();

            if (sanPham != null && chiTietSanPhams != null && chiTietSanPhams.Any())
            {
                var model = new ProductView
                {
                    SanPham = sanPham,
                    ChiTietSanPhams = chiTietSanPhams,
                     HinhAnhs = hinhAnhs
                };


                // hien thi thong tin feedback

                var listFeedback = db.FeedBack_SanPham(MaSP).ToList();
                
                ViewBag.feedback = listFeedback;

                // lấy thông tin khách hàng
                foreach (var feedback in listFeedback)
                {
                    var customer = db.KHACHHANG.FirstOrDefault(kh => kh.MaKH == feedback.MaKH);
                    if (customer != null)
                    {
                        string avatar = customer.Avatar;
                        string tenKhachHang = customer.TenKH;
                        ViewBag.tenKH = tenKhachHang;
                        ViewBag.avatar = avatar;

                    }
                }

                // SO LUONG SAN PHAM BAN DUOC
                string query = "SELECT dbo.SoLuongSanPhamDaBan(@MaSP) AS SoLuongDaBan";

                var result = db.Database.SqlQuery<int>(  query,
                                    new SqlParameter("@MaSP", MaSP))
                                .FirstOrDefault();

                ViewBag.SoLuongDaBan = result;


                // điểm đánh giá của san phẩm
                string query1 = "SELECT dbo.DiemDanhGiaSanPham(@MaSP) AS DiemDanhGia";

                var diemdanhgia = db.Database.SqlQuery<double>(query1,
                                    new SqlParameter("@MaSP", MaSP))
                                .FirstOrDefault();

                ViewBag.diem = diemdanhgia;



                return View(model);
            }

            return View("NotFound"); 
        }

       

        //Show danh mục
        public ActionResult ShowDanhMuc()
        {
            var listDanhMuc = db.DanhMuc.OrderBy(d => d.TenDanhMuc);
            return View(listDanhMuc);
        }

     
        //ALL PRODUCT: Tất cả sản phẩm
        //public ActionResult Fiter_Product(int? maNhaCungCap, int? maDanhMuc, float? minPrice, float? maxPrice, string sortByName, string sortByPrice)
        //{
        //    var productList = db.LocSanPham(maNhaCungCap,maDanhMuc, minPrice,maxPrice, sortByName, sortByPrice).ToList();

        //    // Lấy ds 2 hình ảnh đầu tiên của mỗi sản phẩm và gán vào ViewBag
        //    var imagesPerProduct = db.HinhAnh.GroupBy(h => h.MaSp)
        //                                    .ToDictionary(
        //                                        g => g.Key,
        //                                        g => g.Select(h => h).AsEnumerable().Take(2).ToList()
        //                                    );
        //    ViewBag.Images = imagesPerProduct;

        //    return View(productList);
        //}


        public ActionResult Fiter_Product(int? maNhaCungCap, int? maDanhMuc, float? minPrice, float? maxPrice, string sortByName, string sortByPrice, string searchProduct)
        {
            var maNhaCungCapList = new SelectList(db.NhaCungCap.ToList(), "MaNhaCungCap", "TenNhaCungCap");
            ViewBag.MaNhaCungCap = maNhaCungCapList;

            var danhMucList = new SelectList(db.DanhMuc.ToList(), "MaDanhMuc", "TenDanhMuc");
            ViewBag.DanhMuc = danhMucList;

            var productList = db.Database.SqlQuery<LocSanPham_Result>(
                "EXEC LocSanPham @MaNhaCungCap, @MaDanhMuc, @minPrice, @maxPrice, @sortByName, @sortByPrice",
                new SqlParameter("@MaNhaCungCap", maNhaCungCap ?? (object)DBNull.Value),
                new SqlParameter("@MaDanhMuc", maDanhMuc ?? (object)DBNull.Value),
                new SqlParameter("@minPrice", minPrice ?? (object)DBNull.Value),
                new SqlParameter("@maxPrice", maxPrice ?? (object)DBNull.Value),
                new SqlParameter("@sortByName", string.IsNullOrEmpty(sortByName) ? (object)DBNull.Value : sortByName),
                new SqlParameter("@sortByPrice", string.IsNullOrEmpty(sortByPrice) ? (object)DBNull.Value : sortByPrice)
            ).ToList();

            if (!string.IsNullOrEmpty(searchProduct))
            {
                productList = productList.Where(x => x.TenSP.Contains(searchProduct)).ToList();
            }

            var imagesPerProduct = db.HinhAnh.GroupBy(h => h.MaSp)
                                             .ToDictionary(
                                                 g => g.Key,
                                                 g => g.Select(h => h).AsEnumerable().Take(2).ToList()
                                             );
            ViewBag.Images = imagesPerProduct;

            return View(productList);
        }


        //Thống kê đánh giá sao
        public JsonResult GetProductRating(int maSP)
        {
            var ratings = db.FeedBack_SanPham(maSP).ToList();

            // Tính toán số lượng đánh giá cho mỗi số sao
            var starCounts = Enumerable.Range(1, 5)
                                       .Select(star => ratings.Count(r => r.DanhGia == star))
                                       .ToList();

            return Json(starCounts, JsonRequestBehavior.AllowGet);
        }
    }
}