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
    abstract class WorldObject
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
        public Vector3 Position { get; protected set; }
        public  Vector3 Scale { get; protected set; }
        public Vector3 Rotate { get; protected set; }
        public  Vector3 Color { get; protected set; }

        public readonly TypeWorldObject MyTypeWorldObject;
        public readonly string Name;
        protected readonly World MyWorld;
        public WorldObject(TypeWorldObject get,int health)
        {
            Health = health;
            Position = Vector3.Zero();
            Scale=Vector3.Zero();
            Rotate = Vector3.Zero();
            Color = Vector3.Zero();
            MyTypeWorldObject = get;
            UID =World.GetWorld().GetUIDForWolrdObject();
            Name = GetType().Name + UID;
            Console.WriteLine("NAME:{0}", Name);
            
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
