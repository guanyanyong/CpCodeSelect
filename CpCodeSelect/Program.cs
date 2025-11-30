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
            else if(str== "Form1")
            {
                Application.Run(new Form1());
            }else if (str == "Zu6Kill1")
            {

                Application.Run(new Zu6Kill1Form());
            }else if(str == "Five1Ma")
            {
                Application.Run(new Five1MaForm());
            }else if(str== "Zu6Kill1ZGGT")
            {
                Application.Run(new Zu6Kill1ZGGTForm());
            }else if(str== "Zu6Kill1ZG2")
            {
                Application.Run(new Zu6Kill1ZG2Form());

            }else if(str== "Hou2Select50_20")
            {
                Application.Run(new Hou2Select50_20Form());
            }
            else if(str== "Hou2Select50Auto")
            {
                Application.Run(new Hou2Select50AutoForm());
            }
            else if(str == "Hou2Select50YiLouSet")
            {
                Application.Run(new Hou2Select50YiLouSetForm());
            }else if(str == "Hou2Select50YiLouSetForm3guashang")
            {
                Application.Run(new Hou2Select50YiLouSetForm3guashangForm());
            }

        }
    }
}
