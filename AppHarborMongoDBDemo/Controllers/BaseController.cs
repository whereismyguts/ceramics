using System.Configuration;
using System.Web.Mvc;
using MongoDB.Driver;

namespace AppHarborMongoDBDemo.Controllers
{
	public abstract class BaseController : Controller
	{
		public MongoDatabase Database
		{
			get
			{
				return MongoDatabase.Create(GetMongoDbConnectionString());
			}
		}

		public  string GetMongoDbConnectionString()
		{
            return "mongodb://appharbor_pw3dvl7m:eul4dg6sml88gluo42ur4fg2l6@ds159591.mlab.com:59591/appharbor_pw3dvl7m";

            return ConfigurationManager.AppSettings.Get("MONGOHQ_URL") ??
				ConfigurationManager.AppSettings.Get("MONGOLAB_URI") ??
				"mongodb://localhost/Things";
		}
	}
}
