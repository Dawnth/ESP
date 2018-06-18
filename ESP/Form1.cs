using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.IO.Ports;
using System.Collections;
using ESP.Properties;

namespace ESP
{
    public partial class Form1 : Form
    {
        #region [参数设置-------------------------------------------------------]

        int countD = 0;//用于接受指令的计数
        //int countO = 0;//串口Open次数的记录，奇数可以打开，偶数可以关闭串口
        int countS = 0;//用于保存成功信息的计时
        int first = 0;
        int reopen = 0;//用于reopenfiles[]的下标
        int spcount = 0;//串口加载参数的限制，只有为1时加载参数，在打开串口配置窗体时清零
        long tblength = 0;//textBox字符个数
        int ton = 0;//窗体置顶按钮的次数记录，奇数为置顶，偶数为取消置顶
        private XmlDocument xmlDoc = null;//XML文档声明
        public int countC = 0;//用于发送指令的计数
        public static string[] spconfig = new string[9]{"COM4","9600","None","8","One","1000","1000","4096","2048"};//串口各属性的参数
        int spi = 0;//用于spconfig[]的下标
        //public static string[] protocol = new string[10];//用于保存通讯协议信息
        //int pri = 0;//用于protocol[]的下标
        string[] reopenfiles = new string[5] {"","","","","" };//用于加载打开过的文件的路径
        string adress = null;//XML文档路径声明
        string newpath = null;//保存记录的路径
        string name = null;//选中节点的名称
        string path = null;//当前程序路径

        string[] str = new string[20];//用来存储listView中的值
        string[] xishu = new string[20];//用来存储Parameter中对应的系数
        string[] small = new string[20];//Parameter中是否为小端序
        int mycountin = 0;//build时用来存储str[]的下标
        int mycountout = 0;//build时用来提取str[]的下标
        string start = null;//用来存储开始字节
        string stop = null;//用来存储结束字节
        string checkstyle = null;//用来存储checksum的形式是加和还是求补码
        string commandid = null;//用来存储Command中的commandid的值
        string length = null;//用来存储指令长短
        int check = 0;//用来计算checksum的中间变量计算形式
        string checksum = null;//将check得到的值转化为string显示在textbox中
        string cmi = null;//command index的位置
        string li = null;//length index的位置
        string pi = null;//params的位置
        string csi = null;//checksum的位置
        string islist = null;//是否显示在listView中
        string islength = null;//是否加上length
        string ishex = null;//是否是十六进制
        string ci = null;//commandid位置的引索
        string[] dataindex = new string[20];//dataindex是接收数据位置的索引值
        int mycountindex = 0;//dataindex[]的下标
        int countreceive = 0;//记录是否是第一次添加subItem
        string[] rxishu = new string[20];//用来存储Response中对应的系数
        string[] rsmall = new string[20];//Response中是否为小端序
        string compare = null;//比较方法：1为比较commandid；0为不比较

        //checksum校验和
        //配置文档打算写成      <Response CChecksum="1" CStart=""  CLength="">
        //string cchecksum = null;//是否检查（check）checksum的标志
        //string cstartindex = null;//检查checksum的开始字节位置
        //string clength = null;//检查checksum的字节长度
        string linshistring = null;//用于存放临时字符串的全局变量
        string recall = null;//用来处理的返回字符串
        string restop = null;//用来处理的返回字符串
        string relength = null;//用来处理的返回字符串
        bool _isReceiving = false;//接收标志位，表示串口正在接收数据
        int maxlength = 0;//数据的最大长度
        //CRC 相关参数
        byte CRC8;//最后要生成的CRC校验
        ushort CRC16;//最后要生成的CRC校验
        ushort CRCType = 0;//CRC类型 1 为 Back16CRC，0 为 Front16CRC
        byte Back8CRC = 255;//0xFFFF
        byte Front8CRC = 0;//0x0000
        ushort Back16CRC = 65535;//0xFFFF
        ushort Front16CRC = 0;//0x0000
        string TempCRCTable = null;//从xml文档中加载的CRC表
        byte[] CRC8LookupTable = new byte[256];
        ushort[] CRC16LookupTable = new ushort[256];

        //存储更改过的变量
        public struct mystruct
        {
            public string commandname;
            public string itemname;
            public string itemvalue;
        };
        mystruct[] mystore = new mystruct[200];
        int mystorecounter = 0;//用于数组下标
        int mystoreindex = 0;//用于存储加载下标

        //对象
        Time time = new Time();
        Debug1 debug1 = new Debug1();
        Debug2 debug2 = new Debug2();
        Demo1 demo1 = new Demo1();
        Demo2 demo2 = new Demo2();
        CheckSum CKS = new CheckSum();
        OSC osc = new OSC();

        //更新接收和发送窗口（textBox2）的标志位
        public static string myinflag = "0";
        public static string myoutflag ="0";

        //语言选择标记
        public static int languageflag = 1;
        public enum Language:int
        {
            English = 1,
            Chinese = 2,
        };

        #endregion

        #region [初始化窗体及组件，并在窗体的Load事件中加载参数-----------------]

        //初始化窗口组件
        public Form1()
        {
            //初始化窗体内部组件
            InitializeComponent();
            //
            notifyIcon1.Visible = false;
            //关闭配置按钮Disable
            toolStripMenuItem13.Enabled = false;
            //历史菜单默认隐藏
            toolStripSeparator7.Visible = false;
            toolStripMenuItem126.Enabled = false;
            //隐藏Demo Mode下拉菜单
            toolStripDropDownButton4.Visible = false;
            //Debug模式下AC按钮隐藏
            //toolStripMenuItem31.Visible = false;
            //数字示波器按钮隐藏
            toolStripMenuItem24.Visible = false;
            toolStripSeparator12.Visible = false;
            //帮助部分按钮隐藏
            toolStripMenuItem52.Visible = false;
            toolStripSeparator11.Visible = false;
            toolStripMenuItem53.Visible = false;
        }

        //打开Form1时触发的事件
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                //串口接收如果不用委托，则需要设置false，目前使用委托，所以注释
                //System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;

