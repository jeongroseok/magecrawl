using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Magecrawl.Exceptions
{
    [Serializable]
    public class PlayerDiedException : Exception
    {
        public PlayerDiedException() : base()
        {
        }

        public PlayerDiedException(string s) : base(s)
        {
        }

        public PlayerDiedException(string s, Exception e) : base(s, e)
        {
        }

        protected PlayerDiedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
