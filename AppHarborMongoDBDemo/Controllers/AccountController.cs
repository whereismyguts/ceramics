using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AppHarborMongoDBDemo {
    public class AccountController: BaseController {

        static string returnUrl;
        public ActionResult LogOn(Account model) {

            if(string.IsNullOrEmpty( returnUrl) && !string.IsNullOrEmpty(Request.QueryString["ReturnUrl"]))
                returnUrl = Request.QueryString["ReturnUrl"]; 

            if(model.IsEmpty)
                return View();


            if(model.Login == "x" && model.Password == "x") {
                FormsAuthentication.SetAuthCookie(model.Login, false);
                //string url = FormsAuthentication.GetRedirectUrl(model.Login, false);
                return Redirect(returnUrl);
            }
            else {
                Response.Write("Invalid UserID and Password");
            }

            return View();
        }
    }

    public class Account {
        public bool IsEmpty { get { return string.IsNullOrEmpty(Login) || string.IsNullOrEmpty(Password); } }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}