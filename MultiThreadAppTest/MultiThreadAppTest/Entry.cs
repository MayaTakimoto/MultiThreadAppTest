using System;
using System.Windows;

namespace MultiThreadAppTest
{
    /// <summary>
    /// アプリケーションエントリポイント
    /// </summary>
    public class Entry
    {
        [STAThread()]
        public static void Main()
        {
            Application app = new Application();
            MainWindow mw = new MainWindow();

            app.Run(mw);
        }
    }
}
