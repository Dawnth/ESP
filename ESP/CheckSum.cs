using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO.Ports;
using System.IO;

namespace ESP
{
    public partial class CheckSum : Form
    {
        // 新浪的订阅地址http://rss.sina.com.cn/news/world/focus15.xml

        #region[窗体的加载及关闭----------------------------------------------]

        public bool isshow = false;

        //string sValue;

        public CheckSum()
        {
            InitializeComponent();
        }

        private void CheckSum_Load(object sender, EventArgs e)
        {
            comboBox1.Text = comboBox1.Items[0].ToString();
            isshow = true;
        }

        private void CheckSum_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = FormWindowState.Minimized;
            this.Visible = false;
        }

        private void CheckSum_FormClosed(object sender, FormClosedEventArgs e)
        {
            isshow = false;
        }

        #endregion

        #region[校验和的计算--------------------------------------------------]

        //校验和计算的小工具，执行按钮
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                textBox20.Clear();
                int mycheck = 0;
                if (textBox19.Text != "")//当TextBox19中有数据的情况
                {
                    int length = textBox19.Text.Length / 2;//get lenngth
                    for (int i = 0; i < length; i++)
                    {
                        mycheck += Convert.ToInt32(textBox19.Text.Substring(i * 2, 2), 16);
                    }
                }
                else if (textBox19.Text == "" && textBox2.Text != "" && textBox4.Text != "")//当TextBox19中没有数据的情况
                {
                    if (textBox6.Text != "" && textBox8.Text == "" && textBox10.Text == "" && textBox12.Text == "")
                    {
                        mycheck += Convert.ToInt32(Convert.ToDouble(textBox2.Text) * Convert.ToDouble(textBox1.Text));
                        mycheck += Convert.ToInt32(Convert.ToDouble(textBox4.Text) * Convert.ToDouble(textBox3.Text));
                        mycheck += Convert.ToInt32(Convert.ToDouble(textBox6.Text) * Convert.ToDouble(textBox5.Text));
                    }
                    else if (textBox6.Text != "" && textBox8.Text != "" && textBox10.Text == "" && textBox12.Text == "")
                    {
                        mycheck += Convert.ToInt32(Convert.ToDouble(textBox2.Text) * Convert.ToDouble(textBox1.Text));
                        mycheck += Convert.ToInt32(Convert.ToDouble(textBox4.Text) * Convert.ToDouble(textBox3.Text));
                        mycheck += Convert.ToInt32(Convert.ToDouble(textBox6.Text) * Convert.ToDouble(textBox5.Text));
                        mycheck += Convert.ToInt32(Convert.ToDouble(textBox8.Text) * Convert.ToDouble(textBox7.Text));
                    }
                    else if (textBox6.Text != "" && textBox8.Text != "" && textBox10.Text != "" && textBox12.Text == "")
                    {
                        mycheck += Convert.ToInt32(Convert.ToDouble(textBox2.Text) * Convert.ToDouble(textBox1.Text));
                        mycheck += Convert.ToInt32(Convert.ToDouble(textBox4.Text) * Convert.ToDouble(textBox3.Text));
                        mycheck += Convert.ToInt32(Convert.ToDouble(textBox6.Text) * Convert.ToDouble(textBox5.Text));
                        mycheck += Convert.ToInt32(Convert.ToDouble(textBox8.Text) * Convert.ToDouble(textBox7.Text));
                        mycheck += Convert.ToInt32(Convert.ToDouble(textBox10.Text) * Convert.ToDouble(textBox9.Text));
                    }
                    else if (textBox6.Text != "" && textBox8.Text != "" && textBox10.Text != "" && textBox12.Text != "")
                    {
                        mycheck += Convert.ToInt32(Convert.ToDouble(textBox2.Text) * Convert.ToDouble(textBox1.Text));
                        mycheck += Convert.ToInt32(Convert.ToDouble(textBox4.Text) * Convert.ToDouble(textBox3.Text));
                        mycheck += Convert.ToInt32(Convert.ToDouble(textBox6.Text) * Convert.ToDouble(textBox5.Text));
                        mycheck += Convert.ToInt32(Convert.ToDouble(textBox8.Text) * Convert.ToDouble(textBox7.Text));
                        mycheck += Convert.ToInt32(Convert.ToDouble(textBox10.Text) * Convert.ToDouble(textBox9.Text));
                        mycheck += Convert.ToInt32(Convert.ToDouble(textBox12.Text) * Convert.ToDouble(textBox11.Text));
                    }
                    else
                    {
                        mycheck += Convert.ToInt32(Convert.ToDouble(textBox2.Text) * Convert.ToDouble(textBox1.Text));
                        mycheck += Convert.ToInt32(Convert.ToDouble(textBox4.Text) * Convert.ToDouble(textBox3.Text));
                    }
                }
                else if (textBox19.Text == "" && (textBox2.Text == "" || textBox4.Text != ""))//当TextBox19中没有数据的情况
                { }
                //将校验和通过选择方式计算不同的值赋给textBox20
                if (comboBox1.SelectedIndex == 0)//取反加一方式
                {
                    textBox20.Text = ((256 - mycheck % 256) % 256).ToString("X2");
                }
                else if (comboBox1.SelectedIndex == 1)//累加和方式
                {
                    textBox20.Text = (mycheck % 256).ToString("X2");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //HEX到ASCII
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                textBox14.Text = HexAndAscII.toascii(textBox13.Text); 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //ASCII到HEX
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                textBox13.Text = HexAndAscII.tohex(textBox14.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region[将ASCII还原成字符---------------------------------------------]
        /// <summary>
        /// 将ASCII还原成字符
        /// </summary>
        /// <param name="s">传入字符串</param>
        /// <returns></returns>
        public static string ASCIIToChar(string s)
        {
            string[] charAscii = s.Split(' ');
            string AsciiStr = "";
            if (s == "" || s == null)
            { }
            else
            {
                ASCIIEncoding myAs = new ASCIIEncoding();
                for (int i = 0; i < charAscii.Length; i++)
                {
                    int j = Int16.Parse(charAscii[i].ToString());
                    byte[] bte = new byte[] { (byte)j };
                    AsciiStr += myAs.GetString(bte);
                }
            }
            return AsciiStr;
        }
        #endregion

        #region[将字符转化成ASCII---------------------------------------------]
        /// <summary>
        /// 将字符转化成ASCII
        /// </summary>
        /// <param name="s">字符</param>
        /// <param name="fengefu">是否添加分隔符，true，false</param>
        /// <returns></returns>
        public static string CharToASCII(string s, bool fengefu)
        {
            ASCIIEncoding myAs = new ASCIIEncoding();
            byte[] myByte = myAs.GetBytes(s);
            string str = "";
            string mark = "";
            for (int i = 0; i < myByte.Length; i++)
            {
                if (fengefu == true)
                {
                    str = str + mark + Convert.ToInt16(myByte[i]).ToString();
                    mark = " ";
                }
                else
                {
                    str = str + Convert.ToInt16(myByte[i]).ToString();
                }
            }
            return str;
        }
        #endregion

        #region[将HEX码还原成字符---------------------------------------------]
        /// <summary>
        /// 将HEX码还原成字符
        /// </summary>
        /// <param name="hex">hex码</param>
        /// <param name="charset">编码,如"utf-8","gb2312"</param>
        /// <returns></returns>
        public static string HexToChar(string hex)//,string charset)
        {
            if (hex == null)
                throw new ArgumentNullException("hex");
            hex = hex.Replace(",", "");
            hex = hex.Replace("\n", "");
            hex = hex.Replace("\\", "");
            hex = hex.Replace(" ", "");
            string Str = "";
            if (hex.Length % 2 != 0)
            {
                hex += "20";//空格
            }
            // 需要将 hex 转换成 byte 数组。 
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                try
                {
                    // 每两个字符是一个 byte。 
                    bytes[i] = byte.Parse(hex.Substring(i * 2, 2),
                    System.Globalization.NumberStyles.HexNumber);
                    if (bytes[i] > 100)
                    {
                        Str += bytes[i].ToString("X");
                    }
                }
                catch
                {
                    throw new ArgumentException("hex is not a valid hex number!", "hex");
                }
            }
            System.Text.Encoding chs = System.Text.Encoding.UTF8;
            //.GetEncoding("");//charset);
            Str = Str + chs.GetString(bytes);
            return Str;
        }
        #endregion

        #region[将字符转化成HEX串---------------------------------------------]
        /// <summary>
        /// 将字符转化成HEX串
        /// </summary>
        /// <param name="s"></param>
        /// <param name="charset">编码,如"utf-8","gb2312"</param>
        /// <param name="fenge">是否每字符中间用空格分开，true，false</param>
        /// <returns></returns>
        public static string CharToHex(string s, bool fengefu)
        {
            System.Text.Encoding myHex = System.Text.Encoding.UTF8;
            byte[] bytes = myHex.GetBytes(s);
            string str = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                int mark = Int16.Parse(string.Format("{00:X}", bytes[i]));
                if (mark >= 10)
                {
                    str += string.Format("{0:X}", bytes[i]);

                    if (fengefu && (i != bytes.Length - 1))
                    {
                        str += string.Format("{0}", " ");
                    }
                }
                else
                {
                    str += string.Format("0{0:X}", bytes[i]);

                    if (fengefu && (i != bytes.Length - 1))
                    {
                        str += string.Format("{0}", " ");
                    }
                }
            }
            return str.ToLower();
        }
        #endregion

        #region[十进制负数转化为十六进制的补码--------------------------------]
        private void textBox15_Leave(object sender, EventArgs e)
        {
            try
            {
                //// 正常的转化方法，自己写的
                //long tempint = 0;
                //// 判断如果小于零则求补码
                //if (Convert.ToDouble(textBox15.Text) < 0)
                //{
                //    tempint = -Convert.ToInt32(textBox15.Text);
                //    tempint = ~tempint;
                //    tempint = tempint + 1;
                //    tempint = tempint | 0x80000000;
                //    textBox16.Text = tempint.ToString("X").Substring(8, 8);
                //}
                //else if (Convert.ToDouble(textBox15.Text) >= 0)
                //{
                //    textBox16.Text = Convert.ToInt32(textBox15.Text).ToString("X").PadLeft(8, '0');
                //}

                // 这是臭他妈不要脸的C#默认就是补码的方法
                // 以后都用这个都他妈成脑残了
                textBox16.Text = Convert.ToInt32(textBox15.Text).ToString("X").PadLeft(8,'0');
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

    }
}