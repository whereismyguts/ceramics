using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppHarborMongoDBDemo {
    public static class WebDataProvider {
        public static string Base64 { get; internal set; }


        public static List<Thingy> Things(string id = "") {
            var db = new MongoClient(BaseController.MongoDbConnectionString).GetDatabase(BaseController.MongoDbDatabaseName);
            // db.GetCollection<Thingy>("Thingies").DeleteMany(x=>x.Name!="");
            if(string.IsNullOrEmpty(id))
                return db.GetCollection<Thingy>("Thingies").Find(x => x.Name != null && x.Name != "").ToList();

            var objId = new ObjectId(id);
            return db.GetCollection<Thingy>("Thingies").Find(x => x.Name != null && x.Name != "" && x.Id != objId).ToList();
        }

      


    }
}