using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{

    /// <summary>
    /// 获取文件信息
    /// </summary>
    public class fileinfo
    {
        /// <summary>
        /// 200表示成功，不为200时必须返回对应的错误信息
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 错误信息，可以给web显示的	
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 错误信息，一个json数据，如果是错误，预览将把错误信息全部返回给前端，便于定位对接模块错误范围
        /// </summary>
        public object detail { get; set; }


        /// <summary>
        /// 文档唯一id，建议用文档sha1，预览服务通过这个id来判断预览文件是否已经生成。必须返回。
        /// </summary>
        public string uniqueId { get; set; }
        /// <summary>
        /// 文件的名字，包含后缀，预览服务根据文档名的后缀来判断是否支持此文档的预览。必须返回。
        /// </summary>
        public string fname { get; set; }
        /// <summary>
        /// 获取文档的方式，目前支持三种：localfile, localfilewait, download。必须返回。
        /// </summary>
        public string getFileWay { get; set; }
        /// <summary>
        /// 当获取方式为localfile、download时，表示文档的本地路径或下载地址
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 是否可复制，默认为true
        /// </summary>
        public bool enableCopy { get; set; }
        /// <summary>
        /// 是否有水印，0:无水印 1：字符串水印  2：图片水印
        /// </summary>
        public int watermarkType { get; set; }
        /// <summary>
        /// watermarkType为1：水印字符串  2：图片的url地址
        /// </summary>
        public string watermark { get; set; }
        /// <summary>
        /// json结构体，水印相关设置，可选
        /// </summary>
        public WatermarkSetting watermarksetting { get; set; }


        /// <summary>
        /// 构造函数设置默认值
        /// </summary>
        public fileinfo()
        {
            code = 200;
            msg = "ok";
            detail = null;

            enableCopy = true;

            getFileWay = "download";
            watermarkType = 1;
            watermark = "金山WPS在线预览";
        }

        /// <summary>
        /// 序列化成Json字符串
        /// </summary>
        /// <returns></returns>
        public bool ValidateEntity()
        {
            if(200 != code && string.IsNullOrWhiteSpace(msg))
            {
                throw new Exception("当code不为200时，必须返回对应的错误信息");
            }
            if(string.IsNullOrWhiteSpace(fname) || !System.IO.Path.HasExtension(fname))
            {
                throw new Exception("文件的名字包含后缀，预览服务根据文档名的后缀来判断是否支持此文档的预览。必须返回。");
            }
            if(string.IsNullOrWhiteSpace(getFileWay) || !("localfile" == getFileWay || "localfilewait" == getFileWay || "download" == getFileWay))
            {
                throw new Exception("获取文档的方式，目前支持三种：localfile, localfilewait, download。必须返回。");
            }
            if("localfile" == getFileWay && (string.IsNullOrWhiteSpace(url) || url.ToLower().IndexOf(":\\") != 1))
            {
                throw new Exception("当获取方式为localfile、download时，表示文档的本地路径或下载地址");
            }
            else if("download" == getFileWay && (string.IsNullOrWhiteSpace(url) || (url.ToLower().IndexOf("http://") != 0 && url.ToLower().IndexOf("https://") != 0)))
            {
                throw new Exception("当获取方式为localfile、download时，表示文档的本地路径或下载地址");
            }
            if(!(0 == watermarkType || 1 == watermarkType || 2 == watermarkType))
            {
                throw new Exception("是否有水印，0:无水印 1：字符串水印  2：图片水印");
            }
            else if(2 == watermarkType && (string.IsNullOrWhiteSpace(watermark) || (watermark.ToLower().IndexOf("http://") != 0 && watermark.ToLower().IndexOf("https://") != 0)))
            {
                throw new Exception("当 watermarkType是2时，watermark是图片的url地址");
            }

            return true;
        }
    }

    /// <summary>
    /// 水印相关设置
    /// </summary>
    public struct WatermarkSetting
    {
        /// <summary>
        /// 单个水印的宽度，默认值：208
        /// </summary>
        public int width { get; set; }
        /// <summary>
        /// 单水印的高度，默认值：177
        /// </summary>
        public int height { get; set; }
        /// <summary>
        /// 水印的透明度，默认值：rgba(192,192,192,0.0)
        /// </summary>
        public string fillstyle { get; set; }
        /// <summary>
        /// 水印的字体,默认值:bold 20px Serif
        /// </summary>
        public string font { get; set; }
        /// <summary>
        /// 水印的旋转度，默认值：-Math.PI / 4
        /// </summary>
        public float rotate { get; set; }
    }


    public class notify
    {
        /// <summary>
        /// 通知类型：viewNotify，convertNotify，downloadFileNotify
        /// </summary>
        public string cmd { get; set; }
        /// <summary>
        /// 通知内容（根据cmd不同，data返回内容不同）
        /// </summary>
        public notifyData data { get; set; }

        public notify()
        {
            data = new notifyData();
        }
    }

    public struct notifyData
    {
        /// <summary>
        /// 持续时间（毫秒）
        /// </summary>
        public int duration { get; set; }
        /// <summary>
        /// 文本消息
        /// </summary>
        public string error { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string fname { get; set; }
        /// <summary>
        /// 文件唯一标识
        /// </summary>
        public string uniqueId { get; set; }

        /// <summary>
        /// 文件源url（可能是网络下载地址或物理地址）
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 文档识别转换第一页耗时（毫秒）
        /// </summary>
        public int fp1CostTime { get; set; }
        /// <summary>
        /// 文档识别转换错误码（正常时为0）
        /// </summary>
        public int resultCode { get; set; }
        /// <summary>
        /// 文档识别转换结果码（正常时为0）
        /// </summary>
        public int scode { get; set; }
        /// <summary>
        /// 文档识别转换完成总耗时（毫秒）
        /// </summary>
        public int totalTime { get; set; }
    }

}