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

//using static System.Math;
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

        private Dictionary<string, PointInfo> pointdelay = new Dictionary<string, PointInfo>();

        private bool pointselected;

        private List<string> pointstrselected = new List<string>();

        /// <summary>
        /// initialize
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            #region hide
            /*temp test*/
            //Calculator.GetRealCount(5, new DateTime(2015, 1, 1, 12, 38, 0), new DateTime(2015, 1, 1, 14, 38, 0));
            //Calculator.GetAvgCount(5, new DateTime(2015, 1, 1, 12, 38, 0), new DateTime(2015, 1, 1, 14, 38, 0));
            #endregion
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
            //machine time delay

            for (int i = 1; i < 10; i++)
            {
                try
                { 
                    string[] machineinfo = ((string)(new AppSettingsReader()).GetValue("machinedelay_" + i.ToString(), typeof(string))).Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach(string point in machineinfo[1].Split(','))
                    {
                        if(pointdelay.ContainsKey(point))
                        {
                            pointdelay[point].delay = int.Parse(machineinfo[0]);
                            pointdelay[point].machineid = i;
                        }
                        else
                        {
                            pointdelay.Add(point, new PointInfo()
                            {
                                delay = int.Parse(machineinfo[0]),
                                machineid = i
                            });
                        }
                    }
                }
                catch(Exception ex)
                {

                }
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


        private bool PointSelected()
        {
            if (PointListBox.SelectedItems.Count == 0)
            {
                return false;
            }
            else
                return true;
        }

        /// <summary>
        /// real minitues' values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void realupdate_btn_Click(object sender, EventArgs e)
        {
            if ((pointselected = PointSelected()) == false)
            {
                MessageBox.Show("未在列表框选中计量点");
                return;
            }
            else
            {
                pointstrselected.Clear();
                foreach(string ps in PointListBox.CheckedItems)
                {
                    pointstrselected.Add(ps);
                }
            }
            StartInvalid();
            //calculate the amout of calculation
            WorkArgs wa = calculateamout(worktype.real);
            if (wa != null)
                bgw.RunWorkerAsync(wa);
            else
                MessageBox.Show("无效参数");
        }

        /// <summary>
        /// avg hours' value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void avgupdate_btn_Click(object sender, EventArgs e)
        {
            if ((pointselected = PointSelected()) == false)
            {
                MessageBox.Show("未在列表框选中计量点");
                return;
            }
            else
            {
                pointstrselected.Clear();
                foreach (string ps in PointListBox.CheckedItems)
                {
                    pointstrselected.Add(ps);
                }
            }
            StartInvalid();
            //calculate the amout of calculation
            WorkArgs wa = calculateamout(worktype.avg);
            if (wa != null)
                bgw.RunWorkerAsync(wa);
            else
                MessageBox.Show("无效参数");
        }

        /// <summary>
        /// both of above two values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bothupdate_btn_Click(object sender, EventArgs e)
        {
            if ((pointselected = PointSelected()) == false)
            {
                MessageBox.Show("未在列表框选中计量点");
                return;
            }
            else
            {
                pointstrselected.Clear();
                foreach (string ps in PointListBox.CheckedItems)
                {
                    pointstrselected.Add(ps);
                }
            }
            StartInvalid();
            //calculate the amout of calculation
            WorkArgs wa = calculateamout(worktype.both);
            if (wa != null)
                bgw.RunWorkerAsync(wa);
            else
                MessageBox.Show("无效参数");
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
            if (wt == worktype.avg)
            {
                return new WorkArgs() { currentdone = 0, total = Calculator.GetAvgCount(pointstrselected.Count, DateTime.Parse(StartdateTimePicker.Text), DateTime.Parse(EnddateTimePicker.Text)), wt = wt };
            }
            else if (wt == worktype.real)
            {
                return new WorkArgs() { currentdone = 0, total = Calculator.GetRealCount(pointstrselected.Count, DateTime.Parse(StartdateTimePicker.Text), DateTime.Parse(EnddateTimePicker.Text)), wt = wt };
            }
            else if (wt == worktype.both)
            {
                return new WorkArgs() { currentdone = 0, total = Calculator.GetRealCount(pointstrselected.Count, DateTime.Parse(StartdateTimePicker.Text), DateTime.Parse(EnddateTimePicker.Text)) + Calculator.GetAvgCount(pointstrselected.Count, DateTime.Parse(StartdateTimePicker.Text), DateTime.Parse(EnddateTimePicker.Text)), wt = wt };
            }
            else
                return null;
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
            realstatuslabel.Text = "状态: " + cs.pointname + " " + cs.ts.ToString("yyyy-MM-dd HH:mm:ss") + " " + cs.pv.ToString();
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
            //warg.total = 100;
            foreach(string ps in pointstrselected)
            {
                DateTime st = DateTime.Parse(StartdateTimePicker.Text);
                DateTime et = DateTime.Parse(EnddateTimePicker.Text);
                while (st <= et)
                {
                    if (((BackgroundWorker)sender).CancellationPending)
                    {
                        //set cancelled true
                        e.Cancel = true;
                        return;
                    }

                    //get his value for the point
                    PI.RetVal rv = (new PI.PIFunc2((string)(new AppSettingsReader()).GetValue("ip", typeof(string)), (string)(new AppSettingsReader()).GetValue("username", typeof(string)), (string)(new AppSettingsReader()).GetValue("password", typeof(string)))).GetPointHisValue(ps, st);
                    if (rv != null)
                    {
                        int status = (new LocalPIData.SQLPart()).UpdatePiRecord(st, ps, (float)rv.pvalue, pointdelay[ps].machineid, (int)plantid);
                        if(status == -1)
                        {
                            (new Log()).AddExceptionLog(new ExceptionBody() { et = ExceptionType.Error, info = "数据调整异常", ts = DateTime.Now }, logtype.console);
                        }
                    }                    

                    warg.currentdone++;
                    bgw.ReportProgress((int)(((float)warg.currentdone / (float)warg.total) * 100), new CurrentState() { pointname = ps, ts = st, pv = rv.pvalue });
                    st = st.AddMinutes(1.0);
                }
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
            //warg.total = 100;
            foreach (string ps in pointstrselected)
            {
                DateTime st = DateTime.Parse(DateTime.Parse(StartdateTimePicker.Text).ToString("yyyy-MM-dd HH:00:00"));
                DateTime et = DateTime.Parse(DateTime.Parse(EnddateTimePicker.Text).ToString("yyyy-MM-dd HH:00:00"));
                while (st <= et)
                {
                    if (((BackgroundWorker)sender).CancellationPending)
                    {
                        //set cancelled true
                        e.Cancel = true;
                        return;
                    }

                    //get value for the point
                    double? rv;
                    if (pointdelay.ContainsKey(ps))
                    {
                        rv = (new PI.PIFunc2((string)(new AppSettingsReader()).GetValue("ip", typeof(string)), (string)(new AppSettingsReader()).GetValue("username", typeof(string)), (string)(new AppSettingsReader()).GetValue("password", typeof(string)))).GetAverageValue(ps, st.AddSeconds(pointdelay[ps].delay), st.AddHours(1.0).AddSeconds(pointdelay[ps].delay));
                        //rv = 0;
                    }
                    else
                    {
                        rv = null;
                    }

                    if (rv != null)
                    {
                        (new LocalPIData.SQLPart()).UpdateAvgRd(ps, st, (float)rv);
                    }

                    warg.currentdone++;
                    bgw.ReportProgress((int)(((float)warg.currentdone / (float)warg.total) * 100), new CurrentState() { pointname = ps, ts = st, pv = rv });
                    st = st.AddHours(1.0);
                }
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
        public long total { get; set; } //= 0;
        public long currentdone { get; set; } //= 0;
        public worktype wt { get; set; }
    }

    /// <summary>
    /// 当前的线程获取值
    /// </summary>
    public class CurrentState
    {
        public string pointname { get; set; }
        public DateTime ts { get; set; }
        public double? pv { get; set; }
    }

    public class PointInfo
    {
        public int delay { get; set; }
        public int machineid { get; set; }
    }
}
