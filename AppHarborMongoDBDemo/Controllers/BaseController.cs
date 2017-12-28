using System;
using System.Configuration;
using System.Web.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;

namespace AppHarborMongoDBDemo {
    public static class DbLayer {
        public static IMongoDatabase Database {
            get {
                return new MongoClient(MongoDbConnectionString).GetDatabase(MongoDbDatabaseName);
            }
        }

        public static string MongoDbDatabaseName {
            get {

                return "sorokin_sad";
                // return ConfigurationManager.AppSettings.Get("CUSTOM_MONGOLAB_DATABASE") ?? "sorokin_sad"; 
            }
        }


        public static string MongoDbConnectionString {
            get {
                return
                    ConfigurationManager.AppSettings.Get("CUSTOM_MONGOLAB_URI") ??
                    "mongodb://localhost/Things";
            }
        }

        internal static Thingy GetEntity (string table, ObjectId id) {
            var col = Database.GetCollection<Thingy>(table);
            return col.Find(x => x.Id == id).FirstOrDefault();
        }
    }

    public abstract class BaseController: Controller {
        public IMongoDatabase Database {
            get {
                return DbLayer.Database;
            }
        }

        public static string MongoDbDatabaseName {
            get { return DbLayer.MongoDbDatabaseName; }
        }


        public static string MongoDbConnectionString {
            get {
                return DbLayer.MongoDbConnectionString;
            }
        }
    }
}
