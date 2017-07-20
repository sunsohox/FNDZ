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
using Admin.EntityDAO;
using Admin.EntityDefinitions;
using Common;


namespace FNDZWeb.Controllers
{
    [Authorize]
    public class SystemManageController : Controller
    {
        #region 用户管理
        public ActionResult AUser()
        {
            return View();
        }

        public ActionResult AUserMgr(string UserCode, string New)
        {
            AUserData dsAUserData = AUserRule.GetUserDataByCode(UserCode);
            AUserData.AUserRow row = null;
            if (dsAUserData != null && dsAUserData.AUser.Count > 0)
            {
                dsAUserData.AUser[0].Password = PasswordLocker.DESDecrypt(dsAUserData.AUser[0].Password);
                row = dsAUserData.AUser[0];
            }
            else
            {
                row = dsAUserData.AUser.NewAUserRow();
            }
            ViewBag.UserData = row;
            ViewBag.UserCode = (UserCode == "null" || string.IsNullOrEmpty(UserCode)) ? "" : UserCode;
            ViewBag.NUserCode = (New == "Y") ? "" : UserCode;
            ViewBag.New = (New == "null" || string.IsNullOrEmpty(New)) ? "Y" : New;
            ViewBag.LogonUserCode=LoginInfo.UserCode;
            return View();


            //return View();
        }

        [HttpPost]
        public ContentResult SaveUserMgr(string isNew, string CuUserRole)
        {
            ContentResult result = new ContentResult();
            AUserData dsAUserData = AUserRule.GetUserDataByCode(Request["UserCode"]);
            if (isNew == "Y" && dsAUserData.AUser.Count > 0)
            {
                result.Content = "已经存在相同用户代码，请确认！";
                return result;
            }
            try
            {
                if (dsAUserData != null && dsAUserData.AUser.Count > 0)
                {
                    dsAUserData.AUser[0].UserName = Request["UserName"];
                    dsAUserData.AUser[0].Password = PasswordLocker.DESEncrypt(Request["Password"]);
                    dsAUserData.AUser[0].EmailAddr = (string.IsNullOrEmpty(Request["EmailAddr"])) ? "" : Request["EmailAddr"];
                    dsAUserData.AUser[0].Remark = (string.IsNullOrEmpty(Request["Remark"])) ? "" : Request["Remark"];
                    dsAUserData.AUser[0].Phone = (string.IsNullOrEmpty(Request["Phone"])) ? "" : Request["Phone"];
                    dsAUserData.AUser[0].Mobile = (string.IsNullOrEmpty(Request["Mobile"])) ? "" : Request["Mobile"];
                    dsAUserData.AUser[0].Active = (Request.Form["Active"] == "on") ? 1 : 0;
                    dsAUserData.AUser[0].Department = (string.IsNullOrEmpty(Request["Department"])) ? "" : Request["Department"];
                    dsAUserData.AUser[0].Regional = Request["Regional"];
                    dsAUserData.AUser[0].TeamName = Request["TeamName"];
                    AUserRule.UpdateData(dsAUserData);
                    AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "修改用户", Request["UserCode"], "用户管理", Request.UserHostAddress);

                }
                else
                {
                    AUserData.AUserRow row = dsAUserData.AUser.NewAUserRow();
                    row.UserCode = Request["UserCode"];
                    row.UserName = Request["UserName"];
                    row.Password = PasswordLocker.DESEncrypt(Request["Password"]);
                    row.EmailAddr = (string.IsNullOrEmpty(Request["EmailAddr"])) ? "" : Request["EmailAddr"];
                    row.Remark = (string.IsNullOrEmpty(Request["Remark"])) ? "" : Request["Remark"];
                    row.Phone = (string.IsNullOrEmpty(Request["Phone"])) ? "" : Request["Phone"];
                    row.Mobile = (string.IsNullOrEmpty(Request["Mobile"])) ? "" : Request["Mobile"];
                    row.Active = (Request.Form["Active"] == "on") ? 1 : 0;
                    row.Department = (string.IsNullOrEmpty(Request["Department"])) ? "" : Request["Department"];
                    row.Regional = Request["Regional"];
                    row.TeamName = Request["TeamName"];
                    dsAUserData.AUser.AddAUserRow(row);
                    AUserRule.UpdateData(dsAUserData);

                    //默认插入当前客户
                    ACustomerUserRule.InsertACustomerUser(LoginInfo.CustomerCode, Request["UserCode"],"1","1");


                    AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "新增用户", Request["UserCode"], "用户管理", Request.UserHostAddress);
                    
                }

                List<string> roleList = new List<string>();
                try
                {
                    roleList = CuUserRole.Split('~').ToList<string>();
                }
                catch { }
                AUserRule.UpdateUserRole(Request["UserCode"], roleList);                

