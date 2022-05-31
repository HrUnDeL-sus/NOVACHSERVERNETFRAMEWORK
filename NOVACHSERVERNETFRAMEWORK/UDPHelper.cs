using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
namespace NOVACHSERVERNETFRAMEWORK
{
    internal static class UDPHelper
    {
        public static int Send<T>(this UdpClient udpClient, T value,IPEndPoint iPEndPoint)
        {
            byte[] buffer = GetBytes(value);

            return udpClient.Send(buffer, buffer.Length, iPEndPoint);
        }
        public static Task<int> SendAsync<T>(this UdpClient udpClient, T value, IPEndPoint iPEndPoint)
        {
            byte[] buffer = GetBytes(value);

            return  udpClient.SendAsync(buffer, buffer.Length, iPEndPoint);
        }
        private static byte[] GetBytes<T>(T value) {
            byte[] _sentBytes = new byte[0];
            if (value is int iI)
                _sentBytes = BitConverter.GetBytes(Convert.ToInt32(iI));
            else if (value is float iF)
                _sentBytes = BitConverter.GetBytes(Convert.ToSingle(iF));
            else if (value is double iD)
                _sentBytes = BitConverter.GetBytes(Convert.ToDouble(iD));
            else if (value is float[] iFArray)
            {
                _sentBytes = new byte[iFArray.Length * 4];
                Buffer.BlockCopy(iFArray, 0, _sentBytes, 0, _sentBytes.Length);
            }
            else if (value is int[] iIArray)
            {
                _sentBytes = new byte[iIArray.Length * 4];
                Buffer.BlockCopy(iIArray, 0, _sentBytes, 0, _sentBytes.Length);
            }
            else
            {
                throw new Exception("Error convert " + value.GetType() + " to byte array");
            }
            return _sentBytes;
        }
        public static T Receive<T>(this UdpClient udpClient,ref IPEndPoint iPEndPoint)
        {
            T t=default(T);
            iPEndPoint = new IPEndPoint(IPAddress.None, 0);
            byte[] _reciveBytes = udpClient.Receive(ref iPEndPoint);
            if (t is int)
                return (T)(object)BitConverter.ToInt32(_reciveBytes,0);
            else if (t is float)
                return (T)(object)BitConverter.ToSingle(_reciveBytes, 0);
            else if (t is double)
                return (T)(object)BitConverter.ToDouble(_reciveBytes, 0);
            return default(T);

        }
    }
}
