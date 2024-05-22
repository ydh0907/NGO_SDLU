using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    internal class Program
    {
        static readonly int port = 3000;

        static void Main(string[] args)
        {
            // 받을 사람들의 정보, 열어줄 포트
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
            // TCP 소켓 만들기
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // 위치에 놓고
            socket.Bind(endPoint);
            // 최대 3명까지 대기시킨다
            socket.Listen(3);

            // IP 출력
            PrintIPAddress();

            // Client를 받는다
            Socket clientSocket = socket.Accept();

            // 정보를 받는 부분
            byte[] buffer = new byte[1024];
            int length = clientSocket.Receive(buffer);

            // 스트링으로 정보를 변환
            string message = Encoding.UTF8.GetString(buffer, 0, length);

            // 상대의 정보와 메시지 출력
            Console.WriteLine($"{clientSocket.RemoteEndPoint} : {message}");

            message = $"Server: {message}";

            // 바이트로 변환
            length = Encoding.UTF8.GetBytes(message, 0, message.Length, buffer, 0);

            // 전송
            length = clientSocket.Send(buffer, 0, length, SocketFlags.None);

            Console.ReadKey();
        }

        private static void PrintIPAddress()
        {
            string hostName = Dns.GetHostName();
            IPAddress[] addresses = Dns.GetHostAddresses(hostName);

            foreach (IPAddress address in addresses)
            {
                if(address.AddressFamily == AddressFamily.InterNetwork)
                {
                    Console.WriteLine($"Server Listen : {address}:{port}");
                }
            }
        }
    }
}
