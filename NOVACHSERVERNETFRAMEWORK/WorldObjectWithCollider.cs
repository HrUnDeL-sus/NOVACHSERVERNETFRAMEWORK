using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOVACHSERVERNETFRAMEWORK
{
    internal abstract class WorldObjectWithCollider:WorldObject,IBoxCollider
    {
        protected BoxCollider boxCollider;
        protected bool CheckCollision = true;
        public override Vector3 Position
        {
            get => base.Position; protected set
            {
             
                Vector3 _startMoveVector = Vector3.Zero();
                int count = 0;
                if (Position == null|| CheckCollision==false)
                {
                    base.Position = value;
                    CheckCollision = true;
                    return;
                }
               
                Vector3 localPosition = Position;
                Vector3 vector3 = Position;
                if(value.X!= localPosition.X)
                    _startMoveVector.X = value.X> localPosition.X ? 1:-1;
                if (value.Y != localPosition.Y)
                    _startMoveVector.Y = value.Y > localPosition.Y ? 1 : -1;
                if (value.Z != localPosition.Z)
                    _startMoveVector.Z = value.Z > localPosition.Z ? 1 : -1;
                _startMoveVector *= 0.1f;
                bool hasCollision = false;
                while (Math.Abs(localPosition.X-value.X)>0.1f||
                    Math.Abs(localPosition.Y - value.Y) >0.1f ||
                    Math.Abs(localPosition.Z - value.Z) >0.1f)
                {
                    localPosition += _startMoveVector;
                    vector3 = localPosition;
                    //  Console.WriteLine("POSITION: {0} {1} {2}\n End Position: {3} {4} {5}\n Move Vector: {6} {7} {8}", localPosition.X, localPosition.Y, localPosition.Z,value.X,value.Y,value.Z,_startMoveVector.X,_startMoveVector.Y,_startMoveVector.Z);
                    WorldObject[] get;
                    if (hasCollision)
                        return;
                    if (_startMoveVector.Y < 0 && (get = boxCollider.HasCollisionPlane(TypePlane.Center, ref vector3, localPosition)).Length != 0)
                    {
                        hasCollision = true;
                        base.Position = vector3;
                        OnUpdatedBoxCollider(TypePlane.Center, get);
                        World.GetWorld().MoveWorldObject(this);
                    }
                    if (_startMoveVector.X > 0 &&(get=boxCollider.HasCollisionPlane(TypePlane.Right, ref vector3, localPosition)).Length != 0)
                    {
                        hasCollision = true;
                        base.Position = vector3;
                       
                            World.GetWorld().MoveWorldObject(this);
                        OnUpdatedBoxCollider(TypePlane.Right, get);
                   
                    }
                    else if (_startMoveVector.X < 0 && (get = boxCollider.HasCollisionPlane(TypePlane.Left, ref vector3, localPosition)).Length != 0)
                    {
                        hasCollision = true;
                        base.Position = vector3;
                      
                            World.GetWorld().MoveWorldObject(this);
                        OnUpdatedBoxCollider(TypePlane.Left, get);
                       
                    }
                     if (_startMoveVector.Y > 0 && (get = boxCollider.HasCollisionPlane(TypePlane.Up, ref vector3, localPosition)).Length != 0)
                    {
                        hasCollision = true;
                        base.Position = vector3;
                        OnUpdatedBoxCollider(TypePlane.Up, get);
                        if (count < 1)
                            continue;
                        World.GetWorld().MoveWorldObject(this);
                       
                       
                    }
                    else if (_startMoveVector.Y < 0 && (get = boxCollider.HasCollisionPlane(TypePlane.Down, ref vector3, localPosition)).Length != 0)
                    {
                        hasCollision = true;
                        base.Position = vector3;
                        OnUpdatedBoxCollider(TypePlane.Down, get);
                        if (count < 1)
                            continue;
                        World.GetWorld().MoveWorldObject(this);
                           
                       
                       
                    }
                   
                    if (_startMoveVector.Z > 0 && (get = boxCollider.HasCollisionPlane(TypePlane.Forward, ref vector3, localPosition)).Length != 0)
                    {
                        hasCollision = true;
                        base.Position = vector3;
                       
                            World.GetWorld().MoveWorldObject(this);
                        OnUpdatedBoxCollider(TypePlane.Forward, get);
                       
                    }
                    else if (_startMoveVector.Z < 0 && (get = boxCollider.HasCollisionPlane(TypePlane.Back, ref vector3, localPosition)).Length != 0)
                    {
                        hasCollision = true;
                        base.Position = vector3;
                      
                            World.GetWorld().MoveWorldObject(this);
                        OnUpdatedBoxCollider(TypePlane.Back, get);
                      
                    }

                    if (hasCollision)
                        return;
                    if (count == 10)
                    {
                        base.Position = localPosition;
                        World.GetWorld().MoveWorldObject(this);
                        count = 0;
                       
                    }
                    count += 1;
                }
                base.Position = value;
                OnUpdatedBoxCollider(TypePlane.None,null);
                World.GetWorld().MoveWorldObject(this);
            }
        }

        public WorldObjectWithCollider(TypeWorldObject typeWorldObject,int health):base(typeWorldObject, health)
        {
            boxCollider = new BoxCollider(this);
        }
        
        BoxCollider IBoxCollider.GetBoxCollider()
        {
            return boxCollider;
        }

        protected abstract void OnUpdatedBoxCollider(TypePlane typePlane,WorldObject[] worldObjects);
    }
}
