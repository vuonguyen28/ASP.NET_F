using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopThoiTrang.App_Start;
using ShopThoiTrang.Models;

namespace ShopThoiTrang.Areas.Admin.Controllers
{
    public class SupplierController : Controller
    {


        //32	Xem_NhaCungCap
        //33	Xoa_NhaCungCap
        //34	ThemNhacungcao
        //35	SuaNhaCungCap
        QL_SHOPTHOITRANG_DOANEntities db = new QL_SHOPTHOITRANG_DOANEntities();

        // GET: Admin/Supplier : nha cung cấp

        //32	Xem_NhaCungCap


        [AdminAuthorize(idChucNang = 32)]

        public ActionResult Show(string searching)
        {
            return View(db.NhaCungCap.Where(x => x.TenNhaCungCap.Contains(searching) || searching == null).ToList());
        }


        //34	ThemNhacungcao

        [AdminAuthorize(idChucNang = 34)]

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NhaCungCap ncc)
        {
            if (ModelState.IsValid)
            {
                db.NhaCungCap.Add(ncc);
                db.SaveChanges();
                return RedirectToAction("Show");
            }
            return View(ncc);
        }


        //35	SuaNhaCungCap
        [AdminAuthorize(idChucNang = 35)]

        public ActionResult Edit(int id = 0)
        {
            NhaCungCap ncc = db.NhaCungCap.Single(d => d.MaNhaCungCap == id);
            if (ncc == null)
            {
                return HttpNotFound();
            }
            return View(ncc);
        }
        [HttpPost]
        public ActionResult Edit(NhaCungCap ncc)
        {
            if (ModelState.IsValid)
            {
                db.NhaCungCap.Attach(ncc);
                db.Entry(ncc).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Show", "Supplier");
            }
            return View(ncc);
        }


        //33	Xoa_NhaCungCap
        [AdminAuthorize(idChucNang = 33)]

        [HttpPost]
        public ActionResult DeleteSelected(IEnumerable<int> selectedItems)
        {
            if (selectedItems != null && selectedItems.Any())
            {
                foreach (var itemId in selectedItems)
                {
                    // Kiểm tra xem có sản phẩm nào sử dụng MaDanhMuc này không
                    var isUsed = db.SanPham.Any(sp => sp.MaNhaCungCap == itemId);

                    if (isUsed)
                    {
                        // Hiển thị thông báo không thể xóa nếu MaDanhMuc này được sử dụng trong sản phẩm
                        TempData["ErrorMessage"] = "Không thể xóa màu  này vì có sản phẩm sử dụng.";
                        return RedirectToAction("Show");
                    }
                }

                // Tiến hành xóa nếu không có sản phẩm nào sử dụng MaDanhMuc này
                var list = db.NhaCungCap.Where(x => selectedItems.Contains(x.MaNhaCungCap)).ToList();
                db.NhaCungCap.RemoveRange(list);
                db.SaveChanges();
            }

            return RedirectToAction("Show");
        }

    }
}





