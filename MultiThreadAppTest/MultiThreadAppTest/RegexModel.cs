using KaoriYa.Migemo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MultiThreadAppTest
{
    internal class RegexModel : IDisposable
    {
        // Migemo正規表現クラス
        private Migemo migemo;

        //// インスタンス
        //private static RegexModel regexModel;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RegexModel()
        {
            // Migemo使用時はMigemoオブジェクト生成
            if (File.Exists("migemo.dll") &&
                File.Exists("dict/migemo-dict"))
            {
                migemo = new Migemo("dict/migemo-dict");
            }
        }


        //public static RegexModel GetInstance()
        //{
        //    if (regexModel == null)
        //    {
        //        regexModel = new RegexModel();
        //    }

        //    return regexModel;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public Regex GetRegex(string pattern)
        {
            if (migemo != null &&
                pattern.StartsWith("/") &&
                !pattern.EndsWith("/"))
            {
                return migemo.GetRegex(pattern.Substring(1));
            }
            else
            {
                return new Regex(pattern);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (migemo != null)
            {
                migemo.Dispose();
            }
        }
    }
}
