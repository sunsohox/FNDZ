using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Text;
using System.Data;
using Admin.EntityDAO;
using Admin.EntityDefinitions;
using Common;

namespace FNDZWeb.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            AUserData dsAUserData = AUserRule.GetUserDataByCode(LoginInfo.UserCode);
            if (dsAUserData.AUser.Rows.Count > 0)
            {
                //string strSchoolName = "";
                //if (!string.IsNullOrEmpty(dsAUserData.AUser[0].SchoolID))
                //{
                //    FNDZ_MSchool dsFNDZ_MSchool = BLL_MasterManagement.GetMSchoolInfo(dsFNDZ_AUser.SchoolID);
                //    if (dsFNDZ_MSchool != null) strSchoolName = dsFNDZ_MSchool.SchoolName;
                //}

                ViewBag.UserCode = dsAUserData.AUser[0].UserCode;
                //ViewBag.UserCode = dsFNDZ_AUser.UserCode + "[" + dsFNDZ_AUser.UserName + ":" + strSchoolName + "]";
            }
            return View();
        }

        public ActionResult DefaultShow()
        {
            ViewBag.BagUserCode = LoginInfo.UserCode;
            return View();
        }

        [AllowAnonymousAttribute]
        public ActionResult Login()
        {
            if (Request.Cookies["FNDZUser"] != null)
            {
                ViewBag.BagUserCode = Request.Cookies["FNDZUser"].Values["User"];
                ViewBag.BagUserPWD = Request.Cookies["FNDZUser"].Values["PWD"];
            }
            else
            {
                ViewBag.BagUserCode = "";
                ViewBag.BagUserPWD = "";
            }
            return View();
        }

        public ContentResult SetCustomer(string CustomerCode)
        {
            try
            {
                if (Request.Cookies["currCustomerCode"] == null)
                {
                    HttpCookie Cookie = new HttpCookie("currCustomerCode");
                    Cookie.Expires = DateTime.Now.AddDays(30);
                    Cookie.Value = CustomerCode;
                    System.Web.HttpContext.Current.Response.Cookies.Add(Cookie);
                }
                else
                {
                    Request.Cookies["currCustomerCode"].Value = CustomerCode;
                    System.Web.HttpContext.Current.Response.Cookies.Set(Request.Cookies["currCustomerCode"]);
                }
                FormsAuthentication.SetAuthCookie(LoginInfo.UserCode + "," + ((Request.Cookies["FNDZUser"]==null)?"":Request.Cookies["FNDZUser"].Value) + "," + CustomerCode, true);
                return Content("ok");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(string UserCode)
        {
            AUserData dsAUserData = AUserRule.GetUserDataByCode(UserCode);
            string password = Request["Password"];
            string chklogrem = Request["logrem"];
            if (dsAUserData == null || dsAUserData.AUser.Rows.Count <= 0 || dsAUserData.AUser[0].Password != PasswordLocker.DESEncrypt(password) || dsAUserData.AUser[0].Active == 1)
            {
                return View();
            }
            else
            {
                //string rememberme = Request["remembermepassword"];
                string GroupCoCode = System.Configuration.ConfigurationManager.AppSettings["DefaultGroupCoCode"];
                if (Request.Cookies["FNDZUser"] == null && chklogrem=="on")
                {
                    HttpCookie MyCookie = new HttpCookie("FNDZUser");
                    DateTime now = DateTime.Now;
                    MyCookie.Value = UserCode;
                    MyCookie.Values.Add("User", UserCode);
                    MyCookie.Values.Add("PWD", password);
                    MyCookie.Expires = now.AddYears(100);
                    Response.Cookies.Add(MyCookie);
                    Response.Cookies["FNDZUser"].Expires = now.AddYears(100);
                    
                }
                else if (Request.Cookies["FNDZUser"] != null && chklogrem==null)
                    Response.Cookies["FNDZUser"].Expires = DateTime.Now.AddYears(-1);

                FormsAuthentication.SetAuthCookie(UserCode + "," + ((Request.Cookies["FNDZUser"]==null)?"":Request.Cookies["FNDZUser"].Value)+",", true);

                AUserTraceRule.AddAUserTraceData(UserCode.Trim().ToUpper(), "登陆", "", "Login", Request.UserHostAddress);
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult JSonMenu() {
            return Json(GetMenu(),JsonRequestBehavior.AllowGet); }

        StringBuilder result = new StringBuilder();
        StringBuilder sb = new StringBuilder(); 
        public string GetMenu()        {
            Dictionary<string, List<string>> dRoleFunction = new Dictionary<string, List<string>>();
            DataTable dt = ARoleFunctionRule.GetRoleFunctionByUser(LoginInfo.UserCode);
            string i60 = "\"menuid\": \"60\", \"menuname\": \"系统管理\",\"menus\": [ ";
            string i50 = "\"menuid\": \"50\", \"menuname\": \"基础资料\",\"menus\": [ ";
            string i40 = "\"menuid\": \"40\", \"menuname\": \"实时监控\",\"menus\": [ ";

            string il = "";
            foreach (DataRow row in dt.Rows)
            {
                il = "";
                switch (row["FunctionCode"].ToString())
                {
                    #region 系统管理
                    case "A0601":
                        il = "{ \"menuid\": \"61\", \"menuname\": \"客户管理\", \"url\": \"../SystemManage/ACustomer\" }";
                        SetMenusInfo(ref dRoleFunction, i60, il);
                        break;
                    case "A0101":
                        il = "{ \"menuid\": \"62\", \"menuname\": \"用户管理\", \"url\": \"../SystemManage/AUser\" }";
                        SetMenusInfo(ref dRoleFunction, i60, il);
                        break;
                    //case "A0201":
                    //    il = "{ \"menuid\": \"63\", \"menuname\": \"用户角色\", \"url\": \"../SystemManage/AUserRole\" }";
                    //    SetMenusInfo(ref dRoleFunction, i60, il);
                    //    break;
                    case "A0301":
                        il = "{ \"menuid\": \"64\", \"menuname\": \"角色管理\", \"url\": \"../SystemManage/ARole\" }";
                        SetMenusInfo(ref dRoleFunction, i60, il);
                        break;
                    case "A0401":
                        il = "{ \"menuid\": \"65\", \"menuname\": \"角色功能\", \"url\": \"../SystemManage/ARoleFunction\" }";
                        SetMenusInfo(ref dRoleFunction, i60, il);
                        break;
                    case "A0501":
                        il = "{ \"menuid\": \"66\", \"menuname\": \"操作日志\", \"url\": \"../SystemManage/AUserTrace\" }";
                        SetMenusInfo(ref dRoleFunction, i60, il);
                        break;
                    #endregion

                    #region 基础信息
                    case "M0101":
                        il = "{ \"menuid\": \"51\", \"menuname\": \"公司管理\", \"url\": \"../MasterManage/MCompany\" }";
                        SetMenusInfo(ref dRoleFunction, i50, il);
                        break;
                    case "M0201":
                        il = "{ \"menuid\": \"52\", \"menuname\": \"设备管理\", \"url\": \"../MasterManage/MEquipment\" }";
                        SetMenusInfo(ref dRoleFunction, i50, il);
                        break;
                    case "M0301":
                        il = "{ \"menuid\": \"53\", \"menuname\": \"楼层管理\", \"url\": \"../MasterManage/MFloor\" }";
                        SetMenusInfo(ref dRoleFunction, i50, il);
                        break;
                    case "M0401":
                        il = "{ \"menuid\": \"54\", \"menuname\": \"故障管理\", \"url\": \"../MasterManage/MFaultCodes\" }";
                        SetMenusInfo(ref dRoleFunction, i50, il);
                        break;
                    #endregion

                    #region 实时监控管理
                    case "B0101":
                        il = "{ \"menuid\": \"41\", \"menuname\": \"电梯\", \"url\": \"../BusinessManage/test?EquipmentType=1\" }";
                        SetMenusInfo(ref dRoleFunction, i40, il);
                        break;
                    case "B0201":
                        il = "{ \"menuid\": \"42\", \"menuname\": \"扶梯\", \"url\": \"../BusinessManage/test?EquipmentType=2\" }";
                        SetMenusInfo(ref dRoleFunction, i40, il);
                        break;
                    case "B0301":
                        il = "{ \"menuid\": \"42\", \"menuname\": \"人行道\", \"url\": \"../BusinessManage/test?EquipmentType=3\" }";
                        SetMenusInfo(ref dRoleFunction, i40, il);
                        break;
                    #endregion

                }
            }


            result.Append("{\"menus\":" + " [");
            int i = 0;
            int ip = 0;
            foreach (string skey in dRoleFunction.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value).Keys)
            {
                ip++;
                result.Append("{");
                result.Append(skey);
                i = 0;
                foreach (string li in dRoleFunction[skey])
                {
                    i++;
                    result.Append(li + ((i < dRoleFunction[skey].Count) ? "," : ""));
                }
                result.Append("]}" + ((ip < dRoleFunction.Count) ? "," : ""));
            }
            result.Append("] }");
            if (ip == 0) result.Clear();
            return result.ToString();
        }

        private void SetMenusInfo(ref Dictionary<string, List<string>> dRoleFunction
            ,string sKey,string sCon)
        {
            if (dRoleFunction.ContainsKey(sKey))
                dRoleFunction[sKey].Add(sCon);
            else
            {
                List<string> l = new List<string>();
                l.Add(sCon);
                dRoleFunction.Add(sKey, l);
            }
        }      

            

        public ContentResult Logout()
        {
            try
            {
                FormsAuthentication.SignOut();
                return Content("ok");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public ContentResult ModifyPass(string currPass,string newPass)
        {
            AUserData dsAUserData = AUserRule.GetUserDataByCode(LoginInfo.UserCode);
            if (dsAUserData.AUser.Rows.Count <= 0 || dsAUserData.AUser[0].Password != PasswordLocker.DESEncrypt(currPass))
            {
                return Content("当前密码不正确，请确认");
            }
            else
            {
                dsAUserData.AUser[0].Password = PasswordLocker.DESEncrypt(newPass);
                AUserRule.UpdateData(dsAUserData);
                AUserTraceRule.AddAUserTraceData(LoginInfo.UserCode, "修改密码", LoginInfo.UserCode, "主界面", Request.UserHostAddress);

                return Content("ok");
            }
        }



        /// <summary>
        /// 验证码的校验
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public ContentResult CheckCode()
        {
            //生成验证码
            Common.ValidateCode validateCode = new Common.ValidateCode();
            string code = validateCode.CreateValidateCode(4);
            Session["ValidateCode"] = code;
            byte[] bytes = validateCode.CreateValidateGraphic(code);
            return Content(File(bytes, @"Images/CheckCode").ToString());
        }
        
    }
}
