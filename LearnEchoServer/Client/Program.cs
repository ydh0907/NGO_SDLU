using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    internal class Program
    {
        static readonly int port = 3000;
        
        static void Main(string[] args)
        {
            // 서버의 위치
            string ip = Console.ReadLine();

            IPAddress address = IPAddress.Parse(ip);
            IPEndPoint endPoint = new IPEndPoint(address, port);

            // 소켓 만들고 연결
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(endPoint);

            // 쓴 값을 보내기
            byte[] buffer = new byte[1024];
            string input = Console.ReadLine();
            int length = Encoding.UTF8.GetBytes(input, 0, input.Length, buffer, 0);
            clientSocket.Send(buffer, 0, length, SocketFlags.None);

            // 받아서 출력
            length = clientSocket.Receive(buffer);
            string message = Encoding.UTF8.GetString(buffer, 0, length);
            Console.WriteLine(message);

            Console.ReadKey();
        }
    }
}
