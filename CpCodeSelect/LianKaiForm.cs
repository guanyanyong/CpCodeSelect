using CpCodeSelect.Model;
using CpCodeSelect.Util;
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

namespace CpCodeSelect
{
    public partial class LianKaiForm : Form
    {
        string txtFilePath = "D:\\code\\cp\\CpCodeSelect\\CpCodeSelect\\data\\2025-09-01_Txqq1f.txt";

        private Code lastCode;
        private Code currentCode;
        private Object lockObj = new Object();

        public LianKaiForm()
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
                txtFilePath = "D:\\code\\cp\\CpCodeSelect\\CpCodeSelect\\data\\2025-09-01_Txqq1f.txt";
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
            code.Wan = new PositionNumber
            {
                PositionType = PositionType.万,
                Number = int.Parse(code.CodeNumber[0].ToString())
            };
            code.Qian = new PositionNumber
            {
                PositionType = PositionType.千,
                Number = int.Parse(code.CodeNumber[1].ToString())
            };
            code.Bai = new PositionNumber
            {
                PositionType = PositionType.百,
                Number = int.Parse(code.CodeNumber[2].ToString())
            };
            code.Shi = new PositionNumber
            {
                PositionType = PositionType.十,
                Number = int.Parse(code.CodeNumber[3].ToString())
            };
            code.Ge = new PositionNumber
            {
                PositionType = PositionType.个,
                Number = int.Parse(code.CodeNumber[4].ToString())
            };

            SetAllPositionNumber(code);

            SetLianKaiHouGuaNumber(code);

            //在这里把分析后的可以推荐的号码显示到界面上
            //AddTolistBoxTuiJian(true);
            AddToFile(currentCode.Wan, currentCode);
            AddToFile(currentCode.Qian, currentCode);
            AddToFile(currentCode.Bai, currentCode);
            AddToFile(currentCode.Shi, currentCode);
            AddToFile(currentCode.Ge, currentCode);


        }

        private void AddToFile(PositionNumber positionNumber, Code code)
        {
            using (var writer = new StreamWriter("log.txt", true))
            {
                if (positionNumber.DaXiaoLianKaiGuaCount >= 11)
                {
                    writer.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 记录 #" + $"期号:{code.CodeQiHao},号码：{code.CodeNumber}，" + $"{positionNumber.PositionType}位大小连开后挂推荐:{positionNumber.DaXiaoLianGuaTuiJianNumber},推荐参考:{positionNumber.DaXiaoLianGuaTuiJianCanKao},已挂{positionNumber.DaXiaoLianKaiGuaCount}期,,当前{positionNumber.DaXiaoLianKaiGuaCount + 1}期");
                    writer.Flush();
                }
                if (positionNumber.DanShuangLianKaiGuaCount >= 11)
                {
                    writer.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 记录 #" + $"期号:{code.CodeQiHao},号码：{code.CodeNumber}，" + $"{positionNumber.PositionType}位单双连开后挂推荐:{positionNumber.DanShuangLianGuaTuiJianNumber},推荐参考:{positionNumber.DanShuangLianGuaTuiJianCanKao},已挂{positionNumber.DanShuangLianKaiGuaCount}期,,当前{positionNumber.DanShuangLianKaiGuaCount + 1}期");
                    writer.Flush();
                }
            }
        }


