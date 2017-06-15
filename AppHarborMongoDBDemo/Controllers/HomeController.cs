using System.Web.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;

namespace AppHarborMongoDBDemo {
    public class HomeController: BaseController {
        private readonly IMongoCollection<Thingy> _collection;

        public HomeController() {
            _collection = Database.GetCollection<Thingy>("Thingies");
        }

        public ActionResult Index() {
            //return View(GetMongoDbConnectionString()); // toremove
            var results = _collection.Find(x => x.Name != "").ToList();
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
    }
}
