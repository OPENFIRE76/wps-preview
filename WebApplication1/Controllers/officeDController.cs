using ClassLib4Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    /// <summary>
    /// 3.6.100以上才支持officeD，原来的xconvert将不在支持。
    /// </summary>
    public class officeDController : Controller
    {
        // GET: officeD
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// demo演示根据文件名调用转换PDF
        /// </summary>
        /// <param name="fname"></param>
        /// <returns></returns>
        public ActionResult convertfileDemo(string fname)
        {
            // 接口地址
            string api = ConfigHelper.GetAppSetting("ServerOfficeDUrlPrefix") + "/api/v1/cps/sessions?";
            api += "file_extension=" + System.IO.Path.GetExtension(fname);
            //api += "force_file_handler=wps"; // 可选参数

            // body参数
            string json = "{\"location\":{\"address\":\"[address]\",\"extra\":{}}}";

            //文档地址
            string address = "http://xxh.cn/wpspreview/Content/" + fname;
            json = json.Replace("[address]", address);

            string result = Models.HttpHelper.SendHttpRequest(api, "POST", json); //这里demo采用的是Content-Type=application/json，否则需要将文档流传入。
            LogHelper.Save(string.Format("convertfileDemo > result={0}", result), nameof(officeDController), LogType.Report, LogTime.hour);
            Models.officeDsession session = JsonHelper.DeSerialize<Models.officeDsession>(result);
            if(null != session && session.id != "")
            {
                getConvertFileBySessionId(session.id);
            }

            return Content(result);
        }

        /// <summary>
        /// 根据SessionId获取转换结果
        /// </summary>
        /// <param name="SessionId"></param>
        private void getConvertFileBySessionId(string SessionId)
        {
            // 接口地址
            string api = ConfigHelper.GetAppSetting("ServerOfficeDUrlPrefix") + "/api/v1/cps/sessions/{session-id}/content?location_address={location_address}";
            api = api.Replace("{session-id}", SessionId);
            // 文档保存位置
            string address = ""; //location_address为空时，返回文件临时下载地址
            api = api.Replace("{location_address}", address);

            string result = Models.HttpHelper.SendHttpRequest(api, "GET", "");
            LogHelper.Save(string.Format("getConvertFileBySessionId > result={0}", result), nameof(officeDController), LogType.Report, LogTime.hour);
            Models.officeDsession session = JsonHelper.DeSerialize<Models.officeDsession>(result);
            if(null != session && session.id != "")
            {

            }

        }




    }
}