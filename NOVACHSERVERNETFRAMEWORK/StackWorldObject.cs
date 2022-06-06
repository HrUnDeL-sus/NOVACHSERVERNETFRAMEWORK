using System;
using System.Collections;
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
    internal class StackWorldObjectSort : IComparer<StackWorldObject>
    {
        public int Compare(StackWorldObject a, StackWorldObject b)
        {
            if (a.WorldObjectInStack == StateWorldObjectInStack.Created)
                return 1;
            else if (a.WorldObjectInStack == StateWorldObjectInStack.Deleted)
                return 1;
            else if (b.WorldObjectInStack == StateWorldObjectInStack.Created)
                return -1;
            else if (b.WorldObjectInStack == StateWorldObjectInStack.Deleted)
                return -1;
            else
                return 0;

        }
    }
}
