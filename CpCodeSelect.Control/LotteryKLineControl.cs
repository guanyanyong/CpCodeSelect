using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization.Formatters;
using System.Windows.Forms;
using CpCodeSelect.Model;


namespace CpCodeSelect.Control
{
    public partial class LotteryKLineControl : UserControl
    {
        private List<LotteryData> dataList;
        private Font labelFont = new Font("Arial", 8F);
        private Brush textBrush = Brushes.Black;
        private Font topMessageFont = new Font("微软雅黑", 14F, FontStyle.Bold);
        private Pen upperBandPen = new Pen(Color.Purple, 1);      // 上轨-紫色
        private Pen middleBandPen = new Pen(Color.Blue, 1);       // 中轨-蓝色
        private Pen lowerBandPen = new Pen(Color.Green, 1);       // 下轨-绿色
        private Pen kLinePen = new Pen(Color.Black, 2);           // K值线-黑色
        private Brush winBrush = Brushes.Red;                     // 中奖-红色
        private Brush loseBrush = Brushes.Blue;                   // 未中奖-蓝色
        private Pen gridPen = new Pen(Color.LightGray, 1);       // 网格线-浅灰色

        // 十字线相关属性
        private bool showCrosshair = false;
        private Point crosshairPoint;
        private Pen crosshairPen;
        private Font crosshairInfoFont = new Font("Arial", 9F, FontStyle.Bold);
        private Brush crosshairInfoBrush = Brushes.Black;

        // 顶部提示信息
        private string topMessage = "";
        // 绘图区域
        private Rectangle plotArea;
        private int margin = 60;  // 边距

        public LotteryKLineControl()
        {
            this.InitializeComponent();
            this.DoubleBuffered = true; // 启用双缓冲减少闪烁
            this.ResizeRedraw = true;
        }

        private void InitializeComponent()
        {
            this.crosshairPen = new Pen(Color.Gray, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };

            this.SuspendLayout();
            // 
            // LotteryKLineControl
            // 
            this.Name = "LotteryKLineControl";
            this.Size = new Size(800, 600);
            this.Paint += new PaintEventHandler(this.LotteryKLineControl_Paint);
            this.DoubleClick += new EventHandler(this.LotteryKLineControl_DoubleClick);
            this.MouseMove += new MouseEventHandler(this.LotteryKLineControl_MouseMove);
            this.ResumeLayout(false);
        }

        public void SetData(List<LotteryData> data)
        {
            this.dataList = data;
            this.Invalidate(); // 重绘控件
        }

        public string TopMessage
        {
            get { return topMessage; }
            set { topMessage = value; this.Invalidate(); }
        }
        private void LotteryKLineControl_Paint(object sender, PaintEventArgs e)
        {
            if (dataList == null || dataList.Count == 0)
            {
                // 如果没有数据，绘制提示信息
                string hintText = "暂无数据，请点击'数据加载'按钮";
                SizeF textSize = e.Graphics.MeasureString(hintText, new Font("Arial", 12F));
                PointF center = new PointF((this.Width - textSize.Width) / 2, (this.Height - textSize.Height) / 2);
                e.Graphics.DrawString(hintText, new Font("Arial", 12F), Brushes.Gray, center);
                return;
            }

            // 定义绘图区域（留出边距显示标签）
            plotArea = new Rectangle(
                margin,
                margin,
                this.Width - 2 * margin,
                this.Height - 2 * margin
            );

            // 绘制背景
            e.Graphics.FillRectangle(Brushes.White, this.ClientRectangle);

            // 绘制网格
            //DrawGrid(e.Graphics);
            // 绘制顶部提示信息
            if (!string.IsNullOrEmpty(topMessage))
            {
                SizeF msgSize = e.Graphics.MeasureString(topMessage, topMessageFont);
                float msgX = (this.Width - msgSize.Width) / 2; // 水平居中
                float msgY = 3; // 距离顶部5像素
                e.Graphics.DrawString(topMessage, topMessageFont, textBrush, msgX, msgY);
            }

            // 计算数值范围
            var (minValue, maxValue) = CalculateValueRange();

            // 绘制布林带
            if (double.IsNaN(minValue) || double.IsNaN(maxValue) || minValue == maxValue)
                return; // 或者给出提示
            DrawBollingerBands(e.Graphics, minValue, maxValue);

            // 绘制K值线
            //DrawKLine(e.Graphics, minValue, maxValue);

            // 绘制中奖状态格子
            DrawWinStatus(e.Graphics, minValue, maxValue);

            // 绘制图例
            //DrawLegend(e.Graphics);

            // 绘制Y轴标签
            DrawYAxisLabels(e.Graphics, minValue, maxValue);

            // 绘制X轴标签
            DrawXAxisLabels(e.Graphics);


            // 绘制十字线（如果启用）
            DrawCrosshair(e.Graphics);

            // 绘制十字线信息（如果启用）
            DrawCrosshairInfo(e.Graphics);
        }

