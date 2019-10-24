using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using PIDI.Models;

namespace PIDI.App_Start
{
    public class SessionContext
    {
        public static SessionContext Instance
        {
            get => GetInstance();
            private set { }
        }

        private static SessionContext _instance;

        private static SessionContext GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SessionContext();
                return _instance;
            }
            else
                return _instance;
        }

        public void SetAuthenticationToken(string name, bool isPersistant, UserModel userData)
        {
            string data = null;
            if (userData != null)
                data = new JavaScriptSerializer().Serialize(userData);

            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, name, DateTime.Now, DateTime.Now.AddDays(7), isPersistant , data);

            string cookieData = FormsAuthentication.Encrypt(ticket);
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookieData)
            {
                HttpOnly = true,
                Expires = ticket.Expiration
            };

            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public UserModel GetUserData()
        {
            UserModel userData = null;

            try
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (cookie != null)
                {
                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);

                    userData = new JavaScriptSerializer().Deserialize(ticket.UserData, typeof(UserModel)) as UserModel;
                }
            }
            catch (Exception ex)
            {
                
            }

            return userData;
        }
    }
}

