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
using Common;

namespace FNDZWeb.Controllers
{
    [Authorize]
    public class MasterManageController : Controller
    {
        public ContentResult GetSequence()
        {
            string SequenceCode = Request["SequenceCode"];
            string BhCode = Request["BhCode"];

            return Content(CommonBase.GetSequence(BhCode + DateTime.Now.ToString("yyyyMMdd"), SequenceCode, ""));
        }

        #region 公司管理
        public ActionResult MCompany()
        {
            return View();
        }
        public ActionResult MCompanyList(int page, int rows, string order, string sort)
        {
            ContentResult result = new ContentResult();
            int RecordCount, PageCount;
            DataSet ds = PageList.GetListbyPage("MCompany", "CompanyCode", page, rows,
                "*", "CompanyCode", " and CompanyCode like '%" + Request["CompanyCode"] + "%'", out RecordCount, out  PageCount);
            result.Content = "{\"total\":\"" + RecordCount + "\",\"rows\":" + ConvertJson.ToJson(ds.Tables[0]) + "}";
            return result;
        }
        public ActionResult MCompanyDelete(string idList)
        {
            try
            {
                MCompanyRule.Delete(idList);
                AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "删除公司", idList, "公司管理", Request.UserHostAddress);
                return Json(new { success = true, Message = "删除成功！" });
            }
            catch
            {
                return Json(new { success = true, Message = "删除失败！" });
            }
        }
        public ContentResult MCompanyUpdate()
        {
            MCompanyData dsData = MCompanyRule.GetCompanyDataByCode(Request["CompanyCode"]);
            if (dsData != null && dsData.MCompany.Rows.Count > 0)
            {
                dsData.MCompany[0].CompanyName = Request["CompanyName"];
                dsData.MCompany[0].EmailAddr = (string.IsNullOrEmpty(Request["EmailAddr"])) ? "" : Request["EmailAddr"];
                dsData.MCompany[0].Remark = (string.IsNullOrEmpty(Request["Remark"])) ? "" : Request["Remark"];
                dsData.MCompany[0].ContactMan = (string.IsNullOrEmpty(Request["ContactMan"])) ? "" : Request["ContactMan"];
                dsData.MCompany[0].Tel = (string.IsNullOrEmpty(Request["Tel"])) ? "" : Request["Tel"];
                dsData.MCompany[0].MobilePhone = (string.IsNullOrEmpty(Request["MobilePhone"])) ? "" : Request["MobilePhone"];
                dsData.MCompany[0].Fax = (string.IsNullOrEmpty(Request["Fax"])) ? "" : Request["Fax"];
                dsData.MCompany[0].Address = (string.IsNullOrEmpty(Request["Address"])) ? "" : Request["Address"];
                dsData.MCompany[0].CompanyType = int.Parse(Request["CompanyType"]);
                dsData.MCompany[0].UpdateBy = LoginInfo.UserCode;
                dsData.MCompany[0].UpdateOn = DateTime.Now;
                MCompanyRule.UpdateData(dsData);
                AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "修改公司", Request["CompanyCode"], "公司管理", Request.UserHostAddress);

                return Content("Updateok");
            }
            else
                return Content("保存失败!");
        }
        public ContentResult MCompanySave()
        {
            MCompanyData dsData = MCompanyRule.GetCompanyDataByCode(Request["CompanyCode"]);
            if (dsData != null && dsData.MCompany.Rows.Count == 0)
            {
                MCompanyData.MCompanyRow row = dsData.MCompany.NewMCompanyRow();
                row.CompanyCode = Request["CompanyCode"];
                row.CompanyName = Request["CompanyName"];
                row.EmailAddr = (string.IsNullOrEmpty(Request["EmailAddr"])) ? "" : Request["EmailAddr"];
                row.Remark = (string.IsNullOrEmpty(Request["Remark"])) ? "" : Request["Remark"];
                row.ContactMan = (string.IsNullOrEmpty(Request["ContactMan"])) ? "" : Request["ContactMan"];
                row.Tel = (string.IsNullOrEmpty(Request["Tel"])) ? "" : Request["Tel"];
                row.MobilePhone = (string.IsNullOrEmpty(Request["MobilePhone"])) ? "" : Request["MobilePhone"];
                row.Fax = (string.IsNullOrEmpty(Request["Fax"])) ? "" : Request["Fax"];
                row.Address = (string.IsNullOrEmpty(Request["Address"])) ? "" : Request["Address"];
                row.CompanyType = int.Parse(Request["CompanyType"]);
                row.CreateBy = LoginInfo.UserCode;
                row.CreateOn = DateTime.Now;
                dsData.MCompany.AddMCompanyRow(row);
                MCompanyRule.UpdateData(dsData);
                AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "新增公司", Request["CompanyCode"], "公司管理", Request.UserHostAddress);
                return Content("Saveok");
            }
            else
                return Content("保存失败,存在相同的用户代码!");
        }

        public ContentResult GetMCompany()
        {
            ContentResult result = new ContentResult();
            string sCompanyCode = Request["CompanyCode"];
            DataTable dt = MCompanyRule.GetCompanyDataDByCode(sCompanyCode);
            result.Content = "{\"Msg\":\"ok\"," + ConvertJson.ToJson(dt).Replace("[{", "").Replace("}]", "") + "}";
            return result;
        }


        #endregion

        #region 设备管理
        public ActionResult MEquipment()
        {
            return View();
        }

        public ActionResult MEquipmentList(int page, int rows, string order, string sort)
        {
            ContentResult result = new ContentResult();
            int RecordCount, PageCount;
            DataTable dt = ARoleFunctionRule.GetRoleFunctionBy("T0102", LoginInfo.UserCode);
            DataSet ds = PageList.GetListbyPage("MEquipment", "EquipmentCode", page, rows,
                "*", "EquipmentCode", " and EquipmentCode like '%" + Request["EquipmentCode"] + "%'" + ((dt.Rows.Count > 0 && LoginInfo.UserCode.ToLower() != "admin") ? " and EquipmentCode in (select CustEquipmentCode from dbo.ACustomerEquipment where CustomerCode in  (select UserCustomerCode from dbo.ACustomerUser where UserCode='" + LoginInfo.UserCode + "'))":""), out RecordCount, out  PageCount);
            result.Content = "{\"total\":\"" + RecordCount + "\",\"rows\":" + ConvertJson.ToJson(ds.Tables[0]) + "}";
            return result;
        }

        public ActionResult JSonEquipment(string EquipmentCode)
        {
            ContentResult result = new ContentResult();
            DataTable dt = MEquipmentRule.GetEquipmentDataByEquipmentCode(Request["EquipmentCode"]);

            result.Content = ConvertJson.ToJson(dt);
            return result;
        }


        public ActionResult MEquipmentDelete(string idList)
        {
            try
            {
                MEquipmentRule.Delete(idList);
                AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "删除设备", idList, "设备管理", Request.UserHostAddress);
                return Json(new { success = true, Message = "删除成功！" });
            }
            catch
            {
                return Json(new { success = true, Message = "删除失败！" });
            }
        }
        public ContentResult MEquipmentUpdate()
        {
            MEquipmentData dsData = MEquipmentRule.GetEquipmentDataByProductCode(Request["ProductCode"]);
            if (dsData != null && dsData.MEquipment.Rows.Count > 0)
            {
                //dsData.MEquipment[0].EquipmentName = Request["EquipmentName"];
                dsData.MEquipment[0].EquipmentName = (string.IsNullOrEmpty(Request["EquipmentName"])) ? "" : Request["EquipmentName"];
                dsData.MEquipment[0].ProductCode = (string.IsNullOrEmpty(Request["ProductCode"])) ? "" : Request["ProductCode"];
                //dsData.MEquipment[0].EquipmentType = int.Parse(Request["EquipmentType"]);
                dsData.MEquipment[0].Province = (string.IsNullOrEmpty(Request["Province"])) ? "" : Request["Province"];
                dsData.MEquipment[0].City = (string.IsNullOrEmpty(Request["City"])) ? "" : Request["City"];
                dsData.MEquipment[0].County = (string.IsNullOrEmpty(Request["County"])) ? "" : Request["County"];
                dsData.MEquipment[0].Village = (string.IsNullOrEmpty(Request["Village"])) ? "" : Request["Village"];
                dsData.MEquipment[0].State = (string.IsNullOrEmpty(Request["State"])) ? "" : Request["State"];
                dsData.MEquipment[0].ResidentialArea = (string.IsNullOrEmpty(Request["ResidentialArea"])) ? "" : Request["ResidentialArea"];
                dsData.MEquipment[0].StreetNumber = (string.IsNullOrEmpty(Request["StreetNumber"])) ? "" : Request["StreetNumber"];
                dsData.MEquipment[0].Building = (string.IsNullOrEmpty(Request["Building"])) ? "" : Request["Building"];
                dsData.MEquipment[0].Unit = (string.IsNullOrEmpty(Request["Unit"])) ? "" : Request["Unit"];
                dsData.MEquipment[0].LadderNo = (string.IsNullOrEmpty(Request["LadderNo"])) ? "" : Request["LadderNo"];
                dsData.MEquipment[0].ManufacturerName = (string.IsNullOrEmpty(Request["ManufacturerName"])) ? "" : Request["ManufacturerName"];
                dsData.MEquipment[0].ManufacturerAddress = (string.IsNullOrEmpty(Request["ManufacturerAddress"])) ? "" : Request["ManufacturerAddress"];
                dsData.MEquipment[0].ManufacturerMan = (string.IsNullOrEmpty(Request["ManufacturerMan"])) ? "" : Request["ManufacturerMan"];
                dsData.MEquipment[0].ManufacturerTel = (string.IsNullOrEmpty(Request["ManufacturerTel"])) ? "" : Request["ManufacturerTel"];
                dsData.MEquipment[0].ResiAreaName = (string.IsNullOrEmpty(Request["ResiAreaName"])) ? "" : Request["ResiAreaName"];
                dsData.MEquipment[0].ResiAreaDirector = (string.IsNullOrEmpty(Request["ResiAreaDirector"])) ? "" : Request["ResiAreaDirector"];
                dsData.MEquipment[0].ResiAreaDirectorTel = (string.IsNullOrEmpty(Request["ResiAreaDirectorTel"])) ? "" : Request["ResiAreaDirectorTel"];
                dsData.MEquipment[0].MaintCompanyName = (string.IsNullOrEmpty(Request["MaintCompanyName"])) ? "" : Request["MaintCompanyName"];
                dsData.MEquipment[0].MaintCompanyDirector = (string.IsNullOrEmpty(Request["MaintCompanyDirector"])) ? "" : Request["MaintCompanyDirector"];
                dsData.MEquipment[0].MaintCompanyDirectorTel = (string.IsNullOrEmpty(Request["MaintCompanyDirectorTel"])) ? "" : Request["MaintCompanyDirectorTel"];
                dsData.MEquipment[0].MaintMan1 = (string.IsNullOrEmpty(Request["MaintMan1"])) ? "" : Request["MaintMan1"];
                dsData.MEquipment[0].MaintManTel1 = (string.IsNullOrEmpty(Request["MaintManTel1"])) ? "" : Request["MaintManTel1"];
                dsData.MEquipment[0].MaintMan2 = (string.IsNullOrEmpty(Request["MaintMan2"])) ? "" : Request["MaintMan2"];
                dsData.MEquipment[0].MaintManTel2 = (string.IsNullOrEmpty(Request["MaintManTel2"])) ? "" : Request["MaintManTel2"];
                dsData.MEquipment[0].MaintMan3 = (string.IsNullOrEmpty(Request["MaintMan3"])) ? "" : Request["MaintMan3"];
                dsData.MEquipment[0].MaintManTel3 = (string.IsNullOrEmpty(Request["MaintManTel3"])) ? "" : Request["MaintManTel3"];
                dsData.MEquipment[0].MaintMan4 = (string.IsNullOrEmpty(Request["MaintMan4"])) ? "" : Request["MaintMan4"];
                dsData.MEquipment[0].MaintManTel4 = (string.IsNullOrEmpty(Request["MaintManTel4"])) ? "" : Request["MaintManTel4"];
                dsData.MEquipment[0].MaintMan5 = (string.IsNullOrEmpty(Request["MaintMan5"])) ? "" : Request["MaintMan5"];
                dsData.MEquipment[0].MaintManTel5 = (string.IsNullOrEmpty(Request["MaintManTel5"])) ? "" : Request["MaintManTel5"];
                dsData.MEquipment[0].UpdateBy = LoginInfo.UserCode;
                dsData.MEquipment[0].UpdateOn = DateTime.Now;
                MEquipmentRule.UpdateData(dsData);
                AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "修改设备", Request["EquipmentCode"], "设备管理", Request.UserHostAddress);

                return Content("Updateok");
            }
            else
                return Content("保存失败!");
        }
        public ContentResult MEquipmentSave()
        {
            MEquipmentData dsData = MEquipmentRule.GetEquipmentDataByProductCode(Request["ProductCode"]);
            if (dsData != null && dsData.MEquipment.Rows.Count == 0)
            {
                MEquipmentData.MEquipmentRow row = dsData.MEquipment.NewMEquipmentRow();
                //row.EquipmentCode = CommonBase.GetSequence("SBDM" + DateTime.Now.ToString("yyyyMMdd"), "EquipmentCode", "");
                row.EquipmentCode = Request["EquipmentCode"];
                row.EquipmentName = (string.IsNullOrEmpty(Request["EquipmentName"])) ? "" : Request["EquipmentName"];
                row.ProductCode = (string.IsNullOrEmpty(Request["ProductCode"])) ? "" : Request["ProductCode"];
                row.EquipmentType = int.Parse(Request["EquipmentType"]);
                row.Province = (string.IsNullOrEmpty(Request["Province"])) ? "" : Request["Province"];
                row.City = (string.IsNullOrEmpty(Request["City"])) ? "" : Request["City"];
                row.County = (string.IsNullOrEmpty(Request["County"])) ? "" : Request["County"];
                row.Village = (string.IsNullOrEmpty(Request["Village"])) ? "" : Request["Village"];
                row.State = (string.IsNullOrEmpty(Request["State"])) ? "" : Request["State"];
                row.ResidentialArea = (string.IsNullOrEmpty(Request["ResidentialArea"])) ? "" : Request["ResidentialArea"];
                row.StreetNumber = (string.IsNullOrEmpty(Request["StreetNumber"])) ? "" : Request["StreetNumber"];
                row.Building = (string.IsNullOrEmpty(Request["Building"])) ? "" : Request["Building"];
                row.Unit = (string.IsNullOrEmpty(Request["Unit"])) ? "" : Request["Unit"];
                row.LadderNo = (string.IsNullOrEmpty(Request["LadderNo"])) ? "" : Request["LadderNo"];
                row.ManufacturerName = (string.IsNullOrEmpty(Request["ManufacturerName"])) ? "" : Request["ManufacturerName"];
                row.ManufacturerAddress = (string.IsNullOrEmpty(Request["ManufacturerAddress"])) ? "" : Request["ManufacturerAddress"];
                row.ManufacturerMan = (string.IsNullOrEmpty(Request["ManufacturerMan"])) ? "" : Request["ManufacturerMan"];
                row.ManufacturerTel = (string.IsNullOrEmpty(Request["ManufacturerTel"])) ? "" : Request["ManufacturerTel"];
                row.ResiAreaName = (string.IsNullOrEmpty(Request["ResiAreaName"])) ? "" : Request["ResiAreaName"];
                row.ResiAreaDirector = (string.IsNullOrEmpty(Request["ResiAreaDirector"])) ? "" : Request["ResiAreaDirector"];
                row.ResiAreaDirectorTel = (string.IsNullOrEmpty(Request["ResiAreaDirectorTel"])) ? "" : Request["ResiAreaDirectorTel"];
                row.MaintCompanyName = (string.IsNullOrEmpty(Request["MaintCompanyName"])) ? "" : Request["MaintCompanyName"];
                row.MaintCompanyDirector = (string.IsNullOrEmpty(Request["MaintCompanyDirector"])) ? "" : Request["MaintCompanyDirector"];
                row.MaintCompanyDirectorTel = (string.IsNullOrEmpty(Request["MaintCompanyDirectorTel"])) ? "" : Request["MaintCompanyDirectorTel"];
                row.MaintMan1 = (string.IsNullOrEmpty(Request["MaintMan1"])) ? "" : Request["MaintMan1"];
                row.MaintManTel1 = (string.IsNullOrEmpty(Request["MaintManTel1"])) ? "" : Request["MaintManTel1"];
                row.MaintMan2 = (string.IsNullOrEmpty(Request["MaintMan2"])) ? "" : Request["MaintMan2"];
                row.MaintManTel2 = (string.IsNullOrEmpty(Request["MaintManTel2"])) ? "" : Request["MaintManTel2"];
                row.MaintMan3 = (string.IsNullOrEmpty(Request["MaintMan3"])) ? "" : Request["MaintMan3"];
                row.MaintManTel3 = (string.IsNullOrEmpty(Request["MaintManTel3"])) ? "" : Request["MaintManTel3"];
                row.MaintMan4 = (string.IsNullOrEmpty(Request["MaintMan4"])) ? "" : Request["MaintMan4"];
                row.MaintManTel4 = (string.IsNullOrEmpty(Request["MaintManTel4"])) ? "" : Request["MaintManTel4"];
                row.MaintMan5 = (string.IsNullOrEmpty(Request["MaintMan5"])) ? "" : Request["MaintMan5"];
                row.MaintManTel5 = (string.IsNullOrEmpty(Request["MaintManTel5"])) ? "" : Request["MaintManTel5"];
                row.CreateBy = LoginInfo.UserCode;
                row.CreateOn = DateTime.Now;
                dsData.MEquipment.AddMEquipmentRow(row);
                MEquipmentRule.UpdateData(dsData);
                AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "新增设备", Request["EquipmentCode"], "设备管理", Request.UserHostAddress);
                return Content("Saveok");
            }
            else
                return Content("保存失败,存在相同的产品编码!");
        }



        #endregion

        #region 楼层管理
        public ActionResult MFloor()
        {
            return View();
        }

        public ActionResult MFloorList(int page, int rows, string order, string sort)
        {
            ContentResult result = new ContentResult();
            int RecordCount, PageCount;
            DataSet ds = PageList.GetListbyPage("MFloor", "FloorCode", page, rows,
                "*", "FloorCode", " and FloorCode like '%" + Request["FloorCode"] + "%'", out RecordCount, out  PageCount);
            result.Content = "{\"total\":\"" + RecordCount + "\",\"rows\":" + ConvertJson.ToJson(ds.Tables[0]) + "}";
            return result;
        }


        public ActionResult MFloorDelete(string idList)
        {
            try
            {
                MFloorRule.Delete(idList);
                AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "删除楼层", idList, "楼层管理", Request.UserHostAddress);
                return Json(new { success = true, Message = "删除成功！" });
            }
            catch
            {
                return Json(new { success = true, Message = "删除失败！" });
            }
        }
        public ContentResult MFloorUpdate()
        {
            MFloorData dsData = MFloorRule.GetFloorDataByCode(Request["FloorCode"]);
            if (dsData != null && dsData.MFloor.Rows.Count > 0)
            {
                //dsData.MFloor[0].FloorID = (string.IsNullOrEmpty(Request["FloorID"])) ? "" : Request["FloorID"];
                dsData.MFloor[0].FloorMun = (string.IsNullOrEmpty(Request["FloorMun"])) ? "0" : Request["FloorMun"];
                dsData.MFloor[0].FloorName = (string.IsNullOrEmpty(Request["FloorName"])) ? "" : Request["FloorName"];
                dsData.MFloor[0].Remark = (string.IsNullOrEmpty(Request["Remark"])) ? "" : Request["Remark"];
                dsData.MFloor[0].UpdateBy = LoginInfo.UserCode;
                dsData.MFloor[0].UpdateOn = DateTime.Now;
                MFloorRule.UpdateData(dsData);
                AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "修改楼层", Request["FloorCode"], "楼层管理", Request.UserHostAddress);

                return Content("Updateok");
            }
            else
                return Content("保存失败!");
        }
        public ContentResult MFloorSave()
        {
            MFloorData dsData = MFloorRule.GetFloorDataByCode(Request["FloorCode"]);
            if (dsData != null && dsData.MFloor.Rows.Count == 0)
            {
                MFloorData.MFloorRow row = dsData.MFloor.NewMFloorRow();
                row.FloorCode = (string.IsNullOrEmpty(Request["FloorCode"])) ? "" : Request["FloorCode"];
                //row.FloorID = (string.IsNullOrEmpty(Request["FloorID"])) ? "" : Request["FloorID"];
                row.FloorMun = (string.IsNullOrEmpty(Request["FloorMun"])) ? "0" : Request["FloorMun"];
                row.FloorName = (string.IsNullOrEmpty(Request["FloorName"])) ? "" : Request["FloorName"];
                row.Remark = (string.IsNullOrEmpty(Request["Remark"])) ? "" : Request["Remark"];
                row.CreateBy = LoginInfo.UserCode;
                row.CreateOn = DateTime.Now;
                dsData.MFloor.AddMFloorRow(row);
                MFloorRule.UpdateData(dsData);
                AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "新增楼层", Request["FloorCode"], "楼层管理", Request.UserHostAddress);
                return Content("Saveok");
            }
            else
                return Content("保存失败,存在相同的用户代码!");
        }



        #endregion

        #region 故障管理
        public ActionResult MFaultCodes()
        {
            return View();
        }

        public ActionResult MFaultCodesList(int page, int rows, string order, string sort)
        {
            ContentResult result = new ContentResult();
            int RecordCount, PageCount;
            DataSet ds = PageList.GetListbyPage("MFaultCodes", "FaultCode", page, rows,
                "*", "FaultCode", " and FaultCode like '%" + Request["FaultCode"] + "%'", out RecordCount, out  PageCount);
            result.Content = "{\"total\":\"" + RecordCount + "\",\"rows\":" + ConvertJson.ToJson(ds.Tables[0]) + "}";
            return result;
        }


        public ActionResult MFaultCodesDelete(string idList)
        {
            try
            {
                MFaultCodesRule.Delete(idList);
                AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "删除故障", idList, "故障代码管理", Request.UserHostAddress);
                return Json(new { success = true, Message = "删除成功！" });
            }
            catch
            {
                return Json(new { success = true, Message = "删除失败！" });
            }
        }
        public ContentResult MFaultCodesUpdate()
        {
            MFaultCodesData dsData = MFaultCodesRule.GetMFaultCodesDataByCode(Request["FaultCode"]);
            if (dsData != null && dsData.MFaultCodes.Rows.Count > 0)
            {
                //dsData.MFaultCodes[0].FaultID = (string.IsNullOrEmpty(Request["FaultID"])) ? "" : Request["FaultID"];
                dsData.MFaultCodes[0].FaultName = (string.IsNullOrEmpty(Request["FaultName"])) ? "" : Request["FaultName"];
                dsData.MFaultCodes[0].Remark = (string.IsNullOrEmpty(Request["Remark"])) ? "" : Request["Remark"];
                dsData.MFaultCodes[0].UpdateBy = LoginInfo.UserCode;
                dsData.MFaultCodes[0].UpdateOn = DateTime.Now;
                MFaultCodesRule.UpdateData(dsData);
                AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "修改故障", Request["FaultCode"], "故障代码管理", Request.UserHostAddress);

                return Content("Updateok");
            }
            else
                return Content("保存失败!");
        }
        public ContentResult MFaultCodesSave()
        {
            MFaultCodesData dsData = MFaultCodesRule.GetMFaultCodesDataByCode(Request["FaultCode"]);
            if (dsData != null && dsData.MFaultCodes.Rows.Count == 0)
            {
                MFaultCodesData.MFaultCodesRow row = dsData.MFaultCodes.NewMFaultCodesRow();
                row.FaultCode = (string.IsNullOrEmpty(Request["FaultCode"])) ? "" : Request["FaultCode"];
                //row.FaultID = (string.IsNullOrEmpty(Request["FaultID"])) ? "0" : Request["FaultID"];
                row.FaultName = (string.IsNullOrEmpty(Request["FaultName"])) ? "" : Request["FaultName"];
                row.Remark = (string.IsNullOrEmpty(Request["Remark"])) ? "" : Request["Remark"];
                row.CreateBy = LoginInfo.UserCode;
                row.CreateOn = DateTime.Now;
                dsData.MFaultCodes.AddMFaultCodesRow(row);
                MFaultCodesRule.UpdateData(dsData);
                AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "新增故障", Request["FaultCode"], "故障代码管理", Request.UserHostAddress);
                return Content("Saveok");
            }
            else
                return Content("保存失败,存在相同的用户代码!");
        }



        #endregion

        #region 其他
        public ContentResult GetComInfo(string strType,string InfoName, string EquipmentType)
        {
            string sInfo = "";

            DataTable dt = new DataTable();
            if (strType.ToLower() == "city")
            {
                dt = MEquipmentRule.GetCityByProvince(InfoName, EquipmentType);
                foreach (DataRow row in dt.Rows)
                    sInfo += row["City"] + ",";
            }
            else if (strType.ToLower() == "county")
            {
                dt = MEquipmentRule.GetCountyByCity(InfoName, EquipmentType);
                foreach (DataRow row in dt.Rows)
                    sInfo += row["County"] + ",";
            }
            else if (strType.ToLower() == "village")
            {
                dt = MEquipmentRule.GetVillageByCounty(InfoName, EquipmentType);
                foreach (DataRow row in dt.Rows)
                    sInfo += row["Village"] + ",";
            }
            else if (strType.ToLower() == "state")
            {
                dt = MEquipmentRule.GetStateByVillage(InfoName, EquipmentType);
                foreach (DataRow row in dt.Rows)
                    sInfo += row["State"] + ",";
            }
            else if (strType.ToLower() == "residentialarea")
            {
                dt = MEquipmentRule.GetResidentialAreaByState(InfoName, EquipmentType);
                foreach (DataRow row in dt.Rows)
                    sInfo += row["ResidentialArea"] + ",";
            }
            else if (strType.ToLower() == "streetnumber")
            {
                dt = MEquipmentRule.GetStreetNumberByResidentialArea(InfoName, EquipmentType);
                foreach (DataRow row in dt.Rows)
                    sInfo += row["StreetNumber"] + ",";
            }
            return Content((sInfo.Length > 0) ? sInfo.Substring(0, sInfo.Length - 1) : sInfo);

        }

        #endregion

    }
}
