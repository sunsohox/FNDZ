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
    public class MFloorRule
    {
        public static MFloorData GetFloorData()
        {
            MFloorData ds = new MFloorData();
            string strSQL = "select * from MFloor order by FloorName ";
            SqlCommand cmd = new SqlCommand();
            DBExecute.GetData(cmd, strSQL, ds.MFloor);
            return ds;
        }

        public static MFloorData GetFloorDataOrderByFloorMun()
        {
            MFloorData ds = new MFloorData();
            string strSQL = "select * from MFloor order by FloorMun ";
            SqlCommand cmd = new SqlCommand();
            DBExecute.GetData(cmd, strSQL, ds.MFloor);
            return ds;
        }

        public static MFloorData GetFloorDataByCode(string FloorCode)
        {
            MFloorData ds = new MFloorData();
            string strSQL = "select * from MFloor where FloorCode = @FloorCode ";
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@FloorCode", FloorCode);

            DBExecute.GetData(cmd, strSQL, ds.MFloor);
            return ds;
        }

        public static void UpdateData(MFloorData dsMFloorData)
        {
            DBExecute.UpdateData(dsMFloorData.MFloor);
        }

        public static void Delete(string FloorCode)
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql += "Delete FROM MFloor where FloorCode in ('" + FloorCode.Replace(",","','") + "');";
            DBExecute.ExecuteNonQuery(cmd, sql);
        }
    }
}
