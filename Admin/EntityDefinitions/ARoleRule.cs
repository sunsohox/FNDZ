using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Admin.EntityDAO;
using Common;

namespace Admin.EntityDefinitions
{
    public class ARoleRule
    {
        public static void Delete(string RoleCode)
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql += "Delete FROM dbo.ARole where RoleCode in (@RoleCode);";
            sql += "Delete FROM dbo.ARoleFunction where RoleCode in (@RoleCode);";
            sql += "Delete FROM dbo.ARoleListField where RoleCode in (@RoleCode);";
            sql += "Delete FROM dbo.ARoleSysFunction where RoleCode in (@RoleCode);";
            cmd.Parameters.AddWithValue("@RoleCode", RoleCode);
            DBExecute.ExecuteNonQuery(cmd, sql);
        }

        public static ARoleData GetRoleData()
        {
            ARoleData ds = new ARoleData();
            string strSQL = "select * from ARole order by RoleCode ";
            SqlCommand cmd = new SqlCommand();
            DBExecute.GetData(cmd, strSQL, ds.ARole);
            return ds;
        }

        public static ARoleData GetRoleDataByCode(string RoleCode)
        {
            ARoleData ds = new ARoleData();
            string strSQL = "select * from ARole where RoleCode = @RoleCode ";
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@RoleCode", RoleCode);

            DBExecute.GetData(cmd, strSQL, ds.ARole);
            return ds;
        }

        public static void UpdateData(ARoleData dsARoleData)
        {
            DBExecute.UpdateData(dsARoleData.ARole);
        }
    }
}
