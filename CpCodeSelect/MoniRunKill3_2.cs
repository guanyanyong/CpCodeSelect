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
    public partial class MoniRunKill3_2 : Form
    {
        private Kill3moniBusiness_2 moniBusiness;
        Code beforeCode = null;
        Code currentCode = null;
        public MoniRunKill3_2()
        {
            InitializeComponent();
            moniBusiness=new Kill3moniBusiness_2(CustomLogMethod);
        }
        public void Run(Code code)
        {
            currentCode = code;
            if (beforeCode ==null || beforeCode.CodeQiHao != code.CodeQiHao)
            {
                //当上一期期号和当前期号不一样时，才进行计算
                moniBusiness.CalcCode(code);
                beforeCode = code;
                SetFormTxtValue();
            }
        }
        public void SetFormTxtValue()
        {
            txtCurrentLun.Text= moniBusiness.CurrentLun.ToString();
            txtCurrentBei.Text= moniBusiness.CurrentBei.ToString();
            txtCurrentQi.Text = moniBusiness.CurrentaQi.ToString();
            int zhongjiangCount = 0;
            if (moniBusiness.CurrentLun > 1) zhongjiangCount = moniBusiness.CurrentLunZhongJiangCiShu+1;

            if (moniBusiness.CurrentLun > 0)
                txtCurrentCi.Text = (moniBusiness.CurrentLunZhongJiangCiShu+1 ).ToString();
            else
                txtCurrentCi.Text = (moniBusiness.CurrentLunZhongJiangCiShu).ToString();

            txtTotalAmount.Text=moniBusiness.TotalResult.ToString("0.00");

            txtLiushui.Text=moniBusiness.TotalLiuShui.ToString("0.00");
            txtYiGuaCount.Text = moniBusiness.YiGuaCount.ToString();
        }
        public void CustomLogMethod(string message)
        {
            listBoxExeMsg.Items.Add(message);
            listBoxExeMsg.TopIndex = listBoxExeMsg.Items.Count - 1; // 自动滚动到底部
            SetFormTxtValue();
            using (var writer = new StreamWriter("moni.txt", true))
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
    }
}
