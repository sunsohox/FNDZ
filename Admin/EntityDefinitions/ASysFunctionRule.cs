using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Admin.EntityDAO;
using Common;

namespace Admin.EntityDefinitions
{
    public class ASysFunctionRule
    {
        public static DataTable GetASysFunctionListData()
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql = "  select ScreenCode,ScreenName from dbo.ASysFunction group by ScreenCode,ScreenName";
            DataTable dt = new DataTable();
            DBExecute.GetData(cmd, sql, dt);
            return dt;
        }
    }
}
