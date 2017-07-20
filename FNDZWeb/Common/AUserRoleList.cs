using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FNDZWeb.Common
{
    public class AUserRoleList
    {
        public virtual string RoleCode { get; set; }
        public virtual string RoleName { get; set; }
        public virtual string SeleRole { get; set; }
        public virtual string UserCode { get; set; }
        public virtual string UserName { get; set; }
    }
}