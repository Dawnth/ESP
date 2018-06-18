using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ESP
{
    public partial class ToolStripEA : ToolStrip
    {
        protected override void WndProc(ref Message m)
        {
            const int WM_MOUSEACTIVATE = 0x21;

            if (m.Msg == WM_MOUSEACTIVATE && this.CanFocus && !this.Focused)
            {
                this.Focus(); 
            }

            base.WndProc(ref m);
        }

    }
}
