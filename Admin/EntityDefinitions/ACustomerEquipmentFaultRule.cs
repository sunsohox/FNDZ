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
    public class ACustomerEquipmentFaultRule
    {
        public static DataTable GetACustomerEquipmentFaultListByCode(string CustomerCode, string CustEquipmentCode)
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql = " select a.*,b.FaultCode,b.FaultID,b.FaultName from dbo.ACustomerEquipmentFault a  " +
                  "  left outer join dbo.MFaultCodes b on b.FaultCode=a.CustFaultCode "+
                  "   where a.CustomerCode='" + CustomerCode + "' and a.CustEquipmentCode='" + CustEquipmentCode + "'";
            DataTable dt = new DataTable();
            DBExecute.GetData(cmd, sql, dt);
            return dt;
        }

        public static DataTable GetACustomerEquipmentFaultListByID(string ID)
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql = " select a.*,b.FaultCode,b.FaultID,b.FaultName from dbo.ACustomerEquipmentFault a  " +
                  "  left outer join dbo.MFaultCodes b on b.FaultCode=a.CustFaultCode " +
                  "   where a.ID='" + ID + "'";
            DataTable dt = new DataTable();
            DBExecute.GetData(cmd, sql, dt);
            return dt;
        }

        public static DataTable GetACustomerEquipmentFaultByCode(string CustomerCode, string CustEquipmentCode, string CustFaultCode)
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql = "select * from dbo.ACustomerEquipmentFault where CustomerCode='" + CustomerCode + "' and CustEquipmentCode='" + CustEquipmentCode + "' and CustFaultCode='" + CustFaultCode + "'";
            DataTable dt = new DataTable();
            DBExecute.GetData(cmd, sql, dt);
            return dt;
        }

        public static DataTable GetACustomerEquipmentFaultIDByCode(string CustomerCode, string CustEquipmentCode)
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql = "select * from dbo.ACustomerEquipmentFault where CustomerCode='" + CustomerCode + "' and CustEquipmentCode='" + CustEquipmentCode + "' ";
            DataTable dt = new DataTable();
            DBExecute.GetData(cmd, sql, dt);
            return dt;
        }

        public static bool InsertACustomerEquipmentFault(string ID, string CustomerCode, string CustEquipmentCode, string CustFaultCode, string CustFaultID)
        {
            string sql = "insert into dbo.ACustomerEquipmentFault(ID,CustomerCode,CustEquipmentCode,CustFaultCode,CustFaultID) values('" + ID + "','" + CustomerCode + "','" + CustEquipmentCode + "','" + CustFaultCode + "','" + CustFaultID + "')";
            int i = DBExecute.ExecuteSQL(sql);
            if (i > 0) return true; else return false;
        }

        public static bool UpdateACustomerEquipmentFault(string ID, string CustomerCode, string CustEquipmentCode, string CustFaultCode, string CustFaultID)
        {
            string sql = "update dbo.ACustomerEquipmentFault set CustFaultID='" + CustFaultID + "' where ID='" + ID + "' and CustomerCode='" + CustomerCode + "' and CustEquipmentCode='" + CustEquipmentCode + "' and CustFaultCode='" + CustFaultCode + "'";
            int i = DBExecute.ExecuteSQL(sql);
            if (i > 0) return true; else return false;
        }

        public static bool DeleACustomerEquipmentFault(string CustomerCode, string CustEquipmentCode, string CustFaultCode)
        {
            string sql = "delete from dbo.ACustomerEquipmentFault where CustomerCode='" + CustomerCode + "' and CustEquipmentCode='" + CustEquipmentCode + "' and CustFaultCode='" + CustFaultCode + "'";
            int i = DBExecute.ExecuteSQL(sql);
            if (i > 0) return true; else return false;
        }


        public static bool CopyACustomerEquipmentFault(string ID,string NewID, string CustomerCode, string CustEquipmentCode)
        {
            string sql = "delete from dbo.ACustomerEquipmentFault where CustomerCode='" + CustomerCode + "' and CustEquipmentCode='" + CustEquipmentCode + "' " +
                         "   insert into dbo.ACustomerEquipmentFault(ID, CustomerCode, CustEquipmentCode, CustFaultCode,CustFaultID) " +
                         "   select '" + NewID + "', '" + CustomerCode + "', '" + CustEquipmentCode + "', CustFaultCode,CustFaultID from dbo.ACustomerEquipmentFault where ID='" + ID + "'";
            int i = DBExecute.ExecuteSQL(sql);
            if (i > 0) return true; else return false;
        }


        public static DataTable GetACustomerEquipmentFault()
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql = "select ID from dbo.ACustomerEquipmentFault group by ID order by ID ";
            DataTable dt = new DataTable();
            DBExecute.GetData(cmd, sql, dt);
            return dt;
        }

        public static DataTable GetACustomerEquipmentFaultN()
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql = "select b.CustomerName,c.EquipmentName,a.ID from dbo.ACustomerEquipmentFault  a "+
                  "  left outer join dbo.ACustomer b on b.CustomerCode=a.CustomerCode "+
                  "  left outer join dbo.MEquipment c on c.EquipmentCode=a.CustEquipmentCode "+
                  "  group by b.CustomerName,c.EquipmentName,a.ID  order by b.CustomerName,c.EquipmentName,a.ID  ";
            DataTable dt = new DataTable();
            DBExecute.GetData(cmd, sql, dt);
            return dt;
        }
    }
}
