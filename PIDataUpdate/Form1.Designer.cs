namespace PIDataUpdate
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.PointListBox = new System.Windows.Forms.CheckedListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.StartdateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.EnddateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.realupdate_btn = new System.Windows.Forms.Button();
            this.avgupdate_btn = new System.Windows.Forms.Button();
            this.realstatuslabel = new System.Windows.Forms.Label();
            this.avgstatuslabel = new System.Windows.Forms.Label();
            this.bothupdate_btn = new System.Windows.Forms.Button();
            this.stopupdate_btn = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.UpdateprogressBar = new System.Windows.Forms.ProgressBar();
            this.label7 = new System.Windows.Forms.Label();
            this.percentagelabel = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "计量点:";
            // 
            // PointListBox
            // 
            this.PointListBox.CheckOnClick = true;
            this.PointListBox.FormattingEnabled = true;
            this.PointListBox.Location = new System.Drawing.Point(15, 25);
            this.PointListBox.Name = "PointListBox";
            this.PointListBox.Size = new System.Drawing.Size(199, 292);
            this.PointListBox.TabIndex = 1;
            this.PointListBox.ThreeDCheckBoxes = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(236, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "起始时间:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(236, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "结束时间:";
            // 
            // StartdateTimePicker
            // 
            this.StartdateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:00";
            this.StartdateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.StartdateTimePicker.Location = new System.Drawing.Point(301, 9);
            this.StartdateTimePicker.Name = "StartdateTimePicker";
            this.StartdateTimePicker.Size = new System.Drawing.Size(204, 21);
            this.StartdateTimePicker.TabIndex = 4;
            // 
            // EnddateTimePicker
            // 
            this.EnddateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:00";
            this.EnddateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.EnddateTimePicker.Location = new System.Drawing.Point(301, 56);
            this.EnddateTimePicker.Name = "EnddateTimePicker";
            this.EnddateTimePicker.Size = new System.Drawing.Size(204, 21);
            this.EnddateTimePicker.TabIndex = 5;
            // 
            // realupdate_btn
            // 
            this.realupdate_btn.Location = new System.Drawing.Point(227, 117);
            this.realupdate_btn.Name = "realupdate_btn";
            this.realupdate_btn.Size = new System.Drawing.Size(91, 23);
            this.realupdate_btn.TabIndex = 6;
            this.realupdate_btn.Text = "更新实时值";
            this.realupdate_btn.UseVisualStyleBackColor = true;
            this.realupdate_btn.Click += new System.EventHandler(this.realupdate_btn_Click);
            // 
            // avgupdate_btn
            // 
            this.avgupdate_btn.Location = new System.Drawing.Point(227, 172);
            this.avgupdate_btn.Name = "avgupdate_btn";
            this.avgupdate_btn.Size = new System.Drawing.Size(91, 23);
            this.avgupdate_btn.TabIndex = 7;
            this.avgupdate_btn.Text = "更新小时均值";
            this.avgupdate_btn.UseVisualStyleBackColor = true;
            this.avgupdate_btn.Click += new System.EventHandler(this.avgupdate_btn_Click);
            // 
            // realstatuslabel
            // 
            this.realstatuslabel.AutoSize = true;
            this.realstatuslabel.Location = new System.Drawing.Point(325, 151);
            this.realstatuslabel.Name = "realstatuslabel";
            this.realstatuslabel.Size = new System.Drawing.Size(53, 12);
            this.realstatuslabel.TabIndex = 8;
            this.realstatuslabel.Text = "状态: 无";
            // 
            // avgstatuslabel
            // 
            this.avgstatuslabel.AutoSize = true;
            this.avgstatuslabel.Location = new System.Drawing.Point(325, 177);
            this.avgstatuslabel.Name = "avgstatuslabel";
            this.avgstatuslabel.Size = new System.Drawing.Size(53, 12);
            this.avgstatuslabel.TabIndex = 9;
            this.avgstatuslabel.Text = "状态: 无";
            this.avgstatuslabel.Visible = false;
            // 
            // bothupdate_btn
            // 
            this.bothupdate_btn.Location = new System.Drawing.Point(227, 223);
            this.bothupdate_btn.Name = "bothupdate_btn";
            this.bothupdate_btn.Size = new System.Drawing.Size(144, 23);
            this.bothupdate_btn.TabIndex = 10;
            this.bothupdate_btn.Text = "更新实时值和小时均值";
            this.bothupdate_btn.UseVisualStyleBackColor = true;
            this.bothupdate_btn.Click += new System.EventHandler(this.bothupdate_btn_Click);
            // 
            // stopupdate_btn
            // 
            this.stopupdate_btn.Location = new System.Drawing.Point(227, 271);
            this.stopupdate_btn.Name = "stopupdate_btn";
            this.stopupdate_btn.Size = new System.Drawing.Size(91, 23);
            this.stopupdate_btn.TabIndex = 11;
            this.stopupdate_btn.Text = "停止";
            this.stopupdate_btn.UseVisualStyleBackColor = true;
            this.stopupdate_btn.Click += new System.EventHandler(this.stopupdate_btn_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(290, 383);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(137, 12);
            this.label6.TabIndex = 12;
            this.label6.Text = "智企信息 版权所有 2015";
            // 
            // UpdateprogressBar
            // 
            this.UpdateprogressBar.Location = new System.Drawing.Point(65, 346);
            this.UpdateprogressBar.Name = "UpdateprogressBar";
            this.UpdateprogressBar.Size = new System.Drawing.Size(588, 23);
            this.UpdateprogressBar.Step = 1;
            this.UpdateprogressBar.TabIndex = 13;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 346);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 12);
            this.label7.TabIndex = 14;
            this.label7.Text = "进度:";
            // 
            // percentagelabel
            // 
            this.percentagelabel.AutoSize = true;
            this.percentagelabel.Location = new System.Drawing.Point(672, 346);
            this.percentagelabel.Name = "percentagelabel";
            this.percentagelabel.Size = new System.Drawing.Size(11, 12);
            this.percentagelabel.TabIndex = 15;
            this.percentagelabel.Text = "%";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(500, 288);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(146, 49);
            this.pictureBox1.TabIndex = 16;
            this.pictureBox1.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(711, 403);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.percentagelabel);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.UpdateprogressBar);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.stopupdate_btn);
            this.Controls.Add(this.bothupdate_btn);
            this.Controls.Add(this.avgstatuslabel);
            this.Controls.Add(this.realstatuslabel);
            this.Controls.Add(this.avgupdate_btn);
            this.Controls.Add(this.realupdate_btn);
            this.Controls.Add(this.EnddateTimePicker);
            this.Controls.Add(this.StartdateTimePicker);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.PointListBox);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.ShowIcon = false;
            this.Text = "PIDataUpdate V 0.1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            System.IAsyncResult rst = BeginInvoke(closedl);
            EndInvoke(rst);

            if (logt.ThreadState != System.Threading.ThreadState.Unstarted)
            {
                logt.Abort();
            }
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckedListBox PointListBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker StartdateTimePicker;
        private System.Windows.Forms.DateTimePicker EnddateTimePicker;
        private System.Windows.Forms.Button realupdate_btn;
        private System.Windows.Forms.Button avgupdate_btn;
        private System.Windows.Forms.Label realstatuslabel;
        private System.Windows.Forms.Label avgstatuslabel;
        private System.Windows.Forms.Button bothupdate_btn;
        private System.Windows.Forms.Button stopupdate_btn;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ProgressBar UpdateprogressBar;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label percentagelabel;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

