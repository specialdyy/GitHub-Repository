using Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QiQuForm
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private static UIDataManager mgr;

        private const string COLUMN_TIMEID_NAME = "TimeId";
        private const string COLUMN_PLANNINGTIME_NAME = "PlanningTime";
        private const string COLUMN_WINNINGNUMBER_NAME = "WinningNumber";
        private const string COLUMN_ISWIN_NAME = "IsWin";
        private const string COLUMN_PLANFORECAST_NAME = "PlanForecast";

        private int columnTimeIdWidth = 100;
        private int columnPlanningTimeWidth = 250;
        private int columnWinningNumberWidth = 95;
        private int columnIsWinWidth = 115;
        private int columnPlanForecastWidth = 440;

        //复制成功的提示消息框延迟关闭的毫秒数
        private static int delayCloseMilliseconds = 1500;

        private void MainForm_Load(object sender, EventArgs e)
        {
            //不知道为什么，这里如果不加 try 块的话，有些异常无法捕捉得到，导致软件 “毫无征兆” 地关闭或没有现象。
            try
            {
                mgr = UIDataManager.GetInstance();
                mgr.OnNewWinLogAdded += Mgr_OnNewWinLogAdded;
                mgr.StartRepeatGetNewData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            this.txtNewestPeriod.Text = mgr.CurrentSelectedNewestPeriod.ToString();
            this.txtDingMaCount.Text = mgr.CurrentSelectedDingMaCount.ToString();
            InitialDataGridView();

            this.rbFirstTwo.Checked = mgr.CurrentSelectedForecastType == ForecastType.FirstTwo;
            this.rbLastTwo.Checked = !this.rbFirstTwo.Checked;
        }

        private void InitialDataGridView()
        {
            DataGridView view = this.dgvWinLog;
            view.Columns.Add(COLUMN_TIMEID_NAME, "时间Id");
            view.Columns.Add(COLUMN_PLANNINGTIME_NAME, "计划时间");
            view.Columns.Add(COLUMN_WINNINGNUMBER_NAME, "开奖号");
            view.Columns.Add(COLUMN_ISWIN_NAME, "计划结果");
            view.Columns.Add(COLUMN_PLANFORECAST_NAME, "计划预测");
            view.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle()
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                WrapMode = DataGridViewTriState.True
            };
            view.DefaultCellStyle = new DataGridViewCellStyle()
            {
                Padding = new Padding(5, 10, 5, 10),
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Font = new Font("微软雅黑", 12, FontStyle.Bold),
                WrapMode = DataGridViewTriState.True
            };
            view.Columns[COLUMN_PLANFORECAST_NAME].DefaultCellStyle = new DataGridViewCellStyle()
            {
                Alignment = DataGridViewContentAlignment.MiddleLeft,
            };

            view.RowTemplate.Height = 60;     //设置行高
            
            view.Columns[COLUMN_TIMEID_NAME].MinimumWidth = columnTimeIdWidth;                  //TimeId
            view.Columns[COLUMN_PLANNINGTIME_NAME].MinimumWidth = columnPlanningTimeWidth;      //计划时间
            view.Columns[COLUMN_WINNINGNUMBER_NAME].MinimumWidth = columnWinningNumberWidth;    //开奖号
            view.Columns[COLUMN_ISWIN_NAME].MinimumWidth = columnIsWinWidth;                    //计划结果
            view.Columns[COLUMN_PLANFORECAST_NAME].MinimumWidth = columnPlanForecastWidth;      //计划预测

            view.Columns[COLUMN_PLANFORECAST_NAME].Width = columnPlanForecastWidth;             //计划预测

            view.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            view.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;

            view.LostFocus += View_LostFocus;
        }

        private void View_LostFocus(object sender, EventArgs e)
        {
            DataGridViewRowCollection rows = this.dgvWinLog.Rows;
            foreach (DataGridViewRow row in rows)
            {
                row.Selected = false;
            }
        }

        private void Mgr_OnNewWinLogAdded(UIDataManager manager, WinLog newLog, SourceData sourceData, ReadOnlyCollection<WinLog> allLogs, int oldLogsCount)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    RefreshAllUIData(manager);
                }));
            }
        }

        private void RefreshAllUIData(UIDataManager manager)
        {
            this.lblOnlineNumberValue.Text = manager.CurrentOnlineNumber.ToFullBitString();
            this.lblWinNumberValue.Text = manager.CurrentWinningNumber.ToFullBitString();
            RefreshUIWinLogData();
            this.lblCurrentStateValue.Text = string.Format("共 {0} 期        中 {1} 期        挂 {2} 期        最大连中：{3} 期        最大连挂：{4} 期", manager.TotalWin + manager.TotalLose, manager.TotalWin, manager.TotalLose, manager.MaxContinueWinCount, manager.MaxContinueLoseCount);
            View_LostFocus(null, null);
        }

        private void RefreshUIWinLogData()
        {
            #region 新方式：
            this.dgvWinLog.Rows.Clear();
            DataGridViewRowCollection rows = this.dgvWinLog.Rows;
            string planForecastList, winNumber, current;
            //int lineLength = (mgr.CurrentSelectedDingMaCount + 1) / 2;
            int total, currentRowIndex;
            StringBuilder sb = new StringBuilder();
            foreach (WinLog log in mgr.WinLogs)
            {
                total = log.PlanForecast.Count;
                winNumber = mgr.CurrentSelectedForecastType == ForecastType.FirstTwo ? log.WinningNumber.FirstTwo() : log.WinningNumber.LastTwo();
                sb.Clear();
                sb.Append("预测定码共 " + total + " 个（排除最新期数号 " + log.ActuallyHistoryWinningNumberCount + " 个）：");
                for (int i = 0; i < total; i++)
                {
                    current = log.PlanForecast[i];
                    if (sb.Length > 0)
                    {
                        sb.Append(" ");
                    }
                    else if (total / i == 2)
                    {
                        sb.Append(Environment.NewLine);
                    }
                    sb.Append(current == winNumber ? "  [[--" + current + "--]]  " : current);
                }
                planForecastList = sb.ToString();
                currentRowIndex = rows.Add(log.TimeId, log.PlanningTime.ToString("yy-MM-dd HH:mm:ss"), log.WinningNumber.ToFullBitString(), log.IsWin, planForecastList);
                //rows[currentRowIndex].Tag = log.PlanForecast.ToList();
            }
            #endregion


            #region 旧方式：找出了问题！由于循环体的代码中写死了 “log.WinningNumber.LastTwo()”，所以无论界面上选择的是 “前二码” 还是 “后二码”，效果都是显示的后二码。
            //this.dgvWinLog.Rows.Clear();
            //string planForecastList;
            //int total;
            //foreach (WinLog log in mgr.WinLogs)
            //{
            //    total = log.PlanForecast.Count;
            //    planForecastList = "共 " + total + " 个：" + string.Join(" ", log.PlanForecast).Replace(log.WinningNumber.LastTwo(), "  [" + log.WinningNumber.LastTwo() + "]  ");
            //    this.dgvWinLog.Rows.Add(log.TimeId, log.PlanningTime, log.WinningNumber.ToFullBitString(), log.IsWin, planForecastList);
            //}
            #endregion
        }

        private void rbFirstTwo_And_rbLastTwo_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb.Checked == false)
            {
                return;
            }

            string controlName = rb.Name;
            if (controlName == "rbFirstTwo")
            {
                mgr.UpdateForecastType(ForecastType.FirstTwo);
            }
            else if (controlName == "rbLastTwo")
            {
                mgr.UpdateForecastType(ForecastType.LastTwo);
            }
            else
            {
                MessageBox.Show("触发源的控件名称出现异常！");
                return;
            }
            RefreshAllUIData(mgr);
        }

        private void dgvWinLog_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null)
            {
                return;
            }
            if (dgvWinLog.Columns[e.ColumnIndex].Name == "IsWin")
            {
                e.Value = e.Value.ToString().ToLower() == "true" ? "中" : "挂";
                dgvWinLog.Rows[e.RowIndex].DefaultCellStyle.BackColor = e.Value.ToString() == "中" ? Color.LightGreen : Color.LightPink;
            }
        }


        private void btnRefreshData_Click(object sender, EventArgs e)
        {
            #region 校验 “排除最新期数号” 和 “定码个数” 这两个值的合法性。
            string newestPeriodStr = this.txtNewestPeriod.Text ?? throw new ApplicationException("“排除最新期数号” 的值不能为空！");
            if (false == Regex.IsMatch(newestPeriodStr, @"^[1-9]+[0-9]*$"))
            {
                MessageBox.Show("“排除最新期数号” 的值格式非法！必须为不以 0 开头的非 0 正整数！");
                return;
            }
            int newestPeriod = int.Parse(newestPeriodStr);

            string dingMaString = this.txtDingMaCount.Text ?? throw new ApplicationException("“定码个数” 的值不能为空！");
            if (false == Regex.IsMatch(dingMaString, @"^[1-9]+[0-9]*$"))
            {
                MessageBox.Show("“定码个数” 的值格式非法！必须为不以 0 开头的非 0 正整数！");
                return;
            }
            int dingMaCount = int.Parse(dingMaString);

            if (dingMaCount + newestPeriod > 100)
            {
                MessageBox.Show("所设置的 “排除最新期数号” 和 “定码个数” 两个数的和不能大于 100！请重新设置！");
                return;
            }
            #endregion


            mgr.UpdateSelectedState(newestPeriod, dingMaCount);
            RefreshAllUIData(mgr);
        }

        /// <summary>
        /// “复制计划” 按钮的事件响应函数。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCopyForecastNumberList_Click(object sender, EventArgs e)
        {
            string forecastNumbers = string.Join(" ", mgr.GetNextPlanForecastList());
            Clipboard.SetText(forecastNumbers);
            AutoDelayCloseMessageBox.Show("复制成功", delayCloseMilliseconds);
            //MessageBox.Show("复制成功！");
        }

        /// <summary>
        /// 当在 “排除最新期数号” 和 “定码个数” 输入框里按回车时让程序自动点击右边的 “刷新数据” 按钮，省去再用鼠标点击。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtNewestPeriod_txtDingMaCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            //如果按下的是回车键（键值为 13）。
            if (e.KeyChar == 13)
            {
                this.btnRefreshData.Focus();
                this.btnRefreshData.PerformClick();
            }
        }

        private void dgvWinLog_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //如果用户双击的是左上角的 “全选” 单元格，则全表复制。
            if (e.ColumnIndex == -1 && e.RowIndex == -1)
            {
                Clipboard.SetText(CopyAllList());
                AutoDelayCloseMessageBox.Show("成功把当前表格的全部内容复制到剪贴板中！", delayCloseMilliseconds);
                //MessageBox.Show("成功把当前表格的全部内容复制到剪贴板中！");
                return;
            }
            //如果双击的是某一行的头部 RowHeader 时，复制当前行整行的内容到剪贴板。
            if (e.ColumnIndex == -1)
            {
                Clipboard.SetText(CopyRow(e.RowIndex, true));
                AutoDelayCloseMessageBox.Show("成功把当前行的内容复制到剪贴板中！", delayCloseMilliseconds);
                //MessageBox.Show("成功把当前行的内容复制到剪贴板中！");
                return;
            }
            //如果双击的是某一列的头部 ColumnHeader 时，不作任何处理，直接返回。
            if (e.RowIndex == -1)
            {
                return;
            }
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
            {
                DataGridViewCell currentCell = this.dgvWinLog[e.ColumnIndex, e.RowIndex];
                if (e.ColumnIndex != this.dgvWinLog.Columns[COLUMN_PLANFORECAST_NAME].Index)
                {
                    return;
                }
                //(this.dgvWinLog.Rows[e.RowIndex].Tag as List<string>)
                Clipboard.SetText(currentCell.Value.ToString());
                AutoDelayCloseMessageBox.Show("成功把当前单元格的内容复制到剪贴板中！", delayCloseMilliseconds);
                //MessageBox.Show("成功把当前单元格的内容复制到剪贴板中！");
            }
        }

        private string CopyAllList()
        {
            int rowCount = this.dgvWinLog.Rows.Count;
            DataGridView dgv = this.dgvWinLog;
            if (dgv.Rows.Count < 1)
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                sb.AppendLine(CopyRow(rowIndex, false));
            }
            return sb.ToString();
        }

        private string CopyRow(int rowIndex, bool createNewLineEveryItem)
        {
            DataGridViewRow currentRow = this.dgvWinLog.Rows[rowIndex] ?? throw new ApplicationException("找不到索引值所指定的行！");

            StringBuilder sb = new StringBuilder();
            sb.Append("时间Id = ").Append(currentRow.Cells[COLUMN_TIMEID_NAME].Value).Append(createNewLineEveryItem ? Environment.NewLine : "    ")
                .Append("计划时间 = ").Append(currentRow.Cells[COLUMN_PLANNINGTIME_NAME].Value).Append(createNewLineEveryItem ? Environment.NewLine : "    ")
                .Append("中奖号 = ").Append(currentRow.Cells[COLUMN_WINNINGNUMBER_NAME].Value).Append(createNewLineEveryItem ? Environment.NewLine : "    ")
                .Append("计划结果 = ").Append(Convert.ToBoolean(currentRow.Cells[COLUMN_ISWIN_NAME].Value) ? "中" : "挂").Append(createNewLineEveryItem ? Environment.NewLine : "    ")
                .Append("计划预测 = ").Append(currentRow.Cells[COLUMN_PLANFORECAST_NAME].Value);

            return sb.ToString();
        }

        private void dgvWinLog_Resize(object sender, EventArgs e)
        {
            DataGridView view = this.dgvWinLog;

            view.Columns[COLUMN_TIMEID_NAME].Width = columnTimeIdWidth;               //TimeId
            view.Columns[COLUMN_PLANNINGTIME_NAME].Width = columnPlanningTimeWidth;   //计划时间
            view.Columns[COLUMN_WINNINGNUMBER_NAME].Width = columnWinningNumberWidth; //开奖号
            view.Columns[COLUMN_ISWIN_NAME].Width = columnIsWinWidth;                 //计划结果
            //int totalWidth = view.Width;
            //int remainWidth = totalWidth - columnTimeIdWidth - columnPlanningTimeWidth - columnWinningNumberWidth - columnIsWinWidth - view.RowHeadersWidth;
            //view.Columns[COLUMN_PLANFORECAST_NAME].Width = remainWidth;// Math.Max(remainWidth, view.Columns[COLUMN_PLANFORECAST_NAME].GetPreferredWidth(DataGridViewAutoSizeColumnMode.AllCells, false));   //计划预测

            //int width = view.Columns[COLUMN_PLANFORECAST_NAME].GetPreferredWidth(DataGridViewAutoSizeColumnMode.AllCellsExceptHeader, false);
            int height = view.Rows[0].GetPreferredHeight(0, DataGridViewAutoSizeRowMode.AllCellsExceptHeader, false);
            //this.lblWinNumberValue.Text = "view.Columns[COLUMN_PLANFORECAST_NAME].GetPreferredWidth(DataGridViewAutoSizeColumnMode.Fill, false) = " + width + Environment.NewLine + "view.Rows[0].GetPreferredHeight(0, DataGridViewAutoSizeRowMode.AllCellsExceptHeader, false) = " + height;
        }
    }
}
