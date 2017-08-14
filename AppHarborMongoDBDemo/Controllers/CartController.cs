using MongoDB.Bson;
using MongoDB.Driver;
using RestSharp;
using RestSharp.Authenticators;
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

        public ActionResult Order() {
            string comment = Request.Form["comment"];
            string name = Request.Form["client_name"];
            var c = Request.Cookies.Get("cart");
            if(c == null || Request.Cookies["cart"].Value == null || Request.Cookies["cart"].Value == "") {
                CreateNewCookies("cart");
            }
            else
                Response.Cookies.Add(c);

            if(!string.IsNullOrEmpty(comment) && !string.IsNullOrEmpty(name)) {
                SendMail(comment, name, GetCartItems());
                ClearCart();
                return RedirectToAction("index", "home");
            }
            else
                return View(GetCartItems());
        }

        private void SendMail(string comment, string name, CartItems cartItems) {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator =
            new HttpBasicAuthenticator("api",
                                      "key-8e6fd07f55d3c0a633afc7b102c3ebd2");
            RestRequest request = new RestRequest();
            request.AddParameter("domain", "sandbox21f47bff07d246a08a1cdd9e7a7b485d.mailgun.org", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "New Ceramic order <postmaster@sandbox21f47bff07d246a08a1cdd9e7a7b485d.mailgun.org>");
            request.AddParameter("to", "tony <whereismyguts@gmail.com>");
            request.AddParameter("subject", "Hello tony");
            request.AddParameter("text",
                string.Format("Кто-то по имени {0} заказал:\n\n{1}\nКоментарий к заказу: \n{2}",
                name, cartItems, comment));
            request.Method = Method.POST;
            client.Execute(request);
        }

        private void ClearCart() {
            var userId = Request.Cookies["cart"].Value;
            IMongoCollection<UserCart> cartsCollection = Database.GetCollection<UserCart>("Carts");
            UserCart cart = cartsCollection.Find(x => x.UserId == userId).FirstOrDefault();
            cartsCollection.DeleteMany(x => x.Id == cart.Id);
        }

        [HttpPost]
        public ActionResult GetCartItemsView() {
            var items = GetCartItems();
            return PartialView("_CartItemsList", items);
        }

        CartItems GetCartItems() {
            CartItems results = new CartItems();
            try {
                var userId = Request.Cookies["cart"].Value;

                if(string.IsNullOrEmpty(userId))
                    return results;

                var cartsCollection = Database.GetCollection<UserCart>("Carts");
                var cart = cartsCollection.Find(x => x.UserId == userId).FirstOrDefault();

                if(cart == null)
                    return results;

                foreach(var itemId in cart.Items) {
                    ObjectId obj = new MongoDB.Bson.ObjectId(itemId);
                    //   var results = _collection.Find(x => x.Name != "").ToList();
                    Thingy thing = _collection.Find(x => x.Id == obj).FirstOrDefault();
                    if(thing != null)
                        results.Add(thing);
                }
            }
            catch { }
            results.Items.Sort((x, y) => string.Compare(x.Thing.Name, y.Thing.Name));
            return results;
        }

        [HttpPost]
        public ActionResult CountCartItem(string itemId, int delta) {
            var userId = Request.Cookies["cart"].Value;
            IMongoCollection<UserCart> cartsCollection = Database.GetCollection<UserCart>("Carts");
            UserCart cart = cartsCollection.Find(x => x.UserId == userId).FirstOrDefault();
            if(delta > 0)
                cart.Items.Add(itemId);
            else {
                if(cart.Items.Count<string>(i => i == itemId) > 1)
                    cart.Items.Remove(itemId);
            }
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
            if(c == null || Request.Cookies["cart"].Value == null || Request.Cookies["cart"].Value == "") {
                CreateNewCookies("cart");
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

        private void CreateNewCookies(string cookieName) {
            var cookie = new System.Web.HttpCookie(cookieName, GetNewClientId());
            cookie.Expires = System.DateTime.Now.AddMonths(1);
            Response.Cookies.Set(cookie);
        }

        string GetNewClientId() {
            var id = Guid.NewGuid().ToString();
            return id;
        }
    }
}