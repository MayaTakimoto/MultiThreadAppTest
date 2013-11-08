using System;
using System.Collections.ObjectModel;
using System.IO;
using ProtoBuf;

namespace MultiThreadAppTest
{
    /// <summary>
    /// ファイル読書処理クラス
    /// </summary>
    public class SaveLoadModel
    {
        /// <summary>
        /// ファイル書込処理
        /// </summary>
        /// <typeparam name="T">コレクション要素の型</typeparam>
        /// <param name="oc">コレクション</param>
        public static void Save<T>(ObservableCollection<T> oc)
        {
            byte[] data = null;
            FileStream fs = new FileStream("hist.txt", FileMode.Create, FileAccess.Write, FileShare.Write, 4096, true);

            if (oc == null || oc.Count == 0)
            {
                return;
            }
            
            using (MemoryStream ms = new MemoryStream())
            {
                Serializer.Serialize(ms, oc);
                data = ms.ToArray();
            }

            fs.BeginWrite(data, 0, data.Length, new AsyncCallback(SaveCallback), fs);
        }

        /// <summary>
        /// ファイル読込処理
        /// </summary>
        /// <typeparam name="T">コレクション要素の型</typeparam>
        /// <returns>コレクション</returns>
        public static ObservableCollection<T> Load<T>()
        {
            ObservableCollection<T> oc = null;

            if (File.Exists("hist.txt"))
            {
                using (FileStream fs = new FileStream("hist.txt", FileMode.Open, FileAccess.Read))
                {
                    oc = Serializer.Deserialize<ObservableCollection<T>>(fs);
                }
            }
            else
            {
                oc = new ObservableCollection<T>();
            }

            return oc;
        }

        /// <summary>
        /// 非同期書込処理のコールバックメソッド
        /// </summary>
        /// <param name="ar"></param>
        private static void SaveCallback(IAsyncResult ar)
        {
            // 当該ストリームの取得
            FileStream fs = ar.AsyncState as FileStream;
            
            fs.EndWrite(ar);
            fs.Close();
        }
    }
}
