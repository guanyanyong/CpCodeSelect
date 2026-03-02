using CpCodeSelect.Business;
using CpCodeSelect.Business.Score;
using CpCodeSelect.Business.Score.Moni;
using CpCodeSelect.Business.WuXingDuDan;
using CpCodeSelect.Business.WuXingDuDan.Moni;
using CpCodeSelect.Model;
using CpCodeSelect.Util.Config;
using CpCodeSelect.Util.ZiJinFangAn;
using Microsoft.Win32.SafeHandles;
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

namespace CpCodeSelect.WuXingDuDan.Moni
{
    public partial class MoniWuXingDuDan : Form
    {
        private WuXingDuDanMoniBusiness moniBusiness;
        Code beforeCode = null;
        Code currentCode = null;
        public MoniWuXingDuDan()
        {
            InitializeComponent(); 
            moniBusiness=new WuXingDuDanMoniBusiness(CustomLogMethod, WuXingDuDanBusiness.model350List);
            dataGridView1.DataSource = moniBusiness.yilouStatisticList;
            lblNameMaxGua.Text = $"最大{AppConfig.Current.LunSettings.MaxGuaCount}挂次数";
            //dataGridView2.DataSource = moniBusiness.chuShouWeiZhongList;
        }
        public void Run(Code code, bool zhongHouDelete = false)
        {
            currentCode = code;
            if (beforeCode ==null || beforeCode.CodeQiHao != code.CodeQiHao)
            {
                beforeCode = code;
                //当上一期期号和当前期号不一样时，才进行计算
                //这里需要跑10期后再进行计算
                if (WuXingDuDanBusiness.AllCode.Count >= WuXingDuDanBusiness.RunSkipNumber)
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

            var largeYinKui = (fangAn.LargeTotalPrincipal - ZiJinFangAnV40951.MiddleTotalPrincipalInit);
            var currentYinKui = fangAn.SmallCurrentProfitLoss;
            //当前所有的钱 减去2被的中间轮的本金 和200块的初始本金就是总盈利
            txtTotalAmount.Text=(fangAn.LargeTotalPrincipal 
                + fangAn.MiddleCurrentPrincipalExcludSmall 
                + fangAn.SmallCurrentPrincipal 
                - ZiJinFangAnV40951.MiddleTotalPrincipalInit*2 -200
                ).ToString();
            txtMiddleCurrentlun.Text = fangAn.MiddleCurrentLun.ToString("0.00");
            txtLargeLun.Text = fangAn.LargeCurrentLun.ToString("0.00");

            txtLiushui.Text= fangAn.LargeTotalLiuShui.ToString("0.00");
            txtMaxResult.Text = moniBusiness.MaxResult.ToString("0.00");
            txtMinResult.Text = moniBusiness.MinResult.ToString("0.00");

            txtMaxMiddleLun.Text = moniBusiness.ZiJinFangAn.LargeMaxMiddleLunCount.ToString();
           lblTouZhuBei.Text= $"投注{fangAn.SmallCurrentBetAmount.ToString()}倍";

            lblMaxLianZhong.Text= $"最大连中{fangAn.LargeMaxLianZhongCount}次";
            lblMaxLianGua.Text= $"最大连挂{fangAn.LargeMaxLianGuaCount}次";
            txtMaxGuaCount.Text = moniBusiness.AllCalcMaxGuaSumCount.ToString();
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
            var fangAn = moniBusiness.ZiJinFangAn;
            if(moniBusiness.currentExecute!=null)
            {
                var codeStr = string.Join(" ", moniBusiness.currentExecute.Number156);
                txt156Code.Text = string.Join(" ", codeStr);
                try
                {
                    Clipboard.SetText(codeStr);
                }catch(Exception ex)
                {
                    txt156Code.Text = ex.Message + ex.StackTrace;
                    
                }
            }

        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            var codeStr = string.Join(" ", moniBusiness.currentExecute.Number156);
            Clipboard.SetText(codeStr);
        }

        private void txtReset_Click(object sender, EventArgs e)
        {
            //moniBusiness.ZiJinFangAn.LargeLunOrigianInit();
            moniBusiness.Reset();
            RestFormText();
        }
        private void RestFormText()
        {
            var fangAn = moniBusiness.ZiJinFangAn;
            txtMaxResult.Text = moniBusiness.CurrentLun.ToString();
            txtCurrentAmount.Text = moniBusiness.CurrentAmount.ToString();
            txtMinResult.Text = moniBusiness.CurrentaQi.ToString();
            var zhongCount = fangAn.SmallAllTotalZhong;
            var guaCount = fangAn.SmallAllTotalGua;
            txtTotalGuaCi.Text = guaCount.ToString();
            txtTotalZhongCi.Text = zhongCount.ToString();
            if (guaCount + zhongCount > 0)
            {
                txtZhongJiangLv.Text = (zhongCount * 100M / (guaCount + zhongCount)).ToString("0.00") + "%";
            }
            int zhongjiangCount = 0;
            if (moniBusiness.CurrentLun > 1) zhongjiangCount = moniBusiness.CurrentLunZhongJiangCiShu + 1;

            //当前所有的钱 减去2被的中间轮的本金 和200块的初始本金就是总盈利
            txtTotalAmount.Text = "0";
            txtMiddleCurrentlun.Text = fangAn.MiddleCurrentLun.ToString("0.00");
            txtLargeLun.Text = fangAn.LargeCurrentLun.ToString("0.00");

            txtLiushui.Text = fangAn.LargeTotalLiuShui.ToString("0.00");
            txtMaxResult.Text = moniBusiness.MaxResult.ToString("0.00");
            txtMinResult.Text = moniBusiness.MinResult.ToString("0.00");

            txtMaxMiddleLun.Text = moniBusiness.ZiJinFangAn.LargeMaxMiddleLunCount.ToString();

            txtTotalZhongCi.Text = "0";
            txtTotalGuaCi.Text = "0";
            txtMiddleCurrentlun.Text = "1";
            txtLargeLun.Text = fangAn.LargeCurrentLun.ToString("0.00");
            txtZhongJiangLv.Text = "";

            lblTouZhuBei.Text = "投注0倍";

            listBoxExeMsg.Items.Clear();


            lblMaxLianZhong.Text = $"最大连中0次";
            lblMaxLianGua.Text = $"最大连挂0次";
        }
    }
}
