using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Magecrawl.Maps.Generator
{
    public class MapGenerationFailureException : Exception
    {
        public MapGenerationFailureException() : base()
        {
        }

        public MapGenerationFailureException(string s) : base(s)
        {
        }

        public MapGenerationFailureException(string s, Exception e) : base(s, e)
        {
        }
    }
}
