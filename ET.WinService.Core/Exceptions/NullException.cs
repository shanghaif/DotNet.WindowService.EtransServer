using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ET.WinService.Core.Exceptions
{
    [Serializable]
    public class NullException:ApplicationException
    {
        public NullException()
            : base()
        {
        }

         public NullException(string message)
            : base(message) {
        }

        public NullException(string message, Exception innerException)
            : base(message, innerException) {
        }

        protected NullException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }
    }
}