                result.Content = "ok";
            }
            catch (Exception ex)
            {
                result.Content = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="order"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        /// 
        public ActionResult AUserList(int page, int rows, string order, string sort)
        {
            ContentResult result = new ContentResult();
            int RecordCount, PageCount;
            DataTable dt = ARoleFunctionRule.GetRoleFunctionBy("T0101",LoginInfo.UserCode);
            DataSet ds = PageList.GetListbyPage("AUser", "UserCode", page, rows,
                "*", "UserCode", " and UserName like '%" + Request["UserName"] + "%'" + ((dt.Rows.Count > 0 && LoginInfo.UserCode.ToLower()!="admin") ? " and UserCode in (select UserCode from dbo.ACustomerUser where UserCustomerCode in (select UserCustomerCode from dbo.ACustomerUser where UserCode='" + LoginInfo.UserCode + "'))" : ""), out RecordCount, out  PageCount);
            foreach (DataRow row in ds.Tables[0].Rows)
                row["Password"] = PasswordLocker.DESDecrypt(row["Password"].ToString());
            result.Content = "{\"total\":\"" + RecordCount + "\",\"rows\":" + ConvertJson.ToJson(ds.Tables[0]) + "}";
            return result;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        public ActionResult AUserDelete(string idList)
        { 
            try
            {
                if (idList.ToLower().IndexOf("admin")!=-1)
                {
                    return Json(new { success = false, Message = "删除失败,admin用户不能删除！" });
                }
                AUserRule.RemoveUserInfo(idList);
                AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "删除用户", idList, "用户管理", Request.UserHostAddress);
                return Json(new { success = true, Message = "删除成功！" });
            }
            catch
            {
                return Json(new { success = true, Message = "删除失败！" });
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        public ContentResult AUserUpdate()
        {
            AUserData dsAUserData = AUserRule.GetUserDataByCode(Request["UserCode"]);
            if (dsAUserData != null && dsAUserData.AUser.Rows.Count > 0)
            {
                dsAUserData.AUser[0].UserName = Request["UserName"];
                dsAUserData.AUser[0].Password = PasswordLocker.DESEncrypt(Request["Password"]);
                dsAUserData.AUser[0].EmailAddr = (string.IsNullOrEmpty(Request["EmailAddr"])) ? "" : Request["EmailAddr"];
                dsAUserData.AUser[0].Remark = (string.IsNullOrEmpty(Request["Remark"])) ? "" : Request["Remark"];
                dsAUserData.AUser[0].Phone = (string.IsNullOrEmpty(Request["Phone"])) ? "" : Request["Phone"];
                dsAUserData.AUser[0].Mobile = (string.IsNullOrEmpty(Request["Mobile"])) ? "" : Request["Mobile"];
                dsAUserData.AUser[0].Active = (Request.Form["Active"] == "on") ? 1 : 0;
                dsAUserData.AUser[0].Department = (string.IsNullOrEmpty(Request["Department"])) ? "" : Request["Department"];
                dsAUserData.AUser[0].Regional = Request["Regional"];
                dsAUserData.AUser[0].TeamName = Request["TeamName"];
                AUserRule.UpdateData(dsAUserData);
                AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "修改用户", Request["UserCode"], "用户管理", Request.UserHostAddress);

                return Content("Updateok");
            }
            else
                return Content("保存失败!");
        }

        public ContentResult AUserSave()
        {
            AUserData dsAUserData = AUserRule.GetUserDataByCode(Request["UserCode"]);
            if (dsAUserData != null && dsAUserData.AUser.Rows.Count == 0)
            {
                string dp = Request.Form["Active"];
                AUserData.AUserRow row = dsAUserData.AUser.NewAUserRow();
                row.UserCode = Request["UserCode"];
                row.UserName = Request["UserName"];
                row.Password = PasswordLocker.DESEncrypt(Request["Password"]);
                row.EmailAddr = (string.IsNullOrEmpty(Request["EmailAddr"])) ? "" : Request["EmailAddr"];
                row.Remark = (string.IsNullOrEmpty(Request["Remark"])) ? "" : Request["Remark"];
                row.Phone = (string.IsNullOrEmpty(Request["Phone"])) ? "" : Request["Phone"];
                row.Mobile = (string.IsNullOrEmpty(Request["Mobile"])) ? "" : Request["Mobile"];
                row.Active = (Request.Form["Active"] == "on") ? 1 : 0;
                row.Department = (string.IsNullOrEmpty(Request["Department"])) ? "" : Request["Department"];
                row.Regional = Request["Regional"];
                row.TeamName = Request["TeamName"];
                dsAUserData.AUser.AddAUserRow(row);
                AUserRule.UpdateData(dsAUserData);
                AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "新增用户", Request["UserCode"], "用户管理", Request.UserHostAddress);
                return Content("Saveok");
            }
            else
                return Content("保存失败,存在相同的用户代码!");
        }

        #endregion

        #region 用户客户管理
        public ActionResult AUserCustomer(string UserCode)
        {
            ViewBag.UserCode = (UserCode == "null" || string.IsNullOrEmpty(UserCode)) ? "" : UserCode;
            ViewBag.LogonUserCode = LoginInfo.UserCode;
            return View();
        }

        public ContentResult GetAUserCustomerData(string UserCode)
        {
            ContentResult result = new ContentResult();
            DataTable dt = ACustomerUserRule.GetUserCustomerByUserCode(UserCode);
            result.Content = "{\"total\":\"" + dt.Rows.Count + "\",\"rows\":" + ConvertJson.ToJson(dt) + "}";
            return result;
        }

        public ContentResult GetACustomerData()
        {
            ContentResult result = new ContentResult();
            int RecordCount, PageCount;
            int page = 1;
            int rows = 100000;
            DataSet ds = PageList.GetListbyPage("ACustomer", "CustomerCode", page, rows,
                "*", "CustomerCode", " ", out RecordCount, out  PageCount);
            result.Content = ConvertJson.ToJson(ds.Tables[0]);
            return result;
        }

        public ContentResult SaveAUserCustomer(string UserCode)
        {
            try
            {
                string msg = SaveAUserCustomerData(UserCode);
                if (msg == "")
                {
                    return Content("ok");
                }
                else
                {
                    return Content(msg);
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
        private string SaveAUserCustomerData(string UserCode)
        {
            string strMes = "";
            List<Common.AUserCustomerList> listInsert = ConvertJson.JsonToObject<List<Common.AUserCustomerList>>(Request["InsertContent"], Encoding.UTF8);
            for (int i = 0; i < listInsert.Count; i++)
            {
                if (ACustomerUserRule.GetACustomerUserByCode(listInsert[i].UserCustomerCode, UserCode).ACustomerUser.Count == 0)
                {

                    if (ACustomerUserRule.InsertACustomerUser(listInsert[i].UserCustomerCode, UserCode, (listInsert[i].activeShow == "是" ? "1" : "0"), (listInsert[i].isDefaultShow == "是" ? "1" : "0")))
                        AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "分配用户客户功能", listInsert[i].UserCustomerCode, "用户客户功能", Request.UserHostAddress);
                }
                else
                {
                    strMes = "此客户已经存在";
                }
            }

            List<Common.AUserCustomerList> listupdate = ConvertJson.JsonToObject<List<Common.AUserCustomerList>>(Request["UpdateContent"], Encoding.UTF8);
            for (int i = 0; i < listupdate.Count; i++)
            {
                if (ACustomerUserRule.GetACustomerUserByCode(listupdate[i].UserCustomerCode, UserCode).ACustomerUser.Count > 0)
                {
                    ACustomerUserData dsACustomerUserData = ACustomerUserRule.GetACustomerUserByCode(listupdate[i].UserCustomerCode, UserCode);
                    dsACustomerUserData.ACustomerUser[0].Active = int.Parse((listupdate[i].activeShow == "是" ? "1" : "0"));
                    dsACustomerUserData.ACustomerUser[0].IsDefault = int.Parse((listupdate[i].isDefaultShow == "是" ? "1" : "0"));
                    ACustomerUserRule.UpdateData(dsACustomerUserData);
                    AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "修改用户客户功能", listupdate[i].UserCustomerCode, "用户客户功能", Request.UserHostAddress);
                }
                else
                {
                    strMes = "此客户已经存在";
                }
            }

            List<Common.AUserCustomerList> listdelete = ConvertJson.JsonToObject<List<Common.AUserCustomerList>>(Request["DeleteContent"], Encoding.UTF8);
            for (int i = 0; i < listdelete.Count; i++)
            {
                if (ACustomerUserRule.DeleACustomerUser(listdelete[i].UserCustomerCode, UserCode))
                    AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "删除用户客户功能", listdelete[i].UserCustomerCode, "用户客户功能", Request.UserHostAddress);
            }
            return strMes;
        }

        #endregion

        #region 用户设备管理
        public ActionResult AUserEquipment(string UserCode)
        {
            ViewBag.UserCode = (UserCode == "null" || string.IsNullOrEmpty(UserCode)) ? "" : UserCode;
            return View();
        }

        public ContentResult GetAUserEquipmentData(string UserCode)
        {
            ContentResult result = new ContentResult();
            DataTable dt = AUserEquipmentRule.GetUserEquipmentByUserCode(UserCode);
            result.Content = "{\"total\":\"" + dt.Rows.Count + "\",\"rows\":" + ConvertJson.ToJson(dt) + "}";
            return result;
        }

        public ContentResult GetAEquipmentData(string UserCode)
        {
            ContentResult result = new ContentResult();
            int RecordCount, PageCount;
            int page = 1;
            int rows = 100000;
            DataTable dt = AUserEquipmentRule.GetMEquipmentByUserCode(UserCode);
            result.Content = ConvertJson.ToJson(dt);
            return result;
        }

        public ContentResult SaveAUserEquipment(string UserCode)
        {
            try
            {
                string msg = SaveAUserEquipmentData(UserCode);
                if (msg == "")
                {
                    return Content("ok");
                }
                else
                {
                    return Content(msg);
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
        private string SaveAUserEquipmentData(string UserCode)
        {
            string strMes = "";
            List<Common.AUserEquipmentList> listInsert = ConvertJson.JsonToObject<List<Common.AUserEquipmentList>>(Request["InsertContent"], Encoding.UTF8);
            for (int i = 0; i < listInsert.Count; i++)
            {
                if (AUserEquipmentRule.GetAUserEquipmentByCode(listInsert[i].UserEquipmentCode, UserCode).AUserEquipment.Count == 0)
                {

                    if (AUserEquipmentRule.InsertAUserEquipment(listInsert[i].UserEquipmentCode, UserCode))
                        AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "分配用户设备功能", listInsert[i].UserEquipmentCode, "用户设备功能", Request.UserHostAddress);
                }
                else
                {
                    strMes = "此客户已经存在";
                }
            }

            List<Common.AUserEquipmentList> listdelete = ConvertJson.JsonToObject<List<Common.AUserEquipmentList>>(Request["DeleteContent"], Encoding.UTF8);
            for (int i = 0; i < listdelete.Count; i++)
            {
                if (AUserEquipmentRule.DeleAUserEquipment(listdelete[i].UserEquipmentCode, UserCode))
                    AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "删除用户设备功能", listdelete[i].UserEquipmentCode, "用户设备功能", Request.UserHostAddress);
            }
            return strMes;
        }

        #endregion

        #region 操作日志
        public ActionResult AUserTrace()
        {
            return View();
        }

        public ActionResult AUserTraceList(int page, int rows, string order, string sort)
        {
            ContentResult result = new ContentResult();
            int RecordCount, PageCount;
            DataSet ds = PageList.GetListbyPage("AUserTrace", "UserCode", page, rows,
                "*", "UserCode", "", out RecordCount, out  PageCount);
            result.Content = "{\"total\":\"" + RecordCount + "\",\"rows\":" + ConvertJson.ToJson(ds.Tables[0]) + "}";
            return result;
        }

        //public ActionResult AUserTraceDelete(string idList)
        //{
        //    IList<string> codeList = idList.Split(',');
        //    if (BLL_SystemManagement.RemoveUserTraceInfo(codeList, "ID"))
        //    {
        //        BLL_SystemManagement.AddUserTraceInfo(LoginInfo.UserCode, "删除日志", idList, "操作日志", Request.UserHostAddress);
        //        return Json(new { success = true, Message = "删除成功！" });
        //    }
        //    else
        //    {
        //        return Json(new { success = true, Message = "删除失败！" });
        //    }
        //}

        #endregion

        #region 角色管理

        public ActionResult ARole()
        {
            return View();
        }

        public ActionResult ARoleList(int page, int rows, string order, string sort)
        {
            ContentResult result = new ContentResult();
            int RecordCount, PageCount;
            DataSet ds = PageList.GetListbyPage("ARole", "RoleCode", page, rows,
                "*", "RoleCode", " and RoleName like '%" + Request["RoleName"] + "%'", out RecordCount, out  PageCount);
            result.Content = "{\"total\":\"" + RecordCount + "\",\"rows\":" + ConvertJson.ToJson(ds.Tables[0]) + "}";
            return result;
        }

        public ActionResult ARoleDelete(string idList)
        {
            try
            {
                ARoleRule.Delete(idList);
                AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "删除角色", idList, "角色管理", Request.UserHostAddress);
                return Json(new { success = true, Message = "删除成功！" });
            }
            catch
            {
                return Json(new { success = true, Message = "删除失败！" });
            }
        }

        public ContentResult ARoleUpdate()
        {
            ARoleData dsARoleData = ARoleRule.GetRoleDataByCode(Request["RoleCode"]);
            if (dsARoleData != null && dsARoleData.ARole.Rows.Count > 0)
            {
                dsARoleData.ARole[0].RoleName = Request["RoleName"];
                ARoleRule.UpdateData(dsARoleData);
                AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "修改角色", Request["RoleCode"], "角色管理", Request.UserHostAddress);

                return Content("Updateok");
            }
            else
                return Content("保存失败!");
        }