        private (double minValue, double maxValue) CalculateValueRange()
        {
            if (dataList == null || dataList.Count == 0)
                return (0, 100);

            double min = double.MaxValue;
            double max = double.MinValue;

            foreach (var data in dataList)
            {
                // 检查值是否有效
                if (!double.IsNaN(data.KValue) && !double.IsInfinity(data.KValue))
                    min = Math.Min(min, data.KValue);
                if (!double.IsNaN(data.UpperBand) && !double.IsInfinity(data.UpperBand))
                    min = Math.Min(min, data.UpperBand);
                if (!double.IsNaN(data.LowerBand) && !double.IsInfinity(data.LowerBand))
                    min = Math.Min(min, data.LowerBand);
                if (!double.IsNaN(data.KValue) && !double.IsInfinity(data.KValue))
                    max = Math.Max(max, data.KValue);
                if (!double.IsNaN(data.UpperBand) && !double.IsInfinity(data.UpperBand))
                    max = Math.Max(max, data.UpperBand);
                if (!double.IsNaN(data.LowerBand) && !double.IsInfinity(data.LowerBand))
                    max = Math.Max(max, data.LowerBand);
            }

            // 如果min或max仍然是初始值，说明数据无效
            if (min == double.MaxValue || max == double.MinValue || min > max)
                return (0, 100);

            // 添加一些边距
            double range = max - min;
            if (range == 0) range = 10; // 防止范围为0
            min -= range * 0.1;
            max += range * 0.1;

            // 确保返回值有效
            if (double.IsNaN(min) || double.IsNaN(max) ||
                double.IsInfinity(min) || double.IsInfinity(max))
                return (0, 100);

            return (min, max);
        }

        private void DrawGrid(Graphics g)
        {
            // 绘制水平网格线
            for (int i = 0; i <= 10; i++)
            {
                int y = plotArea.Top + (i * plotArea.Height / 10);
                g.DrawLine(gridPen, plotArea.Left, y, plotArea.Right, y);
            }

            // 绘制垂直网格线
            if (dataList != null && dataList.Count > 1)
            {
                int step = Math.Max(plotArea.Width / Math.Min(dataList.Count, 20), 30); // 最多显示20条垂直线，最小间隔30像素
                for (int i = 0; i * step < plotArea.Width; i++)
                {
                    int x = plotArea.Left + i * step;
                    if (x <= plotArea.Right)
                        g.DrawLine(gridPen, x, plotArea.Top, x, plotArea.Bottom);
                }
            }
        }

        private void DrawBollingerBands(Graphics g, double minValue, double maxValue)
        {
            if (dataList.Count < 2) return;

            // 绘制上轨线
            Point[] upperPoints = new Point[dataList.Count];
            for (int i = 0; i < dataList.Count; i++)
            {
                int x = plotArea.Left + (i * plotArea.Width) / (dataList.Count - 1);
                int y = ConvertValueToY(dataList[i].UpperBand, minValue, maxValue);
                upperPoints[i] = new Point(x, y);
            }
            g.DrawLines(upperBandPen, upperPoints);

            // 绘制中轨线
            Point[] middlePoints = new Point[dataList.Count];
            for (int i = 0; i < dataList.Count; i++)
            {
                int x = plotArea.Left + (i * plotArea.Width) / (dataList.Count - 1);
                int y = ConvertValueToY(dataList[i].MiddleBand, minValue, maxValue);
                middlePoints[i] = new Point(x, y);
            }
            g.DrawLines(middleBandPen, middlePoints);

            // 绘制下轨线
            Point[] lowerPoints = new Point[dataList.Count];
            for (int i = 0; i < dataList.Count; i++)
            {
                int x = plotArea.Left + (i * plotArea.Width) / (dataList.Count - 1);
                int y = ConvertValueToY(dataList[i].LowerBand, minValue, maxValue);
                lowerPoints[i] = new Point(x, y);
            }
            g.DrawLines(lowerBandPen, lowerPoints);
        }

