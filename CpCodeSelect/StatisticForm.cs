using CpCodeSelect.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CpCodeSelect
{
    public partial class StatisticForm : Form
    {
        private Dictionary<int, List<StatisticModel>> StatisticDic = null;
        public StatisticForm()
        {
            InitializeComponent();

        }
        public StatisticForm(Dictionary<int, List<StatisticModel>> statisticDic) : this()
        {

            this.StatisticDic = statisticDic;
        }
        public void SetStatistic(Dictionary<int, List<StatisticModel>> dic)
        {
            StatisticDic = dic;
        }


        private List<StatisticModel> GetStatisticModelList()
        {
            if (StatisticDic != null && StatisticDic.Count > 0)
            {
                var result = new List<StatisticModel>();
                foreach (var record in StatisticDic)
                {
                    result.AddRange(record.Value);
                }
                return result;
            }
            return null;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            var list = GetStatisticModelList();
            if (comboBox1.Text == "大小")
            {
                list = list.FindAll(m => m.StatisticType == "大小");
            }
            else if (comboBox1.Text == "单双")
            {
                list = list.FindAll(m => m.StatisticType == "单双");
            }

            var number = numericUpDown1.Value;
            list=list.Where(p=>p.GuaCount>= number).ToList();
            dataGridView.DataSource = list;

            // 可选：自定义列显示
            CustomizeGridViewColumns();
        }

        private void CustomizeGridViewColumns()
        {
            // 隐藏ID列
            //dataGridView.Columns["Id"].Visible = false;

            // 设置列标题
            dataGridView.Columns["CodeQiHao"].HeaderText = "期号";
            dataGridView.Columns["CodeNumber"].HeaderText = "开奖号";
            dataGridView.Columns["StatisticType"].HeaderText = "统计类型";
            dataGridView.Columns["PositionType"].HeaderText = "位置信息";
            dataGridView.Columns["GuaCount"].HeaderText = "已挂次数";
            dataGridView.Columns["PositionNumber"].Visible = false;
            //dataGridView.Columns["IsActive"].HeaderText = "是否激活";

            // 设置日期格式
            //dataGridView.Columns["BirthDate"].DefaultCellStyle.Format = "yyyy-MM-dd";

            //// 设置布尔值显示
            //dataGridView.Columns["IsActive"].DefaultCellStyle.NullValue = "否";
            //dataGridView.Columns["IsActive"].DefaultCellStyle.TrueValue = "是";
            //dataGridView.Columns["IsActive"].DefaultCellStyle.FalseValue = "否";

            // 自动调整列宽
            dataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        private void StatisticForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
    }
}
