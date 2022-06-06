﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NOVACHSERVERNETFRAMEWORK
{
  
    internal class World
    {
        private static World _world;
        private event MoveObject OnMoved;
        private event ScaleObject OnScaled;
        private event ScaleObject OnUpdated;
        public event StackWorldObjectsWasUpdated OnUpdateStackWorldObjects;
        private int _globalUID = 0;

        private List<WorldObject> _worldObjects;
        private World()
        {
            _worldObjects = new List<WorldObject>();
           
            Thread thread = new Thread(new ThreadStart(Rendering));
            thread.Start();
        }
        public int GetUIDForWolrdObject()
        {
            _globalUID += 1;
            return _globalUID;
        }
        public int GetCountWorldObjects()
        {
            return _worldObjects.Count;
        }
        public void RemoveWorldObject(WorldObject worldObject)
        {
            _worldObjects.Remove(worldObject);
            OnUpdated -= worldObject.OnUpdated;
            if (worldObject is Client)
                OnUpdateStackWorldObjects -= (worldObject as Client).UpdateStackWorldObjects;
            OnUpdateStackWorldObjects?.Invoke(new StackWorldObject(worldObject, StateWorldObjectInStack.Deleted));
          


            Console.WriteLine("{0} destroy", worldObject.Name);
        }
        public WorldObject[] GetWorldObjects()
        {
            return _worldObjects.ToArray();
        }
        public Client GetClient(IPEndPoint iPEndPoint)
        {
            Client client = _worldObjects.Find((t)=>t is Client&&(t as Client).MyIpEndPoint.Address.ToString()==iPEndPoint.Address.ToString()) as Client;
            return client;
        }
      
        public static World GetWorld()
        {
            if (_world == null)
                _world = new World();
            return _world;
        }
        private void InitializateAllWolrdObjects()
        {
            foreach(WorldObject worldObject in _worldObjects)
                OnUpdateStackWorldObjects?.Invoke(new StackWorldObject(worldObject, StateWorldObjectInStack.Created));
        }
        public IBoxCollider[] GetWorldObjectsInRadiusWithBoxCollider(float R,Vector3 pos)
        {
            try
            {
                List<IBoxCollider> worldObjectsInRadius = new List<IBoxCollider>();
                foreach (WorldObject worldObject in _worldObjects)
                {
                    if (worldObject == null)
                        continue;
                    if (Math.Abs(worldObject.Position.Length - pos.Length-worldObject.Scale.Length) < R && worldObject is IBoxCollider)
                    {
                        worldObjectsInRadius.Add(worldObject as IBoxCollider);
                    }
                }
                return worldObjectsInRadius.ToArray();
            }
            catch
            {
                return null;
            }
            
        }
        public void MoveWorldObject(WorldObject worldObject)
        {
            if(_worldObjects.Contains(worldObject))
            OnUpdateStackWorldObjects?.Invoke(new StackWorldObject(worldObject, StateWorldObjectInStack.Updated));
            _worldObjects.Sort(new WorldObjectSort());
        }
        public void AddWorldObject(WorldObject worldObject)
        {
            _worldObjects.Add(worldObject);
            if (_worldObjects[_worldObjects.Count - 1] is Client)
            {
                (_worldObjects[_worldObjects.Count - 1] as Client).OnClientError += ClientErrorHasOccurred;
                OnUpdateStackWorldObjects+=(_worldObjects[_worldObjects.Count - 1] as Client).UpdateStackWorldObjects;
                InitializateAllWolrdObjects();
            }
            else
            OnUpdateStackWorldObjects?.Invoke(new StackWorldObject(_worldObjects[_worldObjects.Count - 1], StateWorldObjectInStack.Created));
            OnMoved += _worldObjects[_worldObjects.Count - 1].OnMove;
            OnScaled += _worldObjects[_worldObjects.Count - 1].OnScale;
            OnUpdated += _worldObjects[_worldObjects.Count - 1].OnUpdated;
            _worldObjects.Sort(new WorldObjectSort());
        }
        private void ClientErrorHasOccurred(Exception e,Client client)
        {
            RemoveWorldObject(client);
          
        }
        public void Rendering()
        {
            while (true)
            {

                OnUpdated?.Invoke();
               
            }
        }
    }
}
