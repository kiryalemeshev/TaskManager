using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics; //!!!
using System.Management; //!!!
using Microsoft.VisualBasic; //!!!

namespace Kurs
{
    internal class Class1
    {


        private List<Process> processes = null; //Главный список хранения процессов

        private ListViewItemComparer comparer = null;



        private void GetProcesses() //метод заполнения списка и его заполнения
        {
            processes.Clear(); //очистка списка

            processes = Process.GetProcesses().ToList<Process>();     //заполнение списка заново и получаем все системные процессы GetProcesses()
        }

        private void RefreshProcessesList() //метод заполнения ListView контента
        {

            listView1.Items.Clear(); //Очистка

            double memSize = 0; //В этой переменной будет при переборе всех процессов храниться память, которую занимает процессор

            foreach (Process p in processes)
            { //Перебор всех процессов

                memSize = 0;

                PerformanceCounter pc = new PerformanceCounter();
                pc.CategoryName = "Process";
                pc.CounterName = "Working Set - Private";
                pc.InstanceName = p.ProcessName;

                memSize = (double)pc.NextValue() / (1000 * 1000);

                string[] row = new string[]
                {p.ProcessName.ToString(), Math.Round(memSize,1).ToString()
                };

                listView1.Items.Add(new ListViewItem(row));

                pc.Close();
                pc.Dispose();
            }

            Text = "Запущено процессов: " + processes.Count.ToString();

        }


        private void RefreshProcessesList(List<Process> processes, string keyword)
        {
            try
            {
                listView1.Items.Clear();

                double memSize = 0;

                foreach (Process p in processes)
                {

                    if (p != null)
                    {
                        memSize = 0;

                        PerformanceCounter pc = new PerformanceCounter();
                        pc.CategoryName = "Process";
                        pc.CounterName = "Working Set - Private";
                        pc.InstanceName = p.ProcessName;

                        memSize = (double)pc.NextValue() / (1000 * 1000);

                        string[] row = new string[]
                        {
                            p.ProcessName.ToString(), Math.Round(memSize,1).ToString()
                        };

                        listView1.Items.Add(new ListViewItem(row));

                        pc.Close();
                        pc.Dispose();
                    }

                }

                Text = $"Запущено процессов '{keyword}': " + processes.Count.ToString();
            }
            catch (Exception) { }

        }




        private void KillProcesses(Process processes)
        {
            processes.Kill();

            processes.WaitForExit();
        }


        private void KillProcessesAndChildren(int pid)
        {
            if (pid == 0)
                return;


            ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                "Select + From Win32_Process Where ParentProcessID=" + pid);

            ManagementObjectCollection objectsCollection = searcher.Get();

            foreach (ManagementObject obj in objectsCollection)
            {
                KillProcessesAndChildren(Convert.ToInt32(obj["ProcessId"]));
            }

            try
            {

                Process p = Process.GetProcessById(pid);

                p.Kill();
                p.WaitForExit();

            }
            catch (ArgumentException) { }
        }


        private int GetParentProccesId(Process p)
        {
            int parentId = 0;

            try
            {
                ManagementObject managementObject = new ManagementObject("win32_procces.handle='" + p.Id + "'");

                managementObject.Get();

                parentId = Convert.ToInt32(managementObject["ParentProcessId"]);
            }
            catch (Exception) { }
            return parentId;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            processes = new List<Process>();

            GetProcesses();
            RefreshProcessesList();


            comparer = new ListViewItemComparer();
            comparer.ColumnIndex = 0;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            GetProcesses(); RefreshProcessesList();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems[0] != null)
                {
                    Process processToKill = processes.Where((x) => x.ProcessName ==
                    listView1.SelectedItems[0].SubItems[0].Text).ToList()[0];

                    KillProcesses(processToKill);

                    GetProcesses(); RefreshProcessesList();
                }
            }
            catch (Exception) { }

        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems[0] != null)
                {
                    Process processToKill = processes.Where((x) => x.ProcessName ==
                    listView1.SelectedItems[0].SubItems[0].Text).ToList()[0];

                    KillProcessesAndChildren(GetParentProccesId(processToKill));

                    GetProcesses(); RefreshProcessesList();


                }
            }
            catch (Exception) { }
        }

        private void завершитьДеревоПроцессовToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems[0] != null)
                {
                    Process processToKill = processes.Where((x) => x.ProcessName ==
                    listView1.SelectedItems[0].SubItems[0].Text).ToList()[0];

                    KillProcessesAndChildren(GetParentProccesId(processToKill));

                    GetProcesses(); RefreshProcessesList();


                }
            }
            catch (Exception) { }
        }

        private void запуститьЗадачуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = Interaction.InputBox("Введите имя программы", "Запуск новой задачи");

            try
            {
                Process.Start(path);
            }
            catch (Exception) { }
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            GetProcesses();

            List<Process> filteredprocesses = processes.Where((x) =>
            x.ProcessName.ToLower().Contains(toolStripTextBox1.Text.ToLower())).ToList<Process>();

            RefreshProcessesList(filteredprocesses, toolStripTextBox1.Text);

        }

        private void завершитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems[0] != null)
                {
                    Process processToKill = processes.Where((x) => x.ProcessName ==
                    listView1.SelectedItems[0].SubItems[0].Text).ToList()[0];

                    KillProcesses(processToKill);

                    GetProcesses(); RefreshProcessesList();
                }
            }
            catch (Exception) { }
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            comparer.ColumnIndex = e.Column;
            comparer.SortDirection = comparer.SortDirection == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;

            listView1.ListViewItemSorter = comparer;
            Form1.listView1.Sort();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
    }




}
