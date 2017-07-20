using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Master.EntityDAO;
using Common;

namespace Master.EntityDefinitions
{
    public class MFaultCodesRule
    {
        public static MFaultCodesData GetMFaultCodesData()
        {
            MFaultCodesData ds = new MFaultCodesData();
            string strSQL = "select * from MFaultCodes order by FaultName ";
            SqlCommand cmd = new SqlCommand();
            DBExecute.GetData(cmd, strSQL, ds.MFaultCodes);
            return ds;
        }

        public static MFaultCodesData GetMFaultCodesDataOrderByFaultID()
        {
            MFaultCodesData ds = new MFaultCodesData();
            string strSQL = "select * from MFaultCodes order by FaultID ";
            SqlCommand cmd = new SqlCommand();
            DBExecute.GetData(cmd, strSQL, ds.MFaultCodes);
            return ds;
        }

        public static MFaultCodesData GetMFaultCodesDataByCode(string FaultCode)
        {
            MFaultCodesData ds = new MFaultCodesData();
            string strSQL = "select * from MFaultCodes where FaultCode = @FaultCode ";
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@FaultCode", FaultCode);

            DBExecute.GetData(cmd, strSQL, ds.MFaultCodes);
            return ds;
        }

        public static void UpdateData(MFaultCodesData dsMFaultCodesData)
        {
            DBExecute.UpdateData(dsMFaultCodesData.MFaultCodes);
        }

        public static void Delete(string FaultCode)
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql += "Delete FROM MFaultCodes where FaultCode in ('" + FaultCode.Replace(",","','") + "');";
            DBExecute.ExecuteNonQuery(cmd, sql);
        }
    }
}
