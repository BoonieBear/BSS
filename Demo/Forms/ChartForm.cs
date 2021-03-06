﻿using System;
using System.Windows.Forms;
using DevComponents.DotNetBar;

namespace Survey.Forms
{
    public partial class ChartForm : Office2007Form
    {
        private readonly MainForm _mf;
        private bool BinitailChart1 = false;
        private bool BinitailChart2 = false;
        private delegate void DisplayDelegate(int ChannelNumber, int DataNeedToRead, byte[] buf);
        private delegate void DisplayTitleDelegate(string title, int width);
        public ChartOption option = new ChartOption();
        public ChartForm(MainForm mf)
        {
            _mf = mf;
            InitializeComponent();
        }

        public void Initial()
        {
            Clear();
            BinitailChart1 = false;
            BinitailChart2 = false;
        }

        public void Clear()
        {
            waveLeft.Clear();
            waveRight.Clear();
            chartLeft.Clear();
            chartRight.Clear();
        }
        private void chartRight_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                PopUpOption(0);
            }
        }

        private void chartLeft_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                PopUpOption(0);
            }
        }

        private void waveRight_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                PopUpOption(0);
            }
        }

        private void waveLeft_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                PopUpOption(0);
            }
        }

        private void SideTable_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                PopUpOption(0);
            }
        }
        
        public void PopUpOption(int idx=0)
        {
            using (ChartOptionForm cof = new ChartOptionForm(option, idx))
            {
                if (cof.ShowDialog() == DialogResult.OK)
                {
                    LayOutSideChange(option.Side);
                    chartLeft.SetColor(option.StartColor,option.EndColor,option.Gamma);
                    chartRight.SetColor(option.StartColor, option.EndColor, option.Gamma);
                    chartLeft.Gain = option.Gain;
                    chartRight.Gain = option.Gain;
                    waveLeft.DisplayAmpMax = option.RawMax;
                    waveRight.DisplayAmpMax = option.RawMax;
                    //do something

                }
            }
        }
        public void ShowRaw(bool bShow = true)
        {
            if (bShow)
            {
                SideTable.RowStyles[0].SizeType = SizeType.Absolute;
                SideTable.RowStyles[0].Height = 100;
                SideTable.RowStyles[1].SizeType = SizeType.Absolute;
                SideTable.RowStyles[1].Height = 416;
            }
            else
            {
                SideTable.RowStyles[0].SizeType = SizeType.Absolute;
                SideTable.RowStyles[0].Height = 0;
                SideTable.RowStyles[1].SizeType = SizeType.Absolute;
                SideTable.RowStyles[1].Height = 416;
            }
            
        }
        private void LayOutSideChange(ShowSide side)
        {
            if (side == ShowSide.Both)
            {
                SideTable.ColumnStyles[0].SizeType = SizeType.Percent;
                SideTable.ColumnStyles[0].Width = 50;
                SideTable.ColumnStyles[1].SizeType = SizeType.Percent;
                SideTable.ColumnStyles[1].Width = 50;
                return;
            }
            if (side == ShowSide.Port)
            {
                SideTable.ColumnStyles[0].SizeType = SizeType.Percent;
                SideTable.ColumnStyles[0].Width = 100;
                SideTable.ColumnStyles[1].SizeType = SizeType.Percent;
                SideTable.ColumnStyles[1].Width = 0;
                return;
            }
            if (side == ShowSide.Starboard)
            {
                SideTable.ColumnStyles[0].SizeType = SizeType.Percent;
                SideTable.ColumnStyles[0].Width = 0;
                SideTable.ColumnStyles[1].SizeType = SizeType.Percent;
                SideTable.ColumnStyles[1].Width = 100;
                return;
            }
        }
        public void DisplayChart(int ChannelNumber, int DataNeedToRead, byte[] buf)
        {
            if (SideTable.InvokeRequired)
            {
                DisplayDelegate d = new DisplayDelegate(DisplayChart);
                this.Invoke(d, new object[] { ChannelNumber, DataNeedToRead, buf });
            }
            else
            {
                if ((option.Fq == Frequence.High && ChannelNumber == 2) ||
                    (option.Fq == Frequence.Low && ChannelNumber == 0))
                {
                    if (!BinitailChart1 || chartLeft.DisplayLength != DataNeedToRead/2)
                    {
                        chartLeft.Initialize(2, (int) DataNeedToRead/2, option.RawMax, 100);
                        waveLeft.Initialize(2, (int) DataNeedToRead/2, option.RawMax, 100);
                        LayOutSideChange(option.Side);
                        chartLeft.SetColor(option.StartColor, option.EndColor, option.Gamma);

                        chartLeft.Gain = option.Gain;

                        waveLeft.DisplayAmpMax = option.RawMax;
                        BinitailChart1 = true;
                    }

                    chartLeft.Display(buf, true);
                    if (MainForm.mf.bShowRaw)
                        waveLeft.Display(buf, false);

                }
                if ((option.Fq == Frequence.High && ChannelNumber == 3) ||
                    (option.Fq == Frequence.Low && ChannelNumber == 1))
                {
                    if (!BinitailChart2 || chartRight.DisplayLength != DataNeedToRead/2)
                    {
                        chartRight.Initialize(2, (int) DataNeedToRead/2, 4096, 100);
                        waveRight.Initialize(2, (int) DataNeedToRead/2, 4096, 100);
                        LayOutSideChange(option.Side);
                        chartRight.SetColor(option.StartColor, option.EndColor, option.Gamma);

                        chartRight.Gain = option.Gain;

                        waveRight.DisplayAmpMax = option.RawMax;
                        BinitailChart2 = true;
                    }



                    chartRight.Display(buf, false);
                    if (MainForm.mf.bShowRaw)
                        waveRight.Display(buf, false);
                }
            }
            
        }

        private void ChartForm_SizeChanged(object sender, EventArgs e)
        {

        }

        private void ChartForm_Click(object sender, EventArgs e)
        {

                PopUpOption(0);

        }

        private void ChartForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            _mf.ChildFormClose(this);
        }


        public void SetTitle(string title, int width)
        {
            if (SideTable.InvokeRequired)
            {
                DisplayTitleDelegate d = new DisplayTitleDelegate(SetTitle);
                this.Invoke(d, new object[] { title ,width });
            }
            else
            {
                this.Text = title;
                chartLeft.DisplayWidthMax = width;
                chartRight.DisplayWidthMax = width;
            }

        }

        private void 图像参数ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PopUpOption(0);
        }

        private void 控制参数ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(option.Fq == Frequence.High)
                MainForm.mf.para.Show(true);
            else
            {
                MainForm.mf.para.Show(false);
            }
        }

       
        #region menu
        
        
        
        #endregion

    }
}
