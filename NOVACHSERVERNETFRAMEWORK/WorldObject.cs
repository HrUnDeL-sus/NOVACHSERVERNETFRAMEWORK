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
    public enum HealthEnum
    {
        Infinity=100000000
    }
    internal class WorldObjectSort : IComparer<WorldObject>
    {
        public int Compare(WorldObject a, WorldObject b)
        {
            if (a.Position.Y > b.Position.Y)
                return -1;
            else if (a.Position.Y < b.Position.Y)
                return 1;
            else
                return 0;
        }
    }
   internal abstract class WorldObject
    {
        public readonly int UID;
        private int _health;
        public int Health
        {
            get => _health; private set
            {
                _health = value;
                if (_health <= 0)
                    OnKilled();
            }
        }
        public virtual Vector3 Position { get; protected set; }
        public  Vector3 Scale { get; protected set; }
        public Vector3 Rotate { get; protected set; }
        public  Vector3 Color { get; protected set; }

        public readonly TypeWorldObject MyTypeWorldObject;
        public readonly string Name;
        protected readonly World MyWorld;
        public WorldObject(TypeWorldObject get,int health)
        {
            Health = health;
            Scale=Vector3.Zero();
            Rotate = Vector3.Zero();
            Color = Vector3.Zero();
            MyTypeWorldObject = get;
            UID =World.GetWorld().GetUIDForWolrdObject();
            Name = GetType().Name + UID;
            Console.WriteLine("NAME:{0}", Name);
            
        }
        protected Vector3 MathWathingVector()
        {
            if (Rotate.Y == -90)
                return new Vector3(1, 0, 0);
            else if (Rotate.Y == 90)
                return new Vector3(-1, 0, 0);
            else if (Rotate.Y == 0)
                return new Vector3(0, 0, -1);
            else if (Rotate.Y == 180)
                return new Vector3(0, 0, 1);
            else
                return Vector3.Zero();
        }
        protected virtual void OnKilled()
        {
            World.GetWorld().RemoveWorldObject(this);
        }
        public void Kill()
        {
            Health = 0;
        }
        public void Damage(int d)
        {
            if (Health == (int)HealthEnum.Infinity)
                return;
            Health -= d;
        }
        public abstract void OnMove();
        public abstract void OnScale();
        public abstract void OnUpdated();
    }
}
