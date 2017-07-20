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
    public class ARoleSysFunctionRule
    {
        public static DataTable GetListData(string RoleCode, string ScreenCode)
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql = " select a.ScreenCode,a.ScreenName,'" + RoleCode + "' as RoleCode,(select RoleName from ARole c  " +
                                "    where c.RoleCode='" + RoleCode + "') as RoleName, a.[Function], a.FunctionName " +
                                "    ,case when(select COUNT(*) from dbo.ARoleSysFunction b where b.RoleCode='" + RoleCode + "'  " +
                                "    and b.ScreenCode=a.ScreenCode and b.[Function]=a.[Function])>0 then 'Y' else 'N' end as SeleFunction " +
                                "    from dbo.ASysFunction a where a.ScreenCode='" + ScreenCode + "'";
            DataTable dt = new DataTable();
            DBExecute.GetData(cmd, sql, dt);
            return dt;
        }

        public static DataTable GetARoleSysFunctionData(string ScreenCode, string Function, string RoleCode)
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql = "select * from dbo.ARoleSysFunction where ScreenCode='" + ScreenCode + "' and  Function='" + Function + "' and RoleCode='" + RoleCode + "'";
            DataTable dt = new DataTable();
            DBExecute.GetData(cmd, sql, dt);
            return dt;
        }

        public static bool InsertARoleSysFunction(string ScreenCode, string Function, string RoleCode)
        {
            string sql = "insert into dbo.ARoleSysFunction( ScreenCode, [Function],  RoleCode) values('" + ScreenCode + "','" + Function + "','" + RoleCode + "')";
            int i = DBExecute.ExecuteSQL(sql);
            if (i > 0) return true; else return false;
        }

        public static bool DeleRoleSysFunction(string ScreenCode, string Function, string RoleCode)
        {
            string sql = "delete from dbo.ARoleSysFunction where ScreenCode='" + ScreenCode + "' and [Function]='" + Function + "' and RoleCode='" + RoleCode + "'";
            int i = DBExecute.ExecuteSQL(sql);
            if (i > 0) return true; else return false;
        }
    }
}
