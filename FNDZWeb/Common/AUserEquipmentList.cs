using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FNDZWeb.Common
{
    public class AUserEquipmentList
    {
        public virtual string UserEquipmentCode { get; set; }
        public virtual string EquipmentName { get; set; }
        public virtual string ProductCode { get; set; }
        public virtual string EquipmentType { get; set; }
    }
}