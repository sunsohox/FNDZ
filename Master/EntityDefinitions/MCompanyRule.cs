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
    public class MCompanyRule
    {
        public static MCompanyData GetCompanyData()
        {
            MCompanyData ds = new MCompanyData();
            string strSQL = "select * from MCompany order by CompanyName ";
            SqlCommand cmd = new SqlCommand();
            DBExecute.GetData(cmd, strSQL, ds.MCompany);
            return ds;
        }

        public static MCompanyData GetCompanyDataByCode(string CompanyCode)
        {
            MCompanyData ds = new MCompanyData();
            string strSQL = "select * from MCompany where CompanyCode = @CompanyCode ";
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);

            DBExecute.GetData(cmd, strSQL, ds.MCompany);
            return ds;
        }

        public static void UpdateData(MCompanyData dsMCompanyData)
        {
            DBExecute.UpdateData(dsMCompanyData.MCompany);
        }

        public static void Delete(string CompanyCode)
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql += "Delete FROM MCompany where CompanyCode in ('" + CompanyCode.Replace(",","','") + "');";
            DBExecute.ExecuteNonQuery(cmd, sql);
        }


        public static DataTable GetCompanyDataDByCode(string CompanyCode)
        {
            DataTable dt = new DataTable();
            string strSQL = "select * from MCompany where CompanyCode = @CompanyCode ";
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);

            DBExecute.GetData(cmd, strSQL, dt);
            return dt;
        }
    }
}
