using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using WMTest.Models;
using System.Data.SqlClient;

namespace WMTest.Controllers
{
    public class HomeController : Controller
    {


        // GET: Home
        public ActionResult Index(List<Products> data)
        {

            try
            {
                var prod = new Products();
  
                ViewBag.ViewModelList = prod.LoadFromDB(null);
                return View();
            }
            catch (Exception e)
            {
                throw e;
            }            
        }
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            var prod = new Products();
            BinaryReader b = new BinaryReader(file.InputStream);
            byte[] binData = b.ReadBytes(file.ContentLength);
            string result = System.Text.Encoding.UTF8.GetString(binData);
            ViewBag.MyProdList = prod.Product_JSON(result);
            TempData["ProdJSONData"] = ViewBag.MyProdList;
            ViewBag.ViewModelList = prod.LoadFromDB(null);
            return View();
        }


        // GET: Home/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Home/Create
        [HttpPost]

        public ActionResult Create(FormCollection collection)
        {
            var prod = new Products();
            prod.CreateProd(collection);
            return RedirectToAction("Index");
        }

       
        public ActionResult SaveJSONToDB()
        {
            try
            {
                var li = (IEnumerable<Products>)TempData["ProdJSONData"];
                var prod = new Products();
                prod.SaveJSONToDB(li.ToList());
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Home/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id > 0)
                {
                    var prod = new Products();
                    ViewBag.EditProd = prod.LoadFromDB(id);
                    return View();
                }
                else
                {
                    return RedirectToAction("Index");
                }

            }
            catch (Exception e)
            {

                throw e;
            }
            
        }

        // POST: Home/Edit/5
        [HttpPost]
        public ActionResult Edit(FormCollection collection)
        {
            try
            {
               
                var prod = new Products();
                prod.UpdateProd(collection);
                return RedirectToAction("Index");

 

                // TODO: Add update logic here
            }
            catch
            {
                return View();
            }
        }


        // POST: Home/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                // TODO: Add delete logic here
                var prod = new Products();
                prod.DeleteProd(id);
                return RedirectToAction("Index");
            }
            catch(Exception e)
            {
                throw e;
                //return RedirectToAction("Index");
            }
        }
    }
}
