using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TCP_Client
{
    internal class Client
    {
        public static readonly int port = 3000;

        static void Main(string[] args)
        {
            Socket clientSocket = Connect();

            Receive(clientSocket);

            byte[] buffer = new byte[1024];
            while (clientSocket.Connected)
            {
                string input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    int length = Encoding.UTF8.GetBytes(input, buffer);
                    Send(clientSocket, new ArraySegment<byte>(buffer, 0, length));
                }
                if (input == "Quit")
                {
                    break;
                }
            }

            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }

        static async void Receive(Socket clientSocket)
        {
            byte[] buffer = new byte[1024];

            while (clientSocket.Connected)
            {
                try
                {
                    int length = await clientSocket.ReceiveAsync(buffer);
                    if (length == 0) throw new Exception("[Receiver] : message is empty");

                    string message = Encoding.UTF8.GetString(buffer, 0, length);
                    Console.WriteLine(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    break;
                }
            }

            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }

        static void Send(Socket clientSocket, ArraySegment<byte> buffer)
        {
            try
            {
                clientSocket.Send(buffer);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
        }

        static Socket Connect()
        {
            Console.WriteLine("Enter Server IPv4");

            bool connected = false;
            IPAddress address = null;
            IPEndPoint endPoint = null;
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            while (!connected)
            {
                bool validated = false;
                while (!validated)
                {
                    string? ads = string.Empty;
                    while (string.IsNullOrEmpty(ads))
                    {
                        ads = Console.ReadLine();
                    }
                    validated = IPAddress.TryParse(ads, out address);
                }

                endPoint = new IPEndPoint(address, port);

                try
                {
                    clientSocket.Connect(endPoint);
                    connected = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            Console.WriteLine($"Connected On {endPoint.ToString()}");
            return clientSocket;
        }
    }
}
