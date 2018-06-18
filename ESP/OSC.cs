using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ESP
{
    public partial class OSC : Form
    {
        double i = 0;
        double j = 0;
        double j2 = 0;

        //用于保存Form打开的状态
        public bool isshow = false;

        public OSC()
        {
            InitializeComponent();
        }

        private void OSC_Load(object sender, EventArgs e)
        {
            isshow = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (; i <= 490; )
            {
              //创建一个Graphics对象
              Graphics grp = CreateGraphics();
              //设置抗锯齿
              grp.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
              //绘制线条 起始窗口坐标10,50 终止坐标20,350 红色，2px宽
              //grp.DrawLine(new Pen(Color.Red, 2), new Point(i += 20, j += 20), new Point(250, 180));
              j = System.Math.Sin(i += 0.1);
              j2 = System.Math.Sin(i - 0.1);
              grp.DrawLine(new Pen(Color.Red, 1), new Point(Convert.ToInt32(i - 0.1) * 20, Convert.ToInt32(j2) * 20 + 180), new Point(Convert.ToInt32(i) * 20, Convert.ToInt32(j) * 20 + 180));
              //释放资源
              grp.Dispose();
              textBox1.Text = Convert.ToString(j);
            }
            if (i >= 490)
            {
                i = 0;
            }
        }

        private void OSC_FormClosed(object sender, FormClosedEventArgs e)
        {
            isshow = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //画圆：无填充色
            Graphics gra1 = CreateGraphics();
            Pen pen = new Pen(Color.GreenYellow,4);//画笔颜色
            gra1.DrawEllipse(pen, 100, 100, 100, 100);//画椭圆的方法，x坐标、y坐标、宽、高，如果是100，则半径为50



            //画圆：有填充色
            Graphics gra2 = CreateGraphics();
            Brush bush = new SolidBrush(Color.Gold);//填充的颜色
            gra2.FillEllipse(bush, 10, 10, 100, 100);//画填充椭圆的方法，x坐标、y坐标、宽、高，如果是100，则半径为50
            gra1.Dispose();
            gra2.Dispose();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Graphics gra = CreateGraphics();
            int x1, x2;
            double y1, y2;
            double a;
            Pen myPen = new Pen(Color.Blue, 3);

            x1 = x2 = 0;
            y1 = y2 = this.ClientSize.Height / 2;
            for (x2 = 0; x2 < this.ClientSize.Width; x2++)
            {
                a = 2 * Math.PI * x2 / this.ClientSize.Width;
                y2 = Math.Sin(a);
                y2 = (1 - y2) * this.ClientSize.Height / 2;
                gra.DrawLine(myPen, x1, (float)y1, x2, (float)y2);
                x1 = x2;
                y1 = y2;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //textBox1.Text = Convert.ToDouble(Convert.ToInt32(textBox2.Text,16)).ToString("X");
        }
    }
}