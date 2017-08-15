using System.Configuration;
using System.Web.Mvc;
using MongoDB.Driver;

namespace AppHarborMongoDBDemo {
    public abstract class BaseController: Controller {
        public IMongoDatabase Database {
            get {
                return new MongoClient(MongoDbConnectionString).GetDatabase(MongoDbDatabaseName);
            }
        }

        public static string MongoDbDatabaseName {
            get { return ConfigurationManager.AppSettings.Get("CUSTOM_MONGOLAB_DATABASE") ?? "sorokin_sad"; } 
        }


        public static string MongoDbConnectionString {
            get {
                return
                    ConfigurationManager.AppSettings.Get("CUSTOM_MONGOLAB_URI") ??
                    "mongodb://localhost/Things";
            }
        }
    }
}
