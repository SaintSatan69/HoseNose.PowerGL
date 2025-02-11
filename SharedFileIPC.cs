using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Text.Json;
using HoseRenderer.PowerGl;
using HoseRenderer.NamedPipes;
using System.Security.Principal;
namespace HoseRenderer
{
    /// <summary>
    /// this class handles the file bassed IPC used by powershell to send to the rendering engine so that its easy without named pipes and so that you can make sure an object property is correctly being serialized
    /// </summary>
    public class SharedFileIPC
    {
        private static Logger logger = new Logger("IPC",null);
        /// <summary>
        /// Creates the Shape.POWERGLSHAPE file
        /// </summary>
        /// <param name="IPCFOLDERPATH"></param>
        public static void InitalizeFileIPC(string IPCFOLDERPATH)
        {
            //Directory.SetCurrentDirectory(@"C:\Github\HoseRenderer\bin\debug\net8.0");
            File.Create((IPCFOLDERPATH + @"\Shape.POWERGLSHAPE")).Close();
            logger.Log("Application IPC Initialized with no problems");
        }
        /// <summary>
        /// deletes the Shape.POWERGLSHAPE file
        /// </summary>
        /// <param name="IPCFOLDERPATH"></param>
        public static void UninitalizeFileIPC(string IPCFOLDERPATH) 
        {
            try
            {
                //Directory.SetCurrentDirectory(@"C:\Github\HoseRenderer\bin\debug\net8.0");
                File.Delete(IPCFOLDERPATH + @"\Shapes.POWERGLSHAPE");
                logger.Log("Application IPC Uninitilize with no problems");
            }
            catch { }
        }
        /// <summary>
        /// write the shapes to the IPC file
        /// </summary>
        /// <param name="IPCFOLDERPATH"></param>
        /// <param name="ShapeObject"></param>
        public static void WriteFileIPC(string IPCFOLDERPATH, Shape[] ShapeObject)
        {
            //Directory.SetCurrentDirectory(@"C:\Github\HoseRenderer\bin\debug\net8.0");
            //serialization apparently doesn't like Vector3s Debugging is last thing needed before HoseRenderer.PowerGL.Shapes can contain a method of .Render() ( The Solution, 3 Other
            //Properties that gets glued back into a vect3)
            Stream stream = File.Open(IPCFOLDERPATH + @"\Shape.POWERGLSHAPE",FileMode.Append);
            stream.Write(JsonSerializer.SerializeToUtf8Bytes(ShapeObject,JsonSerializerOptions.Default));
            stream.Close();
        }
        /// <summary>
        /// reads the IPC file from disk and returns an array of shape objects if for what ever reason there is an error processing the file such as it doesn't exist it will throw an exception caugh in the main threads loadobject()
        /// </summary>
        /// <returns>
        /// An array of shape objects deserialzed from the IPC file
        /// </returns>
        /// <exception cref="Exception"></exception>
        public static Shape[] ReadShapeIPC() 
        {
            string shapepath = "";
            if (File.Exists(@$"{MainRenderer.Application_Directory}\IPCFILES\Shape.POWERGLSHAPE"))
            {
                shapepath = @$"{MainRenderer.Application_Directory}\IPCFILES\Shape.POWERGLSHAPE";
            }
            if (File.Exists(@$"C:\users\{Environment.UserName}\appdata\local\temp\PowerGL\IPCFILES\Shape.POWERGLSHAPE"))
            {
                shapepath = @$"C:\users\{Environment.UserName}\appdata\local\temp\PowerGL\IPCFILES\Shape.POWERGLSHAPE";
            }
            //Directory.SetCurrentDirectory(@"C:\Github\HoseRenderer\bin\debug\net8.0");
            //if (shapePath != @$"{MainRenderer.Application_Directory}\IPCFILES\Shape.POWERGLSHAPE")
            //{
            //    shapePath = @$"{MainRenderer.Application_Directory}\IPCFILES\Shape.POWERGLSHAPE";
            //} 
            if (shapepath == "")
            {
                throw new Exception("IPC");
            }
            var Jsonbyte = File.ReadAllBytes(shapepath);
            var utfreader = new Utf8JsonReader(Jsonbyte);
            return JsonSerializer.Deserialize<Shape[]>(ref utfreader);
        }
        /// <summary>
        /// the named pipe on the client (rendering engine) connection tool
        /// </summary>
        /// <returns>
        /// The PowerGLPipe Client attached to . 
        /// </returns>
        public static PowerGLPipe AttachToOrchastratorNamedPipe()
        {
            var pipeClient = new NamedPipeClientStream(".","PowerGL",PipeDirection.InOut,PipeOptions.None,TokenImpersonationLevel.Impersonation);
            Console.WriteLine("Attempting to Connect to the powershell process for object movements");
            pipeClient.Connect();
            return new PowerGLPipe(pipeClient);
        }
        /// <summary>
        /// Yeet the program from the Named pipe server that is the powershell process
        /// </summary>
        /// <param name="Pipe"></param>
        public static void DetachFromOrchastratrorNamedPipe(PowerGLPipe Pipe)
        {
            Pipe.Detach();
        }
    }
}
