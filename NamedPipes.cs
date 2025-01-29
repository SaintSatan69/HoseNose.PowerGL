
using System.Diagnostics;
using System.IO.Pipes;
using System.Text;

namespace HoseRenderer
{
    namespace NamedPipes
    {
        public class PowerGLPipe
        {
            private Stream ioStream;
            private UnicodeEncoding encoding;

            public PowerGLPipe (Stream ioStream)
            {
                this.ioStream = ioStream;
                encoding = new UnicodeEncoding();
            }
            public string ReadString()
            {
                int len;// = 256;
                len = ioStream.ReadByte() * 16;
                len += ioStream.ReadByte();
                Debugger.Log(1, "IPC_NAMED_PIPE", $"BUFFER LEN::{len}::OF IPC_NAMED_PIPE {Environment.NewLine}");
                var inBuffer = new byte[len];
                ioStream.Read(inBuffer, 0, len);

                //byte[] erase_buff = new byte[len];
                //ioStream.Write(erase_buff, 0, len);

                return encoding.GetString(inBuffer);
            }
            public int WriteString(string outstr)
            {
                //TODO: FIX THIS SINCE THIS IS PROBLEM WITH THE READER
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
            public void Detach()
            {
                this.ioStream.Close();
            }

        }
        public class NamedPipeServer
        {
            public string PipeName { get; private set; }
            public PipeDirection Direction { get; private set; }
            public NamedPipeServerStream Pipe { get; private set; }
            public PowerGLPipe StreamString { get; private set; }
            public NamedPipeServer(string pipeName, PipeDirection direction)
            {
                PipeName = pipeName;
                Direction = direction;
                Pipe = new NamedPipeServerStream(PipeName, Direction, 1);
                Pipe.WaitForConnection();
                StreamString = new PowerGLPipe(Pipe);
            }
            public int WriteDirective(string Message)
            {
                return this.StreamString.WriteString(Message);
            }
        }
    }
}
