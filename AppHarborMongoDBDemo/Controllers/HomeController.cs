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


        public ActionResult Work(string id) {

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
                    return View(new string[] { });
                }
            }
            return View(images);
        }



        [HttpPost]
        public ActionResult GetCartItems() {
            var userId = Request.Cookies["cart"].Value;
            if(string.IsNullOrEmpty(userId))
                return Json("");

            var cartsCollection = Database.GetCollection<UserCart>("Carts");
            var cart = cartsCollection.Find(x => x.UserId == userId).FirstOrDefault();
            if(cart == null) {
                return Json("");
            }


            CartItems results = new CartItems();
            foreach(var itemId in cart.Items) {
                ObjectId obj = new MongoDB.Bson.ObjectId(itemId);
                //   var results = _collection.Find(x => x.Name != "").ToList();
                Thingy thing = _collection.Find(x => x.Id == obj).FirstOrDefault();

                results.Add(thing);
            }
            if(results.Items.Count > 0)
                return PartialView("_CartItemsList", results);
            return Json("");
        }

        [HttpPost]
        public ActionResult CountCartItem(string itemId, int delta) {
            var userId = Request.Cookies["cart"].Value;
            IMongoCollection<UserCart> cartsCollection = Database.GetCollection<UserCart>("Carts");
            UserCart cart = cartsCollection.Find(x => x.UserId == userId).FirstOrDefault();
            if(delta > 0)
                cart.Items.Add(itemId);
            else
                cart.Items.Remove(itemId);
            cartsCollection.ReplaceOne(x => x.Id == cart.Id, cart);
            return Json(itemId + " " + delta);
        }

        [HttpPost]
        public ActionResult RemoveCartItem(string itemId) {
            var userId = Request.Cookies["cart"].Value;
            IMongoCollection<UserCart> cartsCollection = Database.GetCollection<UserCart>("Carts");
            UserCart cart = cartsCollection.Find(x => x.UserId == userId).FirstOrDefault();
            cart.Items.RemoveAll(i => i == itemId);
            cartsCollection.ReplaceOne(x => x.Id == cart.Id, cart);
            return Json(itemId + " removed from cart");
        }
        [HttpPost]
        public ActionResult AddToCart(string itemId) {
            var objId = new ObjectId(itemId);
            if(string.IsNullOrEmpty(Request.Cookies["cart"].Value)) {
                var cookie = new HttpCookie("cart", GetNewClientId());
                cookie.Expires = System.DateTime.Now.AddMonths(1);
                Response.Cookies.Set(cookie);

            }
            string userId = Request.Cookies["cart"].Value;
            IMongoCollection<UserCart> cartsCollection = Database.GetCollection<UserCart>("Carts");
            UserCart cart = cartsCollection.Find(x => x.UserId == userId).FirstOrDefault();
            if(cart == null) {
                cart = new UserCart(userId, itemId);
                cartsCollection.InsertOne(cart);
            }
            else {
                cart.Items.Add(itemId);
                cartsCollection.ReplaceOne(x => x.Id == cart.Id, cart);
            }

            return Json(cart.Items);
        }
        Random r = new Random();
        private string GetNewClientId() {
            var id = Guid.NewGuid().ToString();
            return id;
        }
    }
}
