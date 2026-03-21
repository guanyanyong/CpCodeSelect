using CpCodeSelect.FileWatch;
using CpCodeSelect.Score;
using CpCodeSelect.Score500;
using CpCodeSelect.WuXingDuDan;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace CpCodeSelect
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var str = ConfigurationManager.AppSettings["SetUpForm"];
            if (str == "Kill3")
            {
                Application.Run(new Kill3Form());
            }
            else if (str == "Form1")
            {
                Application.Run(new Form1());
            }
            else if (str == "Zu6Kill1")
            {

                Application.Run(new Zu6Kill1Form());
            }
            else if (str == "Five1Ma")
            {
                Application.Run(new Five1MaForm());
            }
            else if (str == "Zu6Kill1ZGGT")
            {
                Application.Run(new Zu6Kill1ZGGTForm());
            }
            else if (str == "Zu6Kill1ZG2")
            {
                Application.Run(new Zu6Kill1ZG2Form());

            }
            else if (str == "Hou2Select50_20")
            {
                Application.Run(new Hou2Select50_20Form());
            }
            else if (str == "Hou2Select50Auto")
            {
                Application.Run(new Hou2Select50AutoForm());
            }
            else if (str == "Hou2Select50YiLouSet")
            {
                Application.Run(new Hou2Select50YiLouSetForm());
            }
            else if (str == "Hou2Select50YiLouSetForm3guashang")
            {
                Application.Run(new Hou2Select50YiLouSetForm3guashangForm());
            }
            else if (str == "Hou2Select50YiLouSetForm3guashang25zhu")
            {
                Application.Run(new Hou2Select50YiLouSetForm3guashang25zhu());
            }
            else if (str == "Hou2Select50YiLouSetFormZhouQiZhong")
            {
                Application.Run(new Hou2Select50YiLouSetFormZhouQiZhong());
            }
            else if (str == "Hou3Select350YiLouSetFormZhouQiZhong")
            {
                Application.Run(new Hou3Select350YiLouSetFormZhouQiZhong());
            }
            else if (str == "Hou3Select270YiLouSetFormZhouQiZhong")
            {
                Application.Run(new Hou3Select270YiLouSetFormZhouQiZhong());
            }
            else if (str == "Hou3Select350YiLouSetFormDuoZhouQiZhong")
            {
                Application.Run(new Hou3Select350YiLouSetFormDuoZhouQiZhong());
            }
            else if (str == "Hou2Select35YiLouSetForm")
            {
                Application.Run(new Hou2Select35YiLouSetForm());
            }
            else if (str == "Hou3Select350YiLouSetFormZiRanGenerate")
            {
                Application.Run(new Hou3Select350YiLouSetFormZiRanGenerate());
            }
            else if (str == "Hou3Select350YiLouSetFormXiaJiangXiaoFanTan")
            {
                Application.Run(new Hou3Select350YiLouSetFormXiaJiangXiaoFanTan());
            }
            else if (str == "Hou3Select350YiLouSetFormScoreAndChuShou")
            {
                Application.Run(new Hou3Select350YiLouSetFormScoreAndChuShou());
            }
            else if (str == "Hou3Select350YiLouSetFormMACDIndex")
            {
                Application.Run(new Hou3Select350YiLouSetFormMACDIndex());
            }
            else if (str == "Hou2Selct27Three18SetForm")
            {
                Application.Run(new Hou2Selct27Three18SetForm());
            }
            else if (str == "Hou3Select156Hot4Dan")
            {
                Application.Run(new Hou3Select156Hot4Dan());
            }
            else if (str == "Hou3Select156YiLouSetFormScoreAndChuShou")
            {
                Application.Run(new Hou3Select156YiLouSetFormScoreAndChuShou());
            }else if(str == "WuXingDuDanSetForm")
            {
                Application.Run(new WuXingDuDanSetForm());
            }else if(str== "FileWatchForm")
            {
                Application.Run(new FileWatchForm());
            }else if(str== "Hou3Select500YiLouSetFormScoreAndChuShou")
            {
                Application.Run(new Hou3Select500YiLouSetFormScoreAndChuShou());
            }
        }
    }
}
