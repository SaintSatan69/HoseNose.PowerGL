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
        private static Logger logger = new Logger("IPC",null);
        public static void InitalizeFileIPC(string IPCFOLDERPATH)
        {
            //Directory.SetCurrentDirectory(@"C:\Github\HoseRenderer\bin\debug\net8.0");
            File.Create((IPCFOLDERPATH + @"\Shape.POWERGLSHAPE")).Close();
            logger.Log("Application IPC Initialized with no problems");
        }
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
        public static void WriteFileIPC(string IPCFOLDERPATH, Shape[] ShapeObject)
        {
            //Directory.SetCurrentDirectory(@"C:\Github\HoseRenderer\bin\debug\net8.0");
            //serialization apparently doesn't like Vector3s Debugging is last thing needed before HoseRenderer.PowerGL.Shapes can contain a method of .Render() ( The Solution, 3 Other
            //Properties that gets glued back into a vect3)
            Stream stream = File.Open(IPCFOLDERPATH + @"\Shape.POWERGLSHAPE",FileMode.Append);
            stream.Write(JsonSerializer.SerializeToUtf8Bytes(ShapeObject,JsonSerializerOptions.Default));
            stream.Close();
        }
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
