using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ESP
{
    public partial class LineH : UserControl
    {
        public LineH()
        {
            InitializeComponent();
        }
        private void LineH_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {

            Graphics g = e.Graphics;

            Rectangle r = this.ClientRectangle;



            Pen darkPen = new Pen(SystemColors.ControlDark, 1);

            Pen LightPen = new Pen(Color.White);



            //用暗色调处理上边缘

            g.DrawLine(darkPen, r.Left, r.Top, r.Right, r.Top);



            //用亮色调处理下边缘

            g.DrawLine(LightPen, r.Left, r.Top + 1, r.Right, r.Top + 1);

        }
    }
}
