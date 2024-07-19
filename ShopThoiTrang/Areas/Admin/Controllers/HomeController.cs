using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ShopThoiTrang.App_Start;
using ShopThoiTrang.Models;

namespace ShopThoiTrang.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        QL_SHOPTHOITRANG_DOANEntities db = new QL_SHOPTHOITRANG_DOANEntities();
         // GET: Admin/Home
        [AdminAuthorize(idChucNang = 31)]
        public ActionResult Index()
        {
            DateTime ngayHienTai = DateTime.Now.Date;

            // Tính ngày bắt đầu và kết thúc của ngày hiện tại
            DateTime ngayBatDauNgay = ngayHienTai.Date;
            DateTime ngayKetThucNgay = ngayHienTai.Date.AddDays(1).AddTicks(-1);

            // Truy vấn để lấy tổng tiền sau khi giảm giá trong ngày
            double tongTienSauGiamGiaTrongNgay = db.DonHang
                .Where(dh => dh.NgayDat != null && dh.NgayDat >= ngayBatDauNgay && dh.NgayDat <= ngayKetThucNgay && dh.TongTienSauKhiGiamGia != null)
                .Sum(dh => dh.TongTienSauKhiGiamGia.Value);

            // Đặt kết quả vào ViewBag cho tổng tiền giảm giá trong ngày
            ViewBag.TongTienSauGiamGiaTrongNgay = tongTienSauGiamGiaTrongNgay;

            // Tính ngày đầu tiên và cuối cùng của tuần hiện tại
            DateTime ngayDauTuan = ngayHienTai.AddDays(-(int)ngayHienTai.DayOfWeek);
            DateTime ngayCuoiTuan = ngayDauTuan.AddDays(6).AddDays(1).AddTicks(-1);

            // Truy vấn để lấy tổng tiền sau khi giảm giá trong tuần
            double tongTienSauGiamGiaTrongTuan = db.DonHang
                .Where(dh => dh.NgayDat != null && dh.NgayDat >= ngayDauTuan && dh.NgayDat <= ngayCuoiTuan && dh.TongTienSauKhiGiamGia != null)
                .Sum(dh => dh.TongTienSauKhiGiamGia.Value);

            // Đặt kết quả vào ViewBag cho tổng tiền giảm giá trong tuần
            ViewBag.TongTienSauGiamGiaTrongTuan = tongTienSauGiamGiaTrongTuan;

            // Tính ngày đầu tiên và cuối cùng của tháng hiện tại
            DateTime ngayDauThang = new DateTime(ngayHienTai.Year, ngayHienTai.Month, 1);
            DateTime ngayCuoiThang = ngayDauThang.AddMonths(1).AddDays(-1).AddDays(1).AddTicks(-1);

            // Truy vấn để lấy tổng tiền sau khi giảm giá trong tháng
            double tongTienSauGiamGiaTrongThang = db.DonHang
                .Where(dh => dh.NgayDat != null && dh.NgayDat >= ngayDauThang && dh.NgayDat <= ngayCuoiThang && dh.TongTienSauKhiGiamGia != null)
                .Sum(dh => dh.TongTienSauKhiGiamGia.Value);

            // Đặt kết quả vào ViewBag cho tổng tiền giảm giá trong tháng
            ViewBag.TongTienSauGiamGiaTrongThang = tongTienSauGiamGiaTrongThang;

            // Tính ngày đầu tiên và cuối cùng của năm hiện tại
            DateTime ngayDauNam = new DateTime(ngayHienTai.Year, 1, 1);
            DateTime ngayCuoiNam = ngayDauNam.AddYears(1).AddDays(-1).AddDays(1).AddTicks(-1);

            // Truy vấn để lấy tổng tiền sau khi giảm giá trong năm
            double tongTienSauGiamGiaTrongNam = db.DonHang
                .Where(dh => dh.NgayDat != null && dh.NgayDat >= ngayDauNam && dh.NgayDat <= ngayCuoiNam && dh.TongTienSauKhiGiamGia != null)
                .Sum(dh => dh.TongTienSauKhiGiamGia.Value);

            // Đặt kết quả vào ViewBag cho tổng tiền giảm giá trong năm
            ViewBag.TongTienSauGiamGiaTrongNam = tongTienSauGiamGiaTrongNam;

            // đêm số lượng khách hàng
            int totalCustomers = db.KHACHHANG.Count();
            ViewBag.SoLuongTK_Khach = totalCustomers;

            // đêm số lượng đơn hàng
            int totalBills = db.DonHang.Count();
            ViewBag.SoLuongBills = totalBills;

            // đêm số lượng đơn hàng
            int totalVoucher = db.VOUCHER_KHACHHANG.Count();
            ViewBag.SoLuongVoucher = totalVoucher;

            // đêm số lượng sản phẩm
            int totalProduct = db.SanPham.Count();
            ViewBag.SLSANPHAM = totalProduct;

            return View();
        }

        public ActionResult CanNotAccess()
        {
            return View();
        }

        public ActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(string user, string password)
        {
            var nhanvien = db.NHANVIEN.SingleOrDefault(m => m.Username.ToLower() == user.ToLower() && m.Password == password);

            if (nhanvien != null )
            {
                Session["user"] = nhanvien;

                // Lưu mã id nhân viên vào Session
                Session["MaNV"] = nhanvien.MaNV;

                return RedirectToAction("index");
            }
            else
            {
                TempData["error"] = "Tài Khoản Đăng Nhập Không Đúng";
                return View();
            }
        }

        //Load info employ to profile
        public ActionResult ShowProfile()
        {
            // Kiểm tra xem Session["MaNV"] có tồn tại không
            if (Session["MaNV"] != null)
            {
                int maNV = Convert.ToInt32(Session["MaNV"]);

                // Truy vấn cơ sở dữ liệu để lấy thông tin nhân viên dựa vào mã nhân viên
                var nhanvien = db.NHANVIEN.SingleOrDefault(m => m.MaNV == maNV);
                // Kiểm tra xem nhân viên có tồn tại không
                if (nhanvien != null)
                {
                    // Chuyển hướng đến trang hiển thị thông tin nhân viên
                    return View(nhanvien);
                }
            }

            // Nếu không có Session["MaNV"] hoặc không tìm thấy nhân viên, chuyển hướng về trang đăng nhập
            return RedirectToAction("DangNhap");
        }


        //Edit profile
        [HttpGet]
        public ActionResult EditProfile()
        {
            // Lấy giá trị MaNV từ session
            int? id = (int?)Session["MaNV"];

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (QL_SHOPTHOITRANG_DOANEntities context = new QL_SHOPTHOITRANG_DOANEntities())
            {
                NHANVIEN employee = context.NHANVIEN.Find(id);

                if (employee == null)
                {
                    return HttpNotFound();
                }

                return View(employee);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(NHANVIEN editedEmployee, HttpPostedFileBase avatarFile)
        {
            if (ModelState.IsValid)
            {
                using (QL_SHOPTHOITRANG_DOANEntities context = new QL_SHOPTHOITRANG_DOANEntities())
                {
                    try
                    {
                        int? id = (int?)Session["MaNV"];

                        if (id == null)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }

                        NHANVIEN originalEmployee = context.NHANVIEN.Find(id);

                        if (originalEmployee == null)
                        {
                            return HttpNotFound();
                        }

                        // Xác định xem có file avatar mới được cung cấp không
                        if (avatarFile != null && avatarFile.ContentLength > 0)
                        {
                            string fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(avatarFile.FileName);
                            string path = Server.MapPath("~/Image/") + fileName;

                            avatarFile.SaveAs(path);

                            // Cập nhật đường dẫn avatar trong đối tượng nhân viên đã chỉnh sửa
                            editedEmployee.HinhAnhNV = fileName;
                        }
                        else
                        {
                            // Nếu không có file mới được cung cấp, giữ nguyên đường dẫn avatar hiện tại
                            editedEmployee.HinhAnhNV = originalEmployee.HinhAnhNV;
                        }

                        // Cập nhật các thuộc tính khác của nhân viên
                        originalEmployee.HoTenNV = editedEmployee.HoTenNV;
                        // Cập nhật các thuộc tính khác nếu cần...

                        // Đánh dấu nhân viên gốc là đã được chỉnh sửa
                        context.Entry(originalEmployee).CurrentValues.SetValues(editedEmployee);

                        // Lưu các thay đổi vào cơ sở dữ liệu
                        context.SaveChanges();

                        // Kiểm tra xem dữ liệu hình ảnh đã được cập nhật chưa
                        var employeeWithImage = context.NHANVIEN.Find(id);
                        var imagePath = employeeWithImage.HinhAnhNV;

                        // Chuyển hướng đến trang index hoặc details sau khi chỉnh sửa thành công
                        ModelState.AddModelError(string.Empty, "Luwu thanh cong.");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi chỉnh sửa nhân viên.");
                    }
                }
            }

            // Nếu ModelState không hợp lệ, quay lại view chỉnh sửa với các lỗi kiểm tra
            return View(editedEmployee);
        }

        public ActionResult DangXuat()
        {
            Session.Clear(); 
            return RedirectToAction("DangNhap"); 
        }

    }
}