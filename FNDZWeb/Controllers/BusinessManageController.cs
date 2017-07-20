using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Configuration;
using System.Configuration;
using System.IO;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using Master.EntityDAO;
using Master.EntityDefinitions;
using Admin.EntityDefinitions;
using Business.EntityDefinitions;
using Common;

namespace FNDZWeb.Controllers
{
    public class BusinessManageController : Controller
    {
        public ActionResult test(string EquipmentType)
        {
            ViewBag.EquipmentType = (EquipmentType == "null" || string.IsNullOrEmpty(EquipmentType)) ? "" : EquipmentType;
            return View();
        }

        public ActionResult BEquipmentElevator(string EquipmentType)
        {
            ViewBag.EquipmentType = (EquipmentType == "null" || string.IsNullOrEmpty(EquipmentType)) ? "" : EquipmentType;
            return View();
        }

       

        public ActionResult JSonEquipmentStatus(string EquipmentType,string width, string height, string left, string top
            , string Province, string City, string County, string Village, string State, string ResidentialArea, string StreetNumber
            , string EquipmentCode, string EquipmentName, string ProductCode, string CurrPage)
        {
            result.Clear();
            return Json(GetEquipmentStatus(EquipmentType,width, height, left, top
                , (Province=="" || Province=="--请选择--")?"":Province
                , (City == "" || City == "--请选择--") ? "" : City
                , (County == "" || County == "--请选择--") ? "" : County
                , (Village == "" || Village == "--请选择--") ? "" : Village
                , (State == "" || State == "--请选择--") ? "" : State
                , (ResidentialArea == "" || ResidentialArea == "--请选择--") ? "" : ResidentialArea
                , (StreetNumber == "" || StreetNumber == "--请选择--") ? "" : StreetNumber
            , EquipmentCode, EquipmentName, ProductCode, CurrPage),
                JsonRequestBehavior.AllowGet);
        }

        StringBuilder result = new StringBuilder();
        StringBuilder sb = new StringBuilder();
        public string GetEquipmentStatus(string EquipmentType,string width, string height, string left, string top
            , string Province, string City, string County, string Village, string State, string ResidentialArea, string StreetNumber
            , string EquipmentCode, string EquipmentName, string ProductCode, string CurrPage)
        {
            int irow = 1;
            double iwidth = 130;
            double iheight = 200;
            double iMargin = 5;
            double iHMargin = 5;
            double iTwidth = double.Parse(width);
            double iTheight = double.Parse(height);

            double irowwidth = 0;
            double ilowwidth = 0;

            double dleft = double.Parse(left);
            double dtop = double.Parse(top);
            double x = 0;
            double y = 0;

            DataTable dtEquipment = MEquipmentRule.GetEquipmentStatusListByProcedure(LoginInfo.UserCode, EquipmentType
                , EquipmentCode, EquipmentName, ProductCode, Province, City, County, Village, State, ResidentialArea, StreetNumber, "", "", "", CurrPage);
            for (int i = 0; i < dtEquipment.Rows.Count; i++)
            {
                DataRow row = dtEquipment.Rows[i];
                if (iTwidth >= (irowwidth + iMargin + iwidth))
                {
                    x = irowwidth + iMargin;
                    y = ilowwidth + iHMargin;
                }
                else
                {
                    irowwidth = 0;
                    ilowwidth += (iHMargin + iheight);
                    x = irowwidth + iMargin;
                    y = ilowwidth + iHMargin;
                    irow++;
                }

                x += dleft;
                y += dtop;
                string sImg = GetImg(EquipmentType, row["EquipmentStatus"].ToString(), row["MotionStatus"].ToString(), row["DoorStatus"].ToString()); 

                
                result.Append("d" + i.ToString()+"^"+
                    sImg + "^"+
                    row["EquipmentStatus"] + "^" +
                    row["EquipmentName"] + "^" +
                    row["ProductCode"] + "^" +
                    row["MotionStatus"] + "^" +//运行状态
                    row["FloorMun"] + "^" +//当前楼层数
                    row["EquipmentInterfaceID"] + "^" +
                    row["DoorStatus"] + "^" +//当前开关门
                    row["TotalCount"] + "^" +
                    row["OffLineCount"] + "^" +
                    row["FaultCount"] + "^" +
                    row["NormalCount"] + "^" +
                    row["TotalPage"]  +"^" +
                    row["EquipmentCode"] + "^" +
                    ((row["ResidentialArea"] is DBNull) ? "" : row["ResidentialArea"]) + ((row["StreetNumber"] is DBNull) ? "" : row["StreetNumber"]) + ((row["Unit"] is DBNull) ? "" : row["Unit"]) + ((row["LadderNo"] is DBNull) ? "" : row["LadderNo"]) + ((i == dtEquipment.Rows.Count - 1) ? "" : "<div>"));
                irowwidth += (iwidth + iMargin);
            }

            result.Append("height~" + (dtop + (irow * (iheight + iHMargin))).ToString());

            return result.ToString();
        }

