using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ESP
{
    public partial class ListViewEA : ListView
    {
        //自定义控件
        public NumericUpDown myNumUD;

        public ListViewEA()
        {
            // 自定义numericUpDown控件myNumUD
            myNumUD = new NumericUpDown();
            // 更改numericUpDown的外观
            myNumUD.BorderStyle = BorderStyle.None;
            // myNumUD.Multiline = false;
            myNumUD.Visible = false;
            // myNumUD.Maximum = 65535;
            myNumUD.Minimum = -4294967295;
            myNumUD.Maximum = 4294967295;
            // 小数位为2位
            myNumUD.DecimalPlaces = 2;
            // 增幅为0.25  1
            myNumUD.Increment = 0.25M;
            this.GridLines = true;
            this.FullRowSelect = true;
            this.Controls.Add(myNumUD);
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            this.myNumUD.Visible = false;
            base.OnSelectedIndexChanged(e);
        }

        // 双击
        protected override void OnDoubleClick(EventArgs e)
        {
            Point tmpPoint = this.PointToClient(Cursor.Position);
            ListViewItem.ListViewSubItem subitem = this.HitTest(tmpPoint).SubItem;
            ListViewItem item = this.HitTest(tmpPoint).Item;
            if (subitem != null)
            {
                if (item.SubItems[0].Equals(subitem))
                {
                }
                else if (item.SubItems[1].Equals(subitem))
                {
                }
                else if (item.SubItems[2].Equals(subitem))
                {
                }
                //当选中的是第4个subitem时执行EditItem方法
                else if (item.SubItems[3].Equals(subitem))
                {
                    EditItem(subitem);
                }
            }

            base.OnDoubleClick(e);
        }

        // 进入myNumUD的状态
        private void EditItem(ListViewItem.ListViewSubItem subItem)
        {
            // 判断Item是否为空
            if (this.SelectedItems.Count <= 0)
            {
                return;
            }
            Rectangle _rect = subItem.Bounds;
            // 美化操作
            _rect.X = _rect.X + 5;
            _rect.Width = _rect.Width - 6; 
            // 将subItem的属性赋给myNumUD
            // 完善myNumUD的其他属性
            myNumUD.Bounds = _rect;
            myNumUD.BringToFront();
            myNumUD.Text = subItem.Text;
            myNumUD.Select(0, 100);
            // 添加事件
            myNumUD.KeyDown += new System.Windows.Forms.KeyEventHandler(myNumUD_KeyDown);
            myNumUD.Leave += new EventHandler(tb_Leave);
            myNumUD.TextChanged += new EventHandler(myNumUD_TextChanged);
            myNumUD.Visible = true;
            myNumUD.Tag = subItem;
            myNumUD.Select();
        }

        // 当按下enter键时执行tb_Leave方法
        private void myNumUD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.tb_Leave(sender, e);
            }
        }

        // 当更改完成，myNumUD不再获得焦点时，使其的可见性为false，并且执行myNumUD_TextChanged方法
        private void tb_Leave(object sender, EventArgs e)
        {
            //myNumUD.TextChanged -= new EventHandler(myNumUD_TextChanged);
            if ((sender as NumericUpDown).Tag is ListViewItem.ListViewSubItem)
            {
                if (this.myNumUD.Text == "")
                {
                    (this.myNumUD.Tag as ListViewItem.ListViewSubItem).Text = "00";
                }
                else
                {
                    (this.myNumUD.Tag as ListViewItem.ListViewSubItem).Text = this.myNumUD.Value.ToString("0.00");
                }
            }
            myNumUD.Visible = false;
        }
        
        // myNumUD内容改变时，将其值赋值给listView.listViewSubItem.Text
        public void myNumUD_TextChanged(object sender, EventArgs e)
        {
            //// 目前来看textchange的事件是不必要的，只需要一个leave事件即可
            //if ((sender as NumericUpDown).Tag is ListViewItem.ListViewSubItem)
            //{
            //    if (this.myNumUD.Text == "")
            //    {
            //        (this.myNumUD.Tag as ListViewItem.ListViewSubItem).Text = "00";
            //    }
            //    else
            //    {
            //        (this.myNumUD.Tag as ListViewItem.ListViewSubItem).Text = this.myNumUD.Text;
            //    }
            //}
        }

        //// 滚轮动作，我怎么没看明白。。。应该与上sytemcommand啊
        //// 重写消息事件，没什么用啊，我注释掉了一一样好用的
        //protected override void WndProc(ref   Message m)
        //{
        //    if (m.Msg == 0x115 || m.Msg == 0x114)
        //    {
        //        this.myNumUD.Visible = false;
        //    }
        //    base.WndProc(ref   m);
        //}


    }
}
