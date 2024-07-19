using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopThoiTrang.Models;
using ShopThoiTrang.App_Start;
using System.Diagnostics;
using System.Data.Entity.Infrastructure;

namespace ShopThoiTrang.Areas.Admin.Controllers
{
    public class ItemProductController : Controller
    {

        //17	Xem_ChiTietSanPham
        //18	Xoa_ChiTietSanPham
        //19	Sua_ChiTietSanPham _ chưa làm chức năng này
        //20	Them_chiTietSanPham
        // GET: Admin/ItemProduct
        QL_SHOPTHOITRANG_DOANEntities db = new QL_SHOPTHOITRANG_DOANEntities();

        //17	Xem_ChiTietSanPham
        [AdminAuthorize(idChucNang = 17)]
        public ActionResult Show(string searching)
        {
            var listItemProduct = db.ChiTietSanPham
                .Include("SanPham")
                .Include("MauSac")
                .Include("KichThuoc")
                .Include("SanPham.DanhMuc")
                .Where(x => x.SanPham.TenSP.Contains(searching) || searching == null)
                .ToList();

            return View(listItemProduct);
        }


        //20	Them_chiTietSanPham
        [AdminAuthorize(idChucNang = 20)]
        public ActionResult Create()
        {
            ViewBag.MaMau = new SelectList(db.MauSac, "MaMau", "TenMau");
            ViewBag.MaKichThuoc = new SelectList(db.KichThuoc, "MaKichThuoc", "TenKichThuoc");
            ViewBag.MaSP = new SelectList(db.SanPham, "MaSP", "TenSP");


            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ChiTietSanPham itemPro)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem MaSP, MaMau, MaKichThuoc đã tồn tại trong cơ sở dữ liệu chưa
                bool existingSP = db.SanPham.Any(s => s.MaSP == itemPro.MaSP);
                bool existingMau = db.MauSac.Any(m => m.MaMau == itemPro.MaMau);
                bool existingKichThuoc = db.KichThuoc.Any(k => k.MaKichThuoc == itemPro.MaKichThuoc);

                if (existingSP && existingMau && existingKichThuoc)
                {
                    // Tất cả các khóa ngoại đã tồn tại, có thể thêm ChiTietSanPham mới
                    db.ChiTietSanPham.Add(itemPro);
                    db.SaveChanges();
                    return RedirectToAction("Show");
                }
                else
                {
                    // Hiển thị thông báo hoặc xử lý theo ý muốn của bạn nếu không tồn tại các khóa ngoại
                    ViewBag.ErrorMessage = "Không thể tạo ChiTietSanPham vì MaSP, MaMau hoặc MaKichThuoc không tồn tại.";
                    // Trả về view với model và thông báo lỗi
                    ViewBag.MaMau = new SelectList(db.MauSac, "MaMau", "TenMau", itemPro.MaMau);
                    ViewBag.MaKichThuoc = new SelectList(db.KichThuoc, "MaKichThuoc", "TenKichThuoc", itemPro.MaKichThuoc);
                    ViewBag.MaSP = new SelectList(db.SanPham, "MaSP", "TenSP", itemPro.MaSP);
                    return View(itemPro);
                }
            }

