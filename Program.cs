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

            // https://www.nuget.org/packages/Geckofx60.64/
            // https://qiita.com/toryuneko/items/a6fa383d01aa2949f8d5
            var app = GetGeckoDllPath();
            Gecko.Xpcom.Initialize(app);
            var geckoWebBrowser = new GeckoWebBrowser { Dock = DockStyle.Fill };
            geckoWebBrowser.Name = "geckoWebBrowser";

            Form form = new Form1();
            var controls = form.Controls.Find("Panel1", true);
            ((Panel)controls[0]).Controls.Add(geckoWebBrowser);

            Application.Run(form);
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