        private string GetImg(string EquipmentType, string EquipmentStatus, string MotionStatus, string DoorStatus)
        {
            //"上行", "下行", "呼救", "救援", "前门开门", "前门关门", "后门开门", "后门关门", "上平层", "下平层", "上极限", "下极限", "人感开关", "维保信号", "备用", "备用"
            //"上行", "下行", "快速", "慢速", "节能待机", "检修", "备用", "备用", "备用", "备用", "备用", "备用", "备用", "备用", "备用", "备用"
            string rImg = "";
            switch (EquipmentType)
            {
                case "1"://电梯
                    #region
                    if (EquipmentStatus == "离线")
                        rImg = "imgElNoSignal";
                    else
                    {
                        if (MotionStatus.Contains("维保"))
                            rImg = "imgElMaintenance";
                        else if (MotionStatus.Contains("上行") && EquipmentStatus == "正常")
                            rImg = "imgElUp";
                        else if (MotionStatus.Contains("上行") && EquipmentStatus != "正常")
                            rImg = "imgElUpFault";
                        else if (MotionStatus.Contains("下行") && EquipmentStatus == "正常")
                            rImg = "imgElDown";
                        else if (MotionStatus.Contains("下行") && EquipmentStatus != "正常")
                            rImg = "imgElDownFault";
                        else if (DoorStatus.Contains("开门") && EquipmentStatus == "正常")
                            rImg = "imgElOpen";
                        else if (DoorStatus.Contains("开门") && EquipmentStatus != "正常")
                            rImg = "imgElOpenFault";
                        else if (DoorStatus.Contains("关门") && EquipmentStatus == "正常")
                            rImg = "imgElClose";
                        else if (DoorStatus.Contains("关门") && EquipmentStatus != "正常")
                            rImg = "imgElCloseFault";
                        else rImg = "imgElCloseFault";
                    }
                    #endregion
                    break;
                default:
                    if (EquipmentStatus == "离线")//无信号
                        rImg = (EquipmentType == "2") ? "imgEsNoSignal" : "imgSiNoSignal";
                    else
                    {
                        
                        if (MotionStatus.Contains("快速") && MotionStatus.Contains("上行") && EquipmentStatus == "正常")//快速上行
                            rImg = (EquipmentType == "2") ? "imgEsFastUp" : "imgSiFastUp";
                        else if (MotionStatus.Contains("快速") && MotionStatus.Contains("下行") && EquipmentStatus == "正常")//快速下行
                            rImg = (EquipmentType == "2") ? "imgEsFastDown" : "imgSiFastDown";
                        else if (MotionStatus.Contains("慢速") && MotionStatus.Contains("上行") && EquipmentStatus == "正常")//慢速上行
                            rImg = (EquipmentType == "2") ? "imgEsSlowUp" : "imgSiSlowUp";
                        else if (MotionStatus.Contains("慢速") && MotionStatus.Contains("下行") && EquipmentStatus == "正常")//慢速下行
                            rImg = (EquipmentType == "2") ? "imgEsSlowDown" : "imgSiSlowDown";
                        else if (MotionStatus.Contains("上行") && EquipmentStatus == "正常")//上行
                            rImg = (EquipmentType == "2") ? "imgEsUp" : "imgSiUp";
                        else if (MotionStatus.Contains("下行") && EquipmentStatus == "正常")//下行
                            rImg = (EquipmentType == "2") ? "imgEsDown" : "imgSiDown";
                        else if (MotionStatus.Contains("待机") && EquipmentStatus == "正常")//待机
                            rImg = (EquipmentType == "2") ? "imgEsStandby" : "imgSiStandby";                        
                        else if (MotionStatus.Contains("快速") && MotionStatus.Contains("上行") && EquipmentStatus != "正常")//快速上行带故障
                            rImg = (EquipmentType == "2") ? "imgEsFastUpPolice" : "imgSiFastUpPolice";
                        else if (MotionStatus.Contains("快速") && MotionStatus.Contains("下行") && EquipmentStatus != "正常")//快速下行带故障
                            rImg = (EquipmentType == "2") ? "imgEsFastDownPolice" : "imgSiFastDownPolice";
                        else if (MotionStatus.Contains("慢速") && MotionStatus.Contains("上行") && EquipmentStatus != "正常")//慢速上行带故障
                            rImg = (EquipmentType == "2") ? "imgEsSlowUpPolice" : "imgSiSlowUpPolice";
                        else if (MotionStatus.Contains("慢速") && MotionStatus.Contains("下行") && EquipmentStatus != "正常")//慢速下行带故障
                            rImg = (EquipmentType == "2") ? "imgEsSlowDownPolice" : "imgSiSlowDownPolice";
                        else if (MotionStatus.Contains("上行") && EquipmentStatus != "正常")//上行带故障
                            rImg = (EquipmentType == "2") ? "imgEsUpPolice" : "imgSiUpPolice";
                        else if (MotionStatus.Contains("下行") && EquipmentStatus != "正常")//下行带故障
                            rImg = (EquipmentType == "2") ? "imgEsDownPolice" : "imgSiDownPolice";
                        else if (MotionStatus.Contains("待机") && EquipmentStatus != "正常")//待机带故障
                            rImg = (EquipmentType == "2") ? "imgEsStandbyPolice" : "imgSiStandbyPolice";
                        else if (MotionStatus.Contains("检修"))//维保
                            rImg = (EquipmentType == "2") ? "imgEsMaintenance" : "imgSiMaintenance";
                        else rImg = (EquipmentType == "2") ? "imgEsStandby" : "imgSiStandbyPolice";
                        //else rImg = (EquipmentType == "2") ? "imgEsStandbyPolice" : "imgSiStandbyPolice";
                    }
                    break;
            }
            return rImg;
        }



