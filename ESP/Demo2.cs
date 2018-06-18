using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ESP
{
    public partial class Demo2 : Form
    {
        public bool isshow = false;
        public Demo2()
        {
            InitializeComponent();
        }

        private void Demo2_Load(object sender, EventArgs e)
        {
            isshow = true;
        }

        private void Demo2_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = FormWindowState.Minimized;
            this.Visible = false;
        }

        private void Demo2_FormClosed(object sender, FormClosedEventArgs e)
        {
            isshow = false;
        }
    }
}