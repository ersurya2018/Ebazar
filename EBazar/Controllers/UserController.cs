using EBazar.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EBazar.Controllers
{
    public class UserController : Controller
    {
        dbemarketingEntities dbobj = new dbemarketingEntities();
        // GET: User
        public ActionResult Index(int? page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = dbobj.tbl_category.Where(x => x.cat_status == "1").OrderByDescending(x => x.cat_id).ToList();
            IPagedList<tbl_category> stu = list.ToPagedList(pageindex, pagesize);
            return View(stu);
        }
        [HttpGet]
        public ActionResult SignUp()
        {   
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(tbl_user cvm, HttpPostedFileBase imgfile)
        {
            string path = uploadimgfile(imgfile);
            if (path.Equals("-1"))
            {
                Response.Write("<script>alert('Please Login First'); </script>");
                ViewBag.error = "Image could not be uploaded....";
            }
            else
            {
                tbl_user userobj = new tbl_user();
                userobj.u_name = cvm.u_name;
                userobj.u_email = cvm.u_email;
                userobj.u_password = cvm.u_password;
                userobj.u_image = path;
                userobj.u_contact = cvm.u_contact;
                dbobj.tbl_user.Add(userobj);
                dbobj.SaveChanges();
            }
            return RedirectToAction("login");
        }

        //End code----------------------------@

        [HttpGet]
        public ActionResult login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult login(tbl_user uvm)
        {
            tbl_user user = dbobj.tbl_user.Where(x => x.u_email == uvm.u_email && x.u_password == uvm.u_password).FirstOrDefault();
            if(user!=null)
            {
                Session["user_id"] = user.u_id.ToString();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.error = "EmailId or Passwrod not match";
            }
            return View();
        }

        public ActionResult CreateAd()
        {
            List<tbl_category> li = dbobj.tbl_category.ToList();
            ViewBag.categorylist = new SelectList(li, "cat_id", "cat_name");
            return View();
        }

        public string uploadimgfile(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();
            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                {
                    try
                    {

                        path = Path.Combine(Server.MapPath("~/Content/upload"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/Content/upload/" + random + Path.GetFileName(file.FileName);

                        //ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        path = "-1";
                    }
                }
                else
                {
                    Response.Write("<script>alert('Only jpg ,jpeg or png formats are acceptable....'); </script>");
                }
            }

            else
            {
                Response.Write("<script>alert('Please select a file'); </script>");
                path = "-1";
            }

            return path;
        }
    }
}