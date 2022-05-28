using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOVACHSERVERNETFRAMEWORK
{
    internal class Sun : WorldObject
    {
        private static Sun _sun;
        private Timer _timerMove = new Timer(10);
        private Vector3 _moveVector = Vector3.Zero();
        public Sun() : base(TypeWorldObject.Sun)
        {
            Position = new Vector3(0, -5, 0);
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
        //   _moveVector += new Vector3(0.1f, 0.1f, 0.1f);
         //   Position += new Vector3((float)Math.Cos(_moveVector.X) * 100, (float)Math.Sin(_moveVector.Y)*100, (float)Math.Cos(_moveVector.Z)*100);
        //    World.GetWorld().MoveWorldObject(this);
        }
    }
}
