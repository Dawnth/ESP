using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ESP
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        private void About_Load(object sender, EventArgs e)
        {
            //原始大小 371, 246
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            //splitContainer1.Panel2Collapsed = false;
            label5.Font = new System.Drawing.Font("Marlett", 9, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)2);
            label5.Text = "4";
            label5.Cursor = Cursors.Hand;
            toolTip1.SetToolTip(label5, "显示更新记录");
            this.Size = new System.Drawing.Size(371, 246);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label5_Click(object sender, EventArgs e)
        {
            if (label5.Text == "3")
            {
                label5.Text = "4";
                toolTip1.SetToolTip(label5, "显示更新记录");
                //splitContainer1.Panel2Collapsed = true;
                this.Size = new System.Drawing.Size(371, 246);
            }
            else
            {
                label5.Text = "3";
                toolTip1.SetToolTip(label5, "隐藏更新记录");
                //splitContainer1.Panel2Collapsed = false;
                this.Size = new System.Drawing.Size(627, 246);
            }
        }
    }
}