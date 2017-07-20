using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FNDZWeb.Common
{
    public class ACustomerEquipmentFaultList
    {
        public virtual string CustFaultCode { get; set; }
        public virtual string CustFaultID { get; set; }
        public virtual string FaultName { get; set; }
    }
}