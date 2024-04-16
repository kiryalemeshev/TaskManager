using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;
using System.Runtime.InteropServices;

namespace Kurs
{
    public partial class Form2 : MetroForm
    {

        private float cpu;

        private float ram;

        private ulong installedMemory;

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            MEMORYSTATUSEX mEMORYSTATUSEX = new MEMORYSTATUSEX();

            if(GlobalMemoryStatusEx(mEMORYSTATUSEX))
            {
                installedMemory = mEMORYSTATUSEX.ullTotalPhys;
            }

            metroLabel10.Text = Convert.ToString(installedMemory  / 1000000000) + "Гб";

            timer1.Interval = 1000;
            timer1.Start();
        }



        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]

        private class MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLength;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;



            public MEMORYSTATUSEX()
            {
                this.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            }

        }


        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]

        static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);

        private void timer1_Tick(object sender, EventArgs e)
        {
            cpu = performanceCPU.NextValue(); //Считывает данные с perfomancecouner
            ram = performanceRAM.NextValue(); //Считывает данные с perfomancecouner

            metroProgressBar1.Value = (int)cpu; //Присвоим значения в metroProgressBar1
            metroProgressBar2.Value = (int)ram; //Присвоим значения в metroProgressBar2

            metroLabel2.Text = Convert.ToString(Math.Round(cpu, 1)) + "%"; //Конвертируем в текст и переводим с точностью до 1 знака после запятой
            metroLabel3.Text = Convert.ToString(Math.Round(ram, 1)) + "%";

            metroLabel6.Text = Convert.ToString(Math.Round((ram / 100 * installedMemory) / 1000000000, 1)) + " Гб";
           // metroLabel8.Text = Convert.ToString(Math.Round((installedMemory - ram / 100 * installedMemory) / 1000000000, 1)) +" Гб";


            chart1.Series["ЦП"].Points.AddY(cpu); //Обращаемся к линиям графика как к ключу словаря через ЦП
            chart1.Series["ОЗУ"].Points.AddY(ram); //Обращаемся к линиям графика как к ключу словаря через ОЗУ

        }

        private void metroLabel8_Click(object sender, EventArgs e)
        {

        }
    }

}
