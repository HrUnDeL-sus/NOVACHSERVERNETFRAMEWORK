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
        public bool CanBeginRead { get; private set; }
        public Client(IPEndPoint iPEndPoint)
        {
            MyIpEndPoint = iPEndPoint;
            OnClientError += HasClientError;
            _timerActive = new Timer(250);
            CanWrite = true;
        }
        public void UpdateStackWorldObjects(StackWorldObject stackWorldObject)
        {
           if(_stackWorldObjects.Count>=0)
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

                if (stackWorldObject.WorldObject is Player)
                    vs.Add((stackWorldObject.WorldObject as Player).GetActiveAction());
                else
                    vs.Add(-1);
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
                           List<float[]> floatArrays = GetDataStackObjectForRendering();
                        udpClient.SendAsync(new float[] {Position.X,Position.Y,Position.Z}, MyIpEndPoint);
                        for(int i = 0; i < floatArrays.Count; i += 1)
                        {
                            udpClient.SendAsync(floatArrays[i], MyIpEndPoint);
                        }
                        udpClient.SendAsync(new float[16], MyIpEndPoint);
                        //     Update();
                        break;
                    case (int)ClientAction.MoveForward:
                        Move(new Vector3(0, 0, 1), new Vector3(0, 180, 0));
                        break;
                    case (int)ClientAction.MoveBack:
                        Move(new Vector3(0, 0, -1), Vector3.Zero());
                        break;
                    case (int)ClientAction.MoveLeft:
                        Move(new Vector3(-1, 0, 0),new Vector3(0,90,0));
                        break;
                    case (int)ClientAction.MoveRight:
                        Move(new Vector3(1, 0, 0), new Vector3(0, -90, 0));
                        break;
                    case (int)ClientAction.RotateLeft:
                        Rotation(new Vector3(0, 90, 0));
                        break;
                    case (int)ClientAction.RotateRight:
                        Rotation(new Vector3(0, -90, 0));
                        break;
                    case (int)ClientAction.Action:
                        PerformAnAction();
                        break;
                    case (int)ClientAction.GetCameraPosition:
                        
                       break;
                    case (int)ClientAction.Jump:
                        IsJump(true);
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
                            
                        break;

                }
               

                stopwatch.Stop();
               
                TimeSpan ts = stopwatch.Elapsed;
               
                    Console.WriteLine("{0} time:{1}\n Action:{2}\n", Name, ts.Ticks, (ClientAction)state);
                CanWrite = true;
              
            }
            catch (Exception e)
            {
                OnClientError?.Invoke(e, this);
            }
        }

    }
}
