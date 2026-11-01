namespace ChipAgingCheck.UI
{
    partial class MainForm
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblRecipeCaption = new System.Windows.Forms.Label();
            this.cmbRecipe = new System.Windows.Forms.ComboBox();
            this.btnRecipeParameters = new System.Windows.Forms.Button();
            this.lblDeviceCaption = new System.Windows.Forms.Label();
            this.cmbDevice = new System.Windows.Forms.ComboBox();
            this.btnDeviceParam = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnAlarmConfirm = new System.Windows.Forms.Button();
            this.lblRunState = new System.Windows.Forms.Label();
            this.lblTemp = new System.Windows.Forms.Label();
            this.lblConnCount = new System.Windows.Forms.Label();
            this.lblBatch = new System.Windows.Forms.Label();
            this.lblRunTime = new System.Windows.Forms.Label();
            this.lblGridTitle = new System.Windows.Forms.Label();
            this.GridPanel = new System.Windows.Forms.Panel();
            this.lblAlarmTitle = new System.Windows.Forms.Label();
            this.txtAlarm = new System.Windows.Forms.TextBox();
            this.lblLogTitle = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("阿里巴巴普惠体 3.0 65 Medium", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTitle.Location = new System.Drawing.Point(16, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(220, 40);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "芯片老化检测";
            // 
            // lblRecipeCaption
            // 
            this.lblRecipeCaption.AutoSize = true;
            this.lblRecipeCaption.Location = new System.Drawing.Point(16, 66);
            this.lblRecipeCaption.Name = "lblRecipeCaption";
            this.lblRecipeCaption.Size = new System.Drawing.Size(60, 28);
            this.lblRecipeCaption.TabIndex = 1;
            this.lblRecipeCaption.Text = "配方";
            // 
            // cmbRecipe
            // 
            this.cmbRecipe.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRecipe.Location = new System.Drawing.Point(86, 60);
            this.cmbRecipe.Name = "cmbRecipe";
            this.cmbRecipe.Size = new System.Drawing.Size(220, 36);
            this.cmbRecipe.TabIndex = 2;
            // 
            // btnRecipeParameters
            // 
            this.btnRecipeParameters.Location = new System.Drawing.Point(318, 58);
            this.btnRecipeParameters.Name = "btnRecipeParameters";
            this.btnRecipeParameters.Size = new System.Drawing.Size(130, 40);
            this.btnRecipeParameters.TabIndex = 3;
            this.btnRecipeParameters.Text = "配方配置";
            this.btnRecipeParameters.UseVisualStyleBackColor = true;
            // 
            // lblDeviceCaption
            // 
            this.lblDeviceCaption.AutoSize = true;
            this.lblDeviceCaption.Location = new System.Drawing.Point(470, 66);
            this.lblDeviceCaption.Name = "lblDeviceCaption";
            this.lblDeviceCaption.Size = new System.Drawing.Size(60, 28);
            this.lblDeviceCaption.TabIndex = 4;
            this.lblDeviceCaption.Text = "设备";
            // 
            // cmbDevice
            // 
            this.cmbDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDevice.Location = new System.Drawing.Point(540, 60);
            this.cmbDevice.Name = "cmbDevice";
            this.cmbDevice.Size = new System.Drawing.Size(200, 36);
            this.cmbDevice.TabIndex = 5;
            // 
            // btnDeviceParam
            // 
            this.btnDeviceParam.Location = new System.Drawing.Point(752, 58);
            this.btnDeviceParam.Name = "btnDeviceParam";
            this.btnDeviceParam.Size = new System.Drawing.Size(130, 40);
            this.btnDeviceParam.TabIndex = 6;
            this.btnDeviceParam.Text = "设备配置";
            this.btnDeviceParam.UseVisualStyleBackColor = true;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(16, 112);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(140, 52);
            this.btnStart.TabIndex = 7;
            this.btnStart.Text = "启动";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(172, 112);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(140, 52);
            this.btnStop.TabIndex = 8;
            this.btnStop.Text = "停止";
            this.btnStop.UseVisualStyleBackColor = true;
            // 
            // lblRunState
            // 
            this.lblRunState.AutoSize = true;
            this.lblRunState.Location = new System.Drawing.Point(492, 124);
            this.lblRunState.Name = "lblRunState";
            this.lblRunState.Size = new System.Drawing.Size(140, 28);
            this.lblRunState.TabIndex = 9;
            this.lblRunState.Text = "状态：未运行";
            // 
            // lblTemp
            // 
            this.lblTemp.AutoSize = true;
            this.lblTemp.Location = new System.Drawing.Point(900, 66);
            this.lblTemp.Name = "lblTemp";
            this.lblTemp.Size = new System.Drawing.Size(160, 28);
            this.lblTemp.TabIndex = 10;
            this.lblTemp.Text = "炉温：--.- ℃";
            // 
            // lblConnCount
            // 
            this.lblConnCount.AutoSize = true;
            this.lblConnCount.Location = new System.Drawing.Point(1090, 66);
            this.lblConnCount.Name = "lblConnCount";
            this.lblConnCount.Size = new System.Drawing.Size(160, 28);
            this.lblConnCount.TabIndex = 11;
            this.lblConnCount.Text = "在线：0 / 8 台";
            // 
            // lblBatch
            // 
            this.lblBatch.AutoSize = true;
            this.lblBatch.Location = new System.Drawing.Point(1090, 124);
            this.lblBatch.Name = "lblBatch";
            this.lblBatch.Size = new System.Drawing.Size(160, 28);
            this.lblBatch.TabIndex = 12;
            this.lblBatch.Text = "批次：未开始";
            // 
            // lblRunTime
            // 
            this.lblRunTime.AutoSize = true;
            this.lblRunTime.Location = new System.Drawing.Point(900, 124);
            this.lblRunTime.Name = "lblRunTime";
            this.lblRunTime.Size = new System.Drawing.Size(160, 28);
            this.lblRunTime.TabIndex = 13;
            this.lblRunTime.Text = "运行：00:00:00";
            // 
            // lblGridTitle
            // 
            this.lblGridTitle.AutoSize = true;
            this.lblGridTitle.Location = new System.Drawing.Point(16, 178);
            this.lblGridTitle.Name = "lblGridTitle";
            this.lblGridTitle.Size = new System.Drawing.Size(560, 28);
            this.lblGridTitle.TabIndex = 14;
            this.lblGridTitle.Text = "512 路实时状态（8 台 × 64 路）：绿=正常  红=报警  灰=离线";
            // 
            // GridPanel
            // 
            this.GridPanel.BackColor = System.Drawing.Color.White;
            this.GridPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.GridPanel.Location = new System.Drawing.Point(16, 212);
            this.GridPanel.Name = "GridPanel";
            this.GridPanel.Size = new System.Drawing.Size(1268, 240);
            this.GridPanel.TabIndex = 15;
            this.GridPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.GridPanel_Paint);
            // 
            // lblAlarmTitle
            // 
            this.lblAlarmTitle.AutoSize = true;
            this.lblAlarmTitle.Location = new System.Drawing.Point(16, 464);
            this.lblAlarmTitle.Name = "lblAlarmTitle";
            this.lblAlarmTitle.Size = new System.Drawing.Size(140, 28);
            this.lblAlarmTitle.TabIndex = 16;
            this.lblAlarmTitle.Text = "报警信息";
            // 
            // txtAlarm
            // 
            this.txtAlarm.BackColor = System.Drawing.Color.White;
            this.txtAlarm.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtAlarm.Location = new System.Drawing.Point(16, 496);
            this.txtAlarm.Multiline = true;
            this.txtAlarm.Name = "txtAlarm";
            this.txtAlarm.ReadOnly = true;
            this.txtAlarm.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAlarm.Size = new System.Drawing.Size(620, 230);
            this.txtAlarm.TabIndex = 17;
            // 
            // lblLogTitle
            // 
            this.lblLogTitle.AutoSize = true;
            this.lblLogTitle.Location = new System.Drawing.Point(652, 464);
            this.lblLogTitle.Name = "lblLogTitle";
            this.lblLogTitle.Size = new System.Drawing.Size(140, 28);
            this.lblLogTitle.TabIndex = 18;
            this.lblLogTitle.Text = "运行日志";
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.Color.White;
            this.txtLog.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtLog.Location = new System.Drawing.Point(652, 496);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(632, 230);
            this.txtLog.TabIndex = 19;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblStatus.ForeColor = System.Drawing.Color.DimGray;
            this.lblStatus.Location = new System.Drawing.Point(16, 734);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(500, 24);
            this.lblStatus.TabIndex = 20;
            this.lblStatus.Text = "就绪";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 28F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1300, 768);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.lblLogTitle);
            this.Controls.Add(this.txtAlarm);
            this.Controls.Add(this.lblAlarmTitle);
            this.Controls.Add(this.GridPanel);
            this.Controls.Add(this.lblGridTitle);
            this.Controls.Add(this.lblRunTime);
            this.Controls.Add(this.lblBatch);
            this.Controls.Add(this.lblConnCount);
            this.Controls.Add(this.lblTemp);
            this.Controls.Add(this.lblRunState);
            this.Controls.Add(this.btnAlarmConfirm);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnDeviceParam);
            this.Controls.Add(this.cmbDevice);
            this.Controls.Add(this.lblDeviceCaption);
            this.Controls.Add(this.btnRecipeParameters);
            this.Controls.Add(this.cmbRecipe);
            this.Controls.Add(this.lblRecipeCaption);
            this.Controls.Add(this.lblTitle);
            this.Font = new System.Drawing.Font("阿里巴巴普惠体 3.0 65 Medium", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "芯片老化检测";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblRecipeCaption;
        private System.Windows.Forms.ComboBox cmbRecipe;
        private System.Windows.Forms.Button btnRecipeParameters;
        private System.Windows.Forms.Label lblDeviceCaption;
        private System.Windows.Forms.ComboBox cmbDevice;
        private System.Windows.Forms.Button btnDeviceParam;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnAlarmConfirm;
        private System.Windows.Forms.Label lblRunState;
        private System.Windows.Forms.Label lblTemp;
        private System.Windows.Forms.Label lblConnCount;
        private System.Windows.Forms.Label lblBatch;
        private System.Windows.Forms.Label lblRunTime;
        private System.Windows.Forms.Label lblGridTitle;
        private System.Windows.Forms.Panel GridPanel;
        private System.Windows.Forms.Label lblAlarmTitle;
        private System.Windows.Forms.TextBox txtAlarm;
        private System.Windows.Forms.Label lblLogTitle;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Label lblStatus;
    }
}