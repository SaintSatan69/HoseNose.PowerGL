using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoseRenderer
{
    namespace Exceptions
    {
        public class AnomalousObjectException : Exception
        {
#pragma warning disable CS8618

            public string ObjectProperty;
            public AnomalousObjectException()

            {
            }
            public AnomalousObjectException(string message) : base(message) 
            { 
            }
            public AnomalousObjectException(string message, Exception inner) : base(message, inner) 
            { 
            }
            public AnomalousObjectException(string message, string objectproperty) : this(message) {
                ObjectProperty = objectproperty;
            }
        }
    }
}
#pragma warning restore CS8618
