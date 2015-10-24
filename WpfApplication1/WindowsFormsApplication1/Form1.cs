using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private int i = 0;
        public Form1()
        {
            InitializeComponent();
        }
        private void foo()
        {
            while (i < 10)
            {
                Console.WriteLine($"i={i}");
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            await Task.Run(() => foo());
            textBox1.Clear();
            textBox1.Text = "Task finished";
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            i = trackBar1.Value;
        }
    }
}
