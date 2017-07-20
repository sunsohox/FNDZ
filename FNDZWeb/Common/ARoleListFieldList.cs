using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FNDZWeb.Common
{
    public class ARoleListFieldList
    {
        public virtual int Choice { get; set; }
        public virtual string Field { get; set; }
        public virtual string FieldName { get; set; }
        public virtual string RoleCode { get; set; }
        public virtual string RoleName { get; set; }
        public virtual string ScreenCode { get; set; }
        public virtual string SeleField { get; set; }
    }
}