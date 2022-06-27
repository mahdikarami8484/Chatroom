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
using System.Net;
using System.Threading;

namespace messager
{
    public partial class Form1 : Form
    {

        //System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
        private static IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        private IPEndPoint remoteEP = new IPEndPoint(ipAddress, 8080);
        private int userId;
        private static Socket server = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StartClient();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sendMessage(server, textBox2.Text);
            textBox2.Text = "";
        }

        private void setText(string msg)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(setText), new object[] { msg });
                return;
            }
            textBox1.AppendText("\r\n"+checkId(msg));
        }

        private string checkId(string msg)
        {
            string[] msgs = msg.Split(';');
            if (Convert.ToInt32(msgs[0]) == userId)
            {
                return "شما : "+msgs[1];
            }
            return "کاربر " + Convert.ToInt32(msgs[0]) + " : " + msgs[1];
        }

        private void StartClient()
        {
           
            try
            {
                //IPHostEntry host = Dns.GetHostEntry("localhsot");


                try
                {
                    server.Connect(remoteEP);

                    textBox1.Text = "به سرور" + server.RemoteEndPoint.ToString()+ " متصل شدید.";
                    try
                    {
                        byte[] bytes = new byte[1024];

                        int bytesRec = server.Receive(bytes);
                        userId = Convert.ToInt32(Encoding.UTF8.GetString(bytes, 0, bytesRec));
                        this.Text = "کاربر : "+userId;
                    }
                    catch
                    {
                        MessageBox.Show("Error");
                    }

                    Thread getMessage = new Thread(GetMessage);
                    getMessage.Start();
                }
                catch(ArgumentNullException ane)
                {
                    MessageBox.Show("ArgumentNullException : "+ane.ToString());
                }
                catch(SocketException se)
                {
                    MessageBox.Show("SocketException : " + se.ToString());
                }
                catch (Exception e)
                {
                    MessageBox.Show("Exception : " + e.ToString());
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void sendMessage(Socket sender, string msg)
        {
            byte[] bytes = new byte[1024];
            byte[] message = Encoding.UTF8.GetBytes(msg);

            int bytesSent = sender.Send(message);

            //int bytesRec = sender.Receive(bytes);

            //textBox1.Text += "\r\n " + Encoding.UTF8.GetString(bytes, 0, bytesRec);
        }

        private void textBox2_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(this, new EventArgs());
            }
        }

        private void GetMessage()
        {
            while (true)
            {
                byte[] bytes = new byte[1024];

                int bytesRec = server.Receive(bytes);

                setText("\r\n" + Encoding.UTF8.GetString(bytes, 0, bytesRec));
            }
        }
    }
}
