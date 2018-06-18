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
    public partial class Debug2 : Form
    {
        #region 参数

        //用于保存Form打开的状态
        public bool isshow = false;
        //checksum是否与位置
        string mychecksum = null;
        //是否是十六进制发送
        string ishex = null;//是否是十六进制发送
        //保存起始位和结束位
        string mystart = null;
        string mystop = null;
        //左转参数
        string commandleft = null;//左转指令
        string mycommandleft = null;//左转指令
        string lindex = null;//左转的速度位置引索
        string laindex = null;//左转的加速度位置引索
        string lcoefficient = null;//左转的速度系数
        string lacoefficient = null;//左转的加速度系数
        //右转参数
        string commandright = null;
        string mycommandright = null;
        string rindex = null;
        string raindex = null;
        string rcoefficient = null;
        string racoefficient = null;
        //停止参数
        string commandstop = null;
        string mycommandstop = null;
        string sindex = null;
        string saindex = null;
        string scoefficient = null;
        string sacoefficient = null;
        //脱水参数
        string spinlow = null;//1档
        string spinmid = null;//2档
        string spinhigh = null;//3档
        string spinbeyond = null;//4档
        string myspinlow = null;//1档
        string myspinmid = null;//2档
        string myspinhigh = null;//3档
        string myspinbeyond = null;//4档
        string spindex = null;//速度位置索引
        string spaindex = null;//加速度位置索引
        string spincoefficient = null;//速度系数
        string spinacoefficient = null;//加速度系数

        //最大长度，用来控制发送指令的间隔时间
        int maxlength = 0;

        //计数
        ulong count1 = 0;//timer1的计数
        int count2 = 0;//timer2的计数
        int count3 = 0;//timer3的计数
        int count4 = 0;//timer的计数，当为1时使能timer2，当为2时使能timer3，并且清零
        int count5 = 0;//timer5的计数
        int count6 = 0;//timer6的计数
        int count7 = 0;//timer7的计数
        int count8 = 0;//timer8的计数
        ulong count9 = 0;//timer9的计数
        int count10 = 0;//timer10的计数
        int countstop = 0;//timer4的计数，用来使能停止的计数
        //int countstart = 0;//用于开始按钮按下的计数，在很多地方被清零，貌似目前存在一点点问题哈，我在debug一下
        int countsp = 0;//用于串口的打开的计数，在值为1时加载串口配置，在关闭配置文档时被清空
        int ton = 0;//置顶的计数
        ulong summer = 0;//用于总时间的计数
        ulong myintermission = 10;//中间间隔时间的值，包括洗涤和脱水中间，两次流程之间，可能不够严谨，目前没有更改的打算
        ulong abc = 0;//保存intermission的值，用于计算，并且更改，这么写的目的很简单，就是防止将intermission的值改变
        int washcount = 1;//用于记录洗涤周期的次数，感觉有误差，在1左右
        int spincount = 0;//在洗涤加脱水的整流程中进行计数，即当洗涤结束后并且在intermission时间过后，会自加，当值为1时，触发脱水的timer，在很多流程结束后会被清零

        //挂起
        int countsave = 0;//挂起操作的计数
        string save_name = null;//保存当前运行的timer的名字用于恢复时使用
        bool save_timer = true;//用于保存当前timer1的状态，感觉有点多余，不过，好像删了还少点啥，所以还没有删除

        //配置文档的参数
        XmlDocument ecovixml = new XmlDocument();
        string filename = null;//用于从adress中提取文件的名称
        string adress = null;//用于从ECOVIConfig.cds中提取路径，并加载此路径

        //CRC相关参数
        ushort[] CRC16LookupTable = new ushort[256]
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

        #endregion

        #region 窗体相关的定义及方法

        public Debug2()
        {
            InitializeComponent();
            //加载语言选项
            Debug2_Language_Selected(Form1.languageflag);
        }

        //窗体的load事件
        private void Debug2_Load(object sender, EventArgs e)
        {
            isshow = true;
            this.MaximizeBox = false;
            groupBox2.Enabled = false;
            toolStripButton1.Enabled = false;
            toolStripMenuItem12.Enabled = false;
            toolStripMenuItem13.Enabled = false;
            textBoxNumEx29.Enabled = false;
            xmlload();
        }

        /* 串口断开
        //重写窗体事件，实现当检测到串口异常时也能反馈到系统，提示关闭
        protected override void WndProc(ref Message m)
        {
            
            if (m.Msg == 0x0219)
            {
                //串口被拔出 
                if (m.WParam.ToInt32() == 0x8004)
                {
                    timer8.Enabled = false;
                    timer7.Enabled = false;
                    timer6.Enabled = false;
                    timer5.Enabled = false;
                    timer4.Enabled = false;
                    timer3.Enabled = false;
                    timer2.Enabled = false;
                    timer1.Enabled = false;
                    count1 = 0;
                    count2 = 0;
                    count3 = 0;
                    count4 = 0;
                    count5 = 0;
                    count6 = 0;
                    count7 = 0;
                    count8 = 0;
                    //countstart = 0;
                    washcount = 0;
                    spincount = 0;
                    toolStripMenuItem11.Enabled = true;
                    toolStripMenuItem12.Enabled = true;
                    toolStripMenuItem13.Enabled = false;
                    toolStripButton1.Text = "开始运行";
                    textBoxNowStatus.Text = "已经停止运行！";
                    serialPort1.Close();

                    MessageBox.Show("串口断开！系统将关闭！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    System.Threading.Thread.CurrentThread.Abort();
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
            }
            base.WndProc(ref m);   
        }
        */

        //在窗体LOAD时为控件加载参数
        private void xmlload()
        {
            try
            {
                //如果有配置 则读取配置文件
                if (File.Exists(Application.StartupPath + @"\ESPConfig.cds"))
                {
                    XmlDocument cdsxml = new XmlDocument();
                    cdsxml.Load(Application.StartupPath + @"\ESPConfig.cds");
                    if (cdsxml.SelectSingleNode("Config/Lastdebug2").InnerText != "")
                    {
                        adress = cdsxml.SelectSingleNode("Config/Lastdebug2").InnerText;
                        readecvoi(adress);
                    }
                    else 
                    {
                        //如果没有配置文档，打开配置文档对话框
                        //openFileDialog1.ShowDialog();
                    }
                }
                else
                { }
            }
            catch
            {
                MessageBox.Show("文档结构无效！");
                openFileDialog1.ShowDialog();
            }
        }

        //关闭时发生的事件
        private void Debug2_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = FormWindowState.Minimized;
            this.Visible = false;
        }

        //关闭后发生的事件
        private void Debug2_FormClosed(object sender, FormClosedEventArgs e)
        {
            isshow = false;
        }

        #endregion

        #region 打开读取配置文档

        //打开配置文档
        private void toolStripMenuItem11_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Application.StartupPath;
            openFileDialog1.ShowDialog();
        }

        //FileOK的事件
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            //将打开的路径保存到ESPConfig.cds中
            XmlDocument cdsxml = new XmlDocument();
            cdsxml.Load(Application.StartupPath + @"\ESPConfig.cds");
            cdsxml.SelectSingleNode("Config/Lastdebug2").InnerText = openFileDialog1.FileName;
            cdsxml.Save(Application.StartupPath + @"\ESPConfig.cds");

            //读取打开文件
            readecvoi(openFileDialog1.FileName);
        }

        //读取打开文件的内容入参为路径
        private void readecvoi(string path)
        {
            try
            {
                groupBox2.Enabled = true;
                ecovixml.Load(path);
                adress = path;
                filename = adress.Substring(adress.LastIndexOf("\\") + 1);
                this.Text = "Washing machine[Debug]" + " - " + filename;

                ///
                /// 变量读取参数
                ///
                //十六进制发送
                ishex = ecovixml.SelectSingleNode("Ecovi/Debug").Attributes["HEX"].InnerText;
                //读取start、stop和checksum的值
                mychecksum = ecovixml.SelectSingleNode("Ecovi/Debug").Attributes["Checksum"].InnerText;
                mystart = ecovixml.SelectSingleNode("Ecovi/Debug").Attributes["Start"].InnerText;
                mystop = ecovixml.SelectSingleNode("Ecovi/Debug").Attributes["Stop"].InnerText;
                //读取是否循环流程的参数
                myintermission = Convert.ToUInt64(ecovixml.SelectSingleNode("Ecovi/Debug").Attributes["Intermission"].InnerText);
                //左转参数
                commandleft = ecovixml.SelectSingleNode("Ecovi/Debug/left").Attributes["Command"].InnerText;
                lindex = ecovixml.SelectSingleNode("Ecovi/Debug/left").Attributes["Sindex"].InnerText;
                laindex = ecovixml.SelectSingleNode("Ecovi/Debug/left").Attributes["Aindex"].InnerText;
                lcoefficient = ecovixml.SelectSingleNode("Ecovi/Debug/left").Attributes["Scoefficient"].InnerText;
                lacoefficient = ecovixml.SelectSingleNode("Ecovi/Debug/left").Attributes["Acoefficient"].InnerText;
                //右转参数
                commandright = ecovixml.SelectSingleNode("Ecovi/Debug/right").Attributes["Command"].InnerText;
                rindex = ecovixml.SelectSingleNode("Ecovi/Debug/right").Attributes["Sindex"].InnerText;
                raindex = ecovixml.SelectSingleNode("Ecovi/Debug/right").Attributes["Aindex"].InnerText;
                rcoefficient = ecovixml.SelectSingleNode("Ecovi/Debug/right").Attributes["Scoefficient"].InnerText;
                racoefficient = ecovixml.SelectSingleNode("Ecovi/Debug/right").Attributes["Acoefficient"].InnerText;
                //停止参数
                commandstop = ecovixml.SelectSingleNode("Ecovi/Debug/stop").Attributes["Command"].InnerText;
                sindex = ecovixml.SelectSingleNode("Ecovi/Debug/stop").Attributes["Sindex"].InnerText;
                saindex = ecovixml.SelectSingleNode("Ecovi/Debug/stop").Attributes["Aindex"].InnerText;
                scoefficient = ecovixml.SelectSingleNode("Ecovi/Debug/stop").Attributes["Scoefficient"].InnerText;
                sacoefficient = ecovixml.SelectSingleNode("Ecovi/Debug/stop").Attributes["Acoefficient"].InnerText;
                //脱水参数
                spinlow = ecovixml.SelectSingleNode("Ecovi/Debug/SPIN_1").Attributes["Command"].InnerText;
                spindex = ecovixml.SelectSingleNode("Ecovi/Debug/SPIN_1").Attributes["Sindex"].InnerText;
                spaindex = ecovixml.SelectSingleNode("Ecovi/Debug/SPIN_1").Attributes["Aindex"].InnerText;
                spincoefficient = ecovixml.SelectSingleNode("Ecovi/Debug/SPIN_1").Attributes["Scoefficient"].InnerText;
                spinacoefficient = ecovixml.SelectSingleNode("Ecovi/Debug/SPIN_1").Attributes["Acoefficient"].InnerText;
                spinmid = ecovixml.SelectSingleNode("Ecovi/Debug/SPIN_2").Attributes["Command"].InnerText;
                spinhigh = ecovixml.SelectSingleNode("Ecovi/Debug/SPIN_3").Attributes["Command"].InnerText;
                spinbeyond = ecovixml.SelectSingleNode("Ecovi/Debug/SPIN_4").Attributes["Command"].InnerText;

                ///
                /// 控件读取参数
                /////十六进制发送
                checkBox4.Checked = Convert.ToBoolean(ecovixml.SelectSingleNode("Ecovi/Debug").Attributes["HEX"].InnerText);
                //循环复选框和间隔时间
                checkBox3.Checked = Convert.ToBoolean(ecovixml.SelectSingleNode("Ecovi/Debug").Attributes["Repeat"].InnerText);
                textBoxNumEx29.Text = ecovixml.SelectSingleNode("Ecovi/Debug").Attributes["Intermission"].InnerText;
                //读取洗涤的参数
                checkBox1.Checked = Convert.ToBoolean(ecovixml.SelectSingleNode("Ecovi/Wash").Attributes["Enable"].InnerText);
                textBoxNumEx1.Text = ecovixml.SelectSingleNode("Ecovi/Wash").Attributes["Time"].InnerText;
                //读取洗涤左转的参数
                textBoxNumEx2.Text = ecovixml.SelectSingleNode("Ecovi/Wash/Item_1").Attributes["Time"].InnerText;
                textBoxNumEx3.Text = ecovixml.SelectSingleNode("Ecovi/Wash/Item_1").Attributes["Speed"].InnerText;
                textBoxNumEx19.Text = ecovixml.SelectSingleNode("Ecovi/Wash/Item_1").Attributes["Acceleration"].InnerText;
                //读取洗涤右转的参数
                textBoxNumEx4.Text = ecovixml.SelectSingleNode("Ecovi/Wash/Item_2").Attributes["Time"].InnerText;
                textBoxNumEx5.Text = ecovixml.SelectSingleNode("Ecovi/Wash/Item_2").Attributes["Speed"].InnerText;
                textBoxNumEx20.Text = ecovixml.SelectSingleNode("Ecovi/Wash/Item_2").Attributes["Acceleration"].InnerText;
                //读取洗涤的停止参数
                textBoxNumEx6.Text = ecovixml.SelectSingleNode("Ecovi/Wash/Item_3").Attributes["Time"].InnerText;
                textBoxNumEx25.Text = ecovixml.SelectSingleNode("Ecovi/Wash/Item_3").Attributes["Speed"].InnerText;
                textBoxNumEx26.Text = ecovixml.SelectSingleNode("Ecovi/Wash/Item_3").Attributes["Acceleration"].InnerText;
                //读取脱水的参数
                checkBox2.Checked = Convert.ToBoolean(ecovixml.SelectSingleNode("Ecovi/SPIN").Attributes["Enable"].InnerText);
                textBoxNumEx9.Text = ecovixml.SelectSingleNode("Ecovi/SPIN").Attributes["Time"].InnerText;
                //读取脱水的low参数
                textBoxNumEx10.Text = ecovixml.SelectSingleNode("Ecovi/SPIN/Item_1").Attributes["Time"].InnerText;
                textBoxNumEx11.Text = ecovixml.SelectSingleNode("Ecovi/SPIN/Item_1").Attributes["Speed"].InnerText;
                textBoxNumEx21.Text = ecovixml.SelectSingleNode("Ecovi/SPIN/Item_1").Attributes["Acceleration"].InnerText;
                //读取脱水的mid参数
                textBoxNumEx12.Text = ecovixml.SelectSingleNode("Ecovi/SPIN/Item_2").Attributes["Time"].InnerText;
                textBoxNumEx13.Text = ecovixml.SelectSingleNode("Ecovi/SPIN/Item_2").Attributes["Speed"].InnerText;
                textBoxNumEx22.Text = ecovixml.SelectSingleNode("Ecovi/SPIN/Item_2").Attributes["Acceleration"].InnerText;
                //读取脱水的high参数
                textBoxNumEx14.Text = ecovixml.SelectSingleNode("Ecovi/SPIN/Item_3").Attributes["Time"].InnerText;
                textBoxNumEx15.Text = ecovixml.SelectSingleNode("Ecovi/SPIN/Item_3").Attributes["Speed"].InnerText;
                textBoxNumEx23.Text = ecovixml.SelectSingleNode("Ecovi/SPIN/Item_3").Attributes["Acceleration"].InnerText;
                //读取脱水的beyond参数
                textBoxNumEx16.Text = ecovixml.SelectSingleNode("Ecovi/SPIN/Item_4").Attributes["Time"].InnerText;
                textBoxNumEx17.Text = ecovixml.SelectSingleNode("Ecovi/SPIN/Item_4").Attributes["Speed"].InnerText;
                textBoxNumEx24.Text = ecovixml.SelectSingleNode("Ecovi/SPIN/Item_4").Attributes["Acceleration"].InnerText;
            }
            catch
            {
                MessageBox.Show("配置文档出错，请重新生成配置文档！");
            }
        }

        //关闭配置文档
        private void toolStripMenuItem12_Click(object sender, EventArgs e)
        {
            groupBox2.Enabled = false;
            toolStripButton1.Enabled = false;
            countsp = 0;
            this.Text = "Washing machine[Debug]";
            XmlDocument deletefile = new XmlDocument();
            deletefile.Load(Application.StartupPath + @"\ESPConfig.cds");
            deletefile.SelectSingleNode("Config/Lastdebug2").InnerText = "";
            deletefile.Save(Application.StartupPath + @"\ESPConfig.cds");
        }

        //窗体标题文字更改的事件
        private void Debug2_TextChanged(object sender, EventArgs e)
        {
            if (this.Text == "Washing machine[Debug]")
            { toolStripMenuItem12.Enabled = false; }
            else
            { toolStripMenuItem12.Enabled = true; }
        }

        #endregion

        #region 生成按钮、修改按钮

        //生成并保存按钮的单击事件
        private void button1_Click(object sender, EventArgs e)
        {
            countsp++;
            toolStripLabel1.Visible = false;
            TimerTextBox.Visible = false;
            if (countsp == 1)
            {
                serialPort1.PortName = Form1.spconfig[0];
                serialPort1.BaudRate = Convert.ToInt32(Form1.spconfig[1]);
                if (Form1.spconfig[2] == "None")
                { serialPort1.Parity = Parity.None; }
                else if (Form1.spconfig[2] == "Odd")
                { serialPort1.Parity = Parity.Odd; }
                else if (Form1.spconfig[2] == "Even")
                { serialPort1.Parity = Parity.Even; }
                else if (Form1.spconfig[2] == "Mark")
                { serialPort1.Parity = Parity.Mark; }
                else if (Form1.spconfig[2] == "Space")
                { serialPort1.Parity = Parity.Space; }
                serialPort1.DataBits = Convert.ToInt32(Form1.spconfig[3]);
                if (Form1.spconfig[4] == "None")
                { serialPort1.StopBits = StopBits.None; }
                else if (Form1.spconfig[4] == "One")
                { serialPort1.StopBits = StopBits.One; }
                else if (Form1.spconfig[4] == "Two")
                { serialPort1.StopBits = StopBits.Two; }
                else if (Form1.spconfig[4] == "OnePointFive")
                { serialPort1.StopBits = StopBits.OnePointFive; }
                serialPort1.ReadTimeout = Convert.ToInt32(Form1.spconfig[5]);
                serialPort1.WriteTimeout = Convert.ToInt32(Form1.spconfig[6]);
                serialPort1.ReadBufferSize = Convert.ToInt32(Form1.spconfig[7]);
                serialPort1.WriteBufferSize = Convert.ToInt32(Form1.spconfig[8]);
            }
            //将变更保存到配置文档
            ecovixml.Load(Application.StartupPath + "\\" + @filename);
            ecovixml.SelectSingleNode("Ecovi/Debug").Attributes["Intermission"].InnerText = textBoxNumEx29.Text;
            ecovixml.SelectSingleNode("Ecovi/Debug").Attributes["Repeat"].InnerText = Convert.ToString(checkBox3.Checked);
            //保存洗涤的参数
            ecovixml.SelectSingleNode("Ecovi/Wash").Attributes["Enable"].InnerText = Convert.ToString(checkBox1.Checked);
            ecovixml.SelectSingleNode("Ecovi/Wash").Attributes["Time"].InnerText = textBoxNumEx1.Text;
            //保存洗涤左转的参数
            ecovixml.SelectSingleNode("Ecovi/Wash/Item_1").Attributes["Time"].InnerText = textBoxNumEx2.Text;
            ecovixml.SelectSingleNode("Ecovi/Wash/Item_1").Attributes["Speed"].InnerText = textBoxNumEx3.Text;
            ecovixml.SelectSingleNode("Ecovi/Wash/Item_1").Attributes["Acceleration"].InnerText = textBoxNumEx19.Text;
            //保存洗涤右转的参数
            ecovixml.SelectSingleNode("Ecovi/Wash/Item_2").Attributes["Time"].InnerText = textBoxNumEx4.Text;
            ecovixml.SelectSingleNode("Ecovi/Wash/Item_2").Attributes["Speed"].InnerText = textBoxNumEx5.Text;
            ecovixml.SelectSingleNode("Ecovi/Wash/Item_2").Attributes["Acceleration"].InnerText = textBoxNumEx20.Text;
            //保存洗涤的停止参数
            ecovixml.SelectSingleNode("Ecovi/Wash/Item_3").Attributes["Time"].InnerText = textBoxNumEx6.Text;
            ecovixml.SelectSingleNode("Ecovi/Wash/Item_3").Attributes["Speed"].InnerText = textBoxNumEx25.Text;
            ecovixml.SelectSingleNode("Ecovi/Wash/Item_3").Attributes["Acceleration"].InnerText = textBoxNumEx26.Text;
            //保存脱水的参数
            ecovixml.SelectSingleNode("Ecovi/SPIN").Attributes["Enable"].InnerText = Convert.ToString(checkBox2.Checked);
            ecovixml.SelectSingleNode("Ecovi/SPIN").Attributes["Time"].InnerText = textBoxNumEx9.Text;
            //保存脱水的low参数
            ecovixml.SelectSingleNode("Ecovi/SPIN/Item_1").Attributes["Time"].InnerText = textBoxNumEx10.Text;
            ecovixml.SelectSingleNode("Ecovi/SPIN/Item_1").Attributes["Speed"].InnerText = textBoxNumEx11.Text;
            ecovixml.SelectSingleNode("Ecovi/SPIN/Item_1").Attributes["Acceleration"].InnerText = textBoxNumEx21.Text;
            //保存脱水的mid参数
            ecovixml.SelectSingleNode("Ecovi/SPIN/Item_2").Attributes["Time"].InnerText = textBoxNumEx12.Text;
            ecovixml.SelectSingleNode("Ecovi/SPIN/Item_2").Attributes["Speed"].InnerText = textBoxNumEx13.Text;
            ecovixml.SelectSingleNode("Ecovi/SPIN/Item_2").Attributes["Acceleration"].InnerText = textBoxNumEx22.Text;
            //保存脱水的high参数
            ecovixml.SelectSingleNode("Ecovi/SPIN/Item_3").Attributes["Time"].InnerText = textBoxNumEx14.Text;
            ecovixml.SelectSingleNode("Ecovi/SPIN/Item_3").Attributes["Speed"].InnerText = textBoxNumEx15.Text;
            ecovixml.SelectSingleNode("Ecovi/SPIN/Item_3").Attributes["Acceleration"].InnerText = textBoxNumEx23.Text;
            //保存脱水的beyond参数
            ecovixml.SelectSingleNode("Ecovi/SPIN/Item_4").Attributes["Time"].InnerText = textBoxNumEx16.Text;
            ecovixml.SelectSingleNode("Ecovi/SPIN/Item_4").Attributes["Speed"].InnerText = textBoxNumEx17.Text;
            ecovixml.SelectSingleNode("Ecovi/SPIN/Item_4").Attributes["Acceleration"].InnerText = textBoxNumEx24.Text;
            //保存变更
            ecovixml.Save(Application.StartupPath + "\\" + @filename);

            //不同选择的处理
            if (checkBox1.Checked == true && checkBox2.Checked == false)//洗涤
            {
                if (   textBoxNumEx1.Text.Trim() != "" 
                    && textBoxNumEx2.Text.Trim() != "" 
                    && textBoxNumEx3.Text.Trim() != "" 
                    && textBoxNumEx4.Text.Trim() != "" 
                    && textBoxNumEx5.Text.Trim() != "" 
                    && textBoxNumEx6.Text.Trim() != "" 
                    && textBoxNumEx19.Text.Trim() != "" 
                    && textBoxNumEx20.Text.Trim() != "")
                {
                    summer = Convert.ToUInt64(textBoxNumEx1.Text.Trim());
                   
                    //计算指令的变量值后，重新给指令赋值
                    convert("commandleft", commandleft, lcoefficient, lacoefficient, lindex, laindex, textBoxNumEx3, textBoxNumEx19);
                    convert("commandright", commandright, rcoefficient, racoefficient, rindex, raindex, textBoxNumEx5, textBoxNumEx20);
                    convert("commandstop", commandstop, scoefficient, sacoefficient, sindex, saindex, textBoxNumEx25, textBoxNumEx26);
                    if (   Convert.ToDouble(textBoxNumEx2.Text.Trim()) != 0
                        && Convert.ToDouble(textBoxNumEx4.Text.Trim()) != 0
                        && Convert.ToDouble(textBoxNumEx6.Text.Trim()) != 0)
                    {
                        //按钮的制约关系
                        groupBox3.Enabled = false;
                        groupBox4.Enabled = false;
                        checkBox1.Enabled = false;
                        checkBox2.Enabled = false;
                        button1.Enabled = false;
                        button2.Enabled = true;
                        toolStripButton1.Enabled = true;
                    }
                    else
                    {
                        textBoxNumEx2.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("洗涤：请正确填写参数！");
                    textBoxNumEx1.Focus();
                }
            }
            else if (checkBox1.Checked == false && checkBox2.Checked == true)//脱水
            {
                if (   textBoxNumEx9.Text.Trim() != ""
                    && textBoxNumEx10.Text.Trim() != ""
                    && textBoxNumEx11.Text.Trim() != ""
                    && textBoxNumEx12.Text.Trim() != ""
                    && textBoxNumEx13.Text.Trim() != ""
                    && textBoxNumEx14.Text.Trim() != ""
                    && textBoxNumEx15.Text.Trim() != ""
                    && textBoxNumEx16.Text.Trim() != ""
                    && textBoxNumEx17.Text.Trim() != ""
                    && textBoxNumEx21.Text.Trim() != ""
                    && textBoxNumEx22.Text.Trim() != ""
                    && textBoxNumEx23.Text.Trim() != ""
                    && textBoxNumEx24.Text.Trim() != "")
                {
                    summer = Convert.ToUInt64(textBoxNumEx9.Text.Trim());
                    
                    //计算指令的变量值后，重新给指令赋值
                    convert("commandstop", commandstop, scoefficient, sacoefficient, sindex, saindex, textBoxNumEx25, textBoxNumEx26);
                    convert("spinlow", spinlow, spincoefficient, spinacoefficient, spindex, spaindex, textBoxNumEx11, textBoxNumEx21);
                    convert("spinmid", spinmid, spincoefficient, spinacoefficient, spindex, spaindex, textBoxNumEx13, textBoxNumEx22);
                    convert("spinhigh", spinhigh, spincoefficient, spinacoefficient, spindex, spaindex, textBoxNumEx15, textBoxNumEx23);
                    convert("spinbeyond", spinbeyond, spincoefficient, spinacoefficient, spindex, spaindex, textBoxNumEx17, textBoxNumEx24);
                    if (   Convert.ToDouble(textBoxNumEx10.Text.Trim()) != 0
                        && Convert.ToDouble(textBoxNumEx12.Text.Trim()) != 0
                        && Convert.ToDouble(textBoxNumEx14.Text.Trim()) != 0
                        && Convert.ToDouble(textBoxNumEx16.Text.Trim()) != 0)
                    {
                        //按钮的制约关系
                        groupBox3.Enabled = false;
                        groupBox4.Enabled = false;
                        checkBox1.Enabled = false;
                        checkBox2.Enabled = false;
                        button1.Enabled = false;
                        button2.Enabled = true;
                        toolStripButton1.Enabled = true;
                    }
                    else
                    {
                        textBoxNumEx10.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("脱水：请正确填写参数！");
                    textBoxNumEx9.Focus();
                }
            }
            else if (checkBox1.Checked == true && checkBox2.Checked == true)//洗涤和脱水
            {
                if (textBoxNumEx1.Text.Trim() != ""
                    && textBoxNumEx2.Text.Trim() != ""
                    && textBoxNumEx3.Text.Trim() != ""
                    && textBoxNumEx4.Text.Trim() != ""
                    && textBoxNumEx5.Text.Trim() != ""
                    && textBoxNumEx6.Text.Trim() != ""
                    && textBoxNumEx19.Text.Trim() != ""
                    && textBoxNumEx20.Text.Trim() != ""
                    && textBoxNumEx9.Text.Trim() != ""
                    && textBoxNumEx10.Text.Trim() != ""
                    && textBoxNumEx11.Text.Trim() != ""
                    && textBoxNumEx12.Text.Trim() != ""
                    && textBoxNumEx13.Text.Trim() != ""
                    && textBoxNumEx14.Text.Trim() != ""
                    && textBoxNumEx15.Text.Trim() != ""
                    && textBoxNumEx16.Text.Trim() != ""
                    && textBoxNumEx17.Text.Trim() != ""
                    && textBoxNumEx21.Text.Trim() != ""
                    && textBoxNumEx22.Text.Trim() != ""
                    && textBoxNumEx23.Text.Trim() != ""
                    && textBoxNumEx24.Text.Trim() != "")
                {
                    summer = Convert.ToUInt64(textBoxNumEx1.Text.Trim()) + Convert.ToUInt64(textBoxNumEx9.Text.Trim()) + myintermission;

                    //计算指令的变量值后，重新给指令赋值
                    convert("commandleft", commandleft, lcoefficient, lacoefficient, lindex, laindex, textBoxNumEx3, textBoxNumEx19);
                    convert("commandright", commandright, rcoefficient, racoefficient, rindex, raindex, textBoxNumEx5, textBoxNumEx20);
                    convert("commandstop", commandstop, scoefficient, sacoefficient, sindex, saindex, textBoxNumEx25, textBoxNumEx26);
                    convert("spinlow", spinlow, spincoefficient, spinacoefficient, spindex, spaindex, textBoxNumEx11, textBoxNumEx21);
                    convert("spinmid", spinmid, spincoefficient, spinacoefficient, spindex, spaindex, textBoxNumEx13, textBoxNumEx22);
                    convert("spinhigh", spinhigh, spincoefficient, spinacoefficient, spindex, spaindex, textBoxNumEx15, textBoxNumEx23);
                    convert("spinbeyond", spinbeyond, spincoefficient, spinacoefficient, spindex, spaindex, textBoxNumEx17, textBoxNumEx24);
                    if (   Convert.ToDouble(textBoxNumEx2.Text.Trim()) != 0
                        && Convert.ToDouble(textBoxNumEx4.Text.Trim()) != 0
                        && Convert.ToDouble(textBoxNumEx6.Text.Trim()) != 0
                        && Convert.ToDouble(textBoxNumEx10.Text.Trim()) != 0
                        && Convert.ToDouble(textBoxNumEx12.Text.Trim()) != 0
                        && Convert.ToDouble(textBoxNumEx14.Text.Trim()) != 0
                        && Convert.ToDouble(textBoxNumEx16.Text.Trim()) != 0)
                    {
                        //按钮的制约关系
                        groupBox3.Enabled = false;
                        groupBox4.Enabled = false;
                        checkBox1.Enabled = false;
                        checkBox2.Enabled = false;
                        button1.Enabled = false;
                        button2.Enabled = true;
                        toolStripButton1.Enabled = true;
                    }
                    else
                    {
                        textBoxNumEx1.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("洗涤并脱水：请正确填写参数！");
                    textBoxNumEx1.Focus();
                }
            }
            else
            {
                MessageBox.Show("请正确填写参数！");
                checkBox1.Focus();
            }
        }

        //速度的计算函数，并将相应数值写入index的位置
        private void convert(string name, string command, string coefficient, string acoefficient, string sindex, string aindex, TextBoxNumEx textBoxNumEx, TextBoxNumEx atextBoxNumEx)
        {
            int length = Convert.ToInt32(command.Length / 2);
            string tempcommands = null;
            string temp1 = null;//片段1
            string temp2 = null;//片段2
            string temp3 = null;//片段3
            string temp4 = null;//片段4
            string temp5 = null;//checksum
            double ttemp2 = 0;
            double ttemp3 = 0;
            int check = 0;
            //当符合if要求时
            if (Convert.ToInt32(sindex.Substring(0, 1)) <= length && Convert.ToInt32(sindex.Substring(0, 1)) != 0)
            {
                //当第二位为1时，即为1个字节长度
                if (Convert.ToInt32(sindex.Substring(1, 1)) == 1)
                {
                    temp1 = command.Substring(0, (Convert.ToInt32(sindex.Substring(0, 1)) - 1) * 2);//保存前面不需要改动的值
                    temp2 = Convert.ToInt32(Convert.ToDouble(textBoxNumEx.Text) * Convert.ToDouble(coefficient)).ToString("X2");//计算第一个系数
                    //不为零且小于字符串长度时
                    if (Convert.ToInt32(aindex.Substring(0, 1)) <= length && Convert.ToInt32(aindex.Substring(0, 1)) != 0)
                    {
                        //第二系数的特殊照顾
                        if (Convert.ToInt32(aindex.Substring(1, 1)) == 1)//第二系数为1字节
                        {
                            temp3 = Convert.ToInt32(Convert.ToDouble(atextBoxNumEx.Text) * Convert.ToDouble(acoefficient)).ToString("X2");//计算第二个系数
                        }
                        else if (Convert.ToInt32(aindex.Substring(1, 1)) == 2 && Convert.ToInt32(aindex.Substring(2, 1)) == 1)//第二系数为2字节且小端序
                        {
                            ttemp3 = Convert.ToDouble(atextBoxNumEx.Text) * Convert.ToDouble(acoefficient);
                            temp3 = Convert.ToInt32(ttemp3 % 256).ToString("X2") + Convert.ToInt32(ttemp3 / 256).ToString("X2");//计算第二个系数
                        }
                        else if (Convert.ToInt32(aindex.Substring(1, 1)) == 2 && Convert.ToInt32(aindex.Substring(2, 1)) == 0)//第二系数为2字节且大端序
                        {
                            temp3 = Convert.ToInt32(Convert.ToDouble(atextBoxNumEx.Text) * Convert.ToDouble(acoefficient)).ToString("X4");//计算第二个系数
                        }
                    }
                    else
                    {
                        temp3 = "";
                    }
                }
                //当第二位为2时，即为2个字节长度；第三位为1时，即为小端序
                else if (Convert.ToInt32(sindex.Substring(1, 1)) == 2 && Convert.ToInt32(sindex.Substring(2, 1)) == 1)
                {
                    temp1 = command.Substring(0, (Convert.ToInt32(sindex.Substring(0, 1)) - 1) * 2);//保存前面不需要改动的值
                    ttemp2 = Convert.ToDouble(textBoxNumEx.Text) * Convert.ToDouble(coefficient);
                    temp2 = Convert.ToInt32(ttemp2 % 256).ToString("X2") + Convert.ToInt32(ttemp2 / 256).ToString("X2");//计算第二个系数
                    if (Convert.ToInt32(aindex.Substring(0, 1)) <= length && Convert.ToInt32(aindex.Substring(0, 1)) != 0)
                    {
                        //第二系数的特殊照顾
                        if (Convert.ToInt32(aindex.Substring(1, 1)) == 1)//第二系数为1字节
                        {
                            temp3 = Convert.ToInt32(Convert.ToDouble(atextBoxNumEx.Text) * Convert.ToDouble(acoefficient)).ToString("X2");//计算第二个系数
                        }
                        else if (Convert.ToInt32(aindex.Substring(1, 1)) == 2 && Convert.ToInt32(aindex.Substring(2, 1)) == 1)//第二系数为2字节且小端序
                        {
                            ttemp3 = Convert.ToDouble(atextBoxNumEx.Text) * Convert.ToDouble(acoefficient);
                            temp3 = Convert.ToInt32(ttemp3 % 256).ToString("X2") + Convert.ToInt32(ttemp3 / 256).ToString("X2");//计算第二个系数
                        }
                        else if (Convert.ToInt32(aindex.Substring(1, 1)) == 2 && Convert.ToInt32(aindex.Substring(2, 1)) == 0)//第二系数为2字节且大端序
                        {
                            temp3 = Convert.ToInt32(Convert.ToDouble(atextBoxNumEx.Text) * Convert.ToDouble(acoefficient)).ToString("X4");//计算第二个系数
                        }
                    }
                    else
                    {
                        temp3 = "";
                    }
                    //temp4 = command.Substring((Convert.ToInt32(aindex.Substring(0, 1)) - 1 + Convert.ToInt32(aindex.Substring(1, 1))) * 2, (length - Convert.ToInt32(aindex.Substring(0, 1)) + 1 - Convert.ToInt32(aindex.Substring(1, 1))) * 2);//保存最后不需要改动的值
                }
                //当第二位为2时，即为2个字节长度；第三位为0时，即为大端序
                else if (Convert.ToInt32(sindex.Substring(1, 1)) == 2 && Convert.ToInt32(sindex.Substring(2, 1)) == 0)
                {
                    temp1 = command.Substring(0, (Convert.ToInt32(sindex.Substring(0, 1)) - 1) * 2);//保存前面不需要改动的值
                    temp2 = Convert.ToInt32(Convert.ToDouble(textBoxNumEx.Text) * Convert.ToDouble(coefficient)).ToString("X4");//计算第一个系数
                    if (Convert.ToInt32(aindex.Substring(0, 1)) <= length && Convert.ToInt32(aindex.Substring(0, 1)) != 0)
                    {
                        //第二系数的特殊照顾
                        if (Convert.ToInt32(aindex.Substring(1, 1)) == 1)//第二系数为1字节
                        {
                            temp3 = Convert.ToInt32(Convert.ToDouble(atextBoxNumEx.Text) * Convert.ToDouble(acoefficient)).ToString("X2");//计算第二个系数
                        }
                        else if (Convert.ToInt32(aindex.Substring(1, 1)) == 2 && Convert.ToInt32(aindex.Substring(2, 1)) == 1)//第二系数为2字节且小端序
                        {
                            ttemp3 = Convert.ToDouble(atextBoxNumEx.Text) * Convert.ToDouble(acoefficient);
                            temp3 = Convert.ToInt32(ttemp3 % 256).ToString("X2") + Convert.ToInt32(ttemp3 / 256).ToString("X2");//计算第二个系数
                        }
                        else if (Convert.ToInt32(aindex.Substring(1, 1)) == 2 && Convert.ToInt32(aindex.Substring(2, 1)) == 0)//第二系数为2字节且大端序
                        {
                            temp3 = Convert.ToInt32(Convert.ToDouble(atextBoxNumEx.Text) * Convert.ToDouble(acoefficient)).ToString("X4");//计算第二个系数
                        }
                    }
                    else
                    {
                        temp3 = "";
                    }
                    //temp4 = command.Substring((Convert.ToInt32(aindex.Substring(0, 1)) - 1 + Convert.ToInt32(aindex.Substring(1, 1))) * 2, (length - Convert.ToInt32(aindex.Substring(0, 1)) + 1 - Convert.ToInt32(aindex.Substring(1, 1))) * 2);//保存最后不需要改动的值
                }
                //第四片段的处理(剩余不做处理的片段)
                if (Convert.ToInt32(sindex.Substring(0, 1)) < Convert.ToInt32(aindex.Substring(0, 1)) && Convert.ToInt32(aindex.Substring(0, 1)) <= length)
                {
                    temp4 = command.Substring((Convert.ToInt32(aindex.Substring(0, 1)) - 1 + Convert.ToInt32(aindex.Substring(1, 1))) * 2, (length - Convert.ToInt32(aindex.Substring(0, 1)) + 1 - Convert.ToInt32(aindex.Substring(1, 1))) * 2);//保存最后不需要改动的值
                }
                else if (Convert.ToInt32(sindex.Substring(0, 1)) < Convert.ToInt32(aindex.Substring(0, 1)) && Convert.ToInt32(aindex.Substring(0, 1)) > length)
                {
                    temp4 = command.Substring((Convert.ToInt32(sindex.Substring(0, 1)) - 1 + Convert.ToInt32(sindex.Substring(1, 1))) * 2, (length - Convert.ToInt32(sindex.Substring(0, 1)) + 1 - Convert.ToInt32(sindex.Substring(1, 1))) * 2);//保存最后不需要改动的值
                }
                else if (Convert.ToInt32(sindex.Substring(0, 1)) > Convert.ToInt32(aindex.Substring(0, 1)))
                {
                    temp4 = command.Substring((Convert.ToInt32(sindex.Substring(0, 1)) - 1 + Convert.ToInt32(sindex.Substring(1, 1))) * 2, (length - Convert.ToInt32(sindex.Substring(0, 1)) + 1 - Convert.ToInt32(sindex.Substring(1, 1))) * 2);//保存最后不需要改动的值
                }
                //片段整合
                if (Convert.ToInt32(sindex.Substring(0, 1)) < Convert.ToInt32(aindex.Substring(0, 1)))
                {
                    tempcommands = temp1 + temp2 + temp3 + temp4;//整合 
                }
                else
                {
                    tempcommands = temp1 + temp3 + temp2 + temp4;//整合 
                }
                
                //checksum的计算
                for (int i = 0; i < length; i++ )
                {
                    check += Convert.ToInt32(tempcommands.Substring(i * 2, 2), 16);
                }
                if (Convert.ToInt32(mychecksum.Substring(0, 1)) == 2)//仅加和
                {
                    temp5 = (check % 256).ToString("X2");
                    if (Convert.ToInt32(mychecksum.Substring(1, 1)) == 0)
                    {
                        tempcommands = temp5 + tempcommands;//整合 
                    }
                    else if (Convert.ToInt32(mychecksum.Substring(1, 1)) == 1)
                    {
                        tempcommands = tempcommands + temp5;//整合 
                    }
                }
                else if (Convert.ToInt32(mychecksum.Substring(0, 1)) == 1)//求补码
                {
                    temp5 = ((256 - check % 256) % 256).ToString("X2");
                    if (Convert.ToInt32(mychecksum.Substring(1, 1)) == 0)
                    {
                        tempcommands = temp5 + tempcommands;//整合 
                    }
                    else if (Convert.ToInt32(mychecksum.Substring(1, 1)) == 1)
                    {
                        tempcommands = tempcommands + temp5;//整合 
                    }
                }
                else if (Convert.ToInt32(mychecksum.Substring(0, 1)) == 0)//不checksum
                {
                    temp5 = "";
                    //tempcommands = tempcommands;//整合 
                }
                //CRC16_1
                else if (Convert.ToInt32(mychecksum.Substring(0, 1)) == 3)
                {
                    ushort i;
                    ushort CRC16 = 65535;
                    ushort tempcrc1;
                    ushort tempcrc2;
                    ushort[] CurrentDataByte = new ushort[length - 1];
                    for (int cc = 0; cc < CurrentDataByte.Length; cc++)
                    {
                        CurrentDataByte[cc] += Convert.ToUInt16(tempcommands.Substring((cc + 1) * 2, 2), 16);
                    }
                    for (i = 0; i < CurrentDataByte.Length; i++)
                    {
                        // Since the CRC is the remainder of modulo 2 division, we lookup the
                        // pre-computed remainder value from the table.
                        // CRC = Table[(CRC>>8) xor Data] xor (CRC<<8)
                        tempcrc1 = Convert.ToUInt16(((CRC16 >> 8) ^ CurrentDataByte[i]) & 0xFF);
                        tempcrc2 = Convert.ToUInt16((Convert.ToString((CRC16 << 8), 2).PadLeft(24, '0').Substring(8, 16)), 2);
                        CRC16 = Convert.ToUInt16(CRC16LookupTable[tempcrc1] ^ (tempcrc2));
                    }
                    temp5 = CRC16.ToString("X4");
                    tempcommands = tempcommands + temp5;//整合 
                }
                //CRC8_1
                else if (Convert.ToInt32(mychecksum.Substring(0, 1)) == 4)
                {
                    byte CRC8 = 0;
                    for (int i = 0; i < length; i++)
                    {
                        CRC8 = CRC8_Process(Convert.ToByte(Convert.ToInt16(tempcommands.Substring(i * 2, 2), 16)), CRC8);
                    }
                    temp5 = CRC8.ToString("X2");
                    tempcommands = tempcommands + temp5;//整合 
                }
            }
            //当不符合上述要求时
            else
            {
                tempcommands = command;
            }
            //加入开始字节
            if (mystart == "")
            { ;}
            else
            {
                if (ishex == "true")
                {
                    tempcommands = mystart + tempcommands;
                }
                else if (ishex == "false")
                {
                    tempcommands = toascii(mystart) + tempcommands;
                }
            }
            //加入停止字节
            if (mystop == "")
            { ;}
            else
            {
                if (ishex == "true")
                {
                    tempcommands = tempcommands + mystop;
                }
                else if (ishex == "false")
                {
                    tempcommands = tempcommands + toascii(mystop);
                }
            }

            switch (name)
            {
                case "commandleft":
                    mycommandleft = tempcommands;
                    break;
                case "commandright":
                    mycommandright = tempcommands;
                    break;
                case "commandstop":
                    mycommandstop = tempcommands;
                    break;
                case "spinlow":
                    myspinlow = tempcommands;
                    break;
                case "spinmid":
                    myspinmid = tempcommands;
                    break;
                case "spinhigh":
                    myspinhigh = tempcommands;
                    break;
                case "spinbeyond":
                    myspinbeyond = tempcommands;
                    break;
                default:
                    break;
            }

            //根据数据长度调整默认间隔时间
            if (tempcommands.Length > maxlength)
            {
                maxlength = tempcommands.Length;
                //test length is 20
                //12345678901234567890
                if (ishex == "true")
                {
                    if (maxlength <= 30)
                    {
                        TimerTextBox.Text = "50";
                    }
                    else if (maxlength > 30 && maxlength <= 40)
                    {
                        TimerTextBox.Text = "80";
                    }
                    else
                    {
                        TimerTextBox.Text = "500";
                    }
                }
                else
                {
                    if (serialPort1.BaudRate < 2400)
                    {
                        TimerTextBox.Text = "200";
                    }
                    else if (serialPort1.BaudRate >= 2400 && serialPort1.BaudRate < 4800)
                    {
                        if (maxlength <= 15)
                        {
                            TimerTextBox.Text = "100";
                        }
                        else if (maxlength > 15 && maxlength < 20)
                        {
                            TimerTextBox.Text = "100";
                        }
                        else if (maxlength >= 20 && maxlength < 24)
                        {
                            TimerTextBox.Text = "100";
                        }
                        else if (maxlength >= 24 && maxlength < 28)
                        {
                            TimerTextBox.Text = "100";
                        }
                        else if (maxlength >= 28 && maxlength < 32)
                        {
                            TimerTextBox.Text = "150";
                        }
                        else if (maxlength >= 32 && maxlength < 36)
                        {
                            TimerTextBox.Text = "150";
                        }
                        else if (maxlength >= 36 && maxlength < 40)
                        {
                            TimerTextBox.Text = "150";
                        }
                        else
                        {
                            TimerTextBox.Text = "500";
                        }
                    }
                    else if (serialPort1.BaudRate >= 4800 && serialPort1.BaudRate < 9600)
                    {
                        if (maxlength <= 31)
                        {
                            TimerTextBox.Text = "100";
                        }
                        else if (maxlength > 31 && maxlength < 40)
                        {
                            TimerTextBox.Text = "100";
                        }
                        else
                        {
                            TimerTextBox.Text = "500";
                        }
                    }
                    else
                    {
                        if (maxlength <= 40)
                        {
                            TimerTextBox.Text = "100";
                        }
                        else
                        {
                            TimerTextBox.Text = "500";
                        }
                    }
                }
            }
        }

        private byte CRC8_Process(byte Data, byte OldCRC8)
        {
            return (CRC8_Table[Data ^ OldCRC8]);
        }

        //十六进制到ASCII
        private string toascii(string x)
        {
            int iValue;
            string sValue;
            iValue = Convert.ToInt32(x, 16); // 16进制->10进制
            sValue = System.Text.Encoding.ASCII.GetString(System.BitConverter.GetBytes(iValue));  //Int-> ASCII 
            sValue = sValue.Substring(0,1);
            return sValue;
        }

        //修改
        private void button2_Click(object sender, EventArgs e)
        {
            //toolStripLabel1.Visible = true;
            //TimerTextBox.Visible = true;
            if (checkBox1.Checked == true)
            {
                groupBox3.Enabled = true;
                checkBox1.Enabled = true;
                checkBox2.Enabled = true;
            }
            if (checkBox2.Checked == true)
            {
                groupBox4.Enabled = true;
                checkBox1.Enabled = true;
                checkBox2.Enabled = true;
            }
            button1.Enabled = true;
            button2.Enabled = false;
            //toolStripButton1.Enabled = false;
        }

        //开始按钮
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            startbutton();
        }

        //开始按钮的事件
        private void startbutton()
        {
            try
            {
                //countstart++;
                if (toolStripButton1.Text == "开始运行")
                {
                    serialPort1.Open();
                    toolStripMenuItem11.Enabled = false;
                    toolStripMenuItem12.Enabled = false;
                    toolStripMenuItem13.Enabled = true;
                    textBoxNumEx27.Text = "00:00:00";
                    textBoxNumEx28.Text = "0";
                    toolStripButton1.Text = "停止运行";
                    abc = myintermission;//中间间隔的时间
                    if (checkBox1.Checked == true && checkBox2.Checked == false)
                    {
                        timer2.Enabled = true;
                        timer1.Enabled = true;
                    }
                    else if (checkBox1.Checked == false && checkBox2.Checked == true)
                    {
                        timer5.Enabled = true;
                        timer1.Enabled = true;
                    }
                    else if (checkBox1.Checked == true && checkBox2.Checked == true)
                    {
                        timer2.Enabled = true;
                        timer1.Enabled = true;
                    }
                }
                else if (toolStripButton1.Text == "停止运行")
                {
                    DialogResult result = MessageBox.Show("Are you sure?", "Stop", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (result == DialogResult.OK)
                    {
                        //发送停止指令
                        //Debug2Action.debug2Action(this, mycommandstop,ishex);
                        //serialPort1.Close();
                        //groupBox2.Enabled = true;
                        timer8.Enabled = false;
                        timer7.Enabled = false;
                        timer6.Enabled = false;
                        timer5.Enabled = false;
                        timer4.Enabled = false;
                        timer3.Enabled = false;
                        timer2.Enabled = false;
                        timer1.Enabled = false;
                        count1 = 0;
                        count2 = 0;
                        count3 = 0;
                        count4 = 0;
                        count5 = 0;
                        count6 = 0;
                        count7 = 0;
                        count8 = 0;
                        //countstart = 0;
                        washcount = 0;
                        spincount = 0;
                        toolStripMenuItem11.Enabled = true;
                        toolStripMenuItem12.Enabled = true;
                        toolStripMenuItem13.Enabled = false;
                        toolStripButton1.Text = "开始运行";
                        toolStripButton1.Enabled = false;
                        if (serialPort1.IsOpen)
                        {
                            timer10.Enabled = true;
                        }
                        else
                        {
                            toolStripButton1.Enabled = true;
                        }
                    }
                    else
                    {
                        //countstart = 1;
                        toolStripButton1.Text = "停止运行";
                    }
                } 
                else if (toolStripButton1.Text == "START")
                {
                    serialPort1.Open();
                    toolStripMenuItem11.Enabled = false;
                    toolStripMenuItem12.Enabled = false;
                    toolStripMenuItem13.Enabled = true;
                    textBoxNumEx27.Text = "00:00:00";
                    textBoxNumEx28.Text = "0";
                    toolStripButton1.Text = "STOP";
                    abc = myintermission;//中间间隔的时间
                    if (checkBox1.Checked == true && checkBox2.Checked == false)
                    {
                        timer2.Enabled = true;
                        timer1.Enabled = true;
                    }
                    else if (checkBox1.Checked == false && checkBox2.Checked == true)
                    {
                        timer5.Enabled = true;
                        timer1.Enabled = true;
                    }
                    else if (checkBox1.Checked == true && checkBox2.Checked == true)
                    {
                        timer2.Enabled = true;
                        timer1.Enabled = true;
                    }
                }
                else if (toolStripButton1.Text == "STOP")
                {
                    DialogResult result = MessageBox.Show("Are you sure?", "Stop", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (result == DialogResult.OK)
                    {
                        //发送停止指令
                        //Debug2Action.debug2Action(this, mycommandstop,ishex);
                        //serialPort1.Close();
                        //groupBox2.Enabled = true;
                        timer8.Enabled = false;
                        timer7.Enabled = false;
                        timer6.Enabled = false;
                        timer5.Enabled = false;
                        timer4.Enabled = false;
                        timer3.Enabled = false;
                        timer2.Enabled = false;
                        timer1.Enabled = false;
                        count1 = 0;
                        count2 = 0;
                        count3 = 0;
                        count4 = 0;
                        count5 = 0;
                        count6 = 0;
                        count7 = 0;
                        count8 = 0;
                        //countstart = 0;
                        washcount = 0;
                        spincount = 0;
                        toolStripMenuItem11.Enabled = true;
                        toolStripMenuItem12.Enabled = true;
                        toolStripMenuItem13.Enabled = false;
                        toolStripButton1.Text = "START";
                        toolStripButton1.Enabled = false;
                        if (serialPort1.IsOpen)
                        {
                            timer10.Enabled = true;
                        }
                        else
                        {
                            toolStripButton1.Enabled = true;
                        }
                    }
                    else
                    {
                        //countstart = 1;
                        toolStripButton1.Text = "STOP";
                    }
                }
            }
            catch (Exception ex)
            {
                //countstart = 0;
                if (Form1.languageflag == (int)Form1.Language.English)
                {
                    toolStripButton1.Text = "START";
                }
                else if (Form1.languageflag == (int)Form1.Language.Chinese)
                {
                    toolStripButton1.Text = "开始运行";
                }
                MessageBox.Show("请将主窗体的串口关闭，或者端口不存在!", ex.Message);
            }
        }

        //停止使用的程序
        private void timer10_Tick(object sender, EventArgs e)
        {
            count10++;
            Debug2Action.debug2Action(this, mycommandstop, ishex);
            timer8.Enabled = false;
            timer7.Enabled = false;
            timer6.Enabled = false;
            timer5.Enabled = false;
            timer4.Enabled = false;
            timer3.Enabled = false;
            timer2.Enabled = false;
            timer1.Enabled = false;
            count1 = 0;
            count2 = 0;
            count3 = 0;
            count4 = 0;
            count5 = 0;
            count6 = 0;
            count7 = 0;
            count8 = 0;
            //countstart = 0;
            washcount = 0;
            spincount = 0;
            if (count10 >= (2000 / Convert.ToDouble(timer10.Interval)))
            {
                count10 = 0;
                serialPort1.Close();
                timer10.Enabled = false;
                toolStripButton1.Enabled = true;
            }
        }

        #endregion

        #region Timer计时器相关代码

        //连续运行的间隔时间的计时
        private void timer9_Tick(object sender, EventArgs e)
        {
            count9++;
            textBoxNowStatus.Text = "Wait for：" + Convert.ToString(myintermission - count9);
            if (count9 > myintermission)
            {
                startbutton();
                timer9.Enabled = false;
                count9 = 0;
            }
        }

        //timer1的tick，主流程控制
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                count1++;
                textBoxNumEx27.Text = (count1 / 60 / 60).ToString("D2") + ":" + (count1 / 60 % 60).ToString("D2") + ":" + (count1 % 60).ToString("D2");
                //只洗涤
                if (checkBox1.Checked == true && checkBox2.Checked == false)
                {
                    //if (count1 % Convert.ToDouble(textBoxNumEx8.Text) == 0)
                    //{
                    //    textBoxNumEx28.Text = Convert.ToInt32(count1 / Convert.ToDouble(textBoxNumEx8.Text)).ToString();
                    //}
                    textBoxNumEx28.Text = washcount.ToString();
                    if (count1 >= summer)
                    {
                        Debug2Action.debug2Action(this, mycommandstop, ishex);
                        serialPort1.Close();
                        timer8.Enabled = false;
                        timer7.Enabled = false;
                        timer6.Enabled = false;
                        timer5.Enabled = false;
                        timer4.Enabled = false;
                        timer3.Enabled = false;
                        timer2.Enabled = false;
                        timer1.Enabled = false;
                        count1 = 0;
                        count2 = 0;
                        count3 = 0;
                        count4 = 0;
                        count5 = 0;
                        count6 = 0;
                        count7 = 0;
                        count8 = 0;
                        //countstart = 0;
                        washcount = 0;
                        spincount = 0;
                        toolStripMenuItem11.Enabled = true;
                        toolStripMenuItem12.Enabled = true;
                        toolStripMenuItem13.Enabled = false;
                        if (Form1.languageflag == (int)Form1.Language.English)
                        {
                            toolStripButton1.Text = "START";
                        }
                        else if (Form1.languageflag == (int)Form1.Language.Chinese)
                        {
                            toolStripButton1.Text = "开始运行";
                        }
                        textBoxNowStatus.Text = "Already Stoped！";
                        if (checkBox3.Checked == true)
                        {
                            timer9.Enabled = true;
                        }
                    }
                }
                //只脱水
                else if (checkBox1.Checked == false && checkBox2.Checked == true)
                {
                    if (count1 >= summer)//判断后停止运行
                    {
                        Debug2Action.debug2Action(this, mycommandstop, ishex);
                        serialPort1.Close();
                        timer8.Enabled = false;
                        timer7.Enabled = false;
                        timer6.Enabled = false;
                        timer5.Enabled = false;
                        timer4.Enabled = false;
                        timer3.Enabled = false;
                        timer2.Enabled = false;
                        timer1.Enabled = false;
                        count1 = 0;
                        count2 = 0;
                        count3 = 0;
                        count4 = 0;
                        count5 = 0;
                        count6 = 0;
                        count7 = 0;
                        count8 = 0;
                        //countstart = 0;
                        washcount = 0;
                        spincount = 0;
                        toolStripMenuItem11.Enabled = true;
                        toolStripMenuItem12.Enabled = true;
                        toolStripMenuItem13.Enabled = false;
                        if (Form1.languageflag == (int)Form1.Language.English)
                        {
                            toolStripButton1.Text = "START";
                        }
                        else if (Form1.languageflag == (int)Form1.Language.Chinese)
                        {
                            toolStripButton1.Text = "开始运行";
                        }
                        textBoxNowStatus.Text = "Already Stoped！";
                        if (checkBox3.Checked == true)
                        {
                            timer9.Enabled = true;
                        }
                    }
                }
                //先洗涤后脱水
                else if (checkBox1.Checked == true && checkBox2.Checked == true)
                {
                    textBoxNumEx28.Text = washcount.ToString();
                    if (Convert.ToDouble(count1) >= Convert.ToDouble(textBoxNumEx1.Text) && Convert.ToDouble(count1) <= (Convert.ToDouble(textBoxNumEx1.Text) + Convert.ToDouble(myintermission)))
                    {
                        Debug2Action.debug2Action(this, mycommandstop, ishex);
                        timer2.Enabled = false;
                        timer3.Enabled = false;
                        timer4.Enabled = false;
                        textBoxNowStatus.Text = "Wait for " + abc + "s to SPIN";
                        abc--;
                    }
                    else if (Convert.ToDouble(count1) > (Convert.ToDouble(textBoxNumEx1.Text) + Convert.ToDouble(myintermission)) && count1 < summer)
                    {
                        spincount++;
                        if (spincount == 1)
                        {
                            timer5.Enabled = true;
                        }
                    }
                    else if (count1 >= summer)
                    {
                        Debug2Action.debug2Action(this, mycommandstop, ishex);
                        serialPort1.Close();
                        timer8.Enabled = false;
                        timer7.Enabled = false;
                        timer6.Enabled = false;
                        timer5.Enabled = false;
                        timer4.Enabled = false;
                        timer3.Enabled = false;
                        timer2.Enabled = false;
                        timer1.Enabled = false;
                        count1 = 0;
                        count2 = 0;
                        count3 = 0;
                        count4 = 0;
                        count5 = 0;
                        count6 = 0;
                        count7 = 0;
                        count8 = 0;
                        //countstart = 0;
                        washcount = 0;
                        spincount = 0;
                        toolStripMenuItem11.Enabled = true;
                        toolStripMenuItem12.Enabled = true;
                        toolStripMenuItem13.Enabled = false;
                        if (Form1.languageflag == (int)Form1.Language.English)
                        {
                            toolStripButton1.Text = "START";
                        }
                        else if (Form1.languageflag == (int)Form1.Language.Chinese)
                        {
                            toolStripButton1.Text = "开始运行";
                        }
                        textBoxNowStatus.Text = "Already Stoped！";
                        if (checkBox3.Checked == true)
                        {
                            timer9.Enabled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                timer1.Enabled = false;
                serialPort1.Close();
                MessageBox.Show("Timer1's Error" + ex.Message);
                Debug2Action.debug2Action(this, mycommandstop, ishex);
                timer8.Enabled = false;
                timer7.Enabled = false;
                timer6.Enabled = false;
                timer5.Enabled = false;
                timer4.Enabled = false;
                timer3.Enabled = false;
                timer2.Enabled = false;
                count1 = 0;
                count2 = 0;
                count3 = 0;
                count4 = 0;
                count5 = 0;
                count6 = 0;
                count7 = 0;
                count8 = 0;
                //countstart = 0;
                if (Form1.languageflag == (int)Form1.Language.English)
                {
                    toolStripButton1.Text = "START";
                }
                else if (Form1.languageflag == (int)Form1.Language.Chinese)
                {
                    toolStripButton1.Text = "开始运行";
                }
            }
        }

        /// <summary>
        /// 洗涤部分
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        //timer2的tick
        private void timer2_Tick(object sender, EventArgs e)
        {
            try
            {
                count2++;
                //此处发送停止命令
                Debug2Action.debug2Action(this, mycommandstop, ishex);
                //textBoxNowStatus.Clear();
                textBoxNowStatus.Text = "STOP";
                //if (Convert.ToDouble(count2) >= (Convert.ToDouble(textBoxNumEx6.Text) * 20))
                if (Convert.ToDouble(count2) >= (1000 / Convert.ToDouble(timer2.Interval) * Convert.ToDouble(textBoxNumEx6.Text)))
                //if (count2 >= 10)
                {
                    count4++;
                    countstop = 0;
                    timer2.Enabled = false;
                    timer4.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Timer2's Error" + ex.Message);
                timer2.Enabled = false;
            }
        }

        //timer3的tick
        private void timer3_Tick(object sender, EventArgs e)
        {
            try
            {
                count3++;
                //此处发送停止命令
                Debug2Action.debug2Action(this, mycommandstop, ishex);
                //textBoxNowStatus.Clear();
                textBoxNowStatus.Text = "STOP";
                //if (Convert.ToDouble(count3) >= (Convert.ToDouble(textBoxNumEx6.Text) * 20))
                if (Convert.ToDouble(count3) >= (1000 / Convert.ToDouble(timer3.Interval) * Convert.ToDouble(textBoxNumEx6.Text)))
                {
                    count4++;
                    countstop = 0;
                    timer3.Enabled = false;
                    timer4.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Timer3's Error" + ex.Message);
                timer3.Enabled = false;
            }
        }

        //timer4的tick
        private void timer4_Tick(object sender, EventArgs e)
        {
            try
            {
                countstop++;
                if (count4 == 1)
                {
                    //此处发送左转命令
                    Debug2Action.debug2Action(this, mycommandleft, ishex);
                    //textBoxNowStatus.Clear();
                    textBoxNowStatus.Text = "WASH CW";
                    //if (Convert.ToDouble(countstop) >= (Convert.ToDouble(textBoxNumEx2.Text) * 20))
                    if (Convert.ToDouble(countstop) >= (1000 / Convert.ToDouble(timer4.Interval) * Convert.ToDouble(textBoxNumEx2.Text)))
                    {
                        count2 = 0;
                        timer4.Enabled = false;
                        timer2.Enabled = true;
                    }
                }
                else if (count4 == 2)
                {
                    //此处发送右转命令
                    Debug2Action.debug2Action(this, mycommandright, ishex);
                    //textBoxNowStatus.Clear();
                    textBoxNowStatus.Text = "WASH CCW";
                    //if (Convert.ToDouble(countstop) >= (Convert.ToDouble(textBoxNumEx4.Text) * 20))
                    if (Convert.ToDouble(countstop) >= (1000 / Convert.ToDouble(timer4.Interval) * Convert.ToDouble(textBoxNumEx4.Text)))
                    {
                        washcount++;
                        count4 = 0;
                        timer4.Enabled = false;
                        count3 = 0;
                        timer3.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Timer4's Error" + ex.Message);
                timer4.Enabled = false;
            }
        }

        /// <summary>
        /// 脱水部分
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //timer5的tick
        private void timer5_Tick(object sender, EventArgs e)
        {
            try
            {
                count5++;
                //此处发送脱水low命令
                Debug2Action.debug2Action(this, myspinlow, ishex);
                textBoxNowStatus.Text = "SPIN low";
                //if (count5 == (Convert.ToDouble(textBoxNumEx10.Text) * 20))
                if (count5 >= (1000 / Convert.ToDouble(timer5.Interval) * Convert.ToDouble(textBoxNumEx10.Text)))
                {
                    count5 = 0;
                    timer5.Enabled = false;
                    timer6.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Timer5's Error" + ex.Message);
                timer5.Enabled = false;
            }
        }

        //timer6的tick
        private void timer6_Tick(object sender, EventArgs e)
        {
            try
            {
                count6++;
                //此处发送脱水mid命令
                Debug2Action.debug2Action(this, myspinmid, ishex);
                textBoxNowStatus.Text = "SPIN mid";
                //if (count6 == (Convert.ToDouble(textBoxNumEx12.Text) * 20))
                if (count6 >= (1000 / Convert.ToDouble(timer6.Interval) * Convert.ToDouble(textBoxNumEx12.Text)))
                {
                    count6 = 0;
                    timer6.Enabled = false;
                    timer7.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Timer6's Error" + ex.Message);
                timer6.Enabled = false;
            }
        }

        //timer7的tick
        private void timer7_Tick(object sender, EventArgs e)
        {
            try
            {
                count7++;
                //此处发送脱水high命令
                Debug2Action.debug2Action(this, myspinhigh, ishex);
                textBoxNowStatus.Text = "SPIN high";
                //if (count7 == (Convert.ToDouble(textBoxNumEx14.Text) * 20))
                if (count7 >= (1000 / Convert.ToDouble(timer7.Interval) * Convert.ToDouble(textBoxNumEx14.Text)))
                {
                    count7 = 0;
                    timer7.Enabled = false;
                    timer8.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Timer7's Error" + ex.Message);
                timer7.Enabled = false;
            }
        }

        //timer8的tick
        private void timer8_Tick(object sender, EventArgs e)
        {
            try
            {
                count8++;
                //此处发送脱水beyond命令
                Debug2Action.debug2Action(this, myspinbeyond, ishex);
                textBoxNowStatus.Text = "SPIN beyond";
                //if (count8 == (Convert.ToDouble(textBoxNumEx16.Text) * 20))
                if (count8 >= (1000 / Convert.ToDouble(timer8.Interval) * Convert.ToDouble(textBoxNumEx16.Text)))
                {
                    count8 = 0;
                    timer8.Enabled = false;
                    //Debug2Action.debug2Action(this, commandstop);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Timer8's Error" + ex.Message);
                timer8.Enabled = false;
            }
        }

        #endregion

        #region checkBox事件、置顶事件、挂起事件

        //checkBox1控制洗涤设置区
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                checkBox3.Enabled = true;
                groupBox3.Enabled = true;
                button1.Enabled = true;
                if (checkBox2.Checked == false)
                {
                    if (checkBox3.Checked == false)
                    {
                        textBoxNumEx29.Enabled = false;
                    }
                    else
                    {
                        textBoxNumEx29.Enabled = true;
                    }
                    if (textBoxNumEx1.Text.Trim() != "")
                    {
                        textBoxNumEx18.Text = (Convert.ToUInt64(textBoxNumEx1.Text.Trim()) / 60 / 60).ToString("D2") + ":" + (Convert.ToUInt64(textBoxNumEx1.Text.Trim()) / 60 % 60).ToString("D2") + ":" + (Convert.ToUInt64(textBoxNumEx1.Text.Trim()) % 60).ToString("D2");
                        summer = Convert.ToUInt64(textBoxNumEx1.Text.Trim());
                    }
                    else
                    {
                        textBoxNumEx1.Focus();
                        summer = 0;
                    }
                }
                else
                {
                    textBoxNumEx29.Enabled = true;
                    if (textBoxNumEx1.Text.Trim() != "" && textBoxNumEx9.Text.Trim() != "")
                    {
                        //textBoxNumEx18.Text = ((Convert.ToUInt64(textBoxNumEx1.Text.Trim()) + Convert.ToUInt64(textBoxNumEx9.Text.Trim())) / 60 / 60).ToString("D2") + ":" + ((Convert.ToUInt64(textBoxNumEx1.Text.Trim()) + Convert.ToUInt64(textBoxNumEx9.Text.Trim())) / 60).ToString("D2") + ":" + ((Convert.ToUInt64(textBoxNumEx1.Text.Trim()) + Convert.ToUInt64(textBoxNumEx9.Text.Trim())) % 60).ToString("D2");
                        //summer = Convert.ToUInt64(textBoxNumEx1.Text.Trim()) + Convert.ToUInt64(textBoxNumEx9.Text.Trim());
                        textBoxNumEx18.Text = ((Convert.ToUInt64(textBoxNumEx1.Text.Trim()) + Convert.ToUInt64(textBoxNumEx9.Text.Trim()) + myintermission) / 60 / 60).ToString("D2") + ":" + ((Convert.ToUInt64(textBoxNumEx1.Text.Trim()) + Convert.ToUInt64(textBoxNumEx9.Text.Trim()) + myintermission) / 60 % 60).ToString("D2") + ":" + ((Convert.ToUInt64(textBoxNumEx1.Text.Trim()) + Convert.ToUInt64(textBoxNumEx9.Text.Trim()) + myintermission) % 60).ToString("D2");
                        summer = Convert.ToUInt64(textBoxNumEx1.Text.Trim()) + Convert.ToUInt64(textBoxNumEx9.Text.Trim()) + myintermission;
                    }
                    else if (textBoxNumEx1.Text.Trim() != "" && textBoxNumEx9.Text.Trim() == "")
                    {
                        textBoxNumEx18.Text = (Convert.ToUInt64(textBoxNumEx1.Text.Trim()) / 60 / 60).ToString("D2") + ":" + (Convert.ToUInt64(textBoxNumEx1.Text.Trim()) / 60 % 60).ToString("D2") + ":" + (Convert.ToUInt64(textBoxNumEx1.Text.Trim()) % 60).ToString("D2");
                        summer = Convert.ToUInt64(textBoxNumEx1.Text.Trim());
                    }
                    else if (textBoxNumEx1.Text.Trim() == "" && textBoxNumEx9.Text.Trim() != "")
                    {
                        textBoxNumEx18.Text = (Convert.ToUInt64(textBoxNumEx9.Text.Trim()) / 60 / 60).ToString("D2") + ":" + (Convert.ToUInt64(textBoxNumEx9.Text.Trim()) / 60 % 60).ToString("D2") + ":" + (Convert.ToUInt64(textBoxNumEx9.Text.Trim()) % 60).ToString("D2");
                        summer = Convert.ToUInt64(textBoxNumEx9.Text.Trim());
                    }
                    else if (textBoxNumEx1.Text.Trim() == "" && textBoxNumEx9.Text.Trim() == "")
                    {
                        textBoxNumEx1.Focus();
                        summer = 0;
                    }
                }
            }
            else
            {
                groupBox3.Enabled = false;
                if (checkBox2.Checked == false)
                {
                    checkBox3.Enabled = false;
                    textBoxNumEx29.Enabled = false;
                    button1.Enabled = false;
                    textBoxNumEx18.Text = "";
                    summer = 0;
                }
                else
                {
                    checkBox3.Enabled = true;
                    if (checkBox3.Checked == false)
                    {
                        textBoxNumEx29.Enabled = false;
                    }
                    else
                    {
                        textBoxNumEx29.Enabled = true;
                    }
                    button1.Enabled = true;
                    if (textBoxNumEx9.Text.Trim() != "")
                    {
                        textBoxNumEx18.Text = (Convert.ToUInt64(textBoxNumEx9.Text.Trim()) / 60 / 60).ToString("D2") + ":" + (Convert.ToUInt64(textBoxNumEx9.Text.Trim()) / 60 % 60).ToString("D2") + ":" + (Convert.ToUInt64(textBoxNumEx9.Text.Trim()) % 60).ToString("D2");
                        summer = Convert.ToUInt64(textBoxNumEx9.Text.Trim());
                    }
                    else
                    {
                        textBoxNumEx18.Text = "";
                        summer = 0;
                    }
                }
            }
        }

        //checkBox2控制脱水设置区
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                checkBox3.Enabled = true;
                groupBox4.Enabled = true;
                button1.Enabled = true;
                if (checkBox1.Checked == false)
                {
                    if (checkBox3.Checked == false)
                    {
                        textBoxNumEx29.Enabled = false;
                    }
                    else
                    {
                        textBoxNumEx29.Enabled = true;
                    }
                    if (textBoxNumEx9.Text.Trim() != "")
                    {
                        textBoxNumEx18.Text = (Convert.ToUInt64(textBoxNumEx9.Text.Trim()) / 60 / 60).ToString("D2") + ":" + (Convert.ToUInt64(textBoxNumEx9.Text.Trim()) / 60 % 60).ToString("D2") + ":" + (Convert.ToUInt64(textBoxNumEx9.Text.Trim()) % 60).ToString("D2");
                        summer = Convert.ToUInt64(textBoxNumEx1.Text.Trim());
                    }
                    else
                    {
                        textBoxNumEx9.Focus();
                        summer = 0;
                    }
                }
                else
                {
                    textBoxNumEx29.Enabled = true;
                    if (textBoxNumEx1.Text.Trim() != "" && textBoxNumEx9.Text.Trim() != "")
                    {
                        textBoxNumEx18.Text = ((Convert.ToUInt64(textBoxNumEx1.Text.Trim()) + Convert.ToUInt64(textBoxNumEx9.Text.Trim()) + myintermission) / 60 / 60).ToString("D2") + ":" + ((Convert.ToUInt64(textBoxNumEx1.Text.Trim()) + Convert.ToUInt64(textBoxNumEx9.Text.Trim()) + myintermission) / 60 % 60).ToString("D2") + ":" + ((Convert.ToUInt64(textBoxNumEx1.Text.Trim()) + Convert.ToUInt64(textBoxNumEx9.Text.Trim()) + myintermission) % 60).ToString("D2");
                        summer = Convert.ToUInt64(textBoxNumEx1.Text.Trim()) + Convert.ToUInt64(textBoxNumEx9.Text.Trim()) + myintermission;
                    }
                    else if (textBoxNumEx1.Text.Trim() != "" && textBoxNumEx9.Text.Trim() == "")
                    {
                        textBoxNumEx18.Text = (Convert.ToUInt64(textBoxNumEx1.Text.Trim()) / 60 / 60).ToString("D2") + ":" + (Convert.ToUInt64(textBoxNumEx1.Text.Trim()) / 60 % 60).ToString("D2") + ":" + (Convert.ToUInt64(textBoxNumEx1.Text.Trim()) % 60).ToString("D2");
                        summer = Convert.ToUInt64(textBoxNumEx1.Text.Trim());
                    }
                    else if (textBoxNumEx1.Text.Trim() == "" && textBoxNumEx9.Text.Trim() != "")
                    {
                        textBoxNumEx18.Text = (Convert.ToUInt64(textBoxNumEx9.Text.Trim()) / 60 / 60).ToString("D2") + ":" + (Convert.ToUInt64(textBoxNumEx9.Text.Trim()) / 60 % 60).ToString("D2") + ":" + (Convert.ToUInt64(textBoxNumEx9.Text.Trim()) % 60).ToString("D2");
                        summer = Convert.ToUInt64(textBoxNumEx9.Text.Trim());
                    }
                    else if (textBoxNumEx1.Text.Trim() == "" && textBoxNumEx9.Text.Trim() == "")
                    {
                        textBoxNumEx1.Focus();
                        summer = 0;
                    }
                }
            }
            else
            {
                groupBox4.Enabled = false;
                if (checkBox1.Checked == false)
                {
                    checkBox3.Enabled = false;
                    textBoxNumEx29.Enabled = false;
                    button1.Enabled = false;
                    textBoxNumEx18.Text = "";
                    summer = 0;
                }
                else
                {
                    checkBox3.Enabled = true;
                    if (checkBox3.Checked == false)
                    {
                        textBoxNumEx29.Enabled = false;
                    }
                    else
                    {
                        textBoxNumEx29.Enabled = true;
                    }
                    button1.Enabled = true;
                    if (textBoxNumEx1.Text.Trim() != "")
                    {
                        textBoxNumEx18.Text = (Convert.ToUInt64(textBoxNumEx1.Text.Trim()) / 60 / 60).ToString("D2") + ":" + (Convert.ToUInt64(textBoxNumEx1.Text.Trim()) / 60 % 60).ToString("D2") + ":" + (Convert.ToUInt64(textBoxNumEx1.Text.Trim()) % 60).ToString("D2");
                        summer = Convert.ToUInt64(textBoxNumEx1.Text.Trim());
                    }
                    else
                    {
                        textBoxNumEx18.Text = "";
                        summer = 0;
                    }
                }
            }
        }

        //循环的选择
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked == false)
            {
                if (checkBox1.Checked == true && checkBox2.Checked == true)
                {
                    textBoxNumEx29.Enabled = true;
                }
                else
                {
                    textBoxNumEx29.Enabled = false;
                }
            }
            else
            {
                if (checkBox1.Checked == true || checkBox2.Checked == true)
                {
                    textBoxNumEx29.Enabled = true;
                }
                else
                {
                    textBoxNumEx29.Enabled = false;
                }
            }
        }

        //置顶
        private void toolStripMenuItem14_Click(object sender, EventArgs e)
        {
            ton++;
            if (ton % 2 != 0)
            {
                this.TopMost = true;
            }
            else
            {
                this.TopMost = false;
                if (ton == 2)
                {
                    ton = 0;
                }
            }
        }

        //挂起
        private void toolStripMenuItem13_Click(object sender, EventArgs e)
        {
            countsave++;
            if (countsave == 1)
            {
                save_timer = timer1.Enabled;
                timer1.Enabled = false;
                if (timer2.Enabled == true)
                {
                    save_name = "timer2";
                    timer2.Enabled = false;
                }
                else if (timer3.Enabled == true)
                {
                    save_name = "timer3";
                    timer3.Enabled = false;
                }
                else if (timer4.Enabled == true)
                {
                    save_name = "timer4";
                    timer4.Enabled = false;
                }
                else if (timer5.Enabled == true)
                {
                    save_name = "timer5";
                    timer5.Enabled = false;
                }
                else if (timer6.Enabled == true)
                {
                    save_name = "timer6";
                    timer6.Enabled = false;
                }
                else if (timer7.Enabled == true)
                {
                    save_name = "timer7";
                    timer7.Enabled = false;
                }
                else if (timer8.Enabled == true)
                {
                    save_name = "timer8";
                    timer8.Enabled = false;
                }
                else
                {
                }
            }
            else if (countsave == 2)
            {
                timer1.Enabled = save_timer;
                switch (save_name)
                {
                    case "timer2":
                        timer2.Enabled = true;
                        break;
                    case "timer3":
                        timer3.Enabled = true;
                        break;
                    case "timer4":
                        timer4.Enabled = true;
                        break;
                    case "timer5":
                        timer5.Enabled = true;
                        break;
                    case "timer6":
                        timer6.Enabled = true;
                        break;
                    case "timer7":
                        timer7.Enabled = true;
                        break;
                    case "timer8":
                        timer8.Enabled = true;
                        break;
                    default:
                        break;
                }
                countsave = 0;
            }
        }

        //隐藏的textBox事件
        private void TimerTextBox_Leave(object sender, EventArgs e)
        {
            //try
            //{
            //    if (TimerTextBox.Text != "" && (Convert.ToInt32(TimerTextBox.Text) >= 50))
            //    {
            //        timer2.Interval = Convert.ToInt32(TimerTextBox.Text);
            //        timer3.Interval = Convert.ToInt32(TimerTextBox.Text);
            //        timer4.Interval = Convert.ToInt32(TimerTextBox.Text);
            //        timer5.Interval = Convert.ToInt32(TimerTextBox.Text);
            //        timer6.Interval = Convert.ToInt32(TimerTextBox.Text);
            //        timer7.Interval = Convert.ToInt32(TimerTextBox.Text);
            //        timer8.Interval = Convert.ToInt32(TimerTextBox.Text);
            //    }
            //    else
            //    {
            //        TimerTextBox.Text = "50";
            //        timer2.Interval = 47;
            //        timer3.Interval = 47;
            //        timer4.Interval = 47;
            //        timer5.Interval = 47;
            //        timer6.Interval = 47;
            //        timer7.Interval = 47;
            //        timer8.Interval = 47;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    TimerTextBox.Text = "50";
            //    MessageBox.Show(ex.Message);
            //}
        }

        //隐藏的textBox事件
        private void TimerTextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (TimerTextBox.Text != "" && (Convert.ToInt32(TimerTextBox.Text) >= 50))
                {
                    timer2.Interval = Convert.ToInt32(TimerTextBox.Text);
                    timer3.Interval = Convert.ToInt32(TimerTextBox.Text);
                    timer4.Interval = Convert.ToInt32(TimerTextBox.Text);
                    timer5.Interval = Convert.ToInt32(TimerTextBox.Text);
                    timer6.Interval = Convert.ToInt32(TimerTextBox.Text);
                    timer7.Interval = Convert.ToInt32(TimerTextBox.Text);
                    timer8.Interval = Convert.ToInt32(TimerTextBox.Text);
                }
                else
                {
                    TimerTextBox.Text = "50";
                    timer2.Interval = 47;
                    timer3.Interval = 47;
                    timer4.Interval = 47;
                    timer5.Interval = 47;
                    timer6.Interval = 47;
                    timer7.Interval = 47;
                    timer8.Interval = 47;
                }
            }
            catch (Exception ex)
            {
                TimerTextBox.Text = "50";
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region textBox的文字更改事件
        //左转的textchanged事件
        private void textBoxNumEx2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                textBoxNumEx4.Text = textBoxNumEx2.Text;
                if (textBoxNumEx2.Text.Trim() != ""
                    && textBoxNumEx4.Text.Trim() != ""
                    && textBoxNumEx6.Text.Trim() != ""
                    && Convert.ToDouble(textBoxNumEx2.Text.Trim()) != 0
                    && Convert.ToDouble(textBoxNumEx4.Text.Trim()) != 0
                    && Convert.ToDouble(textBoxNumEx6.Text.Trim()) != 0)
                {
                    textBoxNumEx8.Text = (Convert.ToDouble(textBoxNumEx2.Text.Trim()) + Convert.ToDouble(textBoxNumEx4.Text.Trim()) + Convert.ToDouble(textBoxNumEx6.Text.Trim()) * 2).ToString();
                }
                else
                {
                    //MessageBox.Show("请正确填写参数！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                textBoxNumEx2.Text = "";
                textBoxNumEx2.Focus();
            }
        }

        //右转的textchanged事件
        private void textBoxNumEx4_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (   textBoxNumEx2.Text.Trim() != "" 
                    && textBoxNumEx4.Text.Trim() != ""
                    && textBoxNumEx6.Text.Trim() != ""
                    && Convert.ToDouble(textBoxNumEx2.Text.Trim()) != 0
                    && Convert.ToDouble(textBoxNumEx4.Text.Trim()) != 0
                    && Convert.ToDouble(textBoxNumEx6.Text.Trim()) != 0)
                {
                    textBoxNumEx8.Text = (Convert.ToDouble(textBoxNumEx2.Text.Trim()) + Convert.ToDouble(textBoxNumEx4.Text.Trim()) + Convert.ToDouble(textBoxNumEx6.Text.Trim()) * 2).ToString();
                }
                else
                {
                    //MessageBox.Show("请正确填写参数！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                textBoxNumEx4.Text = "";
                textBoxNumEx4.Focus();
            }
        }

        //停止的textchanged事件
        private void textBoxNumEx6_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (   textBoxNumEx2.Text.Trim() != "" 
                    && textBoxNumEx4.Text.Trim() != "" 
                    && textBoxNumEx6.Text.Trim() != "" 
                    && Convert.ToDouble(textBoxNumEx2.Text.Trim()) != 0 
                    && Convert.ToDouble(textBoxNumEx4.Text.Trim()) != 0 
                    && Convert.ToDouble(textBoxNumEx6.Text.Trim()) != 0)
                {
                    textBoxNumEx8.Text = (Convert.ToDouble(textBoxNumEx2.Text.Trim()) + Convert.ToDouble(textBoxNumEx4.Text.Trim()) + Convert.ToDouble(textBoxNumEx6.Text.Trim()) * 2).ToString();
                }
                else
                {
                    //MessageBox.Show("请正确填写参数！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                textBoxNumEx6.Text = "";
                textBoxNumEx6.Focus();
            }
        }

        //洗涤单周期的textchanged事件
        private void textBoxNumEx8_TextChanged(object sender, EventArgs e)
        {
            if (textBoxNumEx1.Text.Trim() != "" && textBoxNumEx8.Text.Trim() != "" && Convert.ToDouble(textBoxNumEx1.Text.Trim()) != 0 && Convert.ToDouble(textBoxNumEx8.Text.Trim()) != 0)
            {
                textBoxNumEx7.Text = Convert.ToInt32(Convert.ToDouble(textBoxNumEx1.Text.Trim()) / Convert.ToDouble(textBoxNumEx8.Text.Trim())).ToString();
            }
            else
            {
                MessageBox.Show("请正确填写时间参数！");
            }

        }

        //洗涤时间的textchanged事件
        private void textBoxNumEx1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (textBoxNumEx1.Text.Trim() != "" && textBoxNumEx8.Text.Trim() != "" && Convert.ToDouble(textBoxNumEx1.Text.Trim()) != 0 && Convert.ToDouble(textBoxNumEx8.Text.Trim()) != 0)
                {
                    //计算总周期
                    textBoxNumEx7.Text = Convert.ToUInt64(Convert.ToUInt64(textBoxNumEx1.Text.Trim()) / Convert.ToDouble(textBoxNumEx8.Text.Trim())).ToString();
                }
                //计算总时间
                if (textBoxNumEx1.Text.Trim() != "" && textBoxNumEx9.Text.Trim() != "")
                {
                    if (checkBox1.Checked == true && checkBox2.Checked == true)
                    {
                        //textBoxNumEx18.Text = ((Convert.ToUInt64(textBoxNumEx1.Text.Trim()) + Convert.ToUInt64(textBoxNumEx9.Text.Trim())) / 60 / 60).ToString("D2") + ":" + ((Convert.ToUInt64(textBoxNumEx1.Text.Trim()) + Convert.ToUInt64(textBoxNumEx9.Text.Trim())) / 60).ToString("D2") + ":" + ((Convert.ToUInt64(textBoxNumEx1.Text.Trim()) + Convert.ToUInt64(textBoxNumEx9.Text.Trim())) % 60).ToString("D2");
                        textBoxNumEx18.Text = ((Convert.ToUInt64(textBoxNumEx1.Text.Trim()) + Convert.ToUInt64(textBoxNumEx9.Text.Trim()) + myintermission) / 60 / 60).ToString("D2") + ":" + ((Convert.ToUInt64(textBoxNumEx1.Text.Trim()) + Convert.ToUInt64(textBoxNumEx9.Text.Trim()) + myintermission) / 60 % 60).ToString("D2") + ":" + ((Convert.ToUInt64(textBoxNumEx1.Text.Trim()) + Convert.ToUInt64(textBoxNumEx9.Text.Trim()) + myintermission) % 60).ToString("D2");
                        summer = Convert.ToUInt64(textBoxNumEx1.Text.Trim()) + Convert.ToUInt64(textBoxNumEx9.Text.Trim()) + myintermission;
                    }
                    else if (checkBox1.Checked == true && checkBox2.Checked == false)
                    {
                        textBoxNumEx18.Text = (Convert.ToUInt64(textBoxNumEx1.Text.Trim()) / 60 / 60).ToString("D2") + ":" + (Convert.ToUInt64(textBoxNumEx1.Text.Trim()) / 60 % 60).ToString("D2") + ":" + (Convert.ToUInt64(textBoxNumEx1.Text.Trim()) % 60).ToString("D2");
                        summer = Convert.ToUInt64(textBoxNumEx1.Text.Trim());
                    }
                    else if (checkBox1.Checked == false && checkBox2.Checked == true)
                    {
                        textBoxNumEx18.Text = (Convert.ToUInt64(textBoxNumEx9.Text.Trim()) / 60 / 60).ToString("D2") + ":" + (Convert.ToUInt64(textBoxNumEx9.Text.Trim()) / 60 % 60).ToString("D2") + ":" + (Convert.ToUInt64(textBoxNumEx9.Text.Trim()) % 60).ToString("D2");
                        summer = Convert.ToUInt64(textBoxNumEx9.Text.Trim());
                    }
                    else if (checkBox1.Checked == false && checkBox2.Checked == false)
                    {
                        textBoxNumEx18.Text = "";
                    }
                }
                else if (textBoxNumEx1.Text.Trim() != "" && textBoxNumEx9.Text.Trim() == "")
                {
                    if (checkBox1.Checked == true)
                    {
                        textBoxNumEx18.Text = (Convert.ToUInt64(textBoxNumEx1.Text.Trim()) / 60 / 60).ToString("D2") + ":" + (Convert.ToUInt64(textBoxNumEx1.Text.Trim()) / 60 % 60).ToString("D2") + ":" + (Convert.ToUInt64(textBoxNumEx1.Text.Trim()) % 60).ToString("D2");
                        summer = Convert.ToUInt64(textBoxNumEx1.Text.Trim());
                    }
                    else
                    {
                        textBoxNumEx18.Text = "";
                    }
                }
                else if (textBoxNumEx1.Text.Trim() == "" && textBoxNumEx9.Text.Trim() != "")
                {
                    if (checkBox2.Checked == true)
                    {
                        textBoxNumEx18.Text = (Convert.ToUInt64(textBoxNumEx9.Text.Trim()) / 60 / 60).ToString("D2") + ":" + (Convert.ToUInt64(textBoxNumEx9.Text.Trim()) / 60 % 60).ToString("D2") + ":" + (Convert.ToUInt64(textBoxNumEx9.Text.Trim()) % 60).ToString("D2");
                        summer = Convert.ToUInt64(textBoxNumEx9.Text.Trim());
                    }
                    else
                    {
                        textBoxNumEx18.Text = "";
                    }
                }
                else if (textBoxNumEx1.Text.Trim() == "" && textBoxNumEx9.Text.Trim() == "")
                {
                    textBoxNumEx18.Text = "";
                    if ((checkBox1.Checked == true && checkBox2.Checked == false) || (checkBox1.Checked == true && checkBox2.Checked == true))
                    {
                        textBoxNumEx1.Focus();
                    }
                    else if (checkBox1.Checked == false && checkBox2.Checked == true)
                    {
                        textBoxNumEx9.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                textBoxNumEx1.Text = "";
                textBoxNumEx1.Focus();
            }
        }

        //脱水时间的textchanged事件
        private void textBoxNumEx9_TextChanged(object sender, EventArgs e)
        {
            try
            {
                //计算总时间
                if (textBoxNumEx1.Text.Trim() != "" && textBoxNumEx9.Text.Trim() != "")
                {
                    if (checkBox1.Checked == true && checkBox2.Checked == true)
                    {
                        //textBoxNumEx18.Text = ((Convert.ToUInt64(textBoxNumEx1.Text.Trim()) + Convert.ToUInt64(textBoxNumEx9.Text.Trim())) / 60 / 60).ToString("D2") + ":" + ((Convert.ToUInt64(textBoxNumEx1.Text.Trim()) + Convert.ToUInt64(textBoxNumEx9.Text.Trim())) / 60).ToString("D2") + ":" + ((Convert.ToUInt64(textBoxNumEx1.Text.Trim()) + Convert.ToUInt64(textBoxNumEx9.Text.Trim())) % 60).ToString("D2");
                        textBoxNumEx18.Text = ((Convert.ToUInt64(textBoxNumEx1.Text.Trim()) + Convert.ToUInt64(textBoxNumEx9.Text.Trim()) + myintermission) / 60 / 60).ToString("D2") + ":" + ((Convert.ToUInt64(textBoxNumEx1.Text.Trim()) + Convert.ToUInt64(textBoxNumEx9.Text.Trim()) + myintermission) / 60 % 60).ToString("D2") + ":" + ((Convert.ToUInt64(textBoxNumEx1.Text.Trim()) + Convert.ToUInt64(textBoxNumEx9.Text.Trim()) + myintermission) % 60).ToString("D2");
                        summer = Convert.ToUInt64(textBoxNumEx1.Text.Trim()) + Convert.ToUInt64(textBoxNumEx9.Text.Trim()) + myintermission;
                    }
                    else if (checkBox1.Checked == true && checkBox2.Checked == false)
                    {
                        textBoxNumEx18.Text = (Convert.ToUInt64(textBoxNumEx1.Text.Trim()) / 60 / 60).ToString("D2") + ":" + (Convert.ToUInt64(textBoxNumEx1.Text.Trim()) / 60 % 60).ToString("D2") + ":" + (Convert.ToUInt64(textBoxNumEx1.Text.Trim()) % 60).ToString("D2");
                        summer = Convert.ToUInt64(textBoxNumEx1.Text.Trim());
                    }
                    else if (checkBox1.Checked == false && checkBox2.Checked == true)
                    {
                        textBoxNumEx18.Text = (Convert.ToUInt64(textBoxNumEx9.Text.Trim()) / 60 / 60).ToString("D2") + ":" + (Convert.ToUInt64(textBoxNumEx9.Text.Trim()) / 60 % 60).ToString("D2") + ":" + (Convert.ToUInt64(textBoxNumEx9.Text.Trim()) % 60).ToString("D2");
                        summer = Convert.ToUInt64(textBoxNumEx9.Text.Trim());
                    }
                    else if (checkBox1.Checked == false && checkBox2.Checked == false)
                    {
                        textBoxNumEx18.Text = "";
                    }
                }
                else if (textBoxNumEx1.Text.Trim() != "" && textBoxNumEx9.Text.Trim() == "")
                {
                    if (checkBox1.Checked == true)
                    {
                        textBoxNumEx18.Text = (Convert.ToUInt64(textBoxNumEx1.Text.Trim()) / 60 / 60).ToString("D2") + ":" + (Convert.ToUInt64(textBoxNumEx1.Text.Trim()) / 60 % 60).ToString("D2") + ":" + (Convert.ToUInt64(textBoxNumEx1.Text.Trim()) % 60).ToString("D2");
                        summer = Convert.ToUInt64(textBoxNumEx1.Text.Trim());
                    }
                    else
                    {
                        textBoxNumEx18.Text = "";
                    }
                }
                else if (textBoxNumEx1.Text.Trim() == "" && textBoxNumEx9.Text.Trim() != "")
                {
                    if (checkBox2.Checked == true)
                    {
                        textBoxNumEx18.Text = (Convert.ToUInt64(textBoxNumEx9.Text.Trim()) / 60 / 60).ToString("D2") + ":" + (Convert.ToUInt64(textBoxNumEx9.Text.Trim()) / 60 % 60).ToString("D2") + ":" + (Convert.ToUInt64(textBoxNumEx9.Text.Trim()) % 60).ToString("D2");
                        summer = Convert.ToUInt64(textBoxNumEx9.Text.Trim());
                    }
                    else
                    {
                        textBoxNumEx18.Text = "";
                    }
                }
                else if (textBoxNumEx1.Text.Trim() == "" && textBoxNumEx9.Text.Trim() == "")
                {
                    textBoxNumEx18.Text = "";
                    if ((checkBox1.Checked == false && checkBox2.Checked == true) || (checkBox1.Checked == true && checkBox2.Checked == true))
                    {
                        textBoxNumEx9.Focus();
                    }
                    else if (checkBox1.Checked == true && checkBox2.Checked == false)
                    {
                        textBoxNumEx1.Focus();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                textBoxNumEx9.Text = "";
                textBoxNumEx9.Focus();
            }
        }

        //洗涤速度的快速设置
        private void textBoxNumEx3_TextChanged(object sender, EventArgs e)
        {
            textBoxNumEx5.Text = textBoxNumEx3.Text;
        }

        //洗涤加速度的快速设置
        private void textBoxNumEx19_TextChanged(object sender, EventArgs e)
        {
            textBoxNumEx20.Text = textBoxNumEx19.Text;
        }

        //脱水时间的快速设置
        private void textBoxNumEx10_TextChanged(object sender, EventArgs e)
        {
            textBoxNumEx12.Text = textBoxNumEx10.Text;
            textBoxNumEx14.Text = textBoxNumEx10.Text;
            textBoxNumEx16.Text = textBoxNumEx10.Text;
        }

        //脱水速度的快速设置
        private void textBoxNumEx11_TextChanged(object sender, EventArgs e)
        {
            textBoxNumEx13.Text = textBoxNumEx11.Text;
            textBoxNumEx15.Text = textBoxNumEx11.Text;
            textBoxNumEx17.Text = textBoxNumEx11.Text;
        }

        //脱水加速度的快速设置
        private void textBoxNumEx21_TextChanged(object sender, EventArgs e)
        {
            textBoxNumEx22.Text = textBoxNumEx21.Text;
            textBoxNumEx23.Text = textBoxNumEx21.Text;
            textBoxNumEx24.Text = textBoxNumEx21.Text;
        }

        //间隔时间
        private void textBoxNumEx29_TextChanged(object sender, EventArgs e)
        {
            if (textBoxNumEx29.Text != "" && Convert.ToUInt64(textBoxNumEx29.Text) != 0)
            {
                myintermission = Convert.ToUInt64(textBoxNumEx29.Text);
                if (checkBox1.Checked == true && checkBox2.Checked == true)
                {
                    textBoxNumEx18.Text = ((Convert.ToUInt64(textBoxNumEx1.Text.Trim()) + Convert.ToUInt64(textBoxNumEx9.Text.Trim()) + myintermission) / 60 / 60).ToString("D2") + ":" + ((Convert.ToUInt64(textBoxNumEx1.Text.Trim()) + Convert.ToUInt64(textBoxNumEx9.Text.Trim()) + myintermission) / 60 % 60).ToString("D2") + ":" + ((Convert.ToUInt64(textBoxNumEx1.Text.Trim()) + Convert.ToUInt64(textBoxNumEx9.Text.Trim()) + myintermission) % 60).ToString("D2");
                    summer = Convert.ToUInt64(textBoxNumEx1.Text.Trim()) + Convert.ToUInt64(textBoxNumEx9.Text.Trim()) + myintermission;
                }
            }
            else if (textBoxNumEx29.Text == "" || Convert.ToUInt64(textBoxNumEx29.Text) == 0)
            {
                textBoxNumEx29.Text = "10";
                myintermission = Convert.ToUInt64(textBoxNumEx29.Text);
                if (checkBox1.Checked == true && checkBox2.Checked == true)
                {
                    textBoxNumEx18.Text = ((Convert.ToUInt64(textBoxNumEx1.Text.Trim()) + Convert.ToUInt64(textBoxNumEx9.Text.Trim()) + myintermission) / 60 / 60).ToString("D2") + ":" + ((Convert.ToUInt64(textBoxNumEx1.Text.Trim()) + Convert.ToUInt64(textBoxNumEx9.Text.Trim()) + myintermission) / 60 % 60).ToString("D2") + ":" + ((Convert.ToUInt64(textBoxNumEx1.Text.Trim()) + Convert.ToUInt64(textBoxNumEx9.Text.Trim()) + myintermission) % 60).ToString("D2");
                    summer = Convert.ToUInt64(textBoxNumEx1.Text.Trim()) + Convert.ToUInt64(textBoxNumEx9.Text.Trim()) + myintermission;
                }
            }

        }
        //重写窗体事件，实现当检测到串口异常，停止发送提示错误
        protected override void WndProc(ref Message m)
        {

            if (m.Msg == 0x0219)
            {
                //串口被拔出 
                if (m.WParam.ToInt32() == 0x8004)
                {
                    //MessageBox.Show("串口断开！系统将关闭！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    timer8.Enabled = false;
                    timer7.Enabled = false;
                    timer6.Enabled = false;
                    timer5.Enabled = false;
                    timer4.Enabled = false;
                    timer3.Enabled = false;
                    timer2.Enabled = false;
                    timer1.Enabled = false;
                    count1 = 0;
                    count2 = 0;
                    count3 = 0;
                    count4 = 0;
                    count5 = 0;
                    count6 = 0;
                    count7 = 0;
                    count8 = 0;
                    //countstart = 0;
                    washcount = 0;
                    spincount = 0;
                    toolStripMenuItem11.Enabled = true;
                    toolStripMenuItem12.Enabled = true;
                    toolStripMenuItem13.Enabled = false;
                    if (Form1.languageflag == (int)Form1.Language.English)
                    {
                        toolStripButton1.Text = "START";
                    }
                    else if (Form1.languageflag == (int)Form1.Language.Chinese)
                    {
                        toolStripButton1.Text = "开始运行";
                    }
                    toolStripButton1.Enabled = false;
                    if (serialPort1.IsOpen)
                    {
                        timer10.Enabled = true;
                    }
                    else
                    {
                        toolStripButton1.Enabled = true;
                    }
                }
            }
            base.WndProc(ref m);
        }
        #endregion

        #region 语言选择

        public void Debug2_Language_Selected(int mylanguage)
        {
            switch (mylanguage)
            {
                case (int)Form1.Language.English:
                    if (this.toolStripButton1.Text == "开始运行")
                    {
                        this.toolStripButton1.Text = "START";
                    }
                    else if (this.toolStripButton1.Text == "停止运行")
                    {
                        this.toolStripButton1.Text = "STOP";
                    }
                    this.groupBox1.Text = "Display Area";
                    this.label11.Text = "TotalWashCycle";
                    this.label15.Text = "Actual CMD";
                    this.label4.Text = "Target Speed";
                    this.label22.Text = "Total Time";
                    this.label3.Text = "Mode";
                    this.label2.Text = "Actual WashCycle";
                    this.label1.Text = "Running Time";
                    this.label12.Text = "Cycle Time";
                    this.groupBox2.Text = "SET";
                    this.checkBox3.Text = "Repeat";
                    this.label23.Text = "Time units：second";
                    this.checkBox2.Text = "SPIN";
                    this.checkBox1.Text = "WASH";
                    this.groupBox4.Text = "SPIN Configuration";
                    this.label31.Text = "Accel";
                    this.label27.Text = "Accel";
                    this.label30.Text = "Accel";
                    this.label29.Text = "Accel";
                    this.label25.Text = "Speed";
                    this.label20.Text = "Speed";
                    this.label13.Text = "SPIN Time";
                    this.label14.Text = "Low Time";
                    this.label16.Text = "Mid Time";
                    this.label17.Text = "High Time";
                    this.label18.Text = "Speed";
                    this.label19.Text = "Speed";
                    this.label21.Text = "Beyond Time";
                    this.groupBox3.Text = "WASH Configuration";
                    this.label32.Text = "Accel";
                    this.label33.Text = "Speed";
                    this.label26.Text = "Accel";
                    this.label28.Text = "Accel";
                    this.label5.Text = "WASH Time";
                    this.label6.Text = "CW Time";
                    this.label7.Text = "CCW Time";
                    this.label8.Text = "Stop Time";
                    this.label9.Text = "Speed";
                    this.label10.Text = "Speed";
                    this.button2.Text = "Change";
                    this.button1.Text = "Build";
                    this.label24.Text = "Repeat Interval";
                    this.toolStripDropDownButton1.Text = "&SET";
                    this.toolStripMenuItem11.Text = "&Open Structure";
                    this.toolStripMenuItem12.Text = "Close Structure";
                    this.toolStripMenuItem13.Text = "&Pending";
                    this.toolStripMenuItem14.Text = "To Top(&T)";
                    this.toolStripLabel1.Text = "CMD Interval(ms)";
                    break;
                case (int)Form1.Language.Chinese:
                    if (this.toolStripButton1.Text == "START")
                    {
                        this.toolStripButton1.Text = "开始运行";
                    }
                    else if (this.toolStripButton1.Text == "STOP")
                    {
                        this.toolStripButton1.Text = "停止运行";
                    }
                    this.groupBox1.Text = "显示区";
                    this.label11.Text = "洗涤总周期";
                    this.label15.Text = "当前发送指令";
                    this.label4.Text = "目标速度";
                    this.label22.Text = "总时间";
                    this.label3.Text = "模式";
                    this.label2.Text = "当前洗涤周期";
                    this.label1.Text = "已测时间";
                    this.label12.Text = "周期时间";
                    this.groupBox2.Text = "设置";
                    this.checkBox3.Text = "循环";
                    this.label23.Text = "时间单位：秒";
                    this.checkBox2.Text = "脱水";
                    this.checkBox1.Text = "洗涤";
                    this.groupBox4.Text = "脱水设置";
                    this.label31.Text = "加速度";
                    this.label27.Text = "加速度";
                    this.label30.Text = "加速度";
                    this.label29.Text = "加速度";
                    this.label25.Text = "速度";
                    this.label20.Text = "速度";
                    this.label13.Text = "脱水时间";
                    this.label14.Text = "低速时间";
                    this.label16.Text = "中速时间";
                    this.label17.Text = "高速时间";
                    this.label18.Text = "速度";
                    this.label19.Text = "速度";
                    this.label21.Text = "极限时间";
                    this.groupBox3.Text = "洗涤设置";
                    this.label32.Text = "加速度";
                    this.label33.Text = "速度";
                    this.label26.Text = "加速度";
                    this.label28.Text = "加速度";
                    this.label5.Text = "洗涤时间";
                    this.label6.Text = "左转时间";
                    this.label7.Text = "右转时间";
                    this.label8.Text = "停止时间";
                    this.label9.Text = "速度";
                    this.label10.Text = "速度";
                    this.button2.Text = "修改";
                    this.button1.Text = "生成";
                    this.label24.Text = "间隔时间";
                    this.toolStripDropDownButton1.Text = "配置(&S)";
                    this.toolStripMenuItem11.Text = "打开配置(&O)";
                    this.toolStripMenuItem12.Text = "关闭配置";
                    this.toolStripMenuItem13.Text = "挂起(&P)";
                    this.toolStripMenuItem14.Text = "置顶(&T)"; 
                    this.toolStripLabel1.Text = "指令间隔时间(ms)";
                    break;
                default:
                    break;
            }
        }

        #endregion

    }
}