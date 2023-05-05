using Mult_ecommerce.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mult_ecommerce.Controllers
{
    public class AdminsController : Controller
    {
        MarketingDB db = new MarketingDB();
        // GET: Admin//login
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        //Register
        [HttpPost]
        public ActionResult Index(Admin avm)
        {
            Admin admin = db.Admins.Where(model => model.Username == avm.Username && model.Password == avm.Password).SingleOrDefault();
            if (admin != null)
            {
                Session["ID"] = admin.ID.ToString();
                return RedirectToAction("ViewCategory");
            }
            else
            {
                ViewBag.error = "Invalid username or password";
            }
            return View();
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(Admin avm)
        {
            Admin admin = db.Admins.Where(model => model.Username == avm.Username).SingleOrDefault();
            if (admin != null)
            {
                ViewBag.error = "You cannot register with this username";
            }
            else
            {
                Admin ad = new Admin();
                ad.Username = avm.Username;
                ad.Password = avm.Password;
                db.Admins.Add(ad);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpGet]
        public ActionResult ViewCategory(int? page)
        {
            int pagesize = 8, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.Categories.Where(model => model.Ca_status == 1).OrderByDescending(x => x.ID).ToList();
            IPagedList<Category> stu = list.ToPagedList(pageindex, pagesize);
            return View(stu);
        }
        
        public ActionResult CreateCategory()
        {
            if (Session["ID"] == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public ActionResult CreateCategory(Category cvm, HttpPostedFileBase imgfile)
        {
            if (imgfile == null || imgfile.ContentLength == 0)
            {
                ModelState.AddModelError("imgfile", "Image is required");
            }
            if (!ModelState.IsValid)
            {
                return View(cvm);
            }
            string path = Uploading(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Image could not be uploaded...";
                return View(cvm);
            }

            Category Categorys = new Category
            {

                Cat_name = cvm.Cat_name,
                Cat_image = path,
                Ca_status = 1,
                Cat_fk_ad = Convert.ToInt32(Session["ID"].ToString())
            };
            db.Categories.Add(Categorys);
            db.SaveChanges();
            return RedirectToAction("ViewCategory");
        }

        public string Uploading(HttpPostedFileBase file)
        {
            string path = "-1";
            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".png") || extension.ToLower().Equals(".jpeg"))
                {
                    try
                    {
                        string fileName = $"{Guid.NewGuid()}{extension}";
                        string uploadPath = Path.Combine(Server.MapPath("~/Uploads"), fileName);
                        Directory.CreateDirectory(Server.MapPath("~/Upload"));
                        file.SaveAs(uploadPath);
                        path = $"/Uploads/{fileName}";
                    }
                    catch (Exception ex)
                    {
                        path = "-1";
                    }
                }
                else
                {
                    Response.Write("<script>alert('Only jpg, and png formats are accepted...');</script>");
                }
            }
            return path;
        }

    }
}