        private void DrawKLine(Graphics g, double minValue, double maxValue)
        {
            if (dataList.Count < 2) return;

            // 绘制K值连线
            Point[] kPoints = new Point[dataList.Count];
            for (int i = 0; i < dataList.Count; i++)
            {
                int x = plotArea.Left + (i * plotArea.Width) / (dataList.Count - 1);
                int y = ConvertValueToY(dataList[i].KValue, minValue, maxValue);
                kPoints[i] = new Point(x, y);
            }
            g.DrawLines(kLinePen, kPoints);
        }

        private void DrawWinStatus(Graphics g, double minValue, double maxValue)
        {
            //int boxWidth = Math.Max(4, Math.Min(10, plotArea.Width / Math.Max(dataList.Count, 10))); // 确保格子不会太大或重叠
            ////boxWidth -= 1;
            //double baseHeight = 7.5; // 基础高度

            //// 为每个数据点绘制一个柱状图
            //for (int i = 0; i < dataList.Count; i++)
            //{
            //    int x = plotArea.Left + (i * plotArea.Width) / Math.Max(dataList.Count - 1, 1);

            //    // 使用数据中的HeightFactor来确定柱子高度
            //    double heightFactor = dataList[i].LostHeightFactor;
            //    int height = (int)(baseHeight * Math.Abs(heightFactor));

            //    // 确定柱子绘制的起始位置和方向
            //    int kValueY = ConvertValueToY(dataList[i].KValue, minValue, maxValue);
            //    Brush brush;
            //    int topY;

            //    if (dataList[i].IsWin)
            //    {
            //        // 中奖：红格子从K值往下绘制（向下）
            //        brush = winBrush;
            //        topY = kValueY; // 从K值线开始向下绘制
            //        height = (int)(baseHeight * Math.Abs(1.857));
            //    }
            //    else
            //    {
            //        // 未中奖：蓝格子从K值往上绘制（向上）
            //        brush = loseBrush;
            //        topY = kValueY - height; // 从K值线往上绘制
            //    }

            //    Pen pen = Pens.Black;

            //    // 绘制柱子（矩形）
            //    Rectangle rect = new Rectangle(x - boxWidth / 2, topY, boxWidth, height);

            //    g.FillRectangle(brush, rect);
            //    g.DrawRectangle(pen, rect);
            //}


            int boxWidth = Math.Max(4, Math.Min(10, plotArea.Width / Math.Max(dataList.Count, 10))); // 确保格子不会太大或重叠
            boxWidth = 3;
            // 为每个数据点绘制一个柱状图，表示从上一期K值到本期K值的变化
            for (int i = 0; i < dataList.Count; i++)
            {
                int x = plotArea.Left + (i * plotArea.Width) / Math.Max(dataList.Count - 1, 1);

                // 确定当前期和上一期的K值
                double currentKValue = dataList[i].KValue;
                double previousKValue = currentKValue; // 如果是第一期，则与自身比较

                if (i > 0)
                {
                    previousKValue = dataList[i - 1].KValue;
                }

                // 检查数值是否有效
                if (double.IsNaN(currentKValue) || double.IsInfinity(currentKValue) ||
                    double.IsNaN(previousKValue) || double.IsInfinity(previousKValue))
                    continue; // 跳过无效数据

                // 计算起始和结束的Y坐标
                int previousKValueY = ConvertValueToY(previousKValue, minValue, maxValue);
                int currentKValueY = ConvertValueToY(currentKValue, minValue, maxValue);

                // 根据K值变化确定格子的起始位置和高度
                int topY = Math.Min(previousKValueY, currentKValueY);
                int height = Math.Abs(currentKValueY - previousKValueY);

                // 确保高度有效
                if (height <= 0)
                    continue; // 跳过高度为0或负数的情况

                // 确定画笔和画刷
                Brush brush = dataList[i].IsWin ? winBrush : loseBrush; // 中奖用红格子，未中奖用蓝格子
                Pen pen = Pens.Black;

                // 绘制柱子（矩形），表示从上一期K值到本期K值的变化
                Rectangle rect = new Rectangle(x - boxWidth / 2, topY, boxWidth, height);

                g.FillRectangle(brush, rect);
                g.DrawRectangle(pen, rect);
            }
        }

