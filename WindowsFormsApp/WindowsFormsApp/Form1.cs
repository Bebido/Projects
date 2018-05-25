using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp
{
    public partial class Form1 : Form
    {
        TcpClient tcpClient;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        async private void button1_Click(object sender, EventArgs e)
        {
            tcpClient = new TcpClient("localhost", int.Parse(textBox1.Text));
            while (true)
            {
                byte[] bufor = new byte[1];
                int x = await tcpClient.GetStream().ReadAsync(bufor, 0, 1);
                System.Console.Write((char)bufor[0]);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string s = "gęśla jaźń";
            byte[] b = System.Text.Encoding.UTF8.GetBytes(s);
            tcpClient.GetStream().Write(b, 0, b.Count());
        }
    }
}
