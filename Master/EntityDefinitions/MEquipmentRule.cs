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
    public class MEquipmentRule
    {
        public static DataTable GetEquipmentStatusList(string UserCode,string EquipmentType,string EquipmentCode,string EquipmentName
            ,string ProductCode,string Province,string City,string County,string Village,string State,
            string ResiAreaDirector,string StreetNumber,string Building,string Unit,string LadderNo)
        {
            DataTable dt = new DataTable();
            string strSQL = "IF OBJECT_ID('tempdb..#taMEquipment') IS NOT NULL  DROP TABLE #taMEquipment "+
                            "select * into #taMEquipment from dbo.MEquipment where EquipmentCode in ("+
                            "select CustEquipmentCode from dbo.ACustomerEquipment where CustomerCode='"+LoginInfo.CustomerCode+"')"+
                            ((LoginInfo.UserCode.ToLower()=="admin")?"":" and EquipmentCode in (select UserEquipmentCode from dbo.AUserEquipment where UserCode='"+LoginInfo.UserCode+"')")+
                            " IF OBJECT_ID('tempdb..#ta') IS NOT NULL  DROP TABLE #ta " +
                            "  select b.EquipmentInterfaceID, b.FaultCode, b.FaultName, b.MotionStatus, b.FloorCode, b.FloorMun, b.DoorStatus, a.EquipmentType, a.EquipmentCode, a.EquipmentName,a.ProductCode,case when ISNULL(b.FaultCode,'')=''  or ISNULL(b.FaultCode,'')='0' then case when datediff(ss,ISNULL(b.CreateOn,DATEADD(DAY,-1,getdate())),getdate())>" + System.Configuration.ConfigurationSettings.AppSettings["FaultTime"] + " " +
                            " then '离线' else '正常' end else b.FaultName end as EquipmentStatus into #ta from dbo.MEquipment a " +
                            " left outer join ( "+
                            " select * from dbo.BEquipmentStatus where EquipmentCode+'~'+CONVERT(varchar(100),CreateOn, 13) in (  select EquipmentCode+'~'+CONVERT(varchar(100), "+
                            " MAX(CreateOn), 13) from dbo.BEquipmentStatus group by EquipmentCode) "+ 
                            " ) b on b.EquipmentCode=a.EquipmentCode and b.EquipmentType=a.EquipmentType "+
                            " where a.EquipmentType='" + EquipmentType + "'  and a.EquipmentCode in (select EquipmentCode from #taMEquipment)" +
                            ((string.IsNullOrEmpty(EquipmentCode)) ? "" : " and a.EquipmentCode like '%" + EquipmentCode + "%' ") +
                            ((string.IsNullOrEmpty(EquipmentName)) ? "" : " and a.EquipmentName like '%" + EquipmentName + "%' ") +
                            ((string.IsNullOrEmpty(ProductCode)) ? "" : " and a.ProductCode like '%" + ProductCode + "%' ") +
                            ((string.IsNullOrEmpty(Province)) ? "" : " and a.Province='" + Province + "' ") +
                            ((string.IsNullOrEmpty(City)) ? "" : " and a.City='" + City + "' ") +
                            ((string.IsNullOrEmpty(County)) ? "" : " and a.County='" + County + "' ") +
                            ((string.IsNullOrEmpty(Village)) ? "" : " and a.Village='" + Village + "' ") +
                            ((string.IsNullOrEmpty(State)) ? "" : " and a.State='" + State + "' ") +
                            ((string.IsNullOrEmpty(ResiAreaDirector)) ? "" : " and a.ResidentialArea='" + ResiAreaDirector + "' ") +
                            ((string.IsNullOrEmpty(StreetNumber)) ? "" : " and a.StreetNumber='" + StreetNumber + "' ") +
                            ((string.IsNullOrEmpty(Building)) ? "" : " and a.Building='" + Building + "' ") +
                            ((string.IsNullOrEmpty(Unit)) ? "" : " and a.Unit='" + Unit + "' ") +
                            ((string.IsNullOrEmpty(LadderNo)) ? "" : " anda. LadderNo='" + LadderNo + "' ") +
                            ((string.IsNullOrEmpty(UserCode) || UserCode.ToLower() == "admin") ? "" : " and a.EquipmentCode in (select CustEquipmentCode from dbo.ACustomerEquipment where CustomerCode in (select CustomerCode from dbo.ACustomerUser where UserCode='" + UserCode + "'))");


            strSQL += "select (select COUNT(*) from #ta) as TotalCount " +
                    " ,(select COUNT(*) from #ta where EquipmentStatus='离线') as OffLineCount " +
                    " ,(select COUNT(*) from #ta where EquipmentStatus!='离线' and EquipmentStatus!='正常') as FaultCount " +
                    " ,(select COUNT(*) from #ta where EquipmentStatus='正常') as NormalCount " +
                    " ,* from #ta  order by EquipmentCode ";
            SqlCommand cmd = new SqlCommand();
            DBExecute.GetData(cmd, strSQL, dt);
            return dt;
        }

        public static DataTable GetEquipmentStatusListByProcedure(string UserCode,string EquipmentType,string EquipmentCode,string EquipmentName
            ,string ProductCode,string Province,string City,string County,string Village,string State,
            string ResiAreaDirector,string StreetNumber,string Building,string Unit,string LadderNo,string CurrPage)
        {
            DataTable dt = new DataTable();
            string strSQL = " GetEquipmentList '" + UserCode + "','"+EquipmentType+"','"+EquipmentCode+"','"+EquipmentName+"'"+
            ",'"+ProductCode+"','"+Province+"','"+City+"','"+County+"','"+Village+"','"+State+"','"+ResiAreaDirector+"'"+
            ",'"+StreetNumber+"','"+Building+"','"+Unit+"','"+LadderNo+"','"+LoginInfo.CustomerCode+"'"+
            ",'" + System.Configuration.ConfigurationSettings.AppSettings["FaultTime"] + "','8','" + CurrPage + "'";

            SqlCommand cmd = new SqlCommand();
            DBExecute.GetData(cmd, strSQL, dt);
            return dt;
        }

        public static MEquipmentData GetEquipmentData()
        {
            MEquipmentData ds = new MEquipmentData();
            string strSQL = "select * from MEquipment order by EquipmentName ";
            SqlCommand cmd = new SqlCommand();
            DBExecute.GetData(cmd, strSQL, ds.MEquipment);
            return ds;
        }

        public static DataTable GetEquipmentDataByEquipmentCode(string EquipmentCode)
        {
            DataTable dt = new DataTable();
            string strSQL = "select * from MEquipment where EquipmentCode=@EquipmentCode";
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@EquipmentCode", EquipmentCode);

            DBExecute.GetData(cmd, strSQL, dt);
            return dt;
        }

        public static MEquipmentData GetEquipmentDataByCode(string EquipmentCode)
        {
            MEquipmentData ds = new MEquipmentData();
            string strSQL = "select * from MEquipment where EquipmentCode = @EquipmentCode ";
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@EquipmentCode", EquipmentCode);

            DBExecute.GetData(cmd, strSQL, ds.MEquipment);
            return ds;
        }

        public static MEquipmentData GetEquipmentDataByProductCode(string ProductCode)
        {
            MEquipmentData ds = new MEquipmentData();
            string strSQL = "select * from MEquipment where ProductCode = @ProductCode ";
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@ProductCode", ProductCode);

            DBExecute.GetData(cmd, strSQL, ds.MEquipment);
            return ds;
        }

        public static void UpdateData(MEquipmentData dsMEquipmentData)
        {
            DBExecute.UpdateData(dsMEquipmentData.MEquipment);
        }

        public static void Delete(string EquipmentCode)
        {
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql += "Delete FROM MEquipment where EquipmentCode in (@EquipmentCode);";
            cmd.Parameters.AddWithValue("@EquipmentCode", EquipmentCode);
            DBExecute.ExecuteNonQuery(cmd, sql);
        }


        public static DataTable GetProvince(string EquipmentType)
        {
            DataTable dt = new DataTable();
            string strSQL = "select Province from dbo.MEquipment where Province!='' and EquipmentType='" + EquipmentType + "' group by Province order by Province ";
            SqlCommand cmd = new SqlCommand();
            DBExecute.GetData(cmd, strSQL, dt);
            return dt;
        }

        public static DataTable GetCity(string EquipmentType)
        {
            DataTable dt = new DataTable();
            string strSQL = "select City from dbo.MEquipment where City!='' and EquipmentType='" + EquipmentType + "' group by City order by City ";
            SqlCommand cmd = new SqlCommand();
            DBExecute.GetData(cmd, strSQL, dt);
            return dt;
        }

        public static DataTable GetCityByProvince(string Province, string EquipmentType)
        {
            DataTable dt = new DataTable();
            string strSQL = "select City from dbo.MEquipment where City!='' and Province='" + Province + "' and EquipmentType='" + EquipmentType + "' group by City order by City ";
            SqlCommand cmd = new SqlCommand();
            DBExecute.GetData(cmd, strSQL, dt);
            return dt;
        }

        public static DataTable GetCounty(string EquipmentType)
        {
            DataTable dt = new DataTable();
            string strSQL = "select County from dbo.MEquipment where County!='' and EquipmentType='" + EquipmentType + "' group by County order by County ";
            SqlCommand cmd = new SqlCommand();
            DBExecute.GetData(cmd, strSQL, dt);
            return dt;
        }

        public static DataTable GetCountyByCity(string City, string EquipmentType)
        {
            DataTable dt = new DataTable();
            string strSQL = "select County from dbo.MEquipment where County!=''  and City='" + City + "' and EquipmentType='" + EquipmentType + "' group by County order by County ";
            SqlCommand cmd = new SqlCommand();
            DBExecute.GetData(cmd, strSQL, dt);
            return dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="City"></param>
        /// <returns></returns>
        public static DataTable GetVillageByCounty(string County, string EquipmentType)
        {
            DataTable dt = new DataTable();
            string strSQL = "select Village from dbo.MEquipment where Village!=''  and County='" + County + "' and EquipmentType='" + EquipmentType + "' group by Village order by Village ";
            SqlCommand cmd = new SqlCommand();
            DBExecute.GetData(cmd, strSQL, dt);
            return dt;
        }
        public static DataTable GetStateByVillage(string Village, string EquipmentType)
        {
            DataTable dt = new DataTable();
            string strSQL = "select State from dbo.MEquipment where State!=''  and Village='" + Village + "' and EquipmentType='" + EquipmentType + "' and EquipmentType='" + EquipmentType + "' group by State order by State ";
            SqlCommand cmd = new SqlCommand();
            DBExecute.GetData(cmd, strSQL, dt);
            return dt;
        }

        public static DataTable GetResidentialAreaByState(string State, string EquipmentType)
        {
            DataTable dt = new DataTable();
            string strSQL = "select ResidentialArea from dbo.MEquipment where ResidentialArea!=''  and State='" + State + "' and EquipmentType='" + EquipmentType + "' group by ResidentialArea order by ResidentialArea ";
            SqlCommand cmd = new SqlCommand();
            DBExecute.GetData(cmd, strSQL, dt);
            return dt;
        }

        public static DataTable GetStreetNumberByResidentialArea(string ResidentialArea, string EquipmentType)
        {
            DataTable dt = new DataTable();
            string strSQL = "select StreetNumber from dbo.MEquipment where StreetNumber!=''  and ResidentialArea='" + ResidentialArea + "' and EquipmentType='" + EquipmentType + "' group by StreetNumber order by StreetNumber ";
            SqlCommand cmd = new SqlCommand();
            DBExecute.GetData(cmd, strSQL, dt);
            return dt;
        }
    }
}
