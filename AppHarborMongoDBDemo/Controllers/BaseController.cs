using System.Configuration;
using System.Web.Mvc;
using MongoDB.Driver;

namespace AppHarborMongoDBDemo.Controllers
{
	public abstract class BaseController : Controller
	{
		public IMongoDatabase Database
		{
			get
			{
                return new MongoClient(GetMongoDbConnectionString()).GetDatabase("appharbor_pw3dvl7m");
			}
		}

		public  string GetMongoDbConnectionString()
		{
            //     return "mongodb://appharbor_pw3dvl7m:eul4dg6sml88gluo42ur4fg2l6@ds159591.mlab.com:59591/appharbor_pw3dvl7m";
            //     return "mongodb://whereismyguts:VisualStudio15@ds115352.mlab.com:15352/sorokin_sad"
            //mongodb://<dbuser>:<dbpassword>@ds115352.mlab.com:15352/sorokin_sad
            return
                //ConfigurationManager.AppSettings.Get("MONGOHQ_URL") ??
                ConfigurationManager.AppSettings.Get("CUSTOM_MONGOLAB_URI") ??
				"mongodb://localhost/Things";
		}
	}
}
