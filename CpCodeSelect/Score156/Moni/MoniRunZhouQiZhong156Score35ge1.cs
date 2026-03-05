using CpCodeSelect.Business;
using CpCodeSelect.Business.Score;
using CpCodeSelect.Business.Score.Moni;
using CpCodeSelect.Model;
using CpCodeSelect.Model.TableModel;
using CpCodeSelect.Score;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CpCodeSelect.Score156.Moni
{
    public partial class MoniRunZhouQiZhong156Score35ge1 : Form
    {
        private static Object lockObj = new Object();
        private MoniRunZhouQiZhong156Score35ge1MoniBusiness moniBusiness;
        private Hou3Select156YiLouSetFormScoreAndChuShou parentForm = null;
        private int LeftRecordCount = -1;
        Code beforeCode = null;
        Code currentCode = null;
        public MoniRunZhouQiZhong156Score35ge1(Hou3Select156YiLouSetFormScoreAndChuShou form):this()
        {
            parentForm = form;
        }
        public MoniRunZhouQiZhong156Score35ge1()
        {
            InitializeComponent();
            moniBusiness=new MoniRunZhouQiZhong156Score35ge1MoniBusiness(CustomLogMethod, Hou3Select156YiLouSetFormScoreAndChuShouBusiness.model350List);
            dataGridView1.DataSource = moniBusiness.yilouStatisticList; 
            var needDeleteMoniFormRecord = ConfigurationManager.AppSettings["NeedDeleteMoniFormRecord"];
            if (!string.IsNullOrEmpty(needDeleteMoniFormRecord))
            {
                int.TryParse(needDeleteMoniFormRecord, out LeftRecordCount);
            }
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


        public void RunCode(Code code)
        {
            currentCode = code;
            if (beforeCode == null || beforeCode.CodeQiHao != code.CodeQiHao)
            {
                beforeCode = code;
                //当上一期期号和当前期号不一样时，才进行计算
                //这里需要跑10期后再进行计算
                if (Hou3Select156YiLouSetFormScoreAndChuShouBusiness.AllCode.Count > 150)
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
            txtCurrentBei.Text = $"{moniBusiness.CurrentBei.ToString()}倍";
            int zhongjiangCount = 0;
            if (moniBusiness.CurrentLun > 1) zhongjiangCount = moniBusiness.CurrentLunZhongJiangCiShu+1;

            txtTotalAmount.Text=(moniBusiness.TotalResult-17400).ToString("0.00");

            txtLiushui.Text=moniBusiness.TotalLiuShui.ToString("0.00");
        }
        public void CustomLogMethod(string message)
        {
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
            if (LeftRecordCount > 0)
            {
                while (listBoxExeMsg.Items.Count > LeftRecordCount)
                {
                    listBoxExeMsg.Items.RemoveAt(listBoxExeMsg.Items.Count - 1);
                }
            }

            listBoxExeMsg.TopIndex = 0; // 自动滚动到底部
            //listBoxExeMsg.TopIndex = listBoxExeMsg.Items.Count - 1; // 自动滚动到底部
            SetFormTxtValue();
            lock (lockObj)
            {
                using (var writer = new StreamWriter("moni35ge1.txt", true))
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
            txt350Code.Text = string.Join(" ", moniBusiness.before270List);
        }

        private void btnCurrent350Code_Click(object sender, EventArgs e)
        {
            txt350Code.Text = string.Join(" ", moniBusiness.current270List);
            Clipboard.SetText(txt350Code.Text);

        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txt350Code.Text);
        }

        private void btnReset_Click(object sender, EventArgs e)
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
            txtCurrentBei.Text = $"0倍";

            txtTotalAmount.Text = "0";

            txtLiushui.Text = "0";
            listBoxExeMsg.Items.Clear();
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = moniBusiness.yilouStatisticList;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TestInfoStatistic model=new TestInfoStatistic();
            model.Win = (moniBusiness.TotalResult - 17400);
            model.LiuShui = moniBusiness.TotalLiuShui;
            model.GuaCount = moniBusiness.TotalGua;
            parentForm.TestInfoStatisticList.Add(model);
        }

        private void MoniRunZhouQiZhong156Score35ge1_Load(object sender, EventArgs e)
        {

        }
    }
}
