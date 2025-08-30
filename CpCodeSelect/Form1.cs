using CpCodeSelect.Model;
using CpCodeSelect.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CpCodeSelect
{
    public partial class Form1 : Form
    {
        private string filePath = @"D:\Program Files (x86)\星欧挂机软件\OpenCode\TXFFC.txt";
        private FileSystemWatcher fileWatcher;
        private Timer showErrorTexttimer;
        private Timer addTextTimer;
        private Code lastCode;
        private Code currentCode;
        private bool firstTime = true;//是否第一次执行
        private Object lockObj = new Object();
        public Form1()
        {
            InitializeComponent();
            Init();
            txtFIlePath.Text = filePath;
        }

        private void StartMonitoring(string filePath)
        {
            // 停止之前的监控
            fileWatcher?.Dispose();


            // 初始化 FileSystemWatcher
            fileWatcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(filePath),
                Filter = Path.GetFileName(filePath),
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.CreationTime
            };

            // 注册事件
            fileWatcher.Changed += FileWatcher_Changed;
            fileWatcher.Created += FileWatcher_Changed;
            fileWatcher.Renamed += FileWatcher_Changed;

            // 开始监控
            fileWatcher.EnableRaisingEvents = true;

            // 立即读取一次第一行
            AnalySisCode();
        }
        private void FileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            // 使用Invoke确保在UI线程上更新
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(AnalySisCode));
            }
            else
            {
                AnalySisCode();
            }
        }

        public void ReadFirstLine()
        {
            var firstRecord = FileUtil.ReadFileFirstRecord(filePath);
            var code =FileAnalysis.GetCodeByStr(firstRecord);
            if (code != null)
            {
                currentCode = code;
                if (lastCode == null || lastCode.CodeQiHao != currentCode.CodeQiHao)
                {
                    lastCode = currentCode;
                    AddRecord($"检测到新号码: 期号={currentCode.CodeQiHao}, 号码={currentCode.CodeNumber}");
                }
            }
        }
        public void ReadAllLine()
        {
            var codeStrList = FileUtil.ReadFileAllRecods(filePath);
            var codeList = FileAnalysis.GetCodeListByCodeListStr(codeStrList);
            if (codeList != null && codeList.Count>0)
            {
                AddRecord("第一次执行,需要从底下最后一条开始执行记录");
                for (int i = codeList.Count - 1; i >= 0; i--)
                {

                    currentCode = codeList[i];
                    if (lastCode == null || lastCode.CodeQiHao != currentCode.CodeQiHao)
                    {
                        lastCode = currentCode;
                        AddRecord($"检测到新号码: 期号={currentCode.CodeQiHao}, 号码={currentCode.CodeNumber}");
                    }
                }
            }
        }
        public void AnalySisCode()
        {
            lock (lockObj)
            {
                if (firstTime)
                {
                    //如果是第一次执行,需要读取全部的号码
                    ReadAllLine();
                    firstTime = false;
                    ReadFirstLine();
                }
                else
                {
                    ReadFirstLine();
                }
            }
        }

        public void Init()
        {
            // 初始化计时器
            showErrorTexttimer = new Timer();
            showErrorTexttimer.Interval = 3000; // 3秒
            showErrorTexttimer.Tick += Timer_Tick;


            addTextTimer = new Timer();
            addTextTimer.Interval = 100; // 0.1秒
            addTextTimer.Tick += AddTexTimer_Tick;


        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            labelError.Text = "";
            showErrorTexttimer.Stop(); // 停止计时器
        }


        private void AddTexTimer_Tick(object sender, EventArgs e)
        {
            addTextTimer.Stop();
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            var fileName = SelectFile();
            if (string.IsNullOrEmpty(fileName))
            {
                labelError.Text = "必须选择文件路径";
                showErrorTexttimer.Start();
            }
            else
            {
                txtFIlePath.Text = fileName;
            }
        }

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

        public void AddValueToHistory(string value)
        {
            listBoxHistory.Items.Insert(0, value);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                AddRecord("开始执行");
                StartMonitoring(filePath);
            }

        }


        private void btnGetLast10record_Click(object sender, EventArgs e)
        {

            if (listBoxHistory.Items.Count > 10)
            {
                while (listBoxHistory.Items.Count > 10)
                {
                    listBoxHistory.Items.RemoveAt(0);
                }
            }
        }
        int recordCount = 0;
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

        private void btnClearHistory_Click(object sender, EventArgs e)
        {
            listBoxHistory.Items.Clear();
        }


        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            fileWatcher?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
