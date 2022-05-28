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
        public Box(Vector3 get) : base(TypeWorldObject.Box)
        {
            Position = get;
            Scale = Vector3.One()*2;
            Color.X = (float)new Random().NextDouble();
            _boxCollider = new BoxCollider(this);
            World.GetWorld().AddWorldObject(this);

        }
        public override void OnMove()
        {
           
        }

        public override void OnScale()
        {
          
        }
        public override void OnUpdated()
        {
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
