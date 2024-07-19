using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopThoiTrang.Models;
using System.Data.SqlClient;
using System.Net;
using System.Data.Entity;
using System.IO;
using ShopThoiTrang.App_Start; //để dùng BinaryReader

namespace ShopThoiTrang.Areas.Admin.Controllers
{
    public class EmployeeController : Controller
    {
        QL_SHOPTHOITRANG_DOANEntities db = new QL_SHOPTHOITRANG_DOANEntities();
        //13	Xem_NhanVien
        //14	Sua_NhanVien
        //15	Them_NhanVien
        //16	Xoa_NhanVien
        // GET: Admin/Employee



        //13	Xem_NhanVien
        [AdminAuthorize(idChucNang = 13)]

        public ActionResult ShowEmpl(string searching)
        {
            var nv = db.NHANVIEN.Where(x => x.HoTenNV.Contains(searching) || searching == null).ToList();
            return View(nv);
        }

       

        //Create new employee

        //15	Them_NhanVien
        [AdminAuthorize(idChucNang = 15)]

        public ActionResult CreateEmpl()
        {
            return View();
        }




        //15	Them_NhanVien
        [AdminAuthorize(idChucNang = 15)]

        public ActionResult Create()
        {
            return View();
        }

        // POST: NhanVien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NHANVIEN newEmployee, HttpPostedFileBase avatarFile)
        {
            if (ModelState.IsValid)
            {
                // Giả sử bạn có một DbContext có tên là DOAN_QLSHOPEntities1
                using (QL_SHOPTHOITRANG_DOANEntities context = new QL_SHOPTHOITRANG_DOANEntities())
                {
                    try
                    {
                        // Xử lý tải lên tập tin hình đại diện
                        if (avatarFile != null && avatarFile.ContentLength > 0)
                        {
                            string fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(avatarFile.FileName);

                            // Xây dựng đường dẫn vật lý nơi tập tin sẽ được lưu trữ trên máy chủ
                            string path = Server.MapPath("~/Image/") + fileName;  // Thêm dấu gạch chéo (/) sau "~/Image/"

                            // Lưu tập tin đã tải lên vào đường dẫn đã chỉ định
                            avatarFile.SaveAs(path);

                            // Cập nhật đường dẫn hình đại diện của nhân viên trong mô hình
                            newEmployee.HinhAnhNV =  fileName;  // Thêm dấu gạch chéo (/) sau "Image/"
                        }

                        // Thêm nhân viên mới vào bối cảnh
                        context.NHANVIEN.Add(newEmployee);

                        // Lưu các thay đổi vào cơ sở dữ liệu
                        context.SaveChanges();

                        // Chuyển hướng đến trang index hoặc trang chi tiết sau khi tạo thành công
                        return RedirectToAction("ShowEmpl", "Employee"); // Thay thế bằng hành động mong muốn của bạn
                    }
                    catch (Exception ex)
                    {
                        // Xử lý các ngoại lệ, ghi log lỗi, v.v.
                        ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi tạo nhân viên.");
                    }
                }
            }

            // Nếu ModelState không hợp lệ, quay lại trang tạo với các lỗi kiểm tra hợp lệ
            return View(newEmployee);
        }


        //14	Sua_NhanVien
        [AdminAuthorize(idChucNang = 14)]

        [HttpGet]
        public ActionResult EditEmpl(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Assuming you have a DbContext called DOAN_QLSHOPEntities1
            using (QL_SHOPTHOITRANG_DOANEntities context = new QL_SHOPTHOITRANG_DOANEntities())
            {
                // Retrieve the employee with the specified id from the database
                NHANVIEN employee = context.NHANVIEN.Find(id);

                if (employee == null)
                {
                    return HttpNotFound();
                }

                // Pass the employee object to the view for editing
                return View(employee);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditEmpl(NHANVIEN editedEmployee, HttpPostedFileBase avatarFile)
        {
            if (ModelState.IsValid)
            {
                using (QL_SHOPTHOITRANG_DOANEntities context = new QL_SHOPTHOITRANG_DOANEntities())
                {
                    try
                    {
                        // Retrieve the original employee from the database
                        NHANVIEN originalEmployee = context.NHANVIEN.Find(editedEmployee.MaNV);

                        if (originalEmployee == null)
                        {
                            return HttpNotFound();
                        }

                        // Handle avatar file upload only if a new file is provided
                        if (avatarFile != null && avatarFile.ContentLength > 0)
                        {
                            string fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(avatarFile.FileName);
                            string path = Server.MapPath("~/Image/") + fileName;

                            avatarFile.SaveAs(path);

                            // Update the avatar path in the edited employee object
                            editedEmployee.HinhAnhNV =  fileName;
                        }
                        else
                        {
                            // If no new file is provided, keep the existing avatar path
                            editedEmployee.HinhAnhNV = originalEmployee.HinhAnhNV;
                        }

                        // Update other properties of the employee
                        originalEmployee.HoTenNV = editedEmployee.HoTenNV;
                        // Update other properties as needed...

                        // Mark the original employee as modified
                        context.Entry(originalEmployee).CurrentValues.SetValues(editedEmployee);

                        // Save changes to the database
                        context.SaveChanges();

                        // Kiểm tra xem dữ liệu hình ảnh đã được cập nhật chưa
                        var employeeWithImage = context.NHANVIEN.Find(editedEmployee.MaNV);
                        var imagePath = employeeWithImage.HinhAnhNV;

                        // Redirect to the index or details page after successful edit
                        return RedirectToAction("ShowEmpl", "Employee");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, "An error occurred while editing the employee.");
                    }
                }
            }

            // If ModelState is not valid, return to the edit view with validation errors
            return View(editedEmployee);
        }

        //16	Xoa_NhanVien
        [AdminAuthorize(idChucNang = 16)]

        [HttpPost]
        public ActionResult DeleteSelected(IEnumerable<int> selectedItems)
        {
            
            if (selectedItems != null && selectedItems.Any())
            {
                foreach (var itemId in selectedItems)
                {
                    // Kiểm tra xem có sản phẩm nào sử dụng MaDanhMuc này không
                    var isUsed = db.PhanQuyen.Any(sp => sp.MaNV == itemId);

                    if (isUsed)
                    {
                        // Hiển thị thông báo không thể xóa nếu MaDanhMuc này được sử dụng trong sản phẩm
                        TempData["ErrorMessage"] = "Không thể nhân viên  này vì đang được cấp quyền nhân viên.";
                        return RedirectToAction("ShowEmpl");
                    }
                }

                // Tiến hành xóa nếu không có sản phẩm nào sử dụng MaDanhMuc này
                var list = db.NHANVIEN.Where(x => selectedItems.Contains(x.MaNV)).ToList();
                db.NHANVIEN.RemoveRange(list);
                db.SaveChanges();
            }

            return RedirectToAction("ShowEmpl");
        }


       

    }
}