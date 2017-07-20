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
    public class ACustomerEquipmentRule
    {
        //public static DataTable GetACustomerEquipmentListData(string CustomerCode, string EquipmentCode, string EquipmentName)
        //{
        //    SqlCommand cmd = new SqlCommand();
        //    string sql = "";
        //    sql = "  select a.*,case when ISNULL(b.EquipmentCode,'')='' then 'N' else 'Y' end as IsCustomerEquipment,b.CustomerInterFace,b.EquipmentFloorCode  from dbo.MEquipment a " +
        //          "  left outer join dbo.ACustomerEquipment b on b.EquipmentCode=a.EquipmentCode and b.CustomerCode='" + CustomerCode + "' " +
        //          "  where 1=1 " + ((string.IsNullOrEmpty(EquipmentCode)) ? "" : " and a.EquipmentCode like '%" + EquipmentCode + "%'")
        //    + ((string.IsNullOrEmpty(EquipmentName)) ? "" : " and a.EquipmentName like '%" + EquipmentName + "%'") + " order by a.EquipmentName";
        //    DataTable dt = new DataTable();
        //    DBExecute.GetData(cmd, sql, dt);
        //    return dt;
        //}
        public static DataTable GetACustomerEquipmentByCustomerCode(string CustomerCode)
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql = "select * from dbo.ACustomerEquipment where CustomerCode='" + CustomerCode + "'";
            DataTable dt = new DataTable();
            DBExecute.GetData(cmd, sql, dt);
            return dt;
        }

        public static DataTable GetACustomerEquipmentListByCustomerCode(string CustomerCode)
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql = " select a.*,b.EquipmentName,b.ProductCode,case b.EquipmentType when 1 then '电梯' when 2 then '扶梯' when 3 then '自动人行道' end EquipmentType from dbo.ACustomerEquipment a " +
                  " left outer join dbo.MEquipment b on b.EquipmentCode=a.CustEquipmentCode where a.CustomerCode='" + CustomerCode + "'";
            DataTable dt = new DataTable();
            DBExecute.GetData(cmd, sql, dt);
            return dt;
        }

        public static DataTable GetACustomerEquipmentListByCustomerCodeOnly(string CustomerCode)
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql = " select a.*,b.EquipmentName,b.ProductCode,case b.EquipmentType when 1 then '电梯' when 2 then '扶梯' when 3 then '自动人行道' end EquipmentType from dbo.ACustomerEquipment a " +
                  " left outer join dbo.MEquipment b on b.EquipmentCode=a.CustEquipmentCode where a.CustomerCode='" + CustomerCode + "'  and b.EquipmentType='1'";
            DataTable dt = new DataTable();
            DBExecute.GetData(cmd, sql, dt);
            return dt;
        }




        public static DataTable GetACustomerEquipmentByCode(string CustomerCode, string CustEquipmentCode)
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql = "select * from dbo.ACustomerEquipment where CustomerCode='" + CustomerCode + "' and CustEquipmentCode='" + CustEquipmentCode + "'";
            DataTable dt = new DataTable();
            DBExecute.GetData(cmd, sql, dt);
            return dt;
        }

        public static ACustomerEquipmentData GetACustomerEquipmentByCodeN(string CustomerCode, string CustEquipmentCode)
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql = "select * from dbo.ACustomerEquipment where CustomerCode='" + CustomerCode + "' and CustEquipmentCode='" + CustEquipmentCode + "'";
            ACustomerEquipmentData dsACustomerEquipmentData = new ACustomerEquipmentData();
            DBExecute.GetData(cmd, sql, dsACustomerEquipmentData.ACustomerEquipment);
            return dsACustomerEquipmentData;
        }

        public static bool InsertACustomerEquipment(string CustomerCode, string CustEquipmentCode)
        {
            string sql = "insert into dbo.ACustomerEquipment(CustomerCode,CustEquipmentCode) values('" + CustomerCode + "','" + CustEquipmentCode + "')";
            int i = DBExecute.ExecuteSQL(sql);
            if (i > 0) return true; else return false;
        }

        public static bool DeleACustomerEquipment(string CustomerCode, string CustEquipmentCode)
        {
            string sql = 
                " delete from dbo.AUserEquipment where UserEquipmentCode in (select CustEquipmentCode from dbo.ACustomerEquipment where CustomerCode='" + CustomerCode + "') and UserEquipmentCode='" + CustEquipmentCode + "'"+
                " delete from dbo.ACustomerEquipment where CustomerCode='" + CustomerCode + "' and CustEquipmentCode='" + CustEquipmentCode + "';" +
                " delete from dbo.ACustomerEquipmentFault where CustomerCode='" + CustomerCode + "' and CustEquipmentCode='" + CustEquipmentCode + "' ;" +
                " delete from dbo.ACustomerEquipmentFloor where CustomerCode='" + CustomerCode + "' and CustEquipmentCode='" + CustEquipmentCode + "' ;";
            int i = DBExecute.ExecuteSQL(sql);
            if (i > 0) return true; else return false;
        }

        public static void UpdateData(ACustomerEquipmentData dsACustomerEquipmentData)
        {
            DBExecute.UpdateData(dsACustomerEquipmentData.ACustomerEquipment);
        }
    }
}
