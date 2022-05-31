using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOVACHSERVERNETFRAMEWORK
{
    internal class Physics
    {
        private const float G = 0.5f;
        private Timer _timerFall = new Timer(3);
        public Physics()
        {

        }
        public bool Fall(ref Vector3 position)
        {

            if (!_timerFall.TimeIsUp())
            {
                return false;
            }
            position -= new Vector3(0, G, 0);
           
            return true;
        }
    }
}
