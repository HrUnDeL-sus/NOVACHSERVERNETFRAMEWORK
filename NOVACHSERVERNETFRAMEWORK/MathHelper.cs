using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOVACHSERVERNETFRAMEWORK
{
    internal static class MathHelper
    {
        public static double DegreesToRadians(double n)
        {
            return n * Math.PI / 180;
        }
    }
}
