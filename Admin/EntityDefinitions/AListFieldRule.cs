using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Admin.EntityDAO;
using Common;

namespace Admin.EntityDefinitions
{
    public class AListFieldRule
    {
        public static DataTable GetAListFieldListData()
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql = "  select ScreenCode,ScreenName from dbo.AListField group by ScreenCode,ScreenName";
            DataTable dt = new DataTable();
            DBExecute.GetData(cmd, sql, dt);
            return dt;
        }
    }
}
