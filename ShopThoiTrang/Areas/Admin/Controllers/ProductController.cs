using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopThoiTrang.App_Start;
using ShopThoiTrang.Models;

namespace ShopThoiTrang.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        //21	Xem_SanPham
        //22	Them_SanPham
        //23	Xoa_SanPham
        //24	Sua_SanPham
        //25	Xem_HinhAnhSanPham
        //26	Xoa_HinhAnhSanPham
        //27	Them_HinhAnhSanPham
        QL_SHOPTHOITRANG_DOANEntities db = new QL_SHOPTHOITRANG_DOANEntities();

        // GET: Admin/Product
        //21	Xem_SanPham
        [AdminAuthorize(idChucNang = 21)]

        public ActionResult ShowProduct(string searching)
        {
            var products = db.SanPham
                .Include("DanhMuc")
                .Include("NhaCungCap")
                .Include("HinhAnh")
                .Where(x => x.TenSP.Contains(searching) || searching == null)
                .ToList();

            return View(products);
        }
        //22	Them_SanPham
        [AdminAuthorize(idChucNang = 22)]

        public ActionResult Create()
        {
            ViewBag.MaDanhMuc = new SelectList(db.DanhMuc, "MaDanhMuc", "TenDanhMuc");
            ViewBag.MaNhaCungCap = new SelectList(db.NhaCungCap, "MaNhaCungCap", "TenNhaCungCap");
            return View();
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]

        [ValidateInput(false)]
        public ActionResult Create(SanPham product, IEnumerable<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.SanPham.Add(product);
                    db.SaveChanges();

                    // Lưu hình ảnh nếu có
                    if (files != null && files.Any())
                    {
                        foreach (var file in files)
                        {
                            if (file != null && file.ContentLength > 0)
                            {
                                HinhAnh image = new HinhAnh();
                                image.MaSp = product.MaSP; // Gán MaSP của sản phẩm cho hình ảnh

                                // Tạo tên file mới sử dụng GUID
                                string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                                string fileExtension = Path.GetExtension(file.FileName);
                                string uniqueFileName = $"{fileName}_{Guid.NewGuid()}{fileExtension}";

                                // Lưu tên file vào cơ sở dữ liệu
                                image.hinhanh1 = uniqueFileName;

                                string path = Server.MapPath("~/Image");
                                string fullPath = Path.Combine(path, uniqueFileName);
                                file.SaveAs(fullPath); // Lưu hình ảnh vào thư mục trên server

                                // Lưu thông tin hình ảnh vào cơ sở dữ liệu
                                db.HinhAnh.Add(image);
                                db.SaveChanges();
                            }
                        }
                    }

                    return RedirectToAction("ShowProduct", "Product"); // Chuyển hướng về trang chủ sau khi tạo thành công
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi khi lưu sản phẩm hoặc hình ảnh nếu cần
                    ModelState.AddModelError("", "Lỗi khi tạo sản phẩm: " + ex.Message);
                }
            }

            // Nếu có lỗi, hiển thị lại form tạo sản phẩm với thông báo lỗi
            ViewBag.MaDanhMuc = new SelectList(db.DanhMuc, "MaDanhMuc", "TenDanhMuc", product.MaDanhMuc);
            ViewBag.MaNhaCungCap = new SelectList(db.NhaCungCap, "MaNhaCungCap", "TenNhaCungCap", product.MaNhaCungCap);
            return View(product);
        }


        public ActionResult ShowProductAndSupplier()
        {
            var listPS = db.NhaCungCap.Include("SanPham").ToList();
            return View(listPS);
        }
        public ActionResult ShowProductAndCategory()
        {
            var listPC = db.DanhMuc.Include("SanPham").ToList();
            return View(listPC);
        }



        //23	Xoa_SanPham
        [AdminAuthorize(idChucNang = 23)]

        [HttpPost]
        public ActionResult DeleteSelectedProducts(List<int> selectedIds)
        {
            try
            {
                if (selectedIds != null && selectedIds.Any())
                {
                    foreach (var id in selectedIds)
                    {
                        var productToDelete = db.SanPham.Find(id);

                        if (productToDelete != null)
                        {
                            // get to list of acttribute products that need to be deleted
                            var imagesToDelete = db.HinhAnh.Where(x => x.MaSp == id).ToList();

                            // delete all image from the folderon the server
                            foreach (var image in imagesToDelete)
                            {
                                string fullPath = Path.Combine(Server.MapPath("~/Image"), image.hinhanh1);
                                if (System.IO.File.Exists(fullPath))
                                {
                                    System.IO.File.Delete(fullPath);
                                }
                            }

                            // deleted all product in database
                            if (imagesToDelete.Any())
                            {
                                db.HinhAnh.RemoveRange(imagesToDelete);
                            }

                            // delete product
                            db.SanPham.Remove(productToDelete);
                        }
                    }

                    db.SaveChanges();
                    return Json("Success"); // return "Success" if deleted successfully
                }
                else
                {
                    return Json("Failure"); // return "Failure" if not selected
                }
            }
            catch (Exception ex)
            { // Ghi lại chi tiết ngoại lệ để gỡ lỗi
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

                return Json("Failure"); //return "Failure" if where was an error during deletion 
            }
        }

        //24	Sua_SanPham
        [AdminAuthorize(idChucNang = 24)]

        public ActionResult EditProduct(int MaSP)
        {
            var product = db.SanPham.Find(MaSP);
            if (product == null)
            {
                return HttpNotFound();
            }

            var danhMucList = db.DanhMuc.ToList(); // Lấy danh sách danh mục
            var nhaCungCapList = db.NhaCungCap.ToList(); // Lấy danh sách nhà cung cấp

            ViewBag.MaDanhMuc = new SelectList(danhMucList, "MaDanhMuc", "TenDanhMuc", product.MaDanhMuc);
            ViewBag.MaNhaCungCap = new SelectList(nhaCungCapList, "MaNhaCungCap", "TenNhaCungCap", product.MaNhaCungCap);

            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditProduct(SanPham updatedProduct)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingProduct = db.SanPham.Find(updatedProduct.MaSP);
                    if (existingProduct == null)
                    {
                        return HttpNotFound();
                    }

                    existingProduct.TenSP = updatedProduct.TenSP;
                    existingProduct.Gia = updatedProduct.Gia;
                    existingProduct.PhanTramGiamGia = updatedProduct.PhanTramGiamGia;
                    existingProduct.MoTa = updatedProduct.MoTa;

                    // Lấy tên danh mục và tên nhà cung cấp từ cơ sở dữ liệu dựa trên mã
                    existingProduct.DanhMuc = db.DanhMuc.FirstOrDefault(d => d.MaDanhMuc == updatedProduct.MaDanhMuc);
                    existingProduct.NhaCungCap = db.NhaCungCap.FirstOrDefault(n => n.MaNhaCungCap == updatedProduct.MaNhaCungCap);

                    db.SaveChanges();


                    return RedirectToAction("ShowProduct");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi cập nhật sản phẩm: " + ex.Message);
            }

            return View(updatedProduct);
        }
        //25	Xem_HinhAnhSanPham
    
        public ActionResult ShowProductImage(int MaSP)
        {
            return View(db.HinhAnh.Where(x => x.MaSp == MaSP).ToList());
        }

        //ví dụ
        //public ActionResult ShowProductImage(int maSP)
        //{

        //    int.TryParse(Request.QueryString["MaSP"], out maSP);
        //    ViewBag.MaSPValue = maSP;
        //    return View(db.HinhAnh.Where(x => x.MaSp == maSP).ToList());
        //}


        //27	Them_HinhAnhSanPham
        [AdminAuthorize(idChucNang = 27)]

        public ActionResult CreateProductImage(int MaSP)
        {
            ViewBag.MaSP = MaSP; // Truyền MaSP vào view để có thể sử dụng trong form
            return View();
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult CreateProductImage(IEnumerable<HttpPostedFileBase> file, int MaSP)
        //{
        //    if (file != null && file.Any()) // kt file có được gửi lên hay ko
        //    {
        //        foreach (var uploadedFile in file)
        //        {
        //            if (uploadedFile != null && uploadedFile.ContentLength > 0)
        //            {
        //                try
        //                {
        //                    // Tạo một đối tượng hình ảnh mới
        //                    HinhAnh image = new HinhAnh();
        //                    image.MaSp = MaSP;

        //                    // Lấy tên file và thêm thời gian vào để tạo tên file duy nhất
        //                    string fileName = Path.GetFileName(uploadedFile.FileName);
        //                    string uniqueFileName = $"{DateTime.Now.Ticks}_{fileName}";

        //                    // Lưu tên file duy nhất vào đối tượng hình ảnh
        //                    image.hinhanh1 = uniqueFileName;

        //                    string path = Server.MapPath("~/Image");
        //                    string filePath = Path.Combine(path, uniqueFileName);
        //                    uploadedFile.SaveAs(filePath);

        //                    // Lưu thông tin hình ảnh vào cơ sở dữ liệu
        //                    db.HinhAnh.Add(image);
        //                    db.SaveChanges();
        //                }
        //                catch (Exception ex)
        //                {
        //                   // xử lý lõi xảy ra
        //                    ModelState.AddModelError("", "Lỗi khi tải lên hình ảnh: " + ex.Message);
        //                }
        //            }
        //        }

        //        return RedirectToAction("ShowProduct");
        //    }

        //    return View();
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProductImage(IEnumerable<HttpPostedFileBase> file, int MaSP)
        {
            if (file != null && file.Any()) // Kiểm tra xem có file được gửi lên không
            {
                try
                {
                    foreach (var uploadedFile in file)
                    {
                        if (uploadedFile != null && uploadedFile.ContentLength > 0)
                        {
                            // Tạo một đối tượng hình ảnh mới
                            HinhAnh image = new HinhAnh();
                            image.MaSp = MaSP;

                            // Tạo tên file duy nhất sử dụng GUID
                            string fileName = Path.GetFileName(uploadedFile.FileName);
                            string fileExtension = Path.GetExtension(fileName);
                            string uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";

                            // Lưu tên file duy nhất vào đối tượng hình ảnh
                            image.hinhanh1 = uniqueFileName;

                            string path = Server.MapPath("~/Image");
                            string filePath = Path.Combine(path, uniqueFileName);
                            uploadedFile.SaveAs(filePath);

                            // Lưu thông tin hình ảnh vào cơ sở dữ liệu
                            db.HinhAnh.Add(image);
                            db.SaveChanges();
                        }
                    }

                    return RedirectToAction("ShowProduct");
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi xảy ra khi lưu hình ảnh
                    ModelState.AddModelError("", "Lỗi khi tải lên hình ảnh: " + ex.Message);
                }
            }

            return View();
        }





        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteSelectedImages(IEnumerable<int> selectedImages)
        //{
        //    if (selectedImages != null && selectedImages.Any())
        //    {
        //        try
        //        {
        //            // Lấy danh sách các hình ảnh được chọn từ MaHinhAnh
        //            var imagesToDelete = db.HinhAnh.Where(x => selectedImages.Contains(x.MaHinhAnh)).ToList();

        //            if (imagesToDelete.Any())
        //            {
        //                // Xóa tất cả hình ảnh đã chọn từ CSDL và thư mục trên server
        //                foreach (var image in imagesToDelete)
        //                {
        //                    string fullPath = Path.Combine(Server.MapPath("~/Image"), image.hinhanh1);
        //                    if (System.IO.File.Exists(fullPath))
        //                    {
        //                        System.IO.File.Delete(fullPath);
        //                    }
        //                }

        //                // Xóa các hình ảnh đã chọn khỏi CSDL
        //                db.HinhAnh.RemoveRange(imagesToDelete);
        //                db.SaveChanges();
        //            }

        //            return RedirectToAction("ShowImages"); // Chuyển hướng về trang hiển thị hình ảnh sau khi xóa thành công
        //        }
        //        catch (Exception ex)
        //        {
        //            // Xử lý lỗi khi xóa hình ảnh nếu cần
        //            ModelState.AddModelError("", "Lỗi khi xóa hình ảnh: " + ex.Message);
        //        }
        //    }

        //    return RedirectToAction(""); // Chuyển hướng về trang hiển thị hình ảnh nếu không có hình ảnh nào được chọn hoặc có lỗi xảy ra
        //}



        //26	Xoa_HinhAnhSanPham
        [AdminAuthorize(idChucNang = 26)]


        [HttpPost]
        public ActionResult DeleteImages(IEnumerable<int> MaHinhAnhList)
        {
            try
            {
                if (MaHinhAnhList != null && MaHinhAnhList.Any())
                {
                    foreach (var MaHinhAnh in MaHinhAnhList)
                    {
                        var imageToDelete = db.HinhAnh.FirstOrDefault(i => i.MaHinhAnh == MaHinhAnh);
                        if (imageToDelete != null)
                        {
                            var imagePath = Server.MapPath("~/Image/" + imageToDelete.hinhanh1);

                            db.HinhAnh.Remove(imageToDelete);
                            db.SaveChanges();

                            if (System.IO.File.Exists(imagePath))
                            {
                                System.IO.File.Delete(imagePath);
                            }
                        }
                    }

                    return Json("Success");
                }
                else
                {
                    return Json("NoImagesSelected");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi xóa danh sách hình ảnh:");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return Json("Error");
            }
        }

        // show chi tiết sản phẩm
        public ActionResult ShowProductItem(int MaSP)
        {
            return View(db.ChiTietSanPham.Where(x => x.MaSP == MaSP).ToList());
        }

        // tạo mới 1 chi tiết sản phẩm
        public ActionResult CreateProductItem(int MaSP)
        {
            ViewBag.MaSP = MaSP; // Truyền MaSP vào view để có thể sử dụng trong form
            return View();
        }

        // này ko phần quyền xem chi tiết sản phẩm


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProductItem(int maSP, int maKichThuoc, int maMau, int soLuongTon)
        {
            // Tạo một đối tượng ChiTietSanPham mới
            ChiTietSanPham newChiTietSanPham = new ChiTietSanPham
            {
                MaSP = maSP,
                MaKichThuoc = maKichThuoc,
                MaMau = maMau,
                SoLuongTon = soLuongTon
            };

            try
            {
                // Thêm ChiTietSanPham mới vào DbContext
                db.ChiTietSanPham.Add(newChiTietSanPham);
                db.SaveChanges();

                return RedirectToAction("ShowProductItem", "Product"); 
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi: " + ex.Message;
                return View();
            }
        }
    }


}


