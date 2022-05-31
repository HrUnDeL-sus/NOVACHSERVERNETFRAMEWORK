using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        SetBlockSand,
        RotateLeft,
        RotateRight,
        GetCameraPosition,
        Jump,
        SetBlockWood
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
        public bool CanWrite = true;
        public event ClientErrorHasOccurred OnClientError;
        private Stack<StackWorldObject> _stackWorldObjects=new Stack<StackWorldObject>();
        public readonly IPEndPoint MyIpEndPoint;
        private Timer _timerActive;
        public bool CanBeginRead { get; private set; }
        public Client(IPEndPoint iPEndPoint)
        {
            MyIpEndPoint = iPEndPoint;
            OnClientError += HasClientError;
            _timerActive = new Timer(20);
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
                if (stackWorldObject == null)
                    continue;
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
        private void HasClientError(Exception exception,Client client)
        {
            Console.WriteLine("{0}:{1}\n{2}", Name, exception.Message,exception.StackTrace);
        }
        public override void OnUpdated()
        {
            if (_timerActive.TimeIsUp())
            {
                World.GetWorld().RemoveWorldObject(this);
                return;
            }
            Update();
        }
       
        public void ReadState(int state,UdpClient udpClient)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                ClientAction action = (ClientAction)state;
                _timerActive.Restart();
                switch (action)
                {
                    case ClientAction.RenderingWorld:
                           List<float[]> floatArrays = GetDataStackObjectForRendering();
                        udpClient.SendAsync(new float[] {Position.X,Position.Y,Position.Z}, MyIpEndPoint);
                        foreach (var item in floatArrays)
                        {
                            int count = 0;
                                count = udpClient.Send(item, MyIpEndPoint);
                                Console.WriteLine("COUNT:{0}", count);
                        }
                           
                        udpClient.SendAsync(new float[15], MyIpEndPoint);
                        //     Update();
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
                    case ClientAction.SetBlockSand:
                        SetBlock(TypeBlock.Sand);
                        break;
                    case ClientAction.SetBlockWood:
                        SetBlock(TypeBlock.Wood);
                        break;
                    case ClientAction.GetCameraPosition:
                        
                       break;
                    case ClientAction.Jump:
                        IsJump(true);

                        break;
                }
               

                stopwatch.Stop();
               
                TimeSpan ts = stopwatch.Elapsed;
               
                    Console.WriteLine("{0} time:{1}\n Action:{2}\n", Name, ts.Ticks, action);
                CanWrite = true;
            }
            catch (Exception e)
            {
                OnClientError?.Invoke(e, this);
            }
        }

    }
}
