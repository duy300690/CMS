using CMSWeb.Models;
using System;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace CMSWeb.Util
{
    public static class SessionContext
    {
        public static void SetAuthenticationToken(string name, bool isPersistant, UserModel userData)
        {
            string data = string.Empty;
            if (userData != null)
                data = new JavaScriptSerializer().Serialize(userData);

            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                                                                            name,
                                                                            DateTime.Now,
                                                                            DateTime.Now.AddDays(1),
                                                                            isPersistant,
                                                                            !string.IsNullOrEmpty(data) ? data : userData.Username.ToString()
                                                                            );

            string cookieData = FormsAuthentication.Encrypt(ticket);
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookieData)
            {
                HttpOnly = true,
                Expires = ticket.Expiration
            };

            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public static UserModel GetUserLogin()
        {
            try
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (cookie != null)
                {
                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    return serializer.Deserialize<UserModel>(ticket.UserData);
                }
            }
            catch (Exception ex)
            {
            }

            return null;
        }

        public static Tuple<bool, CMSService.Secure.Roles> IsAuthentication()
        {
            var role = CMSService.Secure.Roles.USER;
            if (GetUserLogin() != null)
                role = (CMSService.Secure.Roles)Enum.Parse(typeof(CMSService.Secure.Roles), GetUserLogin().Role, true);

            return Tuple.Create(GetUserLogin() != null, role);
        }

        public static void RemoveAuthenticationToken()
        {
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, "")
            {
                Expires = DateTime.Now.AddYears(-1)
            };
            HttpContext.Current.Response.Cookies.Add(cookie);

            FormsAuthentication.SignOut();
        }
    }
}