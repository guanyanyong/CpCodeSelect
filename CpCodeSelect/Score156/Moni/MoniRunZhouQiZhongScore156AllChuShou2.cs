using CpCodeSelect.Business;
using CpCodeSelect.Business.Score;
using CpCodeSelect.Business.Score.Moni;
using CpCodeSelect.Model;
using CpCodeSelect.Util.ZiJinFangAn;
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

namespace CpCodeSelect.Score.Moni
{
    public partial class MoniRunZhouQiZhongScore156AllChuShou2 : Form
    {
        private MoniRunZhouQiZhongScore156AllChuShou2MoniBusiness moniBusiness;
        Code beforeCode = null;
        Code currentCode = null;
        public MoniRunZhouQiZhongScore156AllChuShou2()
        {
            InitializeComponent(); 
            moniBusiness=new MoniRunZhouQiZhongScore156AllChuShou2MoniBusiness(CustomLogMethod, Hou3Select156YiLouSetFormScoreAndChuShouBusiness.model350List);
            dataGridView1.DataSource = moniBusiness.yilouStatisticList;
            dataGridView2.DataSource = moniBusiness.chuShouWeiZhongList;
        }
        public void Run(Code code, bool zhongHouDelete = false)
        {
            currentCode = code;
            if (beforeCode ==null || beforeCode.CodeQiHao != code.CodeQiHao)
            {
                beforeCode = code;
                //当上一期期号和当前期号不一样时，才进行计算
                //这里需要跑10期后再进行计算
                if (Hou3Select156YiLouSetFormScoreAndChuShouBusiness.AllCode.Count >= Hou3Select156YiLouSetFormScoreAndChuShouBusiness.RunSkipNumber)
                {
                    moniBusiness.CalcCode(code, zhongHouDelete);
                    SetFormTxtValue();
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = moniBusiness.yilouStatisticList;
                }
            }
        }
        public void SetFormTxtValue()
        {
            var fangAn = moniBusiness.ZiJinFangAn;
            txtMaxResult.Text= moniBusiness.CurrentLun.ToString();
            txtCurrentAmount.Text= moniBusiness.CurrentAmount.ToString();
            txtMinResult.Text = moniBusiness.CurrentaQi.ToString();
            var zhongCount = fangAn.SmallAllTotalZhong;
            var guaCount = fangAn.SmallAllTotalGua;
            txtTotalGuaCi.Text = guaCount.ToString();
            txtTotalZhongCi.Text = zhongCount.ToString();
            if (guaCount + zhongCount > 0)
            {
                txtZhongJiangLv.Text = (zhongCount * 100M / (guaCount + zhongCount) ).ToString("0.00") + "%";
            }
            int zhongjiangCount = 0;
            if (moniBusiness.CurrentLun > 1) zhongjiangCount = moniBusiness.CurrentLunZhongJiangCiShu+1;

            var largeYinKui = (fangAn.LargeTotalPrincipal - ZiJinFangAnV2.MiddleTotalPrincipalInit);
            var currentYinKui = fangAn.SmallCurrentProfitLoss;
            //当前所有的钱 减去2被的中间轮的本金 和200块的初始本金就是总盈利
            txtTotalAmount.Text=(fangAn.LargeTotalPrincipal 
                + fangAn.MiddleCurrentPrincipalExcludSmall 
                + fangAn.SmallCurrentPrincipal 
                - ZiJinFangAnV2.MiddleTotalPrincipalInit*2
                - 200
                ).ToString();

            txtLiushui.Text=moniBusiness.TotalLiuShui.ToString("0.00");
            txtMaxResult.Text = moniBusiness.MaxResult.ToString("0.00");
            txtMinResult.Text = moniBusiness.MinResult.ToString("0.00");

            txtMaxMiddleLun.Text = moniBusiness.ZiJinFangAn.LargeMaxMiddleLunCount.ToString();
        }
        public void CustomLogMethod(string message)
        {
            //最新消息排在最上面
            listBoxExeMsg.Items.Insert(0,message);
            listBoxExeMsg.TopIndex = 0; // 自动滚动到底部
            //listBoxExeMsg.TopIndex = listBoxExeMsg.Items.Count - 1; // 自动滚动到底部
            SetFormTxtValue();
            using (var writer = new StreamWriter("moni4-2.txt", true))
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
            //txt350Code.Text = string.Join(" ", moniBusiness.before350List);
        }

        private void btnCurrent350Code_Click(object sender, EventArgs e)
        {
            //txt350Code.Text = string.Join(" ", moniBusiness.current350List);

        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            //Clipboard.SetText(txt350Code.Text);
        }
    }
}
