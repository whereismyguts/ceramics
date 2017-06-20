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
            get { return ConfigurationManager.AppSettings.Get("CUSTOM_MONGOLAB_DATABASE") ?? "sorokin_sad"; } //appharbor_pw3dvl7m
        }


        public static string MongoDbConnectionString {
            get {
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
}
