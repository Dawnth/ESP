using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ESP
{
    class Debug2SendCommand
    {
        public static void action(string command, Debug2 fm)
        {
            try
            {
                int digit = command.Length / 2;

                byte[] w = new byte[digit];
                for (int j = 0; j < digit; j++)
                {
                    w[j] = Convert.ToByte(command.Substring(j * 2, 2), 16);
                }
                fm.serialPort1.Write(w, 0, w.Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
