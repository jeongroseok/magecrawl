using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Magecrawl.Exceptions
{
    [Serializable]
    public class PlayerWonException : Exception
    {
        public PlayerWonException() : base()
        {
        }

        public PlayerWonException(string s) : base(s)
        {
        }

        public PlayerWonException(string s, Exception e) : base(s, e)
        {
        }

        protected PlayerWonException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
