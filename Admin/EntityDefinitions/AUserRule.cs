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
    public class AUserRule
    {
        public static AUserData GetUserData()
        {
            AUserData ds = new AUserData();
            string strSQL = "select * from AUser order by UserName ";
            SqlCommand cmd = new SqlCommand();
            DBExecute.GetData(cmd, strSQL, ds.AUser);
            return ds;
        }

        public static AUserData GetUserDataByCode(string userCode)
        {
            AUserData ds = new AUserData();
            string strSQL = "select * from AUser where UserCode = @userCode ";
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@userCode", userCode);

            DBExecute.GetData(cmd, strSQL, ds.AUser);
            return ds;
        }

        public static void UpdateData(AUserData dsAUserData)
        {
            DBExecute.UpdateData(dsAUserData.AUser);
        }

        public static void Delete(string UserCode)
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql += "Delete FROM AUserRole where UserCode=@UserCode;";
            sql += "Delete FROM AUser where UserCode=@UserCode;";
            cmd.Parameters.AddWithValue("@UserCode", UserCode);
            DBExecute.ExecuteNonQuery(cmd, sql);
        }

        public static void RemoveUserInfo(string UserCode)
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql += "Delete FROM AUserRole where UserCode in ('" + UserCode.Replace(",","','") + "');";
            sql += "Delete FROM AUser where UserCode in ('" + UserCode.Replace(",", "','") + "');";
            sql += "Delete FROM ACustomerUser where UserCode in ('" + UserCode.Replace(",", "','") + "');";
            sql += "Delete FROM AUserEquipment where UserCode in ('" + UserCode.Replace(",", "','") + "');";
            DBExecute.ExecuteNonQuery(cmd, sql);
        }


        public static DataTable GetUserRoleData(string UserCode)
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "select '" + UserCode + "' as UserCode,(select UserName from dbo.AUser c where c.UserCode='" + UserCode + "') as UserName,a.*, " +
                               " case when(select COUNT(*) from AUserRole b where b.UserCode='" + UserCode + "' " +
                               " and b.RoleCode=a.RoleCode)>0 then 'Y' else 'N' end as SeleRole from dbo.ARole a ";
            DataTable dt = new DataTable();
            DBExecute.GetData(cmd, sql, dt);
            return dt;
        }

        public static  DataTable GetUserRoleDataByUserAndRole(string UserCode, string RoleCode)
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "select * from dbo.AUserRole where UserCode='" + UserCode + "' and RoleCode='" + RoleCode + "'";
            DataTable dt = new DataTable();
            DBExecute.GetData(cmd, sql, dt);
            return dt;
        }

        public static bool InsertAUserRole(string UserCode, string RoleCode)
        {
            string sql = "insert into dbo.AUserRole values('" + UserCode + "','" + RoleCode + "')";
            int i = DBExecute.ExecuteSQL(sql);
            if (i > 0) return true; else return false;
        }

        public static bool DeleAUserRole(string UserCode, string RoleCode)
        {
            string sql = "delete from dbo.AUserRole where UserCode='" + UserCode + "' and RoleCode='" + RoleCode + "'";
            int i = DBExecute.ExecuteSQL(sql);
            if (i > 0) return true; else return false;
        }

        public static DataTable GetUserRoleByUser(string UserCode, bool isSelected)
        {
            SqlCommand cmd = new SqlCommand();
            DataTable dt = new DataTable();
            string sql = "";
            if (isSelected)
                sql = "select ARole.RoleCode,RoleName from AUserRole left join ARole on AUserRole.RoleCode=ARole.RoleCode	 where UserCode=@UserCode";
            else
                sql = "select RoleCode,RoleName from ARole where RoleCode not in	(select RoleCode from AUserRole where UserCode=@UserCode)";
            cmd.Parameters.AddWithValue("@UserCode", UserCode);
            DBExecute.GetData(cmd, sql, dt);
            return dt;
        }

        public static void UpdateUserRole(string UserCode, List<string> selectRole)
        {
            SqlCommand cmd = new SqlCommand();

            string sql = "delete AUserRole where UserCode=@UserCode; ";
            for (int i = 0; i < selectRole.Count; i++)
            {
                sql += "INSERT INTO AUserRole ([UserCode],[RoleCode]) values (@UserCode,'" + selectRole[i] + "'); ";
            }

            cmd.Parameters.AddWithValue("@UserCode", UserCode);

            DBExecute.ExecuteNonQuery(cmd, sql);
        }
    }
}
