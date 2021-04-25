using CMSWeb.Language;
using CMSWeb.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;

namespace CMSWeb.Util
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class EnumOrderAttribute : Attribute
    {
        public int Order { get; set; }
    }

    public static class Helpers
    {
        public static string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text  
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            //get hash result after compute it  
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits  
                //for each byte  
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }

        //dd-MMM-yyyy hh:mm
        public static string FormatDisplayDateTime(DateTime source)
        {
            string dateTimeFormat = string.Empty;
            if (source != null)
            {
                dateTimeFormat = source.TimeOfDay.TotalSeconds == 0 ?
                                      string.Format(Thread.CurrentThread.CurrentCulture, "{0:dd-MMM-yyyy}", source)
                                    : string.Format(Thread.CurrentThread.CurrentCulture, "{0:dd-MMM-yyyy HH:mm}", source);
            }

            return dateTimeFormat;
        }

        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static IEnumerable<string> GetWithOrder(this Type type)
        {
            if (!type.IsEnum)
            {
                throw new ArgumentException("Type must be an enum");
            }
            // caching for result could be useful
            return type.GetFields()
                                   .Where(field => field.IsStatic)
                                   .Select(field => new
                                   {
                                       field,
                                       attribute = field.GetCustomAttribute<EnumOrderAttribute>()
                                   })
                                    .Select(fieldInfo => new
                                    {
                                        name = fieldInfo.field.Name,
                                        order = fieldInfo.attribute != null ? fieldInfo.attribute.Order : 0
                                    })
                                   .OrderBy(field => field.order)
                                   .Select(field => field.name);
        }

        public static IEnumerable<string> GetWithOrder(this Enum enumVal)
        {
            return enumVal.GetType().GetWithOrder();
        }
        public static string GetDisplayName(this Enum e)
        {
            var rm = new ResourceManager(typeof(Resource));
            var resourceDisplayName = rm.GetString(e.GetType().Name + "__" + e);

            return string.IsNullOrWhiteSpace(resourceDisplayName) ? $"[[{e}]]" : resourceDisplayName;
        }

        public static List<EnumModel> GetListEnumData<T>()
        {
            List<EnumModel> listEnumModel = new List<EnumModel>();

            foreach (var item in GetValues<T>())
            {
                if (typeof(T).IsEnum)
                {
                    Enum tmp = Enum.Parse(typeof(T), item.ToString()) as Enum;
                    tmp.GetWithOrder();

                    EnumModel enumItem = new EnumModel
                    {
                        Value = item.ToString(),
                        Text = tmp.GetDisplayName()
                    };
                    listEnumModel.Add(enumItem);
                }

            }

            return listEnumModel;
        }

        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static string GetFileJsonLocation()
        {
            string json = string.Empty;
            string file = HttpContext.Current.Request.PhysicalApplicationPath + "Models\\local.json";

            using (StreamReader r = new StreamReader(file))
            {
                json = r.ReadToEnd();
            }
            return json;
        }
    }
}