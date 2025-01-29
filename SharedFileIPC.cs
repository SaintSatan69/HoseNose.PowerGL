using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Text.Json;
using HoseRenderer.PowerGl;
using HoseRenderer.NamedPipes;
using System.Security.Principal;
namespace HoseRenderer
{
    public class SharedFileIPC
    {
        private static string shapePath = ".\\IPCFiles\\Shape.POWERGLSHAPE";
        private static Logger logger = new Logger("IPC");
        public static void InitalizeFileIPC()
        {
            Directory.SetCurrentDirectory(@"C:\Github\HoseRenderer\bin\debug\net8.0");
            File.Create(shapePath).Close();
            logger.Log("Application IPC Initialized with no problems");
        }
        public static void UninitalizeFileIPC() 
        {
            try
            {
                Directory.SetCurrentDirectory(@"C:\Github\HoseRenderer\bin\debug\net8.0");
                File.Delete(shapePath);
                logger.Log("Application IPC Uninitilize with no problems");
            }
            catch { }
        }
        public static void WriteFileIPC(Shape[] ShapeObject)
        {
            Directory.SetCurrentDirectory(@"C:\Github\HoseRenderer\bin\debug\net8.0");
            //serialization apparently doesn't like Vector3s Debugging is last thing needed before HoseRenderer.PowerGL.Shapes can contain a method of .Render() ( The Solution, 3 Other
            //Properties that gets glued back into a vect3)
            Stream stream = File.Open(shapePath,FileMode.Append);
            stream.Write(JsonSerializer.SerializeToUtf8Bytes(ShapeObject,JsonSerializerOptions.Default));
            stream.Close();
        }
        public static Shape[] ReadShapeIPC() 
        {
            Directory.SetCurrentDirectory(@"C:\Github\HoseRenderer\bin\debug\net8.0");
            var Jsonbyte = File.ReadAllBytes(shapePath);
            var utfreader = new Utf8JsonReader(Jsonbyte);
            return JsonSerializer.Deserialize<Shape[]>(ref utfreader);
        }
        //actually implement the named pipes you crack head so that powershell can instruct the program on moving objects around
        public static PowerGLPipe AttachToOrchastratorNamedPipe()
        {
            var pipeClient = new NamedPipeClientStream(".","PowerGL",PipeDirection.InOut,PipeOptions.None,TokenImpersonationLevel.Impersonation);
            Console.WriteLine("Attempting to Connect to the powershell process for object movements");
            pipeClient.Connect();
            return new PowerGLPipe(pipeClient);
        }
        public static void DetachFromOrchastratrorNamedPipe(PowerGLPipe Pipe)
        {
            Pipe.Detach();
        }
    }
}
