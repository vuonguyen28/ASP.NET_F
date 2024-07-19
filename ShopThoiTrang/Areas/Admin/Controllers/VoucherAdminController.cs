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
    public class VoucherAdminController : Controller
    {
        // GET: Admin/VoucherAdmin

        QL_SHOPTHOITRANG_DOANEntities db = new QL_SHOPTHOITRANG_DOANEntities();



        //8	Xem_Mau
        [AdminAuthorize(idChucNang = 8)]

        public ActionResult Show(string searching)
        {
            return View(db.VOUCHER.Where(x => x.Ten_VOUCHER.Contains(searching) || searching == null).ToList());
        }


        //5	Thêm_Mau
        [AdminAuthorize(idChucNang = 5)]

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(VOUCHER voucher)
        {
            if (ModelState.IsValid)
            {
                db.VOUCHER.Add(voucher);
                db.SaveChanges();
                return RedirectToAction("Show");
            }
            return View(voucher);
        }


        //7	Sửa_Màu 
        [AdminAuthorize(idChucNang = 7)]

        public ActionResult Edit(string id)
        {
            VOUCHER voucher = db.VOUCHER.Single(d => d.MA_VOUCHER == id);
            if (voucher == null)
            {
                return HttpNotFound();
            }
            return View(voucher);
        }
        [HttpPost]
        public ActionResult Edit(VOUCHER voucher)
        {
            if (ModelState.IsValid)
            {
                db.VOUCHER.Attach(voucher);
                db.Entry(voucher).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Show", "Voucher");
            }
            return View(voucher);
        }


        //6	Xóa_Màu
        [AdminAuthorize(idChucNang = 6)]

        [HttpPost]
        public ActionResult DeleteSelected(IEnumerable<string> selectedItems)
        {
            if (selectedItems != null && selectedItems.Any())
            {
                foreach (var itemId in selectedItems)
                {
                    // Check if any customer is using this voucher
                    var isUsed = db.VOUCHER_KHACHHANG.Any(sp => sp.MA_VOUCHER == itemId);

                    if (isUsed)
                    {
                        TempData["ErrorMessage"] = "Không thể xóa voucher này vì có khách hàng đang sử dụng.";
                        return RedirectToAction("Show");
                    }
                }

                // Delete if no customers are using this voucher
                var vouchersToDelete = db.VOUCHER.Where(x => selectedItems.Contains(x.MA_VOUCHER)).ToList();
                db.VOUCHER.RemoveRange(vouchersToDelete);
                db.SaveChanges();
            }

            return RedirectToAction("Show");
        }
    }
}