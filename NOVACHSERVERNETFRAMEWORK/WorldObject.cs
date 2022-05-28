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
        Sun
    }
    abstract class WorldObject
    {
        public readonly int UID;
        private Vector3 _position;
        private Vector3 _scale;
        private Vector3 _rotate;
        private Vector3 _color;
        public Vector3 Position { get; protected set; }
        public Vector3 Scale { get; protected set; }
        public Vector3 Rotate { get; protected set; }
        public Vector3 Color { get; protected set; }

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
