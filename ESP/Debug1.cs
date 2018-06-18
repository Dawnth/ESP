using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ESP
{
    public partial class Debug1 : Form
    {
        #region ——窗体参数——
        public bool isshow = false;
        public Debug1()
        {
            InitializeComponent();
        }

        private void Debug1_Load(object sender, EventArgs e)
        {
            isshow = true;
        }

        private void Debug1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = FormWindowState.Minimized;
            this.Visible = false;
        }

        private void Debug1_FormClosed(object sender, FormClosedEventArgs e)
        {
            isshow = false;
        }
        #endregion

        #region ——CRC16——
        //ushort CRC16;
        //ushort a_u16CRC = 65535;
        ushort[] m_u16CRCLookupTable = new ushort[256]
        {
    0x0000, 0x1021, 0x2042, 0x3063, 0x4084, 0x50A5, 0x60C6, 0x70E7,
    0x8108, 0x9129, 0xA14A, 0xB16B, 0xC18C, 0xD1AD, 0xE1CE, 0xF1EF,
    0x1231, 0x0210, 0x3273, 0x2252, 0x52B5, 0x4294, 0x72F7, 0x62D6,
    0x9339, 0x8318, 0xB37B, 0xA35A, 0xD3BD, 0xC39C, 0xF3FF, 0xE3DE,
    0x2462, 0x3443, 0x0420, 0x1401, 0x64E6, 0x74C7, 0x44A4, 0x5485,
    0xA56A, 0xB54B, 0x8528, 0x9509, 0xE5EE, 0xF5CF, 0xC5AC, 0xD58D,
    0x3653, 0x2672, 0x1611, 0x0630, 0x76D7, 0x66F6, 0x5695, 0x46B4,
    0xB75B, 0xA77A, 0x9719, 0x8738, 0xF7DF, 0xE7FE, 0xD79D, 0xC7BC,
    0x48C4, 0x58E5, 0x6886, 0x78A7, 0x0840, 0x1861, 0x2802, 0x3823,
    0xC9CC, 0xD9ED, 0xE98E, 0xF9AF, 0x8948, 0x9969, 0xA90A, 0xB92B,
    0x5AF5, 0x4AD4, 0x7AB7, 0x6A96, 0x1A71, 0x0A50, 0x3A33, 0x2A12,
    0xDBFD, 0xCBDC, 0xFBBF, 0xEB9E, 0x9B79, 0x8B58, 0xBB3B, 0xAB1A,
    0x6CA6, 0x7C87, 0x4CE4, 0x5CC5, 0x2C22, 0x3C03, 0x0C60, 0x1C41,
    0xEDAE, 0xFD8F, 0xCDEC, 0xDDCD, 0xAD2A, 0xBD0B, 0x8D68, 0x9D49,
    0x7E97, 0x6EB6, 0x5ED5, 0x4EF4, 0x3E13, 0x2E32, 0x1E51, 0x0E70,
    0xFF9F, 0xEFBE, 0xDFDD, 0xCFFC, 0xBF1B, 0xAF3A, 0x9F59, 0x8F78,
    0x9188, 0x81A9, 0xB1CA, 0xA1EB, 0xD10C, 0xC12D, 0xF14E, 0xE16F,
    0x1080, 0x00A1, 0x30C2, 0x20E3, 0x5004, 0x4025, 0x7046, 0x6067,
    0x83B9, 0x9398, 0xA3FB, 0xB3DA, 0xC33D, 0xD31C, 0xE37F, 0xF35E,
    0x02B1, 0x1290, 0x22F3, 0x32D2, 0x4235, 0x5214, 0x6277, 0x7256,
    0xB5EA, 0xA5CB, 0x95A8, 0x8589, 0xF56E, 0xE54F, 0xD52C, 0xC50D,
    0x34E2, 0x24C3, 0x14A0, 0x0481, 0x7466, 0x6447, 0x5424, 0x4405,
    0xA7DB, 0xB7FA, 0x8799, 0x97B8, 0xE75F, 0xF77E, 0xC71D, 0xD73C,
    0x26D3, 0x36F2, 0x0691, 0x16B0, 0x6657, 0x7676, 0x4615, 0x5634,
    0xD94C, 0xC96D, 0xF90E, 0xE92F, 0x99C8, 0x89E9, 0xB98A, 0xA9AB,
    0x5844, 0x4865, 0x7806, 0x6827, 0x18C0, 0x08E1, 0x3882, 0x28A3,
    0xCB7D, 0xDB5C, 0xEB3F, 0xFB1E, 0x8BF9, 0x9BD8, 0xABBB, 0xBB9A,
    0x4A75, 0x5A54, 0x6A37, 0x7A16, 0x0AF1, 0x1AD0, 0x2AB3, 0x3A92,
    0xFD2E, 0xED0F, 0xDD6C, 0xCD4D, 0xBDAA, 0xAD8B, 0x9DE8, 0x8DC9,
    0x7C26, 0x6C07, 0x5C64, 0x4C45, 0x3CA2, 0x2C83, 0x1CE0, 0x0CC1,
    0xEF1F, 0xFF3E, 0xCF5D, 0xDF7C, 0xAF9B, 0xBFBA, 0x8FD9, 0x9FF8,
    0x6E17, 0x7E36, 0x4E55, 0x5E74, 0x2E93, 0x3EB2, 0x0ED1, 0x1EF0
        };

        ////测试CRC16
        //private void button2_Click(object sender, EventArgs e)
        //{
            //ushort i;
            //CRC16 = a_u16CRC;
            //ushort temp;
            //ushort temp2;
            //ushort[] pu8CurrentDataByte = new ushort[8] { Convert.ToUInt16(textBox1.Text, 16), Convert.ToUInt16("37", 16), 10, 4, 25, 1, 0, 0 };
            //for (i = 0; i < 8; i++)
            //{
            //    // Since the CRC is the remainder of modulo 2 division, we lookup the
            //    // pre-computed remainder value from the table.
            //    // CRC = Table[(CRC>>8) xor Data] xor (CRC<<8)
            //    temp = Convert.ToUInt16(((CRC16 >> 8) ^ pu8CurrentDataByte[i]) & 0xFF);
            //    temp2 = Convert.ToUInt16((Convert.ToString((CRC16 << 8), 2).PadLeft(24, '0').Substring(8, 16)), 2);
            //    CRC16 = Convert.ToUInt16(m_u16CRCLookupTable[temp] ^ (temp2));
            //}
            //textBox4.Text = CRC16.ToString("X4");
        //}
        #endregion

        #region ——CRC8——
        // CRC8校验表
        byte[] CRC8_Table = new byte[256]
{
   0x00, 0x9b, 0xad, 0x36, 0xc1, 0x5a, 0x6c, 0xf7,
   0x19, 0x82, 0xb4, 0x2f, 0xd8, 0x43, 0x75, 0xee,
   0x32, 0xa9, 0x9f, 0x04, 0xf3, 0x68, 0x5e, 0xc5,
   0x2b, 0xb0, 0x86, 0x1d, 0xea, 0x71, 0x47, 0xdc,
   0x64, 0xff, 0xc9, 0x52, 0xa5, 0x3e, 0x08, 0x93,
   0x7d, 0xe6, 0xd0, 0x4b, 0xbc, 0x27, 0x11, 0x8a,
   0x56, 0xcd, 0xfb, 0x60, 0x97, 0x0c, 0x3a, 0xa1,
   0x4f, 0xd4, 0xe2, 0x79, 0x8e, 0x15, 0x23, 0xb8,
   0xc8, 0x53, 0x65, 0xfe, 0x09, 0x92, 0xa4, 0x3f,
   0xd1, 0x4a, 0x7c, 0xe7, 0x10, 0x8b, 0xbd, 0x26,
   0xfa, 0x61, 0x57, 0xcc, 0x3b, 0xa0, 0x96, 0x0d,
   0xe3, 0x78, 0x4e, 0xd5, 0x22, 0xb9, 0x8f, 0x14,
   0xac, 0x37, 0x01, 0x9a, 0x6d, 0xf6, 0xc0, 0x5b,
   0xb5, 0x2e, 0x18, 0x83, 0x74, 0xef, 0xd9, 0x42,
   0x9e, 0x05, 0x33, 0xa8, 0x5f, 0xc4, 0xf2, 0x69,
   0x87, 0x1c, 0x2a, 0xb1, 0x46, 0xdd, 0xeb, 0x70,
   0x0b, 0x90, 0xa6, 0x3d, 0xca, 0x51, 0x67, 0xfc,
   0x12, 0x89, 0xbf, 0x24, 0xd3, 0x48, 0x7e, 0xe5,
   0x39, 0xa2, 0x94, 0x0f, 0xf8, 0x63, 0x55, 0xce,
   0x20, 0xbb, 0x8d, 0x16, 0xe1, 0x7a, 0x4c, 0xd7,
   0x6f, 0xf4, 0xc2, 0x59, 0xae, 0x35, 0x03, 0x98,
   0x76, 0xed, 0xdb, 0x40, 0xb7, 0x2c, 0x1a, 0x81,
   0x5d, 0xc6, 0xf0, 0x6b, 0x9c, 0x07, 0x31, 0xaa,
   0x44, 0xdf, 0xe9, 0x72, 0x85, 0x1e, 0x28, 0xb3,
   0xc3, 0x58, 0x6e, 0xf5, 0x02, 0x99, 0xaf, 0x34,
   0xda, 0x41, 0x77, 0xec, 0x1b, 0x80, 0xb6, 0x2d,
   0xf1, 0x6a, 0x5c, 0xc7, 0x30, 0xab, 0x9d, 0x06,
   0xe8, 0x73, 0x45, 0xde, 0x29, 0xb2, 0x84, 0x1f,
   0xa7, 0x3c, 0x0a, 0x91, 0x66, 0xfd, 0xcb, 0x50,
   0xbe, 0x25, 0x13, 0x88, 0x7f, 0xe4, 0xd2, 0x49,
   0x95, 0x0e, 0x38, 0xa3, 0x54, 0xcf, 0xf9, 0x62,
   0x8c, 0x17, 0x21, 0xba, 0x4d, 0xd6, 0xe0, 0x7b,
};
        //private void button5_Click(object sender, EventArgs e)
        //{
        //    int length;
        //    byte CRC8 = 0;
        //    length = textBox27.Text.Length / 2;
        //    for (int i = 0; i < length; i++)
        //    {
        //        CRC8 = CRC8_Process(Convert.ToByte(Convert.ToInt16(textBox27.Text.Substring(i * 2, 2), 16)), CRC8);
        //    }
        //    textBox28.Text = CRC8.ToString("X2");
        //    textBox29.Text = textBox27.Text + textBox28.Text;
        //}

        //private byte CRC8_Process(byte Data, byte OldCRC8)
        //{
        //    return (CRC8_Table[Data ^ OldCRC8]);
        //}
        #endregion

        #region ——数组长度——
        // 测试数组最后一个元素加逗号和不加的区别
        // 编译都能通过
        // 通过MSDN得出可以加可以不加，影响不大
        // 貌似加了后会对程序的一致性有帮助
        // 体现在增加数组元素时不会导致遗忘性错误
        //int[] s = new int[1] { 1 };

        //private void button6_Click(object sender, EventArgs e)
        //{
        //    textBox30.Text = s[0].ToString();
        //}
        #endregion

        #region ——计算素数——
        // 基础算法，发现MaxNum定义大后会导致速度巨慢
        // 看到网上说想优化算法的一种方法就是查到素数后
        // 删除它所有的倍数
        // 听起来很不错，不过实际操作我还没有想过，
        // 由于是临时温习大学写的代码，且时间不错，就不深究了
        // 待有时间后继续优化算法
        // 贴一个网上看到的程序，本人并未验证
        //#include<time.h> 
        //main() 
        //{ 
        //long int a,b=2,c,d,i; 
        //int begin,end; 
        //time(&begin);          /*取一个系统时间*/ 
        //printf("\n\n\n%u ",b); /*素数2例外，我把他单独输出*/ 
        //for(i=3;i<100000;i++)  /*从3开始一直到100000进行筛选*/ 
        //{ 
        //a=0; 
        //d=(i+1)/3;             /*因为都是单数，所以不能是2的倍数，尽量使计算的范围缩小*/ 
        //for(c=1;c<=d;c++)      /*判断c是否是i的因数↓*/ 
        //{ 
        //if((i%c)==0)           /*如果c是i的因数，a的值增加1*/ 
        //a=a+1; 
        //if(a>=2) 
        //c=d+1;                 /*如果i存在2个以上的因数，结束i的判断*/ 
        //}                      /*判断c是否是i的因数↑*/ 
        //if(a==1)     /*如果i只有一个因数1，则i是素数，因为计算不到i，所以只会有一个因数*/ 
        //printf("%ld ",i++);    /*输出这个素数*/ 
        //} 
        //time(&end);            /*再取一个系统时间*/ 
        //printf("\n%d",end-begin);     /*输出程序运行所消耗的时间*/ 
        //getch();                      /*等待按键*/ 
        //} 
        //// 素数计算，输出从MinNum到MaxNum的全部素数
        //int MinNum = 1;
        //int MaxNum = 1000;
        //int counterPrimeNumber = 1;
        //// 验证任意数据是否为素数
        //int TestNum = 983;

        //private void PrimeNumber()
        //{
        //    // 当TestNum值不为零时，执行测试程序，否则输出规定范围内的素数
        //    if (TestNum != 0)
        //    {
        //        for (int i = 2; i < TestNum; i++)
        //        {
        //            if (TestNum % i == 0)
        //            {
        //                textBox5.Text += (counterPrimeNumber++).ToString().PadLeft(4, '0') + "    " + i.ToString() + "\r\n";
        //            }
        //        }
        //        if (textBox5.Text == "")
        //        {
        //            textBox5.Text = "测试数据为素数";
        //        }
        //    }
        //    else
        //    {
        //        for (int i = MinNum; i <= MaxNum; i++)
        //        {
        //            for (int j = 2; j < i; j++)
        //            {
        //                if (i % j == 0)
        //                { break; }
        //                else if (j == (i - 1))
        //                {
        //                    textBox5.Text += (counterPrimeNumber++).ToString().PadLeft(4, '0') + "    " + i.ToString() + "\r\n";
        //                }
        //            }
        //            if (i == 2)
        //            {
        //                textBox5.Text += (counterPrimeNumber++).ToString().PadLeft(4, '0') + "    " + i.ToString() + "\r\n";
        //            }
        //        }
        //    }
        //}

        //private void button7_Click(object sender, EventArgs e)
        //{
        //    textBox5.Clear();
        //    PrimeNumber();
        //}
        #endregion

        #region ——计算农历——
        //// 输出农历
        //private void button8_Click(object sender, EventArgs e)
        //{
        //    //textBox34.Text;
        //    LunarCalendar.SetGregorian(Convert.ToInt32(textBox31.Text), Convert.ToInt32(textBox32.Text), Convert.ToInt32(textBox33.Text));
        //    LunarCalendar.ComputeChineseFields();
        //    LunarCalendar.ComputeSolarTerms();
        //    LunarCalendar.mylunar(textBox34);
        //}
        #endregion

        #region ——数学——
        //// 数学答案
        //int counter;
        //int good;
        //private void button1_Click(object sender, EventArgs e)
        //{
        //    textBox5.Text = "";
        //    counter = 1;
        //    good = 1000;
        //    for (; good <= 9999; good++)
        //    {
        //        if (Convert.ToInt32(good.ToString().Substring(0, 2)) + Convert.ToInt32(good.ToString().Substring(2, 2)) == Convert.ToInt32(good.ToString().Substring(1, 2)))
        //        {
        //            textBox5.AppendText((counter++).ToString().PadLeft(2, '0') + "  " + good.ToString() + "\r\n");
        //        }
        //    }
        //}
        #endregion

        #region ——矿入——
        ////计算矿入
        // Persistence is the common trait of anyone 
        // who has had a significant impact on the world.
        //private void button3_Click(object sender, EventArgs e)
        //{
        //    button3Click();
        //}
        //private void button3Click()
        //{
        //    try
        //    {
        //        if (
        //               textBox6.Text != ""
        //            && textBox7.Text != ""
        //            && textBox8.Text != ""
        //            && textBox9.Text != ""
        //            && textBox10.Text != ""
        //            && textBox11.Text != ""
        //            && textBox12.Text != ""
        //            && textBox13.Text != ""
        //            && textBox14.Text != ""
        //            && textBox15.Text != ""
        //            && textBox16.Text != ""
        //            && textBox17.Text != ""
        //           )
        //        {
        //            textBox24.Text = (Convert.ToInt32(textBox6.Text) * Convert.ToInt32(textBox7.Text)).ToString();
        //            textBox23.Text = (Convert.ToInt32(textBox8.Text) * Convert.ToInt32(textBox9.Text)).ToString();
        //            textBox22.Text = (Convert.ToInt32(textBox10.Text) * Convert.ToInt32(textBox11.Text)).ToString();
        //            textBox21.Text = (Convert.ToInt32(textBox12.Text) * Convert.ToInt32(textBox13.Text)).ToString();
        //            textBox20.Text = (Convert.ToInt32(textBox14.Text) * Convert.ToInt32(textBox15.Text)).ToString();
        //            textBox19.Text = (Convert.ToInt32(textBox16.Text) * Convert.ToInt32(textBox17.Text)).ToString();
        //            textBox18.Text = (Convert.ToInt32(textBox24.Text) + Convert.ToInt32(textBox23.Text) + Convert.ToInt32(textBox22.Text) + Convert.ToInt32(textBox21.Text) + Convert.ToInt32(textBox20.Text) + Convert.ToInt32(textBox19.Text)).ToString();
        //        }
        //    }
        //    catch// (Exception ex)
        //    {

        //    }
        //}

        //private void button4_Click(object sender, EventArgs e)
        //{
        //    button3Click();
        //    textBox26.Text = (Convert.ToInt32(textBox18.Text) * Convert.ToInt32(textBox25.Text)).ToString();
        //}
        #endregion

    }
}