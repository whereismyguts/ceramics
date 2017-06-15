﻿using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppHarborMongoDBDemo {
    public class ManageController: BaseController {
        static List<byte[]> ImagesToAdd = new List<byte[]>();
        static List<string> ImagesToRemove = new List<string>();
        private readonly IMongoCollection<Thingy> _collection;

        [HttpPost]
        public ActionResult RemoveImage(int id) {
            ImagesToRemove.Add(id.ToString());
            return Json("removed");
        }

        [HttpPost]
        public ActionResult RememberImages(List<string> values) {
            ImagesToAdd = new List<byte[]>();
            if(values == null)
                return Json(false);
            foreach(string s in values) {
                byte[] newBytes = Convert.FromBase64String(s.Split(',')[1]);
                ImagesToAdd.Add(newBytes);
            }

            return Json(true);
        }

        [HttpPost]
        public ActionResult RememberImage() {
            using(var binaryReader = new BinaryReader(Request.Files[0].InputStream)) {
                byte[] fileData = binaryReader.ReadBytes(Request.Files[0].ContentLength);
                if(fileData != null) {
                    ImagesToAdd.Add(fileData);

                    return Json(WebDataProvider.GetBase64(fileData));

                    //if(fileData.LongLength > 32000)
                    //    return Json(WebDataProvider.Resize2Max50Kbytes(fileData));
                    //else
                    //    return Json(Convert.ToBase64String(fileData));

                }
                return Json("error");
            }
        }


       

        public ActionResult Index() {

            //var collection = Database.GetCollection<Thingy>("Thingies");
            //var results = collection.Find(x => x.Name != "").ToList();
            return View("new");
        }

        //public ActionResult New() {
        //    return View();
        //}



        public ActionResult Create(Thingy thingy) {

            thingy.Images = ImagesToAdd;

            _collection.InsertOne(thingy);


            return RedirectToAction("index", "manage");
        }

        public ActionResult Remove(string id) {

            var things = _collection.Find(x => x.Name != null && x.Name != "").ToList();
            ObjectId obj = new MongoDB.Bson.ObjectId(id);
            _collection.DeleteOne(x => x.Id == obj);


            return RedirectToAction("index", "manage");
        }

        public ManageController() {
            _collection = Database.GetCollection<Thingy>("Thingies");
        }
    }
}