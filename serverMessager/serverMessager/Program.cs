using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace serverMessager
{
    class Program
    {
        private IPHostEntry host = Dns.GetHostEntry("localhost");
        private static IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        private IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 8080);
        private Socket[] handler = new Socket[1024];
        public static int Main(string[] args)
        {
            Program start = new Program();
            start.StartServer();
            return 0;
        }

        public void StartServer()
        {


            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);
            listener.Listen(10);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Wating for a connection...");
            Console.ResetColor();

            int i = 0;
            while (true)
            {
                
                //ThreadStart client = new ThreadStart(ConnectClient);
                Thread client = new Thread(ConnectClient);
                handler[client.ManagedThreadId] = listener.Accept();
                //Console.WriteLine(client.ManagedThreadId);
                client.Start();
                i++;
            }
            //try
            //{
            //    Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //    listener.Bind(localEndPoint);
            //    listener.Listen(10);

            //    Console.WriteLine("Wating for a connection...");
            //    handler[0] = listener.Accept();

            //    string data = null;
            //    byte[] bytes = null;

            //    while (true)
            //    {
            //        bytes = new byte[1024];
            //        int bytesRec = handler[0].Receive(bytes);
            //        data = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            //        byte[] msg = Encoding.UTF8.GetBytes(data);
            //        handler[0].Send(msg);
            //    }

            //    Console.WriteLine("Text received : {0}", data);


            //    handler[0].Shutdown(SocketShutdown.Both);
            //    handler[0].Close();
            //}
            //catch(Exception e)
            //{
            //    Console.WriteLine(e.ToString());
            //}

            //Console.WriteLine("\n Press any key to continue....");
            //Console.ReadKey();
        }

        public void ConnectClient()
        {
            int id = Thread.CurrentThread.ManagedThreadId;
            byte[] b = Encoding.UTF8.GetBytes(id.ToString());
            handler[id].Send(b);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("user {0} connected to server", id);
            Console.ResetColor();
            try
            {
                string data = null;
                byte[] bytes = null;

                while (true)
                {
                    //Console.WriteLine("1");
                    bytes = new byte[1024];
                    int bytesRec = handler[id].Receive(bytes);
                    data = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                    if(data == ""){
                        handler[id] = null;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("user {0} disconnected...", id);
                        Console.ResetColor();
                        return;
                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("user {0} send : {1}", id, data);
                    Console.ResetColor();
                    byte[] msg = Encoding.UTF8.GetBytes(id+";"+data);
                    foreach (Socket client in handler)
                    {
                        if (client != null)
                        {
                            client.Send(msg);
                        }
                    }

                }
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("erorr");
                Console.ResetColor();
                handler[id].Shutdown(SocketShutdown.Both);
                handler[id].Close();
            }
        }

    }
}
