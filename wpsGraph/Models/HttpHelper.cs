using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace wpsGraph.Models
{
    public static class HttpHelper
    {
        /// 是否外部数据提交
        public static bool CheckOutWebString()
        {
            string server_v1 = HttpContext.Current.Request.ServerVariables["HTTP_REFERER"]; //获取提交页面
            if(string.IsNullOrEmpty(server_v1)) return true;
            string server_v2 = HttpContext.Current.Request.ServerVariables["SERVER_NAME"];  //获取服务器名          
            return server_v1.Substring(7, server_v2.Length) != server_v2;

        }

        // 去除HTML标记
        public static string ReplaceHtml(string html)
        {
            //删除脚本
            html = Regex.Replace(html, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML
            html = Regex.Replace(html, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"-->", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"<!--.*", "", RegexOptions.IgnoreCase);

            html = Regex.Replace(html, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&middot;", "·", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(rdquo|#8221);", "”", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(ldquo|#8220);", "“", RegexOptions.IgnoreCase);

            html = html.Replace("<", "").Replace(">", "").Replace("\r\n", "");
            return html.Trim();
        }

        //获取request IP
        public static string GetClientIP()
        {
            if(HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null) return HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            if(HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] != null) return HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
            if(HttpContext.Current.Request.UserHostAddress != null) return HttpContext.Current.Request.UserHostAddress;
            return "";
        }

        //获取request IP
        public static string GetIP
        {
            get
            {
                string ipv4 = String.Empty;
                foreach(IPAddress ip in Dns.GetHostAddresses(GetClientIP()))
                {
                    if(ip.AddressFamily.ToString() == "InterNetwork")
                    {
                        ipv4 = ip.ToString();
                        break;
                    }
                }
                if(ipv4 != String.Empty) return ipv4;
                // 原作使用 Dns.GetHostName 方法取回的是 Server 端信息，非 Client 端。
                // 改写为利用 Dns.GetHostEntry 方法，由获取的 IPv6 位址反查 DNS 纪录，
                // 再逐一判断何者为 IPv4 协议，即可转为 IPv4 位址。
                foreach(IPAddress ip in Dns.GetHostEntry(GetClientIP()).AddressList)
                {
                    if(ip.AddressFamily.ToString() == "InterNetwork") return ip.ToString();
                }
                return ipv4;
            }
        }

        //stype="excel","text"
        public static void Http_OutputFile(string sFileName, string SBS, string ChartSet, string stype = "excel")
        {
            string ext = ".xls";
            if(stype == "text") ext = ".txt";
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Charset = ChartSet;
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding(ChartSet);
            string sf = HttpContext.Current.Request.UserAgent.IndexOf("MSIE") > 0 ? "attachment; filename=" + System.Web.HttpUtility.UrlEncode(sFileName + ext, Encoding.GetEncoding("UTF-8")) : "attachment; filename=" + sFileName + ext;
            HttpContext.Current.Response.AddHeader("Content-Disposition", sf);
            HttpContext.Current.Response.ContentType = stype == "excel" ? "application/ms-excel" : "text/plain";
            HttpContext.Current.Response.Write(SBS);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }
        public static void Http_DownFile(string sPath, string ChartSet)
        {
            string FN = System.IO.Path.GetFileName(sPath);
            System.IO.FileStream FS = new System.IO.FileStream(sPath, System.IO.FileMode.Open);
            byte[] bytes = new byte[(int)FS.Length];
            FS.Read(bytes, 0, bytes.Length);
            FS.Close();
            HttpContext.Current.Response.Charset = ChartSet;
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding(ChartSet);
            HttpContext.Current.Response.ContentType = "application/octet-stream";

            string sf = HttpContext.Current.Request.UserAgent.IndexOf("MSIE") > 0 ? "attachment; filename=" + System.Web.HttpUtility.UrlEncode(FN, Encoding.GetEncoding("UTF-8")) : "attachment; filename=" + FN;
            HttpContext.Current.Response.AddHeader("Content-Disposition", sf);
            HttpContext.Current.Response.BinaryWrite(bytes);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }
        public static string VisitUrl(string url, string method, string data)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            request.ContentType = "application/x-www-form-urlencoded";
            if(method.ToLower() == "post" && !string.IsNullOrEmpty(data))
            {
                using(Stream reqStream = request.GetRequestStream())
                {
                    byte[] bs = System.Text.Encoding.UTF8.GetBytes(data);
                    reqStream.Write(bs, 0, bs.Length);
                }
            }
            else
            {
                Uri url_ = new Uri(url);
                if(string.IsNullOrEmpty(url_.Query))
                {
                    request = (HttpWebRequest)WebRequest.Create(url + "?" + data);
                }
                else
                {
                    request = (HttpWebRequest)WebRequest.Create(url + "&" + data);
                }
            }
            try
            {
                using(WebResponse response = request.GetResponse())
                {
                    using(Stream stream = response.GetResponseStream())
                    {
                        using(StreamReader reader = new StreamReader(stream))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                // throw ex; 
                return "";
            }

        }

        public static string VisitUrl(string url, string method, string data, string referUrl, ref CookieContainer cookieContainer, ref string getCookie, string contentType, DateTime headerDate, NameValueCollection headers)
        {
            // string rendValue = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            if(!string.IsNullOrEmpty(referUrl))
            {
                request.Referer = referUrl;

            }
            if(cookieContainer != null && cookieContainer.Count > 0)
            {
                request.CookieContainer = cookieContainer;
            }

            if(method.ToLower() == "get")
            {
                if(!string.IsNullOrWhiteSpace(contentType))
                {
                    request.ContentType = contentType;
                }

                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:43.0) Gecko/20100101 Firefox/43.0";
                request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3");
                request.Headers.Add("Accept-Encoding", "gzip, deflate, sdch");
            }
            else
            {
                request.Method = "POST";
                request.ContentType = !string.IsNullOrWhiteSpace(contentType) ? contentType : "application/x-www-form-urlencoded; charset=UTF-8";

                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:43.0) Gecko/20100101 Firefox/43.0";
                request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3");
                request.Headers.Add("Accept-Encoding", "gzip, deflate");
            }

            request.Date = headerDate;

            if(headers != null && headers.Count > 0)
            {
                for(int i = 0; i < headers.Count; i++)
                {
                    request.Headers.Add(headers.GetKey(i), headers.Get(i));
                }
            }

            if(method.ToLower() == "post" && !string.IsNullOrEmpty(data))
            {
                using(Stream reqStream = request.GetRequestStream())
                {
                    byte[] bs = System.Text.Encoding.UTF8.GetBytes(data);
                    reqStream.Write(bs, 0, bs.Length);
                }
            }
            else if(method.ToLower() == "get" && !string.IsNullOrEmpty(data))
            {
                Uri url_ = new Uri(url);

                if(string.IsNullOrEmpty(url_.Query))
                {
                    request = (HttpWebRequest)WebRequest.Create(url + "?" + data);
                }
                else
                {
                    request = (HttpWebRequest)WebRequest.Create(url + "&" + data);
                }
            }

            try
            {

                using(WebResponse response = request.GetResponse())
                {

                    getCookie = response.Headers["Set-Cookie"];
                    using(Stream stream = response.GetResponseStream())
                    {

                        MemoryStream stmMemory = new MemoryStream();
                        byte[] buffer = new byte[64 * 1024];
                        int i;
                        while((i = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            stmMemory.Write(buffer, 0, i);
                        }
                        byte[] arraryByte = stmMemory.ToArray();
                        stmMemory.Close();
                        return Encoding.UTF8.GetString(arraryByte);
                    }

                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                request.Abort();
            }

        }


        public static string VisitPost(string Url, string data, ref CookieContainer cookieContainer)
        {
            // 设置一些公用的请求头  
            NameValueCollection collection = new NameValueCollection();
            collection.Add("accept-language", "zh-cn,zh;q=0.5");
            collection.Add("accept-encoding", "gzip,deflate");
            collection.Add("accept-charset", "GB2312,utf-8;q=0.7,*;q=0.7");
            collection.Add("cache-control", "max-age=0");
            collection.Add("keep-alive", "115");

            // 来Post数据到登陆页面  
            HttpWebRequest requestLoginToPage = (HttpWebRequest)WebRequest.Create(Url);
            requestLoginToPage.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            requestLoginToPage.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; zh-CN; rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13";
            requestLoginToPage.ContentType = "application/x-www-form-urlencoded";
            requestLoginToPage.Method = "POST";
            requestLoginToPage.Headers.Add(collection);

            requestLoginToPage.CookieContainer = cookieContainer;

            //String data = "phone=15201166786&password=beijing&role_type=2";
            // String data = "phone=15010370230&role_type=2&password=123456&wx_openid=" + openid + "&wx_unionid=" + unionid;
            byte[] bytes = Encoding.ASCII.GetBytes(data);
            requestLoginToPage.ContentLength = bytes.Length;
            Stream streamLoginToPage = requestLoginToPage.GetRequestStream();
            streamLoginToPage.Write(bytes, 0, bytes.Length);
            streamLoginToPage.Flush();
            streamLoginToPage.Close();
            //HttpWebResponse responseLoginToPage = (HttpWebResponse)requestLoginToPage.GetResponse();
            //Console.WriteLine("Post数据结果状态:{0}", responseLoginToPage.StatusCode);
            //return new StreamReader(responseLoginToPage.GetResponseStream(), Encoding.UTF8).ReadToEnd();
            try
            {
                using(HttpWebResponse responseLoginToPage = (HttpWebResponse)requestLoginToPage.GetResponse())
                {
                    using(Stream stream = responseLoginToPage.GetResponseStream())
                    {
                        using(StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                //log.Error("VisitPostOne" + ex.Message);
                return "";
            }
        }
        public static string VisitGet(string Url, CookieContainer cookieContainer, Encoding encoding)
        {
            // 设置一些公用的请求头  
            try
            {
                NameValueCollection collection = new NameValueCollection();
                collection.Add("accept-language", "zh-cn,zh;q=0.5");
                collection.Add("accept-encoding", "gzip,deflate");
                collection.Add("accept-charset", "GB2312,utf-8;q=0.7,*;q=0.7");
                collection.Add("cache-control", "max-age=0");
                collection.Add("keep-alive", "115");

                HttpWebRequest requestResultPage = (HttpWebRequest)WebRequest.Create(Url);
                requestResultPage.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                requestResultPage.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                requestResultPage.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; zh-CN; rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13";
                requestResultPage.Headers.Add(collection);

                requestResultPage.CookieContainer = cookieContainer;

                using(HttpWebResponse responseLoginToPage = (HttpWebResponse)requestResultPage.GetResponse())
                {
                    using(Stream stream = responseLoginToPage.GetResponseStream())
                    {
                        using(StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                //log.Error("VisitGet" + ex.Message);
                return "";
            }
        }
        public static string VisitPost(string Url, string data, CookieContainer cookieContainer)
        {
            // 设置一些公用的请求头  
            try
            {
                NameValueCollection collection = new NameValueCollection();
                collection.Add("accept-language", "zh-cn,zh;q=0.5");
                collection.Add("accept-encoding", "gzip,deflate");
                collection.Add("accept-charset", "GB2312,utf-8;q=0.7,*;q=0.7");
                collection.Add("cache-control", "max-age=0");
                collection.Add("keep-alive", "115");

                // 来Post数据到登陆页面  
                HttpWebRequest requestLoginToPage = (HttpWebRequest)WebRequest.Create(Url);

                requestLoginToPage.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                requestLoginToPage.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                requestLoginToPage.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; zh-CN; rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13";
                requestLoginToPage.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                requestLoginToPage.Method = "POST";
                requestLoginToPage.Headers.Add(collection);

                requestLoginToPage.CookieContainer = cookieContainer;


                byte[] bytes = Encoding.UTF8.GetBytes(data);
                requestLoginToPage.ContentLength = bytes.Length;
                Stream streamLoginToPage = requestLoginToPage.GetRequestStream();
                streamLoginToPage.Write(bytes, 0, bytes.Length);
                streamLoginToPage.Flush();
                streamLoginToPage.Close();
                using(HttpWebResponse responseLoginToPage = (HttpWebResponse)requestLoginToPage.GetResponse())
                {
                    using(Stream stream = responseLoginToPage.GetResponseStream())
                    {
                        using(StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                //log.Error("VisitPost" + ex.Message);
                return "";
            }
        }
        public static string SendMail(string send, string sendname, string recieve, string subject, string mailbody,
            string sEncoding = "utf-8", bool IsBodyHtml = false, int Port = 25, string host = "LocalHost", string uname = "", string pwd = "",
            System.Net.Mail.SmtpDeliveryMethod DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.PickupDirectoryFromIis,
            string PickupDirectoryLocation = "", System.Net.Mail.MailPriority Priority = System.Net.Mail.MailPriority.High, string[] Files = null)
        {
            try
            {
                //生成一个   使用SMTP发送邮件的客户端对象   
                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
                //生成一个主机IP   
                client.Port = Port; //587, 465, 995   
                client.Host = host;
                //表示以当前登录用户的默认凭据进行身份验证   
                client.UseDefaultCredentials = string.IsNullOrEmpty(uname);
                //包含用户名和密码   
                if(!string.IsNullOrEmpty(uname)) client.Credentials = new System.Net.NetworkCredential(uname, pwd);
                //指定如何发送电子邮件。   
                client.DeliveryMethod = DeliveryMethod;
                if(client.DeliveryMethod == System.Net.Mail.SmtpDeliveryMethod.SpecifiedPickupDirectory) client.PickupDirectoryLocation = PickupDirectoryLocation;

                //通过本机SMTP服务器传送该邮件，   
                //其实使用该项的话就可以随意设定“主机,发件者昵称, 密码”，因为你的IIS服务器已经设定好了。而且公司内部发邮件是不需要验证的。   
                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();

                message.To.Add(recieve);

                if(client.DeliveryMethod == System.Net.Mail.SmtpDeliveryMethod.Network)
                {
                    message.From = new System.Net.Mail.MailAddress(send);
                }
                else
                {
                    message.From = new System.Net.Mail.MailAddress(send, sendname, System.Text.Encoding.GetEncoding(sEncoding));
                }
                message.Subject = subject;
                message.Body = mailbody;
                //定义邮件正文，主题的编码方式   
                message.BodyEncoding = System.Text.Encoding.GetEncoding(sEncoding);
                message.SubjectEncoding = System.Text.Encoding.GetEncoding(sEncoding);
                //获取或设置一个值，该值指示电子邮件正文是否为   HTML。   
                message.IsBodyHtml = IsBodyHtml;
                //指定邮件优先级   
                message.Priority = Priority;
                //添加附件   
                //System.Net.Mail.Attachment data = new Attachment(@"E:\9527\tubu\PA260445.JPG", System.Net.Mime.MediaTypeNames.Application.Octet);   
                if(Files != null)
                {
                    foreach(string s in Files) message.Attachments.Add(new System.Net.Mail.Attachment(s));
                }
                //发送   
                client.Send(message);
                return null;
            }
            catch(System.Net.Mail.SmtpException ex)
            {
                return ex.Message;
            }
        }



    }
}