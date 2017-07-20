using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using FNDZWeb.Models;

namespace FNDZWeb.Common
{
    public class StudentInfoHelper
    {
        public static string getStudentInfo = "{0}?{1}&md5String={2}";
        public static string getStudentInfoPara = "functionCode=StudentInfo&platformCode={0}&platformCodeKey={1}&certificateID={2}&courseName={3}";

        //public static StudentInfoResult GetStudentInfo(string urlroot, string platformCode, string platformCodeKey, string certificateID, string courseName)
        //{
        //    string respText = "";
        //    //构造url
        //    string para = string.Format(getStudentInfoPara, platformCode, platformCodeKey, certificateID, courseName);
        //    string paraMD5 = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(para, "MD5");
        //    string url = string.Format(getStudentInfo, urlroot, para, paraMD5);

        //    //获取json数据
        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

        //    using (Stream resStream = response.GetResponseStream())
        //    {
        //        StreamReader reader = new StreamReader(resStream, Encoding.UTF8);
        //        respText = reader.ReadToEnd();
        //        resStream.Close();
        //    }

        //    JavaScriptSerializer Jss = new JavaScriptSerializer();
        //    return Jss.Deserialize<StudentInfoResult>(respText);
        //}

    }
}