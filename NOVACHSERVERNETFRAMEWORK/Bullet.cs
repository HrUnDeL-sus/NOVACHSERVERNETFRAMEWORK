using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOVACHSERVERNETFRAMEWORK
{
    internal class Bullet:WorldObject,IBoxCollider
    {
        private const float SPEED = 4;
        private Timer _timerMove = new Timer(3);
        private Timer _destroy = new Timer(30);
        private readonly Vector3 _moveVector;
        private readonly Vector3 _startMoveVector;
        private BoxCollider _boxCollider;
        public Bullet(Vector3 startVector,Vector3 moveVector):base(TypeWorldObject.Bullet, 10)
        {
            Scale = Vector3.One()*0.5f;
            Position = startVector + moveVector;
            World.GetWorld().AddWorldObject(this);
            _moveVector = moveVector * SPEED;
            _startMoveVector = moveVector / 2;
            _boxCollider = new BoxCollider(this);
        }

        public override void OnMove()
        {
            throw new NotImplementedException();
        }

        public override void OnScale()
        {
            throw new NotImplementedException();
        }

        public override void OnUpdated()
        {
            if (!_timerMove.TimeIsUp())
                return;
            if (_destroy.TimeIsUp())
            {
                Kill();
                return;
            }
            bool hasCollision = false;
            Vector3 resultPosition = Position + _moveVector;
            while (Position > resultPosition ? Position > resultPosition : Position < resultPosition)
            {
                Position += _startMoveVector;
                foreach (var item in _boxCollider.HasAnyCollisionPlaneArray())
                {
                    hasCollision = true;
                    item.Damage(10);
                }
                if (hasCollision)
                    Kill();

            }
            if(!hasCollision)
            World.GetWorld().MoveWorldObject(this);
        }

        public BoxCollider GetBoxCollider()
        {
            return _boxCollider;
        }

        public void OnUpdatedBoxCollider()
        {
            throw new NotImplementedException();
        }
    }
}
