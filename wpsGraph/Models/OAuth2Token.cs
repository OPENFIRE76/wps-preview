using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wpsGraph.Models
{
    /// <summary>
    /// Token实体模型（建议持久化存储，统一管理）
    /// </summary>
    public class OAuth2Token
    {
        /// <summary>
        /// 访问令牌
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// 有效期，以秒为单位
        /// </summary>
        public int expires_in { get; set; }
        /// <summary>
        /// access_token最终访问的范围，即实际授予的权限列表
        /// </summary>
        public string scope { get; set; }
        public string token_type { get; set; }
        public string refresh_token { get; set; }
    }
}