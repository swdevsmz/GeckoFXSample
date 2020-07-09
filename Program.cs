using System;
using Gecko;
using System.Windows.Forms;

namespace GeckoFXSample
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var app = GetGeckoDllPath();
            Gecko.Xpcom.Initialize(app);
            var geckoWebBrowser = new GeckoWebBrowser { Dock = DockStyle.Fill };
            Form f = new Form();
            f.Controls.Add(geckoWebBrowser);
            geckoWebBrowser.Navigate("www.google.com");
            Application.Run(f);
        }
        /// <summary>
        /// Firefoxライブラリのランタイム場所を返すメソッド
        /// 場所はお任せ
        /// x86 ... Firefox
        /// x64 ... Firefox64
        /// </summary>
        /// <returns></returns>
        private static string GetGeckoDllPath() =>
            IntPtr.Size == 4 ?
                System.IO.Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Firefox") :
                System.IO.Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Firefox64");
    }
}
