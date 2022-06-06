using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOVACHSERVERNETFRAMEWORK
{
    internal class Box :WorldObjectWithCollider
    {
        private Physics _physics = new Physics();
        private Timer _physicMaxFall = new Timer(10);
        private Vector3 _pushVector = Vector3.Zero();
        public readonly bool IsPhysics = true;
      

        public Box(Vector3 get,bool isPhysics) : base(TypeWorldObject.Box, 80)
        {
            IsPhysics = isPhysics;
            Position = get;
            Scale = Vector3.One()*2;
            if (IsPhysics)
            {
                Color.X = 1;
                Color.Y = 1;
            }
            else
            {
                Color.X = 0.7f;
                Color.Y = 0.5f;
                Color.Z = 0.4f;
            }
       
            World.GetWorld().AddWorldObject(this);

        }
        public void Push(Vector3 moveVector)
        {
            Position += moveVector;
        }
        public override void OnMove()
        {
           
        }

        public override void OnScale()
        {
          
        }
        public override void OnUpdated()
        {
            if (!IsPhysics)
                return;
            Vector3 fallVector = Position;
            if (_physics.Fall(ref fallVector))
                Position = fallVector;
        }

        protected override void OnUpdatedBoxCollider(TypePlane typePlane,WorldObject[] worldObjects)
        {
            if (typePlane == TypePlane.Center)
                Kill();
        }
    }
}
