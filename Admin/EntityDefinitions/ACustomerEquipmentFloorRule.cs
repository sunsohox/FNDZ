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
    public class ACustomerEquipmentFloorRule
    {
        public static DataTable GetACustomerEquipmentFloorListByCode(string CustomerCode, string CustEquipmentCode)
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql = " select a.*,b.FloorCode,b.FloorMun,b.FloorName,b.FloorID from dbo.ACustomerEquipmentFloor a  " +
                  "  left outer join dbo.MFloor b on b.FloorCode=a.CustFloorCode " +
                  "   where a.CustomerCode='" + CustomerCode + "' and a.CustEquipmentCode='" + CustEquipmentCode + "'";
            DataTable dt = new DataTable();
            DBExecute.GetData(cmd, sql, dt);
            return dt;
        }

        public static DataTable GetACustomerEquipmentFloorListByID(string ID)
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql = " select a.*,b.FloorCode,b.FloorMun,b.FloorName from dbo.ACustomerEquipmentFloor a  " +
                  "  left outer join dbo.MFloor b on b.FloorCode=a.CustFloorCode " +
                  "   where a.ID='" + ID + "'";
            DataTable dt = new DataTable();
            DBExecute.GetData(cmd, sql, dt);
            return dt;
        }

        public static DataTable GetACustomerEquipmentFloorByCode(string CustomerCode, string CustEquipmentCode, string CustFloorCode)
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql = "select * from dbo.ACustomerEquipmentFloor where CustomerCode='" + CustomerCode + "' and CustEquipmentCode='" + CustEquipmentCode + "' and CustFloorCode='" + CustFloorCode + "'";
            DataTable dt = new DataTable();
            DBExecute.GetData(cmd, sql, dt);
            return dt;
        }

        public static DataTable GetACustomerEquipmentFloorIDByCode(string CustomerCode, string CustEquipmentCode)
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql = "select * from dbo.ACustomerEquipmentFloor where CustomerCode='" + CustomerCode + "' and CustEquipmentCode='" + CustEquipmentCode + "' ";
            DataTable dt = new DataTable();
            DBExecute.GetData(cmd, sql, dt);
            return dt;
        }

        public static bool InsertACustomerEquipmentFloor(string ID, string CustomerCode, string CustEquipmentCode, string CustFloorCode, string CustFloorID)
        {
            string sql = "insert into dbo.ACustomerEquipmentFloor(ID,CustomerCode,CustEquipmentCode,CustFloorCode,CustFloorID) values('" + ID + "','" + CustomerCode + "','" + CustEquipmentCode + "','" + CustFloorCode + "','" + CustFloorID + "')";
            int i = DBExecute.ExecuteSQL(sql);
            if (i > 0) return true; else return false;
        }

        public static bool UpdateACustomerEquipmentFloor(string ID, string CustomerCode, string CustEquipmentCode, string CustFloorCode, string CustFloorID)
        {
            string sql = "update dbo.ACustomerEquipmentFloor set CustFloorID='" + CustFloorID + "' where ID='" + ID + "' and CustomerCode='" + CustomerCode + "' and CustEquipmentCode='" + CustEquipmentCode + "' and CustFloorCode='" + CustFloorCode + "'";
            int i = DBExecute.ExecuteSQL(sql);
            if (i > 0) return true; else return false;
        }

        public static bool DeleACustomerEquipmentFloor(string CustomerCode, string CustEquipmentCode, string CustFloorCode)
        {
            string sql = "delete from dbo.ACustomerEquipmentFloor where CustomerCode='" + CustomerCode + "' and CustEquipmentCode='" + CustEquipmentCode + "' and CustFloorCode='" + CustFloorCode + "'";
            int i = DBExecute.ExecuteSQL(sql);
            if (i > 0) return true; else return false;
        }


        public static bool CopyACustomerEquipmentFloor(string ID, string NewID, string CustomerCode, string CustEquipmentCode)
        {
            string sql = "delete from dbo.ACustomerEquipmentFloor where CustomerCode='" + CustomerCode + "' and CustEquipmentCode='" + CustEquipmentCode + "' " +
                         "   insert into dbo.ACustomerEquipmentFloor(ID, CustomerCode, CustEquipmentCode, CustFloorCode,CustFloorID) " +
                         "   select '" + NewID + "', '" + CustomerCode + "', '" + CustEquipmentCode + "', CustFloorCode,CustFloorID from dbo.ACustomerEquipmentFloor where ID='" + ID + "'";
            int i = DBExecute.ExecuteSQL(sql);
            if (i > 0) return true; else return false;
        }


        public static DataTable GetACustomerEquipmentFloor()
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql = "select ID from dbo.ACustomerEquipmentFloor group by ID order by ID ";
            DataTable dt = new DataTable();
            DBExecute.GetData(cmd, sql, dt);
            return dt;
        }

        public static DataTable GetACustomerEquipmentFloorN()
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql = "select b.CustomerName,c.EquipmentName,a.ID from dbo.ACustomerEquipmentFloor  a " +
                  "  left outer join dbo.ACustomer b on b.CustomerCode=a.CustomerCode " +
                  "  left outer join dbo.MEquipment c on c.EquipmentCode=a.CustEquipmentCode " +
                  "  group by b.CustomerName,c.EquipmentName,a.ID  order by b.CustomerName,c.EquipmentName,a.ID  ";
            DataTable dt = new DataTable();
            DBExecute.GetData(cmd, sql, dt);
            return dt;
        }
    }
}
