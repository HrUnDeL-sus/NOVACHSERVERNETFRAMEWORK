using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOVACHSERVERNETFRAMEWORK
{
    internal class Sun : WorldObject
    {
        private Timer _timerMove = new Timer(30);
        private Vector3 _moveVector = new Vector3(0.1f,0.1f, 0.1f);
        public Sun() : base(TypeWorldObject.Sun,1000)
        {
            Position = new Vector3(0, -50, 0);
            Color.X = 1;
            Color.Y = 0.5f;
            Color.Z = 1;
            Scale = Vector3.One();
           
        }
        public override void OnMove()
        {
           
        }

        public override void OnScale()
        {
         
        }

        public override void OnUpdated()
        {
            if (!_timerMove.TimeIsUp())
                return;
            _moveVector += Vector3.One() * 0.1f;
            Position += new Vector3((float)Math.Cos(_moveVector.X) * 10, (float)Math.Sin(_moveVector.Y) * 10, (float)Math.Cos(_moveVector.Z) * 10);
            World.GetWorld().MoveWorldObject(this);
        }
    }
}
