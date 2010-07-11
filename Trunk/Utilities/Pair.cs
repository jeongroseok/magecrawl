using System;
using System.Collections.Generic;

namespace Magecrawl.Utilities
{
    public class Pair<T, U>
    {
        public Pair()
        {
        }

        public Pair(T first, U second)
        {
            this.First = first;
            this.Second = second;
        }

        public T First { get; set; }
        public U Second { get; set; }

        public override string ToString()
        {
            return First.ToString() + "," + Second.ToString();
        }
    }
}
