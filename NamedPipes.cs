
using System.Diagnostics;
using System.IO.Pipes;
using System.Text;

namespace HoseRenderer
{
    namespace NamedPipes
    {
        /// <summary>
        /// A built out class for named pipes that handles more of the backgrounded needed for both sides
        /// </summary>
        public class PowerGLPipe
        {
            private Stream ioStream;
            private UnicodeEncoding encoding;
            /// <summary>
            /// Creaes 
            /// </summary>
            /// <param name="ioStream"></param>
            public PowerGLPipe (Stream ioStream)
            {
                this.ioStream = ioStream;
                encoding = new UnicodeEncoding();
            }
            /// <summary>
            /// Reads the bytes in the pipe
            /// </summary>
            /// <returns>
            /// The unicode string of the bytes in the pipe
            /// </returns>
            public string ReadString()
            {
                int len;// = 256;
                len = ioStream.ReadByte() * 16;
                len += ioStream.ReadByte();
                Debugger.Log(1, "IPC_NAMED_PIPE", $"BUFFER LEN::{len}::OF IPC_NAMED_PIPE {Environment.NewLine}");
                if (len < 1)
                {
                    return "Pipe Has Nothing To Read";
                }
                var inBuffer = new byte[len];
                ioStream.Read(inBuffer, 0, len);

                //byte[] erase_buff = new byte[len];
                //ioStream.Write(erase_buff, 0, len);

                return encoding.GetString(inBuffer);
            }
            /// <summary>
            /// Writes the string to the pipe
            /// </summary>
            /// <param name="outstr"></param>
            /// <returns>
            /// The length of written bytes + 2 for padding
            /// </returns>
            public int WriteString(string outstr)
            {
                //MS Why in your own docs did you forget Unicode is 2 bytes instead of 1 per character
                byte[] outbuffer = encoding.GetBytes(outstr);
                int len = outstr.Length * 2;
                if (len > UInt16.MaxValue) { 
                    len = (int)UInt16.MaxValue;
                }
                ioStream.WriteByte((byte)(len / 16));
                ioStream.WriteByte((byte)(len & 16));
                ioStream.Write(outbuffer, 0, len);
                ioStream.Flush();
                
                return outbuffer.Length + 2;
            }
            /// <summary>
            /// A wrapper for the IOstream .Close without exposing the io stream itself
            /// </summary>
            public void Detach()
            {
                this.ioStream.Close();
            }

        }
        /// <summary>
        /// The Powershell Side launches the server that the rendering engine connects to, Is bound to local hose only :)
        /// </summary>
        public class NamedPipeServer
        {
            /// <summary>
            /// The Named pipe this enging expects "PowerGL"
            /// </summary>
            public string PipeName { get; private set; }
            /// <summary>
            /// The Directionality of the Pipe, InOut is recommended as thats what the engine expects
            /// </summary>
            public PipeDirection Direction { get; private set; }
            /// <summary>
            /// The underlying Pipe from system.io.pipes as this is the server host
            /// </summary>
            public NamedPipeServerStream Pipe { get; private set; }
            /// <summary>
            /// The underlying IO stream and IO operations on the pipe
            /// </summary>
            public PowerGLPipe StreamString { get; private set; }
            /// <summary>
            /// Creates a new NamedPipe server for use to talk with the rendering engine
            /// </summary>
            /// <param name="pipeName"></param>
            /// <param name="direction"></param>
            public NamedPipeServer(string pipeName, PipeDirection direction)
            {
                PipeName = pipeName;
                Direction = direction;
                Pipe = new NamedPipeServerStream(PipeName, Direction, 1);
                Pipe.WaitForConnection();
                StreamString = new PowerGLPipe(Pipe);
            }
            /// <summary>
            /// A wrapper for the underlying IO stream to write Directives (operations) to the Rendering engine during the .render() call
            /// </summary>
            /// <param name="Message"></param>
            /// <returns></returns>
            public int WriteDirective(string Message)
            {
                return this.StreamString.WriteString(Message);
            }
        }
    }
}
