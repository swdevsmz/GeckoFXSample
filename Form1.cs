using Gecko;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeckoFXSample
{

    /// http://hensa40.cutegirl.jp/archives/2971
    /// https://dobon.net/vb/dotnet/programing/displayprogress.html#backgroundworker
    public partial class Form1 : Form
    {

        BackgroundWorker backgroundWorker;
        GeckoWebBrowser geckoWebBrowser;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CancelButton.Enabled = false;

            backgroundWorker = new BackgroundWorker();
            // スレッドのキャンセルを可能とする
            backgroundWorker.WorkerSupportsCancellation = true;
            // スレッドからの進捗通知を可能とする
            backgroundWorker.WorkerReportsProgress = true;
            //イベントハンドラをイベントに関連付ける
            backgroundWorker.DoWork += new DoWorkEventHandler(DoWork);
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted);

            // Program.csでセットしたGeckoWebBrowserを取得
            var controls = this.panel1.Controls.Find("geckoWebBrowser", true);
            geckoWebBrowser = ((GeckoWebBrowser)controls[0]);

           
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            //------------------------------------------
            // スレッドの多重起動はできないための制御
            //------------------------------------------
            // GUIで制御する場合
            StartButton.Enabled = false;
            CancelButton.Enabled = true;

            //// プログラムで制御する場合
            //if (backgroundWorker1.IsBusy) {
            //    MessageBox.Show("すでに実行中です");
            //    return;
            //}

            //------------------------------------------
            // スレッドの起動
            //    backgroundWorker1_DoWork メソッドが別スレッドで実行される
            //    パラメータも渡している（object型なので、パラメータに制約はない）
            int arg = 30;
            backgroundWorker.RunWorkerAsync(arg);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            // GUIで制御する場合
            StartButton.Enabled = true; // スタートボタンを有効にする
            CancelButton.Enabled = false; // キャンセルボタンを無効にする

            //// プログラムで制御する場合
            //if (!backgroundWorker1.IsBusy) { 
            //    MessageBox.Show("スレッドは実行されていません");
            //    return;
            //}

            //------------------------------------------
            // スレッドのキャンセル
            //   強制的に停止するのではなく、スレッドにキャンセルを通知する
            //   スレッドキャンセルの決定はスレッドメソッド内で行う。
            //------------------------------------------
            backgroundWorker.CancelAsync();
        }

        //BackgroundWorkerのDoWorkイベントハンドラ
        //ここで時間のかかる処理を行う
        // 別スレッドで動作するメソッド
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            // パラメータを object 型からキャストして取り出す
            int seconds = (int)(e.Argument);

            // パラメータの秒数分スレッドを実行させる
            for (int i = 1; i <= seconds; i++)
            {
                // キャンセル通知があればスレッドをキャンセルする
                if (backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    //---------------------------------
                    // キャンセル時は、e.Resultを設定しても完了通知で受け取れない
                    //---------------------------------
                    e.Result = "スレッドキャンセル";
                    return;
                }

                //１秒停止
                System.Threading.Thread.Sleep(1000);

                int prog = (int)(i * 100 / seconds); // 進捗を計算(単位：%)
                string msg = "スレッド開始後、" + i.ToString() + " 秒経過しました。";

                //------------------------------------------
                // 進捗を通知する
                //　・backgroundWorker1_ProgressChanged がメインスレッドから呼び出される
                //　・パラメータ１：進捗（%）       int型
                //　・パラメータ２：その他あれば    object型
                //------------------------------------------
                backgroundWorker.ReportProgress(prog, msg);

            }
            e.Result = "スレッド完了";
        }

        // BackgroundWorkerのProgressChangedイベントハンドラ
        // コントロールの操作は必ずここで行い、DoWorkでは絶対にしない
        // スレッドの進捗通知を受け取る(メインスレッドで動作する)
        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // スレッドから通知された、パラメータを受け取る
            progressBar1.Value = e.ProgressPercentage; // パラメータ１
            TextBox1.Text = (string)e.UserState;       // パラメータ２(object型なのでキャスト)
        }

        // BackgroundWorkerのRunWorkerCompletedイベントハンドラ
        // 処理が終わったときに呼び出される
        // スレッドの完了通知を受け取る(メインスレッドで動作する)
        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //ボタンの有効無効を設定する
            StartButton.Enabled = true; // 開始ボタン
            CancelButton.Enabled = false; // キャンセルボタン

            if (e.Cancelled)
            {
                TextBox1.Text = "キャンセルされました";
                progressBar1.Value = 0;
                // キャンセル時にResultプロパティから値を取得すると例外が発生する
                //textBox1.Text = e.Result.ToString()
                return;
            }

            // スレッドの結果を取得
            TextBox1.Text = e.Result.ToString();
        }

        private void navigateButton_Click(object sender, EventArgs e)
        {
            // Googleを表示
            geckoWebBrowser.Navigate(this.urlTextBox.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            geckoWebBrowser.Dispose();
        }
    }
}