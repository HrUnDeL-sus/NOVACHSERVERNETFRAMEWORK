using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
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
                


            }
        }
        private void EndReceive(IAsyncResult res)
        {
            try
            {
                IPEndPoint iPEndPoint = null;
                byte[] vs = _udpClient.EndReceive(res, ref iPEndPoint);
                int state = BitConverter.ToInt32(vs, 0);
                if (state == (int)ClientAction.Connect)
                {
                    Client get = new Client(iPEndPoint);
                    _udpClient.Send(get.UID, iPEndPoint);
                    World.GetWorld().AddWorldObject(get);
                }
                else
                    World.GetWorld().GetClient(iPEndPoint).GetState(state, _udpClient, iPEndPoint);
            }
            finally
            {
                _countUDPClient -= 1;
            }
           
           
        }
    }
   
}
