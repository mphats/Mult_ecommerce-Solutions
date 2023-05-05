using Mult_ecommerce.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mult_ecommerce.Content
{
    public class UserController : Controller
    {
        MarketingDB db = new MarketingDB(); 
        // GET: User
        public ActionResult Index(int? page)
        {
            int pagesize = 8, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.Products.ToList().OrderBy(x => x.Pro_name);
            IPagedList<Product> stu = list.ToPagedList(pageindex, pagesize);
            return View(stu);
        }
       
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Tb_user mvc)
        {
            Tb_user ad = db.Tb_user.Where(x => x.U_email == mvc.U_email && x.U_password == mvc.U_password).SingleOrDefault();
            if (ad != null)
            {
                Session["ID"] = ad.ID.ToString();
                return RedirectToAction("Index");
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
        public ActionResult Register(Tb_user cvm, HttpPostedFileBase imgfile)
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

            Tb_user user = new Tb_user
            {
                U_name = cvm.U_name,
                U_email = cvm.U_email,
                U_password = cvm.U_password,
                U_image = path,
                U_contact = cvm.U_contact
            };

            db.Tb_user.Add(user);
            db.SaveChanges();
            return RedirectToAction("Login");
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
                        string uploadPath = Path.Combine(Server.MapPath("~/Upload"), fileName);
                        Directory.CreateDirectory(Server.MapPath("~/Upload"));
                        file.SaveAs(uploadPath);
                        path = $"/Upload/{fileName}";
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
        public ActionResult SignOut()
        {
            Session.RemoveAll();
            Session.Abandon();
            return RedirectToAction("Login");
        }
        public ActionResult AvailableCategorys(int? page)
        {
            int pagesize = 8, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.Categories.Where(x => x.Ca_status == 1).OrderByDescending(x => x.ID).ToList();
            IPagedList<Category> stu = list.ToPagedList(pageindex, pagesize);
            return View(stu);
        }
        public ActionResult Ads(int? id, int? page)
        {
            int pagesize = 8, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.Products.Where(x => x.Pro_fk_cat == 1).OrderByDescending(x => x.ID).ToList();
            IPagedList<Product> stu = list.ToPagedList(pageindex, pagesize);
            return View(stu);
        }

        public ActionResult ViewAds(int? id)
        {
            ViewAdModel _Order = new ViewAdModel();
            Product product = db.Products.Where(x => x.ID == id).SingleOrDefault();
            _Order.ID = product.ID;
            _Order.Pro_name = product.Pro_name;
            _Order.Pro_price = product.Pro_price;
            _Order.Pro_image = product.Pro_image;
            _Order.Pro_des = product.Pro_des;

            Category category = db.Categories.Where(x => x.ID == product.Pro_fk_cat).SingleOrDefault();
            _Order.Cat_name = category.Cat_name;

            Tb_user user = db.Tb_user.Where(x => x.ID == product.Pro_fk_user).SingleOrDefault();
            _Order.U_name = user.U_name;
            _Order.U_image = user.U_image;
            _Order.U_email = user.U_email;
            _Order.U_contact = user.U_contact;
            _Order.Pro_fk_user = user.ID;
            return View(_Order);
        }
        [HttpPost]
        public ActionResult Ads(int? id, int? page, string search)
        {
            int pagesize = 8, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.Products.Where(x => x.Pro_name.Contains(search)).OrderByDescending(x => x.ID).ToList();
            IPagedList<Product> stu = list.ToPagedList(pageindex, pagesize);
            return View(stu);
        }
        public ActionResult DeletedAd(int? id)
        {
            Product V = db.Products.Where(x => x.ID == id).SingleOrDefault();
            db.Products.Remove(V);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult CreateAds(int? id)
        {
            List<Category> li = db.Categories.ToList();
            ViewBag.CategoryList = new SelectList(li, "ID", "Cat_name");
            return View();
        }

        [HttpPost]
        public ActionResult CreateAds(Product cvm, HttpPostedFileBase imgfile)
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

            Product _Order = new Product
            {
                Pro_name = cvm.Pro_name,
                Pro_fk_cat= cvm.Pro_fk_cat,
                Pro_fk_user= Convert.ToInt32(Session["ID"].ToString()),
                Pro_price = cvm.Pro_price,
                Pro_des = cvm.Pro_des,
                Pro_image = path,
            };

            db.Products.Add(_Order);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}