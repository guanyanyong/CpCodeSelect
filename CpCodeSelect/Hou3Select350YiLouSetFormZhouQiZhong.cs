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
using System.Collections;
using System.Media;
using System.Configuration;
using System.Data.SqlClient;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using CpCodeSelect.Model.ExModel;

namespace CpCodeSelect
{
    public partial class Hou3Select350YiLouSetFormZhouQiZhong : Form
    {
        public Dictionary<int, List<StatisticModel>> StatisticDic = new Dictionary<int, List<StatisticModel>>();
        private string filePath = @"C:\Program Files (x86)\hengshengguaji\OpenCode\TXFFC.txt";
        private FileSystemWatcher fileWatcher;
        private System.Windows.Forms.Timer showErrorTexttimer;
        //private Timer addTextTimer;
        private Code lastCode;
        private Code currentCode;
        private bool firstTime = true;//是否第一次执行
        private Object lockObj = new Object();
        private Object LockPlayObj = new object();
        public MoniRunKill3 moniKill3 = new MoniRunKill3();
        public MoniRunKill3_2 moniKill3_2 = new MoniRunKill3_2();
        public MoniRunKill3_3 moniKill3_3 = new MoniRunKill3_3();
        public StatisticForm statisticForm = new StatisticForm();
        public List<Hou2Select50_ZhouQiZhong> modelList = new List<Hou2Select50_ZhouQiZhong>();
        private int boFangYanhuaCount = 12;
        private string apiUri = "http://127.0.0.1:5000/";
        private string DataSource = "rexguan-hp2024-01";
        private MoniRunZhouQiZhong moniRunZhouQiZhong = new MoniRunZhouQiZhong();
        private MoniRunZhouQiZhongLianXu8 moniRunZhouQiZhongLianXu8 = new MoniRunZhouQiZhongLianXu8();
        private MoniRunZhouQiZhongLianXu8Amount5566 moniRunZhouQiZhongLianXu8Amount5566 = new MoniRunZhouQiZhongLianXu8Amount5566();
        public Hou3Select350YiLouSetFormZhouQiZhong()
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
            //code.NumberCondition = string.Empty;
            InitCode(code);
            //NumberConditionSet(code);
            Hou3Select350YiLouSetFormZhouQiZhongBusiness.InitCode(code);
            InitOfferNumber();
            GenerateOfferNumber();
            SetForm();
            //AddToLogFileZu6Kill1(code, "Hou2Select50.txt");
            AddToLogFileHou2Select50Auto(code);
            moniRunZhouQiZhong.Run(code);
            moniRunZhouQiZhongLianXu8.Run(code);
            moniRunZhouQiZhongLianXu8Amount5566.Run(code);

            //把记录添加到界面上 异步方式
            //AddRecordToPage(code);

            //在这里把分析后的可以推荐的号码显示到界面上

            //执行模拟挂机
            //moniKill3.Run(code);
            //moniKill3_2.Run(code);
            //moniKill3_3.Run(code);

        }
        private void InitCode(Code code)
        {
            code.GetNumberCount = (int)numHaoMa.Value;
            NumberConditionSet(code);
        }
        private void NumberConditionSet(Code code)
        {
            //设置号码的属性
            StringBuilder sb = new StringBuilder();
            sb.Append(num14B.Value.ToString() + "+" + num14E.Value.ToString());
            sb.Append(num13B.Value.ToString() + "+" + num13E.Value.ToString() + ",");
            sb.Append(num12B.Value.ToString() + "+" + num12E.Value.ToString() + ",");
            sb.Append(num11B.Value.ToString() + "+" + num11E.Value.ToString() + ",");
            sb.Append(num10B.Value.ToString() + "+" + num10E.Value.ToString() + ",");
            sb.Append(num9B.Value.ToString() + "+" + num9E.Value.ToString() + ",");
            sb.Append(num8B.Value.ToString() + "+" + num8E.Value.ToString() + ",");
            sb.Append(num7B.Value.ToString() + "+" + num7E.Value.ToString() + ",");
            sb.Append(num6B.Value.ToString() + "+" + num6E.Value.ToString() + ",");
            sb.Append(num5B.Value.ToString() + "+" + num5E.Value.ToString() + ",");
            sb.Append(num4B.Value.ToString() + "+" + num4E.Value.ToString() + ",");
            sb.Append(num3B.Value.ToString() + "+" + num3E.Value.ToString() + ",");
            sb.Append(num2B.Value.ToString() + "+" + num2E.Value.ToString() + ",");
            sb.Append(num1B.Value.ToString() + "+" + num1E.Value.ToString() + ",");
            code.NumberCondition = sb.ToString();
        }

