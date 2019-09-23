using ClassLib4Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.ServerPreviewUrlPrefix = ConfigHelper.GetAppSetting("ServerPreviewUrlPrefix");

            return View();
        }

        #region xview
        /*
        请求预览地址（预转换地址）：
        http://192.168.10.250:8237/v1/view
        在线预览页面（最终预览效果）：
        http://192.168.10.250:8237/v1/view/preview

        示例：
        http://10.13.83.54:8237/v1/view/preview?fname=111.xls&sheetIndex=0
        http://10.13.83.54:8237/v1/view/preview?fname=111.doc&pageIndex=0
        http://10.13.83.54:8237/v1/view/preview?fname=111.ppt&pageIndex=2


        某文件转换状态：
        http://192.168.10.250:8237/v1/view/status

        数据采集接口：
        http://192.168.10.250:8237/v1/view/workstatus?starttime=2019-01-01&endtime=2020-01-01

        
         */

        #endregion

        #region xconvert 已弃用
        /// <summary>
        /// 文档转PDF
        /// </summary>
        /// <returns></returns>
        [Obsolete("已弃用")]
        public ActionResult convertfileDemo()
        {
            Models.convertfileModel model = new Models.convertfileModel();
            model.name = "20181220A.txt";
            model.url = "http://xxh.cn/wpspreview/Content/20181220A.txt";
            model.pass = "";
            model.callback = "http://xxh.cn/wpspreview/Home/callbackForConvertfile";

            string result = Models.HttpHelper.HttpPost("http://10.13.83.54:20883/v1/convertfile", model.ToJson(), "application/json");
            LogHelper.Save(string.Format("convertfileDemo > result={0}", result), nameof(HomeController), LogType.Report, LogTime.hour);

            return Content(result);
        }
        
        /// <summary>
        /// http://xxh.cn/wpspreview/Home/callbackForConvertfile
        /// 
        /// http://192.168.10.250:20883/v1/file?sha1=3266d3b7ffb37a5b130c84644b489baee940a36e
        /// </summary>
        /// <returns></returns>
        [Obsolete("已弃用")]
        public ActionResult callbackForConvertfile()
        {
            var request = System.Web.HttpContext.Current.Request;
            LogHelper.Save(string.Format("callbackForConvertfile > Url={0}，ip={1}，HttpMethod={2}，ContentType={3}", request.Url, SystemInfo.GetIP(), request.HttpMethod, request.ContentType), nameof(HomeController), LogType.Report, LogTime.hour);
            // callbackForConvertfile > Url = http://xxh.cn/wpspreview/Home/callbackForConvertfile，ip=192.168.10.250，HttpMethod=POST，ContentType=application/x-www-form-urlencoded
            if(request.HttpMethod.ToLower() == "get")
            {
            }
            else if(request.HttpMethod.ToLower() == "post")
            {
                System.IO.Stream s = request.InputStream;
                byte[] b = new byte[s.Length];
                s.Read(b, 0, (int)s.Length);
                string postStr = System.Text.Encoding.UTF8.GetString(b);
                if(!string.IsNullOrWhiteSpace(postStr))
                {
                    LogHelper.Save(string.Format("callbackForConvertfile > Url={0}，ip={1}，HttpMethod={2}，ContentType={3}，postStr={4}", request.Url, SystemInfo.GetIP(), request.HttpMethod, request.ContentType, postStr), nameof(HomeController), LogType.Report, LogTime.hour);
                    // callbackForConvertfile > Url=http://xxh.cn/wpspreview/Home/callbackForConvertfile，ip=192.168.10.250，HttpMethod=POST，ContentType=application/x-www-form-urlencoded，postStr={"resultCode":0,"sha1":"3266d3b7ffb37a5b130c84644b489baee940a36e","scode":0,"dur":825}
                }
            }
            else
            {
            }

            return Json(new { resultCode = 0, msg = "ok" });
        }
        #endregion

    }
}