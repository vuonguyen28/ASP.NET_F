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
    public class CategoryController : Controller
    {
        QL_SHOPTHOITRANG_DOANEntities db = new QL_SHOPTHOITRANG_DOANEntities();

        // GET: Admin/Category

        //Phân quyền
        //1	Thêm_DanhMuc
        //2	Xem_DanhMuc
        //3	Xóa_DanhMuc
        //4	Sửa_DanhMuc


        //2	Xem_DanhMuc
        [AdminAuthorize(idChucNang =2)]
        public ActionResult Index(string searching)
        {
            return View(db.DanhMuc.Where(x=>x.TenDanhMuc.Contains(searching) || searching==null).ToList());
        }


        //1	Thêm_DanhMuc
        [AdminAuthorize(idChucNang = 1)]
        public ActionResult CreateCategory()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCategory(DanhMuc danhmuc)
        {
            if(ModelState.IsValid)
            {
                db.DanhMuc.Add(danhmuc);
                db.SaveChanges();
                return RedirectToAction("Index");
            }    
            return View(danhmuc);
        }


        //4	Sửa_DanhMuc
        [AdminAuthorize(idChucNang = 4)] 
        public ActionResult EditCategory(int id =0)
        {
            DanhMuc cate = db.DanhMuc.Single(d => d.MaDanhMuc == id);
            if(cate == null)
            {
                return HttpNotFound();
            }
            return View(cate);
        }
        [HttpPost]
        public ActionResult EditCategory(DanhMuc category)
        {
            if (ModelState.IsValid)
            {
                db.DanhMuc.Attach(category);
                db.Entry(category).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Category");
            }
            return View(category);
        }


        //3	Xóa_DanhMuc
        [AdminAuthorize(idChucNang = 3)]
        [HttpPost]
        public ActionResult DeleteSelected(IEnumerable<int> selectedItems)
        {
            if (selectedItems != null && selectedItems.Any())
            {
                foreach (var itemId in selectedItems)
                {
                    // Kiểm tra xem có sản phẩm nào sử dụng MaDanhMuc này không
                    var isUsed = db.SanPham.Any(sp => sp.MaDanhMuc == itemId);

                    if (isUsed)
                    {
                        // Hiển thị thông báo không thể xóa nếu MaDanhMuc này được sử dụng trong sản phẩm
                        TempData["ErrorMessage"] = "Không thể xóa danh mục này vì có sản phẩm sử dụng.";
                        return RedirectToAction("Index");
                    }
                }

                // Tiến hành xóa nếu không có sản phẩm nào sử dụng MaDanhMuc này
                var listCategory = db.DanhMuc.Where(x => selectedItems.Contains(x.MaDanhMuc)).ToList();
                db.DanhMuc.RemoveRange(listCategory);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }




    }




}
