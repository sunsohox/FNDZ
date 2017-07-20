using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FNDZWeb.Common
{
    public class AUserCustomerList
    {
        public virtual string UserCustomerCode { get; set; }
        public virtual string CustomerName { get; set; }
        public virtual string activeShow { get; set; }
        public virtual string isDefaultShow { get; set; }
    }
}