        private int ConvertValueToY(double value, double minValue, double maxValue)
        {
            // 处理边界情况
            if (double.IsNaN(value) || double.IsInfinity(value))
                return plotArea.Bottom;

            if (maxValue == minValue || double.IsNaN(maxValue) || double.IsNaN(minValue) ||
                double.IsInfinity(maxValue) || double.IsInfinity(minValue))
                return plotArea.Bottom;

            // 将数值转换为Y坐标（注意：Y轴向下递增，所以需要反转）
            double normalized = (value - minValue) / (maxValue - minValue);

            // 再次检查normalized是否有效
            if (double.IsNaN(normalized) || double.IsInfinity(normalized))
                return plotArea.Bottom;

            return plotArea.Bottom - (int)(normalized * plotArea.Height);
        }

        private void DrawYAxisLabels(Graphics g, double minValue, double maxValue)
        {
            // 绘制Y轴标签
            for (int i = 0; i <= 5; i++)
            {
                double value = minValue + (maxValue - minValue) * i / 5;
                int y = ConvertValueToY(value, minValue, maxValue);

                string label = value.ToString("F1");
                g.DrawString(label, labelFont, textBrush,
                    new PointF(plotArea.Left - 40, y - 8));

                // 绘制Y轴刻度线
                g.DrawLine(Pens.Black, plotArea.Left - 5, y, plotArea.Left, y);
            }
        }

        private void DrawXAxisLabels(Graphics g)
        {
            if (dataList == null || dataList.Count == 0) return;

            // 绘制X轴标签
            int labelCount = Math.Min(10, dataList.Count); // 最多显示10个标签
            for (int i = 0; i < labelCount; i++)
            {
                int dataIndex = (i * (dataList.Count - 1)) / Math.Max(1, labelCount - 1);
                int x = plotArea.Left + (dataIndex * plotArea.Width) / Math.Max(dataList.Count - 1, 1);

                string label = $"期{dataIndex + 1}";
                g.DrawString(label, labelFont, textBrush,
                    new PointF(x - 10, plotArea.Bottom + 5));

                // 绘制X轴刻度线
                g.DrawLine(Pens.Black, x, plotArea.Bottom, x, plotArea.Bottom + 5);
            }
        }

        private void DrawLegend(Graphics g)
        {
            // 绘制图例
            int legendX = plotArea.Right - 150;
            int legendY = plotArea.Top + 10;

            // 中奖图例
            g.FillRectangle(winBrush, legendX, legendY, 12, 12);
            g.DrawRectangle(Pens.Black, legendX, legendY, 12, 12);
            g.DrawString("中奖", labelFont, textBrush, legendX + 18, legendY);

            // 未中奖图例
            g.FillRectangle(loseBrush, legendX, legendY + 20, 12, 12);
            g.DrawRectangle(Pens.Black, legendX, legendY + 20, 12, 12);
            g.DrawString("未中奖", labelFont, textBrush, legendX + 18, legendY + 20);

            // 布林带图例
            g.DrawLine(upperBandPen, legendX, legendY + 40, legendX + 12, legendY + 40);
            g.DrawString("上轨", labelFont, textBrush, legendX + 18, legendY + 35);

            g.DrawLine(middleBandPen, legendX, legendY + 55, legendX + 12, legendY + 55);
            g.DrawString("中轨", labelFont, textBrush, legendX + 18, legendY + 50);

            g.DrawLine(lowerBandPen, legendX, legendY + 70, legendX + 12, legendY + 70);
            g.DrawString("下轨", labelFont, textBrush, legendX + 18, legendY + 65);

            // K值线图例
            g.DrawLine(kLinePen, legendX, legendY + 85, legendX + 12, legendY + 85);
            g.DrawString("K值", labelFont, textBrush, legendX + 18, legendY + 80);
        }

