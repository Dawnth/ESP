using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ESP
{
    class TimerActionOne
    {
        /// <summary>
        /// 手动数据 发送的机制
        /// </summary>
        /// <param name="fm"></param>
        /// <param name="type"></param>
        public static void timerActionOne(Form1 fm, int type)
        {
            try
            {

                if (type == 1)
                {
                    if (Form1.languageflag == (int)Form1.Language.English)
                    {
                        fm.button1.Text = "Stop";
                    }
                    else if (Form1.languageflag == (int)Form1.Language.Chinese)
                    {
                        fm.button1.Text = "停止";
                    }
                }
                if (fm.checkBox1.Checked == true)
                {
                    fm.textBox2.AppendText("\r\n" + "    " + fm.textBox3.Text.Substring(fm.textBox3.Text.LastIndexOf("x") + 1) + "\r\n");
                    SendCommand.action(fm.textBox3.Text.Substring(fm.textBox3.Text.LastIndexOf("x") + 1), fm);
                }
                else
                {
                    //fm.serialPort1.Write(Convert.ToInt32(fm.textBox3.Text.Substring(fm.textBox3.Text.LastIndexOf("x") + 1), 16).ToString("X2"));
                    fm.textBox2.AppendText("\r\n" + "    " + fm.textBox3.Text.Substring(fm.textBox3.Text.LastIndexOf("x") + 1) + "\r\n");
                    fm.serialPort1.Write(fm.textBox3.Text.Substring(fm.textBox3.Text.LastIndexOf("x") + 1));
                    fm.countC++;
                    fm.toolStripStatusLabel3.Text = "|  Commands: " + fm.countC + "　　";
                }

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
                MessageBox.Show(ex.Message);
            }
        }
    }
}
