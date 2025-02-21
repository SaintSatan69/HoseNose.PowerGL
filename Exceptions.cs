using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoseRenderer
{
    namespace Exceptions
    {
        /// <summary>
        /// an exception class that is for an illegal object state that sliped through error detection and correction
        /// </summary>
        public class AnomalousObjectException : Exception
        {
#pragma warning disable CS8618
            /// <summary>
            /// if you want to specify what property on an object entered an illegal state
            /// </summary>
            public string ObjectProperty;
            /// <summary>
            /// a messageless exception
            /// </summary>
            public AnomalousObjectException()

            {
            }
            /// <summary>
            /// an exception that has only a message
            /// </summary>
            /// <param name="message"></param>
            public AnomalousObjectException(string message) : base(message) 
            { 
            }
            /// <summary>
            /// an exception that has a message and an inner exception if multiple things are broken
            /// </summary>
            /// <param name="message"></param>
            /// <param name="inner"></param>
            public AnomalousObjectException(string message, Exception inner) : base(message, inner) 
            { 
            }
            /// <summary>
            /// an exception that has a message and the option to specify what object property is causing this exception to be raised
            /// </summary>
            /// <param name="message"></param>
            /// <param name="objectproperty"></param>
            public AnomalousObjectException(string message, string objectproperty) : this(message) {
                ObjectProperty = objectproperty;
            }
        }
    }
}
#pragma warning restore CS8618
