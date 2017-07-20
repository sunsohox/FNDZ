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
    public class AUserEquipmentRule
    {

        public static AUserEquipmentData GetAUserEquipmentByCode(string EquipmentCode, string UserCode)
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql = "select * from dbo.AUserEquipment where UserEquipmentCode='" + EquipmentCode + "' and UserCode='" + UserCode + "'";
            AUserEquipmentData dsAUserEquipmentData = new AUserEquipmentData();
            DBExecute.GetData(cmd, sql, dsAUserEquipmentData.AUserEquipment);
            return dsAUserEquipmentData;
        }

        public static bool InsertAUserEquipment(string EquipmentCode, string UserCode)
        {
            string sql = "insert into dbo.AUserEquipment(UserEquipmentCode,UserCode) values('" + EquipmentCode + "','" + UserCode + "')";
            int i = DBExecute.ExecuteSQL(sql);
            if (i > 0) return true; else return false;
        }

        public static bool DeleAUserEquipment(string EquipmentCode, string UserCode)
        {
            string sql = "delete from dbo.AUserEquipment where UserEquipmentCode='" + EquipmentCode + "' and UserCode='" + UserCode + "'";
            int i = DBExecute.ExecuteSQL(sql);
            if (i > 0) return true; else return false;
        }

        public static void UpdateData(AUserEquipmentData dsAUserEquipmentData)
        {
            DBExecute.UpdateData(dsAUserEquipmentData.AUserEquipment);
        }


        public static DataTable GetUserEquipmentByUserCode(string UserCode)
        {
            SqlCommand cmd = new SqlCommand();
            DataTable dt = new DataTable();
            string sql = "select a.*,b.EquipmentName,b.ProductCode,case when b.EquipmentType='1' then '电梯' when b.EquipmentType='2' then '扶梯' when b.EquipmentType='3' then '自动人行道' end as EquipmentType from dbo.AUserEquipment a  " +
                         " left outer join dbo.MEquipment b on b.EquipmentCode=a.UserEquipmentCode where a.UserCode='" + UserCode + "' order by a.UserEquipmentCode";
            DBExecute.GetData(cmd, sql, dt);
            return dt;
        }

        public static DataTable GetMEquipmentByUserCode(string UserCode)
        {
            SqlCommand cmd = new SqlCommand();
            DataTable dt = new DataTable();
            string sql = "select * from dbo.MEquipment where EquipmentCode  "+
                        " in (select CustEquipmentCode from dbo.ACustomerEquipment where CustomerCode  " +
                        " in (select UserCustomerCode from dbo.ACustomerUser where UserCode='" + UserCode + "') )";
            DBExecute.GetData(cmd, sql, dt);
            return dt;
        }
    }
}
