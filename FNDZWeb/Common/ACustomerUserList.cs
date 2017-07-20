using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FNDZWeb.Common
{
    public class ACustomerUserList
    {
        public virtual string UserCode { get; set; }
        public virtual string UserName { get; set; }
        public virtual string IsCustomerUser { get; set; }
    }
}