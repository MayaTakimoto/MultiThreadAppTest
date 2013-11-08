﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using Livet;

namespace MultiThreadAppTest
{
    /// <summary>
    /// MainWindowのViewModelクラス
    /// </summary>
    public class MainViewModel : ViewModel
    {
        private ObservableCollection<string> listSource;
        private ClipboradWatcherModel cw;
        private Thread thread;

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
        /// コンストラクタ
        /// </summary>
        public MainViewModel()
        {
            cw = new ClipboradWatcherModel();
            CompositeDisposable.Add(cw);
            
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
            var res = ListSource.Where((s) => { return cw.CbText.Equals(s); });
            if (res.Count() > 0)
            {
                return;
            }

            // テキストデータをリストに追加
            Application.Current.Dispatcher.Invoke(
                new Action(() =>
                {
                    ListSource.Add(cw.CbText);
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
            SaveLoadModel.Save<string>(ListSource);
        }

        /// <summary>
        /// 取得済みデータ復元
        /// </summary>
        public void Load()
        {
            ListSource = SaveLoadModel.Load<string>();
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