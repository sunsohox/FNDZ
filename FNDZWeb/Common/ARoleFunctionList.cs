using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FNDZWeb.Common
{
    public class ARoleFunctionList
    {
        public virtual string FunctionCode { get; set; }
        public virtual string FunctionName { get; set; }
        public virtual string ModuleCode { get; set; }
        public virtual string RoleCode { get; set; }
        public virtual string RoleName { get; set; }
        public virtual string SeleRole { get; set; }
    }
}