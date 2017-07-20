using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Common
{
    public class LoginInfo
    {
        public static string UserCode
        {
            get
            {
                return HttpContext.Current.User.Identity.Name.Split(',')[0];
            }
        }        

        public static string Theme
        {
            get
            {
                try
                {
                    return HttpContext.Current.User.Identity.Name.Split(',')[1];
                }
                catch
                {
                    return "";
                }
            }
        }

        public static string CustomerCode
        {
            get
            {
                return HttpContext.Current.User.Identity.Name.Split(',')[2];
            }
        }


    }
}
