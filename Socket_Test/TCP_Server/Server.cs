using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TCP_Server
{
    internal class Server
    {
        public static readonly int port = 3000;
        public static List<Socket> clients = new List<Socket>();

        static void Main(string[] args)
        {
            IPAddress address = IPAddress.Any;
            IPEndPoint endPoint = new IPEndPoint(address, port);

            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(endPoint);
            listener.Listen(1);

            Listen(listener);

            DebugServerStart();

            byte[] buffer = new byte[1024];
            while (true)
            {
                string input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    int length = Encoding.UTF8.GetBytes(input, buffer);
                    Send(new ArraySegment<byte>(buffer, 0, length));
                }
                if (input == "Close")
                {
                    break;
                }
            }

            foreach (Socket socket in clients)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            clients.Clear();

            listener.Shutdown(SocketShutdown.Both);
            listener.Close();
        }

        static async void Listen(Socket listener)
        {
            while (true)
            {
                try
                {
                    Socket clientSocket = await listener.AcceptAsync();
                    clients.Add(clientSocket);
                    Console.WriteLine($"Client Connected : {clientSocket.RemoteEndPoint.ToString()}");
                    Receive(clientSocket);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
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
                    message = $"{clientSocket.RemoteEndPoint.ToString()} : {message}";
                    Console.WriteLine(message);

                    length = Encoding.UTF8.GetBytes(message, buffer);

                    Send(new ArraySegment<byte>(buffer, 0, length), clientSocket);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    break;
                }
            }

            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
            clients.Remove(clientSocket);
        }

        static void Send(ArraySegment<byte> buffer, Socket except = null)
        {
            foreach (Socket client in clients)
            {
                if (except != null && client == except) continue;

                try
                {
                    client.Send(buffer);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                    clients.Remove(client);
                }
            }
        }

        static void DebugServerStart()
        {
            IPAddress serverAddress = null;
            IPAddress[] Addresses = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress ip in Addresses)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    serverAddress = ip;
                }
            }
            IPEndPoint serverEndPoint = new IPEndPoint(serverAddress, port);
            Console.WriteLine($"Server Started On : {serverEndPoint.ToString()}");
        }
    }
}
