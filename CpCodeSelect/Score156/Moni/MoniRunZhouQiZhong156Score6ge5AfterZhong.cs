using CpCodeSelect.Business;
using CpCodeSelect.Business.Score;
using CpCodeSelect.Business.Score.Moni;
using CpCodeSelect.Model;
using CpCodeSelect.Model.TableModel;
using CpCodeSelect.Score;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CpCodeSelect.Score156.Moni
{
    public partial class MoniRunZhouQiZhong156Score6ge5AfterZhong : Form
    {
        private static Object lockObj = new object();
        private Hou3Select156YiLouSetFormZhouQiZhongScore6ge5AfterZhongMoniBusiness moniBusiness;
        Code beforeCode = null;
        Code currentCode = null;
        private Hou3Select156YiLouSetFormScoreAndChuShou6ge5 parentForm = null;
        private int TryCopTime = 0;
        public MoniRunZhouQiZhong156Score6ge5AfterZhong(Hou3Select156YiLouSetFormScoreAndChuShou6ge5 form) : this()
        {
            parentForm = form;
        }
        public MoniRunZhouQiZhong156Score6ge5AfterZhong()
        {
            InitializeComponent();
            moniBusiness = new Hou3Select156YiLouSetFormZhouQiZhongScore6ge5AfterZhongMoniBusiness(CustomLogMethod, Hou3Select156YiLouSetFormScoreAndChuShouBusiness.model350List);
            dataGridView1.DataSource = moniBusiness.yilouStatisticList;
        }
        public void Run(Code code)
        {
            if (dataGridView1.InvokeRequired)
            {
                dataGridView1.BeginInvoke((MethodInvoker)(() =>
                {
                    RunCode(code);
                }));
            }
            else
            {
                RunCode(code);
            }
        }
        private void RunCode(Code code)
        {
            currentCode = code;
            if (beforeCode == null || beforeCode.CodeQiHao != code.CodeQiHao)
            {
                beforeCode = code;
                //当上一期期号和当前期号不一样时，才进行计算
                //这里需要跑10期后再进行计算
                if (Hou3Select156YiLouSetFormScoreAndChuShouBusiness.AllCode.Count >= 270)
                {
                    moniBusiness.CalcCode(code);
                    SetFormTxtValue();
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = moniBusiness.yilouStatisticList;
                }
            }
        }
        public void SetFormTxtValue()
        {
            txtCurrentLun.Text = moniBusiness.CurrentLun.ToString();
            txtCurrentAmount.Text = moniBusiness.CurrentAmount.ToString();
            txtCurrentQi.Text = moniBusiness.CurrentaQi.ToString();
            txtTotalGuaCi.Text = moniBusiness.TotalGua.ToString();
            txtTotalZhongCi.Text = moniBusiness.TotalZhong.ToString();
            txtTotalLun.Text = moniBusiness.TotalLun.ToString();
            int zhongjiangCount = 0;
            if (moniBusiness.CurrentLun > 1) zhongjiangCount = moniBusiness.CurrentLunZhongJiangCiShu + 1;

            txtTotalAmount.Text = moniBusiness.TotalResult.ToString("0.00");

            txtLiushui.Text = moniBusiness.TotalLiuShui.ToString("0.00");

            lblTouZhuBei.Text = $"投注{moniBusiness.CurrentBei.ToString()}倍";
        }
        public void CustomLogMethod(string message)
        {
            lblTouZhuBei.Text = $"投注{moniBusiness.CurrentBei.ToString()}倍";
            //最新消息排在最上面
            if (listBoxExeMsg.InvokeRequired)
            {
                listBoxExeMsg.BeginInvoke((MethodInvoker)(() =>
                {

                    CustomLogMethodInstance(message);
                }));
            }
            else
            {
                CustomLogMethodInstance(message);
            }
        }
        public void CustomLogMethodInstance(string message)
        {
            //最新消息排在最上面
            listBoxExeMsg.Items.Insert(0, message);
            listBoxExeMsg.TopIndex = 0; // 自动滚动到底部
            //listBoxExeMsg.TopIndex = listBoxExeMsg.Items.Count - 1; // 自动滚动到底部
            SetFormTxtValue();
            lock (lockObj)
            {
                using (var writer = new StreamWriter("moni3-5.txt", true))
                {
                    writer.WriteLine(message);
                    writer.Flush();
                }
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
            TryCopTime = 0;
            txt350Code.Text = string.Join(" ", moniBusiness.current350List);
            CopyCurrent350Code();
        }

        private void CopyCurrent350Code()
        {
            try
            {
                if (TryCopTime <= 5)
                {
                    Clipboard.SetText(txt350Code.Text);
                }
            }
            catch (Exception ex)
            {
                Thread.Sleep(1500);
                CopyCurrent350Code();
            }
            finally
            {
                TryCopTime++;
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txt350Code.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TestInfoStatistic model = new TestInfoStatistic();
            model.Win = (moniBusiness.TotalResult);
            model.LiuShui = moniBusiness.TotalLiuShui;
            model.GuaCount = moniBusiness.TotalGua;
            parentForm.TestInfoStatisticList.Add(model);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            moniBusiness.Reset();
            ResetForm();
        }

        private void ResetForm()
        {
            txtCurrentLun.Text = "";
            txtCurrentAmount.Text = "";
            txtCurrentQi.Text = "1";
            txtTotalGuaCi.Text = "0";
            txtTotalZhongCi.Text = "0";
            lblTouZhuBei.Text = $"0倍";

            txtTotalAmount.Text = "0";

            txtLiushui.Text = "0";
            listBoxExeMsg.Items.Clear();
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = moniBusiness.yilouStatisticList;
        }
    }
}
