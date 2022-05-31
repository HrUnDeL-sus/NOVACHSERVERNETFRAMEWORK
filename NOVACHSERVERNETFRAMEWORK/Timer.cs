using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NOVACHSERVERNETFRAMEWORK
{
    internal class Timer
    {
        private int _startTime;
        private readonly int _endTime;
        private static List<Timer> _timers = new List<Timer>();
        private void AddTime()
        {
            _startTime += 1;
           
        }
        public static void StartTimers()
        {
            while (true)
            {
                for(int i = 0; i < _timers.Count; i++)
                {
                    if(i<_timers.Count&&_timers[i]!=null)
                        _timers[i].AddTime();
                }
                Thread.Sleep(10);
            }
        }
        public Timer(int endTime)
        {
            _endTime = endTime;
            _timers.Add(this);
        }
        public void Restart()
        {
            _startTime = 0;
        }
        public bool TimeIsUp()
        {
          
            if (_endTime - _startTime <= 0)
            {

                Restart();
                return true;
            }
          
            return false;
        }
    }
}
