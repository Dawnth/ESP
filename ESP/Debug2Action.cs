using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ESP
{
    class Debug2Action
    {
        public static void debug2Action(Debug2 fm, string mystring, string hex)
        {
            try
            {
                if (hex == "true")
                {
                    if (mystring.Length != 0)
                    {
                        Debug2SendCommand.action(mystring.Substring(mystring.LastIndexOf("x") + 1), fm);
                    }
                    else
                    {
                        MessageBox.Show("No to-be-sent data!");
                    }
                }
                else if (hex == "false")
                {
                    fm.serialPort1.Write(mystring);
                }
            }
            catch (Exception)
            {
                //MessageBox.Show("Procedure Occurrence Mistake:" + ex.Message, "Anomalous", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
