using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FNDZWeb.Common
{
    public class ACustomerEquipmentFloorList
    {
        public virtual string CustFloorCode { get; set; }
        public virtual string CustFloorID { get; set; }
        public virtual string FloorMun { get; set; }
        public virtual string FloorName { get; set; }
    }
}