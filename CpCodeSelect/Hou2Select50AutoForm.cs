using CpCodeSelect.Model;
using CpCodeSelect.Util;
using CpCodeSelect.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace CpCodeSelect
{
    public partial class Hou2Select50AutoForm : Form
    {
        public Dictionary<int, List<StatisticModel>> StatisticDic = new Dictionary<int, List<StatisticModel>>();
        private string filePath = @"D:\Program Files (x86)\益达挂机软件\OpenCode\YDYLTXFFC.txt";
        private FileSystemWatcher fileWatcher;
        private System.Windows.Forms.Timer showErrorTexttimer;
        //private Timer addTextTimer;
        private Code lastCode;
        private Code currentCode;
        private bool firstTime = true;//是否第一次执行
        private Object lockObj = new Object();
        public MoniRunKill3 moniKill3 = new MoniRunKill3();
        public MoniRunKill3_2 moniKill3_2 = new MoniRunKill3_2();
        public MoniRunKill3_3 moniKill3_3 = new MoniRunKill3_3();
        public StatisticForm statisticForm = new StatisticForm();
        public List<Hou2Select50_20Model> modelList = new List<Hou2Select50_20Model>();
        public Hou2Select50AutoForm()
        {
            InitializeComponent();
            Init();
            txtFIlePath.Text = filePath;
            moniKill3.Hide();
        }

        public void AddStatisticToDic(int number, StatisticModel model)
        {
            if (StatisticDic.ContainsKey(number))
            {
                StatisticDic[number].Add(model);
            }
            else
            {
                var list = new List<StatisticModel>();
                list.Add(model);
                StatisticDic.Add(number, list);
            }
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
            GetCodeFromFile();
        }
        private void FileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            // 使用Invoke确保在UI线程上更新
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(GetCodeFromFile));
            }
            else
            {
                GetCodeFromFile();
            }
        }
        /// <summary>
        /// 读取文件的第一行,并分析号码
        /// 只处理最新的一条记录
        /// </summary>
        public void ReadFirstLine()
        {
            var firstRecord = FileUtil.ReadFileFirstRecord(filePath);
            var code = FileAnalysis.GetCodeByStr(firstRecord);
            if (code != null)
            {
                code.PreCode = lastCode;
                currentCode = code;

                if (lastCode == null || lastCode.CodeQiHao != currentCode.CodeQiHao)
                {
                    lastCode = currentCode;
                    AddRecord($"检测到新号码: 期号={currentCode.CodeQiHao}, 号码={currentCode.CodeNumber}");
                    //在这里可以添加对号码的分析处理逻辑
                    AnalySisCode(currentCode);
                }
            }
        }
        /// <summary>
        /// 第一次执行,读取全部的号码
        /// 并依次分析万千百十个位的大小单双统计情况
        /// </summary>
        public void ReadAllLine()
        {
            var codeStrList = FileUtil.ReadFileAllRecods(filePath, 1000);
            var codeList = FileAnalysis.GetCodeListByCodeListStr(codeStrList);
            if (codeList != null && codeList.Count > 0)
            {
                AddRecord("第一次执行,需要从底下最后一条开始执行记录");
                for (int i = codeList.Count - 1; i >= 0; i--)
                {

                    currentCode = codeList[i];
                    currentCode.PreCode = lastCode;
                    if (lastCode == null || lastCode.CodeQiHao != currentCode.CodeQiHao)
                    {
                        lastCode = currentCode;
                        AddRecord($"检测到新号码: 期号={currentCode.CodeQiHao}, 号码={currentCode.CodeNumber}");
                        AnalySisCode(currentCode);
                    }
                }
            }
        }

        /// <summary>
        /// 分析当前号码，设置万千百十个位的大小单双单双属性
        /// </summary>
        public void AnalySisCode(Code code)
        {
            Hou2Select50AutoBusiness.InitCode(code);
            InitOfferNumber();
            GenerateOfferNumber();
            SetForm();
            //AddToLogFileZu6Kill1(code, "Hou2Select50.txt");
            //在这里把分析后的可以推荐的号码显示到界面上


            //执行模拟挂机
            //moniKill3.Run(code);
            //moniKill3_2.Run(code);
            //moniKill3_3.Run(code);

        }
        public void SetForm()
        {
            if (Hou2Select50AutoBusiness.modelList.Count > 0)
            {
                lblMaxGua.Text = Hou2Select50AutoBusiness.modelList.Max(p => p.GuaCount).ToString();
                lblTotalNumber.Text = Hou2Select50AutoBusiness.modelList.Count.ToString();
            }
            else
            {
                lblMaxGua.Text = "0";
                lblTotalNumber.Text = "0";
            }
        }
        public void InitOfferNumber()
        {

        }
        public void GenerateOfferNumber()
        {
            if (chkRefersh.Checked)
            {
                
                /*
                offerNumber01.Text = 
                offerNumber02.Text = Hou2Select50_20Business.GetHou2_50NumerString();
                offerNumber03.Text = Hou2Select50_20Business.GetHou2_50NumerString();
                offerNumber04.Text = Hou2Select50_20Business.GetHou2_50NumerString();
                offerNumber05.Text = Hou2Select50_20Business.GetHou2_50NumerString();
                offerNumber06.Text = Hou2Select50_20Business.GetHou2_50NumerString();
                offerNumber07.Text = Hou2Select50_20Business.GetHou2_50NumerString();
                offerNumber08.Text = Hou2Select50_20Business.GetHou2_50NumerString();
                offerNumber09.Text = Hou2Select50_20Business.GetHou2_50NumerString();
                offerNumber10.Text = Hou2Select50_20Business.GetHou2_50NumerString();
                offerNumber11.Text = Hou2Select50_20Business.GetHou2_50NumerString();
                offerNumber12.Text = Hou2Select50_20Business.GetHou2_50NumerString();
                offerNumber13.Text = Hou2Select50_20Business.GetHou2_50NumerString();
                offerNumber14.Text = Hou2Select50_20Business.GetHou2_50NumerString();
                offerNumber15.Text = Hou2Select50_20Business.GetHou2_50NumerString();
                offerNumber16.Text = Hou2Select50_20Business.GetHou2_50NumerString();
                offerNumber17.Text = Hou2Select50_20Business.GetHou2_50NumerString();
                offerNumber18.Text = Hou2Select50_20Business.GetHou2_50NumerString();
                offerNumber19.Text = Hou2Select50_20Business.GetHou2_50NumerString();
                offerNumber20.Text = Hou2Select50_20Business.GetHou2_50NumerString();
                offerNumber21.Text = Hou2Select50_20Business.GetHou2_50NumerString();
                offerNumber22.Text = Hou2Select50_20Business.GetHou2_50NumerString();
                offerNumber23.Text = Hou2Select50_20Business.GetHou2_50NumerString();
                offerNumber24.Text = Hou2Select50_20Business.GetHou2_50NumerString();
                offerNumber25.Text = Hou2Select50_20Business.GetHou2_50NumerString();
                offerNumber26.Text = Hou2Select50_20Business.GetHou2_50NumerString();
                offerNumber27.Text = Hou2Select50_20Business.GetHou2_50NumerString();
                offerNumber28.Text = Hou2Select50_20Business.GetHou2_50NumerString();
                offerNumber29.Text = Hou2Select50_20Business.GetHou2_50NumerString();
                offerNumber30.Text = Hou2Select50_20Business.GetHou2_50NumerString();
                */
            }
        }

        private void AddToLogFileZu6Kill1(Code code, string zu6Kill1FileName)
        {
            using (var writer = new StreamWriter(zu6Kill1FileName, true))
            {
                if (code.Zu6Kill1ModelList != null && code.Zu6Kill1ModelList.Count > 0)
                {
                    foreach (var zu6Kill1Mode in code.Zu6Kill1ModelList)
                    {
                        bool needFlush = false;
                        var list = zu6Kill1Mode.Zu6Kill1Items.Where(p => (p.IsLianGua && p.GuaCount >= 1) || (!p.IsLianGua && p.LianZhongCount >= 1)).ToList();
                        if (list.Count > 0)
                        {
                            foreach (var recode in list)
                            {
                                if (recode.IsLianGua)
                                {
                                    needFlush = true;
                                    //连挂中
                                    writer.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 记录 #" + $"期号:{code.CodeQiHao},号码：{code.CodeNumber}，{zu6Kill1Mode.Name}的{recode.Number}连挂中,当前连挂次数{recode.GuaCount}");
                                }
                                else
                                {
                                    needFlush = true;
                                    //连中
                                    writer.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 记录 #" + $"期号:{code.CodeQiHao},号码：{code.CodeNumber}，{zu6Kill1Mode.Name}的{recode.Number}连中,连中次数{recode.LianZhongCount}");
                                }
                            }
                        }
                        if (needFlush)
                        {
                            writer.Flush();
                        }
                    }

                }
            }
        }

        /// <summary>
        /// 从文件中获取最新的号码
        /// </summary>
        public void GetCodeFromFile()
        {
            lock (lockObj)
            {
                Thread.Sleep(1000); // 等待1000毫秒，确保文件写入完成
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
            showErrorTexttimer = new System.Windows.Forms.Timer();
            showErrorTexttimer.Interval = 3000; // 3秒
            showErrorTexttimer.Tick += Timer_Tick;


            autoClkTimer = new System.Windows.Forms.Timer();
            autoClkTimer.Interval = 10000; // 10秒
            autoClkTimer.Tick += AutoClkTimer_Tick;

            InitData();
            InitForm();
            //addTextTimer = new Timer();
            //addTextTimer.Interval = 100; // 0.1秒
            //addTextTimer.Tick += AddTexTimer_Tick;
        }
        private void InitForm()
        {
        }
        private void InitData()
        {
            lastCode = null;
            currentCode = null;
            firstTime = true;//是否第一次执行
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            labelError.Text = "";
            showErrorTexttimer.Stop(); // 停止计时器
        }

        private void AutoClkTimer_Tick(object sender, EventArgs e)
        {
            btnSelect.PerformClick();
        }


        private void AddTexTimer_Tick(object sender, EventArgs e)
        {
            //addTextTimer.Stop();
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
                filePath = fileName;
                txtFIlePath.Text = filePath;
            }
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


        private void button1_Click(object sender, EventArgs e)
        {
            StartExec();
        }
        /// <summary>
        /// 开始执行
        /// </summary>
        private void StartExec()
        {
            if (DateTime.Now >= Convert.ToDateTime("2025-11-30"))
            {
                //MessageBox.Show("软件试用期已过期，请联系作者购买正式版");
                return;
            }

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
        /// <summary>
        /// 清除操作历史列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearHistory_Click(object sender, EventArgs e)
        {
            listBoxHistory.Items.Clear();
        }


        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            fileWatcher?.Dispose();
            base.OnFormClosing(e);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TxtFileExecForm form = new TxtFileExecForm();
            form.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            moniKill3.Show();
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            Init();
            StartExec();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Init();
        }

        private void btnMoni2_Click(object sender, EventArgs e)
        {
            moniKill3_2.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            moniKill3_3.Show();
        }

        private void btnStatistic_Click(object sender, EventArgs e)
        {
            statisticForm.SetStatistic(this.StatisticDic);
            statisticForm.Show();
        }

        private void txtFIlePath_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // 检查点击是否有效（非标题行）且是特定的按钮列
            if (e.RowIndex >= 0 && dataGridView1.Columns[e.ColumnIndex].Name == "Numer50")
            {
                // 可以获取当前行的数据
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                // ... 执行你的业务逻辑，例如根据row.Cells["SomeColumn"].Value进行不同操作
                if (row.DataBoundItem is Hou2Select50_20Model)
                {
                    var model = row.DataBoundItem as Hou2Select50_20Model;
                    txt50Number.Text = string.Join(" ", model.Number50);
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txt50Number.Text = "";
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            var numberText = txt50Number.Text ;
            if (string.IsNullOrEmpty(numberText))
            {
                MessageBox.Show("没有可复制的号码");
                return;
            }
            Clipboard.SetText(numberText);
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            var number = numericUpDown1.Value;
            var list = Hou2Select50AutoBusiness.modelList.Where(p=>p.NeedZhong==false && p.GuaCount>= number).OrderByDescending(p=>p.GuaCount).ToList();
            dataGridView1.DataSource = list;

            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            dataGridView1.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);


            dataGridView1.Columns["IsShow"].Visible = false;
            dataGridView1.Columns["NeedZhong"].Visible = false;
            dataGridView1.Columns["ZhongGount"].Visible = false;

        }

        private void btnStartAuto_Click(object sender, EventArgs e)
        {
            autoClkTimer.Start();
            btnStartAuto.Enabled = false;
            btnStopAuto.Enabled = true;
            autoClkTimer.Interval = (int)numericUpDownAutoClick.Value * 1000; // 10秒
            btnSelect.PerformClick();
        }

        private void btnStopAuto_Click(object sender, EventArgs e)
        {
            autoClkTimer.Stop();
            btnStartAuto.Enabled = true;
            btnStopAuto.Enabled = false;
        }
    }
}
