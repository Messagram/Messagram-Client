using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;

namespace Messagram
{
    public partial class Form1 : Form
    {
        public Thread server_thread;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                server_thread = new Thread(Messagram.connect);
                server_thread.Start();
                Thread.Sleep(5000); // Wait 5 seconds to connect to Messagram Server
                if (Messagram.connected)
                {
                    MessageBox.Show("Connected to Messagram Chat!", "Success");
                }
                else
                {
                    MessageBox.Show("Unable to connect to Messagram Chat, Try again later!", "Error");
                }
            } catch
            {
                MessageBox.Show("Error", "Unable to connect to Messagram Chat, Try again later!");
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(()=>Messagram.send_command("testing this\n"));
            t.Start();
            Thread.Sleep(1000);
            t.Abort();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }
    }

    class Messagram
    {
        // Messagram Info & Status
        public static string messagram_ip = "skrillec.ovh";
        public static Int32 messagram_port = 30;
        public static bool connected = false;

        // System.Net.Sockets Objects
        public static TcpClient client;
        public static NetworkStream stream;

        // Threads
        public static Thread listener_thread;

        // Connect To Messagram Server
        public static void connect()
        {
            Messagram.client = new TcpClient(messagram_ip, messagram_port);
            Messagram.connected = true;
            listener_thread = new Thread(Messagram.listener);
            listener_thread.Start();
        }

        public static void listener()
        {
            Byte[] data = new byte[256];
            string Response = String.Empty;
            NetworkStream strm = Messagram.client.GetStream();
            Messagram.stream = strm;
            while (true) {

                Int32 b = strm.Read(data, 0, data.Length);
                Response = System.Text.Encoding.ASCII.GetString(data, 0, b);
            }

        }

        public static void send_command(string msg)
        {
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(msg);
            Messagram.stream.Write(data, 0, data.Length);
        }

    }
}
