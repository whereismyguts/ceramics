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

        public static string GetBase64(byte[] image) {
            if(image.LongLength > 32000)
                return Resize2Max50Kbytes(image);
            return Convert.ToBase64String(image);
        }

        public static string Resize2Max50Kbytes(byte[] data) {
            using(var ms = new MemoryStream(data)) {
                var image = Image.FromStream(ms);

                var ratioX = (double)150 / image.Width;
                var ratioY = (double)50 / image.Height;
                var ratio = Math.Min(ratioX, ratioY);

                var width = (int)(image.Width * ratio);
                var height = (int)(image.Height * ratio);

                var newImage = new Bitmap(width, height);
                Graphics.FromImage(newImage).DrawImage(image, 0, 0, width, height);
                Bitmap bmp = new Bitmap(newImage);

                ImageConverter converter = new ImageConverter();

                data = (byte[])converter.ConvertTo(bmp, typeof(byte[]));

                return Convert.ToBase64String(data);
            }
        }
    }
}