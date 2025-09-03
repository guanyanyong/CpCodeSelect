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
    public partial class Form1 : Form
    {
        private string filePath = @"D:\Program Files (x86)\星欧挂机软件\OpenCode\TXFFC.txt";
        private FileSystemWatcher fileWatcher;
        private System.Windows.Forms.Timer showErrorTexttimer;
        //private Timer addTextTimer;
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
            var codeStrList = FileUtil.ReadFileAllRecods(filePath);
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
            DaXiaoDanShuangBusiness.InitDaXiaoDanShuang(code);
            DragonTigerBusiness.InitDragonTigerBusiness(code);

            SetAllPositionNumber(code);

            SetLianKaiHouGuaNumber(code);

            DragonTigerBusiness.SetPositionNumberDragonTiger(code);

            //在这里把分析后的可以推荐的号码显示到界面上
            AddTolistBoxTuiJian(true);

            AddToLogFile(currentCode.Wan, currentCode, "LogGuaLog");
            AddToLogFile(currentCode.Qian, currentCode, "LogGuaLog");
            AddToLogFile(currentCode.Bai, currentCode, "LogGuaLog");
            AddToLogFile(currentCode.Shi, currentCode, "LogGuaLog");
            AddToLogFile(currentCode.Ge, currentCode, "LogGuaLog");

        }


        private void AddToLogFile(PositionNumber positionNumber, Code code, string logFileName)
        {
            using (var writer = new StreamWriter(logFileName, true))
            {
                if (positionNumber.DaXiaoLianKaiGuaCount >= 6)
                {
                    writer.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 记录 #" + $"期号:{code.CodeQiHao},号码：{code.CodeNumber}，" + $"{positionNumber.PositionType}位大小连开后挂推荐:{positionNumber.DaXiaoLianGuaTuiJianNumber},推荐参考:{positionNumber.DaXiaoLianGuaTuiJianCanKao},已挂{positionNumber.DaXiaoLianKaiGuaCount}期,,当前{positionNumber.DaXiaoLianKaiGuaCount + 1}期");
                    writer.Flush();
                }
                if (positionNumber.DanShuangLianKaiGuaCount >= 6)
                {
                    writer.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 记录 #" + $"期号:{code.CodeQiHao},号码：{code.CodeNumber}，" + $"{positionNumber.PositionType}位单双连开后挂推荐:{positionNumber.DanShuangLianGuaTuiJianNumber},推荐参考:{positionNumber.DanShuangLianGuaTuiJianCanKao},已挂{positionNumber.DanShuangLianKaiGuaCount}期,,当前{positionNumber.DanShuangLianKaiGuaCount + 1}期");
                    writer.Flush();
                }
            }
        }
        private void AddTolistBoxTuiJian(bool needClear)
        {
            if (needClear)
            {
                listBoxTuiJian.Items.Clear();
            }
            AddToListBoxTuiJian(currentCode.Wan, currentCode);
            AddToListBoxTuiJian(currentCode.Qian, currentCode);
            AddToListBoxTuiJian(currentCode.Bai, currentCode);
            AddToListBoxTuiJian(currentCode.Shi, currentCode);
            AddToListBoxTuiJian(currentCode.Ge, currentCode);

            AddDragonTigerToListBoxTuiJian(currentCode);

        }
        private void AddDragonTigerToListBoxTuiJian(Code code)
        {
            if (code.DragonTigerList != null && code.DragonTigerList.Count > 0)
            {
                foreach (var dragonTiger in code.DragonTigerList)
                {
                    if (!string.IsNullOrEmpty(dragonTiger.TuiJianDragonTiger1))
                    {
                        listBoxTuiJian.Items.Add($"{dragonTiger.BeginPositoin}-{dragonTiger.EndPosition}位龙虎推荐:{dragonTiger.TuiJianDragonTiger1},当前第{dragonTiger.HeAfterTime1}期");
                    }
                    if (!string.IsNullOrEmpty(dragonTiger.DisplayMessage))
                    {
                        listBoxTuiJian.Items.Add($"{dragonTiger.BeginPositoin}-{dragonTiger.EndPosition}位龙虎信息:{dragonTiger.DisplayMessage}");

                    }
                }
            }
        }

        private void AddToListBoxTuiJian(PositionNumber positionNumber, Code code)
        {

            if (!string.IsNullOrEmpty(positionNumber.DaXiaoTuijianNumber))
            {
                var tuijianqishu = positionNumber.DaXiaoTuijianNumber == "大" ? positionNumber.DaLianKai : positionNumber.XiaoLianKai;
                listBoxTuiJian.Items.Add($"{positionNumber.PositionType}位大小推荐:{positionNumber.DaXiaoTuijianNumber},推荐原因:连开{tuijianqishu}期");
            }
            if (!string.IsNullOrEmpty(positionNumber.DanShuangTuijianNumber))
            {
                var tuijianqishu = positionNumber.DanShuangTuijianNumber == "单" ? positionNumber.DanLianKai : positionNumber.ShuangLianKai;
                listBoxTuiJian.Items.Add($"{positionNumber.PositionType}位单双推荐:{positionNumber.DanShuangTuijianNumber},推荐原因:连开{tuijianqishu}期");
            }

            if (!string.IsNullOrEmpty(positionNumber.DaXiaoLianGuaTuiJianNumber))
            {
                listBoxTuiJian.Items.Add($"{positionNumber.PositionType}位大小连开后挂推荐:{positionNumber.DaXiaoLianGuaTuiJianNumber},推荐参考:{positionNumber.DaXiaoLianGuaTuiJianCanKao},已挂{positionNumber.DaXiaoLianKaiGuaCount}期,,当前{positionNumber.DaXiaoLianKaiGuaCount + 1}期");
            }
            if (!string.IsNullOrEmpty(positionNumber.DanShuangLianGuaTuiJianNumber))
            {
                listBoxTuiJian.Items.Add($"{positionNumber.PositionType}位单双连开后挂推荐:{positionNumber.DanShuangLianGuaTuiJianNumber},推荐参考:{positionNumber.DanShuangLianGuaTuiJianCanKao},已挂{positionNumber.DanShuangLianKaiGuaCount}期,,当前{positionNumber.DanShuangLianKaiGuaCount + 1}期");
            }

            //using (var writer = new StreamWriter("log.txt", true))
            //{
            //    if (positionNumber.DaXiaoLianKaiGuaCount >= 4)
            //    {
            //        writer.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 记录 #"+$"期号:{code.CodeQiHao},号码：{code.CodeNumber}，" + $"{positionNumber.PositionType}位大小连开后挂推荐:{positionNumber.DaXiaoLianGuaTuiJianNumber},推荐参考:{positionNumber.DaXiaoLianGuaTuiJianCanKao},已挂{positionNumber.DaXiaoLianKaiGuaCount}期,,当前{positionNumber.DaXiaoLianKaiGuaCount + 1}期");
            //        writer.Flush();
            //    }
            //    if (positionNumber.DanShuangLianKaiGuaCount >= 4)
            //    {
            //        writer.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 记录 #" + $"期号:{code.CodeQiHao},号码：{code.CodeNumber}，" + $"{positionNumber.PositionType}位单双连开后挂推荐:{positionNumber.DanShuangLianGuaTuiJianNumber},推荐参考:{positionNumber.DanShuangLianGuaTuiJianCanKao},已挂{positionNumber.DanShuangLianKaiGuaCount}期,,当前{positionNumber.DanShuangLianKaiGuaCount + 1}期");
            //        writer.Flush();
            //    }
            //}

        }
        private void SetAllPositionNumber(Code code)
        {
            SetPositionNumber(code, code.Wan);
            SetPositionNumber(code, code.Qian);
            SetPositionNumber(code, code.Bai);
            SetPositionNumber(code, code.Shi);
            SetPositionNumber(code, code.Ge);
        }
        private void SetPositionNumber(Code code, PositionNumber positionNumber)
        {
            DaXiaoDanShuangBusiness.SetPositionNumberDaXiaoDanShuang(code, positionNumber);

        }
        /// <summary>
        /// 设置连开后挂次数及推荐号码
        /// </summary>
        /// <param name=""></param>
        private void SetLianKaiHouGuaNumber(Code code)
        {
            #region 先设置之前有连开的逻辑
            SetLianKaiHouGuaDiYiCi(code.PreCode == null ? null : code.PreCode.Wan, code.Wan);
            SetLianKaiHouGuaDiYiCi(code.PreCode == null ? null : code.PreCode.Qian, code.Qian);
            SetLianKaiHouGuaDiYiCi(code.PreCode == null ? null : code.PreCode.Bai, code.Bai);
            SetLianKaiHouGuaDiYiCi(code.PreCode == null ? null : code.PreCode.Shi, code.Shi);
            SetLianKaiHouGuaDiYiCi(code.PreCode == null ? null : code.PreCode.Ge, code.Ge);
            #endregion

            #region 再设置已经是连开后挂的逻辑 是否继续挂
            SetLianKaiHouGuaJiXuGua(code.PreCode == null ? null : code.PreCode.Wan, code.Wan);
            SetLianKaiHouGuaJiXuGua(code.PreCode == null ? null : code.PreCode.Qian, code.Qian);
            SetLianKaiHouGuaJiXuGua(code.PreCode == null ? null : code.PreCode.Bai, code.Bai);
            SetLianKaiHouGuaJiXuGua(code.PreCode == null ? null : code.PreCode.Shi, code.Shi);
            SetLianKaiHouGuaJiXuGua(code.PreCode == null ? null : code.PreCode.Ge, code.Ge);
            #endregion
        }
        /// <summary>
        /// 设置位置的连开后挂第一次的情况
        /// </summary>
        /// <param name="prePositionNumber"></param>
        /// <param name="positionNumber"></param>
        private void SetLianKaiHouGuaDiYiCi(PositionNumber prePositionNumber, PositionNumber positionNumber)
        {
            if (prePositionNumber == null)
            {
                return;
            }
            if (prePositionNumber.DaLianKai >= 4 && positionNumber.XiaoLianKai == 1)
            {
                //之前是大连开4期,现在开小了,设置大后挂1,推荐号码为第二期的小
                //对应的是大小小大大小大小小大

                positionNumber.DaXiaoLianKaiGuaCount = 1;
                positionNumber.DaXiaoLianGuaTuiJianNumber = "小";
                positionNumber.DaXiaoLianGuaTuiJianCanKao = "大小小大大小大小小大大小大小大小小大大";
            }
            if (prePositionNumber.XiaoLianKai >= 4 && positionNumber.DaLianKai == 1)
            {
                //之前是小连开超过4期,现在开大了,设置小后挂1,推荐号码为第二期的大
                //对应的是小大大小小大小大大小

                positionNumber.DaXiaoLianKaiGuaCount = 1;
                positionNumber.DaXiaoLianGuaTuiJianNumber = "大";
                positionNumber.DaXiaoLianGuaTuiJianCanKao = "小大大小小大小大大小小大小大小大大小小";
            }
            if (prePositionNumber.DanLianKai >= 4 && positionNumber.ShuangLianKai == 1)
            {
                //之前是单连开4期,现在开双了,设置单后挂1,推荐号码为第二期的双
                //对应的是单双双单单双单双单

                positionNumber.DanShuangLianKaiGuaCount = 1;
                positionNumber.DanShuangLianGuaTuiJianNumber = "双";
                positionNumber.DanShuangLianGuaTuiJianCanKao = "单双双单单双单双双单单双单双单双双单单";
            }
            if (prePositionNumber.ShuangLianKai >= 4 && positionNumber.DanLianKai == 1)
            {
                //之前是双连开4期,现在开单了,设置双后挂1,推荐号码为第二期的单
                //对应的是双单双双单双双单双

                positionNumber.DanShuangLianKaiGuaCount = 1;
                positionNumber.DanShuangLianGuaTuiJianNumber = "单";
                positionNumber.DanShuangLianGuaTuiJianCanKao = "双单单双双单双单单双双单双单双单单双双";
            }

        }

        private void SetLianKaiHouGuaJiXuGua(PositionNumber prePositionNumber, PositionNumber positionNumber)
        {
            if (prePositionNumber == null)
            {
                return;
            }
            if (prePositionNumber.DaXiaoLianKaiGuaCount > 0)
            {
                positionNumber.DaXiaoLianGuaTuiJianCanKao = prePositionNumber.DaXiaoLianGuaTuiJianCanKao;

                //先判断是否继续挂
                if (prePositionNumber.DaXiaoLianGuaTuiJianNumber == "大" && positionNumber.DaXiao == DaXiaoType.大)
                {
                    //上期推荐大,现在结果也是大,结束挂
                    positionNumber.DaXiaoLianKaiGuaCount = 0;
                    positionNumber.DaXiaoLianGuaTuiJianNumber = "";
                }
                else if (prePositionNumber.DaXiaoLianGuaTuiJianNumber == "大" && positionNumber.DaXiao == DaXiaoType.小)
                {
                    ////上期推荐大,现在结果是小,继续挂,连挂数加1,推荐号码设置为往后移一个
                    positionNumber.DaXiaoLianKaiGuaCount = prePositionNumber.DaXiaoLianKaiGuaCount + 1;
                    var daXiaoArray = positionNumber.DaXiaoLianGuaTuiJianCanKao.ToCharArray();
                    positionNumber.DaXiaoLianGuaTuiJianNumber = daXiaoArray[positionNumber.DaXiaoLianKaiGuaCount].ToString();

                }
                if (prePositionNumber.DaXiaoLianGuaTuiJianNumber == "小" && positionNumber.DaXiao == DaXiaoType.小)
                {
                    //推荐小,结果也是小,结束挂
                    positionNumber.DaXiaoLianKaiGuaCount = 0;
                    positionNumber.DaXiaoLianGuaTuiJianNumber = "";
                }
                else if (prePositionNumber.DaXiaoLianGuaTuiJianNumber == "小" && positionNumber.DaXiao == DaXiaoType.大)
                {
                    //推荐小,结果是大,继续挂,连挂数加1,推荐号码设置为往后移一个
                    positionNumber.DaXiaoLianKaiGuaCount = prePositionNumber.DaXiaoLianKaiGuaCount + 1;
                    var daXiaoArray = positionNumber.DaXiaoLianGuaTuiJianCanKao.ToCharArray();
                    positionNumber.DaXiaoLianGuaTuiJianNumber = daXiaoArray[positionNumber.DaXiaoLianKaiGuaCount].ToString();
                }
            }
            if (prePositionNumber.DanShuangLianKaiGuaCount > 0)
            {
                positionNumber.DanShuangLianGuaTuiJianCanKao = prePositionNumber.DanShuangLianGuaTuiJianCanKao;
                //先判断是否继续挂
                if (prePositionNumber.DanShuangLianGuaTuiJianNumber == "单" && positionNumber.DanShuang == DanShuangType.单)
                {
                    //上期推荐单,现在结果也是单,结束挂
                    positionNumber.DanShuangLianKaiGuaCount = 0;
                    positionNumber.DanShuangLianGuaTuiJianNumber = "";
                }
                else if (prePositionNumber.DanShuangLianGuaTuiJianNumber == "单" && positionNumber.DanShuang == DanShuangType.双)
                {
                    //上期推荐单,现在结果是双,继续挂,连挂数加1,推荐号码设置为往后移一个
                    positionNumber.DanShuangLianKaiGuaCount = prePositionNumber.DanShuangLianKaiGuaCount + 1;
                    var danShuangArray = positionNumber.DanShuangLianGuaTuiJianCanKao.ToCharArray();
                    positionNumber.DanShuangLianGuaTuiJianNumber = danShuangArray[positionNumber.DanShuangLianKaiGuaCount].ToString();
                }

                if (prePositionNumber.DanShuangLianGuaTuiJianNumber == "双" && positionNumber.DanShuang == DanShuangType.双)
                {
                    //推荐双,结果也是双,结束挂
                    positionNumber.DanShuangLianKaiGuaCount = 0;
                    positionNumber.DanShuangLianGuaTuiJianNumber = "";
                }
                else if (prePositionNumber.DanShuangLianGuaTuiJianNumber == "双" && positionNumber.DanShuang == DanShuangType.单)
                {
                    //推荐双,结果是单,继续挂,连挂数加1,推荐号码设置为往后移一个
                    positionNumber.DanShuangLianKaiGuaCount = prePositionNumber.DanShuangLianKaiGuaCount + 1;
                    var danShuangArray = positionNumber.DanShuangLianGuaTuiJianCanKao.ToCharArray();
                    positionNumber.DanShuangLianGuaTuiJianNumber = danShuangArray[positionNumber.DanShuangLianKaiGuaCount].ToString();
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


            //addTextTimer = new Timer();
            //addTextTimer.Interval = 100; // 0.1秒
            //addTextTimer.Tick += AddTexTimer_Tick;


        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            labelError.Text = "";
            showErrorTexttimer.Stop(); // 停止计时器
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
            if (DateTime.Now >= Convert.ToDateTime("2025-09-10"))
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

            LianKaiForm form = new LianKaiForm();
            form.ShowDialog();
        }
    }
}
