using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace ConsoleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress ipAddress = IPAddress.Loopback; 
            int port = 3000;

            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);
                Thread thead2 = new Thread(Exit);
                thead2.Start();
                while (true)
                {
                    Socket handler = listener.Accept();
                    Thread thead = new Thread(ClientRun);
                    thead.Start(handler);
                }           
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private static void Exit()
        {
            Console.WriteLine("\nEnter End to stop server...");
            if (Console.ReadLine() == "End")
            {
                Environment.Exit(0);
            }
        }
        private static void ClientRun(Object input)
        {
            Socket handler = (Socket)input;
            Console.WriteLine($"User {handler.RemoteEndPoint} start work.");
            while (true)
            {
                byte[] bytes = new byte[1024];  
                Console.WriteLine("Wait for command");
                int bytesRec = handler.Receive(bytes);
                
                string data = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                if (data == "")
                {   
                    handler.Close();
                    return;
                }
                Console.WriteLine($"Get: '{data}'");

                byte[] msg = Encoding.ASCII.GetBytes(data);
                int bytesSent = handler.Send(msg);
            }
        }
    }
}
