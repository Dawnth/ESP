using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ESP
{
    class TimerActionTwo
    {
        /// <summary>
        /// 自动数据 发送的机制
        /// </summary>
        /// <param name="fm"></param>
        /// <param name="type"></param>
        public static void timerActionTwo(Form1 fm, int type,string mystring)
        {
            //int num = 0;
            try
            {
                //判断长度不为零
                if (mystring.Length != 0)
                {
                    //发送16进制
                    if (fm.checkBox5.Checked == true)
                    {
                        //fm.textBox2.AppendText("\r\n" + "    " + mystring + "\r\n");
                        fm.textBox2.AppendText("\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "  " + mystring + "\r\n" + "                         ");
                        SendCommand.action(mystring.Substring(mystring.LastIndexOf("x") + 1), fm);
                    }
                    //发送ASCII码
                    //else if (fm.checkBox5.Checked == false)
                    else
                    {
                        fm.textBox2.AppendText("\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "  " + mystring + "\r\n" + "                         ");
                        fm.serialPort1.Write(mystring.Substring(mystring.LastIndexOf("x") + 1));
                        //fm.serialPort1.Write(Convert.ToUInt64(fm.label1.Text.Substring(fm.label1.Text.LastIndexOf("x") + 1), 16).ToString("X2"));
                        fm.countC++;
                        fm.toolStripStatusLabel3.Text = "|  Commands: " + fm.countC + "　　";
                    }

                    if (type == 1)
                    {
                        if (Form1.languageflag == (int)Form1.Language.English)
                        {
                            fm.button3.Text = "Stop";
                        }
                        else if (Form1.languageflag == (int)Form1.Language.Chinese)
                        {
                            fm.button3.Text = "停止";
                        }
                        fm.groupBox6.Enabled = false;
                        fm.toolStripEA1.Enabled = false;
                    }
                }
                else
                {
                    if (type == 1)
                    {
                        fm.timer3.Enabled = false;
                        fm.groupBox6.Enabled = true;
                        fm.toolStripEA1.Enabled = true;
                        if (Form1.languageflag == (int)Form1.Language.English)
                        {
                            fm.button3.Text = "Send";
                        }
                        else if (Form1.languageflag == (int)Form1.Language.Chinese)
                        {
                            fm.button3.Text = "发送";
                        }
                    }
                    MessageBox.Show("No to-be-sent data!");
                }

                #region 老代码！！！
#if false
                if (fm.label1.Text.Length != 0)
                {
                    //Convert.ToInt32(fm.label1.Text.Substring(fm.label1.Text.LastIndexOf("x") + 1), 16).ToString("X2");
                    //Convert.ToInt32(fm.textBox1.Text.Substring(fm.textBox1.Text.LastIndexOf("x") + 1), 16).ToString("X2");
                    if (fm.checkBox5.Checked == true)
                    {
                        SendCommand.action(Convert.ToUInt64(fm.label1.Text.Substring(fm.label1.Text.LastIndexOf("x") + 1), 16).ToString("X2"), fm);
                    }
                    else if (fm.checkBox5.Checked == false)
                    {
                        fm.serialPort1.Write(fm.label1.Text.Substring(fm.label1.Text.LastIndexOf("x") + 1));
                        //fm.serialPort1.Write(Convert.ToUInt64(fm.label1.Text.Substring(fm.label1.Text.LastIndexOf("x") + 1), 16).ToString("X2"));
                        fm.countC++;
                        fm.toolStripStatusLabel3.Text = "|  Commands: " + fm.countC + "　　";
                    }
                }
                else
                {
                    num++;
                }
                if (fm.label2.Text.Length != 0)
                {
                    if (fm.checkBox5.Checked == true)
                    {
                        SendCommand.action(Convert.ToUInt64(fm.label2.Text.Substring(fm.label2.Text.LastIndexOf("x") + 1), 16).ToString("X2"), fm);
                    }
                    else if (fm.checkBox5.Checked == false)
                    {
                        fm.serialPort1.Write(Convert.ToUInt64(fm.label2.Text.Substring(fm.label2.Text.LastIndexOf("x") + 1), 16).ToString("X2"));
                        fm.countC++;
                        fm.toolStripStatusLabel3.Text = "|  Commands: " + fm.countC + "　　";
                    }
                }
                else
                {
                    num++;
                }
                if (fm.textBox1.Text.Length != 0)
                {
                    if (fm.checkBox5.Checked == true)
                    {
                        SendCommand.action(Convert.ToUInt64(fm.textBox1.Text.Substring(fm.textBox1.Text.LastIndexOf("x") + 1), 16).ToString("X2"), fm);
                    }
                    else if (fm.checkBox5.Checked == false)
                    {
                        fm.serialPort1.Write(fm.textBox1.Text.Trim());
                        fm.countC++;
                        fm.toolStripStatusLabel3.Text = "|  Commands: " + fm.countC + "　　";
                    }
                }
                else
                {
                    num++;
                }
                if (num == 3)
                {
                    if (type == 1)
                    {
                        fm.timer3.Enabled = false;
                        fm.groupBox6.Enabled = true;
                        if (Form1.languageflag == (int)Form1.Language.English)
                        {
                            fm.button3.Text = "Send";
                        }
                        else if (Form1.languageflag == (int)Form1.Language.Chinese)
                        {
                            fm.button3.Text = "发送";
                        }
                    }
                    MessageBox.Show("No to-be-sent data!");
                }
                else
                {
                    if (type == 1)
                    {
                        if (Form1.languageflag == (int)Form1.Language.English)
                        {
                            fm.button3.Text = "Stop";
                        }
                        else if (Form1.languageflag == (int)Form1.Language.Chinese)
                        {
                            fm.button3.Text = "停止";
                        }
                        fm.groupBox6.Enabled = false;
                    }
                }
#endif
                #endregion
            }
            catch (Exception ex)
            {
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
