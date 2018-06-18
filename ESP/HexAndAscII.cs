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
    class HexAndAscII
    {
        //HEX到ASCII
        public static string toascii(string x)
        {
            //得到x的长度，奇数则除以二加一
            int xlength;
            if (x.Length % 2 == 1)
            {
                xlength = x.Length / 2 + 1;
            }
            else
            {
                xlength = x.Length / 2;
            }
            //定义int数组
            int[] itValue = new int[xlength];
            //定义string数组
            string[] stValue = new string[xlength];
            //定义返回参数
            string sValue = null;
            //计算过程
            for (int i = 0; i < xlength; i++)
            {
                if (x.Length % 2 == 1)
                {
                    //判断是否是最后一个字节
                    //将16进制数转化为10进制数
                    if (i == (xlength - 1))
                    {
                        itValue[i] = Convert.ToInt32(x.Substring(i * 2, 1), 16);
                    }
                    else
                    {
                        itValue[i] = Convert.ToInt32(x.Substring(i * 2, 2), 16);
                    }
                }
                else
                {
                    itValue[i] = Convert.ToInt32(x.Substring(i * 2, 2), 16);
                }
                //十进制数转化为ASCII码，存放在数组里
                stValue[i] = (System.Text.Encoding.ASCII.GetString(System.BitConverter.GetBytes(itValue[i]))).Substring(0, 1);  //Int-> ASCII 
                /*
                //最后一个空格的处理
                if (i == (xlength - 1))
                {
                    //从数组里取出值添加到字符串上
                    sValue += stValue[i];
                }
                else
                {
                    sValue += stValue[i] + " ";
                }
                */
                sValue += stValue[i];
            }
            return sValue;
        }

        //ASCII到HEX
        public static string tohex(string x)
        {
            byte[] ba = System.Text.ASCIIEncoding.Default.GetBytes(x);
            StringBuilder sValue = new StringBuilder();
            foreach (byte b in ba)
            {
                //sValue.Append(b.ToString("X2") + " ");
                sValue.Append(b.ToString("X2"));
            }
            /*
            //返回值与最后一个空格的处理
            return (sValue.ToString()).Substring(0,sValue.Length - 1);
            */
            return sValue.ToString();
        }
    }
}
