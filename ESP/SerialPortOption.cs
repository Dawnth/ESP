using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Xml;

namespace ESP
{
    public partial class SerialPortOption : Form
    {
        public Form1 Gz_fm;
        public SerialPortOption(Form1 fm)
        {
            InitializeComponent();
            Gz_fm = fm;
        }

        //用于更新form1种内容
        //public Form1 Gz_fm;
        //public SerialPortOption(Form1 fm)
        //{
        //    InitializeComponent();
        //    Gz_fm = t_Form1;
        //}

        //private void Port_Select()
        //{//获取机器中的串口地址
        //    string[] ports = Gz_fm.serialPort1.GetPortNames();
        //    comboBox1.Items.Clear();
        //    foreach (string port in ports)
        //    {
        //        comboBox1.Items.Add(port);
        //    }
        //}
        //官方标配+飞利浦原装充电器+飞利浦原装运动三件套+SONY挂绳+耳机绵套+森海塞尔耳机线夹+情侣分线器+包邮=275 (推荐）



        private void SerialPortOption_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            getPortName();
            comboBox1.Text = Form1.spconfig[0];
            comboBox2.Text = Form1.spconfig[1];
            comboBox5.Text = Form1.spconfig[2];
            comboBox3.Text = Form1.spconfig[3];
            comboBox4.Text = Form1.spconfig[4];
            textBox1.Text =  Form1.spconfig[5];
            textBox2.Text =  Form1.spconfig[6];
            textBox3.Text =  Form1.spconfig[7];
            textBox4.Text =  Form1.spconfig[8];
        }

        private void getPortName()
        {
            comboBox1.Items.Clear();
            foreach (string s in SerialPort.GetPortNames())
            {
                comboBox1.Items.Add(s);
                comboBox1.Text = s;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            XmlDocument xd = new XmlDocument();
            xd.Load(Application.StartupPath + @"\ESPConfig.cds");
            Form1.spconfig[0] = comboBox1.Text;
            xd.SelectSingleNode("Config/SerialPortConfig/pn").InnerText = comboBox1.Text;
            Form1.spconfig[1] = comboBox2.Text;
            xd.SelectSingleNode("Config/SerialPortConfig/br").InnerText = comboBox2.Text;
            Form1.spconfig[2] = comboBox5.Text;
            xd.SelectSingleNode("Config/SerialPortConfig/py").InnerText = comboBox5.Text;
            Form1.spconfig[3] = comboBox3.Text;
            xd.SelectSingleNode("Config/SerialPortConfig/db").InnerText = comboBox3.Text;
            Form1.spconfig[4] = comboBox4.Text;
            xd.SelectSingleNode("Config/SerialPortConfig/sb").InnerText = comboBox4.Text;
            Form1.spconfig[5] = textBox1.Text;
            xd.SelectSingleNode("Config/SerialPortConfig/rto").InnerText = textBox1.Text;
            Form1.spconfig[6] = textBox2.Text;
            xd.SelectSingleNode("Config/SerialPortConfig/wto").InnerText = textBox2.Text;
            Form1.spconfig[7] = textBox3.Text;
            xd.SelectSingleNode("Config/SerialPortConfig/rbs").InnerText = textBox3.Text;
            Form1.spconfig[8] = textBox4.Text;
            xd.SelectSingleNode("Config/SerialPortConfig/wbs").InnerText = textBox4.Text;
            xd.Save(Application.StartupPath + @"\ESPConfig.cds");
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SerialPortOption_FormClosed(object sender, FormClosedEventArgs e)
        {
            Gz_fm.refresh();

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.CheckState == CheckState.Checked)
            { textBox1.ReadOnly = false; }
            else
            { textBox1.ReadOnly = true; }
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.CheckState == CheckState.Checked)
            { textBox2.ReadOnly = false; }
            else
            { textBox2.ReadOnly = true; }
        }
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.CheckState == CheckState.Checked)
            { textBox3.ReadOnly = false; }
            else
            { textBox3.ReadOnly = true; }
        }
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.CheckState == CheckState.Checked)
            { textBox4.ReadOnly = false; }
            else
            { textBox4.ReadOnly = true; }
        }

    }
}