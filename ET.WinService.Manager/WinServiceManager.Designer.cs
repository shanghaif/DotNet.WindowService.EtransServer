namespace ET.WinService.Manager
{
    partial class WinServiceManager
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WinServiceManager));
            this.btnInstall = new System.Windows.Forms.Button();
            this.btnUninstall = new System.Windows.Forms.Button();
            this.WSControllerAgent = new System.ServiceProcess.ServiceController();
            this.groupBoxApplicationControlPanel = new System.Windows.Forms.GroupBox();
            this.labelStop = new System.Windows.Forms.Label();
            this.labelPause = new System.Windows.Forms.Label();
            this.labelStartContinue = new System.Windows.Forms.Label();
            this.ButtonStop = new System.Windows.Forms.Button();
            this.ButtonPause = new System.Windows.Forms.Button();
            this.ButtonStart = new System.Windows.Forms.Button();
            this.dgServices = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnEdit = new System.Windows.Forms.Button();
            this.dgTask = new System.Windows.Forms.DataGridView();
            this.lblTrip = new System.Windows.Forms.Label();
            this.groupBoxApplicationControlPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgServices)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgTask)).BeginInit();
            this.SuspendLayout();
            // 
            // btnInstall
            // 
            this.btnInstall.Location = new System.Drawing.Point(210, 4);
            this.btnInstall.Name = "btnInstall";
            this.btnInstall.Size = new System.Drawing.Size(75, 23);
            this.btnInstall.TabIndex = 0;
            this.btnInstall.Text = "Install";
            this.btnInstall.UseVisualStyleBackColor = true;
            this.btnInstall.Click += new System.EventHandler(this.btnInstall_Click);
            // 
            // btnUninstall
            // 
            this.btnUninstall.Location = new System.Drawing.Point(342, 3);
            this.btnUninstall.Name = "btnUninstall";
            this.btnUninstall.Size = new System.Drawing.Size(75, 23);
            this.btnUninstall.TabIndex = 1;
            this.btnUninstall.Text = "Uninstall";
            this.btnUninstall.UseVisualStyleBackColor = true;
            this.btnUninstall.Click += new System.EventHandler(this.btnUninstall_Click);
            // 
            // groupBoxApplicationControlPanel
            // 
            this.groupBoxApplicationControlPanel.Controls.Add(this.labelStop);
            this.groupBoxApplicationControlPanel.Controls.Add(this.labelPause);
            this.groupBoxApplicationControlPanel.Controls.Add(this.labelStartContinue);
            this.groupBoxApplicationControlPanel.Controls.Add(this.ButtonStop);
            this.groupBoxApplicationControlPanel.Controls.Add(this.ButtonPause);
            this.groupBoxApplicationControlPanel.Controls.Add(this.ButtonStart);
            this.groupBoxApplicationControlPanel.Controls.Add(this.dgServices);
            this.groupBoxApplicationControlPanel.Location = new System.Drawing.Point(16, 32);
            this.groupBoxApplicationControlPanel.Name = "groupBoxApplicationControlPanel";
            this.groupBoxApplicationControlPanel.Size = new System.Drawing.Size(594, 183);
            this.groupBoxApplicationControlPanel.TabIndex = 3;
            this.groupBoxApplicationControlPanel.TabStop = false;
            this.groupBoxApplicationControlPanel.Text = "服务列表";
            // 
            // labelStop
            // 
            this.labelStop.AutoSize = true;
            this.labelStop.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStop.Location = new System.Drawing.Point(527, 159);
            this.labelStop.Name = "labelStop";
            this.labelStop.Size = new System.Drawing.Size(36, 16);
            this.labelStop.TabIndex = 58;
            this.labelStop.Text = "Stop";
            // 
            // labelPause
            // 
            this.labelPause.AutoSize = true;
            this.labelPause.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPause.Location = new System.Drawing.Point(304, 159);
            this.labelPause.Name = "labelPause";
            this.labelPause.Size = new System.Drawing.Size(43, 16);
            this.labelPause.TabIndex = 57;
            this.labelPause.Text = "Pause";
            // 
            // labelStartContinue
            // 
            this.labelStartContinue.AutoSize = true;
            this.labelStartContinue.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStartContinue.Location = new System.Drawing.Point(44, 159);
            this.labelStartContinue.Name = "labelStartContinue";
            this.labelStartContinue.Size = new System.Drawing.Size(120, 16);
            this.labelStartContinue.TabIndex = 56;
            this.labelStartContinue.Text = "Start / Continue";
            // 
            // ButtonStop
            // 
            this.ButtonStop.BackColor = System.Drawing.SystemColors.Control;
            this.ButtonStop.Image = ((System.Drawing.Image)(resources.GetObject("ButtonStop.Image")));
            this.ButtonStop.Location = new System.Drawing.Point(496, 152);
            this.ButtonStop.Name = "ButtonStop";
            this.ButtonStop.Size = new System.Drawing.Size(28, 26);
            this.ButtonStop.TabIndex = 54;
            this.ButtonStop.Tag = "Stop";
            this.ButtonStop.UseVisualStyleBackColor = false;
            this.ButtonStop.Click += new System.EventHandler(this.ButtonStop_Click);
            // 
            // ButtonPause
            // 
            this.ButtonPause.BackColor = System.Drawing.SystemColors.Control;
            this.ButtonPause.Image = ((System.Drawing.Image)(resources.GetObject("ButtonPause.Image")));
            this.ButtonPause.Location = new System.Drawing.Point(270, 152);
            this.ButtonPause.Name = "ButtonPause";
            this.ButtonPause.Size = new System.Drawing.Size(28, 26);
            this.ButtonPause.TabIndex = 55;
            this.ButtonPause.Tag = "Pause";
            this.ButtonPause.UseVisualStyleBackColor = false;
            this.ButtonPause.Click += new System.EventHandler(this.ButtonPause_Click);
            // 
            // ButtonStart
            // 
            this.ButtonStart.BackColor = System.Drawing.SystemColors.Control;
            this.ButtonStart.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ButtonStart.Image = ((System.Drawing.Image)(resources.GetObject("ButtonStart.Image")));
            this.ButtonStart.Location = new System.Drawing.Point(10, 152);
            this.ButtonStart.Name = "ButtonStart";
            this.ButtonStart.Size = new System.Drawing.Size(28, 26);
            this.ButtonStart.TabIndex = 53;
            this.ButtonStart.Tag = "Start";
            this.ButtonStart.UseVisualStyleBackColor = false;
            this.ButtonStart.Click += new System.EventHandler(this.ButtonStart_Click);
            // 
            // dgServices
            // 
            this.dgServices.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.dgServices.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgServices.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgServices.Location = new System.Drawing.Point(8, 18);
            this.dgServices.MultiSelect = false;
            this.dgServices.Name = "dgServices";
            this.dgServices.ReadOnly = true;
            this.dgServices.RowTemplate.Height = 23;
            this.dgServices.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgServices.Size = new System.Drawing.Size(580, 129);
            this.dgServices.TabIndex = 2;
            this.dgServices.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dgServices_MouseClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnEdit);
            this.groupBox1.Controls.Add(this.dgTask);
            this.groupBox1.Location = new System.Drawing.Point(16, 223);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(594, 189);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "任务列表";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(530, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 16);
            this.label1.TabIndex = 59;
            // 
            // btnEdit
            // 
            this.btnEdit.BackColor = System.Drawing.SystemColors.Control;
            this.btnEdit.Location = new System.Drawing.Point(508, 10);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(40, 20);
            this.btnEdit.TabIndex = 58;
            this.btnEdit.Tag = "Pause";
            this.btnEdit.Text = "编辑";
            this.btnEdit.UseVisualStyleBackColor = false;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // dgTask
            // 
            this.dgTask.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.dgTask.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgTask.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgTask.Location = new System.Drawing.Point(8, 33);
            this.dgTask.MultiSelect = false;
            this.dgTask.Name = "dgTask";
            this.dgTask.ReadOnly = true;
            this.dgTask.RowTemplate.Height = 23;
            this.dgTask.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgTask.Size = new System.Drawing.Size(580, 147);
            this.dgTask.TabIndex = 2;
            this.dgTask.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgTask_CellDoubleClick);
            // 
            // lblTrip
            // 
            this.lblTrip.AutoSize = true;
            this.lblTrip.Location = new System.Drawing.Point(446, 13);
            this.lblTrip.Name = "lblTrip";
            this.lblTrip.Size = new System.Drawing.Size(101, 12);
            this.lblTrip.TabIndex = 5;
            this.lblTrip.Text = "服务正在安装……";
            this.lblTrip.Visible = false;
            // 
            // WinServiceManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(629, 415);
            this.Controls.Add(this.lblTrip);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBoxApplicationControlPanel);
            this.Controls.Add(this.btnUninstall);
            this.Controls.Add(this.btnInstall);
            this.MaximizeBox = false;
            this.Name = "WinServiceManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WinServiceManager";
            this.Load += new System.EventHandler(this.WinServiceManager_Load);
            this.groupBoxApplicationControlPanel.ResumeLayout(false);
            this.groupBoxApplicationControlPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgServices)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgTask)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnInstall;
        private System.Windows.Forms.Button btnUninstall;
        private System.ServiceProcess.ServiceController WSControllerAgent;
        private System.Windows.Forms.GroupBox groupBoxApplicationControlPanel;
        private System.Windows.Forms.Label labelStop;
        private System.Windows.Forms.Label labelPause;
        private System.Windows.Forms.Label labelStartContinue;
        private System.Windows.Forms.Button ButtonStop;
        private System.Windows.Forms.Button ButtonPause;
        private System.Windows.Forms.Button ButtonStart;
        private System.Windows.Forms.DataGridView dgServices;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgTask;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Label lblTrip;
    }
}