using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
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
            try
            {
                server_thread = new Thread(Messagram.connect);
                server_thread.Start();
                Thread.Sleep(3000); // Wait 5 seconds to connect to Messagram Server
                if (Messagram.connected)
                {
                    MessageBox.Show("Connected to Messagram Chat!", "Success");
                }
                else
                {
                    MessageBox.Show("Unable to connect to Messagram Chat, Try again later!", "Error");
                    Environment.Exit(0);
                }
            }
            catch
            {
                MessageBox.Show("Error", "Unable to connect to Messagram Chat, Try again later!");
                Environment.Exit(0);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == null || textBox2.Text == null) { 
                MessageBox.Show("No information provided!");
                return;
            }
            Messagram.send_command(textBox1.Text + "\n");
            
            Messagram.send_command(textBox2.Text + "\n");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Messagram.client.Close();
        }
    }






    class Buffer
    {
        public static string data;

        public static bool check_new_msg()
        {
            if(Buffer.data != "" || Buffer.data.Length == 0)
            {
                return true;
            }
            return false;
        }
        
        public static string get_new_msg()
        {
            if (Buffer.data != "" || Buffer.data.Length == 0)
            {
                string f = Buffer.data;
                Buffer.data = "";
                return f;
            }
            return "";
        }
    }

    class Messagram
    {
        // Messagram Info & Status
        public static string messagram_ip = "skrillec.ovh";
        public static Int32 messagram_port = 30;
        public static bool connected = false;
        public static string[] cmds;
        public static int cmds_count;

        // System.Net.Sockets Objects
        public static TcpClient client;
        public static NetworkStream stream;

        // Threads
        public static Thread listener_thread;
        public static Thread updown;

        public static WebClient w = new WebClient();

        // Connect To Messagram Server
        public static void connect()
        {
            Messagram.client = new TcpClient(messagram_ip, messagram_port);
            Messagram.connected = true;

            // start messagram server listener
            listener_thread = new Thread(Messagram.listener);
            listener_thread.Start();
        }

        public static void listener()
        {
            try
            {
                Byte[] data = new byte[256];
                string Response = String.Empty;
                NetworkStream strm = Messagram.client.GetStream();
                Messagram.stream = strm;
                while (true)
                {

                    Int32 b = strm.Read(data, 0, data.Length);
                    Response = System.Text.Encoding.ASCII.GetString(data, 0, b);

                    dynamic d = JsonSerializer.Deserialize<object>(Response);



                    switch(Response)
                    {
                        case "msg":
                            // Handle new message JSON response here
                            return;
                    }
                }
            } catch
            {
                Messagram.connected = false;
            }

        }

        public static void send_command(string msg)
        {
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(msg);
            Messagram.stream.Write(data, 0, data.Length);
        }

        public static void shut_down()
        {
            Messagram.client.Close();
            Messagram.connected = false;
            Messagram.listener_thread.Abort();
        }
    }
}
