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
    public class AUserTraceRule
    {
        public static void AddAUserTraceData(string UserCode,string ActionCode, string KeyNo, string ModuleName, string IP)
        {
            AUserTraceData dsAUserTraceData = new AUserTraceData();
            AUserTraceData.AUserTraceRow row = dsAUserTraceData.AUserTrace.NewAUserTraceRow();
            row.UserCode = UserCode;
            row.ActionCode = ActionCode;
            row.KeyNo = KeyNo;
            row.ModuleName = ModuleName;
            row.IP = IP;
            dsAUserTraceData.AUserTrace.AddAUserTraceRow(row);
            DBExecute.UpdateData(dsAUserTraceData.AUserTrace);
        }

        public static void UpdateData(AUserTraceData dsAUserTraceData)
        {
            DBExecute.UpdateData(dsAUserTraceData.AUserTrace);
        }
    }
}
