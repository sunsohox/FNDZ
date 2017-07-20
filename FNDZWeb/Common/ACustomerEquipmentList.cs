using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FNDZWeb.Common
{
    public class ACustomerEquipmentList
    {
        public virtual string CustEquipmentCode { get; set; }
        public virtual string EquipmentName { get; set; }
        public virtual string ProductCode { get; set; }
        public virtual string EquipmentType { get; set; }
    }
}