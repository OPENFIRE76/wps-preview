using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;


namespace WebApplication1.Models
{
    public class HttpHelper
    {

        #region MyRegion

        /// <summary>
        /// 发送GET请求
        /// </summary>
        /// <param name="url">请求URL，如果需要传参，在URL末尾加上“？+参数名=参数值”即可</param>
        /// <returns></returns>
        public static string HttpGet(string url)
        {
            //创建
            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            httpWebRequest.Proxy = null;
            httpWebRequest.KeepAlive = false;
            //ContentType
            //httpWebRequest.ContentType = "text/html;charset=UTF-8";
            httpWebRequest.ContentType = "application/json;charset=UTF-8";
            httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;
            //设置请求方法
            httpWebRequest.Method = "GET";
            //请求超时时间
            httpWebRequest.Timeout = 20000;
            //发送请求
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            //利用Stream流读取返回数据
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8);
            //获得最终数据，一般是json
            string responseContent = streamReader.ReadToEnd();
            streamReader.Close();
            streamReader.Dispose();
            httpWebResponse.Close();

            if(httpWebRequest != null)
            {
                httpWebRequest.Abort();
            }

            return responseContent;
        }
        /// <summary>
        /// 发送POST请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="ContentType"></param>
        /// <returns></returns>
        public static string HttpPost(string url, string data, string contentType = "application/x-www-form-urlencoded")
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            //字符串转换为字节码
            byte[] bs = Encoding.UTF8.GetBytes(data);
            //参数类型，这里是json类型
            //还有别的类型如"application/x-www-form-urlencoded"，不过我没用过(逃
            //httpWebRequest.ContentType = "application/json";
            httpWebRequest.ContentType = contentType;
            //参数数据长度
            httpWebRequest.ContentLength = bs.Length;
            //设置请求类型
            httpWebRequest.Method = "POST";
            //设置超时时间
            httpWebRequest.Timeout = 20000;
            //将参数写入请求地址中
            httpWebRequest.GetRequestStream().Write(bs, 0, bs.Length);
            //发送请求
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            //读取返回数据
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8);
            string responseContent = streamReader.ReadToEnd();
            streamReader.Close();
            streamReader.Dispose();
            httpWebResponse.Close();
            if(httpWebRequest != null)
            {
                httpWebRequest.Abort();
            }

            return responseContent;
        }

        /*
然后引用命名空间即可
using System.Net;
using System.IO;

using System.Security.Cryptography;


Stream s = System.Web.HttpContext.Current.Request.InputStream;
byte[] b = new byte[s.Length];
s.Read(b, 0, (int)s.Length);
return Encoding.UTF8.GetString(b);
————————————————
         */
        #endregion
    }
}