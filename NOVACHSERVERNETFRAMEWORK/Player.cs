using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace NOVACHSERVERNETFRAMEWORK
{
    internal class Player : WorldObject,IBoxCollider
    {
        private Timer _timerFall = new Timer(3);
        private bool _isJump = false;
        private Physics _physics = new Physics();
        private Timer _timerMove = new Timer(3);
        private Timer _timerSetBlock = new Timer(15);
        private Vector3 _moveSpeed = Vector3.One()*0.5f;
        private float _jumpDistanceY = 5;
        private const float MAX_JUMP_DISTANCE_Y = 5;
        private BoxCollider _boxCollider;
        public Player():base(TypeWorldObject.Player)
        {
                Random random = new Random();
                Position.X = random.Next(0,10);
                Position.Z = random.Next(0, 10);
                Color.X = (float)random.NextDouble();
                Color.Z = (float)random.NextDouble();
                Position.Y = 1;
            _boxCollider = new BoxCollider(this);
            Scale = Vector3.One();
        }

        public override void OnMove()
        {

        }
        public void SetBlock()
        {
            if (!_timerSetBlock.TimeIsUp())
            {
                return;
            }
            if (Rotate.Y == -90)
                new Box(Position + new Vector3(1, 0, 0));
            else if (Rotate.Y == 90)
                new Box(Position - new Vector3(1, 0, 0));
            else if (Rotate.Y == 0)
                new Box(Position - new Vector3(0, 0, 1));
            else if (Rotate.Y == 180)
                new Box(Position + new Vector3(0, 0, 1));
        }
        public void Jump()
        {
           
            Position -= new Vector3(0,0.1f,0);
            Vector3 test = Position;
            if (_boxCollider.CollisionPlane(TypePlane.Down, ref test).Length != 0)
                _isJump = true;
            Position = test;
        }
        public void Rotation(Vector3 rotate)
        {
            if (!_timerMove.TimeIsUp())
            {
                return;
            }
            Rotate += rotate;
            if (Rotate.Y > 180)
                Rotate.Y = -90;
            else if (Rotate.Y < -90)
                Rotate.Y = 180;
            World.GetWorld().MoveWorldObject(this);

        }
        public bool Move(Vector3 position,Vector3 rotation)
        {
            if (!_timerMove.TimeIsUp())
            {
                return false;
            }

            position *= _moveSpeed;
           
            Position += position;
           
            Rotate = rotation;
            Vector3 test = Position;

            if (position.X > 0)
                _boxCollider.CollisionPlane(TypePlane.Right, ref test);
            if (position.X < 0)
                _boxCollider.CollisionPlane(TypePlane.Left, ref test);
            if (position.Z > 0)
                _boxCollider.CollisionPlane(TypePlane.Forward, ref test);
            if (position.Z < 0)
                _boxCollider.CollisionPlane(TypePlane.Back, ref test);
            Position = test;



            World.GetWorld().MoveWorldObject(this);
            return true;
        }
        private void UpdateJump()
        {
            if (_isJump)
            {
                Position += new Vector3(0, 0.1f, 0);
                Vector3 jumpVector = Position;
                _jumpDistanceY -= 0.1f;
                if (_boxCollider.CollisionPlane(TypePlane.Down, ref jumpVector).Length != 0 || _jumpDistanceY <= 0)
                {
                    _jumpDistanceY = MAX_JUMP_DISTANCE_Y;
                    _isJump = false;
                }
                Position = jumpVector;
                World.GetWorld().MoveWorldObject(this);
            }
        }
        public void Update()
        {
            Vector3 fallVector = Position;
            UpdateJump();
            if (!_isJump && _boxCollider.CollisionPlane(TypePlane.Down, ref fallVector).Length==0&&_physics.Fall(ref fallVector))
            {
                Position = fallVector;
                _boxCollider.CollisionPlane(TypePlane.Down, ref fallVector);
                Position = fallVector;
                World.GetWorld().MoveWorldObject(this);
               
            }
           
        }

        public override void OnScale()
        {

        }
       

        public override void OnUpdated()
        {
            throw new NotImplementedException();
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