        public ContentResult BEquipmentInterfaceList11(string EquipmentInterfaceID)
        {
            string sInfo = "";
            DataTable dt = BEquipmentInterfaceRule.GetBEquipmentInterfaceByCode(EquipmentInterfaceID);
            foreach (DataColumn col in dt.Columns)
                sInfo += col.ColumnName + ":" + ((dt.Rows[0][col.ColumnName] is DBNull) ? "" : dt.Rows[0][col.ColumnName].ToString())+",";
            sInfo += "EquipmentType~" + dt.Rows[0]["EquipmentType"].ToString();
            return Content(sInfo);
        }

        private string GetColName(string ColumnName)
        {
            string RunName = "";
            switch (ColumnName)
            {
                case "A01":
                    RunName = "当前楼层";
                    break;
                case "A02":
                    RunName="";
                    break;
                case "A03":
                    RunName = "";
                    break;
                case "A031":
                    RunName = "状态消防";
                    break;
                case "A032":
                    RunName = "状态自动或检修";
                    break;
                case "A033":
                    RunName = "状态超载";
                    break;
                case "A034":
                    RunName = "状态满载";
                    break;
                case "A035":
                    RunName = "状态备用";
                    break;
                case "A036":
                    RunName = "状态备用";
                    break;
                case "A037":
                    RunName = "状态备用";
                    break;
                case "A038":
                    RunName = "状态备用";
                    break;
                case "A039":
                    RunName = "状态备用";
                    break;
                case "A0310":
                    RunName = "状态备用";
                    break;
                case "A0311":
                    RunName = "状态备用";
                    break;
                case "A0312":
                    RunName = "状态备用";
                    break;
                case "A0313":
                    RunName = "状态备用";
                    break;
                case "A0314":
                    RunName = "状态备用";
                    break;
                case "A0315":
                    RunName = "状态备用";
                    break;
                case "A0316":
                    RunName = "状态备用";
                    break;

                    break;
                    case "A04":
                    RunName = "备用";
                    break;
                    case "A05":
                    RunName = "备用";
                    break;
                    case "A06":
                    RunName = "故障代码";
                    break;
                    case "A07":
                    RunName = "运行次数高位";
                    break;
                    case "A08":
                    RunName = "运行次数低位";
                    break;
                    case "A09":
                    RunName = "运行时间高位";
                    break;
                    case "A10":
                    RunName = "运行时间低位";
                    break;
                    case "A11":
                    RunName = "门机运行次数高位";
                    break;
                    case "A12":
                    RunName = "门机运行次数低位";
                    break;
                    case "A13":
                    RunName = "制动器运行次数高位";
                    break;
                    case "A14":
                    RunName = "制动器运行次数低位";
                    break;
                    case "A15":
                    RunName = "主副接触器运行次数高位";
                    break;
                    case "A16":
                    RunName = "主副接触器运行次数低位";
                    break;
                    case "A17":
                    RunName = "钢丝绳折弯次数高位";
                    break;
                    case "A18":
                    RunName = "钢丝绳折弯次数低位";
                    break;
                    case "A19":
                    RunName = "电梯运行总距离高位";
                    break;
                    case "A20":
                    RunName = "电梯运行总距离低位";
                    break;


                    case "A021":
                    RunName = "状态上行";
                    break;
                    case "A022":
                    RunName = "状态下行";
                    break;
                    case "A023":
                    RunName = "状态呼救";
                    break;
                    case "A024":
                    RunName = "状态救援";
                    break;
                    case "A025":
                    RunName = "状态前门开门";
                    break;
                    case "A026":
                    RunName = "状态前门关门";
                    break;
                    case "A027":
                    RunName = "状态后门开门";
                    break;
                    case "A028":
                    RunName = "状态后门关门";
                    break;
                    case "A029":
                    RunName = "状态上平层";
                    break;
                    case "A0210":
                    RunName = "状态下平层";
                    break;
                    case "A0211":
                    RunName = "状态上极限";
                    break;
                    case "A0212":
                    RunName = "状态下极限";
                    break;
                    case "A0213":
                    RunName = "状态人感开关";
                    break;
                    case "A0214":
                    RunName = "状态维保信号";
                    break;
                    case "A0215":
                    RunName = "状态备用";
                    break;
                    case "A0216":
                    RunName = "状态备用";
                    break;

                default:
                    if(int.Parse(ColumnName.Substring(1,ColumnName.Length-1))>=21)
                    {
                        RunName = "电梯" + Convert.ToString(int.Parse(ColumnName.Substring(1, ColumnName.Length - 1))-20) + "层停层次数";
                    }
                    break;
            }
            return RunName;

        }

