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
    public enum TypeBlock
    {
        Sand,
        Wood
    }
    internal class Player : WorldObject,IBoxCollider
    {
        private Timer _timerJump=new Timer(3);
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
            Position = Vector3.Zero();
                Color.X = (float)random.NextDouble();
                Color.Z = (float)random.NextDouble();
            _boxCollider = new BoxCollider(this);
            Scale = Vector3.One();
        }

        public override void OnMove()
        {

        }
        protected void IsJump(bool n)
        {
            if(!_isJump)
            _isJump = n;
        }
        public void SetBlock(TypeBlock typeBlock)
        {
            if (!_timerSetBlock.TimeIsUp())
            {
                return;
            }
            if (Rotate.Y == -90)
                new Box(Position + new Vector3(2, typeBlock == TypeBlock.Sand?0:-1, 0), typeBlock==TypeBlock.Sand);
            else if (Rotate.Y == 90)
                new Box(Position - new Vector3(2, typeBlock == TypeBlock.Sand ? 0 : 1, 0), typeBlock == TypeBlock.Sand);
            else if (Rotate.Y == 0)
                new Box(Position - new Vector3(0, typeBlock == TypeBlock.Sand ? 0 : 1, 2), typeBlock == TypeBlock.Sand);
            else if (Rotate.Y == 180)
                new Box(Position + new Vector3(0, typeBlock == TypeBlock.Sand ? 0 : -1, 2), typeBlock == TypeBlock.Sand);
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
       
        public async void Move(Vector3 position,Vector3 rotation)
        {
            await Task.Run(() =>
            {
                if (!_timerMove.TimeIsUp())
                {
                    return;
                }
                Vector3 pushVector = position;
                position *= _moveSpeed;

                Position += position;

                Rotate = rotation;
                Vector3 test = Position;

                if (position.X > 0)
                    Box.PushBoxs(_boxCollider.CollisionPlane(TypePlane.Right, ref test), pushVector * -0.1f);
                if (position.X < 0)
                    Box.PushBoxs(_boxCollider.CollisionPlane(TypePlane.Left, ref test), pushVector * -0.1f);
                if (position.Z > 0)
                    Box.PushBoxs(_boxCollider.CollisionPlane(TypePlane.Forward, ref test), pushVector * -0.1f);
                if (position.Z < 0)
                    Box.PushBoxs(_boxCollider.CollisionPlane(TypePlane.Back, ref test), pushVector * -0.1f);
                Position = test;
                World.GetWorld().MoveWorldObject(this);
            });
        }
        private void UpdateJump()
        {

            if (_isJump)
            {
                float speed = _jumpDistanceY < 0 ? -0.5f : 0.5f;
                if (!_timerJump.TimeIsUp())
                    return;
                Position += new Vector3(0, speed, 0);
                Vector3 jumpVector = Position;
                _jumpDistanceY -= Math.Abs(speed);

                if (_boxCollider.CollisionPlane(TypePlane.Down, ref jumpVector).Length != 0 || _jumpDistanceY <= -MAX_JUMP_DISTANCE_Y)
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
            if (!_isJump&&_physics.Fall(ref fallVector))
            {
                Position = fallVector;
                if(_boxCollider.CollisionPlane(TypePlane.Down, ref fallVector).Length == 0)
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
