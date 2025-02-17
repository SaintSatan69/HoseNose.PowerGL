using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoseRenderer
{
    namespace PowerGl
    {
        /// <summary>
        /// For use with the GUI console, when Enter is pressed send the command to this class to handle the command inside the engine
        /// </summary>
        public class ConsoleCommandHandler
        {
            enum CommandTypes
            {
                MOVE,
                ROTATE,
                SCALE,
                STRETCH,
            }
            /// <summary>
            /// The Function will parse the given command string and execute the given command if its valid
            /// </summary>
            /// <param name="commandstring"></param>
            public static void ExecutePGLCommand(string commandstring)
            {
                
            }
        }
    }
}
