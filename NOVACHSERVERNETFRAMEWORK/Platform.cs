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
        public Platform() : base(TypeWorldObject.Platform,(int)HealthEnum.Infinity)
        {
            Position = new Vector3(0,-30,0);
            Scale = new Vector3(100,20,100);
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

       
    }
}
