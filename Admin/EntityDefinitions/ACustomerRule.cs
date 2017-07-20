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
    public class ACustomerRule
    {
        public static ACustomerData GetCustomerData()
        {
            ACustomerData ds = new ACustomerData();
            string strSQL = "select * from ACustomer order by CustomerName ";
            SqlCommand cmd = new SqlCommand();
            DBExecute.GetData(cmd, strSQL, ds.ACustomer);
            return ds;
        }

        public static ACustomerData GetCustomerDataByCode(string CustomerCode)
        {
            ACustomerData ds = new ACustomerData();
            string strSQL = "select * from ACustomer where CustomerCode = @CustomerCode ";
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@CustomerCode", CustomerCode);

            DBExecute.GetData(cmd, strSQL, ds.ACustomer);
            return ds;
        }

        public static void UpdateData(ACustomerData dsACustomerData)
        {
            DBExecute.UpdateData(dsACustomerData.ACustomer);
        }

        public static void Delete(string CustomerCode)
        {
            string cust = CustomerCode.Replace(",", "','");;
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql += "Delete FROM ACustomer where CustomerCode in ('" + cust + "'); " +
            "delete from dbo.ACustomerUser where UserCustomerCode in ('" + cust + "'); " +
            "delete from dbo.ACustomerEquipment where CustomerCode in ('" + cust + "'); " +
            "delete from dbo.ACustomerEquipmentFault where CustomerCode in ('" + cust + "'); " +
            "delete from dbo.ACustomerEquipmentFloor where CustomerCode in ('" + cust + "'); " +
            "delete  from dbo.AUserEquipment where UserEquipmentCode in (select CustEquipmentCode from dbo.ACustomerEquipment where CustomerCode in ('" + cust + "'))";
            DBExecute.ExecuteNonQuery(cmd, sql);
        }

        public static string GetInterval()
        {
            return System.Configuration.ConfigurationSettings.AppSettings["Interval"];
        }

        public static string GetIntervalMes()
        {
            return System.Configuration.ConfigurationSettings.AppSettings["IntervalMes"];
        }
    }
}
