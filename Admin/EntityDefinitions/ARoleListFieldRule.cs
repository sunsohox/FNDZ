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
    public class ARoleListFieldRule
    {
        public static DataTable GetListData(string RoleCode, string ScreenCode)
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql = " select * from (select a.ScreenCode,a.ScreenName,'" + RoleCode + "' as RoleCode " +
                                "    ,(select RoleName from ARole c  where c.RoleCode='" + RoleCode + "') as RoleName, a.Field, a.FieldName " +
                                "    ,case when(select COUNT(*) from dbo.ARoleListField b where b.RoleCode='" + RoleCode + "'  and b.ScreenCode=a.ScreenCode and b.Field=a.Field)>0 then 'Y' else 'N' end as SeleField  " +
                                "    ,isnull((select b.Choice from dbo.ARoleListField b where b.RoleCode='" + RoleCode + "'  and b.ScreenCode=a.ScreenCode and b.Field=a.Field) ,a.Choice) as Choice " +
                                "    from dbo.AListField a where a.ScreenCode='" + ScreenCode + "'  ) a  order by a.Choice";
            DataTable dt = new DataTable();
            DBExecute.GetData(cmd, sql, dt);
            return dt;

            
        }

        public static DataTable GetRoleListFieldData(string ScreenCode, string Field, string RoleCode)
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql = "select * from dbo.ARoleListField where ScreenCode='" + ScreenCode + "' and  Field='" + Field + "' and RoleCode='" + RoleCode + "'";
            DataTable dt = new DataTable();
            DBExecute.GetData(cmd, sql, dt);
            return dt;
        }

        public static bool InsertARoleListField(string ScreenCode, string Field, string RoleCode, string Choice)
        {
            string sql = "insert into dbo.ARoleListField( ScreenCode, Field,  RoleCode,Choice) values('" + ScreenCode + "','" + Field + "','" + RoleCode + "','" + Choice + "')";
            int i = DBExecute.ExecuteSQL(sql);
            if (i > 0) return true; else return false;

        }

        public static bool DeleRoleListField(string ScreenCode, string Field, string RoleCode)
        {
            string sql = "delete from dbo.ARoleListField where ScreenCode='" + ScreenCode + "' and Field='" + Field + "' and RoleCode='" + RoleCode + "'";
            int i = DBExecute.ExecuteSQL(sql);
            if (i > 0) return true; else return false;
        }

    }
}
