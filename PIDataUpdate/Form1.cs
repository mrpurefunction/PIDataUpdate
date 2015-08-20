using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Configuration;
using System.Threading;

using static System.Math;
using PublicLib;

namespace PIDataUpdate
{

    public partial class Form1 : Form
    {
        private Thread logt = null;

        private int? plantid = null;
        private string[] pipoints = null;
        private BackgroundWorker bgw = new BackgroundWorker();

        private int cancelationToken;
        private object omutex = new object();

        public dl desktopdl;
        public dl_closeform closedl;

        /// <summary>
        /// initialize
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            //start logger thread
            logt = new Thread(new ThreadStart(LogThread));
            logt.Start();

            try
            {
                pipoints = ((string)(new AppSettingsReader()).GetValue("pipoint", typeof(string))).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                PointListBox.Items.AddRange(pipoints);
                plantid = int.Parse((string)(new AppSettingsReader()).GetValue("plantid", typeof(string)));
            }
            catch (Exception ex)
            {
                //Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "---" + ex.Message);
                ExceptionBody eb = new ExceptionBody() { et = ExceptionType.Error, info = "Initialization?" + ex.Message, ts = DateTime.Now };
                (new PublicLib.Log()).AddExceptionLog(eb, logtype.console);
            }

            if ((PointListBox.Items.Count == 0) || (pipoints.Length == 0))
            {
                InvalidControls();
                ExceptionBody eb = new ExceptionBody() { et = ExceptionType.Warning, info = "Initialization?" + "配置文件中没有找到PI计量点", ts = DateTime.Now };
                (new PublicLib.Log()).AddExceptionLog(eb, logtype.console);
                MessageBox.Show("配置文件中没有找到PI计量点");
                return;      
            }
            if (plantid == null)
            {
                InvalidControls();
                ExceptionBody eb = new ExceptionBody() { et = ExceptionType.Warning, info = "Initialization?" + "未能初始化plantid", ts = DateTime.Now };
                (new PublicLib.Log()).AddExceptionLog(eb, logtype.console);
                MessageBox.Show("未能初始化plantid, 检查配置文件");
                return;
            }

            //connect pi
            new PI.PIFunc2((string)(new AppSettingsReader()).GetValue("ip", typeof(string)), (string)(new AppSettingsReader()).GetValue("username", typeof(string)), (string)(new AppSettingsReader()).GetValue("password", typeof(string)));
            //connect sql
            {
                //no special initialization
            }
            //backgroudworker init
            {
                bgw.DoWork += Bgw_DoWork;
                bgw.ProgressChanged += Bgw_ProgressChanged;
                bgw.RunWorkerCompleted += Bgw_RunWorkerCompleted;
                bgw.WorkerReportsProgress = true;
                bgw.WorkerSupportsCancellation = true;
                desktopdl += updatedesktop;
                UpdateprogressBar.Style = ProgressBarStyle.Blocks;
            }
            //other init
            {
                closedl += closeform;
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
                this.FormClosing += Form1_FormClosing;
            }
            StartValid();
        }
        #region invalidcontrols start_valid start_invalid
        private void InvalidControls()
        {
            avgupdate_btn.Enabled = false;
            bothupdate_btn.Enabled = false;
            realupdate_btn.Enabled = false;
            stopupdate_btn.Enabled = false;
        }

        private void StartValid()
        {
            avgupdate_btn.Enabled = true;
            bothupdate_btn.Enabled = true;
            realupdate_btn.Enabled = true;
            stopupdate_btn.Enabled = false;
        }

        private void StartInvalid()
        {
            avgupdate_btn.Enabled = false;
            bothupdate_btn.Enabled = false;
            realupdate_btn.Enabled = false;
            stopupdate_btn.Enabled = true;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show("过程提前结束");
                StartValid();
            }
            else
            {
                MessageBox.Show("过程已完成");
                StartValid();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (this.IsHandleCreated)
                this.Invoke(desktopdl, new object[] { e.ProgressPercentage, e.UserState });
        }


        /// <summary>
        /// real minitues' values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void realupdate_btn_Click(object sender, EventArgs e)
        {
            StartInvalid();
            //calculate the amout of calculation
            WorkArgs wa = calculateamout(worktype.real);
            bgw.RunWorkerAsync(wa);

        }

        /// <summary>
        /// avg hours' value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void avgupdate_btn_Click(object sender, EventArgs e)
        {
            StartInvalid();
            //calculate the amout of calculation
            WorkArgs wa = calculateamout(worktype.avg);
            bgw.RunWorkerAsync(wa);
        }

