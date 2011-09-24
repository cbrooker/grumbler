using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

    public static class Url {

        public static string SiteRoot() {
            var context = HttpContext.Current;
            var Port = context.Request.ServerVariables["SERVER_PORT"];
            if (Port == null || Port == "80" || Port == "443")
                Port = "";
            else
                Port = ":" + Port;
            var Protocol = context.Request.ServerVariables["SERVER_PORT_SECURE"];
            if (Protocol == null || Protocol == "0")
                Protocol = "http://";
            else
                Protocol = "https://";

            var appPath = context.Request.ApplicationPath;
            if (appPath == "/")
                appPath = "";

            var sOut = Protocol + context.Request.ServerVariables["SERVER_NAME"] + Port + appPath;
            return sOut;

        }

        public static string GetReturnUrl() {
            var context = HttpContext.Current;
            var returnUrl = "";

            if (context.Request.QueryString["ReturnUrl"] != null) {
                returnUrl = context.Request.QueryString["ReturnUrl"];
            }

            return returnUrl;
        }

        public static string Crunch(this IList<string> urlData) {
            StringBuilder sb = new StringBuilder();
            var counter = 2;
            foreach (var item in urlData.Skip(1)) {
                sb.Append(item);
                if (counter < urlData.Count)
                    sb.Append("/");
                counter++;
            }
            return sb.ToString();
        }
    }
