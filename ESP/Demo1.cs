using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ESP
{
    public partial class Demo1 : Form
    {
        public bool isshow = false;
        public Demo1()
        {
            InitializeComponent();
        }

        private void Demo1_Load(object sender, EventArgs e)
        {
            isshow = true;
        }

        private void Demo1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = FormWindowState.Minimized;
            this.Visible = false;
        }

        private void Demo1_FormClosed(object sender, FormClosedEventArgs e)
        {
            isshow = false;
        }
    }
}