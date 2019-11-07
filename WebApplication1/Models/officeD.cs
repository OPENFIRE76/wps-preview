using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wpsPreview.Models
{
    public class officeD
    {
    }

    #region Model · 创建会话
    /// <summary>
    /// 会话id
    /// </summary>
    public class officeDsession
    {
        /// <summary>
        /// 会话id
        /// </summary>
        public string id { get; set; }
    }
    #endregion

    #region Model · 执行操作

    /// <summary>
    /// 操作接口请求参数。操作步骤（见下文）数组。数组元素最大个数10
    /// </summary>
    public class operateSteps
    {
        public operateStepItem[] steps { get; set; }
    }
    /// <summary>
    /// 操作接口请求参数。
    /// </summary>
    public class operateStepItem
    {
        /// <summary>
        /// deleteComments，acceptRevisions，convertToPDF，exportText，，，，，
        /// </summary>
        public string operate { get; set; }
        /// <summary>
        /// 操作参数。一般为键值对
        /// </summary>
        public IDictionary<string, string> args { get; set; }
    }




    /// <summary>
    /// 接口返回结果
    /// </summary>
    public class ResultLocation
    {
        public Location location { get; set; }
    }

    public class Location
    {
        public string address { get; set; }
        public Extra extra { get; set; }
    }
    public class Extra
    {
        public string method { get; set; }
        /// <summary>
        ///  "Content-Type": string,"Authorization": string
        /// </summary>
        public IDictionary<string, string> headers { get; set; }
    }

    #endregion

}