        private string GetColNameNotElevator(string ColumnName)
        {
            string RunName = "";
            switch (ColumnName)
            {
                case "A01":
                    RunName = "故障代码";
                    break;
                case "A02":
                    RunName = "";
                    break;
                case "A03":
                    RunName = "备用";
                    break;
                case "A04":
                    RunName = "备用";
                    break;
                case "A05":
                    RunName = "备用";
                    break;
                case "A06":
                    RunName = "抱闸动作次数";
                    break;
                case "A07":
                    RunName = "上行次数";
                    break;
                case "A08":
                    RunName = "下行次数";
                    break;
                case "A09":
                    RunName = "运行时间高位-单位小时";
                    break;
                case "A10":
                    RunName = "运行时间低位-单位小时";
                    break;           

                case "A021":
                    RunName = "状态上行";
                    break;
                case "A022":
                    RunName = "状态下行";
                    break;
                case "A023":
                    RunName = "状态快速";
                    break;
                case "A024":
                    RunName = "状态慢速";
                    break;
                case "A025":
                    RunName = "节能待机";
                    break;
                case "A026":
                    RunName = "检修";
                    break;
            }
            return RunName;

        }

        #region 报文详细信息
        public ContentResult GetEquipmentMessage(string EquipmentCode, string EquipmentType)
        {
            string sInfo = "";
            DataTable dtEquipment = MEquipmentRule.GetEquipmentStatusListByProcedure(LoginInfo.UserCode, EquipmentType
                , EquipmentCode, "", "", "", "", "", "", "", "", "", "", "", "", "1");
            DataRow row = dtEquipment.Rows[0];
            string sImg = GetImg(EquipmentType, row["EquipmentStatus"].ToString(), row["MotionStatus"].ToString(), row["DoorStatus"].ToString());


            sInfo+="d0^" +
                sImg + "^" +
                row["EquipmentStatus"] + "^" +
                row["EquipmentName"] + "^" +
                row["ProductCode"] + "^" +
                row["MotionStatus"] + "^" +//运行状态
                row["FloorMun"] + "^" +//当前楼层数
                row["EquipmentInterfaceID"] + "^" +
                row["DoorStatus"] + "^" +//当前开关门
                row["TotalCount"] + "^" +
                row["OffLineCount"] + "^" +
                row["FaultCount"] + "^" +
                row["NormalCount"] + "^" +
                row["TotalPage"] + "^" +
                row["EquipmentCode"] + "^" +
                ((row["ResidentialArea"] is DBNull) ? "" : row["ResidentialArea"]) + ((row["StreetNumber"] is DBNull) ? "" 
                : row["StreetNumber"]) + ((row["Unit"] is DBNull) ? "" : row["Unit"]) + ((row["LadderNo"] is DBNull) ? "" 
                : row["LadderNo"]);
            return Content(sInfo);
        }

