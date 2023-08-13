namespace DueDisplaySimulator
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DisplayBox = new PictureBox();
            groupBox1 = new GroupBox();
            CmbSelectDevice = new ComboBox();
            BtnClear = new Button();
            BtnExecute = new Button();
            TxtInput = new TextBox();
            groupBox2 = new GroupBox();
            TxtStatus = new StatusStrip();
            StatusLbl = new ToolStripStatusLabel();
            groupBox3 = new GroupBox();
            TxtConsole = new TextBox();
            ((System.ComponentModel.ISupportInitialize)DisplayBox).BeginInit();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            TxtStatus.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // DisplayBox
            // 
            DisplayBox.BorderStyle = BorderStyle.FixedSingle;
            DisplayBox.Location = new Point(23, 22);
            DisplayBox.Name = "DisplayBox";
            DisplayBox.Size = new Size(240, 180);
            DisplayBox.TabIndex = 0;
            DisplayBox.TabStop = false;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(CmbSelectDevice);
            groupBox1.Controls.Add(BtnClear);
            groupBox1.Controls.Add(BtnExecute);
            groupBox1.Controls.Add(TxtInput);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(572, 226);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Input Due Script";
            // 
            // CmbSelectDevice
            // 
            CmbSelectDevice.FormattingEnabled = true;
            CmbSelectDevice.Location = new Point(6, 29);
            CmbSelectDevice.Name = "CmbSelectDevice";
            CmbSelectDevice.Size = new Size(135, 23);
            CmbSelectDevice.TabIndex = 3;
            // 
            // BtnClear
            // 
            BtnClear.Location = new Point(410, 192);
            BtnClear.Name = "BtnClear";
            BtnClear.Size = new Size(75, 23);
            BtnClear.TabIndex = 2;
            BtnClear.Text = "&Clear";
            BtnClear.UseVisualStyleBackColor = true;
            // 
            // BtnExecute
            // 
            BtnExecute.Location = new Point(491, 192);
            BtnExecute.Name = "BtnExecute";
            BtnExecute.Size = new Size(75, 23);
            BtnExecute.TabIndex = 1;
            BtnExecute.Text = "&Execute";
            BtnExecute.UseVisualStyleBackColor = true;
            // 
            // TxtInput
            // 
            TxtInput.Location = new Point(6, 58);
            TxtInput.Multiline = true;
            TxtInput.Name = "TxtInput";
            TxtInput.Size = new Size(560, 128);
            TxtInput.TabIndex = 0;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(DisplayBox);
            groupBox2.Location = new Point(12, 244);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(283, 209);
            groupBox2.TabIndex = 2;
            groupBox2.TabStop = false;
            groupBox2.Text = "Output Display";
            // 
            // TxtStatus
            // 
            TxtStatus.Items.AddRange(new ToolStripItem[] { StatusLbl });
            TxtStatus.Location = new Point(0, 456);
            TxtStatus.Name = "TxtStatus";
            TxtStatus.Size = new Size(594, 22);
            TxtStatus.TabIndex = 3;
            TxtStatus.Text = "Welcome to Due Display Simulator";
            // 
            // StatusLbl
            // 
            StatusLbl.Name = "StatusLbl";
            StatusLbl.Size = new Size(190, 17);
            StatusLbl.Text = "Welcome to Due Display Simulator";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(TxtConsole);
            groupBox3.Location = new Point(301, 244);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(283, 209);
            groupBox3.TabIndex = 4;
            groupBox3.TabStop = false;
            groupBox3.Text = "Output Console";
            // 
            // TxtConsole
            // 
            TxtConsole.Location = new Point(6, 27);
            TxtConsole.Multiline = true;
            TxtConsole.Name = "TxtConsole";
            TxtConsole.Size = new Size(271, 171);
            TxtConsole.TabIndex = 0;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(594, 478);
            Controls.Add(groupBox3);
            Controls.Add(TxtStatus);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Name = "Form1";
            Text = "Due Display Simulator v0.1";
            ((System.ComponentModel.ISupportInitialize)DisplayBox).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            TxtStatus.ResumeLayout(false);
            TxtStatus.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox DisplayBox;
        private GroupBox groupBox1;
        private Button BtnClear;
        private Button BtnExecute;
        private TextBox TxtInput;
        private GroupBox groupBox2;
        private StatusStrip TxtStatus;
        private ToolStripStatusLabel StatusLbl;
        private GroupBox groupBox3;
        private TextBox TxtConsole;
        private ComboBox CmbSelectDevice;
    }
}