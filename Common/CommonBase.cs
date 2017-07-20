using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Common
{
    public class CommonBase
    {
        public static string GetSequence(string sInfo,string sSequenceCode, string sCustomerCode)
        {
            string sReturn = sInfo;
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql = "  select * from dbo.MSequence where SequenceCode='" + sSequenceCode + "' and CustomerCode='" + sCustomerCode + "'";
            DataTable dt = new DataTable();
            DBExecute.GetData(cmd, sql, dt);
            if (dt.Rows.Count > 0)
            {
                sReturn += (int.Parse(dt.Rows[0]["SequenceCount"].ToString()) + 1).ToString().PadLeft(5, '0');
                sql = "  update dbo.MSequence set SequenceCount=SequenceCount+1 where SequenceCode='" + sSequenceCode + "' and CustomerCode='" + sCustomerCode + "'";
            }
            else {
                sReturn += (int.Parse("1") ).ToString().PadLeft(5, '0');
                sql = "  insert into dbo.MSequence select '" + sSequenceCode + "','1','" + sCustomerCode + "'";

            }
            DBExecute.ExecuteSQL(sql);
            return sReturn;
        }
    }
}
