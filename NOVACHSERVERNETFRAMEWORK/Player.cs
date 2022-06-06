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
    public enum ActiveAction
    {
        SetBlock,
        Shot
    }
    internal class Player : WorldObjectWithCollider
    {
        private Timer _timerJump=new Timer(3);
        private bool _canJump = false;
        private bool _isJump = false;
        private Physics _physics = new Physics();
        private Timer _jumpTimer = new Timer(3);
        private Timer _timerMove = new Timer(1);
        private Timer _timerMoveMax = new Timer(40);
      
        private Timer _timerActive = new Timer(15);
        private Vector3 _moveSpeed = Vector3.One()*0.5f;
        private float _jumpDistanceY = 5;
        private const float MAX_JUMP_DISTANCE_Y = 5;
        public TypeBlock SelectedBlock { get; protected set; }
        public ActiveAction MyActiveAction { get; protected set; }
        public Player():base(TypeWorldObject.Player, 100)
        {
                Random random = new Random();
            Position = Vector3.Zero();
                Color.X = (float)random.NextDouble();
                Color.Z = (float)random.NextDouble();
          
            Scale = Vector3.One();
            MyActiveAction = ActiveAction.SetBlock;
            SelectedBlock = TypeBlock.Sand;
        }
        public override void OnMove()
        {

        }
        protected void PerformAnAction()
        {
            if (!_timerActive.TimeIsUp())
                return;
            switch (MyActiveAction)
            {
                case ActiveAction.SetBlock:
                    SetBlock(SelectedBlock);
                    break;
                case ActiveAction.Shot:
                    new Bullet(Position, MathWathingVector());
                    break;
                default:
                    break;
            }
        }
        protected void IsJump(bool n)
        {
            Vector3 vector3 = Vector3.Zero();
            if (!_isJump&&_jumpTimer.TimeIsUp()&& _canJump)
            {
                    _isJump = n;
            }
        }
       
        private void SetBlock(TypeBlock typeBlock)
        {
            if (Rotate.Y == -90)
                new Box(Position + new Vector3(2, typeBlock == TypeBlock.Sand?2:-1, 0), typeBlock==TypeBlock.Sand);
            else if (Rotate.Y == 90)
                new Box(Position - new Vector3(2, typeBlock == TypeBlock.Sand ? -2 : 1, 0), typeBlock == TypeBlock.Sand);
            else if (Rotate.Y == 0)
                new Box(Position - new Vector3(0, typeBlock == TypeBlock.Sand ? -2 : 1, 2), typeBlock == TypeBlock.Sand);
            else if (Rotate.Y == 180)
                new Box(Position + new Vector3(0, typeBlock == TypeBlock.Sand ? 2 : -1, 2), typeBlock == TypeBlock.Sand);
        }
        public int GetActiveAction()
        {
            if (MyActiveAction == ActiveAction.SetBlock)
                return (int)SelectedBlock;
            else
                return ((int)MyActiveAction) + Enum.GetNames(typeof(TypeBlock)).Length-1;
        }
        protected void Rotation(Vector3 rotate)
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

        protected void Move(Vector3 position,Vector3 rotation)
        {
            if (!_timerMove.TimeIsUp())
            {
                return;
            }
           
                Vector3 pushVector = position;
                position *= _moveSpeed;

                Position += position;

                Rotate = rotation;
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

                if ( _jumpDistanceY <= -MAX_JUMP_DISTANCE_Y)
                {

                    _jumpDistanceY = MAX_JUMP_DISTANCE_Y;
                    _isJump = false;
                }
                Position = jumpVector;
             
            }
        }
        protected void Update()
        {
            Vector3 fallVector = Position;
            Vector3 vector3 = Vector3.Zero();
            UpdateJump();
            if (Position.Y < -50)
            {
                CheckCollision = false;
                Position = Vector3.Zero();
            }
                

            if (!_isJump &&_physics.Fall(ref fallVector))
                Position = fallVector;

        }

        public override void OnScale()
        {

        }
       

        public override void OnUpdated()
        {
            throw new NotImplementedException();
        }

        protected override void OnUpdatedBoxCollider(TypePlane typePlane,WorldObject[] worldObjects)
        {
            if (typePlane == TypePlane.Down)
                _canJump = true;
            else
                _canJump = false;
            if (worldObjects == null)
                return;
            foreach (var item in worldObjects)
            {
                if(item is Box&&typePlane!=TypePlane.Down)
                {
                    (item as Box).Push(MathWathingVector());
                }
            }
        }
    }
}
