using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Windows.Forms.Design;
using System.Designer;

namespace ECOVI_SP
{
    [
    ToolboxBitmap(typeof(LineH), "res.LineH.ico"),
    Designer(typeof(LineHDesigner))
    ]

    public class LineH : System.Windows.Forms.UserControl
    {
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
