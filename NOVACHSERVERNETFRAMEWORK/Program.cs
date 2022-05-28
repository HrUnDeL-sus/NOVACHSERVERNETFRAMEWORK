using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
namespace NOVACHSERVERNETFRAMEWORK
{
    internal class Program
    {
        static void Main(string[] args)
        {
            World.GetWorld();
            World.GetWorld().AddWorldObject(new Platform(Vector3.Zero()));
            World.GetWorld().AddWorldObject(new Sun());
            
            new Thread(new ThreadStart(Timer.StartTimers)).Start();
            
            IPAddress localAddr = IPAddress.Parse("192.168.0.10");
            int port = 1243;
            TcpListener server = new TcpListener(localAddr, port);
            server.Start();
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("CLIENT CONNECT");
                NetworkStream stream = client.GetStream();
                World.GetWorld().AddWorldObject(new Client(stream, client));
            }
        }
    }
}
