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
            ViewBag.ServerUrlPrefix = ConfigHelper.GetAppSetting("ServerUrlPrefix");

            return View();
        }

        #region xview
        /*
        请求预览地址（预转换地址）：
        http://192.168.10.250:8237/v1/view
        某文件转换状态：
        http://192.168.10.250:8237/v1/view/status
        在线预览页面（最终预览效果）：
        http://192.168.10.250:8237/v1/view/preview
         */

        #endregion

        #region xconvert 已废弃
        /// <summary>
        /// http://xxh.cn/wpspreview/Home/callbackForConvertfile
        /// 
        /// http://192.168.10.250:20883/v1/file?sha1=3266d3b7ffb37a5b130c84644b489baee940a36e
        /// </summary>
        /// <returns></returns>
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