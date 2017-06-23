using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppHarborMongoDBDemo.Controllers {
    public class CartController: BaseController {
        private readonly IMongoCollection<Thingy> _collection;

        public CartController() {
            _collection = Database.GetCollection<Thingy>("Thingies");
        }

        [HttpPost]
        public ActionResult GetCartItems() {
            CartItems results = new CartItems();
            try {
                var userId = Request.Cookies["cart"].Value;

                if(string.IsNullOrEmpty(userId))
                    return PartialView("_CartItemsList", results);

                var cartsCollection = Database.GetCollection<UserCart>("Carts");
                var cart = cartsCollection.Find(x => x.UserId == userId).FirstOrDefault();

                if(cart == null)
                    return PartialView("_CartItemsList", results);

                foreach(var itemId in cart.Items) {
                    ObjectId obj = new MongoDB.Bson.ObjectId(itemId);
                    //   var results = _collection.Find(x => x.Name != "").ToList();
                    Thingy thing = _collection.Find(x => x.Id == obj).FirstOrDefault();
                    if(thing!=null)
                    results.Add(thing);
                }
            }
            catch { }
            return PartialView("_CartItemsList", results);
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
            var c = Request.Cookies.Get("cart");
            if(c==null ||  Request.Cookies["cart"].Value == null || Request.Cookies["cart"].Value == "") {
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

        string GetNewClientId() {
            var id = Guid.NewGuid().ToString();
            return id;
        }
    }
}