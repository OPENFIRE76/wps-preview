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

        /// <summary>
        /// demo演示根据文件名调用转换PDF
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /*
         * createsession只是用来上传文件，获取session-id的，实际要执行什么操作，是要调用operate接口，获取结果是用content接口。
         * 不同的操作之前可以用monitor 接口获取当前session的状态。
         * 可根据状态来判断是否可以进行下一步操作。
         */

        /// <summary>
        /// 第一步，创建session获取sessionId
        /// </summary>
        /// <param name="fname"></param>
        /// <returns></returns>
        public ActionResult CreateSession(string fname)
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

            // 示例：http://10.13.83.54:8260/cps/sessions?file_extension=.txt
            // {"id":"officed-n1-de5735e6-5010-4c4a-a395-064f39d59be4"}
            string result = Models.HttpHelper.HttpPost(api, json, "application/json"); //这里demo采用的是Content-Type=application/json，否则需要将文档流传入。
            LogHelper.Save(string.Format("CreateSession > result={0}", result), nameof(officeDController), LogType.Report, LogTime.hour);
            Models.officeDsession session = JsonHelper.DeSerialize<Models.officeDsession>(result);
            if(null != session && session.id != "")
            {
                OperateSession(session.id);
            }

            return Content(result);
        }

        /// <summary>
        /// 第二步，根据sessionId执行操作
        /// </summary>
        /// <param name="SessionId"></param>
        private void OperateSession(string SessionId)
        {
            if(null == SessionId || SessionId.Length < 36)
            {
                throw new ArgumentException("参数SessionId是无效的值");
            }

            // 接口地址
            string api = ConfigHelper.GetAppSetting("ServerOfficeDUrlPrefix") + "/api/v1/cps/sessions/{session-id}/operate";
            api = api.Replace("{session-id}", SessionId);
            //body 参数
            Models.operateSteps data = new Models.operateSteps();
            data.steps = new List<Models.operateStepItem>() {
                 new Models.operateStepItem() { operate="convertToPDF", args= new Dictionary<string, string>() }
            }.ToArray();

            // 示例：http://10.13.83.54:8260/api/v1/cps/sessions/officed-n1-bac3be88-ac5c-4101-a2c1-eb9d7f1cb127/operate
            // {"steps":[{"operate":"convertToPDF","args":{}}]}
            // 返回空字符串

            string result = Models.HttpHelper.HttpPost(api, data.ToJson(), "application/json");
            LogHelper.Save(string.Format("OperateSession > result={0}", result), nameof(officeDController), LogType.Report, LogTime.hour);
            if(null != result && "" != result)
            {
                Models.ResultLocation resultLocation = JsonHelper.DeSerialize<Models.ResultLocation>(result);
                if(null != resultLocation && null != resultLocation.location && resultLocation.location.address != "")
                {
                    //执行操作返回结果
                    LogHelper.Save(string.Format("OperateSession >执行操作返回结果 resultLocation.location.address={0}", resultLocation.location.address), nameof(officeDController), LogType.Report, LogTime.hour);
                }
            }

            GetContentSession(SessionId);
        }

        /// <summary>
        /// 第三步，根据SessionId获取转换结果
        /// </summary>
        /// <param name="SessionId"></param>
        private void GetContentSession(string SessionId)
        {
            if(null == SessionId || SessionId.Length < 36)
            {
                throw new ArgumentException("参数SessionId是无效的值");
            }

            // 接口地址
            string api = ConfigHelper.GetAppSetting("ServerOfficeDUrlPrefix") + "/api/v1/cps/sessions/{session-id}/content?location_address={location_address}";
            api = api.Replace("{session-id}", SessionId);
            // 文档保存位置
            string address = "file:///C:/wps/data/download"; //location_address为空时，返回文件临时下载地址
            api = api.Replace("{location_address}", address);

            // 示例：http://10.13.83.54:8260/api/v1/cps/sessions/officed-n1-de5735e6-5010-4c4a-a395-064f39d59be4/content?location_address=
            // {"location":{"address":"file:///C:/wps/data/download/officed-n1-bac3be88-ac5c-4101-a2c1-eb9d7f1cb127.pdf","extra":null}}
            string result = Models.HttpHelper.HttpGet(api);
            LogHelper.Save(string.Format("GetContentSession > result={0}", result), nameof(officeDController), LogType.Report, LogTime.hour);
            Models.ResultLocation resultLocation = JsonHelper.DeSerialize<Models.ResultLocation>(result);
            if(null != resultLocation && null != resultLocation.location && resultLocation.location.address != "")
            {
                //转换结果
                LogHelper.Save(string.Format("GetContentSession >转换完成 resultLocation.location.address={0}", resultLocation.location.address), nameof(officeDController), LogType.Report, LogTime.hour);
            }

        }




    }
}