        private void DrawCrosshair(Graphics g)
        {
            if (!showCrosshair) return;

            // 绘制水平虚线（X轴虚线）
            g.DrawLine(crosshairPen, plotArea.Left, crosshairPoint.Y, plotArea.Right, crosshairPoint.Y);

            // 绘制垂直虚线（Y轴虚线）
            g.DrawLine(crosshairPen, crosshairPoint.X, plotArea.Top, crosshairPoint.X, plotArea.Bottom);
        }

        private void LotteryKLineControl_DoubleClick(object sender, EventArgs e)
        {
            // 切换十字线显示状态
            showCrosshair = !showCrosshair;
            this.Invalidate(); // 重绘控件
        }

        private void LotteryKLineControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (showCrosshair)
            {
                // 更新十字线位置
                crosshairPoint = e.Location;
                this.Invalidate(); // 重绘控件
            }
        }

        private void DrawCrosshairInfo(Graphics g)
        {
            if (!showCrosshair || dataList == null || dataList.Count == 0) return;

            // 计算当前X坐标对应的期数
            int index = GetDataIndexFromX(crosshairPoint.X);
            if (index >= 0 && index < dataList.Count)
            {
                var data = dataList[index];
                // 获取当前数据的Y值（K值对应的Y坐标）
                var (minValue, maxValue) = CalculateValueRange();
                int kValueY = ConvertValueToY(data.KValue, minValue, maxValue);

                // 分别显示信息的每一行
                string periodText = $"期号: {data.Index + 1}";
                string kValueText = $"K值: {data.KValue:F2}";
                string winText = $"中奖: {(data.IsWin ? "是" : "否")}";
                string periodNumberText = $"开奖期号: {data.PeriodNumber}";
                string winningNumbersText = $"开奖号: {data.WinningNumbers}";

                // 测量字体高度以确定行间距
                SizeF textSize = g.MeasureString("Ay", crosshairInfoFont);
                int lineHeight = (int)textSize.Height + 2;

                // 在左上角绘制信息背景（现在有5行文本）
                int infoBoxHeight = 5 + lineHeight * 3 + 5; // 上边距 + 5行文本 + 下边距
                g.FillRectangle(Brushes.LightYellow, new Rectangle(5, 5, 180, infoBoxHeight));
                g.DrawRectangle(Pens.Black, new Rectangle(5, 5, 180, infoBoxHeight));

                // 分别绘制每一行文本
                g.DrawString(periodText, crosshairInfoFont, crosshairInfoBrush, new Point(10, 10));
                g.DrawString(winningNumbersText, crosshairInfoFont, crosshairInfoBrush, new Point(10, 10 + lineHeight));
                g.DrawString(kValueText, crosshairInfoFont, crosshairInfoBrush, new Point(10, 10 + lineHeight*2));
                //g.DrawString(winText, crosshairInfoFont, crosshairInfoBrush, new Point(10, 10 + lineHeight * 2));
                //g.DrawString(periodNumberText, crosshairInfoFont, crosshairInfoBrush, new Point(10, 10 + lineHeight * 3));
            }
        }

        private int GetDataIndexFromX(int mouseX)
        {
            if (dataList == null || dataList.Count == 0 || !plotArea.Contains(mouseX, plotArea.Top)) return -1;

            // 计算鼠标X坐标在数据范围内的索引
            double relativePosition = (double)(mouseX - plotArea.Left) / plotArea.Width;
            int index = (int)(relativePosition * (dataList.Count - 1));

            // 确保索引有效
            index = Math.Max(0, Math.Min(index, dataList.Count - 1));
            return index;
        }
    }
}