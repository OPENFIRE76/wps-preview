using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace WebApplication1.Models
{
    public class FileHelper
    {
        /// <summary>
        /// 获取文件的SHA1值
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static string GetSHA1(string path)
        {
            FileStream file = new FileStream(path, FileMode.Open);
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] retval = sha1.ComputeHash(file);
            file.Close();

            StringBuilder sc = new StringBuilder();
            for(int i = 0; i < retval.Length; i++)
            {
                sc.Append(retval[i].ToString("x2"));
            }
            //Console.WriteLine("文件SHA1：{0}", sc);
            return sc.ToString();
        }

        /// <summary>
        /// 获取文件的MD5值
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static string GetMD5(string path)
        {
            FileStream file = new FileStream(path, FileMode.Open);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retval = md5.ComputeHash(file);
            file.Close();

            StringBuilder sc = new StringBuilder();
            for(int i = 0; i < retval.Length; i++)
            {
                sc.Append(retval[i].ToString("x2"));
            }
            //Console.WriteLine("文件MD5：{0}", sc);
            return sc.ToString();
        }


    }
}