            // ModelState không hợp lệ, trả về view với model và thông báo lỗi
            ViewBag.MaMau = new SelectList(db.MauSac, "MaMau", "TenMau", itemPro.MaMau);
            ViewBag.MaKichThuoc = new SelectList(db.KichThuoc, "MaKichThuoc", "TenKichThuoc", itemPro.MaKichThuoc);
            ViewBag.MaSP = new SelectList(db.SanPham, "MaSP", "TenSP", itemPro.MaSP);
            return View(itemPro);
        }



        //dropdow ItemProductAndColor
        public ActionResult ShowItemProductAndColor()
        {
            var listColor = db.MauSac.Include("ChiTietSanPham").ToList();
            return View(listColor);
        }

        //dropdow ItemProductAndColor
        public ActionResult ShowItemProductAndSize()
        {
            var listSize = db.KichThuoc.Include("ChiTietSanPham").ToList();
            return View(listSize);
        }
        //dropdow ItemProductAndProduct
        public ActionResult ShowItemProductAndPorduct()
        {
            var listProduct = db.SanPham.Include("ChiTietSanPham").ToList();
            return View(listProduct);
        }



        //18	Xoa_ChiTietSanPham
        [AdminAuthorize(idChucNang = 18)]
        [HttpPost]
        public ActionResult DeleteSelected(IEnumerable<int> selectedItems)
        {
            if (selectedItems != null && selectedItems.Any())
            {
                foreach (var itemId in selectedItems)
                {
                    var item = db.ChiTietSanPham.Find(itemId);

                    if (item != null)
                    {
                        // Kiểm tra xem có chi tiết đơn hàng liên quan
                        var hasRelatedOrderDetails = db.ChiTietDonHang.Any(cd => cd.MaChiTietSanPham == itemId);

                        if (hasRelatedOrderDetails)
                        {
                            TempData["ErrorMessage"] = "Không thể xóa chi tiết sản phẩm vì có chi tiết đơn hàng liên quan.";
                            return RedirectToAction("Show");
                        }

                        // Tiến hành xóa chi tiết sản phẩm
                        db.ChiTietSanPham.Remove(item);
                    }
                }

                db.SaveChanges();

                TempData["SuccessMessage"] = "Xóa chi tiết sản phẩm thành công.";
            }

            return RedirectToAction("Show");
        }




        // chinhr sua thogn tin san pham
        public ActionResult EditProductDetail(int id)
        {
            // Lấy thông tin chi tiết sản phẩm từ cơ sở dữ liệu dựa trên id
            var productDetail = db.ChiTietSanPham.FirstOrDefault(p => p.MaChiTiet == id);

            if (productDetail == null)
            {
                return HttpNotFound(); // Xử lý nếu không tìm thấy chi tiết sản phẩm
            }

            ViewBag.MaMau = new SelectList(db.MauSac, "MaMau", "TenMau");
            ViewBag.MaKichThuoc = new SelectList(db.KichThuoc, "MaKichThuoc", "TenKichThuoc");
            ViewBag.MaSP = new SelectList(db.SanPham, "MaSP", "TenSP");


            return View(productDetail); // Trả về view để chỉnh sửa chi tiết sản phẩm
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProductDetail(ChiTietSanPham editedProductDetail)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Tìm chi tiết sản phẩm gốc trong cơ sở dữ liệu
                    var originalProductDetail = db.ChiTietSanPham.FirstOrDefault(p => p.MaChiTiet == editedProductDetail.MaChiTiet);

                    if (originalProductDetail == null)
                    {
                        return HttpNotFound(); // Xử lý nếu không tìm thấy chi tiết sản phẩm
                    }

                    // Cập nhật thông tin chi tiết sản phẩm từ form chỉnh sửa
                    originalProductDetail.MaSP = editedProductDetail.MaSP;
                    originalProductDetail.MaKichThuoc = editedProductDetail.MaKichThuoc;
                    originalProductDetail.MaMau = editedProductDetail.MaMau;
                    originalProductDetail.SoLuongTon = editedProductDetail.SoLuongTon;

                    // Lưu các thay đổi vào cơ sở dữ liệu
                    db.SaveChanges();

                    // Chuyển hướng đến trang hoặc action mong muốn sau khi chỉnh sửa thành công
                    return RedirectToAction("Show", "ItemProduct"); // Ví dụ: trở về trang chủ
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi cập nhật chi tiết sản phẩm.");
                }
            }

            // Nếu ModelState không hợp lệ, quay lại view chỉnh sửa với các lỗi kiểm tra
            return View(editedProductDetail);
        }

    }
}