        /// <summary>
        /// both of above two values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bothupdate_btn_Click(object sender, EventArgs e)
        {
            StartInvalid();
            //calculate the amout of calculation
            WorkArgs wa = calculateamout(worktype.both);
            bgw.RunWorkerAsync(wa);
        }

        /// <summary>
        /// stop the process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopupdate_btn_Click(object sender, EventArgs e)
        {
            lock (omutex)
            {
                if (cancelationToken != 1)
                    cancelationToken = 1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            WorkArgs warg = (WorkArgs)e.Argument;
            if (warg.wt == worktype.real)
            {
                RealFunc(sender, e);
            }
            else if (warg.wt == worktype.avg)
            {
                AvgFunc(sender, e);
            }
            else if (warg.wt == worktype.both)
            {
                RealFunc(sender, e);
                AvgFunc(sender, e);
            }
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private WorkArgs calculateamout(worktype wt)
        {
            //will modify later
            return new WorkArgs() { currentdone = 0, total = 100, wt = wt };
        }

        /// <summary>
        /// delegate method for update desktop info
        /// </summary>
        /// <param name="pecentage"></param>
        /// <param name="cs"></param>
        private void updatedesktop(int percentage, CurrentState cs)
        {
            UpdateprogressBar.Value = percentage;
            percentagelabel.Text = string.Format("{0}%", percentage.ToString());
            int tempcancellation;
            lock (omutex)
            {
                tempcancellation = cancelationToken;
            }
            if (tempcancellation == 1)
            {
                bgw.CancelAsync();
                lock (omutex)
                {
                    if (cancelationToken != 0)
                        cancelationToken = 0;
                }
            }

        }

        /// <summary>
        /// delegate method for closing
        /// </summary>
        /// <returns></returns>
        private int closeform()
        {
            stopupdate_btn_Click(null, null);
            #region
            //int exited;
            //while (true)
            //{
            //    lock (omutex2)
            //    {
            //        exited = appExit;
            //    }
            //    if (exited == 0)
            //        Thread.Sleep(100);
            //    else
            //    {
            //        Thread.Sleep(5000);
            //        break;
            //    }
            //}
            #endregion
            Thread.Sleep(2000);
            return 1;
        }

        /// <summary>
        /// real value backgroud func
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RealFunc(object sender, DoWorkEventArgs e)
        {
            WorkArgs warg = (WorkArgs)e.Argument;
            //the line below will be moved
            warg.total = 100;
            while (true)
            {
                if (((BackgroundWorker)sender).CancellationPending)
                {
                    //set cancelled true
                    e.Cancel = true;
                    return;
                }

                Thread.Sleep(100);
                if (warg.currentdone == 100)
                    return;

                warg.currentdone++;
                bgw.ReportProgress((int)warg.currentdone, new CurrentState());
                //bgw.ReportProgress((int)Math.Floor((double)(warg.currentdone / warg.total * 100)), new CurrentState());
            }
        }

        /// <summary>
        /// avg value background func
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AvgFunc(object sender, DoWorkEventArgs e)
        {
            WorkArgs warg = (WorkArgs)e.Argument;
            //the line below will be moved
            warg.total = 100;
            while (true)
            {
                if (((BackgroundWorker)sender).CancellationPending)
                {
                    //set cancelled true
                    e.Cancel = true;
                    return;
                }

                Thread.Sleep(100);
                if (warg.currentdone == 100)
                    return;

                warg.currentdone++;
                bgw.ReportProgress((int)warg.currentdone, new CurrentState());
                //bgw.ReportProgress((int)Math.Floor((double)(warg.currentdone / warg.total * 100)), new CurrentState());
            }
        }

        /// <summary>
        /// logger thread
        /// </summary>
        private void LogThread()
        {
            while (1 == 1)
            {
                (new PublicLib.Log()).ExportConsoleLog();
                Thread.Sleep(500);
            }
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="i"></param>
    /// <param name="cs"></param>
    public delegate void dl(int i, CurrentState cs);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public delegate int dl_closeform();

    /// <summary>
    /// 
    /// </summary>
    public enum worktype
    {
        real,
        avg,
        both
    }

    /// <summary>
    /// arguments for calculating work progress
    /// </summary>
    public class WorkArgs
    {
        public long total { get; set; } = 0;
        public long currentdone { get; set; } = 0;
        public worktype wt { get; set; }
    }

    /// <summary>
    /// 当前的线程获取值
    /// </summary>
    public class CurrentState
    {
        public string pointname { get; set; }
        public DateTime ts { get; set; }
        public double pv { get; set; }
    }
}