        public ContentResult ARoleSave()
        {
            ARoleData dsARoleData = ARoleRule.GetRoleDataByCode(Request["RoleCode"]);
            if (dsARoleData != null && dsARoleData.ARole.Rows.Count == 0)
            {
                ARoleData.ARoleRow row = dsARoleData.ARole.NewARoleRow();
                row.RoleCode = Request["RoleCode"];
                row.RoleName = Request["RoleName"];
                dsARoleData.ARole.AddARoleRow(row);
                ARoleRule.UpdateData(dsARoleData);
                AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "新增角色", Request["RoleCode"], "角色管理", Request.UserHostAddress);
                return Content("Saveok");
            }
            else
                return Content("保存失败,存在相同的角色代码!");
        }

        #endregion

        #region 角色功能
        public ActionResult ARoleFunction()
        {
            return View();
        }

        public ContentResult GetRoleFunctionData(string RoleCode, string ModuleCode)
        {
            if (String.IsNullOrEmpty(RoleCode))
                return null;

            ContentResult result = new ContentResult();
            DataTable dt = ARoleFunctionRule.GetListDataByCode(RoleCode, ModuleCode);
            result.Content = "{\"total\":\"" + dt.Rows.Count + "\",\"rows\":" + ConvertJson.ToJson(dt) + "}";
            return result;
        }

        [HttpPost]
        public ContentResult SaveRoleFunction()
        {
            ContentResult result = new ContentResult();
            try
            {
                if (!string.IsNullOrEmpty(Request["Content"]))
                {
                    List<Common.ARoleFunctionList> list = ConvertJson.JsonToObject<List<Common.ARoleFunctionList>>(Request["Content"], Encoding.UTF8);
                    foreach (Common.ARoleFunctionList item in list)
                    {
                        DataTable mode = ARoleFunctionRule.GetRoleFunctionByFunctionCodeandRoleCode(item.FunctionCode, item.RoleCode);
                        if (mode != null && mode.Rows.Count == 0 && item.SeleRole == "Y")
                        {
                            if (ARoleFunctionRule.InsertARoleFunction(item.RoleCode, item.FunctionCode))
                                AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "分配角色功能", item.RoleCode, "角色功能", Request.UserHostAddress);
                        }
                        else
                        {
                            if (item.SeleRole != "Y")
                            {
                                if (ARoleFunctionRule.DeleARoleFunction(item.RoleCode, item.FunctionCode))
                                    AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "修改角色功能", item.RoleCode, "角色功能", Request.UserHostAddress);
                            }
                        }
                    }
                }
                result.Content = "ok";
            }
            catch (Exception ex)
            {
                result.Content = ex.Message;
            }
            return result;
        }

        #endregion

        #region 用户角色
        public ActionResult AUserRole()
        {
            return View();
        }

        public ContentResult GetUserRoleData(string UserCode)
        {
            if (String.IsNullOrEmpty(UserCode))
                return null;

            ContentResult result = new ContentResult();
            DataTable dt = AUserRule.GetUserRoleData(UserCode);

            result.Content = "{\"total\":\"" + dt.Rows.Count + "\",\"rows\":" + ConvertJson.ToJson(dt) + "}";
            return result;
        }

        [HttpPost]
        public ContentResult SaveUserRole()
        {
            ContentResult result = new ContentResult();
            try
            {
                if (!string.IsNullOrEmpty(Request["Content"]))
                {
                    List<Common.AUserRoleList> list = ConvertJson.JsonToObject<List<Common.AUserRoleList>>(Request["Content"], Encoding.UTF8);
                    foreach (Common.AUserRoleList item in list)
                    {
                        DataTable mode = AUserRule.GetUserRoleDataByUserAndRole(item.UserCode, item.RoleCode);
                        if (mode != null && mode.Rows.Count == 0 && item.SeleRole == "Y")
                        {
                            if (AUserRule.InsertAUserRole(item.UserCode, item.RoleCode))
                                AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "分配用户角色", item.RoleCode, "用户角色", Request.UserHostAddress);
                        }
                        else
                        {
                            if (item.SeleRole != "Y")
                            {
                                if (AUserRule.DeleAUserRole(item.UserCode, item.RoleCode))
                                    AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "修改用户角色", item.RoleCode, "用户角色", Request.UserHostAddress);
                            }
                        }
                    }
                }
                result.Content = "ok";
            }
            catch (Exception ex)
            {
                result.Content = ex.Message;
            }
            return result;
        }

        #endregion

        #region 角色列显示

        public ActionResult ARoleListField()
        {
            return View();
        }

        public ContentResult GetRoleListFieldData(string RoleCode, string ScreenCode)
        {
            if (String.IsNullOrEmpty(RoleCode))
                return null;

            ContentResult result = new ContentResult();
            DataTable dt = ARoleListFieldRule.GetListData(RoleCode, ScreenCode);

            result.Content = "{\"total\":\"" + dt.Rows.Count + "\",\"rows\":" + ConvertJson.ToJson(dt) + "}";
            return result;
        }

        [HttpPost]
        public ContentResult SaveRoleListField()
        {
            ContentResult result = new ContentResult();
            try
            {
                if (!string.IsNullOrEmpty(Request["Content"]))
                {
                    List<Common.ARoleListFieldList> list = ConvertJson.JsonToObject<List<Common.ARoleListFieldList>>(Request["Content"], Encoding.UTF8);
                    foreach (Common.ARoleListFieldList item in list)
                    {
                        DataTable mode = ARoleListFieldRule.GetRoleListFieldData(item.ScreenCode, item.Field, item.RoleCode);
                        if (mode != null && mode.Rows.Count == 0 && item.SeleField == "Y")
                        {
                            if (ARoleListFieldRule.InsertARoleListField(item.ScreenCode, item.Field, item.RoleCode, item.Choice.ToString()))
                                AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "分配显示列功能", item.RoleCode, "角色功能", Request.UserHostAddress);
                        }
                        else
                        {
                            if (item.SeleField != "Y")
                            {
                                if (ARoleListFieldRule.DeleRoleListField(item.ScreenCode, item.Field, item.RoleCode))
                                    AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "修改显示列功能", item.RoleCode, "角色功能", Request.UserHostAddress);
                            }
                            else
                            {
                                ARoleListFieldRule.DeleRoleListField(item.ScreenCode, item.Field, item.RoleCode);
                                ARoleListFieldRule.InsertARoleListField(item.ScreenCode, item.Field, item.RoleCode, item.Choice.ToString());

                            }
                        }
                    }
                }
                result.Content = "ok";
            }
            catch (Exception ex)
            {
                result.Content = ex.Message;
            }
            return result;
        }

        

        #endregion

        #region 角色功能
        public ActionResult ARoleSysFunction()
        {
            return View();
        }

        public ContentResult GetRoleSysFunctionData(string RoleCode, string ScreenCode)
        {
            if (String.IsNullOrEmpty(RoleCode))
                return null;

            ContentResult result = new ContentResult();
            DataTable dt = ARoleSysFunctionRule.GetListData(RoleCode, ScreenCode);

            result.Content = "{\"total\":\"" + dt.Rows.Count + "\",\"rows\":" + ConvertJson.ToJson(dt) + "}";
            return result;
        }

        [HttpPost]
        public ContentResult SaveRoleSysFunction()
        {
            ContentResult result = new ContentResult();
            try
            {
                if (!string.IsNullOrEmpty(Request["Content"]))
                {
                    List<Common.ARoleSysFunctionList> list = ConvertJson.JsonToObject<List<Common.ARoleSysFunctionList>>(Request["Content"], Encoding.UTF8);
                    foreach (Common.ARoleSysFunctionList item in list)
                    {
                        DataTable mode = ARoleSysFunctionRule.GetARoleSysFunctionData(item.ScreenCode, item.Function, item.RoleCode);
                        if (mode != null && mode.Rows.Count == 0 && item.SeleFunction == "Y")
                        {
                            if (ARoleSysFunctionRule.InsertARoleSysFunction(item.ScreenCode, item.Function, item.RoleCode))
                                AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "分配系统功能", item.RoleCode, "角色功能", Request.UserHostAddress);
                        }
                        else
                        {
                            if (item.SeleFunction != "Y")
                            {
                                if (ARoleSysFunctionRule.DeleRoleSysFunction(item.ScreenCode, item.Function, item.RoleCode))
                                    AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "修改系统功能", item.RoleCode, "角色功能", Request.UserHostAddress);
                            }
                        }
                    }
                }
                result.Content = "ok";
            }
            catch (Exception ex)
            {
                result.Content = ex.Message;
            }
            return result;
        }

        #endregion

        #region 客户管理
        public ActionResult ACustomer()
        {
            return View();
        }

        public ActionResult ACustomerList(int page, int rows, string order, string sort)
        {
            ContentResult result = new ContentResult();
            int RecordCount, PageCount;
            Master.EntityDAO.MCompanyData dsMCompanyData = Master.EntityDefinitions.MCompanyRule.GetCompanyData();
            DataSet ds = PageList.GetListbyPage("ACustomer", "CustomerCode", page, rows,
                "*", "CustomerCode", " and CustomerCode like '%" + Request["CustomerCode"] + "%'", out RecordCount, out  PageCount);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                DataRow [] sele=dsMCompanyData.MCompany.Select(" CompanyCode ='" + row["CompanyCode"] + "'");
                row["CompanyCode"] = (sele.Length>0)?sele[0]["CompanyName"]:"";
            }
            result.Content = "{\"total\":\"" + RecordCount + "\",\"rows\":" + ConvertJson.ToJson(ds.Tables[0]) + "}";
            return result;
        }

        [HttpPost]
        public ContentResult ACustomerMgr()
        {
            ContentResult result = new ContentResult();
            ACustomerData dsData = ACustomerRule.GetCustomerDataByCode(Request["CustomerCode"]);
            try
            {
                if (dsData != null && dsData.ACustomer.Count > 0)
                {
                    dsData.ACustomer[0].CustomerName = (string.IsNullOrEmpty(Request["CustomerName"])) ? "" : Request["CustomerName"];
                    dsData.ACustomer[0].CompanyCode = (string.IsNullOrEmpty(Request["drpCompanyCode"]) || Request["drpCompanyCode"] == "-1") ? "" : Request["drpCompanyCode"];
                    dsData.ACustomer[0].EmailAddr = (string.IsNullOrEmpty(Request["EmailAddr"])) ? "" : Request["EmailAddr"];
                    dsData.ACustomer[0].Remark = (string.IsNullOrEmpty(Request["Remark"])) ? "" : Request["Remark"];
                    dsData.ACustomer[0].ContactMan = (string.IsNullOrEmpty(Request["ContactMan"])) ? "" : Request["ContactMan"];
                    dsData.ACustomer[0].Tel = (string.IsNullOrEmpty(Request["Tel"])) ? "" : Request["Tel"];
                    dsData.ACustomer[0].MobilePhone = (string.IsNullOrEmpty(Request["MobilePhone"])) ? "" : Request["MobilePhone"];
                    dsData.ACustomer[0].Fax = (string.IsNullOrEmpty(Request["Fax"])) ? "" : Request["Fax"];
                    dsData.ACustomer[0].Address = (string.IsNullOrEmpty(Request["Address"])) ? "" : Request["Address"];
                    dsData.ACustomer[0].UpdateBy = LoginInfo.UserCode;
                    dsData.ACustomer[0].UpdateOn = DateTime.Now;
                    ACustomerRule.UpdateData(dsData);
                    AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "客户楼层", Request["CustomerCode"], "客户管理", Request.UserHostAddress);

                }
                else
                {
                    string s = Request.Form["drpCompanyCode"];
                    ACustomerData.ACustomerRow row = dsData.ACustomer.NewACustomerRow();
                    row.CustomerCode = (string.IsNullOrEmpty(Request["CustomerCode"])) ? "" : Request["CustomerCode"];
                    row.CustomerName = (string.IsNullOrEmpty(Request["CustomerName"])) ? "" : Request["CustomerName"];
                    row.CompanyCode = (string.IsNullOrEmpty(Request["drpCompanyCode"]) || Request["drpCompanyCode"]=="-1") ? "" : Request["drpCompanyCode"];
                    row.EmailAddr = (string.IsNullOrEmpty(Request["EmailAddr"])) ? "" : Request["EmailAddr"];
                    row.Remark = (string.IsNullOrEmpty(Request["Remark"])) ? "" : Request["Remark"];
                    row.ContactMan = (string.IsNullOrEmpty(Request["ContactMan"])) ? "" : Request["ContactMan"];
                    row.Tel = (string.IsNullOrEmpty(Request["Tel"])) ? "" : Request["Tel"];
                    row.MobilePhone = (string.IsNullOrEmpty(Request["MobilePhone"])) ? "" : Request["MobilePhone"];
                    row.Fax = (string.IsNullOrEmpty(Request["Fax"])) ? "" : Request["Fax"];
                    row.Address = (string.IsNullOrEmpty(Request["Address"])) ? "" : Request["Address"];
                    row.CreateBy = LoginInfo.UserCode;
                    row.CreateOn = DateTime.Now;
                    dsData.ACustomer.AddACustomerRow(row);
                    ACustomerRule.UpdateData(dsData);
                    AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "新增客户", Request["CustomerCode"], "客户管理", Request.UserHostAddress);
                }
                result.Content = "ok";
            }
            catch (Exception ex)
            {
                result.Content = ex.Message;
            }
            return result;
        }


        public ActionResult ACustomerDelete(string idList)
        {
            try
            {
                ACustomerRule.Delete(idList);
                AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "删除客户", idList, "客户管理", Request.UserHostAddress);
                return Json(new { success = true, Message = "删除成功！" });
            }
            catch
            {
                return Json(new { success = true, Message = "删除失败！" });
            }
        }
        public ContentResult ACustomerUpdate()
        {
            ACustomerData dsData = ACustomerRule.GetCustomerDataByCode(Request["CustomerCode"]);
            if (dsData != null && dsData.ACustomer.Rows.Count > 0)
            {
                dsData.ACustomer[0].CustomerName = (string.IsNullOrEmpty(Request["CustomerName"])) ? "" : Request["CustomerName"];
                dsData.ACustomer[0].CompanyCode = (string.IsNullOrEmpty(Request["CompanyCode"])) ? "" : Request["CompanyCode"];
                dsData.ACustomer[0].EmailAddr = (string.IsNullOrEmpty(Request["EmailAddr"])) ? "" : Request["EmailAddr"];
                dsData.ACustomer[0].Remark = (string.IsNullOrEmpty(Request["Remark"])) ? "" : Request["Remark"];
                dsData.ACustomer[0].ContactMan = (string.IsNullOrEmpty(Request["ContactMan"])) ? "" : Request["ContactMan"];
                dsData.ACustomer[0].Tel = (string.IsNullOrEmpty(Request["Tel"])) ? "" : Request["Tel"];
                dsData.ACustomer[0].MobilePhone = (string.IsNullOrEmpty(Request["MobilePhone"])) ? "" : Request["MobilePhone"];
                dsData.ACustomer[0].Fax = (string.IsNullOrEmpty(Request["Fax"])) ? "" : Request["Fax"];
                dsData.ACustomer[0].Address = (string.IsNullOrEmpty(Request["Address"])) ? "" : Request["Address"];
                dsData.ACustomer[0].UpdateBy = LoginInfo.UserCode;
                dsData.ACustomer[0].UpdateOn = DateTime.Now;
                ACustomerRule.UpdateData(dsData);
                AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "客户楼层", Request["CustomerCode"], "客户管理", Request.UserHostAddress);

                return Content("Updateok");
            }
            else
                return Content("保存失败!");
        }
        public ContentResult ACustomerSave()
        {
            ACustomerData dsData = ACustomerRule.GetCustomerDataByCode(Request["CustomerCode"]);
            if (dsData != null && dsData.ACustomer.Rows.Count == 0)
            {
                ACustomerData.ACustomerRow row = dsData.ACustomer.NewACustomerRow();
                row.CustomerCode = CommonBase.GetSequence("KHDM" + DateTime.Now.ToString("yyyyMMdd"), "CustomerCode", "");
                row.CustomerName = (string.IsNullOrEmpty(Request["CustomerName"])) ? "" : Request["CustomerName"];
                row.CompanyCode = (string.IsNullOrEmpty(Request["CompanyCode"])) ? "" : Request["CompanyCode"];
                row.EmailAddr = (string.IsNullOrEmpty(Request["EmailAddr"])) ? "" : Request["EmailAddr"];
                row.Remark = (string.IsNullOrEmpty(Request["Remark"])) ? "" : Request["Remark"];
                row.ContactMan = (string.IsNullOrEmpty(Request["ContactMan"])) ? "" : Request["ContactMan"];
                row.Tel = (string.IsNullOrEmpty(Request["Tel"])) ? "" : Request["Tel"];
                row.MobilePhone = (string.IsNullOrEmpty(Request["MobilePhone"])) ? "" : Request["MobilePhone"];
                row.Fax = (string.IsNullOrEmpty(Request["Fax"])) ? "" : Request["Fax"];
                row.Address = (string.IsNullOrEmpty(Request["Address"])) ? "" : Request["Address"];
                row.CreateBy = LoginInfo.UserCode;
                row.CreateOn = DateTime.Now;
                dsData.ACustomer.AddACustomerRow(row);
                ACustomerRule.UpdateData(dsData);
                AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "新增客户", Request["CustomerCode"], "客户管理", Request.UserHostAddress);
                return Content("Saveok");
            }
            else
                return Content("保存失败,存在相同的用户代码!");
        }



        #endregion

        #region 客户相关功能管理
        public ActionResult ACustomerMgr(string CustomerCode, string New)
        {
            ACustomerData dsACustomerData = ACustomerRule.GetCustomerDataByCode(CustomerCode);
            ACustomerData.ACustomerRow row = null;
            if (dsACustomerData != null && dsACustomerData.ACustomer.Count > 0)
            {
                row = dsACustomerData.ACustomer[0];
            }
            else
            {
                row = dsACustomerData.ACustomer.NewACustomerRow();
            }
            ViewBag.CompanyData = row;
            ViewBag.CustomerCode = (CustomerCode=="null" || string.IsNullOrEmpty(CustomerCode))?"":CustomerCode;
            ViewBag.NCustomerCode = (New == "Y") ? CommonBase.GetSequence("KHDM" + DateTime.Now.ToString("yyyyMMdd"), "CustomerCode", "") : CustomerCode;
            ViewBag.New = (New == "null" || string.IsNullOrEmpty(New)) ? "" : New;
            return View();


            //return View();
        }

       

        

        public ActionResult GetMFloorData()
        {
            string MFloorData = "";
            Master.EntityDAO.MFloorData dsMFloorData = Master.EntityDefinitions.MFloorRule.GetFloorDataOrderByFloorMun();
            foreach (Master.EntityDAO.MFloorData.MFloorRow _row in dsMFloorData.MFloor.Rows)
            {
                MFloorData = MFloorData + "{ FloorCode: '" + _row.FloorCode + "', FloorName: '" + _row.FloorName + "' },";
            }
            if (!string.IsNullOrEmpty(MFloorData)) { MFloorData = "[" + MFloorData.Substring(0, MFloorData.Length - 1) + "]"; }

            return Json(MFloorData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 客户用户关系管理
        public ActionResult ACustomerUser(string CustomerCode)
        {
            ViewBag.CustomerCode = (CustomerCode == "null" || string.IsNullOrEmpty(CustomerCode)) ? "" : CustomerCode;
            return View();
        }

        public ContentResult GetACustomerUserData(string CustomerCode, string UserCode, string UserName)
        {
            ContentResult result = new ContentResult();
            DataTable dt = ACustomerUserRule.GetACustomerUserListData(CustomerCode, UserCode, UserName);
            result.Content = "{\"total\":\"" + dt.Rows.Count + "\",\"rows\":" + ConvertJson.ToJson(dt) + "}";
            return result;
        }

        [HttpPost]
        public ContentResult SaveCustomerUser()
        {
            ContentResult result = new ContentResult();
            try
            {
                //string CustomerCode = Request["CustomerCode"];
                //if (!string.IsNullOrEmpty(Request["Content"]))
                //{
                //    List<Common.ACustomerUserList> list = ConvertJson.JsonToObject<List<Common.ACustomerUserList>>(Request["Content"], Encoding.UTF8);
                //    foreach (Common.ACustomerUserList item in list)
                //    {
                //        DataTable mode = ACustomerUserRule.GetACustomerUserByCode(CustomerCode, item.UserCode);
                //        if (mode != null && mode.Rows.Count == 0 && item.IsCustomerUser == "Y")
                //        {
                //            if (ACustomerUserRule.InsertACustomerUser(CustomerCode, item.UserCode))
                //                AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "分配客户用户功能", item.UserCode, "客户用户功能", Request.UserHostAddress);
                //        }
                //        else
                //        {
                //            if (item.IsCustomerUser != "Y")
                //            {
                //                if (ACustomerUserRule.DeleACustomerUser(CustomerCode, item.UserCode))
                //                    AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "修改客户用户功能", item.UserCode, "客户用户功能", Request.UserHostAddress);
                //            }
                //        }
                //    }
                //}
                result.Content = "ok";
            }
            catch (Exception ex)
            {
                result.Content = ex.Message;
            }
            return result;
        }

        #endregion

        #region 客户设备关系管理
        public ActionResult ACustomerEquipment(string CustomerCode)
        {
            ViewBag.CustomerCode = (CustomerCode == "null" || string.IsNullOrEmpty(CustomerCode)) ? "" : CustomerCode;
            return View();
        }

        public ContentResult GetACustomerEquipmentData(string CustomerCode)
        {
            ContentResult result = new ContentResult();
            DataTable dt = ACustomerEquipmentRule.GetACustomerEquipmentListByCustomerCode(CustomerCode);
            result.Content = "{\"total\":\"" + dt.Rows.Count + "\",\"rows\":" + ConvertJson.ToJson(dt) + "}";
            return result;
        }

        public ContentResult SaveACustomerEquipment(string CustomerCode)
        {
            try
            {
                string msg = SaveACustomerEquipmentData(CustomerCode);
                if (msg == "")
                {
                    return Content("ok");
                }
                else
                {
                    return Content(msg);
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
        private string SaveACustomerEquipmentData(string CustomerCode)
        {
            string strMes = "";
            List<Common.ACustomerEquipmentList> listInsert = ConvertJson.JsonToObject<List<Common.ACustomerEquipmentList>>(Request["InsertContent"], Encoding.UTF8);
            for (int i = 0; i < listInsert.Count; i++)
            {
                if (ACustomerEquipmentRule.GetACustomerEquipmentByCode(CustomerCode, listInsert[i].CustEquipmentCode).Rows.Count == 0)
                {

                    if (ACustomerEquipmentRule.InsertACustomerEquipment(CustomerCode, listInsert[i].CustEquipmentCode))
                        AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "分配客户设备功能", listInsert[i].CustEquipmentCode, "客户设备功能", Request.UserHostAddress);
                }
                else
                {
                    strMes = "此设备已经存在";
                }
            }
            List<Common.ACustomerEquipmentList> listdelete = ConvertJson.JsonToObject<List<Common.ACustomerEquipmentList>>(Request["DeleteContent"], Encoding.UTF8);
            for (int i = 0; i < listdelete.Count; i++)
            {
                if (ACustomerEquipmentRule.DeleACustomerEquipment(CustomerCode, listdelete[i].CustEquipmentCode))
                    AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "删除客户设备功能", listdelete[i].CustEquipmentCode, "客户设备功能", Request.UserHostAddress);
            }
            return strMes;
        }



        

        public ContentResult ACustomerEquipmentUpdate(string CustomerCode, string EquipmentCode)
        {
            //ACustomerEquipmentData dsACustomerEquipmentData = ACustomerEquipmentRule.GetACustomerEquipmentByCodeN(CustomerCode, EquipmentCode);
            //if (dsACustomerEquipmentData != null && dsACustomerEquipmentData.ACustomerEquipment.Rows.Count > 0)
            //{
            //    dsACustomerEquipmentData.ACustomerEquipment[0].CustomerInterFace = Request["CustomerInterFace"];

            //    ACustomerEquipmentRule.UpdateData(dsACustomerEquipmentData);
            //    AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "修改客户设备报文接口", EquipmentCode, "客户设备管理", Request.UserHostAddress);

            //    return Content("Updateok");
            //}
            //else
                return Content("保存失败!");
        }


        public ContentResult GetMEquipmentData(string CustomerCode)
        {
            ContentResult result = new ContentResult();
            int RecordCount, PageCount;
            int page = 1;
            int rows = 100000;
            DataSet ds = PageList.GetListbyPage("MEquipment", "EquipmentCode", page, rows,
                "*", "EquipmentCode", " and EquipmentCode not in ( select CustEquipmentCode from dbo.ACustomerEquipment where CustomerCode!='" + CustomerCode + "')", out RecordCount, out  PageCount);
            result.Content = ConvertJson.ToJson(ds.Tables[0]);
            return result;
        }

        #endregion

        #region 客户设备故障代码
        public ActionResult ACustomerEquipmentFault(string CustomerCode)
        {
            ViewBag.CustomerCode = (CustomerCode == "null" || string.IsNullOrEmpty(CustomerCode)) ? "" : CustomerCode;
            return View();
        }

        public ContentResult GetACustomerEquipmentFaultData(string CustomerCode, string CustEquipmentCode)
        {
            ContentResult result = new ContentResult();
            DataTable dt = ACustomerEquipmentFaultRule.GetACustomerEquipmentFaultListByCode(CustomerCode, CustEquipmentCode);
            result.Content = "{\"total\":\"" + dt.Rows.Count + "\",\"rows\":" + ConvertJson.ToJson(dt) + "}";
            return result;
        }

        public ContentResult GetACustomerEquipmentFaultDataByID(string ID)
        {
            ContentResult result = new ContentResult();
            DataTable dt = ACustomerEquipmentFaultRule.GetACustomerEquipmentFaultListByID(ID);
            result.Content = "{\"total\":\"" + dt.Rows.Count + "\",\"rows\":" + ConvertJson.ToJson(dt) + "}";
            return result;
        }

        public ActionResult CopyACustomerEquipmentFault(string ID, string CustomerCode, string CustEquipmentCode)
        {
            string sMeg = "";
            string sCustomerEquipmentFaultID = CommonBase.GetSequence("GZDM" + DateTime.Now.ToString("yyyyMMdd"), "CustomerEquipmentFaultID", "");
            if (ACustomerEquipmentFaultRule.CopyACustomerEquipmentFault(ID,sCustomerEquipmentFaultID, CustomerCode, CustEquipmentCode))
            {
                AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "拷贝客户设备故障功能", sCustomerEquipmentFaultID, "客户设备故障功能", Request.UserHostAddress);
                sMeg="Saveok";
            }
            else
                sMeg="保存失败!";

            return Json(sMeg, JsonRequestBehavior.AllowGet);
        }


        public ContentResult GetMFaultCodesData()
        {
            ContentResult result = new ContentResult();
            int RecordCount, PageCount;
            int page = 1;
            int rows = 100000;
            DataSet ds = PageList.GetListbyPage("MFaultCodes", "FaultCode", page, rows,
                "*", "FaultCode", "", out RecordCount, out  PageCount);
            result.Content = ConvertJson.ToJson(ds.Tables[0]);
            return result;
        }

        public ContentResult SaveACustomerEquipmentFault(string CustomerCode, string CustEquipmentCode)
        {
            try
            {
                string msg = SaveACustomerEquipmentFaultData(CustomerCode, CustEquipmentCode);
                if (msg == "")
                {
                    return Content("ok");
                }
                else
                {
                    return Content(msg);
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
        private string SaveACustomerEquipmentFaultData(string CustomerCode, string CustEquipmentCode)
        {
            string strMes = "";
            List<Common.ACustomerEquipmentFaultList> listInsert = ConvertJson.JsonToObject<List<Common.ACustomerEquipmentFaultList>>(Request["InsertContent"], Encoding.UTF8);
            DataTable dt=ACustomerEquipmentFaultRule.GetACustomerEquipmentFaultIDByCode(CustomerCode, CustEquipmentCode);
            string sCustomerEquipmentFaultID = (dt.Rows.Count > 0) ? dt.Rows[0]["ID"].ToString() : CommonBase.GetSequence("GZDM" + DateTime.Now.ToString("yyyyMMdd"), "CustomerEquipmentFaultID", "");
            for (int i = 0; i < listInsert.Count; i++)
            {
                if (ACustomerEquipmentFaultRule.GetACustomerEquipmentFaultByCode(CustomerCode, CustEquipmentCode,listInsert[i].CustFaultCode).Rows.Count == 0)
                {

                    if (ACustomerEquipmentFaultRule.InsertACustomerEquipmentFault(sCustomerEquipmentFaultID, CustomerCode, CustEquipmentCode, listInsert[i].CustFaultCode, listInsert[i].CustFaultID))
                        AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "分配客户设备故障功能", listInsert[i].CustFaultCode, "客户设备故障功能", Request.UserHostAddress);
                }
                else
                {
                    strMes = "此设备故障已经存在";
                }
            }
            List<Common.ACustomerEquipmentFaultList> listupdate = ConvertJson.JsonToObject<List<Common.ACustomerEquipmentFaultList>>(Request["UpdateContent"], Encoding.UTF8);
            for (int i = 0; i < listupdate.Count; i++)
            {
                if (ACustomerEquipmentFaultRule.GetACustomerEquipmentFaultByCode(CustomerCode, CustEquipmentCode, listupdate[i].CustFaultCode).Rows.Count > 0)
                {

                    if (ACustomerEquipmentFaultRule.UpdateACustomerEquipmentFault(sCustomerEquipmentFaultID, CustomerCode, CustEquipmentCode, listupdate[i].CustFaultCode, listupdate[i].CustFaultID))
                        AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "修改客户设备故障功能", listupdate[i].CustFaultCode, "客户设备故障功能", Request.UserHostAddress);
                }
                else
                {
                    strMes = "此设备故障不存在";
                }
            }

            List<Common.ACustomerEquipmentFaultList> listdelete = ConvertJson.JsonToObject<List<Common.ACustomerEquipmentFaultList>>(Request["DeleteContent"], Encoding.UTF8);
            for (int i = 0; i < listdelete.Count; i++)
            {
                if (ACustomerEquipmentFaultRule.DeleACustomerEquipmentFault(CustomerCode, CustEquipmentCode, listdelete[i].CustFaultCode))
                    AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "删除客户设备故障功能", listdelete[i].CustFaultCode, "客户设备故障功能", Request.UserHostAddress);
            }
            return strMes;
        }

        #endregion

        #region 客户设备楼层
        public ActionResult ACustomerEquipmentFloor(string CustomerCode)
        {
            ViewBag.CustomerCode = (CustomerCode == "null" || string.IsNullOrEmpty(CustomerCode)) ? "" : CustomerCode;
            return View();
        }

        public ContentResult GetACustomerEquipmentFloorData(string CustomerCode, string CustEquipmentCode)
        {
            ContentResult result = new ContentResult();
            DataTable dt = ACustomerEquipmentFloorRule.GetACustomerEquipmentFloorListByCode(CustomerCode, CustEquipmentCode);
            result.Content = "{\"total\":\"" + dt.Rows.Count + "\",\"rows\":" + ConvertJson.ToJson(dt) + "}";
            return result;
        }

        public ContentResult GetACustomerEquipmentFloorDataByID(string ID)
        {
            ContentResult result = new ContentResult();
            DataTable dt = ACustomerEquipmentFloorRule.GetACustomerEquipmentFloorListByID(ID);
            result.Content = "{\"total\":\"" + dt.Rows.Count + "\",\"rows\":" + ConvertJson.ToJson(dt) + "}";
            return result;
        }

        public ActionResult CopyACustomerEquipmentFloor(string ID, string CustomerCode, string CustEquipmentCode)
        {
            string sMeg = "";
            string sCustomerEquipmentFloorID = CommonBase.GetSequence("LCDM" + DateTime.Now.ToString("yyyyMMdd"), "CustomerEquipmentFloorID", "");
            if (ACustomerEquipmentFloorRule.CopyACustomerEquipmentFloor(ID, sCustomerEquipmentFloorID, CustomerCode, CustEquipmentCode))
            {
                AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "拷贝客户设备楼层功能", sCustomerEquipmentFloorID, "客户设备楼层功能", Request.UserHostAddress);
                sMeg = "Saveok";
            }
            else
                sMeg = "保存失败!";

            return Json(sMeg, JsonRequestBehavior.AllowGet);
        }


        public ContentResult GetMFloorCodesData()
        {
            ContentResult result = new ContentResult();
            int RecordCount, PageCount;
            int page = 1;
            int rows = 100000;
            DataSet ds = PageList.GetListbyPage("MFloor", "FloorCode", page, rows,
                "*", "FloorCode", "", out RecordCount, out  PageCount);
            result.Content = ConvertJson.ToJson(ds.Tables[0]);
            return result;
        }

        public ContentResult SaveACustomerEquipmentFloor(string CustomerCode, string CustEquipmentCode)
        {
            try
            {
                string msg = SaveACustomerEquipmentFloorData(CustomerCode, CustEquipmentCode);
                if (msg == "")
                {
                    return Content("ok");
                }
                else
                {
                    return Content(msg);
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
        private string SaveACustomerEquipmentFloorData(string CustomerCode, string CustEquipmentCode)
        {
            string strMes = "";
            List<Common.ACustomerEquipmentFloorList> listInsert = ConvertJson.JsonToObject<List<Common.ACustomerEquipmentFloorList>>(Request["InsertContent"], Encoding.UTF8);
            DataTable dt = ACustomerEquipmentFloorRule.GetACustomerEquipmentFloorIDByCode(CustomerCode, CustEquipmentCode);
            string sCustomerEquipmentFloorID = (dt.Rows.Count > 0) ? dt.Rows[0]["ID"].ToString() : CommonBase.GetSequence("LCDM" + DateTime.Now.ToString("yyyyMMdd"), "CustomerEquipmentFloorID", "");
            for (int i = 0; i < listInsert.Count; i++)
            {
                if (ACustomerEquipmentFloorRule.GetACustomerEquipmentFloorByCode(CustomerCode, CustEquipmentCode, listInsert[i].CustFloorCode).Rows.Count == 0)
                {

                    if (ACustomerEquipmentFloorRule.InsertACustomerEquipmentFloor(sCustomerEquipmentFloorID, CustomerCode, CustEquipmentCode, listInsert[i].CustFloorCode, listInsert[i].CustFloorID))
                        AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "分配客户设备楼层功能", listInsert[i].CustFloorCode, "客户设备楼层功能", Request.UserHostAddress);
                }
                else
                {
                    strMes = "此设备故障已经存在";
                }
            }

            List<Common.ACustomerEquipmentFloorList> listupdate = ConvertJson.JsonToObject<List<Common.ACustomerEquipmentFloorList>>(Request["UpdateContent"], Encoding.UTF8);
            for (int i = 0; i < listupdate.Count; i++)
            {
                if (ACustomerEquipmentFloorRule.GetACustomerEquipmentFloorByCode(CustomerCode, CustEquipmentCode, listupdate[i].CustFloorCode).Rows.Count > 0)
                {

                    if (ACustomerEquipmentFloorRule.UpdateACustomerEquipmentFloor(sCustomerEquipmentFloorID, CustomerCode, CustEquipmentCode, listInsert[i].CustFloorCode, listInsert[i].CustFloorID))
                        AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "修改客户设备楼层功能", listupdate[i].CustFloorCode, "客户设备楼层功能", Request.UserHostAddress);
                }
                else
                {
                    strMes = "此设备故障已经存在";
                }
            }


            List<Common.ACustomerEquipmentFloorList> listdelete = ConvertJson.JsonToObject<List<Common.ACustomerEquipmentFloorList>>(Request["DeleteContent"], Encoding.UTF8);
            for (int i = 0; i < listdelete.Count; i++)
            {
                if (ACustomerEquipmentFloorRule.DeleACustomerEquipmentFloor(CustomerCode, CustEquipmentCode, listdelete[i].CustFloorCode))
                    AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "删除客户设备楼层功能", listdelete[i].CustFloorCode, "客户设备楼层功能", Request.UserHostAddress);
            }
            return strMes;
        }

        #endregion

    }
}