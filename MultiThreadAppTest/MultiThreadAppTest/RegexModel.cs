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

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RegexModel(bool useMigemo)
        {
            // Migemo使用時はMigemoオブジェクト生成
            if (useMigemo &&
                File.Exists("migemo.dll") &&
                File.Exists("dict/migemo-dict"))
            {
                migemo = new Migemo("dict/migemo-dict");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public Regex GetRegex(string pattern)
        {
            if (migemo != null)
            {
                return migemo.GetRegex(pattern);
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
