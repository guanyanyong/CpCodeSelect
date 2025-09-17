using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
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

            var str= ConfigurationManager.AppSettings["SetUpForm"];
            if(str== "Kill3")
            {
                Application.Run(new Kill3Form());
            }
            else
            {
                Application.Run(new Form1());
            }

        }
    }
}