        private async void AddRecordToPage(Code code)
        {
            var needAddList = Hou3Select350YiLouSetFormZhouQiZhongBusiness.model350List.Where(p => p.NeedZhong == false && p.GuaCount >= boFangYanhuaCount).OrderByDescending(p => p.GuaCount).ToList();
            List<YiLouSetExModel> exModelList = new List<YiLouSetExModel>();
            if (needAddList.Count > 0)
            {
                foreach (var model in needAddList)
                {
                    YiLouSetExModel exModel = new YiLouSetExModel();
                    exModel.当前期号 = code.CodeQiHao;
                    exModel.当前开奖号 = code.CodeNumber;
                    exModel.遗漏数 = model.GuaCount;
                    exModel.期号 = model.CodeQiHao;
                    exModel.开奖号 = model.CodeNumber;
                    exModel.五十码 = string.Join(" ", model.Number350);
                    exModel.数据来源 = DataSource;
                    exModelList.Add(exModel);
                }
                var data = new { data = exModelList };
                // 将对象序列化为JSON字符串
                string jsonString = JsonConvert.SerializeObject(data);

                // 设置请求内容
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                // 创建HttpClient（在实际应用中，建议使用IHttpClientFactory以避免资源耗尽）
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        // 发送POST请求
                        HttpResponseMessage response = await httpClient.PostAsync(apiUri + @"api/receive_batch_data", content);

                        // 确保请求成功（状态码为2xx）
                        response.EnsureSuccessStatusCode();

                        // 读取响应内容
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"服务器响应: {responseBody}");
                    }
                    catch (HttpRequestException ex)
                    {
                        Console.WriteLine($"请求失败: {ex.Message}");
                    }
                }
            }
        }
        public void SetForm()
        {
            if (Hou3Select350YiLouSetFormZhouQiZhongBusiness.model350List.Where(p => p.NeedZhong == false).ToList().Count > 0)
            {

                lblMaxGua.Text = Hou3Select350YiLouSetFormZhouQiZhongBusiness.model350List.Where(p => p.NeedZhong == false).Max(p => p.GuaCount).ToString();
                lblTotalNumber.Text = Hou3Select350YiLouSetFormZhouQiZhongBusiness.model350List.Count.ToString();

            }
            else
            {
                lblMaxGua.Text = "0";
                lblTotalNumber.Text = "0";
            }

            lblMaxGua2.Text = lblMaxGua.Text;
            lblTotalNumber2.Text = lblTotalNumber.Text;
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

        private void AddToLogFileHou2Select50Auto(Code code)
        {
            bool needPlay = false;
            if (Hou3Select350YiLouSetFormZhouQiZhongBusiness.model350List.Count > 500)
            {

                bool needFlush = false;

                string fileName = "Hou2Select50YiLouSet.txt";
                using (var writer = new StreamWriter(fileName, true))
                {
                    var maxNumber = Hou3Select350YiLouSetFormZhouQiZhongBusiness.model350List.Where(p => p.NeedZhong == false).Max(p => p.GuaCount);
                    if (maxNumber >= boFangYanhuaCount)
                    {
                        needFlush = true;
                        Hou3Select350YiLouSetFormZhouQiZhongBusiness.model350List.Where(p => p.GuaCount == maxNumber).ToList().ForEach(recode =>
                        {
                            writer.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 记录 #" + $"期号:{code.CodeQiHao},号码：{code.CodeNumber}，当前连挂次数{recode.GuaCount}，号码：{string.Join(" ", recode.Number350)}");
                            //writer.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 记录 #" + $"期号:{code.CodeQiHao},号码：{code.CodeNumber}，当前连挂次数{recode.GuaCount}");
                        });
                    }
                    if (needFlush)
                    {
                        writer.Flush();
                    }

                    if (maxNumber >= boFangYanhuaCount)
                    {
                        needPlay = true;
                    }
                }
            }

            if (needPlay)
            {
                lock (LockPlayObj)
                {

                    using (SoundPlayer player = new SoundPlayer(".\\data\\yanhua.wav")) // 替换为你的音乐文件路径
                    {
                        // 播放音乐
                        player.Play();
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

            var boFangYanhuaCountStr = ConfigurationManager.AppSettings["BoFangYanhuaCount"];
            apiUri = ConfigurationManager.AppSettings["apiUri"];
            DataSource = ConfigurationManager.AppSettings["DataSource"];
            if (string.IsNullOrEmpty(apiUri))
                apiUri = "http://127.0.0.1:5000/";
            if (int.TryParse(boFangYanhuaCountStr, out int count))
            {
                boFangYanhuaCount = count;
            }
            else
            {
                boFangYanhuaCount = 12;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            labelError.Text = "";
            showErrorTexttimer.Stop(); // 停止计时器
        }

        private void AutoClkTimer_Tick(object sender, EventArgs e)
        {
            //btnSelect.PerformClick();
            lblError.Text = "";
            BtnSelectClick();
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
            if (DateTime.Now >= Convert.ToDateTime("2026-01-31"))
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
                lblError.Text = "";
                // 可以获取当前行的数据
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                // ... 执行你的业务逻辑，例如根据row.Cells["SomeColumn"].Value进行不同操作
                if (row.DataBoundItem is Hou3Select350_ZhouQiZhong)
                {
                    var model = row.DataBoundItem as Hou3Select350_ZhouQiZhong;
                    txt50Number.Text = string.Join(" ", model.Number350);
                    var numberText = txt50Number.Text;
                    try
                    {
                        Clipboard.SetText(numberText);
                        lblError.Text = "号码已拷贝";
                    }
                    catch
                    {
                        lblError.Text = "拷贝失败,请手动复制";
                    }
                    finally
                    {

                    }
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txt50Number.Text = "";
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            var numberText = txt50Number.Text;
            if (string.IsNullOrEmpty(numberText))
            {
                MessageBox.Show("没有可复制的号码");
                return;
            }
            Clipboard.SetText(numberText);
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            btnSelect.Enabled = false;
            lblError.Text = "";
            txt50Number.Text = "";
            BtnSelectClick();
            btnSelect.Enabled = true;
        }

        private void BtnSelectClick()
        {
            var number = numericUpDown1.Value;
            var list = Hou3Select350YiLouSetFormZhouQiZhongBusiness.model350List.Where(p => p.NeedZhong == false && p.ZhouQiZhongHouGua >= number).OrderByDescending(p => p.ZhouQiZhongHouGua).ThenByDescending(p => p.IsZhouQiZhongHou).ToList();
            var number2 = numericUpDown2.Value;
            var guaCount = numericUpDown3.Value;
            if (number2 >= 0 && number <= number2)
            {
                list = Hou3Select350YiLouSetFormZhouQiZhongBusiness.model350List.Where(p => p.NeedZhong == false && p.ZhouQiZhongHouGua >= number && p.ZhouQiZhongHouGua <= number2).OrderByDescending(p => p.ZhouQiZhongHouGua).ThenByDescending(p => p.IsZhouQiZhongHou).ToList();
            }
            if (guaCount > -1)
            {
                list=list.Where(p=>p.GuaCount>=guaCount).ToList().OrderByDescending(p => p.ZhouQiZhongHouGua).ThenByDescending(p => p.IsZhouQiZhongHou).ToList(); ;
            }
            dataGridView1.DataSource = list;
            lblResultCount.Text = list.Count.ToString();
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            dataGridView1.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);

            //dataGridView1.Columns["GuaCount"].MinimumWidth = 100;
            //dataGridView1.Columns["CodeNumber"].MinimumWidth = 150;
            dataGridView1.Columns["IsShow"].Visible = false;
            dataGridView1.Columns["NeedZhong"].Visible = false;
            dataGridView1.Columns["ZhongGount"].Visible = false;
            dataGridView1.Columns["ZhongBeforeGua"].Visible = false;
            dataGridView1.Columns["Zhong2BeforeGua"].Visible = false;
            dataGridView1.Columns["Zhong3BeforeGua"].Visible = false;

        }

        private void btnStartAuto_Click(object sender, EventArgs e)
        {
            autoClkTimer.Start();
            lblError.Text = $"开始每{(int)numericUpDownAutoClick.Value}秒自动执行一次查询";
            btnStartAuto.Enabled = false;
            btnStopAuto.Enabled = true;
            autoClkTimer.Interval = (int)numericUpDownAutoClick.Value * 1000; // 10秒
            BtnSelectClick();
        }

        private void btnStopAuto_Click(object sender, EventArgs e)
        {
            autoClkTimer.Stop();
            lblError.Text = $"结束自动执行查询";
            btnStartAuto.Enabled = true;
            btnStopAuto.Enabled = false;
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            txtNum2.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
            txtNum2.Update();
            txtNum3.Text = "";
            txtNum3.Update();
            //int b1 = (int)num1B.Value;
            //int e1 = (int)num1E.Value;
            //int b2 = (int)num2B.Value;
            //int e2 = (int)num2E.Value;
            //int b3 = (int)num3B.Value;
            //int e3 = (int)num3E.Value;

            //int n1 = ThreadSafeRandom.Next(b1, e1);
            //int n2 = ThreadSafeRandom.Next(b2, e2);
            //int n3 = ThreadSafeRandom.Next(b3, e3);




            //txtNum1.Text = $"n1={n1},n2={n2},n3={n3}";
            //MessageBox.Show($"n1={n1},n2={n2},n3={n3}");
            int count = 0;
            while (true)
            {
                var takeCodeList = new List<string>();
                var excludeAllList = new List<string>();
                var AllCode = Hou3Select350YiLouSetFormZhouQiZhongBusiness.AllCode;
                var LeftCode = AllCode;
                txtNum1.Text = "";
                Cacl(num14B, num14E, ref LeftCode, ref excludeAllList, ref takeCodeList);
                Cacl(num13B, num13E, ref LeftCode, ref excludeAllList, ref takeCodeList);
                Cacl(num12B, num12E, ref LeftCode, ref excludeAllList, ref takeCodeList);
                Cacl(num11B, num11E, ref LeftCode, ref excludeAllList, ref takeCodeList);
                Cacl(num10B, num10E, ref LeftCode, ref excludeAllList, ref takeCodeList);
                Cacl(num9B, num9E, ref LeftCode, ref excludeAllList, ref takeCodeList);
                Cacl(num8B, num8E, ref LeftCode, ref excludeAllList, ref takeCodeList);
                Cacl(num7B, num7E, ref LeftCode, ref excludeAllList, ref takeCodeList);
                Cacl(num6B, num6E, ref LeftCode, ref excludeAllList, ref takeCodeList);
                Cacl(num5B, num5E, ref LeftCode, ref excludeAllList, ref takeCodeList);
                Cacl(num4B, num4E, ref LeftCode, ref excludeAllList, ref takeCodeList);
                Cacl(num3B, num3E, ref LeftCode, ref excludeAllList, ref takeCodeList);
                Cacl(num2B, num2E, ref LeftCode, ref excludeAllList, ref takeCodeList);
                Cacl(num1B, num1E, ref LeftCode, ref excludeAllList, ref takeCodeList);


                takeCodeList = takeCodeList.Distinct().ToList();
                excludeAllList = excludeAllList.Distinct().ToList();
                int haomaCount = (int)numHaoMa.Value;

                if (!excludeAllList.Any(item => takeCodeList.Contains(item)))
                {
                    //var numerList = NumberSelectForYiLou.Select50NumbersSafe(excludeAllList, takeCodeList);
                    var numerListList = MultiThreadedNumberSelectForYiLou.GenerateMultipleGroups(1, excludeAllList, takeCodeList, haomaCount);
                    var numberList = numerListList[0];
                    numberList = numberList.OrderBy(item => item).ToList();
                    if (numberList.Count > 0)
                    {
                        txtNum3.Text = string.Join(" ", numberList);
                        txtNum2.Text = txtNum2.Text + "\r\n" + $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff")}成功生成号码";
                        txtNum2.Update();
                        Clipboard.SetText(txtNum3.Text);
                        break;
                    }
                }
                else
                {
                    //txtNum2.Text = txtNum2.Text + "\n" + "出现重复号码，请调整参数";
                    txtNum3.Text = "";
                }
                count++;
                if (count > 1000)
                {
                    txtNum2.Text = txtNum2.Text + "\r\n" + $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff")}没有号码";
                    break;
                }
            }
        }


        public void Cacl(NumericUpDown nb, NumericUpDown ne, ref List<Code> LeftCode, ref List<string> excludeAllList, ref List<string> takeCodeList)
        {
            if (takeCodeList == null)
            {
                takeCodeList = new List<string>();
            }


            int b1 = (int)nb.Value;
            int e1 = (int)ne.Value;
            if (b1 >= 0 && e1 >= 0)
            {
                int n1 = ThreadSafeRandom.Next(b1, e1);
                txtNum1.Text = txtNum1.Text.Trim() + $"{nb.Name}={n1},";
                if (n1 > 0)
                {
                    var excludeList = GenerateHou2NumbereFromCode(n1, LeftCode);
                    excludeAllList.AddRange(excludeList);
                    LeftCode = GetCodeFromOriginExceptNumerCode(LeftCode, excludeAllList, n1);
                }
                takeCodeList.Add(LeftCode.Take(1).FirstOrDefault().GetHou2String());
                LeftCode = LeftCode.Skip(1).ToList();
            }

        }


        /// <summary>
        /// 从Code列表中获取从指定位置开始,排除掉exceptList中的号码后的Code列表
        /// </summary>
        /// <param name="codeList"></param>
        /// <param name="exceptList"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public List<Code> GetCodeFromOriginExceptNumerCode(List<Code> codeList, List<string> exceptList, int number)
        {
            int count = 0;
            for (int i = number; i < codeList.Count; i++)
            {
                var code = codeList[i];
                var hou2Str = code.GetHou2String();
                if (exceptList.Contains(hou2Str))
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            return codeList.Skip(number + count).ToList();
        }

        /// <summary>
        /// 从Code列表中生成指定数量的后2号码,已经做了滤重操作
        /// </summary>
        /// <param name="number"></param>
        /// <param name="AllCode"></param>
        /// <returns></returns>
        public static List<string> GenerateHou2NumbereFromCode(int number, List<Code> AllCode)
        {
            Dictionary<string, int> Hou2NumberCount = new Dictionary<string, int>();
            Hou2NumberCount.Clear();
            foreach (var code in AllCode)
            {
                var key = code.GetHou2String();
                if (!Hou2NumberCount.Keys.Contains(key))
                {
                    Hou2NumberCount.Add(key, 1);
                }
                else
                {
                    Hou2NumberCount[key] = Hou2NumberCount[key] + 1;
                }
                if (Hou2NumberCount.Keys.Count == number)
                {
                    break;
                }
            }
            return Hou2NumberCount.Keys.ToList();
        }

        private void BtnCopy2_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtNum3.Text))
                Clipboard.SetText(txtNum3.Text);
        }

        private void btnSetCondition_Click(object sender, EventArgs e)
        {
            int conditionNumber = (int)numCondition.Value;

        }

        private void btnyiloudijian_Click(object sender, EventArgs e)
        {
            num1B.Value = 10;
            num1E.Value = 20;
            num2B.Value = 9;
            num2E.Value = 18;
            num3B.Value = 8;
            num3E.Value = 16;
            num4B.Value = 7;
            num4E.Value = 14;
            num5B.Value = 6;
            num5E.Value = 12;
            num6B.Value = 5;
            num6E.Value = 10;
            num7B.Value = 4;
            num7E.Value = 8;
            num8B.Value = 3;
            num8E.Value = 6;
            num9B.Value = 2;
            num9E.Value = 4;
            num10B.Value = 1;
            num10E.Value = 2;
            num11B.Value = 0;
            num11E.Value = 0;
            num12B.Value = -1;
            num13E.Value = -1;
            num14B.Value = -1;
        }

        private void btn10T30Begin_Click(object sender, EventArgs e)
        {
            num1B.Value = 10;
            num1E.Value = 30;
            num2B.Value = 1;
            num2E.Value = 2;
            num3B.Value = 5;
            num3E.Value = 15;
            num4B.Value = 1;
            num4E.Value = 2;
            num5B.Value = 5;
            num5E.Value = 15;
            num6B.Value = 1;
            num6E.Value = 2;
            num7B.Value = 1;
            num7E.Value = 2;
            num8B.Value = 0;
            num8E.Value = 1;
            num9B.Value = 0;
            num9E.Value = 0;
            num10B.Value = 0;
            num10E.Value = 0;
            num11B.Value = 0;
            num11E.Value = 0;
            num12B.Value = -1;
            num13E.Value = -1;
            num14B.Value = -1;
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            var hou3List = Hou3Select350YiLouSetFormZhouQiZhongBusiness.GenerateHou3NumbereFromCode(270);
            var list = Generate350Code.Generate(hou3List);
            txtNum2.Text = "测试生成350注数据成功";
            txtNum3.Text = string.Join(" ", list.OrderBy(p => p).ToList());
            //txtNum2.Update();
            //txtNum3.Update();
            //Clipboard.SetText(txtNum3.Text);
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            moniRunZhouQiZhong.Show();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            moniRunZhouQiZhongLianXu8.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            moniRunZhouQiZhongLianXu8Amount5566.Show();
        }

        private void btnTestCode_Click(object sender, EventArgs e)
        {
            var list= txtNum3.Text.Split(' ').ToList();
            Hou3Select350_ZhouQiZhong model350 = new Hou3Select350_ZhouQiZhong();
            Code code = Hou3Select350YiLouSetFormZhouQiZhongBusiness.code;
            model350.Number350 = list;
            model350.CodeNumber = code.CodeNumber;
            model350.CodeQiHao = code.CodeQiHao;
            model350.NeedZhong = true;
            model350.KLineList = new List<KLine>();
            KLineCalc.CalcKLineHistoryList(model350, Hou3Select350YiLouSetFormZhouQiZhongBusiness.AllCode, 100);
            var result = KLineCalc.KLineIsEnough(model350.KLineList);
            if (result.Result)
            {
                txtNum2.Text = "满足条件";
            }
            else
            {
                txtNum2.Text = result.Message;
            }
        }
    }
}
