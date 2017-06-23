using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
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

                    return Json(ImageHelper.ByteToBase64(fileData));

                    //if(fileData.LongLength > 32000)
                    //    return Json(WebDataProvider.Resize2Max50Kbytes(fileData));
                    //else
                    //    return Json(Convert.ToBase64String(fileData));

                }
                return Json("error");
            }
        }




        public ActionResult Index() {
            ImagesToAdd.Clear();
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

            ImagesToAdd.Clear();
            return RedirectToAction("index", "manage");
        }

        public ActionResult Update(string id, Thingy newThingy) {
            var objId = new ObjectId(id);
            if(ImagesToAdd.Count > 0)
                newThingy.Images = ImagesToAdd;
            else {
                var oldThing = _collection.Find(x => x.Id == objId).First();
                newThingy.Images = oldThing.Images;
            }
            newThingy.Id = objId;
            _collection.ReplaceOne(x => x.Id == newThingy.Id, newThingy);

            ImagesToAdd.Clear();

            return RedirectToAction("index", "manage");
        }

        public ActionResult Remove(string id) {

            var things = _collection.Find(x => x.Name != null && x.Name != "").ToList();
            ObjectId obj = new MongoDB.Bson.ObjectId(id);
            _collection.DeleteOne(x => x.Id == obj);


            return RedirectToAction("index", "manage");
        }

        public ActionResult Edit(string id) {
            //   var things = _collection.Find(x => x.Name != null && x.Name != "").ToList();
            ImagesToAdd.Clear();
            ObjectId obj = new MongoDB.Bson.ObjectId(id);
            var thing = _collection.Find(x => x.Id == obj).First();
            return View(thing);
        }


        public ManageController() {
            _collection = Database.GetCollection<Thingy>("Thingies");
        }

        public ActionResult Instagram() {
            string id = "sorokin_sad";

            List<Thingy> things = new List<Thingy>();

            using(WebClient wc = new WebClient()) {
                try {
                    string json = wc.DownloadString("https://www.instagram.com/" + id + "/media/");
                    JObject obj = (JObject)JsonConvert.DeserializeObject(json);
                    int i = 1;
                    foreach(var value in obj.First.Values()) {
                        var imageObject = value.Value<JObject>("images").Value<JObject>("standart_resolution");

                        if(imageObject == null)
                            imageObject = value.Value<JObject>("images").Value<JObject>("low_resolution");

                        string imageUrl = imageObject.Value<string>("url");

                        var caption = value.Value<JObject>("caption");

                        string description = caption == null ? "" : caption.GetValue("text").ToString();

                        var array = ImageHelper.GetImage(imageUrl);

                        Thingy thing = new Thingy() {
                            Images = new List<byte[]> { array, array, array },
                            Name = "Из инсты #" + i,
                            Description = description,
                            Price = new Random().Next(1, 15) * 100
                        };
                        things.Add(thing);
                        //images.Add(imageUrl);
                        i++;
                    }
                }
                catch(Exception e) {
                  return   RedirectToAction("index", "manage");
                }
            }
            _collection.DeleteManyAsync(i => true);
            _collection.InsertMany(things);
            // return View("insta",images);
            return RedirectToAction("index", "manage");
        }
    }
}