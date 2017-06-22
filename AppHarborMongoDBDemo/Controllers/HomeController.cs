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
using System.IO;
using System.Web.UI;

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

        public ActionResult Work(string id="") {
            return View();
            List<string> images = new List<string>();

            using(WebClient wc = new WebClient()) {
                try {
                    string json = wc.DownloadString("https://www.instagram.com/" + id + "/media/");
                    JObject obj = (JObject)JsonConvert.DeserializeObject(json);

                    foreach(var value in obj.First.Values()) {
                        var imageObject = value.Value<JObject>("images").Value<JObject>("standart_resolution");

                        if(imageObject == null)
                            imageObject = value.Value<JObject>("images").Value<JObject>("low_resolution");

                        string imageUrl = imageObject.Value<string>("url");

                        images.Add(imageUrl);
                    }
                }
                catch {
                    return View("insta",new string[] { });
                }
            }
            return View("insta",images);
        }
    }
}
