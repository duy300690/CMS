using CMSWeb.Language;
using CMSWeb.Models;
using Hangfire;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Resources;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;

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
        public static List<T> ConvertJsonToObject<T>(string json)
        {
            var serializer = new JavaScriptSerializer
            {
                MaxJsonLength = Int32.MaxValue
            };
            return serializer.Deserialize<List<T>>(json);
        }

        public static string DisplayProvince(string code)
        {
            string strLocationJson = GetFileJsonLocation();
            var dataLocation = ConvertJsonToObject<CityModel>(strLocationJson);
            return dataLocation.FirstOrDefault(x => x.Code.Equals(code)).Name;

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

        public static int GetAge(DateTime bornDate)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - bornDate.Year;
            if (bornDate > today.AddYears(-age))
                age--;

            return age;
        }

        public static double ConvertToTimestamp(DateTime value)
        {
            //create Timespan by subtracting the value provided from
            //the Unix Epoch
            TimeSpan span = (value - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());

            //return the total seconds (which is a UNIX timestamp)
            return (double)span.TotalSeconds;
        }

        public static DateTime? ConvertStringToDate(string strDate)
        {
            DateTime.TryParse(strDate, out DateTime date);
            if (date.Year > 1900)
                return date;
            return null;
        }

        public static void DeleteTemporaryAttachmentsInBackgroundThread(string directoryPath, string filename)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
                //HangFile
                BackgroundJob.Enqueue(() => DeleteAttachment(directoryPath, filename));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private static void DeleteAttachment(string directoryPath, string filename)
        {

            try
            {
                //Delete main file
                string filepath = Path.Combine(directoryPath, filename);
                if (System.IO.File.Exists(filepath))
                    System.IO.File.Delete(filepath);

                //Delete parts file
                var dir = new DirectoryInfo(directoryPath);

                foreach (var file in dir.EnumerateFiles($"{filename}.part*"))
                {
                    file.Delete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string FormatAttachment(string file)
        {
            if (string.IsNullOrEmpty(file)) return string.Empty;

            if (file.Contains("_"))
            {
                return file.Substring(file.IndexOf("_") + 1, file.Length - file.IndexOf("_") - 1);
            }

            return file;
        }

        public static byte[] ReadFile(string filePath)
        {
            byte[] buffer;
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            try
            {
                int length = (int)fileStream.Length;  // get file length
                buffer = new byte[length];            // create buffer
                int count;                            // actual number of bytes read
                int sum = 0;                          // total number of bytes read

                // read until Read method returns 0 (end of the stream has been reached)
                while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                    sum += count;  // sum is a buffer offset for next reading
            }
            finally
            {
                fileStream.Close();
            }
            return buffer;
        }
    }
}