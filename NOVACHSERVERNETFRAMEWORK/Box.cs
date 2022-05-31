using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOVACHSERVERNETFRAMEWORK
{
    internal class Box : WorldObject,IBoxCollider
    {
        private Physics _physics = new Physics();
        private BoxCollider _boxCollider;
        public readonly bool IsPhysics = true;

        public Box(Vector3 get,bool isPhysics) : base(TypeWorldObject.Box)
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
            _boxCollider = new BoxCollider(this);
            World.GetWorld().AddWorldObject(this);

        }
        public override void OnMove()
        {
           
        }

        public override void OnScale()
        {
          
        }
        public static void PushBoxs(WorldObject[] get,Vector3 vector3)
        {
            foreach (var item in get)
            {
                if (item is Box&&(item as Box).IsPhysics)
                    (item as Box).Push(vector3);
            }
        }
        public void Push(Vector3 pushVector3)
        {
            Vector3 vector3 = Vector3.Zero();
            if (pushVector3.X > 0)
                PushBoxs(_boxCollider.CollisionPlane(TypePlane.Right, ref vector3), pushVector3);
            if (pushVector3.X < 0)
                PushBoxs(_boxCollider.CollisionPlane(TypePlane.Left, ref vector3), pushVector3);
            if (pushVector3.Z > 0)
                PushBoxs(_boxCollider.CollisionPlane(TypePlane.Forward, ref vector3), pushVector3);
            if (pushVector3.Z < 0)
                PushBoxs(_boxCollider.CollisionPlane(TypePlane.Back, ref vector3), pushVector3);
            Position -= pushVector3;
            World.GetWorld().MoveWorldObject(this);
        }
        public override void OnUpdated()
        {
            if (!IsPhysics)
                return;
            Vector3 fallVector = Position;
            if (_boxCollider.CollisionPlane(TypePlane.Down, ref fallVector).Length == 0 && _physics.Fall(ref fallVector))
            {
                Position = fallVector;
                _boxCollider.CollisionPlane(TypePlane.Down, ref fallVector);
                Position = fallVector;
                World.GetWorld().MoveWorldObject(this);

            }
        }

        BoxCollider IBoxCollider.GetBoxCollider()
        {
            return _boxCollider;
        }

        void IBoxCollider.OnUpdatedBoxCollider()
        {
            throw new NotImplementedException();
        }
    }
}
