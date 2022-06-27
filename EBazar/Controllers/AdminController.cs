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
    public class AdminController : Controller
    {
        dbemarketingEntities dbobj = new dbemarketingEntities();    
        // GET: Admin
        [HttpGet]
        public ActionResult login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult login(tbl_admin avm)
        {
            tbl_admin ad = dbobj.tbl_admin.Where(m => m.ad_username == avm.ad_username && m.ad_password == avm.ad_password).SingleOrDefault();
            if(ad!=null)
            {
                Session["ad_id"] = ad.ad_id.ToString();
                return RedirectToAction("create");
            }
            else
            {
                ViewBag.error = "Invalid username or password";
            }
            return View();
        }

       
        
        //[Authorize]
        public ActionResult create()
        {
            if(Session["ad_id"]==null)
            {
                Session.Contents.RemoveAll();
                return RedirectToAction("login");

            }
            return View();


        }
        [HttpPost]
        public ActionResult create(tbl_category cvm, HttpPostedFileBase imgfile)
        {
            string path = uploadimgfile(imgfile);
            if (path.Equals("-1"))
            {
                Response.Write("<script>alert('Please Login First'); </script>");
                ViewBag.error = "Image could not be uploaded....";
            }
            else
            {
                try
                {
                    tbl_category cat = new tbl_category();
                    cat.cat_name = cvm.cat_name;
                    cat.cat_image = path;
                    cat.cat_status = "1";
                    cat.cat_fk_ad = Convert.ToInt32(Session["ad_id"].ToString());
                    dbobj.tbl_category.Add(cat);
                    dbobj.SaveChanges();
                    return RedirectToAction("ViewCategory");
                }
                catch(Exception ex)
                {
                    //Response.Write("<script>alert('please fill the category name'); </script>");
                    ViewBag.Message = "please fill the category name";
                }
                
            }

            return View();
        }




            public ActionResult ViewCategory(int? page)
            {
                int pagesize = 9, pageindex = 1;
                pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
                var list = dbobj.tbl_category.Where(x => x.cat_status == "1").OrderByDescending(x => x.cat_id).ToList();
                IPagedList<tbl_category> stu = list.ToPagedList(pageindex, pagesize);


                return View(stu);
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