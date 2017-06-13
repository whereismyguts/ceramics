using System.Web.Mvc;
using AppHarborMongoDBDemo.Models;
using MongoDB.Driver;

namespace AppHarborMongoDBDemo.Controllers
{
	public class HomeController : BaseController
	{
		private readonly MongoCollection<Thingy> _collection;

		public HomeController()
		{

           

			_collection = Database.GetCollection<Thingy>("Thingies");



		}

		public ActionResult Index()
		{

         //   return View(GetMongoDbConnectionString()); // toremove
            return View(_collection.FindAll());
		}

		public ActionResult New()
		{
			return View();
		}

		public ActionResult Create(Thingy thingy)
		{
			_collection.Insert(thingy);
			return RedirectToAction("Index");
		}
	}
}
