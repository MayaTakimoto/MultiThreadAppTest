using System.Threading;
using System.Windows;
using Livet;

namespace MultiThreadAppTest
{
    /// <summary>
    /// クリップボード監視クラス
    /// </summary>
    public class ClipboradWatcherModel : ViewModel
    {
        private string cbText;

        public string CbText
        {
            get 
            { 
                return this.cbText; 
            }
            set
            {
                if (!value.Equals(this.cbText))
                {
                    this.cbText = value;
                    RaisePropertyChanged("CbText");
                }
            }
        }

        public bool StopFlg { get; set; }   // 監視停止フラグ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ClipboradWatcherModel()
        {
            Clipboard.Clear();
            CbText = string.Empty;
        }

        /// <summary>
        /// クリップボードのテキストデータを取得する
        /// </summary>
        public void WatchClipboard()
        {
            while (StopFlg == false)
            {
                CbText = Clipboard.GetText();
                Thread.Sleep(1000);
            }
        }
    }
}
