using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOVACHSERVERNETFRAMEWORK
{
    enum TypePlane
    {
        Right,
        Left,
        Up,
        Down,
        Back,
        Forward
    }
    struct Plane
    {
        //низниз
        public Vector3 xyz1 { get; private set; }
        //низверх
        public Vector3 xyz2 { get; private set; }
        //верхверх
        public Vector3 xyz3 { get; private set; }
        //верхниз
        public Vector3 xyz4 { get; private set; }
        public Plane(Vector3[] vector3s)
        {
            xyz1 = vector3s[0];
            xyz2 = vector3s[1];
            xyz3 = vector3s[2];
            xyz4 = vector3s[3];
        }
        public void Sub(float n)
        {
            xyz1 -= n;
            xyz2 -= n;
            xyz3 -= n;
            xyz4 -= n;
        }
       
    }
    internal class BoxCollider
    {
        private Plane Left;
        private Plane Right;
        private Plane Forward;
        private Plane Back;
        private Plane Up;
        private Plane Down;
        private WorldObject _worldObject;
        public BoxCollider(WorldObject get)
        {
            _worldObject = get;
            UpdatePoints();
        }
        private bool BeetwenPlanes(BoxCollider boxCollider,TypePlane typePlane, Vector3[] vector3)
        {
            foreach (var item in vector3)
            {
                switch (typePlane)
                {
                    case TypePlane.Right:
                    case TypePlane.Left:
                        if(item.X >= boxCollider.Left.xyz2.X && item.X <= boxCollider.Right.xyz2.X && item.Y >= boxCollider.Right.xyz1.Y && item.Y <= boxCollider.Right.xyz3.Y && item.Z >= boxCollider.Right.xyz1.Z && item.Z <= boxCollider.Right.xyz3.Z)
                            return true;
                        break;
                    case TypePlane.Up:
                    case TypePlane.Down:
                        if (item.Y >= boxCollider.Down.xyz2.Y && item.Y <= boxCollider.Up.xyz2.Y && item.X >= boxCollider.Up.xyz1.X && item.X <= boxCollider.Up.xyz3.X && item.Z >= boxCollider.Up.xyz1.Z && item.Z <= boxCollider.Up.xyz3.Z)
                            return true;
                        break;
                    case TypePlane.Back:
                    case TypePlane.Forward:
                        if(item.Z >= boxCollider.Back.xyz2.Z && item.Z <= boxCollider.Forward.xyz2.Z && item.Y >= boxCollider.Back.xyz1.Y && item.Y <= boxCollider.Back.xyz3.Y && item.X >= boxCollider.Back.xyz1.X && item.X <= boxCollider.Back.xyz3.X)
                            return true;
                        break;


                }
            }
            return false;
        }
        public WorldObject[] CollisionPlane(TypePlane typePlane,ref Vector3 resultPosition)
        {
            UpdatePoints();
            List<WorldObject> worldObjects = new List<WorldObject>();
            foreach (IBoxCollider worldObject in World.GetWorld().GetWorldObjectsInRadiusWithBoxCollider(_worldObject.Scale.Length * 150, _worldObject.Position))
            {
                BoxCollider boxCollider = worldObject.GetBoxCollider();
                if (boxCollider == this)
                    continue;
                boxCollider.UpdatePoints();
                switch (typePlane)
                {
                    case TypePlane.Right:
                        if (BeetwenPlanes(this, typePlane, new Vector3[] { boxCollider.Left.xyz1, boxCollider.Left.xyz2, boxCollider.Left.xyz3, boxCollider.Left.xyz4 }) ||
                            BeetwenPlanes(boxCollider, typePlane, new Vector3[] { Right.xyz1, Right.xyz2, Right.xyz3, Right.xyz4 }))
                        {
                            worldObjects.Add(worldObject as WorldObject);
                            resultPosition.X =boxCollider._worldObject.Position.X - boxCollider._worldObject.Scale.X/2 - _worldObject.Scale.X/2- 0.1f;
                        }
                        break;
                    case TypePlane.Left:
                        if (BeetwenPlanes(this,typePlane, new Vector3[] { boxCollider.Right.xyz1, boxCollider.Right.xyz2, boxCollider.Right.xyz3, boxCollider.Right.xyz4 })||
                            BeetwenPlanes(boxCollider, typePlane, new Vector3[] { Left.xyz1, Left.xyz2, Left.xyz3, Left.xyz4 }))
                        {
                            worldObjects.Add(worldObject as WorldObject);
                            resultPosition.X = boxCollider._worldObject.Position.X + boxCollider._worldObject.Scale.X / 2 + _worldObject.Scale.X / 2 + 0.1f;
                        }
                            
                        break;
                    case TypePlane.Up:
                        if (BeetwenPlanes(this,typePlane, new Vector3[] { boxCollider.Down.xyz1, boxCollider.Down.xyz2, boxCollider.Down.xyz3, boxCollider.Down.xyz4 })||
                            BeetwenPlanes(boxCollider, typePlane, new Vector3[] { Up.xyz1, Up.xyz2, Up.xyz3, Up.xyz4 }))
                        {
                            worldObjects.Add(worldObject as WorldObject);
                            resultPosition.Y = boxCollider._worldObject.Position.Y - boxCollider._worldObject.Scale.Y / 2 - _worldObject.Scale.Y / 2 + 0.1f;
                        }
                        break;
                    case TypePlane.Down:
                        if (BeetwenPlanes(this, typePlane, new Vector3[] { boxCollider.Up.xyz1, boxCollider.Up.xyz2, boxCollider.Up.xyz3, boxCollider.Up.xyz4 }) ||
                            BeetwenPlanes(boxCollider, typePlane, new Vector3[] { Back.xyz1, Back.xyz2, Back.xyz3, Back.xyz4 }
                            ))
                        {
                            resultPosition.Y = boxCollider._worldObject.Position.Y + boxCollider._worldObject.Scale.Y / 2 + _worldObject.Scale.Y / 2 + 0.1f;
                            worldObjects.Add(worldObject as WorldObject);
                        }
                           
                        break;
                    case TypePlane.Back:
                        if (BeetwenPlanes(this, typePlane, new Vector3[] { boxCollider.Forward.xyz1, boxCollider.Forward.xyz2, boxCollider.Forward.xyz3, boxCollider.Forward.xyz4 })||
                            BeetwenPlanes(boxCollider, typePlane, new Vector3[] { Back.xyz1, Back.xyz2, Back.xyz3, Back.xyz4 })
                            ) {
                            worldObjects.Add(worldObject as WorldObject);
                            resultPosition.Z = boxCollider._worldObject.Position.Z + boxCollider._worldObject.Scale.Z / 2 + _worldObject.Scale.Z / 2 + 0.1f;
                        }
                           
                        break;
                    case TypePlane.Forward:
                        if (BeetwenPlanes(this, typePlane, new Vector3[] { boxCollider.Back.xyz1, boxCollider.Back.xyz2, boxCollider.Back.xyz3, boxCollider.Back.xyz4 })||
                            BeetwenPlanes(boxCollider, typePlane, new Vector3[] { Forward.xyz1, Forward.xyz2, Forward.xyz3, Forward.xyz4 }))
                        {
                            resultPosition.Z = boxCollider._worldObject.Position.Z - boxCollider._worldObject.Scale.Z / 2 - _worldObject.Scale.Z / 2 - 0.1f;
                            worldObjects.Add(worldObject as WorldObject);
                        }
                            
                        break;
                    default:
                        break;
                }

            }
            return worldObjects.ToArray();
        }
        private void UpdatePoints()
        {
            
            Back = new Plane(new Vector3[] {
                new Vector3(_worldObject.Position.X-_worldObject.Scale.X/2, _worldObject.Position.Y - _worldObject.Scale.Y / 2, _worldObject.Position.Z - _worldObject.Scale.Z / 2),
                new Vector3(_worldObject.Position.X - _worldObject.Scale.X / 2, _worldObject.Position.Y + _worldObject.Scale.Y / 2, _worldObject.Position.Z - _worldObject.Scale.Z / 2),
                new Vector3(_worldObject.Position.X + _worldObject.Scale.X / 2, _worldObject.Position.Y + _worldObject.Scale.Y / 2, _worldObject.Position.Z - _worldObject.Scale.Z / 2),
                new Vector3(_worldObject.Position.X + _worldObject.Scale.X / 2, _worldObject.Position.Y - _worldObject.Scale.Y / 2, _worldObject.Position.Z - _worldObject.Scale.Z / 2)
            });
            Forward = new Plane(new Vector3[] {
                new Vector3(_worldObject.Position.X-_worldObject.Scale.X/2, _worldObject.Position.Y - _worldObject.Scale.Y / 2, _worldObject.Position.Z + _worldObject.Scale.Z / 2),
                new Vector3(_worldObject.Position.X - _worldObject.Scale.X / 2, _worldObject.Position.Y + _worldObject.Scale.Y / 2, _worldObject.Position.Z + _worldObject.Scale.Z / 2),
                new Vector3(_worldObject.Position.X + _worldObject.Scale.X / 2, _worldObject.Position.Y + _worldObject.Scale.Y / 2, _worldObject.Position.Z + _worldObject.Scale.Z / 2),
                new Vector3(_worldObject.Position.X + _worldObject.Scale.X / 2, _worldObject.Position.Y - _worldObject.Scale.Y / 2, _worldObject.Position.Z + _worldObject.Scale.Z / 2)
            });
            Up = new Plane(new Vector3[] {
                new Vector3(_worldObject.Position.X-_worldObject.Scale.X/2, _worldObject.Position.Y + _worldObject.Scale.Y / 2, _worldObject.Position.Z - _worldObject.Scale.Z / 2),
                new Vector3(_worldObject.Position.X - _worldObject.Scale.X / 2, _worldObject.Position.Y + _worldObject.Scale.Y / 2, _worldObject.Position.Z + _worldObject.Scale.Z / 2),
                new Vector3(_worldObject.Position.X + _worldObject.Scale.X / 2, _worldObject.Position.Y + _worldObject.Scale.Y / 2, _worldObject.Position.Z + _worldObject.Scale.Z / 2),
                new Vector3(_worldObject.Position.X + _worldObject.Scale.X / 2, _worldObject.Position.Y + _worldObject.Scale.Y / 2, _worldObject.Position.Z - _worldObject.Scale.Z / 2)
            });
            Down = new Plane(new Vector3[] {
                new Vector3(_worldObject.Position.X-_worldObject.Scale.X/2, _worldObject.Position.Y - _worldObject.Scale.Y / 2, _worldObject.Position.Z - _worldObject.Scale.Z / 2),
                new Vector3(_worldObject.Position.X - _worldObject.Scale.X / 2, _worldObject.Position.Y - _worldObject.Scale.Y / 2, _worldObject.Position.Z + _worldObject.Scale.Z / 2),
                new Vector3(_worldObject.Position.X + _worldObject.Scale.X / 2, _worldObject.Position.Y - _worldObject.Scale.Y / 2, _worldObject.Position.Z + _worldObject.Scale.Z / 2),
                new Vector3(_worldObject.Position.X + _worldObject.Scale.X / 2, _worldObject.Position.Y - _worldObject.Scale.Y / 2, _worldObject.Position.Z - _worldObject.Scale.Z / 2)
            });
            Left = new Plane(new Vector3[] {
                new Vector3(_worldObject.Position.X-_worldObject.Scale.X/2, _worldObject.Position.Y - _worldObject.Scale.Y / 2, _worldObject.Position.Z - _worldObject.Scale.Z / 2),
                new Vector3(_worldObject.Position.X - _worldObject.Scale.X / 2, _worldObject.Position.Y - _worldObject.Scale.Y / 2, _worldObject.Position.Z + _worldObject.Scale.Z / 2),
                new Vector3(_worldObject.Position.X - _worldObject.Scale.X / 2, _worldObject.Position.Y + _worldObject.Scale.Y / 2, _worldObject.Position.Z + _worldObject.Scale.Z / 2),
                new Vector3(_worldObject.Position.X - _worldObject.Scale.X / 2, _worldObject.Position.Y + _worldObject.Scale.Y / 2, _worldObject.Position.Z - _worldObject.Scale.Z / 2)
            });
            Right = new Plane(new Vector3[] {
                new Vector3(_worldObject.Position.X+_worldObject.Scale.X/2, _worldObject.Position.Y - _worldObject.Scale.Y / 2, _worldObject.Position.Z - _worldObject.Scale.Z / 2),
                new Vector3(_worldObject.Position.X + _worldObject.Scale.X / 2, _worldObject.Position.Y - _worldObject.Scale.Y / 2, _worldObject.Position.Z + _worldObject.Scale.Z / 2),
                new Vector3(_worldObject.Position.X + _worldObject.Scale.X / 2, _worldObject.Position.Y + _worldObject.Scale.Y / 2, _worldObject.Position.Z + _worldObject.Scale.Z / 2),
                new Vector3(_worldObject.Position.X + _worldObject.Scale.X / 2, _worldObject.Position.Y + _worldObject.Scale.Y / 2, _worldObject.Position.Z - _worldObject.Scale.Z / 2)
            });
          

        }
    }
}
