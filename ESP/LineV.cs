using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ESP
{
    public partial class LineV : UserControl
    {
        public LineV()
        {
            InitializeComponent();
        }
        private void LineV_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {

            Graphics g = e.Graphics;

            Rectangle r = this.ClientRectangle;



            Pen darkPen = new Pen(SystemColors.ControlDark, 1);

            Pen lightPen = new Pen(Color.White);



            //用暗色调处理左边缘

            g.DrawLine(darkPen, r.Left, r.Top, r.Left, r.Bottom);



            //用亮色调处理右边缘
            g.DrawLine(lightPen, r.Left + 1, r.Top, r.Left + 1, r.Bottom);

        }
    }
}
