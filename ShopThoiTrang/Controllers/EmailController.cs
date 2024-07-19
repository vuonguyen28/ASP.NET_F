using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ShopThoiTrang.Controllers
{
    public class EmailController : Controller
    {
        // GET: Email
        // Action để hiển thị View cho việc gửi email
        public ActionResult SendEmail()
        {
            return View();
        }

        // Action để xử lý việc gửi email từ form
        [HttpPost]
        public ActionResult SendEmail(string receiverEmail, string subject, string body)
        {
            string senderEmail = "cr.havaka@gmail.com"; // Địa chỉ email của bạn
            string password = "xsmtpsib-b24567409f390ae240d386c6dcd17bdbb1dc431fac237548f188e347d24fae50-1VPUcnWBALXbjC04"; // Mật khẩu của bạn

            MailMessage mail = new MailMessage(senderEmail, receiverEmail, subject, body);
            mail.IsBodyHtml = true;

            SmtpClient smtpClient = new SmtpClient("smtp-relay.brevo.com", 587);
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(senderEmail, password);

            try
            {
                smtpClient.Send(mail);
                ViewBag.Message = "Email sent successfully!";
            }
            catch (SmtpException ex)
            {
                ViewBag.Error = $"Error while sending email: {ex.Message}";
            }

            return View();
        }
    }
}