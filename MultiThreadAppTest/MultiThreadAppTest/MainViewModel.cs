//-----------------------------------------------------------------------
// <summary>メインウィンドウViewModel</summary>
// <author>MayaTakimoto</author>
// <date>$Date: 2013-11-01 14:00:00 +9:00 $</date>
// <copyright file="$Name: MainViewModel.cs $" >
// Copyright(c) 2013 MayaTakimoto All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------
using Livet;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;

namespace MultiThreadAppTest
{
    /// <summary>
    /// MainWindowのViewModelクラス
    /// </summary>
    public class MainViewModel : ViewModel
    {
        // 表示用リスト
        private ObservableCollection<string> listSource;

        // データ保持用リスト
        private ObservableCollection<string> listMain;

        // クリップボード監視オブジェクト
        private ClipboradWatcherModel cw;

        // スレッド
        private Thread thread;

        // 正規表現パターン生成オブジェクト
        private RegexModel regex;

        /// <summary>
        /// 表示用リストプロパティ
        /// </summary>
        /// <remarks>画面にはこのリストの内容が反映される</remarks>
        public ObservableCollection<string> ListSource
        {
            get
            {
                return this.listSource;
            }
            set
            {
                this.listSource = value;
                RaisePropertyChanged("ListSource");
            }
        }

        /// <summary>
        /// データ保持用リストプロパティ
        /// </summary>
        /// <remarks>取得した値はこちらに保持される</remarks>
        private ObservableCollection<string> ListMain
        {
            get 
            { 
                return this.listMain; 
            }
            set 
            {
                this.listMain = value;

                // 画面に反映させる
                ListSource = this.listMain;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainViewModel()
        {
            // クリップボード監視前処理
            cw = new ClipboradWatcherModel();
            CompositeDisposable.Add(cw);

            regex = new RegexModel(false);
            CompositeDisposable.Add(regex);
            //Settings.UseMigemo = true;
            
            // 取得済みデータを復元
            this.Load();
        }

        /// <summary>
        /// クリップボード変更検知時イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cw_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // クリップボードが空の場合は何もしない
            if (string.IsNullOrEmpty(cw.CbText))
            {
                return;
            }

            // 同じテキストデータがリスト内に存在する場合は何もしない
            var res = ListMain.Where((s) => { return cw.CbText.Equals(s); });
            if (res.Count() > 0)
            {
                return;
            }

            // テキストデータをリストに追加
            Application.Current.Dispatcher.Invoke(
                new Action(() =>
                {
                    ListMain.Add(cw.CbText);
                })
            );
        }

        /// <summary>
        /// クリップボード監視の開始
        /// </summary>
        public void ThreadStart()
        {
            if (thread == null)
            {
                // クリップボード変更検知イベントを登録
                cw.PropertyChanged += new PropertyChangedEventHandler(cw_PropertyChanged);

                thread = new Thread(new ThreadStart(cw.WatchClipboard));
                thread.SetApartmentState(ApartmentState.STA);

                // 監視開始
                cw.StopFlg = false;
                thread.Start();
            }
        }

        /// <summary>
        /// クリップボード監視の終了
        /// </summary>
        public void ThreadStop()
        {
            if (thread != null)
            {
                cw.StopFlg = true;

                // 監視終了
                thread.Abort();
                thread = null;

                // クリップボード変更検知イベントの登録を解除
                cw.PropertyChanged -= cw_PropertyChanged;
            }
        }

        /// <summary>
        /// 取得済みデータ保存
        /// </summary>
        public void Save()
        {
            SaveLoadModel.Save<string>(ListMain);
        }

        /// <summary>
        /// 取得済みデータ復元
        /// </summary>
        public void Load()
        {
            ListMain = SaveLoadModel.Load<string>();
        }

        /// <summary>
        /// インクリメンタルサーチ
        /// </summary>
        public void Search(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                // テキストが空白ならデータを復元
                ListSource = new ObservableCollection<string>(ListMain);
            }
            else
            {
                // 入力値に応じてフィルタリングする
                ListSource = new ObservableCollection<string>(ListMain.Where((s) =>
                {
                    return regex.GetRegex(searchText).IsMatch(s);
                }));
            }
        }
        
        /// <summary>
        /// アプリケーションの終了
        /// </summary>
        public void Shutdown()
        {
            this.ThreadStop();
            
            // 取得済みデータの保存
            this.Save();

            Application.Current.Shutdown();
        }
    }
}
