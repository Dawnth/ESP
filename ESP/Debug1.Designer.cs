namespace ESP
{
    partial class Debug1
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Debug1));
            this.oscilloscope1 = new ESP.Oscilloscope();
            this.SuspendLayout();
            // 
            // oscilloscope1
            // 
            this.oscilloscope1.Enabled = false;
            this.oscilloscope1.Interval = 2000;
            this.oscilloscope1.Location = new System.Drawing.Point(12, 12);
            this.oscilloscope1.Name = "oscilloscope1";
            this.oscilloscope1.Size = new System.Drawing.Size(150, 150);
            this.oscilloscope1.TabIndex = 0;
            // 
            // Debug1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(690, 439);
            this.Controls.Add(this.oscilloscope1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Debug1";
            this.Text = "Air condition[Debug]";
            this.Load += new System.EventHandler(this.Debug1_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Debug1_FormClosed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Debug1_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private Oscilloscope oscilloscope1;


    }
}