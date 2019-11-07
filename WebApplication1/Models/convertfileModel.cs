using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wpsPreview.Models
{
    public class convertfileModel
    {
        /// <summary>
        /// 文档唯一sha1（完整文件名）
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 转换文档地址，磁盘路径或URL
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 文档的打开密码（没有密码为空即可）
        /// </summary>
        public string pass { get; set; }
        /// <summary>
        /// 转换完成后的回调函数
        /// </summary>
        public string callback { get; set; }
    }
}