using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using CpCodeSelect.Util;
using CpCodeSelect.Model;
using CpCodeSelect.Business;

namespace CpCodeSelect
{
    public partial class Zu6Kill1ZGGTTxtFileExecForm : Form
    {
        string txtFilePath = "D:\\code\\cp\\CpCodeSelect\\CpCodeSelect\\data\\2025-09-09_Txqq1f.txt";

        private Code lastCode;
        private Code currentCode;
        private Object lockObj = new Object();

        public Zu6Kill1ZGGTTxtFileExecForm()
        {
            InitializeComponent();

            txtDataFIlePath.Text = txtFilePath;
        }

        private void btnSetDataFile_Click(object sender, EventArgs e)
        {
            var fileName = SelectFile();
            if (!string.IsNullOrEmpty(fileName))
            {
                txtFilePath = fileName;
            }
            else
            {
                txtFilePath = "D:\\code\\cp\\CpCodeSelect\\CpCodeSelect\\data\\2025-09-09_Txqq1f.txt";
            }

            txtDataFIlePath.Text = txtFilePath;
        }

        /// <summary>
        /// 选择文件,用于设置获取历史开奖号的文件路径
        /// </summary>
        /// <returns></returns>
        public string SelectFile()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // 设置对话框属性
                openFileDialog.Title = "选择文件"; // 对话框标题
                openFileDialog.Filter = "所有文件 (*.*)|*.*"; // 文件过滤器
                openFileDialog.FilterIndex = 1; // 默认过滤器索引
                openFileDialog.RestoreDirectory = true; // 恢复初始目录

                // 显示对话框并检查用户是否点击了"确定"
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // 返回选择的文件路径
                    return openFileDialog.FileName;
                }
            }

            return null; // 用户取消选择
        }

        int recordCount = 0;
        /// <summary>
        /// 添加记录到操作历史列表中
        /// </summary>
        /// <param name="recordStr"></param>
        private void AddRecord(string recordStr)
        {
            recordCount++;
            string record = $"[{DateTime.Now:HH:mm:ss.fff}] 记录 #{recordCount} - {recordStr}";

            // 添加到集合
            listBoxHistory.Items.Add(record);

            // 更新UI显示（只显示最近50条以避免性能问题）
            if (listBoxHistory.Items.Count > 50)
            {
                listBoxHistory.Items.RemoveAt(0);
            }
            listBoxHistory.TopIndex = listBoxHistory.Items.Count - 1; // 自动滚动到底部

        }
        private void button1_Click(object sender, EventArgs e)
        {
            // 一行一行读取文件并分析

            var codeStrList = FileUtil.ReadFileAllRecods(txtFilePath, int.MaxValue);
            var codeList = FileAnalysis.GetCodeListByCodeListStr(codeStrList);
            if (codeList != null && codeList.Count > 0)
            {
                for (int i = 0; i < codeList.Count; i++)
                {
                    lock (lockObj)
                    {
                        currentCode = codeList[i];
                        currentCode.PreCode = lastCode;
                        if (lastCode == null || lastCode.CodeQiHao != currentCode.CodeQiHao)
                        {
                            lastCode = currentCode;
                            if (i % 500 == 0)
                            {
                                AddRecord($"检测到新号码: 期号={currentCode.CodeQiHao}, 号码={currentCode.CodeNumber}");
                            }
                            AnalySisCode(currentCode);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 分析当前号码，设置万千百十个位的大小单双单双属性
        /// </summary>
        public void AnalySisCode(Code code)
        {

            Zu6Kill1ZGGTBusiness.InitCode(code);


            //在这里把分析后的可以推荐的号码显示到界面上
            //AddTolistBoxTuiJian(true);
            AddToFile(currentCode);


        }

        private void AddToFile(Code code)
        {
            using (var writer = new StreamWriter("Zu6Kill1ZGGTlog_2025-10-10.txt", true))
            {
                if (code.Zu6Kill1ZGGTModelList != null && code.Zu6Kill1ZGGTModelList.Count > 0)
                {
                    foreach (var zu6Kill1Mode in code.Zu6Kill1ZGGTModelList)
                    {

                        var list = zu6Kill1Mode.Zu6Kill1ZGGTItems.Where(p => p.IsZGGT && p.ZGGTGuaCount >= 10 ).ToList();
                        
                        if (list.Count > 0)
                        {
                            foreach (var record in list)
                            {
                                if (record.IsZGGT && record.ZGGTZhongCount>0)
                                {
                                    //连挂中
                                    writer.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 记录 #" + $"期号:{code.CodeQiHao},号码：{code.CodeNumber}的{zu6Kill1Mode.Name}{record.Number}连挂中,连挂次数{record.ZGGTGuaCount},挂后中{record.ZGGTZhongCount}次。");
                                    writer.Flush();
                                }
                            }
                        }
                    }
                }
            }
        }

    }
}
