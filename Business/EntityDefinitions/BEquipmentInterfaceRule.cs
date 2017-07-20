using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Business.EntityDAO;
using Common;

namespace Business.EntityDefinitions
{
    public class BEquipmentInterfaceRule
    {
        public static DataTable GetBEquipmentInterfaceByCode(string EquipmentInterfaceID)
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql = "select a.* from dbo.BEquipmentInterface a   where a.ID='" + EquipmentInterfaceID + "'";
            DataTable dt = new DataTable();
            DBExecute.GetData(cmd, sql, dt);
            return dt;
        }

        public static DataTable GetBEquipmentIntByEquipmentCode(string EquipmentCode)
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql = "select top 1 a.* from dbo.BEquipmentInterface a   where a.EquipmentCode='" + EquipmentCode + "' order by a.CreateOn desc";
            DataTable dt = new DataTable();
            DBExecute.GetData(cmd, sql, dt);
            return dt;
        }
    }
}
