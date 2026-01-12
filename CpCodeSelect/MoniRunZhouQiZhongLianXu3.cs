using CpCodeSelect.Business;
using CpCodeSelect.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CpCodeSelect
{
    public partial class MoniRunZhouQiZhongLianXu3 : Form
    {
        private Hou3Select350YiLouSetFormZhouQiZhongLianXu3MoniBusiness moniBusiness;
        Code beforeCode = null;
        Code currentCode = null;
        public MoniRunZhouQiZhongLianXu3()
        {
            InitializeComponent();
            moniBusiness=new Hou3Select350YiLouSetFormZhouQiZhongLianXu3MoniBusiness(CustomLogMethod, Hou3Select350YiLouSetFormDuoZhouQiZhongBusiness.model350List);
            dataGridView1.DataSource = moniBusiness.yilouStatisticList;
        }
        public void Run(Code code)
        {
            currentCode = code;
            if (beforeCode ==null || beforeCode.CodeQiHao != code.CodeQiHao)
            {
                beforeCode = code;
                //当上一期期号和当前期号不一样时，才进行计算
                //这里需要跑10期后再进行计算
                if (Hou3Select350YiLouSetFormDuoZhouQiZhongBusiness.AllCode.Count > 1620)
                {
                    moniBusiness.CalcCode(code);
                    SetFormTxtValue();
                }
            }
        }
        public void SetFormTxtValue()
        {
            txtCurrentLun.Text= moniBusiness.CurrentLun.ToString();
            txtCurrentAmount.Text= moniBusiness.CurrentAmount.ToString();
            txtCurrentQi.Text = moniBusiness.CurrentaQi.ToString();
            txtTotalGuaCi.Text = moniBusiness.TotalGua.ToString();
            txtTotalZhongCi.Text = moniBusiness.TotalZhong.ToString();
            int zhongjiangCount = 0;
            if (moniBusiness.CurrentLun > 1) zhongjiangCount = moniBusiness.CurrentLunZhongJiangCiShu+1;

            txtTotalAmount.Text=moniBusiness.TotalResult.ToString("0.00");

            txtLiushui.Text=moniBusiness.TotalLiuShui.ToString("0.00");
        }
        public void CustomLogMethod(string message)
        {
            //最新消息排在最上面
            listBoxExeMsg.Items.Insert(0,message);
            listBoxExeMsg.TopIndex = 0; // 自动滚动到底部
            //listBoxExeMsg.TopIndex = listBoxExeMsg.Items.Count - 1; // 自动滚动到底部
            SetFormTxtValue();
            using (var writer = new StreamWriter("moni-3期.txt", true))
            {
                writer.WriteLine(message); 
                writer.Flush();
            }
        }

        private void btnYinChang_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void MoniRunDaXiao_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void btnBefore350Code_Click(object sender, EventArgs e)
        {
            txt350Code.Text = string.Join(" ", moniBusiness.before350List);
        }

        private void btnCurrent350Code_Click(object sender, EventArgs e)
        {
            txt350Code.Text = string.Join(" ", moniBusiness.current350List);

        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txt350Code.Text);
        }
    }
}
