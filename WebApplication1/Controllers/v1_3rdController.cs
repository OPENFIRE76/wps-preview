using ClassLib4Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    /// <summary>
    /// 客户要实现的对接模块（即2个接口）
    /// 路由：/v1/3rd/
    /// </summary>
    public class v1_3rdController : Controller
    {
        // GET: v1_3rd

        /*
         * 需要提前将对接模块地址头部配置到预览服务器
         * http://xxh.cn/wpspreview
         */

        /// <summary>
        /// 获取文件信息接口
        /// 调用方法：GET
        /// 示例：http://xxh.cn/wpspreview/v1/3rd/fileinfo?fname=20181220A.txt
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Name = "3rd")]
        public ActionResult fileinfo()
        {
            var request = System.Web.HttpContext.Current.Request;
            LogHelper.Save(string.Format("fileinfo > Url={0}，ip={1}，HttpMethod={2}，ContentType={3}", request.Url, SystemInfo.GetIP(), request.HttpMethod, request.ContentType), nameof(HomeController), LogType.Report, LogTime.hour);
            // fileinfo > Url=http://xxh.cn/wpspreview/v1/3rd/fileinfo?fname=20181220A.txt&operationtype=preview，ip=192.168.10.250，HttpMethod=GET，ContentType=

            var fileinfo = new Models.fileinfo();
            if(request.HttpMethod.ToLower() == "get")
            {
                try
                {
                    string signature = request["signature"];
                    string appid = request["appid"];
                    // 其他自定义参数……

                    string fname = request["fname"];
                    string fpath = AppDomain.CurrentDomain.BaseDirectory + "\\Content\\" + fname;

                    /*
                    fileinfo.uniqueId = "3266d3b7ffb37a5b130c84644b489baee940a36e";
                    fileinfo.fname = "20181220A.txt";
                    fileinfo.getFileWay = "download";
                    fileinfo.url = "http://xxh.cn/wpspreview/Content/20181220A.txt";
                    */
                    fileinfo.uniqueId = Models.FileHelper.GetSHA1(fpath);
                    fileinfo.fname = fname;
                    fileinfo.getFileWay = "download"; // 本demo仅支持：localfile, download
                    fileinfo.url = "http://xxh.cn/wpspreview/Content/" + fname;
                    fileinfo.watermarksetting = new Models.WatermarkSetting()
                    {
                        width = 0,
                        height = 0,
                        fillstyle = "rgba(255,0,0,0.6)",
                        font = "bold 16px Serif",
                        rotate = (float)(-Math.PI / 4)
                    };
                    fileinfo.detail = new { };
                    fileinfo.code = 200;
                    fileinfo.msg = "ok";

                    fileinfo.ValidateEntity();
                }
                catch(Exception ex)
                {
                    LogHelper.Save(ex, string.Format("fileinfo > Url={0}，ip={1}，HttpMethod={2}，ContentType={3}", request.Url, SystemInfo.GetIP(), request.HttpMethod, request.ContentType), nameof(HomeController), LogTime.hour);
                    //throw;
                }
            }
            else if(request.HttpMethod.ToLower() == "post")
            {
                // 不会有post请求
            }
            else
            {
                // 不会有其它请求
            }

            return Json(fileinfo, JsonRequestBehavior.AllowGet);
        }

        #region /v1/3rd/fileinfo 返回json实例：
        /*
        {
            "code": 200,
            "msg": "ok",
            "detail": {},
            "uniqueId": "3266d3b7ffb37a5b130c84644b489baee940a36e",
            "fname": "20181220A.txt",
            "getFileWay": "download",
            "url": "http://xxh.cn/wpspreview/Content/20181220A.txt",
            "enableCopy": true,
            "watermarkType": 1,
            "watermark": "金山WPS在线预览",
            "watermarksetting": {
                "width": 0,
                "height": 0,
                "fillstyle": "rgba(255,0,0,0.6)",
                "font": "bold 16px Serif",
                "rotate": -0.7853982
            }
        } 
        */
        #endregion

        /// <summary>
        /// 消息通知回调
        /// 调用方法：POST
        /// 示例：http://xxh.cn/wpspreview/v1/3rd/notify
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route(Name = "3rd")]
        public ActionResult notify()
        {
            var request = System.Web.HttpContext.Current.Request;
            LogHelper.Save(string.Format("notify > Url={0}，ip={1}，HttpMethod={2}，ContentType={3}", request.Url, SystemInfo.GetIP(), request.HttpMethod, request.ContentType), nameof(HomeController), LogType.Report, LogTime.hour);
            // notify > Url=http://xxh.cn/wpspreview/v1/3rd/notify?operationtype=preview&sha1=3266d3b7ffb37a5b130c84644b489baee940a36e，ip=192.168.10.250，HttpMethod=POST，ContentType=application/json
            if(request.HttpMethod.ToLower() == "post")
            {
                System.IO.Stream s = request.InputStream;
                byte[] b = new byte[s.Length];
                s.Read(b, 0, (int)s.Length);
                string postStr = System.Text.Encoding.UTF8.GetString(b);
                if(!string.IsNullOrWhiteSpace(postStr))
                {
                    LogHelper.Save(string.Format("notify > Url={0}，ip={1}，HttpMethod={2}，ContentType={3}，postStr={4}", request.Url, SystemInfo.GetIP(), request.HttpMethod, request.ContentType, postStr), nameof(HomeController), LogType.Report, LogTime.hour);
                    //// 三种类型通知实例如下：
                    // notify > Url=http://xxh.cn/wpspreview/v1/3rd/notify?fname=20181220A.txt&operationtype=preview，ip=192.168.10.250，HttpMethod=POST，ContentType=application/json，postStr={"cmd":"viewNotify","data":{"fname":"20181220A.txt","uniqueId":"3266d3b7ffb37a5b130c84644b489baee940a36e"}}
                    // notify > Url=http://xxh.cn/wpspreview/v1/3rd/notify，ip=192.168.10.250，HttpMethod=POST，ContentType=application/json，postStr={"cmd":"downloadFileNotify","data":{"duration":9,"fname":"20181220A.txt","uniqueId":"3266d3b7ffb37a5b130c84644b489baee940a36e","url":"http://xxh.cn/wpspreview/Content/20181220A.txt"}}
                    // notify > Url=http://xxh.cn/wpspreview/v1/3rd/notify，ip=192.168.10.250，HttpMethod=POST，ContentType=application/json，postStr={"cmd":"convertNotify","data":{"duration":774,"error":"","fname":"20181220A.txt","fp1CostTime":250,"resultCode":0,"scode":0,"totalTime":776,"uniqueId":"3266d3b7ffb37a5b130c84644b489baee940a36e"}}

                    Models.notify notify = JsonHelper.DeSerialize<Models.notify>(postStr);
                    if(postStr.Contains("viewNotify"))
                    {
                        // 文档预览通知，可以做客户系统自定义处理。例如写入数据库，方便查询、统计。
                        LogHelper.Save(string.Format("文档预览通知：notify > postStr={0}", postStr), nameof(HomeController), LogType.Report, LogTime.hour);
                        //notify.data.uniqueId ……

                    }
                    else if(postStr.Contains("downloadFileNotify"))
                    {
                        // 文档下载通知（下载类型，且新文档或旧文档预览缓存失效时），可以做客户系统自定义处理。例如写入数据库，方便查询、统计。
                        LogHelper.Save(string.Format("文档下载通知：notify > postStr={0}", postStr), nameof(HomeController), LogType.Report, LogTime.hour);
                        //notify.data.uniqueId ……

                    }
                    else if(postStr.Contains("convertNotify"))
                    {
                        // 文档转换通知（新文档或旧文档预览缓存失效时），可以做客户系统自定义处理。例如写入数据库，方便查询、统计。
                        LogHelper.Save(string.Format("文档转换通知：notify > postStr={0}", postStr), nameof(HomeController), LogType.Report, LogTime.hour);
                        //notify.data.uniqueId ……

                    }
                    else
                    {
                    }

                }
            }
            else if(request.HttpMethod.ToLower() == "get")
            {
                // 不会有get请求
            }
            else
            {
                // 不会有其它请求
            }

            return Json(new { resultCode = 0, msg = "ok" }, JsonRequestBehavior.DenyGet);
        }


    }
}