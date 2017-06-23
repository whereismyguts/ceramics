using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace AppHarborMongoDBDemo {
    public  static class ImageHelper {

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

        public static string ByteToBase64(byte[] image) {
            if(image.LongLength > 32000)
                return Resize2Max50Kbytes(image);
            return Convert.ToBase64String(image);
        }

        public static string URLToBase64(string url) {
            StringBuilder _sb = new StringBuilder();
            byte[] array = GetImage(url);
            _sb.Append(ByteToBase64(array));
            return _sb.ToString();
        }

        public static byte[] GetImage(string url) {
            Stream stream = null;
            byte[] buf;

            try {
                WebProxy myProxy = new WebProxy();
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                stream = response.GetResponseStream();

                using(BinaryReader br = new BinaryReader(stream)) {
                    int len = (int)(response.ContentLength);
                    buf = br.ReadBytes(len);
                    br.Close();
                }

                stream.Close();
                response.Close();
            }
            catch(Exception exp) {
                buf = null;
            }

            return (buf);
        }
    }
}