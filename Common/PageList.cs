using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Common
{
    public class PageList
    {
        public static DataSet GetListbyPage(string TableName, string FieldKey, int PageCurrent, int PageSize, string FieldShow, string FieldOrder, string strWhere, out  int RecordCount, out int PageCount)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@tbname", SqlDbType.VarChar,1000),
                new SqlParameter("@FieldKey", SqlDbType.VarChar,1000),
                new SqlParameter("@PageCurrent", SqlDbType.Int,4),
                new SqlParameter("@PageSize", SqlDbType.Int,4),
                new SqlParameter("@FieldShow", SqlDbType.VarChar,1000),
                new SqlParameter("@FieldOrder", SqlDbType.VarChar,1000),
                new SqlParameter("@Where", SqlDbType.VarChar,1000),
                new SqlParameter("@RecordCount", SqlDbType.Int,4),
                new SqlParameter("@PageCount", SqlDbType.Int,4)
            }; 
            parameters[0].Value = TableName;
            parameters[1].Value = FieldKey;
            parameters[2].Value = PageCurrent;
            parameters[3].Value = PageSize;
            parameters[4].Value = FieldShow;
            parameters[5].Value = FieldOrder;
            parameters[6].Value = " 1=1 "+strWhere;
            parameters[7].Direction = ParameterDirection.Output;
            parameters[8].Direction = ParameterDirection.Output;

            return DBExecute.ExecuteQuery(CommandType.StoredProcedure, "PageList", out RecordCount, out PageCount, parameters);
        }

    }
}
