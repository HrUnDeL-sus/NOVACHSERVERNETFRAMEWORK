using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOVACHSERVERNETFRAMEWORK
{
    internal class StackWorldObject
    {
        public readonly WorldObject WorldObject;
        public readonly StateWorldObjectInStack WorldObjectInStack;
        public StackWorldObject(WorldObject get,StateWorldObjectInStack get2)
        {
            WorldObject = get;
            WorldObjectInStack = get2;
        }
    }
}
