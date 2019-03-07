using System;
using System.Web;
using System.Web.Security;
using System.Security.Principal;

namespace LT.Component.Security
{
    public class FormsAuthentic
    {
        public static void Set(string name, DateTime expiration, string userData)
        {
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, name, DateTime.Now, expiration, true, userData, FormsAuthentication.FormsCookiePath);

            //Encrypt the cookie using the machine key for secure transport
            string hash = FormsAuthentication.Encrypt(ticket);

            //Hashed ticket
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, hash);

            //Set the cookie's expiration time to the tickets expiration time
            if (ticket.IsPersistent)
            {
                cookie.Expires = ticket.Expiration;
            }

            // Add the cookie to the list for outgoing response
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public static void Set<T>(string name, DateTime expiration, T userData) where T : class
        {
            //序列化对象数据
            string data = Utility.Serialization.FromModelToString<T>(userData);
            
            //加入票证中
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, name, DateTime.Now, expiration, true, data, FormsAuthentication.FormsCookiePath);

            //Encrypt the cookie using the machine key for secure transport
            string hash = FormsAuthentication.Encrypt(ticket);

            //Hashed ticket
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, hash); 

            //Set the cookie's expiration time to the tickets expiration time
            if (ticket.IsPersistent)
            {
                cookie.Expires = ticket.Expiration;
            }

            // Add the cookie to the list for outgoing response
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        /// <summary>
        /// 获取当前用户数据
        /// </summary>
        /// <returns></returns>
        public static string Get()
        {
            if (HttpContext.Current.User != null)
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    if (HttpContext.Current.User.Identity is FormsIdentity)
                    {
                        FormsIdentity id = (FormsIdentity)HttpContext.Current.User.Identity;
                        FormsAuthenticationTicket ticket = id.Ticket;

                        // Get the stored user-data
                        return ticket.UserData;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 获取当前用户数据
        /// </summary>
        /// <returns></returns>
        public static T Get<T>() where T : class
        {
            if (HttpContext.Current.User != null)
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    if (HttpContext.Current.User.Identity is FormsIdentity)
                    {
                        FormsIdentity id = (FormsIdentity)HttpContext.Current.User.Identity;
                        FormsAuthenticationTicket ticket = id.Ticket;

                        // Get the stored user-data
                        string data = ticket.UserData;

                        //反序列化
                        return Utility.Serialization.FromStringToModel<T>(data);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 提供给 Application_AuthenticateRequest(object sender, EventArgs e) 使用
        /// </summary>
        public static void Load()
        {
            if (HttpContext.Current.User != null)
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    if (HttpContext.Current.User.Identity is FormsIdentity)
                    {
                        FormsIdentity id = (FormsIdentity)HttpContext.Current.User.Identity;
                        FormsAuthenticationTicket ticket = id.Ticket;

                        // Get the stored user-data, in this case, our roles
                        string[] userData = new string[1] { ticket.UserData };
                        HttpContext.Current.User = new GenericPrincipal(id, userData);
                    }
                }
            }
        }

        /// <summary>
        /// 退出票证
        /// </summary>
        public static void Out()
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie != null)
            {
                //将cookies过期
                cookie.Expires = DateTime.Now.AddDays(-1);
            }
            //退出验证 
            FormsAuthentication.SignOut();
            HttpContext.Current.Response.Redirect(FormsAuthentication.LoginUrl);
        }
    }
}
