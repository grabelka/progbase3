using System;
using ClassLibrary;
using Microsoft.Data.Sqlite;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace ConsoleServer
{
    class Program
    {
        static string dbPath = @"C:\Users\nasty.DESKTOP-UTJ8J96\OneDrive\Desktop\progbase3\data\data.db";
        static SqliteConnection connection = new SqliteConnection($"Data Source={dbPath}");
        static UserRepository userRepository = new UserRepository(connection);
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
                Console.WriteLine($"Get: '{data}'");
                XmlSerializer ser = new XmlSerializer(typeof(ExportUser));
                StringReader reader = new StringReader(data);
                ExportUser user = (ExportUser)ser.Deserialize(reader);
                if(user.command == "registration")
                {
                    bool returnData;
                    if (userRepository.FindLogin(user.login) == null) 
                    {
                        returnData = true;
                        Autentification.Register(userRepository, user.name, user.login, user.isModerator, user.password);
                    }
                    else 
                        returnData = false;
                    reader.Close();
                    byte[] msg = Encoding.ASCII.GetBytes(returnData.ToString());
                    int bytesSent = handler.Send(msg);
                }
                if(user.command == "verification")
                {
                    bool returnBool = true;
                    User current = Autentification.Verify(userRepository, user.login, user.password);
                    if (current == null) 
                    {
                        returnBool = false;
                    }
                    reader.Close();
                    string returnData = returnBool.ToString() + "#" + current.name + "#" + current.isModerator;
                    byte[] msg = Encoding.ASCII.GetBytes(returnData);
                    int bytesSent = handler.Send(msg);
                }
            }
        }
    }
}
