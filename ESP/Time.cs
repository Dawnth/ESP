using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ESP
{
    public partial class Time : Form
    {
        int ck = 0;//clockButton's count
        int tr = 0;//timerButton's count
        //int msc = 0;//clock's millisecond
        int sc = 0;//clock's second
        int mc = 0;//clock's minute
        int hc = 0;//clock's hour
        //int mst = 0;//timer's millisecond
        int st = 0;//timer's second
        int mt = 0;//timer's minute
        int ht = 0;//timer's hour

        public bool isshow = false;

        public Time()
        {
            InitializeComponent();
        }

        private void Time_Load(object sender, EventArgs e)
        {
            //this.Visible = true;
            this.MaximizeBox = false;
            //this.MinimizeBox = false;
            isshow = true;
            comboBox1.Text = "00";
            comboBox2.Text = "00";
            comboBox3.Text = "00";
            this.TopMost = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ck++;
            if (ck % 2 != 0)
            {
                timer1.Enabled = true;
                button1.Text = "Stop";
            }
            else
            {
                timer1.Enabled = false;
                button1.Text = "Start";
                if (ck == 2)
                {
                    ck = 0;
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            sc++;
            if (sc < 10)
            {
                label3.Text = "0" + Convert.ToString(sc);
            }
            else if (sc >= 10 && sc <= 59)
            {
                label3.Text = Convert.ToString(sc);
            }
            else if (sc == 60)
            {
                label3.Text = "00";
                mc++;
                if (mc < 10)
                {
                    label2.Text = "0" + Convert.ToString(mc);
                }
                else if (mc >= 10 && mc <= 59)
                {
                    label2.Text = Convert.ToString(mc);
                }
                else if (mc == 60)
                {
                    label2.Text = "00";
                    hc++;
                    if (hc < 10)
                    {
                        label1.Text = "0" + Convert.ToString(hc);
                    }
                    else if (hc >= 10 && hc <= 99)
                    {
                        label1.Text = Convert.ToString(hc);
                    }
                    else if (hc == 100)
                    {
                        hc = 0;
                    }
                    mc = 0;
                }
                sc = 0;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            sc = 0;
            mc = 0;
            hc = 0;
            label1.Text = "00";
            label2.Text = "00";
            label3.Text = "00";
            ck = 0;
            button1.Text = "Start";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            label10.Text = comboBox1.Text;
            label9.Text = comboBox2.Text;
            label8.Text = comboBox3.Text;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            label10.Text = comboBox1.Text;
            label9.Text = comboBox2.Text;
            label8.Text = comboBox3.Text;
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            label10.Text = comboBox1.Text;
            label9.Text = comboBox2.Text;
            label8.Text = comboBox3.Text;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            st = Convert.ToInt32(label8.Text);
            mt = Convert.ToInt32(label9.Text);
            ht = Convert.ToInt32(label10.Text);
            if (st != 0 || mt != 0 || ht != 0)
            {
                tr++;
                if (tr % 2 != 0)
                {
                    timer2.Enabled = true;
                    button4.Text = "Stop";
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = false;
                    comboBox3.Enabled = false;
                }
                else
                {
                    timer2.Enabled = false;
                    button4.Text = "Start";
                    comboBox1.Enabled = true;
                    comboBox2.Enabled = true;
                    comboBox3.Enabled = true;
                    if (tr == 2)
                    {
                        tr = 0;
                    }
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (st > 0)
            {
                st--;
                if (st >= 10 && st <= 59)
                {
                    label8.Text = Convert.ToString(st);
                }
                else if (st >= 0 && st <= 9)
                {
                    label8.Text = "0" + Convert.ToString(st);
                }
            }
            else if (st == 0)
            {
                if (mt > 0)
                {
                    mt--;
                    st = 59;
                    if (mt >= 10 && mt <= 59)
                    {
                        label9.Text = Convert.ToString(mt);
                        label8.Text = Convert.ToString(st); 
                    }
                    else if (mt >= 0 && mt <= 9)
                    {
                        label9.Text = "0" + Convert.ToString(mt);
                        label8.Text = Convert.ToString(st);
                    }
                }
                else if (mt == 0)
                {
                    if (ht > 0)
                    {
                        ht--;
                        mt = 59;
                        st = 59;
                        if (ht >= 10 && ht <= 99)
                        {
                            label10.Text = Convert.ToString(ht);
                            label9.Text = Convert.ToString(mt);
                            label8.Text = Convert.ToString(st); 
                        }
                        else if (ht >= 0 && ht <= 9)
                        {
                            label10.Text = "0" + Convert.ToString(ht);
                            label9.Text = Convert.ToString(mt);
                            label8.Text = Convert.ToString(st); 
                        }

                    }
                    else if (ht == 0)
                    {
                        timer2.Enabled = false;
                        timer2.Enabled = false;
                        st = 0;
                        mt = 0;
                        ht = 0;
                        label10.Text = "00";
                        label9.Text = "00";
                        label8.Text = "00";
                        tr = 0;
                        button4.Text = "Start";
                        comboBox1.Text = "00";
                        comboBox2.Text = "00";
                        comboBox3.Text = "00";
                        comboBox1.Enabled = true;
                        comboBox2.Enabled = true;
                        comboBox3.Enabled = true;
                        MessageBox.Show("预订时间到","提示",MessageBoxButtons.OK);
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer2.Enabled = false;
            st = 0;
            mt = 0;
            ht = 0;
            label10.Text = "00";
            label9.Text = "00";
            label8.Text = "00";
            tr = 0;
            button4.Text = "Start";
            comboBox1.Text = "00";
            comboBox2.Text = "00";
            comboBox3.Text = "00";
            comboBox1.Enabled = true;
            comboBox2.Enabled = true;
            comboBox3.Enabled = true;
        }

        private void Time_FormClosed(object sender, FormClosedEventArgs e)
        {
            isshow = false;
            
        }

        private void Time_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = FormWindowState.Minimized;
            this.Visible = false;
        }

    }
}