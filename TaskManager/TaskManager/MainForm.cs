﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace TaskMenager
{
    public partial class MainForm : Form
    {
        Dictionary<int, Process> d_processes;
        readonly int ramFactor = 1024;
        readonly string suuffix = "kB";
        CommandLine cmd;
        public MainForm()
        {
            InitializeComponent();
            cmd = new CommandLine();
            KDSHDJ();
            statusStrip1.Items.Add("");
            LoadProcesses();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            //LoadProcesses();
            AddNewProcesses();
            RemoveOldProcesse();
            UpdateExistingProcesses();
            statusStrip1.Items[0].Text = $"Количесвто процессов:{listViewProcesses.Items.Count}";
        }
        void KDSHDJ()
        {
            listViewProcesses.Columns.Add("PID");//item
            listViewProcesses.Columns.Add("Name");//subitem 1
            listViewProcesses.Columns.Add("Working set");//subitem 2
            listViewProcesses.Columns.Add("Paek working set");//subitems 3
            listViewProcesses.Columns[0].TextAlign = HorizontalAlignment.Center;
        }
        void LoadProcesses()
        {
            //listViewProcesses.Items.Clear();
            //Process[] processes = Process.GetProcesses();
            //for (int i = 0; i < processes.Length; i++)
            //{
            //    ListViewItem item = new ListViewItem();
            //    item.Text = processes[i].Id.ToString();
            //    item.SubItems.Add(processes[i].ProcessName);
            //    listViewProcesses.Items.Add(item);
            //}
            d_processes = Process.GetProcesses().ToDictionary(item => item.Id, item => item);
            foreach (KeyValuePair<int, Process> i in d_processes)
            {
                //ListViewItem item= new ListViewItem();
                //item.Text = i.Key.ToString();
                //item.SubItems.Add(i.Value.ProcessName);
                //listViewProcesses.Items.Add(item);
                AddProcessToListView(i.Value);
            }
            //statusStrip1.Items[0].Text = $"Количесвто процессов:{listViewProcesses.Items.Count}";
        }
        void AddNewProcesses()
        {
            Dictionary<int, Process> d_processes = Process.GetProcesses().ToDictionary(item => item.Id, item => item);
            foreach (KeyValuePair<int, Process> i in d_processes)
            {
                if (!this.d_processes.ContainsKey(i.Key))
                {
                    //this.d_processes.Add(i.Key, i.Value);
                    AddProcessToListView(i.Value);
                }
            }
            //statusStrip1.Items[0].Text = $"Количесвто процессов:{listViewProcesses.Items.Count}";
        }
        void RemoveOldProcesse()
        {
            this.d_processes = Process.GetProcesses().ToDictionary(item => item.Id, item => item);
            for (int i = 0; i < listViewProcesses.Items.Count; i++)
            {
                //string item_name = listViewProcesses.Items[i].Name;
                if (!d_processes.ContainsKey(Convert.ToInt32(listViewProcesses.Items[i].Text)))
                {
                    listViewProcesses.Items.RemoveAt(i);
                }
            }
        }
        void UpdateExistingProcesses()
        {
            for (int i = 0; i < listViewProcesses.Items.Count; i++)
            {
                int id = Convert.ToInt32(listViewProcesses.Items[i].Text);
                // Process process = d_processes[id];
                listViewProcesses.Items[i].SubItems[2].Text = $"{(d_processes.TryGetValue(id, out var processInfo) ? processInfo.WorkingSet64 / ramFactor : 0)} {suuffix}";
                listViewProcesses.Items[i].SubItems[3].Text = $"{(d_processes.TryGetValue(id, out var processInfo2) ? processInfo2.PeakWorkingSet64 / ramFactor : 0)} {suuffix}";
            }
        }
        void AddProcessToListView(Process process)
        {
            ListViewItem item = new ListViewItem();
            item.Text = process.Id.ToString();
            item.SubItems.Add(process.ProcessName);
            item.SubItems.Add($"{process.WorkingSet64 / ramFactor} {suuffix}");
            item.SubItems.Add($"{process.PeakWorkingSet64 / ramFactor} {suuffix}");
            listViewProcesses.Items.Add(item);
        }
        void RomoveProcessFromListView(int pId)
        {
            listViewProcesses.Items.RemoveByKey(pId.ToString());
        }
        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CommandLine cmd = new CommandLine();
            cmd.ShowDialog();
        }
        int _sortColumn = -1;
        bool _ascending = true;
        private void listViewProcesses_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == _sortColumn)
            {
                _ascending = !_ascending;
            }
            else
            {
                _sortColumn = e.Column;
                _ascending = true;
            }
            listViewProcesses.ListViewItemSorter = new ListViewItemComparer(_sortColumn, _ascending);
        }
        public class ListViewItemComparer : IComparer//реализуем интерфейс IComparer
        {
            private int _column;
            private bool _ascending;
            public ListViewItemComparer(int column, bool ascending)
            {
                _column = column;
                _ascending = ascending;
            }
            public int Compare(object x, object y)
            {
                ListViewItem itemX = (ListViewItem)x;
                ListViewItem itemY = (ListViewItem)y;
                string valueX = itemX.SubItems[_column].Text;
                string valueY = itemY.SubItems[_column].Text;
                if (_column == 2 || _column == 3)
                {
                    int numX = Numbererer(valueX);
                    int numY = Numbererer(valueY);
                    if (_ascending)
                    {
                        return numY.CompareTo(numX);
                    }
                    else
                    {
                        return numX.CompareTo(numY);
                    }
                }
                else
                {
                    if (int.TryParse(valueX, out int numX) && int.TryParse(valueY, out int numY))
                    {
                        if (_ascending)
                        {
                            return numY.CompareTo(numX);
                        }
                        else
                        {
                            return numX.CompareTo(numY);
                        }
                    }
                    else
                    {
                        if (_ascending)
                        {
                            return valueY.CompareTo(valueX);
                        }
                        else
                        {
                            return valueX.CompareTo(valueY);
                        }
                    }
                }
            }
            private int Numbererer(string text)
            {
                //Извлечение числа из строки, игнорируем текст после него
                string numberStr = text.Split(' ')[0];
                return int.Parse(numberStr);
            }
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StreamWriter sw = new StreamWriter("combobox_data.txt");

            for (int i = 0; i < cmd.ComboBoxFileName.Items.Count; i++)
            {
                sw.WriteLine(cmd.ComboBoxFileName.Items[i]);
            }
            sw.Close();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(this, listViewProcesses.SelectedItems[0].Text, "Selected PID", MessageBoxButtons.OK);
            d_processes[Convert.ToInt32(listViewProcesses.SelectedItems[0].Text)].Kill();
        }
    }
}
//System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();//все запущенные процессы
//for (int i = 0; i < processes.Length; i++)
//{
//    Console.WriteLine($"{processes[i].Id}\t{processes[i].MainModule.FileName}\t");
//    //Console.WriteLine($"Name: {processes[i].ProcessName}\t");
//    //Console.WriteLine($"PID: {processes[i].Id}\t");
//    //Console.WriteLine($"Path: {processes[i].MainModule.FileName}\t");
//    //Console.WriteLine();
//}