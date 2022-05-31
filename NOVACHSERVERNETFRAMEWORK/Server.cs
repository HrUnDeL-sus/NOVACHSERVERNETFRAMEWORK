using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NOVACHSERVERNETFRAMEWORK
{
    internal class Server
    {
        private UdpClient _udpClient;
        private const int UDP_PORT = 1243;
        private int _countUDPClient = 0;
        private const int MAX_COUNT_UDP_CLIENT = 15;
       public void Start()
        {
            _udpClient = new UdpClient(UDP_PORT);
            while (true)
            {
                if (_countUDPClient != MAX_COUNT_UDP_CLIENT)
                {
                    _udpClient.BeginReceive(EndReceive, null);
                    _countUDPClient += 1;
                }
                Thread.Sleep(1);


            }
        }
        private void EndReceive(IAsyncResult res)
        {
            IPEndPoint iPEndPoint = default;
           int state=BitConverter.ToInt32(_udpClient.EndReceive(res, ref iPEndPoint),0);
            Client client = World.GetWorld().GetClient(iPEndPoint);
            if ( client == null)
            {
                client = new Client(iPEndPoint);
                World.GetWorld().AddWorldObject(client);
            }
            if(client!=null)
                client.ReadState(state, _udpClient);
            _countUDPClient -= 1;
        }
    }
   
}
