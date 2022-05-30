using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NOVACHSERVERNETFRAMEWORK
{
    enum ClientAction { 
        Connect,
        RenderingObject,
        EndRenderingObject,
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
        private List<int> _createdWolrdObjectsUID = new List<int>();
        public readonly IPEndPoint MyIpEndPoint;
        public bool CanBeginRead { get; private set; }
        public Client(IPEndPoint iPEndPoint)
        {
            MyIpEndPoint = iPEndPoint;
            OnClientError += HasClientError;
        }
        public void UpdateStackWorldObjects(StackWorldObject stackWorldObject)
        {
            if(_stackWorldObjects.Count>=0)
            _stackWorldObjects.Push(stackWorldObject);
        }
        private float[] ConvertStackObjectToFloatArray(StackWorldObject stackWorldObject)
        {
            if (stackWorldObject == null)
                return new float[15];
            WorldObject obj = stackWorldObject.WorldObject;
            List<float> vs = new List<float>();
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
            return vs.ToArray();
        }
        private void HasClientError(Exception exception,Client client)
        {
            Console.WriteLine("{0}:{1}\n{2}", Name, exception.Message,exception.StackTrace);
        }
        
       
        
        public override void OnUpdated()
        {
            Update();
        }
        public void GetState(int state,UdpClient udpClient,IPEndPoint iPEndPoint)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                ClientAction action = (ClientAction)state;
              
                switch (action)
                {
                    case ClientAction.RenderingObject:
                        if (_stackWorldObjects.Count != 0)
                            udpClient.Send(ConvertStackObjectToFloatArray(_stackWorldObjects.Pop()), MyIpEndPoint);
                        else
                            udpClient.Send(new float[15], MyIpEndPoint);
                        //     Update();
                        break;
                    case ClientAction.EndRenderingObject:
                        if (_stackWorldObjects.Count == 0)
                        {
                            udpClient.Send(1, MyIpEndPoint);
                        }
                        else
                        {
                            udpClient.Send(0, MyIpEndPoint);
                        }
                            
                        break;
                    case ClientAction.MoveForward:
                        Move(new Vector3(0, 0, 1), new Vector3(0, 180, 0));
                        break;
                    case ClientAction.MoveBack:
                        Move(new Vector3(0, 0, -1), Vector3.Zero());
                        break;
                    case ClientAction.MoveLeft:
                        Move(new Vector3(-1, 0, 0),new Vector3(0,90,0));
                        break;
                    case ClientAction.MoveRight:
                        Move(new Vector3(1, 0, 0), new Vector3(0, -90, 0));
                        break;
                    case ClientAction.RotateLeft:
                        Rotation(new Vector3(0, 90, 0));
                        break;
                    case ClientAction.RotateRight:
                        Rotation(new Vector3(0, -90, 0));
                        break;
                    case ClientAction.SetBlock:
                        SetBlock();
                        break;
                    case ClientAction.GetCameraPosition:
                        udpClient.Send(new float[] { Position.X, Position.Y, Position.Z },MyIpEndPoint);
                        break;
                    case ClientAction.Jump:
                        Jump();
                        break;
                }
                TimeSpan ts = stopwatch.Elapsed;
                if(ts.Ticks>1000)
                    Console.WriteLine("{0} time:{1}\n Action:{2}\n", Name, ts.Ticks, action);
                
               
            }
            catch (Exception e)
            {
                OnClientError?.Invoke(e, this);
            }
        }

    }
}
