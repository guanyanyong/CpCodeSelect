using CpCodeSelect.Business;
using CpCodeSelect.Business.Score;
using CpCodeSelect.Business.Score.Moni;
using CpCodeSelect.Model;
using CpCodeSelect.Model.TableModel;
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

namespace CpCodeSelect.Score.Moni
{
    public partial class MoniRunZhouQiZhongScore3ge5AfterZhong : Form
    {
        private static Object lockObj = new Object();
        private Hou3Select350YiLouSetFormZhouQiZhongScore3ge5AfterZhongMoniBusiness moniBusiness;
        private Hou3Select350YiLouSetFormScoreAndChuShou parentForm = null;
        private int LeftRecordCount = -1;
        Code beforeCode = null;
        Code currentCode = null;
        public MoniRunZhouQiZhongScore3ge5AfterZhong(Hou3Select350YiLouSetFormScoreAndChuShou form)
        {
            parentForm = form;
            InitializeComponent();
            moniBusiness = new Hou3Select350YiLouSetFormZhouQiZhongScore3ge5AfterZhongMoniBusiness(CustomLogMethod, Hou3Select350YiLouSetFormScoreAndChuShouBusiness.model350List);
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
                if (Hou3Select350YiLouSetFormScoreAndChuShouBusiness.AllCode.Count >= 270)
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
            int zhongjiangCount = 0;
            if (moniBusiness.CurrentLun > 1) zhongjiangCount = moniBusiness.CurrentLunZhongJiangCiShu + 1;

            txtTotalAmount.Text = moniBusiness.TotalResult.ToString("0.00");

            txtLiushui.Text = moniBusiness.TotalLiuShui.ToString("0.00");
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

        private void CustomLogMethodInstance(string message)
        {
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
            txt350Code.Text = string.Join(" ", moniBusiness.current350List);

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
    }
}