                //如果有配置 则读取配置文件
                if (File.Exists(Application.StartupPath + @"\ESPConfig.cds"))
                {
                    //读入XML文档
                    XmlDocument cdsxml = new XmlDocument();
                    cdsxml.Load(Application.StartupPath + @"\ESPConfig.cds");
                    XmlNode cdslanguage = cdsxml.SelectSingleNode("Config/Language");
                    //判断是否为含有语言标记的配置文档，如果没有，重新生成配置文档
                    if (cdslanguage != null)
                    {
                        setSelectedLanguage(Convert.ToInt32(cdslanguage.InnerText));
                        //将串口配置加载到数组中
                        XmlNode cdsxspconfig = cdsxml.SelectSingleNode("Config/SerialPortConfig");
                        if (cdsxspconfig.HasChildNodes)
                        {
                            foreach (XmlNode xmlNode in cdsxspconfig.ChildNodes)
                            {
                                spconfig[spi] = xmlNode.InnerText;
                                spi++;
                            }
                        }
                        //读取打开文件历史路径中的最新打开的结构文档
                        XmlNode cdsesp = cdsxml.SelectSingleNode("Config/MRU/Item_0");
                        if (cdsesp.InnerText != "")
                        {
                            this.LoadTreeNodes(cdsesp.InnerText);//加载xml到treeView中
                        }
                        //将打开文件历史路径加载到reopenfiles中
                        XmlNode cdsmru = cdsxml.SelectSingleNode("Config/MRU");
                        if (cdsmru.HasChildNodes)
                        {
                            foreach (XmlNode reopenNode in cdsmru.ChildNodes)
                            {
                                reopenfiles[reopen] = reopenNode.InnerText;
                                reopen++;
                            }
                        }
                    }
                    else
                    {
                        buildConfigurationFile();
                    }
                }
                else if (!File.Exists(Application.StartupPath + @"\ESPConfig.cds"))
                {
                    buildConfigurationFile();
                }
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message);
                buildConfigurationFile();
            }
            //底部toolStripStatusLabel5显示当前日期/星期/时间
            this.toolStripStatusLabel5.Text = DateTime.Now.ToString("yyyy-MM-dd dddd HH:mm:ss");
            //触发事件，Timer2开始工作一1s为周期，使时间刷新
            this.timer2.Start();
            //底部toolStripStatusLabel2的背景色为红色，表示串口关闭
            toolStripStatusLabel2.BackColor = Color.Red;
            getPortName();//设置串口的属性
            serialPort1.PortName = spconfig[0];
            serialPort1.BaudRate = Convert.ToInt32(spconfig[1]);
            if (spconfig[2] == "None")
            { serialPort1.Parity = Parity.None; }
            else if (spconfig[2] == "Odd")
            { serialPort1.Parity = Parity.Odd; }
            else if (spconfig[2] == "Even")
            { serialPort1.Parity = Parity.Even; }
            else if (spconfig[2] == "Mark")
            { serialPort1.Parity = Parity.Mark; }
            else if (spconfig[2] == "Space")
            { serialPort1.Parity = Parity.Space; }
            serialPort1.DataBits = Convert.ToInt32(spconfig[3]);
            if (spconfig[4] == "None")
            { serialPort1.StopBits = StopBits.None; }
            else if (spconfig[4] == "One")
            { serialPort1.StopBits = StopBits.One; }
            else if (spconfig[4] == "Two")
            { serialPort1.StopBits = StopBits.Two; }
            else if (spconfig[4] == "OnePointFive")
            { serialPort1.StopBits = StopBits.OnePointFive; }
            serialPort1.ReadTimeout = Convert.ToInt32(spconfig[5]);
            serialPort1.WriteTimeout = Convert.ToInt32(spconfig[6]);
            serialPort1.ReadBufferSize = Convert.ToInt32(spconfig[7]);
            serialPort1.WriteBufferSize = Convert.ToInt32(spconfig[8]);
            //底部toolStripStatusLabel1显示串口的状态
            //toolStripStatusLabel1.Text = spconfig[0] + " " + "Closed" + "," + spconfig[1] + "," + spconfig[2] + "," + spconfig[3] + "," + spconfig[4];
            toolStripStatusLabel1.Text = serialPort1.PortName + " " + "Closed" + "," + serialPort1.BaudRate + "," + serialPort1.Parity + "," + serialPort1.DataBits + "," + serialPort1.StopBits;
            //默认选中十六进制发送
            //checkBox1.Checked = true;
            //checkBox5.Checked = true;
            //在reopen中显示最近打开过的文档
            toolStripMenuItem121.Text = reopenfiles[0];
            toolStripMenuItem122.Text = reopenfiles[1];
            toolStripMenuItem123.Text = reopenfiles[2];
            toolStripMenuItem124.Text = reopenfiles[3];
            toolStripMenuItem125.Text = reopenfiles[4];
        }

        //生成配置文档
        private void buildConfigurationFile()
        {
            File.Delete(Application.StartupPath + @"\ESPConfig.cds");
            XmlDocument cdsxml = new XmlDocument();
            cdsxml.LoadXml(
                "<Config>" +
                "<Language>1</Language>" +
                "<SerialPortConfig>" +
                "<pn>COM4</pn>" +
                "<br>9600</br>" +
                "<py>None</py>" +
                "<db>8</db>" +
                "<sb>One</sb>" +
                "<rto>-1</rto>" +
                "<wto>-1</wto>" +
                "<rbs>4096</rbs>" +
                "<wbs>2048</wbs>" +
                "</SerialPortConfig>" +
                "<MRU>" +
                "<Item_0></Item_0>" +
                "<Item_1></Item_1>" +
                "<Item_2></Item_2>" +
                "<Item_3></Item_3>" +
                "<Item_4></Item_4>" +
                "</MRU>" +
                "<Lastdebug1>" +
                "</Lastdebug1>" +
                "<Lastdebug2>" +
                "</Lastdebug2>" +
                "<Lastdemo1>" +
                "</Lastdemo1>" +
                "<Lastdemo2>" +
                "</Lastdemo2>" +
                "</Config>");
            cdsxml.Save(Application.StartupPath + @"\ESPConfig.cds");
        }

        //得到当前活动串口的名字
        private void getPortName()
        {
            foreach (string s in SerialPort.GetPortNames())
            {
                spconfig[0] = s;
            }
        }

        #endregion

        #region [手动串口发送数据处理部分---------------------------------------]

        //手动数据的发送
        //表示循环发送的确定与否1为循环，0为不循环
        public int type1 = 0;
        //发送串口数据1
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //串口是否打开
                if (serialPort1.IsOpen)
                {
                    //清理输入缓冲区
                    serialPort1.DiscardInBuffer();
                    serialPort1.DiscardOutBuffer();
                    //是否为连续发送状态
                    if (button1.Text == "Send" || button1.Text == "发送")
                    {
                        //发送内容是否为空
                        if (textBox3.Text.Trim() != "")
                        {
                            //是否为连续发送
                            if (checkBox2.Checked == true)
                            {
                                //连续发送的时候，将无法更改连续发送的状态
                                //同时也无法更改数值条，将连续发送状态标志位type1置一
                                //更改timer1的时钟周期，然后使能timer1
                                checkBox2.Enabled = false;
                                numericUpDown1.Enabled = false;
                                groupBox4.Enabled = false;
                                toolStripEA1.Enabled = false;
                                type1 = 1;
                                TimerActionOne.timerActionOne(this, type1);
                                if (languageflag == (int)Language.English)
                                {
                                    button1.Text = "Stop";
                                }
                                else if (languageflag == (int)Language.Chinese)
                                {
                                    button1.Text = "停止";
                                }
                                timer1.Interval = Convert.ToInt32(numericUpDown1.Value);
                                timer1.Enabled = true;
                            }
                            else if (checkBox2.Checked == false)
                            {
                                //非连续发送时，将连续发送状态标志位清零
                                //调用串口发送函数时带着参数
                                //为防止变化，将文本值保持为Send
                                type1 = 0;
                                TimerActionOne.timerActionOne(this, type1);
                                if (languageflag == (int)Language.English)
                                {
                                    button1.Text = "Send";
                                }
                                else if (languageflag == (int)Language.Chinese)
                                {
                                    button1.Text = "发送";
                                }
                            }
                        }
                        else
                        {
                            textBox3.Focus();
                            MessageBox.Show("没有需要发送数据!");
                        }
                    }
                    else if (button1.Text == "Stop"||button1.Text == "停止")
                    {
                        groupBox4.Enabled = true;
                        toolStripEA1.Enabled = true;
                        timer1.Enabled = false;
                        if (languageflag == (int)Language.English)
                        {
                            button1.Text = "Send";
                        }
                        else if (languageflag == (int)Language.Chinese)
                        {
                            button1.Text = "发送";
                        }
                        checkBox2.Enabled = true;
                        numericUpDown1.Enabled = true;
                    }
                }
                else
                {
                    //打开串口
                    spopen();
                    //是否为连续发送状态
                    if (button1.Text == "Send" || button1.Text == "发送")
                    {
                        //发送内容是否为空
                        if (textBox3.Text.Trim() != "")
                        {
                            //是否为连续发送
                            if (checkBox2.Checked == true)
                            {
                                //连续发送的时候，将无法更改连续发送的状态
                                //同时也无法更改数值条，将连续发送状态标志位type1置一
                                //更改timer1的时钟周期，然后使能timer1
                                checkBox2.Enabled = false;
                                numericUpDown1.Enabled = false;
                                groupBox4.Enabled = false;
                                toolStripEA1.Enabled = false;
                                type1 = 1;
                                //timer1.Interval = Convert.ToInt32(numericUpDown1.Value);
                                timer1.Enabled = true;
                            }
                            else if (checkBox2.Checked == false)
                            {
                                //非连续发送时，将连续发送状态标志位清零
                                //调用串口发送函数时带着参数
                                //为防止变化，将文本值保持为Send
                                type1 = 0;
                                TimerActionOne.timerActionOne(this, type1);
                                if (languageflag == (int)Language.English)
                                {
                                    button1.Text = "Send";
                                }
                                else if (languageflag == (int)Language.Chinese)
                                {
                                    button1.Text = "发送";
                                }
                            }
                        }
                        else
                        {
                            textBox3.Focus();
                            MessageBox.Show("没有需要发送数据!");
                        }
                    }
                    else if (button1.Text == "Stop"|| button1.Text == "停止")
                    {
                        groupBox4.Enabled = true;
                        toolStripEA1.Enabled = true;
                        timer1.Enabled = false;
                        if (languageflag == (int)Language.English)
                        {
                            button1.Text = "Send";
                        }
                        else if (languageflag == (int)Language.Chinese)
                        {
                            button1.Text = "发送";
                        }
                        checkBox2.Enabled = true;
                        numericUpDown1.Enabled = true;
                    }
                }
            }
            catch (Exception)
            {
                //MessageBox.Show("程序错误:" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //TIMER1发送串口数据1
        private void timer1_Tick(object sender, EventArgs e)
        {
            TimerActionOne.timerActionOne(this, type1);
        }

        //连续发送的选择框1
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            { numericUpDown1.Enabled = true; }
            else
            { numericUpDown1.Enabled = false; }
        }

        //判断数据的长度来决定连续发送时间的最小值
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            int myvalue = 0;
            maxlength = textBox3.Text.Length;
            //test length is 20
            //12345678901234567890
            if (ishex == "true")
            {
                if (maxlength <= 30)
                {
                    myvalue = 50;
                }
                else if (maxlength > 30 && maxlength <= 40)
                {
                    myvalue = 80;
                }
                else
                {
                    myvalue = 500;
                }
            }
            else
            {
                if (serialPort1.BaudRate < 2400)
                {
                    myvalue = 500;
                }
                else if (serialPort1.BaudRate >= 2400 && serialPort1.BaudRate < 4800)
                {
                    if (maxlength <= 15)
                    {
                        myvalue = 50;
                    }
                    else if (maxlength > 15 && maxlength < 20)
                    {
                        myvalue = 70;
                    }
                    else if (maxlength >= 20 && maxlength < 24)
                    {
                        myvalue = 80;
                    }
                    else if (maxlength >= 24 && maxlength < 28)
                    {
                        myvalue = 100;
                    }
                    else if (maxlength >= 28 && maxlength < 32)
                    {
                        myvalue = 110;
                    }
                    else if (maxlength >= 32 && maxlength < 36)
                    {
                        myvalue = 130;
                    }
                    else if (maxlength >= 36 && maxlength < 40)
                    {
                        myvalue = 150;
                    }
                    else
                    {
                        myvalue = 500;
                    }
                }
                else if (serialPort1.BaudRate >= 4800 && serialPort1.BaudRate < 9600)
                {
                    if (maxlength <= 31)
                    {
                        myvalue = 50;
                    }
                    else if (maxlength > 31 && maxlength < 40)
                    {
                        myvalue = 70;
                    }
                    else
                    {
                        myvalue = 500;
                    }
                }
                else
                {
                    if (maxlength <= 40)
                    {
                        myvalue = 50;
                    }
                    else
                    {
                        myvalue = 500;
                    }
                }
            }
            if (myvalue >= Convert.ToInt32(numericUpDown1.Value))
            {
                numericUpDown1.Value = myvalue;
            }
        }

        //连续发送的时间值改变直接赋值给timer
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(numericUpDown2.Value) >= 50
                && Convert.ToInt32(numericUpDown2.Value) <= 65535
               )
            {
                timer1.Interval = Convert.ToInt32(numericUpDown1.Value);
            }
        }

        #endregion

        #region [自动串口发送数据处理部分---------------------------------------]

        //表示循环发送的确定与否1为循环，0为不循环
        public int type2 = 0;
        //发送串口数据2
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                //模仿build的动作
                //buildbutton();
                //判断串口是否打开
                if (serialPort1.IsOpen)
                {
                    //清理输入缓冲区
                    serialPort1.DiscardInBuffer();
                    serialPort1.DiscardOutBuffer();
                    //判断是否为未连续发送状态
                    if (button3.Text == "Send" || button3.Text == "发送")
                    {
                        //判断发送数据是否为空
                        if (textBox1.Text != "")
                        {
                            //设置标志为为1
                            myinflag = "1";
                            //判断是否连续发送
                            if (checkBox4.Checked == true)
                            {
                                //连续发送的时候，将无法更改连续发送的状态
                                //同时也无法更改数值条，将连续发送状态标志位type2置一
                                //更改timer3的时钟周期，然后使能timer3
                                checkBox4.Enabled = false;
                                numericUpDown2.Enabled = false;
                                type2 = 1;
                                TimerActionTwo.timerActionTwo(this, type2, textBox1.Text);
                                if (languageflag == (int)Language.English)
                                {
                                    button3.Text = "Stop";
                                }
                                else if (languageflag == (int)Language.Chinese)
                                {
                                    button3.Text = "停止";
                                }
                                timer3.Interval = Convert.ToInt32(numericUpDown2.Value);
                                timer3.Enabled = true;
                            }
                            else if (checkBox4.Checked == false)
                            {
                                //非连续发送时，将连续发送状态标志位清零
                                //调用串口发送函数时带着参数
                                //为防止变化，将文本值保持为Send
                                type2 = 0;
                                TimerActionTwo.timerActionTwo(this, type2, textBox1.Text); 
                                if (languageflag == (int)Language.English)
                                {
                                    button3.Text = "Send";
                                }
                                else if (languageflag == (int)Language.Chinese)
                                {
                                    button3.Text = "发送";
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("没有需要发送数据!");
                        }
                    }
                    else if (button3.Text == "Stop" || button3.Text == "停止")
                    {
                        groupBox6.Enabled = true;
                        toolStripEA1.Enabled = true;
                        timer3.Enabled = false; 
                        if (languageflag == (int)Language.English)
                        {
                            button3.Text = "Send";
                        }
                        else if (languageflag == (int)Language.Chinese)
                        {
                            button3.Text = "发送";
                        }
                        checkBox4.Enabled = true;
                        numericUpDown2.Enabled = true;
                    }
                }
                else
                {
                    //打开串口
                    spopen();
                    //判断是否为未连续发送状态
                    if (button3.Text == "Send" || button3.Text == "发送")
                    {
                        //判断发送数据是否为空
                        if (textBox1.Text != "")
                        {
                            //判断是否连续发送
                            if (checkBox4.Checked == true)
                            {
                                //连续发送的时候，将无法更改连续发送的状态
                                //同时也无法更改数值条，将连续发送状态标志位type2置一
                                //更改timer3的时钟周期，然后使能timer3
                                checkBox4.Enabled = false;
                                numericUpDown2.Enabled = false;
                                type2 = 1;
                                timer3.Interval = Convert.ToInt32(numericUpDown2.Value);
                                timer3.Enabled = true;
                            }
                            else if (checkBox4.Checked == false)
                            {
                                //非连续发送时，将连续发送状态标志位清零
                                //调用串口发送函数时带着参数
                                //为防止变化，将文本值保持为Send
                                type2 = 0;
                                TimerActionTwo.timerActionTwo(this, type2, textBox1.Text); 
                                if (languageflag == (int)Language.English)
                                {
                                    button3.Text = "Send";
                                }
                                else if (languageflag == (int)Language.Chinese)
                                {
                                    button3.Text = "发送";
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("没有需要发送数据!");
                        }
                    }
                    else if (button3.Text == "Stop" || button3.Text == "停止")
                    {
                        groupBox6.Enabled = true;
                        timer3.Enabled = false; 
                        if (languageflag == (int)Language.English)
                        {
                            button3.Text = "Send";
                        }
                        else if (languageflag == (int)Language.Chinese)
                        {
                            button3.Text = "发送";
                        }
                        checkBox4.Enabled = true;
                        numericUpDown2.Enabled = true;
                    }
                }
            }
            catch (Exception)
            {
                //MessageBox.Show("程序错误:" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //TIMER3发送串口数据2
        private void timer3_Tick(object sender, EventArgs e)
        {
            TimerActionTwo.timerActionTwo(this, type2, textBox1.Text);
        }

        //连续发送的选择框2
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
            { numericUpDown2.Enabled = true; }
            else
            { numericUpDown2.Enabled = false; }
        }

        //判断数据的长度来决定连续发送时间的最小值
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int myvalue = 0;
            maxlength = textBox1.Text.Length;
            //test length is 20
            //12345678901234567890
            if (ishex == "true")
            {
                if (maxlength <= 30)
                {
                    myvalue = 50;
                }
                else if (maxlength > 30 && maxlength <= 40)
                {
                    myvalue = 80;
                }
                else
                {
                    myvalue = 500;
                }
            }
            else
            {
                if (serialPort1.BaudRate < 2400)
                {
                    myvalue = 500;
                }
                else if (serialPort1.BaudRate >= 2400 && serialPort1.BaudRate < 4800)
                {
                    if (maxlength <= 15)
                    {
                        myvalue = 50;
                    }
                    else if (maxlength > 15 && maxlength < 20)
                    {
                        myvalue = 70;
                    }
                    else if (maxlength >= 20 && maxlength < 24)
                    {
                        myvalue = 80;
                    }
                    else if (maxlength >= 24 && maxlength < 28)
                    {
                        myvalue = 100;
                    }
                    else if (maxlength >= 28 && maxlength < 32)
                    {
                        myvalue = 110;
                    }
                    else if (maxlength >= 32 && maxlength < 36)
                    {
                        myvalue = 130;
                    }
                    else if (maxlength >= 36 && maxlength < 40)
                    {
                        myvalue = 150;
                    }
                    else
                    {
                        myvalue = 500;
                    }
                }
                else if (serialPort1.BaudRate >= 4800 && serialPort1.BaudRate < 9600)
                {
                    if (maxlength <= 31)
                    {
                        myvalue = 50;
                    }
                    else if (maxlength > 31 && maxlength < 40)
                    {
                        myvalue = 70;
                    }
                    else
                    {
                        myvalue = 500;
                    }
                }
                else
                {
                    if (maxlength <= 40)
                    {
                        myvalue = 50;
                    }
                    else
                    {
                        myvalue = 500;
                    }
                }
            }
            if (myvalue >= Convert.ToInt32(numericUpDown2.Value))
            {
                numericUpDown2.Value = myvalue;
            }
        }
        
        //连续发送的时间值改变直接赋值给timer
        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(numericUpDown2.Value) >= 50
                && Convert.ToInt32(numericUpDown2.Value) <= 65535
               )
            {
                timer3.Interval = Convert.ToInt32(numericUpDown2.Value);
            }
        }

        #endregion

        #region [串口接收数据处理部分-------------------------------------------]

        //数据接收的处理
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                _isReceiving = true;//接收标志位置一
                string text = string.Empty;//临时存放接收数据的字符串
                //byte[] result = new byte[serialPort1.BytesToRead];//定义存储接收数据的数组
                byte[] result = new byte[serialPort1.BytesToRead];//定义存储接收数据的数组
                serialPort1.Read(result, 0, result.Length);//定义接收字符串存储的数组、启示位置、长度
                if (checkBox3.Checked == true)//接收十六进制数时
                {
                    foreach (byte bhex in result)
                    {
                        text = text + bhex.ToString("X2");
                    }
                    this.setRich(text);
                    //textBox2.AppendText(text);
                    //Response(text);
                }
                else if (checkBox3.Checked == false)//接收ASCII码时
                {
                    foreach (byte basc in result)
                    {
                        text = text + ((char)ulong.Parse(basc.ToString(), System.Globalization.NumberStyles.HexNumber - 10)).ToString();//减10，能得出正确的结果，我也不明白原因，有待研究！！！
                    }
                    this.setRich(text);
                    //textBox2.AppendText(text);
                    //Response(text);
                }
                //dosomething(text);
                _isReceiving = false;//接收标志位清零

            }
            catch (Exception)
            {
                serialPort1.DiscardInBuffer();
                _isReceiving = false;
                //MessageBox.Show(ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //在GE的接收过程中存在一定问题，问题未可知
        //目前应该已经解决
        //委托
        private delegate void myDelegate(string tbstr);
        private void setRich(string tbstr)
        {
            if (this.textBox2.InvokeRequired)
            {
                myDelegate md = new myDelegate(this.setRich);
                this.Invoke(md, new object[] { tbstr });
            }
            else
            {
                // compare的值不为X，则进行接收数据处理，否则，直接写入到textBox.text中
                if (compare != "X")
                {
                    //if (this.Text != "ESP" && tbstr.Length >= Convert.ToInt32(ci))
                    //当加载配置文档的时候执行比较之类的东西
                    if (this.Text != "ESP")
                    {
                        tblength = textBox2.Text.Length;
                        //linshistring = "";
                        if (ishex == "false")//当不是十六进制时，比较十六进制转化为ascii码的值
                        {
                            if (restop != "")
                            {
                                //返回的开始和结尾都与配置文档匹配
                                if (tbstr.Substring(0, Convert.ToInt32(ci)) == toascii(recall) && tbstr.Substring(tbstr.Length - Convert.ToInt32(ci), Convert.ToInt32(ci)) == toascii(stop))
                                {
                                    //textBox2.AppendText(tbstr + "\r\n");
                                    textBox2.AppendText(tbstr);
                                    textBox2_TextChanged_Function();
                                    Response(tbstr);
                                }
                                //开始的匹配，但结尾不匹配
                                else if (tbstr.Substring(0, Convert.ToInt32(ci)) == toascii(recall) && tbstr.Substring(tbstr.Length - Convert.ToInt32(ci), Convert.ToInt32(ci)) != toascii(stop))
                                {
                                    textBox2.AppendText(tbstr);
                                    linshistring = tbstr;//保存到临时变量中
                                }
                                //开始不匹配，结尾不匹配
                                else if (tbstr.Substring(0, Convert.ToInt32(ci)) != toascii(recall) && tbstr.Substring(tbstr.Length - Convert.ToInt32(ci), Convert.ToInt32(ci)) != toascii(stop))
                                {
                                    textBox2.AppendText(tbstr);
                                    linshistring += tbstr;//保存到临时变量中
                                }
                                //开始不匹配，结尾匹配
                                else if (tbstr.Substring(0, Convert.ToInt32(ci)) != toascii(recall) && tbstr.Substring(tbstr.Length - Convert.ToInt32(ci), Convert.ToInt32(ci)) == toascii(stop))
                                {
                                    //textBox2.AppendText(tbstr + "\r\n");
                                    textBox2.AppendText(tbstr);
                                    textBox2_TextChanged_Function();
                                    linshistring += tbstr;//保存到临时变量中
                                    if (linshistring.Substring(0, Convert.ToInt32(ci)) == toascii(recall))
                                    {
                                        //执行回复处理方法
                                        Response(linshistring);
                                    }
                                    else
                                    {
                                        //清空linshistring
                                        linshistring = "";
                                    }
                                }
                            }
                            else if (restop == "")
                            {
                                //开始的匹配
                                if (tbstr.Substring(0, Convert.ToInt32(ci)) == toascii(recall))
                                {
                                    textBox2.AppendText(tbstr);
                                    linshistring = tbstr;//保存到临时变量中
                                }
                                else
                                {
                                    textBox2.AppendText(tbstr);
                                    linshistring += tbstr;//保存到临时变量中
                                }
                                //由于没有结束标志位，所以需要通过长度来判断是否接收结束
                                if (relength != "" && linshistring.Length / 2 == Convert.ToInt32(relength))
                                {
                                    Response(linshistring);
                                    //textBox2.AppendText("\r\n");
                                    textBox2.AppendText("");
                                    textBox2_TextChanged_Function();
                                }
                            }
                            //其他情况
                            else
                            {
                                //textBox2.AppendText(tbstr + "\r\n");
                                textBox2.AppendText(tbstr);
                                textBox2_TextChanged_Function();
                            }
                        }
                        else if (ishex == "true")
                        {
                            if (restop != "")
                            {
                                //返回的开始和结尾都与配置文档匹配
                                if (tbstr.Substring(0, Convert.ToInt32(ci)) == recall && tbstr.Substring(tbstr.Length - Convert.ToInt32(ci), Convert.ToInt32(ci)) == restop)
                                {
                                    //textBox2.AppendText(tbstr + "\r\n");
                                    textBox2.AppendText(tbstr);
                                    textBox2_TextChanged_Function();
                                    Response(tbstr);
                                }
                                //开始的匹配，但结尾不匹配
                                else if (tbstr.Substring(0, Convert.ToInt32(ci)) == recall && tbstr.Substring(tbstr.Length - Convert.ToInt32(ci), Convert.ToInt32(ci)) != restop)
                                {
                                    textBox2.AppendText(tbstr);
                                    linshistring = tbstr;//保存到临时变量中
                                }
                                //开始不匹配，结尾不匹配
                                else if (tbstr.Substring(0, Convert.ToInt32(ci)) != recall && tbstr.Substring(tbstr.Length - Convert.ToInt32(ci), Convert.ToInt32(ci)) != restop)
                                {
                                    textBox2.AppendText(tbstr);
                                    linshistring += tbstr;//保存到临时变量中
                                }
                                //开始不匹配，结尾匹配
                                else if (tbstr.Substring(0, Convert.ToInt32(ci)) != recall && tbstr.Substring(tbstr.Length - Convert.ToInt32(ci), Convert.ToInt32(ci)) == restop)
                                {
                                    //textBox2.AppendText(tbstr + "\r\n");
                                    textBox2.AppendText(tbstr);
                                    textBox2_TextChanged_Function();
                                    linshistring += tbstr;//保存到临时变量中
                                    if (linshistring.Substring(0, Convert.ToInt32(ci)) == recall)
                                    {
                                        //执行回复处理方法
                                        Response(linshistring);
                                    }
                                    else
                                    {
                                        //清空linshistring
                                        linshistring = "";
                                    }
                                }
                            }
                            else if (restop == "")
                            {
                                //开始的匹配
                                if (tbstr.Substring(0, Convert.ToInt32(ci)) == recall)
                                {
                                    textBox2.AppendText(tbstr);
                                    linshistring = tbstr;//保存到临时变量中
                                }
                                else
                                {
                                    textBox2.AppendText(tbstr);
                                    linshistring += tbstr;//保存到临时变量中
                                }
                                //由于没有结束标志位，所以需要通过长度来判断是否接收结束
                                if (relength != "" && linshistring.Length / 2 == Convert.ToInt32(relength))
                                {
                                    Response(linshistring);
                                    //textBox2.AppendText("\r\n");
                                    textBox2.AppendText("");
                                    textBox2_TextChanged_Function();
                                }
                            }
                            //其他情况
                            else
                            {
                                //textBox2.AppendText(tbstr + "\r\n"); 
                                textBox2.AppendText(tbstr);
                                textBox2_TextChanged_Function();
                            }
                        }
                    }
                    //如果不加载配置文档则在textbox中写入接收的内容并且不做处理
                    else
                    {
                        textBox2.AppendText(tbstr);
                    }
                }
                else
                {
                    textBox2.AppendText(tbstr);
                }
            }
        }

        //接收数据时选择处理数据的方式
        private void Response(string response)
        {
            if (start == "")//当Start位为空时
            {
                //listViewEA2.Items[0];
                if (commandid == response.Substring(Convert.ToInt32(ci), 2))
                { }
                else
                { }
            }
            else//当Start位不为空时
            {
                if (compare == "1")//是否需要验证
                {
                    if (commandid == response.Substring(Convert.ToInt32(ci), 2))//验证返回是否是当前CommandID
                    {
                        myResponse(response);
                    }
                    else//非当前CommandID
                    { }
                }
                else if (compare == "0")//如果不需要验证
                {
                    myResponse(response);//其实我觉得这个验证没有什么必要哈，不过感觉如果不这么写就少了点什么，难道是我神经质？囧
                }
            }
        }

        //接收数据处理事件
        private void myResponse(string response)
        {
            string t5 = null;//用来存储显示信息
            countreceive++;
            for (mycountindex = 0; mycountindex < listViewEA2.Items.Count; mycountindex++)
            {
                if (countreceive == 1)//第一次读取处理数据
                {
                    //为一个byte时
                    if (listViewEA2.Items[mycountindex].SubItems[2].Text == "1" && listViewEA2.Items[mycountindex].SubItems[1].Text != "1" && listViewEA2.Items[mycountindex].SubItems[1].Text != "String_1")
                    {
                        if (response.Length >= (Convert.ToInt32(dataindex[mycountindex]) + Convert.ToInt32(ci) + Convert.ToInt32(listViewEA2.Items[mycountindex].SubItems[2].Text) * 2))
                        {
                            string tresponse = response.Substring(Convert.ToInt32(dataindex[mycountindex]) + Convert.ToInt32(ci), Convert.ToInt32(listViewEA2.Items[mycountindex].SubItems[2].Text) * 2);
                            if (rxishu[mycountindex] != "1")
                            {
                                int t1 = Convert.ToInt32(tresponse, 16);
                                double t2 = Convert.ToDouble(t1);
                                double t3 = Convert.ToDouble(rxishu[mycountindex]);
                                double t4 = t2 / t3;
                                //t5 = Convert.ToInt32(t4).ToString("X2");
                                //t5 = Convert.ToInt32(t4).ToString();
                                t5 = t4.ToString("0.00");
                            }
                            else
                            {
                                t5 = Convert.ToInt32(tresponse,16).ToString();
                            }
                            listViewEA2.Items[mycountindex].SubItems.Add(t5);
                        }
                        else
                        {
                            listViewEA2.Items[mycountindex].SubItems.Add("");
                        }
                    }
                    //大端字节序，高字节存于内存低地址，低字节存于内存高地址；小端字节序反之。如一个long型数据0x12345678
                    //大端字节序：         
                    //          内存低地址   
                    //          0x12
                    //          0x34
                    //          0x56
                    //          0x78
                    //          内存高地址
                    //小端字节序：
                    //          内存低地址
                    //          0x78           
                    //          0x56
                    //          0x34
                    //          0x12
                    //          内存高地址
                    //为两个byte且为small时，小端序，高字节在
                    else if (listViewEA2.Items[mycountindex].SubItems[2].Text == "2" && rsmall[mycountindex] == "1" && listViewEA2.Items[mycountindex].SubItems[1].Text != "2" && listViewEA2.Items[mycountindex].SubItems[1].Text != "String_2")
                    {
                        if (response.Length >= (Convert.ToInt32(dataindex[mycountindex]) + Convert.ToInt32(ci) + Convert.ToInt32(listViewEA2.Items[mycountindex].SubItems[2].Text) * 2))
                        {
                            string tresponse = response.Substring(Convert.ToInt32(dataindex[mycountindex]) + Convert.ToInt32(ci), Convert.ToInt32(listViewEA2.Items[mycountindex].SubItems[2].Text) * 2);
                            string t01 = tresponse.Substring(0, 2);
                            string t02 = tresponse.Substring(2, 2);
                            if (rxishu[mycountindex] != "1")
                            {
                                int t11 = Convert.ToInt32(t01, 16);
                                int t12 = Convert.ToInt32(t02, 16);
                                double t21 = Convert.ToDouble(t11);
                                double t22 = Convert.ToDouble(t12);
                                double t3 = Convert.ToDouble(rxishu[mycountindex]);
                                double t41 = t21 / t3;
                                double t42 = (t22 * 256) / t3;
                                //t5 = Convert.ToInt32(t41 + t42).ToString("X4");
                                //t5 = Convert.ToInt32(t41 + t42).ToString();
                                t5 = (t41 + t42).ToString("0.00");
                            }
                            else
                            {
                                t5 = Convert.ToInt32(t02 + t01,16).ToString();
                            }
                            listViewEA2.Items[mycountindex].SubItems.Add(t5);
                        }
                        else
                        {
                            listViewEA2.Items[mycountindex].SubItems.Add("");
                        }
                    }
                    //为两个byte且为非small时
                    else if (listViewEA2.Items[mycountindex].SubItems[2].Text == "2" && rsmall[mycountindex] == "0" && listViewEA2.Items[mycountindex].SubItems[1].Text != "2" && listViewEA2.Items[mycountindex].SubItems[1].Text != "String_2")
                    {
                        if (response.Length >= (Convert.ToInt32(dataindex[mycountindex]) + Convert.ToInt32(ci) + Convert.ToInt32(listViewEA2.Items[mycountindex].SubItems[2].Text) * 2))
                        {
                            string tresponse = response.Substring(Convert.ToInt32(dataindex[mycountindex]) + Convert.ToInt32(ci), Convert.ToInt32(listViewEA2.Items[mycountindex].SubItems[2].Text) * 2);
                            if (rxishu[mycountindex] != "1")
                            {
                                int t1 = Convert.ToInt32(tresponse, 16);
                                double t2 = Convert.ToDouble(t1);
                                double t3 = Convert.ToDouble(rxishu[mycountindex]);
                                double t4 = t2 / t3;
                                //t5 = Convert.ToInt32(t4).ToString("X2");
                                //t5 = Convert.ToInt32(t4).ToString();
                                t5 = t4.ToString("0.00");
                            }
                            else
                            {
                                t5 = Convert.ToInt32(tresponse,16).ToString();
                            }
                            listViewEA2.Items[mycountindex].SubItems.Add(t5);
                        }
                        else
                        {
                            listViewEA2.Items[mycountindex].SubItems.Add("");
                        }
                    }
                    //当长度为3和4时，加载方式如下
                    else if (Convert.ToInt32(listViewEA2.Items[mycountindex].SubItems[2].Text) == 3 && listViewEA2.Items[mycountindex].SubItems[1].Text != "String_3")
                    {
                        // 判断长度是否够，当长度符合操作条件时才进行运算
                        if (response.Length >= (Convert.ToInt32(dataindex[mycountindex]) + Convert.ToInt32(ci) + Convert.ToInt32(listViewEA2.Items[mycountindex].SubItems[2].Text) * 2))
                        {
                            // 截取需要计算的参数
                            string tresponse = response.Substring(Convert.ToInt32(dataindex[mycountindex]) + Convert.ToInt32(ci), Convert.ToInt32(listViewEA2.Items[mycountindex].SubItems[2].Text) * 2);
                            string t01 = tresponse.Substring(0, 2);
                            string t02 = tresponse.Substring(2, 2);
                            string t03 = tresponse.Substring(4, 2);
                            string t1 = t03 + t02 + t01; 
                            if (rxishu[mycountindex] != "1")
                            {
                                double t2 = Convert.ToDouble(Convert.ToInt64(t1, 16));
                                double t3 = Convert.ToDouble(rxishu[mycountindex]);
                                double t4 = t2 / t3;
                                //t5 = Convert.ToInt32(t41 + t42).ToString("X4");
                                //t5 = Convert.ToInt32(t4).ToString();
                                t5 = t4.ToString("0.00");
                            }
                            else
                            {
                                t5 = t1;
                            }
                            listViewEA2.Items[mycountindex].SubItems.Add(t5);
                        }
                        else
                        {
                            listViewEA2.Items[mycountindex].SubItems.Add("");
                        }
                    }
                    else if (Convert.ToInt32(listViewEA2.Items[mycountindex].SubItems[2].Text) == 4 && listViewEA2.Items[mycountindex].SubItems[1].Text != "String_4")
                    {
                        // 判断长度是否够，当长度符合操作条件时才进行运算
                        if (response.Length >= (Convert.ToInt32(dataindex[mycountindex]) + Convert.ToInt32(ci) + Convert.ToInt32(listViewEA2.Items[mycountindex].SubItems[2].Text) * 2))
                        {
                            // 截取需要计算的参数
                            string tresponse = response.Substring(Convert.ToInt32(dataindex[mycountindex]) + Convert.ToInt32(ci), Convert.ToInt32(listViewEA2.Items[mycountindex].SubItems[2].Text) * 2);
                            string t01 = tresponse.Substring(0, 2);
                            string t02 = tresponse.Substring(2, 2);
                            string t03 = tresponse.Substring(4, 2);
                            string t04 = tresponse.Substring(6, 2);
                            string t1 = t04 + t03 + t02 + t01;
                            if (rxishu[mycountindex] != "1")
                            {
                                double t2 = Convert.ToDouble(Convert.ToInt64(t1, 16));
                                double t3 = Convert.ToDouble(rxishu[mycountindex]);
                                double t4 = t2 / t3;
                                //t5 = Convert.ToInt32(t41 + t42).ToString("X4");
                                //t5 = Convert.ToInt32(t4).ToString();
                                t5 = t4.ToString("0.00");
                            }
                            else
                            {
                                t5 = t1;
                            }
                            listViewEA2.Items[mycountindex].SubItems.Add(t5);
                        }
                        else
                        {
                            listViewEA2.Items[mycountindex].SubItems.Add("");
                        }
                    }
                    //当得到数据的长度大于2时，即理论上应该是字符串时，执行如下函数
                    else
                    {
                        if (response.Length >= (Convert.ToInt32(dataindex[mycountindex]) + Convert.ToInt32(ci) + Convert.ToInt32(listViewEA2.Items[mycountindex].SubItems[2].Text) * 2))
                        {
                            string tresponse = response.Substring(Convert.ToInt32(dataindex[mycountindex]) + Convert.ToInt32(ci), Convert.ToInt32(listViewEA2.Items[mycountindex].SubItems[2].Text) * 2);
                            t5 = HexAndAscII.toascii(tresponse);
                            listViewEA2.Items[mycountindex].SubItems.Add(t5);
                        }
                        else
                        {
                            listViewEA2.Items[mycountindex].SubItems.Add("");
                        }
                    }
                }
                else//第二次及以后处理数据
                {
                    //为一个byte时
                    if (listViewEA2.Items[mycountindex].SubItems[2].Text == "1" && listViewEA2.Items[mycountindex].SubItems[1].Text != "1" && listViewEA2.Items[mycountindex].SubItems[1].Text != "String_1")
                    {
                        if (response.Length >= (Convert.ToInt32(dataindex[mycountindex]) + Convert.ToInt32(ci) + Convert.ToInt32(listViewEA2.Items[mycountindex].SubItems[2].Text) * 2))
                        {
                            string tresponse = response.Substring(Convert.ToInt32(dataindex[mycountindex]) + Convert.ToInt32(ci), Convert.ToInt32(listViewEA2.Items[mycountindex].SubItems[2].Text) * 2);
                            if (rxishu[mycountindex] != "1")
                            {
                                int t1 = Convert.ToInt32(tresponse, 16);
                                double t2 = Convert.ToDouble(t1);
                                double t3 = Convert.ToDouble(rxishu[mycountindex]);
                                double t4 = t2 / t3;
                                //t5 = Convert.ToInt32(t4).ToString("X2");
                                //t5 = Convert.ToInt32(t4).ToString();
                                t5 = t4.ToString("0.00");
                            }
                            else
                            {
                                t5 = Convert.ToInt32(tresponse,16).ToString();
                            }
                            listViewEA2.Items[mycountindex].SubItems[3].Text = t5;
                        }
                        else
                        {
                            listViewEA2.Items[mycountindex].SubItems[3].Text = "";
                        }
                    }
                    //为两个byte且为small时
                    else if (listViewEA2.Items[mycountindex].SubItems[2].Text == "2" && rsmall[mycountindex] == "1" && listViewEA2.Items[mycountindex].SubItems[1].Text != "2" && listViewEA2.Items[mycountindex].SubItems[1].Text != "String_2")
                    {
                        if (response.Length >= (Convert.ToInt32(dataindex[mycountindex]) + Convert.ToInt32(ci) + Convert.ToInt32(listViewEA2.Items[mycountindex].SubItems[2].Text) * 2))
                        {
                            string tresponse = response.Substring(Convert.ToInt32(dataindex[mycountindex]) + Convert.ToInt32(ci), Convert.ToInt32(listViewEA2.Items[mycountindex].SubItems[2].Text) * 2);
                            string t01 = tresponse.Substring(0, 2);
                            string t02 = tresponse.Substring(2, 2);
                            if (rxishu[mycountindex] != "1")
                            {
                                int t11 = Convert.ToInt32(t01, 16);
                                int t12 = Convert.ToInt32(t02, 16);
                                double t21 = Convert.ToDouble(t11);
                                double t22 = Convert.ToDouble(t12);
                                double t3 = Convert.ToDouble(rxishu[mycountindex]);
                                double t41 = t21 / t3;
                                double t42 = (t22 * 256) / t3;
                                //t5 = Convert.ToInt32(t41 + t42).ToString("X4");
                                //t5 = Convert.ToInt32(t41 + t42).ToString();
                                t5 = (t41 + t42).ToString("0.00");
                            }
                            else
                            {
                                t5 = Convert.ToInt32(t02 + t01,16).ToString();
                            }
                            listViewEA2.Items[mycountindex].SubItems[3].Text = t5;
                        }
                        else
                        {
                            listViewEA2.Items[mycountindex].SubItems[3].Text = "";
                        }
                    }
                    //为两个byte且为非small时
                    else if (listViewEA2.Items[mycountindex].SubItems[2].Text == "2" && rsmall[mycountindex] == "0" && listViewEA2.Items[mycountindex].SubItems[1].Text != "2" && listViewEA2.Items[mycountindex].SubItems[1].Text != "String_2")
                    {
                        if (response.Length >= (Convert.ToInt32(dataindex[mycountindex]) + Convert.ToInt32(ci) + Convert.ToInt32(listViewEA2.Items[mycountindex].SubItems[2].Text) * 2))
                        {
                            string tresponse = response.Substring(Convert.ToInt32(dataindex[mycountindex]) + Convert.ToInt32(ci), Convert.ToInt32(listViewEA2.Items[mycountindex].SubItems[2].Text) * 2);
                            if (rxishu[mycountindex] != "1")
                            {
                                int t1 = Convert.ToInt32(tresponse, 16);
                                double t2 = Convert.ToDouble(t1);
                                double t3 = Convert.ToDouble(rxishu[mycountindex]);
                                double t4 = t2 / t3;
                                //t5 = Convert.ToInt32(t4).ToString("X2");
                                //t5 = Convert.ToInt32(t4).ToString();
                                t5 = t4.ToString("0.00");
                            }
                            else
                            {
                                t5 = Convert.ToInt32(tresponse,16).ToString();
                            }
                            listViewEA2.Items[mycountindex].SubItems[3].Text = t5;
                        }
                        else
                        {
                            listViewEA2.Items[mycountindex].SubItems[3].Text = "";
                        }
                    }
                    //当长度为3和4时，加载方式如下
                    else if (Convert.ToInt32(listViewEA2.Items[mycountindex].SubItems[2].Text) == 3 && listViewEA2.Items[mycountindex].SubItems[1].Text != "String_3")
                    {
                        // 判断长度是否够，当长度符合操作条件时才进行运算
                        if (response.Length >= (Convert.ToInt32(dataindex[mycountindex]) + Convert.ToInt32(ci) + Convert.ToInt32(listViewEA2.Items[mycountindex].SubItems[2].Text) * 2))
                        {
                            // 截取需要计算的参数
                            string tresponse = response.Substring(Convert.ToInt32(dataindex[mycountindex]) + Convert.ToInt32(ci), Convert.ToInt32(listViewEA2.Items[mycountindex].SubItems[2].Text) * 2);
                            string t01 = tresponse.Substring(0, 2);
                            string t02 = tresponse.Substring(2, 2);
                            string t03 = tresponse.Substring(4, 2);
                            string t1 = t03 + t02 + t01;
                            if (rxishu[mycountindex] != "1")
                            {
                                double t2 = Convert.ToDouble(Convert.ToInt64(t1, 16));
                                double t3 = Convert.ToDouble(rxishu[mycountindex]);
                                double t4 = t2 / t3;
                                //t5 = Convert.ToInt32(t41 + t42).ToString("X4");
                                //t5 = Convert.ToInt32(t4).ToString();
                                t5 = t4.ToString("0.00");
                            }
                            else
                            {
                                t5 = t1;
                            } 
                            listViewEA2.Items[mycountindex].SubItems[3].Text = t5;
                        }
                        else
                        {
                            listViewEA2.Items[mycountindex].SubItems[3].Text = "";
                        }
                    }
                    else if (Convert.ToInt32(listViewEA2.Items[mycountindex].SubItems[2].Text) == 4 && listViewEA2.Items[mycountindex].SubItems[1].Text != "String_4")
                    {
                        // 判断长度是否够，当长度符合操作条件时才进行运算
                        if (response.Length >= (Convert.ToInt32(dataindex[mycountindex]) + Convert.ToInt32(ci) + Convert.ToInt32(listViewEA2.Items[mycountindex].SubItems[2].Text) * 2))
                        {
                            // 截取需要计算的参数
                            string tresponse = response.Substring(Convert.ToInt32(dataindex[mycountindex]) + Convert.ToInt32(ci), Convert.ToInt32(listViewEA2.Items[mycountindex].SubItems[2].Text) * 2);
                            string t01 = tresponse.Substring(0, 2);
                            string t02 = tresponse.Substring(2, 2);
                            string t03 = tresponse.Substring(4, 2);
                            string t04 = tresponse.Substring(6, 2);
                            string t1 = t04 + t03 + t02 + t01;
                            if (rxishu[mycountindex] != "1")
                            {
                                double t2 = Convert.ToDouble(Convert.ToInt64(t1, 16));
                                double t3 = Convert.ToDouble(rxishu[mycountindex]);
                                double t4 = t2 / t3;
                                //t5 = Convert.ToInt32(t41 + t42).ToString("X4");
                                //t5 = Convert.ToInt32(t4).ToString();
                                t5 = t4.ToString("0.00");
                            }
                            else
                            {
                                t5 = t1;
                            }
                            listViewEA2.Items[mycountindex].SubItems[3].Text = t5;
                        }
                        else
                        {
                            listViewEA2.Items[mycountindex].SubItems[3].Text = "";
                        }
                    }
                    //当得到数据的长度大于2时，即理论上应该是字符串时，执行如下函数
                    else
                    {
                        if (response.Length >= (Convert.ToInt32(dataindex[mycountindex]) + Convert.ToInt32(ci) + Convert.ToInt32(listViewEA2.Items[mycountindex].SubItems[2].Text) * 2))
                        {
                            string tresponse = response.Substring(Convert.ToInt32(dataindex[mycountindex]) + Convert.ToInt32(ci), Convert.ToInt32(listViewEA2.Items[mycountindex].SubItems[2].Text) * 2);
                            t5 = HexAndAscII.toascii(tresponse);
                            listViewEA2.Items[mycountindex].SubItems[3].Text = t5;
                        }
                        else
                        {
                            listViewEA2.Items[mycountindex].SubItems[3].Text = "";
                        }
                    }
                }

            }
            //测试
            //textBox3.Text = response.Length.ToString() + "  ";
            //textBox3.Text += dataindex[0];
            //listViewEA2.Items[0].SubItems.Add("999");
            //textBox3.Text = "11111111";
        }
        //更新Responses的值，表示接收到数据的个数
        /*
        private void textBox2_TextChanged_Function(object sender, EventArgs e)
        {
            if (textBox2.Text.Length != tblength)
            {
                countD++;
                toolStripStatusLabel4.Text = "Responses: " + countD + "  |";
            }
            if (textBox2.Text.Length >= (textBox2.MaxLength - 1000))
            {
                textBox2.Text = "";
            }
        }
        */
        private void textBox2_TextChanged_Function()
        {
            if (textBox2.Text.Length != tblength)
            {
                countD++;
                toolStripStatusLabel4.Text = "Responses: " + countD + "  |";
            }
            if (textBox2.Text.Length >= (textBox2.MaxLength - 1000))
            {
                textBox2.Text = "";
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // textBox2的最大长度定义为 1999999999
            if (textBox2.Text.Length >= (textBox2.MaxLength - 10000))
            {
                saved_event();
                textBox2.Clear();
                toolStripStatusLabel3.Text = "|  Commands: 0　　";
                toolStripStatusLabel4.Text = "Responses: 0  |";
                countC = 0;
                countD = 0;
            }
        }
        #endregion

        #region [打开配置XML文档，并将XML文档的节点读入到TreeView中-------------]

        //打开配置文档
        private void toolStripMenuItem11_Click(object sender, EventArgs e)
        {
            path = Application.StartupPath;
            openFileDialog1.InitialDirectory = path;
            openFileDialog1.ShowDialog();
        }

        //点击"打开文件"按钮触发的事件
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            //保存到.cds配置文件
            try
            {
                //加载到treeView中
                this.LoadTreeNodes(openFileDialog1.FileName);
            }
            catch (Exception)
            {
                //MessageBox.Show("配置文档结构错误：" + ex.Message);
                this.Text = "ESP";
                XmlDocument itemcds = new XmlDocument();
                treeView1.Nodes.Clear();
                itemcds.Load(Application.StartupPath + @"\ESPConfig.cds");
                itemcds.SelectSingleNode("Config/MRU/Item_0").InnerText = itemcds.SelectSingleNode("Config/MRU/Item_1").InnerText;
                itemcds.SelectSingleNode("Config/MRU/Item_1").InnerText = itemcds.SelectSingleNode("Config/MRU/Item_2").InnerText;
                itemcds.SelectSingleNode("Config/MRU/Item_2").InnerText = itemcds.SelectSingleNode("Config/MRU/Item_3").InnerText;
                itemcds.SelectSingleNode("Config/MRU/Item_3").InnerText = itemcds.SelectSingleNode("Config/MRU/Item_4").InnerText;
                itemcds.SelectSingleNode("Config/MRU/Item_4").InnerText = "";
                itemcds.Save(Application.StartupPath + @"\ESPConfig.cds");
                toolStripMenuItem121.Text = toolStripMenuItem122.Text;
                toolStripMenuItem122.Text = toolStripMenuItem123.Text;
                toolStripMenuItem123.Text = toolStripMenuItem124.Text;
                toolStripMenuItem124.Text = toolStripMenuItem125.Text;
                toolStripMenuItem125.Text = "";
            }
        }

        //添加节点
        private void LoadTreeNodes(string xmlPath)
        {
            try
            {
                //清空存储值
                //Array.Clear(mystore,0,mystore.Length);
                //下标置零
                //mystorecounter = 0;
                mystoreindex = 0;
                mycountindex = 0;
                mycountin = 0;
                //清空操作
                label1.Text = "";
                treeView1.Nodes.Clear();
                listViewEA1.Items.Clear();
                listViewEA2.Items.Clear();
                //将地址保存在adress中，在单击treeView的节点时会读取adress的值
                adress = xmlPath;
                //判断打开的文件名和历史记录中的文件名是否一样
                if (xmlPath == toolStripMenuItem121.Text)
                { }
                else if (xmlPath == toolStripMenuItem122.Text)
                {
                    toolStripMenuItem122.Text = toolStripMenuItem121.Text;
                    toolStripMenuItem121.Text = xmlPath;
                }
                else if (xmlPath == toolStripMenuItem123.Text)
                {
                    toolStripMenuItem123.Text = toolStripMenuItem122.Text;
                    toolStripMenuItem122.Text = toolStripMenuItem121.Text;
                    toolStripMenuItem121.Text = xmlPath;
                }
                else if (xmlPath == toolStripMenuItem124.Text)
                {
                    toolStripMenuItem124.Text = toolStripMenuItem123.Text;
                    toolStripMenuItem123.Text = toolStripMenuItem122.Text;
                    toolStripMenuItem122.Text = toolStripMenuItem121.Text;
                    toolStripMenuItem121.Text = xmlPath;
                }
                else if (xmlPath == toolStripMenuItem125.Text)
                {
                    toolStripMenuItem125.Text = toolStripMenuItem124.Text;
                    toolStripMenuItem124.Text = toolStripMenuItem123.Text;
                    toolStripMenuItem123.Text = toolStripMenuItem122.Text;
                    toolStripMenuItem122.Text = toolStripMenuItem121.Text;
                    toolStripMenuItem121.Text = xmlPath;
                }
                else if (xmlPath != toolStripMenuItem121.Text && xmlPath != toolStripMenuItem122.Text && xmlPath != toolStripMenuItem123.Text && xmlPath != toolStripMenuItem124.Text && xmlPath != toolStripMenuItem125.Text)
                {
                    toolStripMenuItem125.Text = toolStripMenuItem124.Text;
                    toolStripMenuItem124.Text = toolStripMenuItem123.Text;
                    toolStripMenuItem123.Text = toolStripMenuItem122.Text;
                    toolStripMenuItem122.Text = toolStripMenuItem121.Text;
                    toolStripMenuItem121.Text = xmlPath;
                }
                //将路径添加到ESPConfig.cds中
                XmlDocument savecds = new XmlDocument();
                if (File.Exists(Application.StartupPath + @"\ESPConfig.cds"))
                {
                    savecds.Load(Application.StartupPath + @"\ESPConfig.cds");
                    if (xmlPath == savecds.SelectSingleNode("Config/MRU/Item_0").InnerText)
                    { }
                    else if (xmlPath == savecds.SelectSingleNode("Config/MRU/Item_1").InnerText)
                    {
                        savecds.SelectSingleNode("Config/MRU/Item_1").InnerText = savecds.SelectSingleNode("Config/MRU/Item_0").InnerText;
                        savecds.SelectSingleNode("Config/MRU/Item_0").InnerText = xmlPath;
                    }
                    else if (xmlPath == savecds.SelectSingleNode("Config/MRU/Item_2").InnerText)
                    {
                        savecds.SelectSingleNode("Config/MRU/Item_2").InnerText = savecds.SelectSingleNode("Config/MRU/Item_1").InnerText;
                        savecds.SelectSingleNode("Config/MRU/Item_1").InnerText = savecds.SelectSingleNode("Config/MRU/Item_0").InnerText;
                        savecds.SelectSingleNode("Config/MRU/Item_0").InnerText = xmlPath;
                    }
                    else if (xmlPath == savecds.SelectSingleNode("Config/MRU/Item_3").InnerText)
                    {
                        savecds.SelectSingleNode("Config/MRU/Item_3").InnerText = savecds.SelectSingleNode("Config/MRU/Item_2").InnerText;
                        savecds.SelectSingleNode("Config/MRU/Item_2").InnerText = savecds.SelectSingleNode("Config/MRU/Item_1").InnerText;
                        savecds.SelectSingleNode("Config/MRU/Item_1").InnerText = savecds.SelectSingleNode("Config/MRU/Item_0").InnerText;
                        savecds.SelectSingleNode("Config/MRU/Item_0").InnerText = xmlPath;
                    }
                    else if (xmlPath == savecds.SelectSingleNode("Config/MRU/Item_4").InnerText)
                    {
                        savecds.SelectSingleNode("Config/MRU/Item_4").InnerText = savecds.SelectSingleNode("Config/MRU/Item_3").InnerText;
                        savecds.SelectSingleNode("Config/MRU/Item_3").InnerText = savecds.SelectSingleNode("Config/MRU/Item_2").InnerText;
                        savecds.SelectSingleNode("Config/MRU/Item_2").InnerText = savecds.SelectSingleNode("Config/MRU/Item_1").InnerText;
                        savecds.SelectSingleNode("Config/MRU/Item_1").InnerText = savecds.SelectSingleNode("Config/MRU/Item_0").InnerText;
                        savecds.SelectSingleNode("Config/MRU/Item_0").InnerText = xmlPath;
                    }
                    else if (xmlPath != savecds.SelectSingleNode("Config/MRU/Item_0").InnerText && xmlPath != savecds.SelectSingleNode("Config/MRU/Item_1").InnerText && xmlPath != savecds.SelectSingleNode("Config/MRU/Item_2").InnerText && xmlPath != savecds.SelectSingleNode("Config/MRU/Item_3").InnerText && xmlPath != savecds.SelectSingleNode("Config/MRU/Item_4").InnerText)
                    {
                        savecds.SelectSingleNode("Config/MRU/Item_4").InnerText = savecds.SelectSingleNode("Config/MRU/Item_3").InnerText;
                        savecds.SelectSingleNode("Config/MRU/Item_3").InnerText = savecds.SelectSingleNode("Config/MRU/Item_2").InnerText;
                        savecds.SelectSingleNode("Config/MRU/Item_2").InnerText = savecds.SelectSingleNode("Config/MRU/Item_1").InnerText;
                        savecds.SelectSingleNode("Config/MRU/Item_1").InnerText = savecds.SelectSingleNode("Config/MRU/Item_0").InnerText;
                        savecds.SelectSingleNode("Config/MRU/Item_0").InnerText = xmlPath;
                    }
                    savecds.Save(Application.StartupPath + @"\ESPConfig.cds");
                }
                else
                {
                    //如果不存在则生成
                    buildConfigurationFile();
                }
                //将路径信息去掉，只保留文件名称和文件类型
                string filename = xmlPath.Substring(xmlPath.LastIndexOf("\\") + 1);
                //显示在Form1窗体的标题上
                this.Text = "ESP" + " - " + filename;
                this.xmlDoc = new XmlDocument();
                this.xmlDoc.Load(xmlPath);
                

                start = xmlDoc.SelectSingleNode("Personality").Attributes["Start"].InnerText;
                stop = xmlDoc.SelectSingleNode("Personality").Attributes["Stop"].InnerText;
                checkstyle = xmlDoc.SelectSingleNode("Personality").Attributes["CheckStyle"].InnerText;
                ci = xmlDoc.SelectSingleNode("Personality").Attributes["CI"].InnerText;
                cmi = xmlDoc.SelectSingleNode("Personality").Attributes["CMI"].InnerText;
                li = xmlDoc.SelectSingleNode("Personality").Attributes["LI"].InnerText;
                pi = xmlDoc.SelectSingleNode("Personality").Attributes["PI"].InnerText;
                csi = xmlDoc.SelectSingleNode("Personality").Attributes["CSI"].InnerText;
                compare = xmlDoc.SelectSingleNode("Personality").Attributes["Compare"].InnerText;
                relength = xmlDoc.SelectSingleNode("Personality").Attributes["Relength"].InnerText;
                recall = xmlDoc.SelectSingleNode("Personality").Attributes["Recall"].InnerText;
                restop = xmlDoc.SelectSingleNode("Personality").Attributes["Restop"].InnerText;
                ishex = xmlDoc.SelectSingleNode("Personality").Attributes["HEX"].InnerText;
                checkBox5.Checked = Convert.ToBoolean(ishex);
                checkBox3.Checked = Convert.ToBoolean(ishex);

                //若存在串口配置则读取串口配置
                XmlNode nodesp = this.xmlDoc.SelectSingleNode("Personality/Serialport");
                if (nodesp != null)
                {
                    spconfig[1] = nodesp.Attributes["braudrate"].InnerText;
                    spconfig[2] = nodesp.Attributes["parity"].InnerText;
                    serialPort1.BaudRate = Convert.ToInt32(spconfig[1]);
                    if (spconfig[2] == "None")
                    { serialPort1.Parity = Parity.None; }
                    else if (spconfig[2] == "Odd")
                    { serialPort1.Parity = Parity.Odd; }
                    else if (spconfig[2] == "Even")
                    { serialPort1.Parity = Parity.Even; }
                    else if (spconfig[2] == "Mark")
                    { serialPort1.Parity = Parity.Mark; }
                    else if (spconfig[2] == "Space")
                    { serialPort1.Parity = Parity.Space; }
                    toolStripStatusLabel1.Text = serialPort1.PortName + " " + "Closed" + "," + serialPort1.BaudRate + "," + serialPort1.Parity + "," + serialPort1.DataBits + "," + serialPort1.StopBits;
                }

                //若存在CRC表则读取CRC表
                XmlNode CRCNode = this.xmlDoc.SelectSingleNode("Personality/CRC");
                if (CRCNode != null)
                {
                    TempCRCTable = CRCNode.InnerText;
                    CRCType = Convert.ToUInt16(CRCNode.Attributes["crctype"].InnerText);
                    if (checkstyle == "CRC16_1")
                    {
                        if (TempCRCTable.Length == 6 * 256)
                        {
                            for (int cc = 0; cc < 256; cc++)
                            {
                                CRC16LookupTable[cc] = Convert.ToUInt16(TempCRCTable.Substring(cc * 6 + 2, 4), 16);
                            }
                        }
                    }
                    else if (checkstyle == "CRC8_1")
                    {
                        if (TempCRCTable.Length == 4 * 256)
                        {
                            for (int cc = 0; cc < 256; cc++)
                            {
                                CRC8LookupTable[cc] = Convert.ToByte(TempCRCTable.Substring(cc * 4 + 2, 2), 16);
                            }
                        }
                    }
                    else if (checkstyle == "CRC8_2")
                    {
                        if (TempCRCTable.Length == 4 * 256)
                        {
                            for (int cc = 0; cc < 256; cc++)
                            {
                                CRC8LookupTable[cc] = Convert.ToByte(TempCRCTable.Substring(cc * 4 + 2, 2), 16);
                            }
                        }
                    }
                    else if (checkstyle == "CRC8_3")
                    {
                        if (TempCRCTable.Length == 4 * 256)
                        {
                            for (int cc = 0; cc < 256; cc++)
                            {
                                CRC8LookupTable[cc] = Convert.ToByte(TempCRCTable.Substring(cc * 4 + 2, 2), 16);
                            }
                        }
                    }
                    else if (checkstyle == "CRC8_4")
                    {
                        if (TempCRCTable.Length == 4 * 256)
                        {
                            for (int cc = 0; cc < 256; cc++)
                            {
                                CRC8LookupTable[cc] = Convert.ToByte(TempCRCTable.Substring(cc * 4 + 2, 2), 16);
                            }
                        }
                    }
                }

                XmlNodeList nodelist = this.xmlDoc.SelectNodes("Personality/Module");
                this.treeView1.BeginUpdate();
                this.treeView1.Nodes.Clear();
                this.ConvertXmlNodeToTreeNode(nodelist, this.treeView1.Nodes);
                this.treeView1.EndUpdate();
            }
            catch (Exception)
            {
                //MessageBox.Show("程序错误:" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                XmlDocument itemcds = new XmlDocument();
                treeView1.Nodes.Clear();
                itemcds.Load(Application.StartupPath + @"\ESPConfig.cds");
                itemcds.SelectSingleNode("Config/MRU/Item_0").InnerText = itemcds.SelectSingleNode("Config/MRU/Item_1").InnerText;
                itemcds.SelectSingleNode("Config/MRU/Item_1").InnerText = itemcds.SelectSingleNode("Config/MRU/Item_2").InnerText;
                itemcds.SelectSingleNode("Config/MRU/Item_2").InnerText = itemcds.SelectSingleNode("Config/MRU/Item_3").InnerText;
                itemcds.SelectSingleNode("Config/MRU/Item_3").InnerText = itemcds.SelectSingleNode("Config/MRU/Item_4").InnerText;
                itemcds.SelectSingleNode("Config/MRU/Item_4").InnerText = "";
                this.Text = "ESP";
                itemcds.Save(Application.StartupPath + @"\ESPConfig.cds");
                toolStripMenuItem121.Text = toolStripMenuItem122.Text;
                toolStripMenuItem122.Text = toolStripMenuItem123.Text;
                toolStripMenuItem123.Text = toolStripMenuItem124.Text;
                toolStripMenuItem124.Text = toolStripMenuItem125.Text;
                toolStripMenuItem125.Text = "";
            }
        }

        //转化XmlNode到TreeNode
        private void ConvertXmlNodeToTreeNode(XmlNodeList nodelist, TreeNodeCollection treeNodes)
        {
            try
            {
                foreach (XmlNode xmlNode in nodelist)
                {
                    string nodeText = xmlNode.Attributes["Name"].InnerText;
                    TreeNode newTreeNode = new TreeNode(nodeText);
                    treeNodes.Add(newTreeNode);
                    if (xmlNode.HasChildNodes)
                    {
                        this.ConvertXmlNodeToTreeNodeson(xmlNode.ChildNodes, newTreeNode.Nodes);
                    }
                }
            }
            catch (Exception)
            {
                //MessageBox.Show("程序错误:" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //子节点的处理
        private void ConvertXmlNodeToTreeNodeson(XmlNodeList nodelist, TreeNodeCollection treeNodes)
        {
            try
            {
                foreach (XmlNode xmlNode in nodelist)
                {
                    string nodeText = xmlNode.Attributes["Name"].InnerText;
                    TreeNode newTreeNode = new TreeNode(nodeText);
                    treeNodes.Add(newTreeNode);
                }
            }
            catch (Exception)
            {
                //MessageBox.Show("程序错误:" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region [单击双击TreeView节点触发事件，使XML文档的信息加载到ListView中--]

        //获得选中节点的等级
        public int NodeLevel(TreeNode n)
        {
            int i = 0;
            while (!(n.Parent == null))
            {
                n = n.Parent;
                i += 1;
            }
            return i;
        }

        //双击treeview节点的事件
        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // 右键无效
            if (e.Button == MouseButtons.Right)
            {
                return;
            }

            //第一级节点
            if (NodeLevel(e.Node).ToString() == "0")
            {
            }
            // 第二级节点,由于e.Node在treeView中是鼠标悬停时的节点，
            // 所以当双击父节点时，节点展开，会出现鼠标悬停在子节点上
            // 导致出现认为是双击子节点的bug
            // 解决方法是，同时满足选中的节点的父节点不为空，即选中的是子节点才执行有效双击事件
            else if ((NodeLevel(e.Node).ToString() == "1") && (treeView1.SelectedNode.Parent != null))
            {
                //选中子节点 
                buildbutton();
                try
                {
                    if (serialPort1.IsOpen)//判断串口打开
                    {
                        if ((button3.Text == "Send" && button1.Text == "Send") ||(button3.Text == "发送" && button1.Text == "发送"))//当两个发送按钮都是就位状态时才能双击发送
                        {
                            if (textBox1.Text != "")//当不为空时，发送
                            {
                                type2 = 0;
                                TimerActionTwo.timerActionTwo(this, type2, textBox1.Text);
                            }
                            else//为空时显示无数据
                            {
                                MessageBox.Show("No to-be-sent data!");
                            }
                        }
                        else if (button3.Text == "Stop" || button1.Text == "Stop" || button3.Text == "停止" || button1.Text == "停止")//当任意发送按钮已经按下且发送数据时，不做操作
                        {
                        }
                    }
                    else//如果串口没有打开
                    {
                        spopen();//打开串口
                        if ((button3.Text == "Send" && button1.Text == "Send") ||(button3.Text == "发送" && button1.Text == "发送"))//当两个发送按钮都是就位状态时才能双击发送
                        {
                            if (textBox1.Text != "")//当不为空时，发送
                            {
                                type2 = 0;
                                TimerActionTwo.timerActionTwo(this, type2, textBox1.Text);
                            }
                            else//为空时显示无数据
                            {
                                MessageBox.Show("No to-be-sent data!");
                            }
                        }
                        else if (button3.Text == "Stop" || button1.Text == "Stop" || button3.Text == "停止" || button1.Text == "停止")//当任意发送按钮已经按下且发送数据时，不做操作
                        {
                        }
                    }
                }
                catch (Exception)
                {
                    //MessageBox.Show("程序错误:" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (NodeLevel(e.Node).ToString() == "2")
            {
            }
        }

        //单机treeview节点的事件
        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (NodeLevel(e.Node).ToString() == "0")
            {
            }
            else if (NodeLevel(e.Node).ToString() == "1")
            {
                mycountin = 0;
                mycountindex = 0;
                //选中子节点 
                //当选中的节点和label1.Text一致则不改变值，如果不一致则更改值
                if (e.Node.Text != label1.Text)
                {
                    listViewEA1.Items.Clear();
                    listViewEA2.Items.Clear();
                    //加载文件路径
                    this.xmlDoc.Load(adress);
                    //得到选中节点的名称
                    name = e.Node.Text;
                    XmlNodeList nodelist = this.xmlDoc.SelectNodes("Personality/Module/Command");
                    //加载相关内容
                    getCheckedNode(nodelist);
                    label1.Text = e.Node.Text;
                    for (mycountin = 0; mycountin < listViewEA1.Items.Count; mycountin++)
                    {
                        for (mystorecounter = 0; mystorecounter < 200; mystorecounter++)
                        {
                            if (mystore[mystorecounter].commandname == label1.Text)
                            {
                                if (mystore[mystorecounter].itemname == listViewEA1.Items[mycountin].SubItems[0].Text)
                                {
                                    listViewEA1.Items[mycountin].SubItems[3].Text = mystore[mystorecounter].itemvalue;
                                }
                            }
                        }
                    }
                    //接收数据时，当改变Command时，重置countreceive的值使其加载方式为Add
                    countreceive = 0;
                }
                else//一致则不变化
                {
                    //此处countreceive不改变值，使其加载方式为subItem[3].Text
                }
            }
            else if (NodeLevel(e.Node).ToString() == "2")
            {
            }
        }

        //得到选中节点的子节点
        private void getCheckedNode(XmlNodeList nodelist)
        {
            try
            {
                foreach (XmlNode xmlnode in nodelist)
                {
                    string tmpname = xmlnode.Attributes["Name"].InnerText;
                    if (tmpname == name)
                    {
                        //此处读取配置文档里的相关值
                        commandid = xmlnode.Attributes["CommandID"].InnerText;
                        if (xmlnode.HasChildNodes)
                        {
                            getCheckedNodeChrildren(xmlnode.ChildNodes);
                        }
                    }
                }
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        //得到选中节点子节点名称向显示属性窗体方法不同写入不同的实参
        private void getCheckedNodeChrildren(XmlNodeList nodelist)
        {
            foreach (XmlNode xmlnode in nodelist)
            {
                if (xmlnode.Name == "Parameters")
                {
                    if (xmlnode.HasChildNodes)
                    {
                        this.ConvertXmlNodeToListView(xmlnode.ChildNodes, listViewEA1);
                    }
                }
                if (xmlnode.Name == "Response")
                {
                    if (xmlnode.HasChildNodes)
                    {
                        this.ConvertXmlNodeToListView(xmlnode.ChildNodes, listViewEA2);
                    }
                }
            }
        }

        //罗列所有的属性
        private void ConvertXmlNodeToListView(XmlNodeList nodelist, ListViewEA listViewEA)
        {
            ListViewItem lvi = new ListViewItem();
            ListViewItem.ListViewSubItem lvsi;
            int i = 0;
            //int tempj = 0;
            foreach (XmlNode xmlNode in nodelist)
            {
                //下边是打算做检查checksum的操作
                //if (xmlNode.ParentNode.Name == "Response")
                //{
                //    cchecksum = xmlNode.ParentNode.Attributes["CChecksum"].InnerText;
                //    cstartindex = xmlNode.ParentNode.Attributes["CStartIndex"].InnerText;
                //    clength = xmlNode.ParentNode.Attributes["CLength"].InnerText; 
                //}
                XmlAttributeCollection col = xmlNode.Attributes;
                //当item的Name为length时，直接读取其值，也就是说，在写配置文档的时候，对于item的Name为length时就不需要其它过多的属性
                if (xmlNode.Attributes["Name"].InnerText == "Length")
                {
                    length = xmlNode.Attributes["Value"].InnerText;
                }
                if (col.Count > 0)
                {
                    lvi = new ListViewItem();
                    foreach (XmlAttribute att in col)
                    {
                        i++;
                        lvsi = new ListViewItem.ListViewSubItem();
                        if (i == 1)
                        {
                            islist = att.Value;
                        }
                        else if (i == 2 && att.Name == "Name")
                        {
                            if (islist == "1")
                            {
                                lvi.Text = att.Value;
                            }
                            else if (islist == "0")
                            {
                                if (att.Value == "Length")
                                { islength = "1"; }
                                else if (att.Value == "Length0")
                                { islength = "0"; }
                                break;
                            }
                        }
                        else if (i == 3 && att.Name == "ItemType")
                        {
                            lvsi.Text = att.Value;
                            lvi.SubItems.Add(lvsi);
                            if (att.Value == "String")
                            {
                                lvi.SubItems.Add("1");
                            }
                            else if (att.Value == "UInt8")
                            {
                                lvi.SubItems.Add("1");
                            }
                            else if (att.Value == "U8_8LE")
                            {
                                lvi.SubItems.Add("2");
                            }
                            else if (att.Value == "U16_Q8")
                            {
                                lvi.SubItems.Add("2");
                            }
                            else if (att.Value == "U11_5LE")
                            {
                                lvi.SubItems.Add("2");
                            }
                            else if (att.Value == "U16_Q5")
                            {
                                lvi.SubItems.Add("2");
                            }
                            else if (att.Value == "U3_13LE")
                            {
                                lvi.SubItems.Add("2");
                            }
                            else if (att.Value == "U16_Q13")
                            {
                                lvi.SubItems.Add("2");
                            }
                            else if (att.Value == "UInt16LE")
                            {
                                lvi.SubItems.Add("2");
                            }
                            else if (att.Value == "Int16LE")
                            {
                                lvi.SubItems.Add("2");
                            }
                            else if (att.Value == "UInt16")
                            {
                                lvi.SubItems.Add("2");
                            }
                            else if (att.Value == "UInt32")
                            {
                                lvi.SubItems.Add("4");
                            }
                            else if (att.Value == "U32_Q25")
                            {
                                lvi.SubItems.Add("4");
                            }
                            else if (att.Value == "U32_Q21")
                            {
                                lvi.SubItems.Add("4");
                            }
                            else if (att.Value == "1")
                            {
                                lvi.SubItems.Add("1");
                            }
                            else if (att.Value == "2")
                            {
                                lvi.SubItems.Add("2");
                            }
                            else if (att.Value == "3")
                            {
                                lvi.SubItems.Add("3");
                            }
                            else if (att.Value == "4")
                            {
                                lvi.SubItems.Add("4");
                            }
                            else if (att.Value == "5")
                            {
                                lvi.SubItems.Add("5");
                            }
                            else if (att.Value == "6")
                            {
                                lvi.SubItems.Add("6");
                            }
                            else if (att.Value == "7")
                            {
                                lvi.SubItems.Add("7");
                            }
                            else if (att.Value == "8")
                            {
                                lvi.SubItems.Add("8");
                            }
                            else if (att.Value == "9")
                            {
                                lvi.SubItems.Add("9");
                            }
                            else if (att.Value == "10")
                            {
                                lvi.SubItems.Add("10");
                            }
                            else if (att.Value == "String_1")
                            {
                                lvi.SubItems.Add("1");
                            }
                            else if (att.Value == "String_2")
                            {
                                lvi.SubItems.Add("2");
                            }
                            else if (att.Value == "String_3")
                            {
                                lvi.SubItems.Add("3");
                            }
                            else if (att.Value == "String_4")
                            {
                                lvi.SubItems.Add("4");
                            }
                            else if (att.Value == "String_5")
                            {
                                lvi.SubItems.Add("5");
                            }
                            else if (att.Value == "String_6")
                            {
                                lvi.SubItems.Add("6");
                            }
                            else if (att.Value == "String_7")
                            {
                                lvi.SubItems.Add("7");
                            }
                            else if (att.Value == "String_8")
                            {
                                lvi.SubItems.Add("8");
                            }
                            else if (att.Value == "String_9")
                            {
                                lvi.SubItems.Add("9");
                            }
                            else if (att.Value == "String_10")
                            {
                                lvi.SubItems.Add("10");
                            }
                            else if (att.Value == "ERROR_3")
                            {
                                lvi.SubItems.Add("3");
                            }
                            else if (att.Value == "ERROR_4")
                            {
                                lvi.SubItems.Add("4");
                            }
                        }
                        else if (i == 4 && att.Name == "Value")
                        {
                            //取出16进制还是10进制
                            //lvsi.Text = Convert.ToString(Convert.ToUInt32(att.Value, 16));
                            lvsi.Text = att.Value;
                            lvi.SubItems.Add(lvsi);
                        }
                        else if (i == 4 && att.Name == "Index")
                        {
                            dataindex[mycountindex] = att.Value;
                        }
                        else if (i == 5 && att.Name == "Coefficient")
                        {
                            xishu[mycountin] = att.Value;
                        }
                        else if (i == 5 && att.Name == "rCoefficient")
                        {
                            rxishu[mycountindex] = att.Value;
                        }
                        else if (i == 6 && att.Name == "Small")
                        {
                            small[mycountin] = att.Value;
                            mycountin++;
                        }
                        else if (i == 6 && att.Name == "rSmall")
                        {
                            rsmall[mycountindex] = att.Value;
                            mycountindex++;
                        }
                    }
                    if (lvi.Text != "")
                    {
                        listViewEA.Items.Add(lvi);
                    }
                    i = 0;
                }
                // test
                //tempj++;
                //tempj = tempj;
            }
        }

        #endregion

        #region [Build生成，刷新，保存，清除------------------------------------]

        //刷新数据
        public void refresh()
        {
            toolStripStatusLabel2.BackColor = Color.Red;

            serialPort1.PortName = spconfig[0];
            serialPort1.BaudRate = Convert.ToInt32(spconfig[1]);
            if (spconfig[2] == "None")
            { serialPort1.Parity = Parity.None; }
            else if (spconfig[2] == "Odd")
            { serialPort1.Parity = Parity.Odd; }
            else if (spconfig[2] == "Even")
            { serialPort1.Parity = Parity.Even; }
            else if (spconfig[2] == "Mark")
            { serialPort1.Parity = Parity.Mark; }
            else if (spconfig[2] == "Space")
            { serialPort1.Parity = Parity.Space; }
            serialPort1.DataBits = Convert.ToInt32(spconfig[3]);
            if (spconfig[4] == "None")
            { serialPort1.StopBits = StopBits.None; }
            else if (spconfig[4] == "One")
            { serialPort1.StopBits = StopBits.One; }
            else if (spconfig[4] == "Two")
            { serialPort1.StopBits = StopBits.Two; }
            else if (spconfig[4] == "OnePointFive")
            { serialPort1.StopBits = StopBits.OnePointFive; }
            serialPort1.ReadTimeout = Convert.ToInt32(spconfig[5]);
            serialPort1.WriteTimeout = Convert.ToInt32(spconfig[6]);
            serialPort1.ReadBufferSize = Convert.ToInt32(spconfig[7]);
            serialPort1.WriteBufferSize = Convert.ToInt32(spconfig[8]);
            this.toolStripStatusLabel1.Text = serialPort1.PortName + " " + "Closed" + "," + spconfig[1] + "," + spconfig[2] + "," + spconfig[3] + "," + spconfig[4];
        }

        //在底部显示系统日期时间以及延迟的处理
        private void timer2_Tick(object sender, EventArgs e)
        {
            this.toolStripStatusLabel5.Text = DateTime.Now.ToString("yyyy-MM-dd dddd HH:mm:ss");
            if (label4.Text.Trim() != "")
            {
                countS++;
                if (countS == 2)
                {
                    label4.Text = " ";
                    countS = 0;
                }
            }
        }

        //Build按钮生成传输数据
        private void button4_Click(object sender, EventArgs e)
        {
            if (label1.Text == "")
            { }
            else
            {
                buildbutton();
            }
            //验证补码单元测试
            //string a = "3333";
            //int b = (256 - Convert.ToInt32(a, 16) % 256) % 256;
            //textBox1.Text = "八位二进制" + Convert.ToString(b, 2).PadLeft(8, '0') + "  " + "两位十六进制" + b.ToString("X2");
        }

        //Build按钮的事件
        private void buildbutton()
        {
            try
            {
                double tempdouble = 0;
                long tempint = 0;
                check = 0;
                mycountin = 0;
                mycountout = 0;
                textBox1.Text = "";
                //获取value*xishu的值并保存
                //当listView中存在元素时
                //计算listView中的元素并保存
                if (listViewEA1.Items.Count > 0)
                {
                    for (mycountin = 0; mycountin < listViewEA1.Items.Count; mycountin++)
                    {
                        tempdouble = Convert.ToDouble(listViewEA1.Items[mycountin].SubItems[3].Text) * Convert.ToDouble(xishu[mycountin]);
                        //tempint = Convert.ToInt32(tempdouble);
                        // 判断如果小于零则求补码
                        if (Convert.ToDouble(listViewEA1.Items[mycountin].SubItems[3].Text) < 0)
                        {
                            tempint = -Convert.ToInt32(tempdouble);
                            tempint = ~tempint;
                            tempint = tempint + 1;
                            if (listViewEA1.Items[mycountin].SubItems[2].Text == "1")
                            {
                                tempint = tempint | 0x80;
                                tempdouble = Convert.ToDouble(Convert.ToInt16(tempint.ToString("X").Substring(14, 2), 16));
                            }
                            else if (listViewEA1.Items[mycountin].SubItems[2].Text == "2")
                            {
                                tempint = tempint | 0x8000;
                                tempdouble = Convert.ToDouble(Convert.ToInt32(tempint.ToString("X").Substring(12, 4), 16));
                            }
                            else if (listViewEA1.Items[mycountin].SubItems[2].Text == "4")
                            {
                                tempint = tempint | 0x80000000;
                                tempdouble = Convert.ToDouble(Convert.ToInt64(tempint.ToString("X").Substring(8, 8), 16));
                            }
                        }
                        //当length为1个字节时
                        if (listViewEA1.Items[mycountin].SubItems[2].Text == "1")//单字节
                        {
                            str[mycountin] = Convert.ToInt32(tempdouble).ToString("X2");//保留小数部分的运算会导致出错
                        }
                        //当length为2个字节时且为小端序
                        else if (listViewEA1.Items[mycountin].SubItems[2].Text == "2" && small[mycountin] == "1")//2字节且小端序
                        {
                            str[mycountin] = Convert.ToInt32(Convert.ToInt32(tempdouble) % 256).ToString("X2") + Convert.ToInt32(Convert.ToInt32(tempdouble) / 256).ToString("X2");//保留小数部分的运算会导致出错
                        }
                        //当length为2个字节时且为大端序
                        else if (listViewEA1.Items[mycountin].SubItems[2].Text == "2" && small[mycountin] == "0")//2字节且大端序
                        {
                            str[mycountin] = Convert.ToInt32(tempdouble).ToString("X4");//保留小数部分的运算会导致出错
                        }
                        //当length为4个字节时且为小端序
                        else if (listViewEA1.Items[mycountin].SubItems[2].Text == "4" && small[mycountin] == "1")//2字节且小端序
                        {
                            str[mycountin] = Convert.ToInt64(Convert.ToInt64(tempdouble) % 256).ToString("X2") + Convert.ToInt64(Convert.ToInt64(tempdouble) % 65536 / 256).ToString("X2") + Convert.ToInt64(Convert.ToInt64(tempdouble) % 16777216 / 65536).ToString("X2") + Convert.ToInt64(Convert.ToInt64(tempdouble) / 16777216).ToString("X2");//保留小数部分的运算会导致出错
                        }


                        // 保存已设置变量的程序段
                        //在foreach循环中每一个元素进行判断后都加一，
                        //当ss大于等于200时，证明mystore不存在符合条件的元素
                        //则将当前操作的item都存入mystore数组，数组的下标在完成操作后自加
                        int ss = 0;
                        foreach (mystruct mysstruct in mystore)
                        {

                            if (mysstruct.commandname == label1.Text)
                            {
                                if (mysstruct.itemname == listViewEA1.Items[mycountin].SubItems[0].Text)
                                {
                                    mystore[ss].itemvalue = listViewEA1.Items[mycountin].SubItems[3].Text;
                                    //这个break对于严谨性很重要，虽然不写也不会影响功能
                                    //不写的话会有很多一样的itemname存在，可能导致混乱
                                    break;
                                }
                            }
                            else
                            {}
                            ss++;
                            if (ss >= 200)
                            {   
                                //存储command
                                mystore[mystoreindex].commandname = label1.Text;
                                //存储item的name
                                mystore[mystoreindex].itemname = listViewEA1.Items[mycountin].SubItems[0].Text;
                                //存储item的value
                                mystore[mystoreindex].itemvalue = listViewEA1.Items[mycountin].SubItems[3].Text;
                                mystoreindex++;
                                if (mystoreindex > 200)
                                {
                                    mystoreindex = 0;
                                }
                            }
                        }
                        //测试
                        //textBox2.Text += mystore[mystorecounter].commandname + " " + mystore[mystorecounter].itemname + " " + mystore[mystorecounter++].itemvalue + "\r\n";
                        //mystorecounter++;
                    }
                }
                else
                { ;}
                //起始字节
                if (start == "")
                { ;}
                else
                {
                    if (ishex == "true")
                    {
                        textBox1.Text = start; 
                    }
                    else if (ishex == "false")
                    {
                        textBox1.Text = toascii(start);
                    }
                }
                //第二第三第四字节
                if (cmi == "1" && li == "2" && pi == "3")
                {
                    textBox1.Text += commandid;//command字节
                    if (islength == "1")
                    {
                        textBox1.Text += length;//长度字节
                    }
                    else
                    { ;}
                    for (mycountout = 0; mycountout < listViewEA1.Items.Count; mycountout++)
                    {
                        //check += Convert.ToInt32(str[mycountout], 16);
                        textBox1.Text += str[mycountout];
                    }
                }
                else if (cmi == "1" && li == "3" && pi == "2")
                {
                    textBox1.Text += commandid;//command字节
                    for (mycountout = 0; mycountout < listViewEA1.Items.Count; mycountout++)
                    {
                        //check += Convert.ToInt32(str[mycountout], 16);
                        //textBox1.Text += Convert.ToInt32(str[mycountout]).ToString("X2");
                        textBox1.Text += str[mycountout];
                    }
                    if (islength == "1")
                    {
                        textBox1.Text += length;//长度字节
                    }
                    else
                    { ; }
                }
                else if (cmi == "2" && li == "3" && pi == "1")
                {
                    for (mycountout = 0; mycountout < listViewEA1.Items.Count; mycountout++)
                    {
                        //check += Convert.ToInt32(str[mycountout], 16);
                        //textBox1.Text += Convert.ToInt32(str[mycountout]).ToString("X2");
                        textBox1.Text += str[mycountout];
                    }
                    textBox1.Text += commandid;//command字节
                    if (islength == "1")
                    {
                        textBox1.Text += length;//长度字节
                    }
                    else
                    { ;}
                }

                else if (cmi == "2" && li == "1" && pi == "3")
                {
                    if (islength == "1")
                    {
                        textBox1.Text += length;//长度字节
                    }
                    else
                    { ;}
                    textBox1.Text += commandid;//command字节
                    for (mycountout = 0; mycountout < listViewEA1.Items.Count; mycountout++)
                    {
                        //check += Convert.ToInt32(str[mycountout], 16);
                        //textBox1.Text += Convert.ToInt32(str[mycountout]).ToString("X2");
                        textBox1.Text += str[mycountout];
                    }
                }
                else if (cmi == "3" && li == "1" && pi == "2")
                {
                    if (islength == "1")
                    {
                        textBox1.Text += length;//长度字节
                    }
                    else
                    { ;}
                    for (mycountout = 0; mycountout < listViewEA1.Items.Count; mycountout++)
                    {
                        //check += Convert.ToInt32(str[mycountout], 16);
                        //textBox1.Text += Convert.ToInt32(str[mycountout]).ToString("X2");
                        textBox1.Text += str[mycountout];
                    }
                    textBox1.Text += commandid;//command字节
                }
                else if (cmi == "3" && li == "2" && pi == "1")
                {
                    for (mycountout = 0; mycountout < listViewEA1.Items.Count; mycountout++)
                    {
                        //check += Convert.ToInt32(str[mycountout], 16);
                        //textBox1.Text += Convert.ToInt32(str[mycountout]).ToString("X2");
                        textBox1.Text += str[mycountout];
                    }
                    if (islength == "1")
                    {
                        textBox1.Text += length;//长度字节
                    }
                    else
                    { ;}
                    textBox1.Text += commandid;//command字节
                }
                //checksum字节进行的判断
                if (checkstyle == "None")
                { ;}
                else
                {

                    if (checkstyle == "Complement")
                    {   
                        //check += Convert.ToInt32(commandid, 16);//需要计算的值就转化为十进制加进去
                        //check += Convert.ToInt32(length, 16);//需要计算的值
                        //累加Command
                        for (int cc = 0; cc < commandid.Length / 2; cc++)
                        {
                            check += Convert.ToInt32(commandid.Substring(cc * 2, 2), 16);
                        }
                        //累加length                        
                        if (islength == "1")
                        {
                            for (int ll = 0; ll < length.Length / 2; ll++)
                            {
                                check += Convert.ToInt32(length.Substring(ll * 2, 2), 16);
                            }
                        }
                        //累加变量
                        for (mycountout = 0; mycountout < listViewEA1.Items.Count; mycountout++)
                        {
                            for (int i = 0; i < (str[mycountout].Length / 2); i++)
                            {
                                check += Convert.ToInt32(str[mycountout].Substring(i * 2, 2), 16);
                            }
                            //textBox1.Text += str[mycountout];
                        }
                        checksum = ((256 - check % 256) % 256).ToString("X2");
                    }
                    else if (checkstyle == "Add")
                    {   
                        //check += Convert.ToInt32(commandid, 16);//需要计算的值就转化为十进制加进去
                        //check += Convert.ToInt32(length, 16);//需要计算的值
                        for (int cc = 0; cc < commandid.Length / 2; cc++)
                        {
                            check += Convert.ToInt32(commandid.Substring(cc * 2, 2), 16);
                        }
                        if (islength == "1")
                        {
                            for (int ll = 0; ll < length.Length / 2; ll++)
                            {
                                check += Convert.ToInt32(length.Substring(ll * 2, 2), 16);
                            }
                        }
                        for (mycountout = 0; mycountout < listViewEA1.Items.Count; mycountout++)
                        {
                            for (int i = 0; i < (str[mycountout].Length / 2); i++)
                            {
                                check += Convert.ToInt32(str[mycountout].Substring(i * 2, 2), 16);
                            }
                            //textBox1.Text += str[mycountout];
                        }
                        checksum = (check % 256).ToString("X2");
                    }
                    else if (checkstyle == "CRC16_1")
                    {
                        ushort i;
                        if (CRCType == 1)
                        {
                            CRC16 = Back16CRC;
                        }
                        else
                        {
                            CRC16 = Front16CRC;
                        }
                        ushort temp;
                        ushort temp2;
                        ushort[] CurrentDataByte = new ushort[textBox1.Text.Length / 2 - 1];
                        for (int cc = 0; cc < CurrentDataByte.Length; cc++)
                        {
                            CurrentDataByte[cc] += Convert.ToUInt16(textBox1.Text.Substring((cc + 1) * 2, 2), 16);
                        }
                        for (i = 0; i < CurrentDataByte.Length; i++)
                        {
                            // Since the CRC is the remainder of modulo 2 division, we lookup the
                            // pre-computed remainder value from the table.
                            // CRC = Table[(CRC>>8) xor Data] xor (CRC<<8)
                            temp = Convert.ToUInt16(((CRC16 >> 8) ^ CurrentDataByte[i]) & 0xFF);
                            temp2 = Convert.ToUInt16((Convert.ToString((CRC16 << 8), 2).PadLeft(24, '0').Substring(8, 16)), 2);
                            CRC16 = Convert.ToUInt16(CRC16LookupTable[temp] ^ (temp2));
                        }
                        checksum = CRC16.ToString("X4");
                    }
                    else if (checkstyle == "CRC8_1")
                    {
                        byte i;
                        if (CRCType == 1)
                        {
                            CRC8 = Back8CRC;
                        }
                        else
                        {
                            CRC8 = Front8CRC;
                        }
                        int length;
                        length = textBox1.Text.Length / 2;
                        for (i = 0; i < length; i++)
                        {
                            CRC8 = CRC8_Process(Convert.ToByte(Convert.ToInt16(textBox1.Text.Substring(i * 2, 2), 16)), CRC8);
                        }
                        checksum = CRC8.ToString("X2");
                    }
                    else if (checkstyle == "CRC8_2")
                    {
                        byte i;
                        if (CRCType == 1)
                        {
                            CRC8 = Back8CRC;
                        }
                        else
                        {
                            CRC8 = Front8CRC;
                        }
                        int length;
                        length = textBox1.Text.Length / 2;
                        for (i = 1; i < length; i++)
                        {
                            CRC8 = CRC8_Process(Convert.ToByte(Convert.ToInt16(textBox1.Text.Substring(i * 2, 2), 16)), CRC8);
                        }
                        checksum = CRC8.ToString("X2");
                    }
                    else if (checkstyle == "CRC8_3")
                    {
                        byte i;
                        if (CRCType == 1)
                        {
                            CRC8 = Back8CRC;
                        }
                        else
                        {
                            CRC8 = Front8CRC;
                        }
                        int length;
                        length = textBox1.Text.Length / 2;
                        for (i = 0; i < length; i++)
                        {
                            CRC8 = CRC8_3_Process(Convert.ToByte(Convert.ToInt16(textBox1.Text.Substring(i * 2, 2), 16)), CRC8);
                        }
                        checksum = CRC8.ToString("X2");
                    }
                    else if (checkstyle == "CRC8_4") // eBus 2400 and
                    {
                        byte i;
                        if (CRCType == 1)
                        {
                            CRC8 = Back8CRC;
                        }
                        else
                        {
                            CRC8 = Front8CRC;
                        }
                        int length;
                        length = textBox1.Text.Length / 2;
                        for (i = 1; i < length; i++)
                        {
                            CRC8 = CRC8_3_Process(Convert.ToByte(Convert.ToInt16(textBox1.Text.Substring(i * 2, 2), 16)), CRC8);
                        }
                        checksum = CRC8.ToString("X2");
                    }
                    textBox1.Text += checksum;
                }
                if (stop == "")
                { ;}
                else
                {
                    if (ishex == "true")
                    {
                        textBox1.Text += stop;
                    }
                    else if (ishex == "false")
                    {
                        textBox1.Text += toascii(stop);
                    }
                }
            }
            catch (Exception)
            {
                //MessageBox.Show("Build Error :" + ex.Message);
            }
        }

        //CRC8_1计算
        private byte CRC8_Process(byte Data, byte OldCRC8)
        {
            return (CRC8LookupTable[Data ^ OldCRC8]);
        }

        //CRC8_3计算
        private byte CRC8_3_Process(byte Data, byte OldCRC8)
        {
            return (Convert.ToByte(CRC8LookupTable[OldCRC8]^ Data));
        }

        //十六进制到ASCII
        private string toascii(string x)
        {
            int iValue;
            string sValue;
            iValue = Convert.ToInt32(x, 16); // 16进制->10进制
            sValue = System.Text.Encoding.ASCII.GetString(System.BitConverter.GetBytes(iValue));  //Int-> ASCII 
            sValue = sValue.Substring(0, 1);
            //textBox3.Text = sValue;
            //textBox3.Text += "\r";
            return sValue;
        }
        
        //Clear按钮清空textBox2和状态栏记数
        private void button6_Click(object sender, EventArgs e)
        {
            textBox2.Clear();
            toolStripStatusLabel3.Text = "|  Commands: 0　　";
            toolStripStatusLabel4.Text = "Responses: 0  |";
            countC = 0;
            countD = 0;
        }

        //Clear按钮清空textBox3
        private void button8_Click(object sender, EventArgs e)
        {
            textBox3.Text = "";
        }

        //Receive按钮打算写成接受数据处理的程序，目前酝酿中
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                //textBox2.Text = Convert.ToInt32("100",16).ToString("X4");
                textBox2.Text = (256 - Convert.ToInt32("121",16) % 256).ToString("X2"); ;
                //string[] ports = serialPort1.GetPortNames();
                //textBox2.Text = Convert.ToInt32(textBox3.Text.Substring(textBox3.Text.LastIndexOf("x") + 1), 16).ToString("X2");
                //path = Application.StartupPath;
                //newpath = path + "\\" + "\\Records";
                //textBox2.Text = newpath + "\\R" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".txt";
            }
            catch (Exception)
            {
                //MessageBox.Show("程序错误:" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Save按钮保存textBox2中的数据
        private void button7_Click(object sender, EventArgs e)
        {   saved_event();
            //textBox2.Text += "\r\n" + Application.StartupPath;
            //textBox2.Text += "\r\n" + this.GetType().Assembly.Location;
            //textBox2.Text += "\r\n" + System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            //textBox2.Text += "\r\n" + System.Environment.CurrentDirectory;
            //textBox2.Text += "\r\n" + System.AppDomain.CurrentDomain.BaseDirectory;//这个好
            //textBox2.Text += "\r\n" + System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        }

        private void saved_event()
        {
            if (textBox2.Text != "")
            {
                countS = 0;
                if (Directory.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "Records"))
                {
                    path = System.AppDomain.CurrentDomain.BaseDirectory;
                    newpath = path + "Records";
                    string temppath = newpath + "\\R_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";
                    StreamWriter sw = new StreamWriter(@temppath, true, Encoding.UTF8);
                    sw.Write(this.textBox2.Text);
                    sw.Close();
                }
                else
                {
                    path = System.AppDomain.CurrentDomain.BaseDirectory;
                    newpath = path + "Records";
                    Directory.CreateDirectory(newpath);
                    string temppath = newpath + "\\R_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";
                    StreamWriter sw = new StreamWriter(@temppath, true, Encoding.UTF8);
                    sw.Write(this.textBox2.Text);
                    sw.Close();
                }
                label4.Text = "Data saved!";
            }
        }

        //Where按钮打开Records目录
        private void button5_Click(object sender, EventArgs e)
        {
            path = System.AppDomain.CurrentDomain.BaseDirectory;
            newpath = path + "Records";
            System.Diagnostics.Process.Start("explorer.exe", newpath);
        }

        #endregion

        #region [打开串口时进行配置串口参数，关闭串口---------------------------]

        //打开串口时加载参数，关闭串口
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            spopen();
        }

        private void spopen()
        {
            try
            {
                spcount++;
                //加载参数
                if (spcount == 1)
                {
                    serialPort1.PortName = spconfig[0];
                    serialPort1.BaudRate = Convert.ToInt32(spconfig[1]);
                    if (spconfig[2] == "None")
                    { serialPort1.Parity = Parity.None; }
                    else if (spconfig[2] == "Odd")
                    { serialPort1.Parity = Parity.Odd; }
                    else if (spconfig[2] == "Even")
                    { serialPort1.Parity = Parity.Even; }
                    else if (spconfig[2] == "Mark")
                    { serialPort1.Parity = Parity.Mark; }
                    else if (spconfig[2] == "Space")
                    { serialPort1.Parity = Parity.Space; }
                    serialPort1.DataBits = Convert.ToInt32(spconfig[3]);
                    if (spconfig[4] == "None")
                    { serialPort1.StopBits = StopBits.None; }
                    else if (spconfig[4] == "One")
                    { serialPort1.StopBits = StopBits.One; }
                    else if (spconfig[4] == "Two")
                    { serialPort1.StopBits = StopBits.Two; }
                    else if (spconfig[4] == "OnePointFive")
                    { serialPort1.StopBits = StopBits.OnePointFive; }
                    serialPort1.ReadTimeout = Convert.ToInt32(spconfig[5]);
                    serialPort1.WriteTimeout = Convert.ToInt32(spconfig[6]);
                    serialPort1.ReadBufferSize = Convert.ToInt32(spconfig[7]);
                    serialPort1.WriteBufferSize = Convert.ToInt32(spconfig[8]);
                }
                if (toolStripButton1.Text == "Open  SP" || toolStripButton1.Text == "打开串口")
                {
                    //打开串口
                    serialPort1.Open();
                    //清理输入缓冲区
                    //serialPort1.DiscardInBuffer();
                    //serialPort1.DiscardOutBuffer();
                    toolStripStatusLabel2.BackColor = Color.Green;
                    toolStripStatusLabel1.Text = serialPort1.PortName + " " + "Opened" + "," + serialPort1.BaudRate + "," + serialPort1.Parity + "," + serialPort1.DataBits + "," + serialPort1.StopBits;
                    if (languageflag == (int)Language.English)
                    {
                        toolStripButton1.Text = "Close SP";
                    }
                    else if (languageflag == (int)Language.Chinese)
                    {
                        toolStripButton1.Text = "关闭串口";
                    }
                    toolStripMenuItem21.Enabled = false;
                }
                else
                {
                    //清理输入缓冲区
                    serialPort1.DiscardInBuffer();
                    serialPort1.DiscardOutBuffer();
                    //防止死锁的处理
                    int i = Environment.TickCount;
                    while (Environment.TickCount - i < 2000 && _isReceiving)
                    {
                        Application.DoEvents();
                    }
                    //关闭串口
                    serialPort1.Close();//现在没有死锁了，关闭串口
                    toolStripStatusLabel2.BackColor = Color.Red;
                    toolStripStatusLabel1.Text = serialPort1.PortName + " " + "Closed" + "," + serialPort1.BaudRate + "," + serialPort1.Parity + "," + serialPort1.DataBits + "," + serialPort1.StopBits;
                    if (languageflag == (int)Language.English)
                    {
                        toolStripButton1.Text = "Open  SP";
                    }
                    else if (languageflag == (int)Language.Chinese)
                    {
                        toolStripButton1.Text = "打开串口";
                    }
                    toolStripMenuItem21.Enabled = true;
                }
            }
            catch (Exception)
            {
                //MessageBox.Show("程序错误:" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //清理输入缓冲区
                //serialPort1.DiscardInBuffer();
                //防止死锁的处理
                int i = Environment.TickCount;
                while (Environment.TickCount - i < 2000 && _isReceiving)
                {
                    Application.DoEvents();
                }
                //关闭串口
                serialPort1.Close();//现在没有死锁了，关闭串口
                toolStripStatusLabel2.BackColor = Color.Red;
                toolStripStatusLabel1.Text = serialPort1.PortName + " " + "Closed" + "," + serialPort1.BaudRate + "," + serialPort1.Parity + "," + serialPort1.DataBits + "," + serialPort1.StopBits;
                if (languageflag == (int)Language.English)
                {
                    toolStripButton1.Text = "Open  SP";
                }
                else if (languageflag == (int)Language.Chinese)
                {
                    toolStripButton1.Text = "打开串口";
                }
                toolStripMenuItem21.Enabled = true;
            }
        }

        #endregion

        #region [退出窗体，串口被关闭，及最小化到托盘---------------------------]

        //单击Close Structure按钮关闭目前显示的文档
        private void toolStripMenuItem13_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            listViewEA1.Items.Clear();
            listViewEA2.Items.Clear();
            this.Text = "ESP";
        }

        //单击file菜单下Exit按钮退出程序
        private void toolStripMenuItem14_Click(object sender, EventArgs e)
        {

            notifyIcon1.Visible = false;
            this.Dispose();
            System.Environment.Exit(0);

            //DialogResult result = MessageBox.Show("Are you sure?", "Exit", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            //if (result == DialogResult.OK)
            //{
            //    notifyIcon1.Visible = false;
            //    this.Dispose();
            //    System.Environment.Exit(0);
            //}
            //else
            //{ }

        }

        //单击关闭按钮退出程序
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //取消窗体关闭事件
            e.Cancel = true;
            //最小化窗口
            this.WindowState = FormWindowState.Minimized;
            //在Windows任务栏中不显示窗体
            this.ShowInTaskbar = false;
            notifyIcon1.Visible = true;
            first++;
            if (first == 1)
            {
                this.notifyIcon1.ShowBalloonTip(1, "托盘提示(单击使气球消失)", "双击图标还原程序，右键单击图标显示菜单", ToolTipIcon.Info);
            }

            //点击关闭按钮弹出messagebox
            //DialogResult result = MessageBox.Show("Are you sure?", "Exit", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            //if (result == DialogResult.OK)
            //{
                //Application.ExitThread();
            //    this.Dispose();
            //    System.Environment.Exit(0);
            //}
            //else
            //{
            //    e.Cancel = true;
            //}
        }

        //窗体最小化到任务栏控件的双击事件
        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            //判断窗体是不是最小化
            if (this.WindowState == FormWindowState.Minimized)   
            {
                //如果是最小化就把窗体状态改为默认大小
                this.WindowState = FormWindowState.Normal;  
            }
            //激活窗体并赋予焦点，这句话可以不写
            //this.Activate();
            //在Windows任务栏中显示窗体
            this.ShowInTaskbar = true;
            notifyIcon1.Visible = false;
        }

        private void toolStripMenuItem01_Click(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
            //判断窗体是不是最小化
            if (this.WindowState == FormWindowState.Minimized)
            {
                //如果是最小化就把窗体状态改为默认大小
                this.WindowState = FormWindowState.Normal;
            }
            //激活窗体并赋予焦点，这句话可以不写
            this.Activate();
            //在Windows任务栏中显示窗体
            this.ShowInTaskbar = true;
        }

        private void toolStripMenuItem021_Click(object sender, EventArgs e)
        {
            if (debug1.isshow == false)
            {
                debug1.Show();
            }
            else if (debug2.isshow == true)
            {
                debug1.Visible = true;
                debug1.WindowState = FormWindowState.Normal;
                debug1.Activate();
            }
        }

        private void toolStripMenuItem022_Click(object sender, EventArgs e)
        {
            if (debug2.isshow == false)
            {
                debug2.Show();
            }
            else if (debug2.isshow == true)
            {
                debug2.Visible = true;
                debug2.WindowState = FormWindowState.Normal;
                debug2.Activate();
            }
        }

        private void toolStripMenuItem031_Click(object sender, EventArgs e)
        {
            if (demo1.isshow == false)
            {
                demo1.Show();
            }
            else if (demo1.isshow == true)
            {
                demo1.Visible = true;
                demo1.WindowState = FormWindowState.Normal;
                demo1.Activate();
            }
        }

        private void toolStripMenuItem032_Click(object sender, EventArgs e)
        {
            if (demo2.isshow == false)
            {
                demo2.Show();
            }
            else if (demo2.isshow == true)
            {
                demo2.Visible = true;
                demo2.WindowState = FormWindowState.Normal;
                demo2.Activate();
            }
        }

        private void toolStripMenuItem05_Click(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
            this.Dispose();
            System.Environment.Exit(0);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            notifyIcon1.Visible = false;
        }
        //取消掉串口拔出自动关闭，因为并没有解决串口死机的问题
#if false
        //重写窗体事件，实现当检测到串口异常时也能反馈到系统，提示关闭
        protected override void WndProc(ref Message m)
        {
        
            if (m.Msg == 0x0219)
            {
                //串口被拔出 
                if (m.WParam.ToInt32() == 0x8004)
                {
                    MessageBox.Show("串口断开！系统将关闭！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    System.Threading.Thread.CurrentThread.Abort();
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
            }
            base.WndProc(ref m);
        }
#endif
        //重写窗体事件，实现当检测到串口异常，停止发送提示错误
        protected override void WndProc(ref Message m)
        {

            if (m.Msg == 0x0219)
            {
                //串口被拔出 
                if (m.WParam.ToInt32() == 0x8004)
                {
                    //MessageBox.Show("串口断开！系统将关闭！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    groupBox6.Enabled = true;
                    toolStripEA1.Enabled = true;
                    timer3.Enabled = false;
                    if (languageflag == (int)Language.English)
                    {
                        button3.Text = "Send";
                    }
                    else if (languageflag == (int)Language.Chinese)
                    {
                        button3.Text = "发送";
                    }
                    checkBox4.Enabled = true;
                    numericUpDown2.Enabled = true;
                    groupBox4.Enabled = true;
                    toolStripEA1.Enabled = true;
                    timer1.Enabled = false;
                    if (languageflag == (int)Language.English)
                    {
                        button1.Text = "Send";
                    }
                    else if (languageflag == (int)Language.Chinese)
                    {
                        button1.Text = "发送";
                    }
                    checkBox2.Enabled = true;
                    numericUpDown1.Enabled = true;
                }
            }
            base.WndProc(ref m);
        }
        #endregion

        #region [最近打开文档的按钮相关处理-------------------------------------]

        //最近打开文档的按钮的显示与隐藏
        private void toolStripMenuItem121_TextChanged(object sender, EventArgs e)
        {
            if (toolStripMenuItem121.Text == "")
            {
                toolStripMenuItem121.Visible = false;
                toolStripSeparator7.Visible = false;
                toolStripMenuItem126.Enabled = false;
            }
            else
            {
                toolStripMenuItem121.Visible = true;
                toolStripSeparator7.Visible = true;
                toolStripMenuItem126.Enabled = true;
            }
        }

        private void toolStripMenuItem122_TextChanged(object sender, EventArgs e)
        {
            if (toolStripMenuItem122.Text == "")
            {
                toolStripMenuItem122.Visible = false;
            }
            else
            {
                toolStripMenuItem122.Visible = true;
            }
        }

        private void toolStripMenuItem123_TextChanged(object sender, EventArgs e)
        {
            if (toolStripMenuItem123.Text == "")
            {
                toolStripMenuItem123.Visible = false;
            }
            else
            {
                toolStripMenuItem123.Visible = true;
            }
        }

        private void toolStripMenuItem124_TextChanged(object sender, EventArgs e)
        {
            if (toolStripMenuItem124.Text == "")
            {
                toolStripMenuItem124.Visible = false;
            }
            else
            {
                toolStripMenuItem124.Visible = true;
            }
        }

        private void toolStripMenuItem125_TextChanged(object sender, EventArgs e)
        {
            if (toolStripMenuItem125.Text == "")
            {
                toolStripMenuItem125.Visible = false;
            }
            else
            {
                toolStripMenuItem125.Visible = true;
            }
        }

        //最近打开文档按钮的click事件
        private void toolStripMenuItem121_Click(object sender, EventArgs e)
        {
            try
            {
                //adress = toolStripMenuItem121.Text;
                this.LoadTreeNodes(toolStripMenuItem121.Text);
            }
            catch (Exception)
            {
                //MessageBox.Show("打开失败：" + ex.Message);
                toolStripMenuItem121.Text = toolStripMenuItem122.Text;
                toolStripMenuItem122.Text = toolStripMenuItem123.Text;
                toolStripMenuItem123.Text = toolStripMenuItem124.Text;
                toolStripMenuItem124.Text = toolStripMenuItem125.Text;
                toolStripMenuItem125.Text = "";
                this.Text = "ESP";
            }
        }

        private void toolStripMenuItem122_Click(object sender, EventArgs e)
        {
            try
            {
                //adress = toolStripMenuItem122.Text;
                this.LoadTreeNodes(toolStripMenuItem122.Text);
                //string filename = toolStripMenuItem122.Text.Substring(toolStripMenuItem122.Text.LastIndexOf("\\") + 1);
                //string sstring = toolStripMenuItem122.Text;
                //toolStripMenuItem122.Text = toolStripMenuItem121.Text;
                //toolStripMenuItem121.Text = sstring;
            }
            catch (Exception)
            {
                //MessageBox.Show("打开失败：" + ex.Message);
                toolStripMenuItem122.Text = toolStripMenuItem123.Text;
                toolStripMenuItem123.Text = toolStripMenuItem124.Text;
                toolStripMenuItem124.Text = toolStripMenuItem125.Text;
                toolStripMenuItem125.Text = "";
                this.Text = "ESP";
            }
        }

        private void toolStripMenuItem123_Click(object sender, EventArgs e)
        {
            try
            {
                //adress = toolStripMenuItem123.Text;
                this.LoadTreeNodes(toolStripMenuItem123.Text);
                //string filename = toolStripMenuItem123.Text.Substring(toolStripMenuItem123.Text.LastIndexOf("\\") + 1);
                //string sstring = toolStripMenuItem123.Text;
                //toolStripMenuItem123.Text = toolStripMenuItem122.Text;
                //toolStripMenuItem122.Text = toolStripMenuItem121.Text;
                //toolStripMenuItem121.Text = sstring;
            }
            catch (Exception)
            {
                //MessageBox.Show("打开失败：" + ex.Message);
                toolStripMenuItem123.Text = toolStripMenuItem124.Text;
                toolStripMenuItem124.Text = toolStripMenuItem125.Text;
                toolStripMenuItem125.Text = "";
                this.Text = "ESP";
            }
        }

        private void toolStripMenuItem124_Click(object sender, EventArgs e)
        {
            try
            {
                //adress = toolStripMenuItem124.Text;
                this.LoadTreeNodes(toolStripMenuItem124.Text);
                //string filename = toolStripMenuItem124.Text.Substring(toolStripMenuItem124.Text.LastIndexOf("\\") + 1);
                //string sstring = toolStripMenuItem124.Text;
                ////toolStripMenuItem124.Text = toolStripMenuItem123.Text;
                //toolStripMenuItem123.Text = toolStripMenuItem122.Text;
                //toolStripMenuItem122.Text = toolStripMenuItem121.Text;
                //toolStripMenuItem121.Text = sstring;
            }
            catch (Exception)
            {
                //MessageBox.Show("打开失败：" + ex.Message);
                toolStripMenuItem124.Text = toolStripMenuItem125.Text;
                toolStripMenuItem125.Text = "";
                this.Text = "ESP";
            }
        }

        private void toolStripMenuItem125_Click(object sender, EventArgs e)
        {
            try
            {
                //adress = toolStripMenuItem125.Text;
                this.LoadTreeNodes(toolStripMenuItem125.Text);
                //string filename = toolStripMenuItem125.Text.Substring(toolStripMenuItem125.Text.LastIndexOf("\\") + 1);
                //this.Text = "ESP" + " - " + filename;
                //string sstring = toolStripMenuItem125.Text;
                //toolStripMenuItem125.Text = toolStripMenuItem124.Text;
                //toolStripMenuItem124.Text = toolStripMenuItem123.Text;
                //toolStripMenuItem123.Text = toolStripMenuItem122.Text;
                //toolStripMenuItem122.Text = toolStripMenuItem121.Text;
                //toolStripMenuItem121.Text = sstring;
            }
            catch (Exception)
            {
                //MessageBox.Show("打开失败：" + ex.Message);
                toolStripMenuItem125.Text = "";
                this.Text = "ESP";
            }
        }

        //清除最近保存的记录
        private void toolStripMenuItem126_Click(object sender, EventArgs e)
        {
            toolStripMenuItem125.Text = "";
            toolStripMenuItem124.Text = "";
            toolStripMenuItem123.Text = "";
            toolStripMenuItem122.Text = "";
            toolStripMenuItem121.Text = "";
            XmlDocument deletefile = new XmlDocument();
            deletefile.Load(Application.StartupPath + @"\ESPConfig.cds");
            deletefile.SelectSingleNode("Config/MRU/Item_4").InnerText = "";
            deletefile.SelectSingleNode("Config/MRU/Item_3").InnerText = "";
            deletefile.SelectSingleNode("Config/MRU/Item_2").InnerText = "";
            deletefile.SelectSingleNode("Config/MRU/Item_1").InnerText = "";
            deletefile.SelectSingleNode("Config/MRU/Item_0").InnerText = "";
            deletefile.Save(Application.StartupPath + @"\ESPConfig.cds");
        }

        #endregion

        #region [关于帮助以及配置文档-------------------------------------------]
        //创建ESPConfig配置文档
        private void toolStripMenuItem531_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region [打开小工具，配置串口，打开demo窗口以及关闭按钮-----------------]

        //主窗体的头标题变化的事件
        private void Form1_TextChanged(object sender, EventArgs e)
        {
            if (this.Text == "ESP")
            {
                toolStripMenuItem13.Enabled = false;
            }
            else
            {
                toolStripMenuItem13.Enabled = true;
            }
        }

        //关于窗体
        private void toolStripMenuItem51_Click(object sender, EventArgs e)
        {
            About ecoviabout = new About();
            ecoviabout.ShowDialog();
        }

        //设置窗体置顶和取消置顶
        private void toolStripMenuItem23_Click(object sender, EventArgs e)
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

        //打开串口配置界面
        private void toolStripMenuItem21_Click(object sender, EventArgs e)
        {
            //serialportoption窗体对象
            SerialPortOption spoption = new SerialPortOption(this);//带参数用来更改Form1窗体的信息
            spcount = 0;
            spoption.ShowDialog(this);
        }

        //打开定时计时器工具
        private void toolStripMenuItem22_Click(object sender, EventArgs e)
        {
            if (time.isshow == false)
            {
                time.Show();
            }
            else if (time.isshow == true)
            {
                time.Visible = true;
                time.WindowState = FormWindowState.Normal;
                time.Activate();
            }
        }

        //调试模式（Debug）的Air condition按钮单击事件
        private void toolStripMenuItem31_Click(object sender, EventArgs e)
        {
            if (debug1.isshow == false)
            {
                debug1.Show();
            }
            else if (debug1.isshow == true)
            {
                debug1.Visible = true;
                debug1.WindowState = FormWindowState.Normal;
                debug1.Activate();
            }
        }

        //调试模式（Debug）的Washing machine按钮单击事件
        private void toolStripMenuItem32_Click(object sender, EventArgs e)
        {
            if (debug2.isshow == false)
            {
                debug2.Show();
                serialPort1.Close();
                toolStripStatusLabel2.BackColor = Color.Red;
                toolStripStatusLabel1.Text = serialPort1.PortName + " " + "Closed" + "," + serialPort1.BaudRate + "," + serialPort1.Parity + "," + serialPort1.DataBits + "," + serialPort1.StopBits;
                if (languageflag == (int)Language.English)
                {
                    toolStripButton1.Text = "Open  SP";
                }
                else if (languageflag == (int)Language.Chinese)
                {
                    toolStripButton1.Text = "打开串口";
                }
                toolStripMenuItem21.Enabled = true;
                this.Close();
            }
            else if (debug2.isshow == true)
            {
                serialPort1.Close();
                toolStripStatusLabel2.BackColor = Color.Red; 
                toolStripStatusLabel1.Text = serialPort1.PortName + " " + "Closed" + "," + serialPort1.BaudRate + "," + serialPort1.Parity + "," + serialPort1.DataBits + "," + serialPort1.StopBits;
                if (languageflag == (int)Language.English)
                {
                    toolStripButton1.Text = "Open  SP";
                }
                else if (languageflag == (int)Language.Chinese)
                {
                    toolStripButton1.Text = "打开串口";
                }
                toolStripMenuItem21.Enabled = true;
                this.Close();
                debug2.Visible = true;
                debug2.WindowState = FormWindowState.Normal;
                debug2.Activate();
            }
        }

        //演示模式（Demo）的Air condition按钮单击事件
        private void toolStripMenuItem41_Click(object sender, EventArgs e)
        {
            if (demo1.isshow == false)
            {
                demo1.Show();
            }
            else if (demo1.isshow == true)
            {
                demo1.Visible = true;
                demo1.WindowState = FormWindowState.Normal;
                demo1.Activate();
            }
        }

        //演示模式（Demo）的Washing machine按钮单击事件
        private void toolStripMenuItem42_Click(object sender, EventArgs e)
        {
            if (demo2.isshow == false)
            {
                demo2.Show();
            }
            else if (demo2.isshow == true)
            {
                demo2.Visible = true;
                demo2.WindowState = FormWindowState.Normal;
                demo2.Activate();
            }
        }

        //单独运算校验和的工具
        private void toolStripMenuItem23_Click_1(object sender, EventArgs e)
        {
            if (CKS.isshow == false)
            {
                CKS.Show();
            }
            else if (CKS.isshow == true)
            {
                CKS.Visible = true;
                CKS.WindowState = FormWindowState.Normal;
                CKS.Show();
            }
        }

        //示波器界面
        private void toolStripMenuItem24_Click(object sender, EventArgs e)
        {
            if (osc.isshow == false)
            {
                osc.Show();
            }
            else if (osc.isshow == true)
            {
                osc.Visible = true;
                osc.WindowState = FormWindowState.Normal;
                osc.Show();
            }
        }

        #endregion

        #region [语言选择-------------------------------------------------------]

        // 处理语言选择的事件
        // 选择英文
        private void toolStripMenuItem61_Click(object sender, EventArgs e)
        {
            setSelectedLanguage((int)Language.English);
        }
        // 选择中文
        private void toolStripMenuItem62_Click(object sender, EventArgs e)
        {
            setSelectedLanguage((int)Language.Chinese);
        }
        // 调用语言及保存选择的语言
        private void setSelectedLanguage(int mylanguage)
        {
            languageflag = mylanguage;
            try
            {
                // 语言按钮的选择的控制
                switch (mylanguage)
                {
                    case (int)Language.English:
                        toolStripMenuItem61.Checked = true;
                        toolStripMenuItem62.Checked = false;
                        break;
                    case (int)Language.Chinese:
                        toolStripMenuItem61.Checked = false;
                        toolStripMenuItem62.Checked = true;
                        break;
                    default:
                        break;
                }

                // 在相关窗体中执行语言转化
                this.debug2.Debug2_Language_Selected(mylanguage);
                this.Form1_Language_Selected(mylanguage);

                // 存储选择的语言
                if (File.Exists(Application.StartupPath + @"\ESPConfig.cds"))
                {
                    // 读入XML文档
                    XmlDocument cdsxml = new XmlDocument();
                    cdsxml.Load(Application.StartupPath + @"\ESPConfig.cds");
                    XmlNode cdslanguage = cdsxml.SelectSingleNode("Config/Language");
                    if (cdslanguage != null)
                    {
                        cdslanguage.InnerText = mylanguage.ToString();
                        cdsxml.Save(Application.StartupPath + @"\ESPConfig.cds");
                    }
                    else
                    {
                        buildConfigurationFile();
                    }
                }
            }
            catch (Exception)
            { }
        }

        //
        //主界面字体变更
        //
        public void Form1_Language_Selected(int mylanguage)
        {
            switch (mylanguage)
            {
                case (int)Form1.Language.English:
                    if (this.toolStripButton1.Text == "打开串口")
                    {
                        this.toolStripButton1.Text = "Open  SP";
                    }
                    else if (this.toolStripButton1.Text == "关闭串口")
                    {
                        this.toolStripButton1.Text = "Close SP";
                    }
                    if (this.button3.Text == "发送")
                    {
                        this.button3.Text = "Send";
                    }
                    else if (this.button3.Text == "停止")
                    {
                        this.button3.Text = "Stop";
                    }
                    if (this.button1.Text == "发送")
                    {
                        this.button1.Text = "Send";
                    }
                    else if (this.button1.Text == "停止")
                    {
                        this.button1.Text = "Stop";
                    }
                    this.toolStripDropDownButton1.Text = "&File";
                    this.toolStripMenuItem11.Text = "&Open Structure";
                    this.toolStripMenuItem12.Text = "&Reopen Structure";
                    this.toolStripMenuItem126.Text = "Clear &History";
                    this.toolStripMenuItem13.Text = "&Close Structure";
                    this.toolStripMenuItem14.Text = "&Exit";
                    this.toolStripDropDownButton2.Text = "&Tools";
                    this.toolStripMenuItem21.Text = "&SerialPort Configuration";
                    this.toolStripMenuItem22.Text = "&Clock/Timer";
                    this.toolStripMenuItem23.Text = "Chec&ksum and ASCII";
                    this.toolStripMenuItem24.Text = "OSC";
                    this.toolStripMenuItem29.Text = "&ToTop";
                    this.toolStripDropDownButton3.Text = "De&bug Mode";
                    this.toolStripMenuItem31.Text = "&Air condition";
                    this.toolStripMenuItem32.Text = "&Washing machine";
                    this.toolStripDropDownButton4.Text = "De&mo Mode";
                    this.toolStripMenuItem41.Text = "&Air condition";
                    this.toolStripMenuItem42.Text = "&Washing machine";
                    this.toolStripDropDownButton5.Text = "&Help";
                    this.toolStripMenuItem51.Text = "&About";
                    this.toolStripMenuItem52.Text = "Help";
                    this.toolStripMenuItem53.Text = "Create";
                    this.toolStripMenuItem531.Text = "Create SaveConfig";
                    this.toolStripMenuItem532.Text = "Create Others";
                    this.label2.Text = "Command :";
                    this.groupBox4.Text = "Parameters";
                    this.checkBox5.Text = "Send HEX";
                    this.checkBox4.Text = "Auto Send (ms)";
                    this.button4.Text = "Build";
                    this.groupBox6.Text = "Manual Command";
                    this.button8.Text = "Clear";
                    this.checkBox2.Text = "Auto Send (ms)";
                    this.checkBox1.Text = "Send HEX";
                    this.groupBox5.Text = "Response";
                    this.button5.Text = "Where";
                    this.button6.Text = "Clear";
                    this.checkBox3.Text = "Show HEX";
                    this.button2.Text = "Receive";
                    this.button7.Text = "Save";
                    this.toolStripDropDownButton6.Text = "语言(&Language)";
                    break;
                case (int)Form1.Language.Chinese:
                    if (this.toolStripButton1.Text == "Open  SP")
                    {
                        this.toolStripButton1.Text = "打开串口";
                    }
                    else if (this.toolStripButton1.Text == "Close SP")
                    {
                        this.toolStripButton1.Text = "关闭串口";
                    }
                    if (this.button3.Text == "Send")
                    {
                        this.button3.Text = "发送";
                    }
                    else if (this.button3.Text == "Stop")
                    {
                        this.button3.Text = "停止";
                    }
                    if (this.button1.Text == "Send")
                    {
                        this.button1.Text = "发送";
                    }
                    else if (this.button1.Text == "Stop")
                    {
                        this.button1.Text = "停止";
                    }
                    this.toolStripDropDownButton1.Text = "文件(&F)";
                    this.toolStripMenuItem11.Text = "打开配置文档(&O)";
                    this.toolStripMenuItem12.Text = "历史记录(&R)";
                    this.toolStripMenuItem126.Text = "清除历史(&H)";
                    this.toolStripMenuItem13.Text = "关闭配置文档(&C)";
                    this.toolStripMenuItem14.Text = "退出(&E)";
                    this.toolStripDropDownButton2.Text = "工具(&T)";
                    this.toolStripMenuItem21.Text = "串口配置(&S)";
                    this.toolStripMenuItem22.Text = "定时器/计数器(&C)";
                    this.toolStripMenuItem23.Text = "校验和计算与ASCII码转换(&K)";
                    this.toolStripMenuItem24.Text = "模拟示波器";
                    this.toolStripMenuItem29.Text = "置顶(&T)";
                    this.toolStripDropDownButton3.Text = "调试模式(&B)";
                    this.toolStripMenuItem31.Text = "空调调试模式(&A)";
                    this.toolStripMenuItem32.Text = "洗衣机调试模式(&W)";
                    this.toolStripDropDownButton4.Text = "演示模式(&M)";
                    this.toolStripMenuItem41.Text = "空调演示模式(&A)";
                    this.toolStripMenuItem42.Text = "洗衣机演示模式(&W)";
                    this.toolStripDropDownButton5.Text = "帮助(&H)";
                    this.toolStripMenuItem51.Text = "关于(&A)";
                    this.toolStripMenuItem52.Text = "帮助文档";
                    this.toolStripMenuItem53.Text = "创建系统文件";
                    this.toolStripMenuItem531.Text = "创建保存配置的系统文件";
                    this.toolStripMenuItem532.Text = "创建其他文件";
                    this.label2.Text = "选中指令 :";
                    this.groupBox4.Text = "参数";
                    this.checkBox5.Text = "十六进制发送";
                    this.checkBox4.Text = "自动发送(毫秒)";
                    this.button4.Text = "生成";
                    this.groupBox6.Text = "手动指令";
                    this.button8.Text = "清空";
                    this.checkBox2.Text = "自动发送(毫秒)";
                    this.checkBox1.Text = "十六进制发送";
                    this.groupBox5.Text = "回复";
                    this.button5.Text = "路径";
                    this.button6.Text = "清空";
                    this.checkBox3.Text = "十六进制";
                    this.button2.Text = "接收";
                    this.button7.Text = "保存";
                    this.toolStripDropDownButton6.Text = "&Language(语言)";
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region [其他注释掉的代码-----------------------------------------------]

        /*
        FormWindowState fws = FormWindowState.Normal;
        //private Form ff;
        int fw,sw;
        int fh,sh;
        int ls;
        
        //获得窗口最大化和最小化的事件
        protected override void OnResize(EventArgs e)
        {
            if (WindowState == FormWindowState.Maximized)
            {
                fws = FormWindowState.Maximized;
                //最大化时所需的操作 
                //MessageBox.Show("max");
            }
            else if (WindowState == FormWindowState.Minimized)
            {
                fws = FormWindowState.Minimized;
                //最小化时所需的操作
                //MessageBox.Show("min");
            }
            else if (WindowState == FormWindowState.Normal)
            {
                if (fws == FormWindowState.Maximized)
                {
                    fws = FormWindowState.Normal;
                    //MessageBox.Show("return");
                    if (fw == 702 && fh == 595)
                    {
                        splitContainer1.Size = new Size(splitContainer1.MinimumSize.Width, splitContainer1.MinimumSize.Height);
                        splitContainer1.Panel2MinSize = 172;
                    }
                    else
                    {
                        this.Size = new Size(fw, fh);
                        splitContainer1.Size = new Size(sw, sh);
                    }
                }
            }
            else
            {
                fw = this.Size.Width;
                fh = this.Size.Height;
            }
        }
        */

        #endregion

    }
}
