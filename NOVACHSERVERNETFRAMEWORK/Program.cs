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
            World.GetWorld().AddWorldObject(new Platform());
            World.GetWorld().AddWorldObject(new Sun());
            new Thread(new ThreadStart(new Server().Start)).Start();
            new Thread(new ThreadStart(Timer.StartTimers)).Start();
        }
    }
}