        private void SetAllPositionNumber(Code code)
        {
            SetPositionNumber(code, code.Wan);
            SetPositionNumber(code, code.Qian);
            SetPositionNumber(code, code.Bai);
            SetPositionNumber(code, code.Shi);
            SetPositionNumber(code, code.Ge);
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

        private void SetPositionNumber(Code code, PositionNumber positionNumber)
        {
            SetPositionNumberDaXiaoDanShuang(code, positionNumber);

        }
        /// <summary>
        /// 设置位置的大小单双属性
        /// </summary>
        /// <param name="code"></param>
        /// <param name="positionNumber"></param>
        private void SetPositionNumberDaXiaoDanShuang(Code code, PositionNumber positionNumber)
        {
            positionNumber.DaXiao = positionNumber.Number > 4 ? DaXiaoType.大 : DaXiaoType.小;
            positionNumber.DanShuang = positionNumber.DanShuang = (positionNumber.Number % 2 == 0) ? DanShuangType.双 : DanShuangType.单;
            if (code.PreCode == null)
            {
                // 如果没有上一期号码，初始化连开次数
                //设置大小
                if (positionNumber.DaXiao == DaXiaoType.大)
                {
                    positionNumber.DaLianKai = 1;
                    positionNumber.XiaoLianKai = 0;
                }
                else
                {
                    positionNumber.DaLianKai = 0;
                    positionNumber.XiaoLianKai = 1;
                }
                //设置单双
                if (positionNumber.DanShuang == DanShuangType.单)
                {

                    positionNumber.DanLianKai = 1;
                    positionNumber.ShuangLianKai = 0;
                }
                else
                {
                    positionNumber.DanLianKai = 0;
                    positionNumber.ShuangLianKai = 1;
                }
            }
            else
            {
                //如果有上一期号码,需要设置大小单双的连开数值
                //设置大小
                if (positionNumber.DaXiao == DaXiaoType.大)
                {
                    var preDaxiao = DaXiaoType.大;
                    int preDaxiaoLianKai = 0;
                    if (positionNumber.PositionType == PositionType.万)
                    {
                        preDaxiao = code.PreCode.Wan.DaXiao;
                        preDaxiaoLianKai = code.PreCode.Wan.DaLianKai;
                    }
                    if (positionNumber.PositionType == PositionType.千)
                    {
                        preDaxiao = code.PreCode.Qian.DaXiao;
                        preDaxiaoLianKai = code.PreCode.Qian.DaLianKai;
                    }
                    if (positionNumber.PositionType == PositionType.百)
                    {
                        preDaxiao = code.PreCode.Bai.DaXiao;
                        preDaxiaoLianKai = code.PreCode.Bai.DaLianKai;
                    }
                    if (positionNumber.PositionType == PositionType.十)
                    {
                        preDaxiao = code.PreCode.Shi.DaXiao;
                        preDaxiaoLianKai = code.PreCode.Shi.DaLianKai;
                    }
                    if (positionNumber.PositionType == PositionType.个)
                    {
                        preDaxiao = code.PreCode.Ge.DaXiao;
                        preDaxiaoLianKai = code.PreCode.Ge.DaLianKai;
                    }
                    if (preDaxiao == DaXiaoType.大)
                    {
                        positionNumber.DaLianKai = preDaxiaoLianKai + 1;
                        positionNumber.XiaoLianKai = 0;
                    }
                    else
                    {
                        positionNumber.DaLianKai = 1;
                        positionNumber.XiaoLianKai = 0;
                    }
                }
                else
                {
                    var preDaxiao = DaXiaoType.小;
                    int preDaxiaoLianKai = 0;
                    if (positionNumber.PositionType == PositionType.万)
                    {
                        preDaxiao = code.PreCode.Wan.DaXiao;
                        preDaxiaoLianKai = code.PreCode.Wan.XiaoLianKai;
                    }
                    if (positionNumber.PositionType == PositionType.千)
                    {
                        preDaxiao = code.PreCode.Qian.DaXiao;
                        preDaxiaoLianKai = code.PreCode.Qian.XiaoLianKai;
                    }
                    if (positionNumber.PositionType == PositionType.百)
                    {
                        preDaxiao = code.PreCode.Bai.DaXiao;
                        preDaxiaoLianKai = code.PreCode.Bai.XiaoLianKai;
                    }
                    if (positionNumber.PositionType == PositionType.十)
                    {
                        preDaxiao = code.PreCode.Shi.DaXiao;
                        preDaxiaoLianKai = code.PreCode.Shi.XiaoLianKai;
                    }
                    if (positionNumber.PositionType == PositionType.个)
                    {
                        preDaxiao = code.PreCode.Ge.DaXiao;
                        preDaxiaoLianKai = code.PreCode.Ge.XiaoLianKai;
                    }
                    if (preDaxiao == DaXiaoType.小)
                    {
                        positionNumber.XiaoLianKai = preDaxiaoLianKai + 1;
                        positionNumber.DaLianKai = 0;
                    }
                    else
                    {
                        positionNumber.XiaoLianKai = 1;
                        positionNumber.DaLianKai = 0;
                    }
                }
                //设置单双
                if (positionNumber.DanShuang == DanShuangType.单)
                {
                    var preDanShuang = DanShuangType.单;
                    int preDanShuangLianKai = 0;
                    if (positionNumber.PositionType == PositionType.万)
                    {
                        preDanShuang = code.PreCode.Wan.DanShuang;
                        preDanShuangLianKai = code.PreCode.Wan.DanLianKai;
                    }
                    if (positionNumber.PositionType == PositionType.千)
                    {
                        preDanShuang = code.PreCode.Qian.DanShuang;
                        preDanShuangLianKai = code.PreCode.Qian.DanLianKai;
                    }
                    if (positionNumber.PositionType == PositionType.百)
                    {
                        preDanShuang = code.PreCode.Bai.DanShuang;
                        preDanShuangLianKai = code.PreCode.Bai.DanLianKai;
                    }
                    if (positionNumber.PositionType == PositionType.十)
                    {
                        preDanShuang = code.PreCode.Shi.DanShuang;
                        preDanShuangLianKai = code.PreCode.Shi.DanLianKai;
                    }
                    if (positionNumber.PositionType == PositionType.个)
                    {
                        preDanShuang = code.PreCode.Ge.DanShuang;
                        preDanShuangLianKai = code.PreCode.Ge.DanLianKai;
                    }
                    if (preDanShuang == DanShuangType.单)
                    {
                        positionNumber.DanLianKai = preDanShuangLianKai + 1;
                        positionNumber.ShuangLianKai = 0;
                    }
                    else
                    {
                        positionNumber.DanLianKai = 1;
                        positionNumber.ShuangLianKai = 0;
                    }
                }
                else
                {
                    var preDanShuang = DanShuangType.双;
                    int preDanShuangLianKai = 0;
                    if (positionNumber.PositionType == PositionType.万)
                    {
                        preDanShuang = code.PreCode.Wan.DanShuang;
                        preDanShuangLianKai = code.PreCode.Wan.ShuangLianKai;
                    }
                    if (positionNumber.PositionType == PositionType.千)
                    {
                        preDanShuang = code.PreCode.Qian.DanShuang;
                        preDanShuangLianKai = code.PreCode.Qian.ShuangLianKai;
                    }
                    if (positionNumber.PositionType == PositionType.百)
                    {
                        preDanShuang = code.PreCode.Bai.DanShuang;
                        preDanShuangLianKai = code.PreCode.Bai.ShuangLianKai;
                    }
                    if (positionNumber.PositionType == PositionType.十)
                    {
                        preDanShuang = code.PreCode.Shi.DanShuang;
                        preDanShuangLianKai = code.PreCode.Shi.ShuangLianKai;
                    }
                    if (positionNumber.PositionType == PositionType.个)
                    {
                        preDanShuang = code.PreCode.Ge.DanShuang;
                        preDanShuangLianKai = code.PreCode.Ge.ShuangLianKai;
                    }
                    if (preDanShuang == DanShuangType.双)
                    {
                        positionNumber.ShuangLianKai = preDanShuangLianKai + 1;
                        positionNumber.DanLianKai = 0;
                    }
                    else
                    {
                        positionNumber.ShuangLianKai = 1;
                        positionNumber.DanLianKai = 0;
                    }
                }

                //根据大小单双的连开次数,设置推荐号码
                //todo 目前设置的是连开4期推荐,后续可以更改为可配置
                if (positionNumber.XiaoLianKai >= 4)
                {
                    positionNumber.DaXiaoTuijianNumber = "小";
                }
                else if (positionNumber.DaLianKai >= 4)
                {
                    positionNumber.DaXiaoTuijianNumber = "大";
                }

                if (positionNumber.DanLianKai >= 4)
                {
                    positionNumber.DanShuangTuijianNumber = "单";
                }
                else if (positionNumber.ShuangLianKai >= 4)
                {
                    positionNumber.DanShuangTuijianNumber = "双";
                }
            }
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
                positionNumber.DaXiaoLianGuaTuiJianNumber = "大";
                positionNumber.DaXiaoLianGuaTuiJianCanKao = "大大大大大大大大大大大大大大大大大大大大大大大大大大大大大大大大大大大大大大大大大大大大大";
            }
            if (prePositionNumber.XiaoLianKai >= 4 && positionNumber.DaLianKai == 1)
            {
                //之前是小连开超过4期,现在开大了,设置小后挂1,推荐号码为第二期的小
                //对应的是小大大小小大小大大小

                positionNumber.DaXiaoLianKaiGuaCount = 1;
                positionNumber.DaXiaoLianGuaTuiJianNumber = "小";
                positionNumber.DaXiaoLianGuaTuiJianCanKao = "小小小小小小小小小小小小小小小小小小小小小小小小小小小小小小小小小小小小小小小小小小小小小小";

            }
            if (prePositionNumber.DanLianKai >= 4 && positionNumber.ShuangLianKai == 1)
            {
                //之前是单连开4期,现在开双了,设置单后挂1,推荐号码为第二期的双
                //对应的是单双双单单双单双单

                positionNumber.DanShuangLianKaiGuaCount = 1;
                positionNumber.DanShuangLianGuaTuiJianNumber = "单";
                positionNumber.DanShuangLianGuaTuiJianCanKao = "单单单单单单单单单单单单单单单单单单单单单单单单单单单单单单单单单单单单单单单单单单单单单单单单单单单单单单单单单单单单单单单单";
            }
            if (prePositionNumber.ShuangLianKai >= 4 && positionNumber.DanLianKai == 1)
            {
                //之前是双连开4期,现在开单了,设置双后挂1,推荐号码为第二期的单
                //对应的是双单双双单双双单双

                positionNumber.DanShuangLianKaiGuaCount = 1;
                positionNumber.DanShuangLianGuaTuiJianNumber = "双";
                positionNumber.DanShuangLianGuaTuiJianCanKao = "双双双双双双双双双双双双双双双双双双双双双双双双双双双双双双双双双双双双双双双双双双双双双双双双双双";
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

        private void button1_Click_1(object sender, EventArgs e)
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
    }
}
