using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Boids.ui
{
    public class ControlPair<TFirst, TSecond>
    {
        public TFirst First { get; set; }
        public TSecond Second { get; set; }

        public ControlPair(TFirst first, TSecond second)
        {
            First = first;
            Second = second;
        }
    }
}
