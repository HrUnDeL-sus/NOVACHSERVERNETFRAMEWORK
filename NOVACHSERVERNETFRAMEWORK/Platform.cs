using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOVACHSERVERNETFRAMEWORK
{
    internal class Platform : WorldObject,IBoxCollider
    {
        private BoxCollider _boxCollider;
        public Platform(Vector3 get) : base(TypeWorldObject.Platform)
        {
            Position = get-new Vector3(0,1,0);
            Scale = new Vector3(100,1,100);
            Color.Y = (float)(new Random().NextDouble()+0.7);
            _boxCollider = new BoxCollider(this);
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
