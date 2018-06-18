using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ESP
{
    class SendCommand
    {
        /// <summary>
        /// 处理要发送的数据，十六进制
        /// </summary>
        /// <param name="command"></param>
        /// <param name="fm"></param>
        public static void action(string command, Form1 fm)
        {
            try
            {
                //command = fm.textBox3.Text;
                int digit = command.Length / 2;

                byte[] w = new byte[digit];
                for (int j = 0; j < digit; j++)
                {
                    w[j] = Convert.ToByte(command.Substring(j * 2, 2), 16);
                }
                fm.serialPort1.Write(w, 0, w.Length);

                fm.countC++;
                fm.toolStripStatusLabel3.Text = "|  Commands: " + fm.countC + "　　";
            }
            catch (Exception ex)
            {
                fm.timer1.Enabled = false;
                fm.groupBox4.Enabled = true;
                fm.toolStripEA1.Enabled = true;
                if (fm.checkBox2.Checked == true)
                {
                    fm.checkBox2.Enabled = true;
                    fm.numericUpDown1.Enabled = true;
                }
                else if (fm.checkBox2.Checked == false)
                {
                    fm.checkBox2.Enabled = true;
                }
                if (Form1.languageflag == (int)Form1.Language.English)
                {
                    fm.button1.Text = "Send";
                }
                else if (Form1.languageflag == (int)Form1.Language.Chinese)
                {
                    fm.button1.Text = "发送";
                }
                fm.timer3.Enabled = false;
                fm.groupBox6.Enabled = true;
                if (fm.checkBox4.Checked == true)
                {
                    fm.checkBox4.Enabled = true;
                    fm.numericUpDown2.Enabled = true;
                }
                else if (fm.checkBox4.Checked == false)
                {
                    fm.checkBox4.Enabled = true;
                }
                if (Form1.languageflag == (int)Form1.Language.English)
                {
                    fm.button3.Text = "Send";
                }
                else if (Form1.languageflag == (int)Form1.Language.Chinese)
                {
                    fm.button3.Text = "发送";
                }
                MessageBox.Show(ex.Message);
            }
        }
    }
}
