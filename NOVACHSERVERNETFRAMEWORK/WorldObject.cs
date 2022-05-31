using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NOVACHSERVERNETFRAMEWORK
{
    delegate void MoveObject();
    delegate void ScaleObject();
    delegate void UpdateObject();
    enum TypeWorldObject
    {
        Player,
        Platform,
        Box,
        Sun,
        Bullet,
        None
    }
    abstract class WorldObject
    {
        public readonly int UID;
        protected Vector3 position;
        protected Vector3 scale;
        protected Vector3 color;
        protected Vector3 rotate;
        public Vector3 Position { get=>position; protected set=>position=value; }
        public  Vector3 Scale { get => scale; protected set=>scale=value; }
        public Vector3 Rotate { get => rotate; protected set=> rotate = value; }
        public  Vector3 Color { get => color; protected set=>color=value; }

        public readonly TypeWorldObject MyTypeWorldObject;
        public readonly string Name;
        protected readonly World MyWorld;
        public WorldObject(TypeWorldObject get)
        {
            Position = Vector3.Zero();
            Scale=Vector3.Zero();
            Rotate = Vector3.Zero();
            Color = Vector3.Zero();
            MyTypeWorldObject = get;
            UID =World.GetWorld().GetUIDForWolrdObject();
            Name = GetType().Name + UID;
            Console.WriteLine("NAME:{0}", Name);
            
        }
        public abstract void OnMove();
        public abstract void OnScale();
        public abstract void OnUpdated();
    }
}