        public ActionResult BEquipmentInterfaceList(int page, int rows, string order, string sort)
        {
            ContentResult result = new ContentResult();
            DataTable dt = BEquipmentInterfaceRule.GetBEquipmentIntByEquipmentCode(Request["EquipmentCode"]);
            DataTable dtInfo = new DataTable();
            dtInfo.Columns.Add("ColumnOrder", typeof(string));
            dtInfo.Columns.Add("ColumnNameOld", typeof(string));
            dtInfo.Columns.Add("ColumnName", typeof(string));
            dtInfo.Columns.Add("ColumnValue", typeof(string));
            int i = 1;
            if (dt.Rows.Count > 0)
            {
                foreach (DataColumn col in dt.Columns)
                {
                    if (col.ColumnName == "ID" || col.ColumnName == "RcvID" || col.ColumnName == "EquipmentType" || col.ColumnName == "EquipmentCode" || col.ColumnName == "CreateBy"
                        || col.ColumnName == "CreateOn" || col.ColumnName == "A02") continue;
                    DataRow row = dtInfo.NewRow();
                    row["ColumnOrder"] = Convert.ToString(i).PadLeft(2, '0');
                    row["ColumnNameOld"] = col.ColumnName;
                    row["ColumnName"] = (dt.Rows[0]["EquipmentType"].ToString() == "1") ? GetColName(col.ColumnName) : GetColNameNotElevator(col.ColumnName);
                    row["ColumnValue"] = ((dt.Rows[0][col.ColumnName] is DBNull) ? "" : dt.Rows[0][col.ColumnName].ToString());

                    if ((dt.Rows[0]["EquipmentType"].ToString() == "1" && col.ColumnName == "A06")
                        || (dt.Rows[0]["EquipmentType"].ToString() != "1" && col.ColumnName == "A01"))
                    {
                        DataTable dtFault = ACustomerEquipmentFaultRule.GetACustomerEquipmentFaultListByCode(LoginInfo.CustomerCode, dt.Rows[0]["EquipmentCode"].ToString());
                        DataRow[] rowFault = dtFault.Select("CustFaultID='" + dt.Rows[0][col.ColumnName].ToString() + "'");
                        if (rowFault.Length > 0) row["ColumnValue"] = row["ColumnValue"] + "(" + rowFault[0]["FaultName"] + ")";
                    }

                    if (col.ColumnName == "A01" && dt.Rows[0]["EquipmentType"].ToString() == "1")
                    {
                        DataTable dtFloor = ACustomerEquipmentFloorRule.GetACustomerEquipmentFloorIDByCode(LoginInfo.CustomerCode, dt.Rows[0]["EquipmentCode"].ToString());
                        DataRow[] rowFloor = dtFloor.Select("CustFloorID='" + dt.Rows[0][col.ColumnName].ToString() + "'");
                        if (rowFloor.Length > 0)
                        {
                            MFloorData dtMFloorData = MFloorRule.GetFloorDataByCode(rowFloor[0]["CustFloorCode"].ToString());
                            if (dtMFloorData.MFloor.Count>0)
                                row["ColumnValue"] = row["ColumnValue"] + "(" + dtMFloorData.MFloor[0].FloorMun + ")";
                        }
                    }
                        //+ ((col.ColumnName != "A02")?"":((row["FaultName"] is DBNull) ? "" : "("+row["FaultName"].ToString()+")"));
                    if (!string.IsNullOrEmpty(row["ColumnName"].ToString()))
                    dtInfo.Rows.Add(row);
                    i++;
                }
            }
            DataTable dtCopy = dtInfo.Copy();
            DataView dv = dtInfo.DefaultView;
            dv.Sort = "ColumnNameOld";
            dtCopy = dv.ToTable();
            result.Content = "{\"total\":\"" + dtCopy.Rows.Count + "\",\"rows\":" + ConvertJson.ToJson(dtCopy) + "}";
            return result;
        }

        #endregion
    }
}
