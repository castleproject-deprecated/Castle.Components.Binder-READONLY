using System;
using System.Collections.Generic;
using System.Text;

namespace Castle.MicroKernel.Exceptions
{
    
    [global::System.Serializable]
    public class CircularDependecyException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public CircularDependecyException() { }
        public CircularDependecyException(string message) : base(message) { }
        public CircularDependecyException(string message, Exception inner) : base(message, inner) { }
        protected CircularDependecyException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
