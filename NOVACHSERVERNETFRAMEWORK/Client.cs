using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NOVACHSERVERNETFRAMEWORK
{
    enum ClientAction { 
        RenderingWorld,
        MoveLeft,
        MoveRight,
        MoveUp,
        MoveDown,
        MoveForward,
        MoveBack,
        SetBlock,
        RotateLeft,
        RotateRight,
        GetCameraPosition,
        Jump
    }
    enum StateWorldObjectInStack
    {
        Created,
        Deleted,
        Updated
    }
    delegate void ClientErrorHasOccurred(Exception exception,Client client);
    delegate void StackWorldObjectsWasUpdated(StackWorldObject stackWorldObject);
    internal class Client:Player
    {
        public event ClientErrorHasOccurred OnClientError;
        private Stack<StackWorldObject> _stackWorldObjects=new Stack<StackWorldObject>();
        private NetworkStream _myNetworkStream;
        private int _countThreads = 0;
        private const int MAX_COUNT_THREADS = 10;
        private byte[] _receivedBytes;
        private byte[] _sentBytes;
        public bool CanBeginRead { get; private set; }
        public Client(NetworkStream stream,TcpClient tcpClient)
        {
            _myNetworkStream = stream;
            CanBeginRead = true;
            OnClientError += HasClientError;
            ClearBytes();
            BeginRead();

        }
        public void UpdateStackWorldObjects(StackWorldObject stackWorldObject)
        {
            
            _stackWorldObjects.Push(stackWorldObject);
        }
        private List<float[]> GetDataStackObjectForRendering()
        {
          
            List<float[]> vs2 = new List<float[]>();
            Stack<StackWorldObject> _localStack = _stackWorldObjects;
           
            while (_localStack.Count != 0)
            {
                List<float> vs = new List<float>();
                StackWorldObject stackWorldObject = _localStack.Pop();
              
                 WorldObject obj = stackWorldObject.WorldObject;
                vs.Add((int)stackWorldObject.WorldObjectInStack);
                vs.Add(obj.Position.X);
                vs.Add(obj.Position.Y);
                vs.Add(obj.Position.Z);
                vs.Add(obj.Scale.X);
                vs.Add(obj.Scale.Y);
                vs.Add(obj.Scale.Z);
                vs.Add(obj.Rotate.X);
                vs.Add(obj.Rotate.Y);
                vs.Add(obj.Rotate.Z);
                vs.Add(obj.Color.X);
                vs.Add(obj.Color.Y);
                vs.Add(obj.Color.Z);
                vs.Add((int)obj.MyTypeWorldObject);
                vs.Add(obj.UID);
                vs2.Add(vs.ToArray());
                
            }
            return vs2;
        }
        private void ClearBytes()
        {
            _receivedBytes = new byte[4096];
            _sentBytes = new byte[4096];
        }
        protected void BeginRead()
        {
            try
            {
                _myNetworkStream.BeginRead(_receivedBytes, 0, _receivedBytes.Length, OnReadFromNetworkStream, null);
            }
            catch (Exception e)
            {
                OnClientError?.Invoke(e, this);
            }

        }
        private void HasClientError(Exception exception,Client client)
        {
            Console.WriteLine("{0}:{1}\n{2}", Name, exception.Message,exception.StackTrace);
        }
        
       
        private void SendByte()
        {
            _sentBytes = new byte[1];
            _myNetworkStream.Write(_sentBytes, 0, _sentBytes.Length);
        }
        private void SendInt(int i,bool waitAnswer)
        {
            _sentBytes = BitConverter.GetBytes(i);
            _myNetworkStream.WriteAsync(_sentBytes, 0, _sentBytes.Length);
            if (waitAnswer)
                _myNetworkStream.ReadByte();
        }
        public override void OnUpdated()
        {
            Update();
        }
        private void SendFloatArray(float[] array)
        {
            _sentBytes = new byte[array.Length * 4];
            Buffer.BlockCopy(array, 0, _sentBytes, 0, _sentBytes.Length);
            _myNetworkStream.WriteAsync(_sentBytes, 0, _sentBytes.Length);
        }
        private void OnReadFromNetworkStream(IAsyncResult asyncResult)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                int count=_myNetworkStream.EndRead(asyncResult);
               
                _receivedBytes = _receivedBytes.ToList().Take(count).ToArray();
                int t = BitConverter.ToInt32(_receivedBytes, 0);
                ClientAction action = (ClientAction)t;
              
                switch (action)
                {
                    case ClientAction.RenderingWorld:
                           List<float[]> floatArrays = GetDataStackObjectForRendering();
                        foreach (var item in floatArrays)
                            SendFloatArray(item);
                        SendFloatArray(new float[15]);
                        //     Update();
                        break;
                    case ClientAction.MoveForward:
                        Move(new Vector3(0, 0, 1), new Vector3(0, 180, 0));
                        SendByte();
                        break;
                    case ClientAction.MoveBack:
                        Move(new Vector3(0, 0, -1), Vector3.Zero());
                            SendByte();
                        break;
                    case ClientAction.MoveLeft:
                        Move(new Vector3(-1, 0, 0),new Vector3(0,90,0));
                        SendByte();
                        break;
                    case ClientAction.MoveRight:
                        Move(new Vector3(1, 0, 0), new Vector3(0, -90, 0));
                        SendByte();
                        break;
                    case ClientAction.RotateLeft:
                        Rotation(new Vector3(0, 90, 0));
                        SendByte();
                        break;
                    case ClientAction.RotateRight:
                        Rotation(new Vector3(0, -90, 0));
                        SendByte();
                        break;
                    case ClientAction.SetBlock:
                        SetBlock();
                        SendByte();
                        break;
                    case ClientAction.GetCameraPosition:
                        SendFloatArray(new float[] { Position.X, Position.Y, Position.Z });
                        SendByte();
                        break;
                    case ClientAction.Jump:
                        Jump();
                        SendByte();
                        break;
                }
               
                ClearBytes();
                stopwatch.Stop();
                BeginRead();
                TimeSpan ts = stopwatch.Elapsed;
                    Console.WriteLine("{0} time:{1}\n Action:{2}\n", Name, ts.Milliseconds, action);
                
               
            }
            catch (Exception e)
            {
                OnClientError?.Invoke(e, this);
            }
        }

    }
}
