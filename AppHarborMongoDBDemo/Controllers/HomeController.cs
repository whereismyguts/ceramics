using System.Web.Mvc;
using AppHarborMongoDBDemo.Models;
using MongoDB.Driver;

namespace AppHarborMongoDBDemo.Controllers
{
	public class HomeController : BaseController
	{
		private readonly IMongoCollection<Thingy> _collection;

		public HomeController()
		{
			_collection = Database.GetCollection<Thingy>("Thingies");
		}

		public ActionResult Index()
		{

            //return View(GetMongoDbConnectionString()); // toremove

            var results = _collection.Find(x => x.Name!="").ToList();
            return View(results);
		}

		public ActionResult New()
		{
			return View();
		}

		public ActionResult Create(Thingy thingy)
		{
			_collection.InsertOne(thingy);
			return RedirectToAction("Index");
		}
	}
}
