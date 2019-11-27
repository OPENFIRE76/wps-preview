using ClassLib4Net;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;
using System.Text;
using System.Web.Mvc;

namespace wpsGraph.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        #region GetToken
        public JsonResult GetToken()
        {
            Models.OAuth2Token token = oAuth2ClientCredentials(ConfigHelper.GetAppSetting("ScopeStr"), "xiongxuehao");
            return Json(token, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redirect_uri">【必需】授权后要回调的URI</param>
        /// <param name="code">【必需】授权码（自定义）</param>
        /// <param name="state">不少于8个字符。用于保持请求和回调的状态，授权服务器在回调时（重定向用户浏览器到“redirect_uri”时），会在Query Parameter中原样回传该参数</param>
        private string oAuth2AuthCode(string redirect_uri, string code, string scope, string state = "xiongxuehao")
        {
            if(string.IsNullOrWhiteSpace(redirect_uri))
                throw new ArgumentException("必需参数无效", nameof(redirect_uri));
            if(string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("必需参数无效", nameof(code));
            if(string.IsNullOrWhiteSpace(scope))
                throw new ArgumentException("必需参数无效", nameof(scope));
            if(string.IsNullOrWhiteSpace(state) || state.Length < 8)
                throw new ArgumentException("必需参数无效", nameof(state));

            //string grant_type = "authorization_code";
            string url = ConfigHelper.GetAppSetting("GraphDomain") + "oauth2/auth";
            CookieContainer cookieContainer = null;
            string getCookie = string.Empty;
            NameValueCollection headers = new NameValueCollection();
            headers.Add("Authorization", ConfigHelper.GetAppSetting("AuthorizationStr"));

            redirect_uri = HttpUtility.UrlEncode(redirect_uri);
            string client_id = ConfigHelper.GetAppSetting("AppID");
            string data = string.Format($"client_id={client_id}&response_type=code&state={state}&redirect_uri={redirect_uri}&scope={scope}");

            string rsp = Models.HttpHelper.VisitUrl(url, "get", data, "", ref cookieContainer, ref getCookie, "application/x-www-form-urlencoded", DateTime.Now, headers);

            return rsp;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope">【必需】以空格分隔的权限列表</param>
        /// <param name="code"></param>
        /// <returns></returns>
        private Models.OAuth2Token oAuth2ClientCredentials(string scope, string code)
        {
            if(string.IsNullOrWhiteSpace(scope))
                throw new ArgumentException("必需参数无效", nameof(scope));

            string grant_type = "client_credentials";
            string url = ConfigHelper.GetAppSetting("GraphDomain") + "oauth2/token";
            CookieContainer cookieContainer = null;
            string getCookie = string.Empty;
            NameValueCollection headers = new NameValueCollection();
            headers.Add("Authorization", ConfigHelper.GetAppSetting("AuthorizationStr"));
            string data = string.Format($"grant_type={grant_type}&code={code}&scope={scope}");

            // application/json
            string rsp = Models.HttpHelper.VisitUrl(url, "post", data, "", ref cookieContainer, ref getCookie, "application/x-www-form-urlencoded", DateTime.Now, headers);
            if(!string.IsNullOrWhiteSpace(rsp))
            {
                /*
    {"access_token":"iQjrux063MMtcQX9Ow1mLCsKWODfeh26r2QpEicGz7E.LBiHWFJJPBEHv8QbXJ_IpyX8AC28vr5IDUL5ThPdWUQ","expires_in":3599,"scope":"App.Files.Read App.Files.ReadWrite App.Company.Read App.Company.ReadWrite App.CompanyMembers.Read App.CompanyMembers.ReadWrite App.Depts.Read App.Depts.ReadWrite App.DeptMembers.Read App.DeptMembers.ReadWrite App.Groups.Read App.Groups.ReadWrite App.GroupMembers.Read App.GroupMembers.ReadWrite App.Search App.Webhook Common.ContentProcessing.Session.Read Common.ContentProcessing.Session.ReadWrite Common.ContentProcessing.Operate.ReadWrite","token_type":"bearer"}
                 */
                return JsonHelper.DeSerialize<Models.OAuth2Token>(rsp);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="refresh_token">【必需】</param>
        private void oAuth2RefreshToken(string refresh_token)
        {
            string grant_type = "refresh_token";
            string url = ConfigHelper.GetAppSetting("GraphDomain") + "oauth2/token";
        }

        #endregion

    }
}