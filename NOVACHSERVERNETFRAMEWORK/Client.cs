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
        Action,
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
        public bool CanWrite { get; private set; }
        public event ClientErrorHasOccurred OnClientError;
        private Stack<StackWorldObject> _stackWorldObjects=new Stack<StackWorldObject>();
        public readonly IPEndPoint MyIpEndPoint;
        private Timer _timerActive;
        private object _obj=new object();
        private int _maxDataReceive;
        private long _sped = 0;
        private int _count = 0;
        public bool CanBeginRead { get; private set; }
        public Client(IPEndPoint iPEndPoint,int maxDataReceive)
        {
            _maxDataReceive = maxDataReceive;
            MyIpEndPoint = iPEndPoint;
            OnClientError += HasClientError;
            _timerActive = new Timer(500);
            CanWrite = true;
        }
        public void UpdateStackWorldObjects(StackWorldObject stackWorldObject)
        {
                if (_stackWorldObjects.Count >= 0)
                {
                    _stackWorldObjects.Push(stackWorldObject);
                Console.WriteLine("Item:{0} Type:{1} Count:{2}", stackWorldObject.WorldObject.Name, stackWorldObject.WorldObjectInStack, _stackWorldObjects.Count);

                }
            
        }
        private float[] GetDataStackObjectForRendering(int maxCount)
        {
          
          
            Stack<StackWorldObject> _localStack = _stackWorldObjects;
            List<float> vs = new List<float>();
            int count = 0;
            while (_localStack.Count != 0&&count!=maxCount)
            {
                
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

                if (stackWorldObject.WorldObject is Player)
                    vs.Add((stackWorldObject.WorldObject as Player).GetActiveAction());
                else
                    vs.Add(-1);
                count += 16;
            }
            if(count!=maxCount)
                for (int i = 0; i < maxCount-count; i++)
                {
                    vs.Add(0);
                }
            return vs.ToArray();


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
        protected override void OnKilled()
        {
            Position = Vector3.Zero();
            World.GetWorld().MoveWorldObject(this);
        }

        public void ReadState(int state,UdpClient udpClient)
        {
            try
            {
                CanWrite = false;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                _timerActive.Restart();
                switch (state)
                {
                    case (int)ClientAction.RenderingWorld:
                          float[] floatArray = GetDataStackObjectForRendering(_maxDataReceive);
                        udpClient.Send(new float[] {Position.X,Position.Y,Position.Z}, MyIpEndPoint);
                        udpClient.Send(floatArray, MyIpEndPoint);
                        //     Update();
                        break;
                    case (int)ClientAction.MoveForward:
                        Move(new Vector3(0, 0, 1), new Vector3(0, 180, 0));
                        udpClient.Send(1,MyIpEndPoint);
                        break;
                    case (int)ClientAction.MoveBack:
                        Move(new Vector3(0, 0, -1), Vector3.Zero());
                        udpClient.Send(1, MyIpEndPoint);
                        break;
                    case (int)ClientAction.MoveLeft:
                        Move(new Vector3(-1, 0, 0),new Vector3(0,90,0));
                        udpClient.Send(1, MyIpEndPoint);
                        break;
                    case (int)ClientAction.MoveRight:
                        Move(new Vector3(1, 0, 0), new Vector3(0, -90, 0));
                        udpClient.Send(1, MyIpEndPoint);
                        break;
                    case (int)ClientAction.RotateLeft:
                        Rotation(new Vector3(0, 90, 0));
                        udpClient.Send(1, MyIpEndPoint);
                        break;
                    case (int)ClientAction.RotateRight:
                        Rotation(new Vector3(0, -90, 0));
                        udpClient.Send(1, MyIpEndPoint);
                        break;
                    case (int)ClientAction.Action:
                        PerformAnAction();
                        udpClient.Send(1, MyIpEndPoint);
                        break;
                    case (int)ClientAction.GetCameraPosition:
                        
                       break;
                    case (int)ClientAction.Jump:
                        IsJump(true);
                        udpClient.Send(1, MyIpEndPoint);
                        break;
                    default:
                        if (state >= 40 && state < 42)
                        {
                                SelectedBlock = (TypeBlock)(state - 40);
                            MyActiveAction = ActiveAction.SetBlock;
                            World.GetWorld().MoveWorldObject(this);
                        }
                        else if (state == 42)
                        {
                            MyActiveAction = ActiveAction.Shot;
                            World.GetWorld().MoveWorldObject(this);
                        }
                        udpClient.Send(1, MyIpEndPoint);
                        break;

                }
               

                stopwatch.Stop();
               
                TimeSpan ts = stopwatch.Elapsed;
                _count += 1;
                _sped += ts.Ticks;
            
                _timerActive.Restart();
                CanWrite = true;
              
            }
            catch (Exception e)
            {
                OnClientError?.Invoke(e, this);
            }
        }

    }
}
