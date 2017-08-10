using System.Web.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Web;
using System;
using System.Text;

namespace AppHarborMongoDBDemo {
    public class HomeController: BaseController {
        private readonly IMongoCollection<Thingy> _collection;

        public HomeController() {
            _collection = Database.GetCollection<Thingy>("Thingies");
        }

        public ActionResult Index() {
            //return View(GetMongoDbConnectionString()); // toremove
            var results = _collection.Find(x => x.Name != null && x.Name != "").ToList();
            return View(results);
        }

        public ActionResult Item(string id) {
            ObjectId obj = new MongoDB.Bson.ObjectId(id);
            //   var results = _collection.Find(x => x.Name != "").ToList();
            var result = _collection.Find(x => x.Id == obj).FirstOrDefault();
            if(result != null)
                return View(result);
            return RedirectToAction("Index");
        }

        public ActionResult Work() {
            return View();
        }
        public ActionResult Contact() {
            return View();
        }
        public ActionResult Order() {
            return View();
        }
    }
}
