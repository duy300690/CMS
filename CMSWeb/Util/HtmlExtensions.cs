using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace CMSWeb.Util
{
    public static class HtmlExtensions
    {
    }
    public static class XUtil
    {
        public static string JsonDie(string message = "Internal server error", int code = 500)
        {
            var js = new JavaScriptSerializer();
            var data = new { message = message, code = code };

            return js.Serialize(data);
        }

        public static string JsonDie(string url, string message = "Internal server error", int code = 500)
        {
            var js = new JavaScriptSerializer();
            var data = new { url, message = message, code = code };

            return js.Serialize(data);
        }

        public static string JsonDie(string url, string message = "Internal server error", int code = 500, int? id = null)
        {
            var js = new JavaScriptSerializer();
            var data = new { url, message = message, code = code, id = id ?? 0 };

            return js.Serialize(data);
        }

        public static string Object2Json(object obj)
        {
            var js = new JavaScriptSerializer();
            return js.Serialize(obj);
        }

        public static string DataTableToJSON(DataTable table)
        {
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(table);
            return JSONString;
        }
    }
}