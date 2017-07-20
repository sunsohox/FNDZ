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
    public class ACustomerUserRule
    {
        public static DataTable GetACustomerUserListData(string CustomerCode, string UserCode, string UserName)
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql = "  select a.*,case when ISNULL(b.UserCode,'')='' then 'N' else 'Y' end as IsCustomerUser  from dbo.AUser a "+
                  "  left outer join dbo.ACustomerUser b on b.UserCode=a.UserCode and b.CustomerCode='"+CustomerCode+"' "+
                  "  where a.Active=0 "+((string.IsNullOrEmpty(UserCode))?"":" and a.UserCode like '%"+UserCode+"%'" )
            +((string.IsNullOrEmpty(UserName))?"":" and a.UserName like '%"+UserName+"%'" ) +" order by a.UserName";
            DataTable dt = new DataTable();
            DBExecute.GetData(cmd, sql, dt);
            return dt;
        }

        public static ACustomerUserData GetACustomerUserByCode(string CustomerCode, string UserCode)
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql = "select * from dbo.ACustomerUser where UserCustomerCode='" + CustomerCode + "' and UserCode='" + UserCode + "'";
            ACustomerUserData dsACustomerUserData = new ACustomerUserData();
            DBExecute.GetData(cmd, sql, dsACustomerUserData.ACustomerUser);
            return dsACustomerUserData;
        }

        public static bool InsertACustomerUser(string CustomerCode, string UserCode, string Active, string IsDefault)
        {
            string sql = "insert into dbo.ACustomerUser(UserCustomerCode,UserCode,Active,IsDefault) values('" + CustomerCode + "','" + UserCode + "','" + Active + "','" + IsDefault + "')";
            int i = DBExecute.ExecuteSQL(sql);
            if (i > 0) return true; else return false;
        }

        public static bool DeleACustomerUser(string CustomerCode, string UserCode)
        {
            string sql = "delete from dbo.ACustomerUser where UserCustomerCode='" + CustomerCode + "' and UserCode='" + UserCode + "';"+
                " delete from dbo.AUserEquipment where UserCode='" + UserCode + "' and UserEquipmentCode in (select CustEquipmentCode from dbo.ACustomerEquipment where CustomerCode='" + CustomerCode + "')";
            int i = DBExecute.ExecuteSQL(sql);
            if (i > 0) return true; else return false;
        }

        public static void UpdateData(ACustomerUserData dsACustomerUserData)
        {
            DBExecute.UpdateData(dsACustomerUserData.ACustomerUser);
        }

        public static DataTable GetACustomerUserByCode(string UserCode)
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql = "select * " + ((UserCode.ToLower() == "admin") ? ",'0' as IsDefault" : ",(select IsDefault from dbo.ACustomerUser where UserCustomerCode=ACustomer.CustomerCode and UserCode='" + UserCode + "')  as IsDefault") + " from dbo.ACustomer where 1=1  " + ((UserCode.ToLower() == "admin") ? "" : " and CustomerCode in (select UserCustomerCode from dbo.ACustomerUser where UserCode='" + UserCode + "' and Active='1')") + " order by CustomerName"; 
            DataTable dt = new DataTable();
            DBExecute.GetData(cmd, sql, dt);
            return dt;
        }


        public static DataTable GetUserCustomerByUserCode(string UserCode)
        {
            SqlCommand cmd = new SqlCommand();
            DataTable dt = new DataTable();
            string sql = "select a.*,b.CustomerName, case a.Active when 1 then '是'  else '否' end as activeShow,case a.IsDefault when 1  then '是'  else '否' end as isDefaultShow  " + 
                        " from dbo.ACustomerUser a "+
                        " left outer join dbo.ACustomer b on b.CustomerCode=a.UserCustomerCode where a.UserCode='" + UserCode + "' order by a.UserCustomerCode";
            DBExecute.GetData(cmd, sql, dt);
            return dt;
        }
    }
}
