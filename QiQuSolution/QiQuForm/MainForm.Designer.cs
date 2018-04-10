namespace QiQuForm
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
            this.lblOnlineNumber = new System.Windows.Forms.Label();
            this.lblOnlineNumberValue = new System.Windows.Forms.Label();
            this.lblWinNumberValue = new System.Windows.Forms.Label();
            this.lblWinNumber = new System.Windows.Forms.Label();
            this.rbFirstTwo = new System.Windows.Forms.RadioButton();
            this.rbLastTwo = new System.Windows.Forms.RadioButton();
            this.lblNewestPeriod = new System.Windows.Forms.Label();
            this.lblDingMaCount = new System.Windows.Forms.Label();
            this.dgvWinLog = new System.Windows.Forms.DataGridView();
            this.btnOpenHistory = new System.Windows.Forms.Button();
            this.btnCopyForecastNumberList = new System.Windows.Forms.Button();
            this.lblCurrentStateValue = new System.Windows.Forms.Label();
            this.lblCurrentState = new System.Windows.Forms.Label();
            this.txtNewestPeriod = new System.Windows.Forms.TextBox();
            this.txtDingMaCount = new System.Windows.Forms.TextBox();
            this.btnRefreshData = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvWinLog)).BeginInit();
            this.SuspendLayout();
            // 
            // lblOnlineNumber
            // 
            this.lblOnlineNumber.AutoSize = true;
            this.lblOnlineNumber.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblOnlineNumber.Location = new System.Drawing.Point(126, 18);
            this.lblOnlineNumber.Name = "lblOnlineNumber";
            this.lblOnlineNumber.Size = new System.Drawing.Size(191, 25);
            this.lblOnlineNumber.TabIndex = 0;
            this.lblOnlineNumber.Text = "最新在线人数/分钟：";
            // 
            // lblOnlineNumberValue
            // 
            this.lblOnlineNumberValue.AutoSize = true;
            this.lblOnlineNumberValue.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblOnlineNumberValue.Location = new System.Drawing.Point(323, 18);
            this.lblOnlineNumberValue.Name = "lblOnlineNumberValue";
            this.lblOnlineNumberValue.Size = new System.Drawing.Size(191, 25);
            this.lblOnlineNumberValue.TabIndex = 1;
            this.lblOnlineNumberValue.Text = "最新在线人数/分钟：";
            // 
            // lblWinNumberValue
            // 
            this.lblWinNumberValue.AutoSize = true;
            this.lblWinNumberValue.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblWinNumberValue.Location = new System.Drawing.Point(915, 18);
            this.lblWinNumberValue.Name = "lblWinNumberValue";
            this.lblWinNumberValue.Size = new System.Drawing.Size(107, 25);
            this.lblWinNumberValue.TabIndex = 3;
            this.lblWinNumberValue.Text = "开奖号码：";
            // 
            // lblWinNumber
            // 
            this.lblWinNumber.AutoSize = true;
            this.lblWinNumber.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblWinNumber.Location = new System.Drawing.Point(802, 18);
            this.lblWinNumber.Name = "lblWinNumber";
            this.lblWinNumber.Size = new System.Drawing.Size(107, 25);
            this.lblWinNumber.TabIndex = 2;
            this.lblWinNumber.Text = "开奖号码：";
            // 
            // rbFirstTwo
            // 
            this.rbFirstTwo.AutoSize = true;
            this.rbFirstTwo.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rbFirstTwo.Location = new System.Drawing.Point(214, 69);
            this.rbFirstTwo.Name = "rbFirstTwo";
            this.rbFirstTwo.Size = new System.Drawing.Size(68, 29);
            this.rbFirstTwo.TabIndex = 4;
            this.rbFirstTwo.TabStop = true;
            this.rbFirstTwo.Text = "前二";
            this.rbFirstTwo.UseVisualStyleBackColor = true;
            this.rbFirstTwo.CheckedChanged += new System.EventHandler(this.rbFirstTwo_And_rbLastTwo_CheckedChanged);
            // 
            // rbLastTwo
            // 
            this.rbLastTwo.AutoSize = true;
            this.rbLastTwo.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rbLastTwo.Location = new System.Drawing.Point(131, 69);
            this.rbLastTwo.Name = "rbLastTwo";
            this.rbLastTwo.Size = new System.Drawing.Size(68, 29);
            this.rbLastTwo.TabIndex = 5;
            this.rbLastTwo.TabStop = true;
            this.rbLastTwo.Text = "后二";
            this.rbLastTwo.UseVisualStyleBackColor = true;
            this.rbLastTwo.CheckedChanged += new System.EventHandler(this.rbFirstTwo_And_rbLastTwo_CheckedChanged);
            // 
            // lblNewestPeriod
            // 
            this.lblNewestPeriod.AutoSize = true;
            this.lblNewestPeriod.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblNewestPeriod.Location = new System.Drawing.Point(126, 115);
            this.lblNewestPeriod.Name = "lblNewestPeriod";
            this.lblNewestPeriod.Size = new System.Drawing.Size(164, 25);
            this.lblNewestPeriod.TabIndex = 6;
            this.lblNewestPeriod.Text = "排除最新期数号：";
            // 
            // lblDingMaCount
            // 
            this.lblDingMaCount.AutoSize = true;
            this.lblDingMaCount.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblDingMaCount.Location = new System.Drawing.Point(392, 115);
            this.lblDingMaCount.Name = "lblDingMaCount";
            this.lblDingMaCount.Size = new System.Drawing.Size(107, 25);
            this.lblDingMaCount.TabIndex = 8;
            this.lblDingMaCount.Text = "定码个数：";
            // 
            // dgvWinLog
            // 
            this.dgvWinLog.AllowUserToAddRows = false;
            this.dgvWinLog.AllowUserToDeleteRows = false;
            this.dgvWinLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvWinLog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvWinLog.Location = new System.Drawing.Point(0, 198);
            this.dgvWinLog.Name = "dgvWinLog";
            this.dgvWinLog.ReadOnly = true;
            this.dgvWinLog.RowTemplate.Height = 23;
            this.dgvWinLog.Size = new System.Drawing.Size(1386, 793);
            this.dgvWinLog.TabIndex = 10;
            this.dgvWinLog.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvWinLog_CellDoubleClick);
            this.dgvWinLog.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvWinLog_CellFormatting);
            this.dgvWinLog.Resize += new System.EventHandler(this.dgvWinLog_Resize);
            // 
            // btnOpenHistory
            // 
            this.btnOpenHistory.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnOpenHistory.Location = new System.Drawing.Point(822, 67);
            this.btnOpenHistory.Name = "btnOpenHistory";
            this.btnOpenHistory.Size = new System.Drawing.Size(107, 33);
            this.btnOpenHistory.TabIndex = 11;
            this.btnOpenHistory.Text = "历史记录";
            this.btnOpenHistory.UseVisualStyleBackColor = true;
            this.btnOpenHistory.Visible = false;
            // 
            // btnCopyForecastNumberList
            // 
            this.btnCopyForecastNumberList.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCopyForecastNumberList.Location = new System.Drawing.Point(955, 67);
            this.btnCopyForecastNumberList.Name = "btnCopyForecastNumberList";
            this.btnCopyForecastNumberList.Size = new System.Drawing.Size(107, 33);
            this.btnCopyForecastNumberList.TabIndex = 12;
            this.btnCopyForecastNumberList.Text = "复制计划";
            this.btnCopyForecastNumberList.UseVisualStyleBackColor = true;
            this.btnCopyForecastNumberList.Click += new System.EventHandler(this.btnCopyForecastNumberList_Click);
            // 
            // lblCurrentStateValue
            // 
            this.lblCurrentStateValue.AutoSize = true;
            this.lblCurrentStateValue.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblCurrentStateValue.Location = new System.Drawing.Point(277, 162);
            this.lblCurrentStateValue.Name = "lblCurrentStateValue";
            this.lblCurrentStateValue.Size = new System.Drawing.Size(145, 25);
            this.lblCurrentStateValue.TabIndex = 14;
            this.lblCurrentStateValue.Text = "当前计划状态：";
            // 
            // lblCurrentState
            // 
            this.lblCurrentState.AutoSize = true;
            this.lblCurrentState.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblCurrentState.Location = new System.Drawing.Point(126, 162);
            this.lblCurrentState.Name = "lblCurrentState";
            this.lblCurrentState.Size = new System.Drawing.Size(145, 25);
            this.lblCurrentState.TabIndex = 13;
            this.lblCurrentState.Text = "当前计划状态：";
            // 
            // txtNewestPeriod
            // 
            this.txtNewestPeriod.Font = new System.Drawing.Font("微软雅黑", 14.25F);
            this.txtNewestPeriod.Location = new System.Drawing.Point(296, 112);
            this.txtNewestPeriod.Name = "txtNewestPeriod";
            this.txtNewestPeriod.Size = new System.Drawing.Size(67, 33);
            this.txtNewestPeriod.TabIndex = 15;
            this.txtNewestPeriod.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtNewestPeriod.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtNewestPeriod_txtDingMaCount_KeyPress);
            // 
            // txtDingMaCount
            // 
            this.txtDingMaCount.Font = new System.Drawing.Font("微软雅黑", 14.25F);
            this.txtDingMaCount.Location = new System.Drawing.Point(505, 112);
            this.txtDingMaCount.Name = "txtDingMaCount";
            this.txtDingMaCount.Size = new System.Drawing.Size(59, 33);
            this.txtDingMaCount.TabIndex = 16;
            this.txtDingMaCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtDingMaCount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtNewestPeriod_txtDingMaCount_KeyPress);
            // 
            // btnRefreshData
            // 
            this.btnRefreshData.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnRefreshData.Location = new System.Drawing.Point(592, 111);
            this.btnRefreshData.Name = "btnRefreshData";
            this.btnRefreshData.Size = new System.Drawing.Size(107, 33);
            this.btnRefreshData.TabIndex = 17;
            this.btnRefreshData.Text = "刷新数据";
            this.btnRefreshData.UseVisualStyleBackColor = true;
            this.btnRefreshData.Click += new System.EventHandler(this.btnRefreshData_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1386, 991);
            this.Controls.Add(this.btnRefreshData);
            this.Controls.Add(this.txtDingMaCount);
            this.Controls.Add(this.txtNewestPeriod);
            this.Controls.Add(this.lblCurrentStateValue);
            this.Controls.Add(this.lblCurrentState);
            this.Controls.Add(this.btnCopyForecastNumberList);
            this.Controls.Add(this.btnOpenHistory);
            this.Controls.Add(this.dgvWinLog);
            this.Controls.Add(this.lblDingMaCount);
            this.Controls.Add(this.lblNewestPeriod);
            this.Controls.Add(this.rbLastTwo);
            this.Controls.Add(this.rbFirstTwo);
            this.Controls.Add(this.lblWinNumberValue);
            this.Controls.Add(this.lblWinNumber);
            this.Controls.Add(this.lblOnlineNumberValue);
            this.Controls.Add(this.lblOnlineNumber);
            this.Name = "MainForm";
            this.Text = "奇趣";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvWinLog)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblOnlineNumber;
        private System.Windows.Forms.Label lblOnlineNumberValue;
        private System.Windows.Forms.Label lblWinNumberValue;
        private System.Windows.Forms.Label lblWinNumber;
        private System.Windows.Forms.RadioButton rbFirstTwo;
        private System.Windows.Forms.RadioButton rbLastTwo;
        private System.Windows.Forms.Label lblNewestPeriod;
        private System.Windows.Forms.Label lblDingMaCount;
        private System.Windows.Forms.DataGridView dgvWinLog;
        private System.Windows.Forms.Button btnOpenHistory;
        private System.Windows.Forms.Button btnCopyForecastNumberList;
        private System.Windows.Forms.Label lblCurrentStateValue;
        private System.Windows.Forms.Label lblCurrentState;
        private System.Windows.Forms.TextBox txtNewestPeriod;
        private System.Windows.Forms.TextBox txtDingMaCount;
        private System.Windows.Forms.Button btnRefreshData;
    }
}

