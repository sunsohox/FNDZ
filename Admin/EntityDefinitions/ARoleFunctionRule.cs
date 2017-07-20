using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Admin.EntityDAO;
using Common;

namespace Admin.EntityDefinitions
{
    public class ARoleFunctionRule
    {
        public static DataTable GetRoleFunctionByUser(string usercode)
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql = "select distinct RF.FunctionCode from ARoleFunction RF " + ((usercode.ToLower() == "admin") ? "" : "inner join AUserRole UR on RF.RoleCode=UR.RoleCode where UR.UserCode='" + usercode + "' order by RF.FunctionCode");
            DataTable dt = new DataTable();
            DBExecute.GetData(cmd, sql, dt);
            return dt;
        }

        public static DataTable GetListDataByCode(string RoleCode, string ModuleCode)
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql = "select a.ModuleCode,'" + RoleCode + "' as RoleCode,(select RoleName from ARole c where c.RoleCode='" + RoleCode + "') as RoleName, a.FunctionCode, a.FunctionName " +
                      "  ,case when(select COUNT(*) from ARoleFunction b where b.RoleCode='" + RoleCode + "' " +
                      "  and b.FunctionCode=a.FunctionCode)>0 then 'Y' else 'N' end as SeleRole " +
                      "  from dbo.AFunction a where a.ModuleCode='" + ModuleCode + "'";
            DataTable dt = new DataTable();
            DBExecute.GetData(cmd, sql, dt);
            return dt;
        }

        public static DataTable GetRoleFunctionByFunctionCodeandRoleCode(string FunctionCode, string RoleCode)
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql = "select * from dbo.ARoleFunction where FunctionCode='" + FunctionCode + "' and RoleCode='" + RoleCode + "'";
            DataTable dt = new DataTable();
            DBExecute.GetData(cmd, sql, dt);
            return dt;
        }

        public static bool InsertARoleFunction(string RoleCode, string FunctionCode)
        {
            string sql = "insert into dbo.ARoleFunction values('" + FunctionCode + "','" + RoleCode + "')";
            int i = DBExecute.ExecuteSQL(sql);
            if (i > 0) return true; else return false;
        }

        public static bool DeleARoleFunction(string RoleCode, string FunctionCode)
        {
            string sql = "delete from dbo.ARoleFunction where FunctionCode='" + FunctionCode + "' and RoleCode='" + RoleCode + "'";
            int i = DBExecute.ExecuteSQL(sql);
            if (i > 0) return true; else return false;
        }

        public static DataTable GetRoleFunctionBy(string FunctionCode, string UserCode)
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql = "select * from dbo.ARoleFunction where RoleCode in (select RoleCode from dbo.AUserRole where UserCode='" + UserCode + "') and FunctionCode='" + FunctionCode + "'";
            DataTable dt = new DataTable();
            DBExecute.GetData(cmd, sql, dt);
            return dt;
        }      


        
    }
}
