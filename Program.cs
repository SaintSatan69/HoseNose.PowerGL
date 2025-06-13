using Silk.NET.Input;
using Silk.NET.Windowing;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using System.Numerics;
using HoseRenderer.PowerGl;
using HoseRenderer.NamedPipes;
using HoseRenderer.ExtraUtils;
using System.Diagnostics;
using System.Drawing;
using ImGuiNET;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Text.Json;
using System.Net.WebSockets;
using System.Threading.Tasks;


namespace HoseRenderer
{
    /// <summary>
    /// This is the cursed C# powered PowerShell OpenGL (+Maybe Vulkan) rendering engine because making something practical is no fun anyways
    /// </summary>
    public class MainRenderer
    {
#pragma warning disable CS8618 

        //Engine specific data used by thing checking engine versioning
        /// <summary>
        /// The Static sting of the engine verion
        /// </summary>
        public static readonly string EngineVersion = "0.0.4.0";

        /// <summary>
        /// The Engine Config Object Represents the way the developer intended to use the avaiable editable attributes of the engine
        /// </summary>
        public static EngineConfiguration Config;

        private static EngineConfiguration Previous_config;

        //OpenGL Things
        private static IWindow window;

        private static GL Gl;
        private static IKeyboard primaryKeyboard;
        private static IMouse mouse;

        private static VertexArrayObject<float, uint> VaoCube;
        private static BufferObject<float> Vbo;
        private static BufferObject<uint> Ebo;


        private static Camera Camera;

        private static Vector2 LastMousePosition;

        private static DateTime StartTime;

        private static PowerGLPipe Pipe;

        private static string[] FLAGS = new string[10];


        private static Dictionary<string, Texture> _Name_to_texture_dict = new();
        private static Dictionary<string, Model> _Compiled_Models = new();

        private static int _Frame_counter = 0;

        private static Shape[] Shapes = [];
        /// <summary>
        /// A int that states how many shapes are loaded into the engine at a given time.
        /// </summary>
        public static int GlobalShapeCount = 0;

        private static uint _player_1_controllerObject;
        private static uint _player_2_controllerObject;
        private static float _player_speed = 0.01f;

        /// <summary>
        /// A Global Logger for the main engine can be used by other classes if the log pertains to the functionality of the main engine (Do not have seperate threads touch this)
        /// </summary>
        public static readonly Logger EngineLogger = new("RenderEngine", null, Environment.CurrentManagedThreadId);

        /// <summary>
        /// Used By Other Objects the Static Directory of the powershell 7-preview Modules folder so that all objects that need to reference files inside the engine know where it is on the filesystem
        /// </summary>
        public static readonly string Application_Directory = @"C:\Program Files\PowerShell\7-preview\Modules\PowerGL";
        /// <summary>
        /// The Full path to the current users temp folder for use with the file IPC and logging
        /// </summary>
        public static readonly string UserDir = @$"C:\users\{Environment.UserName}\appdata\local\temp";
        //threading things
        private static Thread PipeThread;
        private static string _Pipe_THREAD_DATA = "";

        private static Thread FPS_Thread;
        private static uint FPS;

        //GUI things
        private static ImGuiController guiController;
        private static int _Gui_Frame_state = 0;
        private static int _Last_frametime_GUI_change = 0;
        private static bool HIDE_SHAPES_FOR_GUI = false;
        /// <summary>
        /// A Signal handler to tell objects to not render when the GUI is Active since the GUI was being rendered correctly but because of the sky box cloud it was hidden
        /// </summary>
        public static bool IsGUICalled { get => HIDE_SHAPES_FOR_GUI; }
        private static string CMD_PROC = "";
        private static uint PROC_LEN = 255;
        private static int _char_frametime = 0;
        private static string GUI_CNS_TEXT = "";

        //HttpAPI Things
        private static bool IsEngineAwaitingProcessRequest = false;

        private static int WebSocketBufferLength = 1024;


#pragma warning restore CS8618 
        /// <summary>
        /// thanks VS I totally need a summary of the Main function Who doesn't know what this does and programs C# ???????
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += HandleEngineCrash;
            string argstring = "";
            for (int a = 0; a < args.Length; a++)
            {
                argstring += ("-" + args[a]);
            }
            EngineLogger.Log($"Engine Initalization Started args {argstring}");
            FLAGS[3] = "PHYSICS";
            for (int arg = 0; arg < args.Length; arg++)
            {
                if (args[arg].ToUpper() == "PIPE_ENABLE")
                {
                    FLAGS[0] = "IPC_NAMED_PIPE_ENABLE";
                    Console.WriteLine("FLAG IPC_NAMED_PIPE_ENABLE has been enabled");
                }
                if (args[arg].ToUpper() == "DEBUG")
                {
                    FLAGS[2] = "DEBUG";
                }
                if (args[arg].ToUpper() == "PHYSICS_DISABLE")
                {
                    FLAGS[3] = "";
                }
                if (args[arg].ToUpper() == "DEV_BUGLAND")
                {
                    FLAGS[0] = "IPC_NAMED_PIPE_ENABLE";
                    FLAGS[2] = "DEBUG";
                    FLAGS[3] = "PHYSICS";
                    Console.WriteLine("AWAITING DEBUGGER");
                    while (!Debugger.IsAttached)
                    {
                        Thread.Sleep(100);
                    }
                }
                if (args[arg].ToUpper() == "CONFIGFILE")
                {
                    try
                    {
                        Config = EngineConfiguration.ReadEngineConfig(args[arg + 1]);
                        FLAGS[4] = "Engine_Config_Read";
                    }
                    catch (Exception ex)
                    {
                        EngineLogger.Log($"//WARNING// Failed to read Engine config from {args[arg + 1]} for reason {ex.Message}");
                    }
                }
            }
            if (FLAGS[4] != "Engine_Config_Read")
            {
                //technically i could then use this data to make the engine_location property but I would rather make sure its defined in the config that this will attempt to discover incase the dev desides to make the engines default assets not live next to the engine
                var files = Directory.EnumerateFiles(
                    AppContext.BaseDirectory,
                    "*.EngineConfig"
                );
                if (files == null || files.Count() == 0)
                {

                    var user_profile_Search = Directory.EnumerateFiles(
                        UserDir + @"\PowerGL",
                        "*.EngineConfig",
                        SearchOption.AllDirectories
                    );
                    if (user_profile_Search != null)
                    {
                        string config_file = user_profile_Search.First();
                        EngineLogger.Log($"Engine config file(s) found total count {user_profile_Search.Count()}: Loaded config {config_file}");
                        Config = EngineConfiguration.ReadEngineConfig(config_file);
                    }
                    else
                    {
                        EngineLogger.Log("No Engine Config Provided or Read Successfully generating now");
                        Config = EngineConfiguration.ReadEngineConfig("");
                        EngineLogger.Log(@$"Generated new engine config at {UserDir}\PowerGL\PowerGL.EngineConfig");
                    }
                }
                else
                {
                    string config_file = files.First();
                    EngineLogger.Log($"Engine config file(s) found total count {files.Count()}: Loaded config {config_file}");
                    Config = EngineConfiguration.ReadEngineConfig(config_file);
                }
                Previous_config = Config;
                FLAGS[4] = "Engine_Config_Read";
            }

            //I really need to clean this up
            //if (args.Length > 0) {
            //    if (args[0] == "pipe_enable") {
            //        FLAGS[0] = "IPC_NAMED_PIPE_ENABLE";
            //        Console.WriteLine("FLAG IPC_NAMED_PIPE_ENABLE has been enabled");
            //    }
            //    if (args[0] != "render_extracube")
            //    {
            //        FLAGS[1] = "";
            //    }
            //    else
            //    {
            //        FLAGS[1] = "RENDER_DEFAULT_CUBES_REGARDLESS";
            //    }
            //    if (args[0] == "debug_msg_enable")
            //    {
            //        FLAGS[2] = "DEBUG";

            //    }
            //    if (args[0] == "DEV_BUGLAND")
            //    {
            //        FLAGS[0] = "IPC_NAMED_PIPE_ENABLE";
            //        FLAGS[2] = "DEBUG";
            //        FLAGS[3] = "PHYSICS";
            //        //FLAGS[4] = "MULTITHREADED_RENDERER";
            //        Console.WriteLine("AWAITING DEBUGGER");
            //        while (!Debugger.IsAttached)
            //        {
            //            Thread.Sleep(100);
            //        }
            //    }
            //}
            //make sure to comment this when not debugging the cubes
            //FLAGS[0] = "IPC_NAMED_PIPE_ENABLE";
            //FLAGS[1] = "RENDER_DEFAULT_CUBES_REGARDLESS";
            //FLAGS[2] = "DEBUG";
            //end of Debugging flags to tinker from the source code

            PrintFunnyLogo();

            WindowOptions options = WindowOptions.Default;
            if (FLAGS[4] != "Engine_Config_Read")
            {
                // FALL BACK OPTIONS IN THE EVENT A CONFIG LOADING ERROR HAPPENS
                options.Size = new Vector2D<int>(1920, 1080);
                options.Title = "HoseNose.PowerGL";
                options.Position = new Vector2D<int>(0, 30);
                options.VSync = true;
            }
            else
            {
                try
                {
                    options.Size = new Vector2D<int>(Config.WindowX, Config.WindowY);
                    options.Title = Config.WindowTitle;
                    options.Position = new Vector2D<int>(0, 30);
                    options.VSync = Config.IsVSyncEnabled;
                }
                catch (Exception ex)
                {
                    EngineLogger.Log($"//CRITICAL FAILURE// CONFIGURATION IS INVALID EXCEPTION MESSAGE: {ex.Message}{Environment.NewLine}");
                }
            }
            //DO NOT TURN VSYNC OFF WITHOUT LOGIC TO WARN THE USER THAT THE ENGINES PHYSICS MIGHT BE A LITTLE WONKY IF ITS DISABLED SINCE A LOT OF PHYSICS ARE CALCULATED ON A FRAME
            //options.VSync = false;
            Console.WriteLine("HoseNose.PowerGL has been loaded");
            Console.WriteLine($"{options.API.API.ToString()}{options.Size}");
            var Platforms = Window.Platforms.Count;
            Console.WriteLine($"Num of platforms: {Platforms}");
            window = Window.Create(options);
            //why must techonology not like powershell :( (we are cheating by having powershell just do the control of shapes engine, the shape properties and when this program is launched leaving
            //this to do the rendering calls to OpenGL through Silk.Net)
            Console.WriteLine($"{window.API.API} {window.WindowState}");
            if (FLAGS[0] == "IPC_NAMED_PIPE_ENABLE")
            {
                // We sleep here to give powershell enough time after the program starts to create the named pipe server it could be faster, but im not tempting fate by getting close and closer to a thread lock
                Thread.Sleep(2000);
                Pipe = SharedFileIPC.AttachToOrchastratorNamedPipe();
                PipeThread = new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    while (true)
                    {
                        //May not be thread safe but fuck it we ball, its just to tell the Pipe client to chill out while the GUI is active
                        while (IsGUICalled)
                        {
                            Thread.Sleep(10);
                        }
                        _Pipe_THREAD_DATA = Pipe.ReadString();
                    }
                });
                PipeThread.Name = "PS_IPC_THREAD";
                PipeThread.Start();
            }
            //In case you'd like to do more actions then what the named pipe can do / want to retrive data out of the engine the HTTP API is the best bet
            if (Config.IsHTTPAPIEnabled)
            {
                string[] ValidMethods;
                int Port = Config.HTTPPort;
                if (Config.IsHTTPReadOnly)
                {
                    ValidMethods = new string[] { "GET" };
                }
                else
                {
                    ValidMethods = new string[] { "GET", "POST", "PATCH" };
                }
                var netinfo = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();
                var IPPortBlob = netinfo.GetActiveTcpListeners();
                foreach (IPEndPoint point in IPPortBlob)
                {
                    if (point.Port == Port)
                    {
                        Console.WriteLine($"HTTP Port:{Port} Is already in use using port +1 maybe");
                        Port++;
                    }
                }
                HttpListener APISERVER = new();
                APISERVER.Prefixes.Add($"http://localhost:{Port}/api/shapes/");
                APISERVER.Start();
                Logger HttpAPILogger = new Logger("HTTPAPI", null);
                Thread HTTPThread = new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    Thread.CurrentThread.Name = "HTTP_API";

                    while (APISERVER.IsListening)
                    {
                        var Client = APISERVER.GetContext();
                        if (Client != null)
                        {
                            if (Client.Request.IsWebSocketRequest)
                            {
                                HttpAPILogger.Log("WebSocket Client Being Handled");
                                try
                                {
                                    ProcessWebSocket(Client, HttpAPILogger);
                                }
                                catch(Exception ex)
                                {
                                    Debugger.Log(1,"",$"Exception during the Websocket handling {ex.Message}{Environment.NewLine}");
                                    HttpAPILogger.Log($"During normal handling of the websocket the following exception occured {ex.Message}{Environment.NewLine} For Bug Reports the stack trace:{ex.StackTrace}");
                                }
                            }
                            else
                            {
                                //Stuff that is unique per client
                                var Request = Client.Request;
                                var Response = Client.Response;
                                Stream Output = Response.OutputStream;

                                //Check the user agent to log incase the user agent isn't powershell which isn't supported but it still will respond
                                if (Request.UserAgent.Contains("PowerShell"))
                                {
                                    Debugger.Log(1, "", $"PowerShell Request detected YIPPE{Environment.NewLine}");
                                }
                                else
                                {
                                    HttpAPILogger.Log($"Non Powershell user agent made the request user agent is {Request.UserAgent}, If this is a web browser it may not behave correctly");
                                }
                                HttpObject? requestData = null;
                                try
                                {
                                    requestData = JsonSerializer.Deserialize<HttpObject>(Request.InputStream);
                                }
                                catch
                                {
                                    requestData = null;
                                }
                                if (requestData == null && Request.HttpMethod == HttpMethod.Post.ToString())
                                {
                                    HttpAPILogger.Log("Incomplete Request Sent");
                                    Response.StatusCode = (int)HttpStatusCode.PartialContent;
                                    var incompletedatamsgbyte = Encoding.UTF8.GetBytes("Incomplete Data to Complete Request");
                                    Response.ContentLength64 = incompletedatamsgbyte.Length;
                                    Output.Write(incompletedatamsgbyte, 0, incompletedatamsgbyte.Length);
                                    Response.Close();
                                }
                                else if (Request.HttpMethod == HttpMethod.Get.ToString())
                                {
                                    if (requestData != null)
                                    {
                                        Shape RequestedShape = Shapes[requestData.ShapeNumber];
                                        HttpObject RequestedShapeResponse = requestData.Property switch
                                        {
                                            "Position" => new HttpObject(RequestedShape.ShapeNum, "Position", RequestedShape.PosX, RequestedShape.PosY, RequestedShape.PosZ),
                                            "Size" => new HttpObject(RequestedShape.ShapeNum, "Size", RequestedShape.Size, 0, 0),
                                            "Stretch" => new HttpObject(RequestedShape.ShapeNum, "Stretch", RequestedShape.StrX, RequestedShape.StrY, RequestedShape.StrZ),
                                            "Shear" => new HttpObject(RequestedShape.ShapeNum, "Shear", RequestedShape.ShrX, RequestedShape.ShrY, RequestedShape.ShrZ),
                                            "Restitution" => new HttpObject(RequestedShape.ShapeNum, "Restitution", RequestedShape.Restitution, 0, 0),
                                            "Momentum" => new HttpObject(RequestedShape.ShapeNum, "Momentum", RequestedShape.MomentumX, RequestedShape.MomentumY, RequestedShape.MomentumZ),
                                            "Rotation" => new HttpObject(RequestedShape.ShapeNum, "Rotation", RequestedShape.RotX, RequestedShape.RotY, RequestedShape.RotZ),
                                            _ => new HttpObject(0, "NONE OR INVALID PROPERTY", 0, 0, 0)
                                        };
                                        Response.StatusCode = 200;
                                        Output.Write(Encoding.UTF8.GetBytes(JsonSerializer.Serialize<HttpObject>(RequestedShapeResponse)));
                                        Response.Close();
                                    }
                                    else
                                    {
                                        //StringBuilder JSON_BUILDER = new StringBuilder();
                                        //JSON_BUILDER.Append('[');
                                        //Might thread lock if its trying to render while also trying to serialize it, might need to add a deep clone and synchronization into it and store a copy
                                        ShapeHttpObjectCollectionEntry[] shapeHttpObjectCollectionEntries = new ShapeHttpObjectCollectionEntry[Shapes.Length];
                                        int Indexer = 0;
                                        foreach (Shape shape in Shapes)
                                        {
                                            shapeHttpObjectCollectionEntries[Indexer] = new ShapeHttpObjectCollectionEntry(shape);
                                            Indexer++;
                                        }
                                        Output.Write(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(shapeHttpObjectCollectionEntries)));
                                        Response.StatusCode = 200;
                                        Response.Close();
                                    }
                                }
                                else if (Request.HttpMethod == HttpMethod.Post.ToString() && requestData != null)
                                {
                                    float? scale = 0;
                                    if (requestData.Property.ToLower() == "scale")
                                    {
                                        scale = requestData.ValueX;
                                    }
                                    else
                                    {
                                        scale = null;
                                    }
                                    int modifyStatus = ModifyShapeProperty((int)requestData.ShapeNumber, requestData.Property, requestData.ValueX, requestData.ValueY, requestData.ValueZ, null, null, null, scale);
                                    if (modifyStatus == 0)
                                    {
                                        Response.StatusCode = 200;
                                        Response.Close();
                                    }
                                    else if (modifyStatus == -1)
                                    {
                                        Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                        Output.Write(Encoding.UTF8.GetBytes($"Failed to apply changes to {requestData.ShapeNumber} as no valid property was presented to the engine"));
                                        Response.Close();
                                    }
                                    else if (modifyStatus == -2)
                                    {
                                        Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                        Output.Write(Encoding.UTF8.GetBytes($"Failed to apply changes to {requestData.ShapeNumber}-{requestData.Property} Please look at the log at {HttpAPILogger.Log_Directory}"));
                                        Response.Close();
                                    }
                                }
                                else
                                {
                                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                                    string invalidhttpmethodbody = "Invalid HTTP method sent to server";
                                }
                            }
                        }
                    }
                });
                HTTPThread.Start();
            }





            window.Load += OnLoad;
            window.Render += OnRender;
            window.FramebufferResize += OnFramebufferResize;
            window.Update += OnUpdate;
            window.Closing += OnClose;
            window.Run();
            window.Dispose();
        }
        private static void LoadObjects()
        {
            try
            {
                Shapes = SharedFileIPC.ReadShapeIPC();
                Console.WriteLine($"SUCCESS MAYBE OBJECT [0] is {Shapes[0].ShapeName}");
                EngineLogger.Log($"Loaded Objects successfully shape count is {Shapes.Length}");
            }
            catch (Exception ex)
            {
                EngineLogger.Log($"Load Objects Failed Exception:{ex.Message}");
                Console.WriteLine("IPC never initialized loading default scene");
            }
        }
        private static unsafe void OnLoad()
        {
            StartTime = DateTime.Now;
            IInputContext input = window.CreateInput();
            primaryKeyboard = input.Keyboards.FirstOrDefault();
            if (primaryKeyboard != null)
            {
                primaryKeyboard.KeyDown += KeyDown;
            }
            for (int i = 0; i < input.Mice.Count; i++)
            {
                input.Mice[i].Cursor.CursorMode = CursorMode.Raw;
                input.Mice[i].MouseMove += OnMouseMove;
                input.Mice[i].Scroll += OnMouseWheel;
            }
            if (input.Mice.Count == 1)
            {
                mouse = input.Mice[0];
            }
            MainRenderer.EngineLogger.Log($"Mouse and keyboards enumerated there are {input.Keyboards.Count} keyboards and {input.Mice.Count} mice");
            Gl = GL.GetApi(window);
            LoadObjects();
            Console.WriteLine($"Shapes array from LOAD_OBJECTS:{Shapes.Length}");
            guiController = new(Gl, window, input);
            var size = window.FramebufferSize;
            Camera = new Camera(new Vector3(-6.0f, 0.0f, 6.0f), Vector3.UnitX, Vector3.UnitY, (float)size.X / size.Y);
            if (Shapes.Length != 0)
            {
                //shapevertfloats = new();
                for (int i = 0; i < Shapes.Length; i++)
                {
                    Shapes[i].Glcontext = Gl;
                    Shapes[i].Camera = Camera;
                    Shapes[i].CompileShader();
                    if (Shapes[i].TexturePath != "" && Shapes[i].TexturePath != null)
                    {
                        string _texturepath = Shapes[i].TexturePath;
                        if (_Name_to_texture_dict.ContainsKey(Shapes[i].TexturePath))
                        {
                            Shapes[i].CompiledTexture = _Name_to_texture_dict[Shapes[i].TexturePath];
                        }
                        else
                        {
                            Shapes[i].CompileTexture();
                            var _compiled_texture_path = Shapes[i].TexturePath;
                            var _compiled_texture_texture = Shapes[i].CompiledTexture;
                            _Name_to_texture_dict.Add(_compiled_texture_path, _compiled_texture_texture);
                        }
                    }
                    if (Shapes[i].IsModel)
                    {
                        //TODO Add List of compiled modles and like textures attempt to reuse already made models and in the future do the same for shaders
                        if (_Compiled_Models.ContainsKey(Shapes[i].ModelPath))
                        {
                            Shapes[i].Model = _Compiled_Models[Shapes[i].ModelPath];
                        }
                        else
                        {
                            Shapes[i].CompileModelMesh();
                            _Compiled_Models.Add(Shapes[i].ModelPath, Shapes[i].Model);
                        }
                    }
                    if (Shapes[i].Player_moveable == 1 && Shapes[i].Player_scheme == 1)
                    {
                        _player_1_controllerObject = (uint)i;
                    }
                    if (Shapes[i].Player_moveable == 1 && Shapes[i].Player_scheme == 2)
                    {
                        _player_2_controllerObject = (uint)i;
                    }
                    Shapes[i].GlueShape();
                    if (FLAGS[2] == "DEBUG")
                    {
                        Shapes[i].Debug = 1;
                    }
                }
            }
            if (FLAGS[2] == "DEBUG")
            {
                foreach (var _texture_name_mapper in _Name_to_texture_dict)
                {
                    Console.WriteLine(_texture_name_mapper.Key);
                }
            }

            FPS_Thread = new Thread(() =>
            {

                while (true)
                {
                    Thread.CurrentThread.Name = "FPS_THREAD";
                    Thread.CurrentThread.IsBackground = true;
                    uint _frame_counter_start = (uint)_Frame_counter;
                    Thread.Sleep(1000);
                    uint _frame_counter_end = (uint)_Frame_counter;
                    FPS = _frame_counter_end - _frame_counter_start;
                    Debugger.Log(1, "THREADING", $"FPS::{FPS}{Environment.NewLine}");
                }
            });
            FPS_Thread.Start();


            GlobalShapeCount = Shapes.Length;

        }

        private static unsafe void OnRender(double dt)
        {
            guiController.Update((float)dt);
            _Frame_counter++;
            Gl.Enable(EnableCap.DepthTest);
            if (IsGUICalled)
            {
                Gl.ClearColor(Color.FromArgb(255, (int)(.45f * 255), (int)(.55f * 255), (int)(.60f * 255)));
            }
            else
            {
                Gl.ClearColor(Color.FromArgb(255, (int)(.01f * 255), (int)(.01f * 255), (int)(.01f * 255)));
            }
            Gl.Clear((uint)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
            if (FLAGS[3] == "PHYSICS")
            {
                if (!IsGUICalled)
                {
                    ComputeWhetherOrNotThereIsAShapeInsideAShapeThatSupportsCollions(Shapes);
                    for (int i = 0; i < Shapes.Length; i++)
                    {
                        if (Shapes[i].IsEffectedByGravity)
                        {
                            ApplyGravity(Shapes[i], dt);
                        }
                    }
                }
            }
            //guiController.Update((float)dt);
            if (_Last_frametime_GUI_change == _Frame_counter)
            {
                Debugger.Log(4, "INTERNAL_GUI", $"GUI_TYPE{_Gui_Frame_state} {Environment.NewLine}");
            }
            if (_Gui_Frame_state == 1)
            {
                mouse.Cursor.CursorMode = CursorMode.Normal;
                try
                {
                    BuildPowerGLGUI();
                    guiController.Render();
                }
                catch { }
            }
            else
            {
                mouse.Cursor.CursorMode = CursorMode.Raw;
            }



            if (FLAGS[0] == "IPC_NAMED_PIPE_ENABLE")
            {
                if (_Pipe_THREAD_DATA == "DEBUG:PIPETALKED")
                {
                    Debugger.Log(1, "IPC", $"PIPE COMMUNION SUCCESSFUL{Environment.NewLine}");
                    _Pipe_THREAD_DATA = "";
                }
                string Pipecontents = _Pipe_THREAD_DATA;
                //var Pipecontents = Pipe.ReadString();
                if (Pipecontents.Length > 1)
                {
                    try
                    {
                        string DISPATCH_TARGET = Pipecontents.Split(",")[0];
                        int DISPATCH_TARGETNUMBER = Int32.Parse(DISPATCH_TARGET);
                        try
                        {
                            Shapes[DISPATCH_TARGETNUMBER].AwaitingDispatchFromNamedPipe = 1;
                            Shapes[DISPATCH_TARGETNUMBER].DISPATCH_TARGET_STRING = Pipecontents;
                        }
                        catch
                        {
                            throw new Exception($"DISPATCH_TARGETNUMBER {DISPATCH_TARGETNUMBER} IS OVER INDEXING THE SHAPES ARRAY THATS BAD GOODBYE");
                        }
                    }
                    catch (Exception e)
                    {
                        DebugMessages.PrintDebugMSG($"Error read pipe contents with exception {e.StackTrace}");
                        Debugger.Log(1, "IPC_PIPE", "Chances are you just sent a string that doesn't contain a number and angered the .parse");
                    }
                    Pipecontents = "";
                    _Pipe_THREAD_DATA = "";
                }

            }
            for (int i = 0; i < Shapes.Length; i++)
            {
                Shapes[i].Render();
            }
        }
        private static void OnFramebufferResize(Vector2D<int> newSize)
        {
            Gl.Viewport(newSize);
            Camera.AspectRatio = (float)newSize.X / newSize.Y;
        }
        private static void OnUpdate(double dt)
        {
            var moveSpeed = 2.5f * (float)dt;
            var sprintspeed = 20.0f * (float)dt;
            if (primaryKeyboard.IsKeyPressed(Key.W))
            {
                if (primaryKeyboard.IsKeyPressed(Key.ShiftLeft))
                {
                    //WHY DOES MAKING THE SPEED HIGHER MAKE IT MOVE SLOWER ????????? AND WHY DOES DIVIDING IT MAKE IT FASTER???????
                    Camera.Position += moveSpeed / sprintspeed * Camera.Front;
                }
                else
                {
                    Camera.Position += moveSpeed * Camera.Front;
                }
            }
            if (primaryKeyboard.IsKeyPressed(Key.S))
            {
                Camera.Position -= moveSpeed * Camera.Front;
            }
            if (primaryKeyboard.IsKeyPressed(Key.A))
            {
                Camera.Position -= Vector3.Normalize(Vector3.Cross(Camera.Front, Camera.Up)) * moveSpeed;
            }
            if (primaryKeyboard.IsKeyPressed(Key.D))
            {
                Camera.Position += Vector3.Normalize(Vector3.Cross(Camera.Front, Camera.Up)) * moveSpeed;
            }
            //small bug in the camera, I gotta fix the camera getting wonky after moving the camera a little bit where these two buttons are get out of alligment or get inverted
            if (primaryKeyboard.IsKeyPressed(Key.Space))
            {
                Camera.Position -= Vector3.Normalize(Vector3.Cross(Camera.Front, Vector3.UnitX)) * moveSpeed;
            }
            if (primaryKeyboard.IsKeyPressed(Key.C))
            {
                Camera.Position += Vector3.Normalize(Vector3.Cross(Camera.Front, Vector3.UnitX)) * moveSpeed;
            }
            if (primaryKeyboard.IsKeyPressed(Key.GraveAccent))
            {
                if (_Frame_counter >= _Last_frametime_GUI_change + 30)
                {

                    if (_Gui_Frame_state < 1)
                    {
                        HIDE_SHAPES_FOR_GUI = true;
                        _Gui_Frame_state++;
                        _Last_frametime_GUI_change = _Frame_counter;
                    }
                    else
                    {
                        HIDE_SHAPES_FOR_GUI = false;
                        _Gui_Frame_state = 0;
                        _Last_frametime_GUI_change = _Frame_counter;
                    }
                }
                Thread.Sleep(10);
            }
            if (primaryKeyboard.IsKeyPressed(Key.T))
            {
                Shapes[_player_1_controllerObject].PosX += _player_speed;
            }
            if (primaryKeyboard.IsKeyPressed(Key.G))
            {
                Shapes[_player_1_controllerObject].PosX -= _player_speed;
            }
            if (primaryKeyboard.IsKeyPressed(Key.F))
            {
                Shapes[_player_1_controllerObject].PosZ -= _player_speed;
            }
            if (primaryKeyboard.IsKeyPressed(Key.H))
            {
                Shapes[_player_1_controllerObject].PosZ += _player_speed;
            }
            if (primaryKeyboard.IsKeyPressed(Key.R))
            {
                Shapes[_player_1_controllerObject].PosY += _player_speed;
            }
            if (primaryKeyboard.IsKeyPressed(Key.Y))
            {
                Shapes[_player_1_controllerObject].PosY -= _player_speed;
            }
        }
        private static unsafe void OnMouseMove(IMouse mouse, Vector2 postition)
        {

            var lookSensitivity = 0.1f;
            if (LastMousePosition == default) { LastMousePosition = postition; }
            else
            {
                var xMouseOffset = (postition.X - LastMousePosition.X) * lookSensitivity;
                var yMouseOffset = (postition.Y - LastMousePosition.Y) * lookSensitivity;
                LastMousePosition = postition;
                Camera.ModifyDirection(xMouseOffset, yMouseOffset);
            }
        }
        private static unsafe void OnMouseWheel(IMouse mouse, ScrollWheel scrollWheel)
        {
            if (!IsGUICalled)
            {
                Camera.ModifyZoom(scrollWheel.Y);
            }
        }
        private static void OnClose()
        {
            EngineLogger.Log("Engine Closing because ESCAPE was pressed BYE BYE");
        }
        private static void KeyDown(IKeyboard arg1, Key arg2, int arg3)
        {
            if (arg2 == Key.Escape && !IsGUICalled)
            {
                window.Close();
            }
        }
        private static void ComputeWhetherOrNotThereIsAShapeInsideAShapeThatSupportsCollions(Shape[] Shapes)
        {
            for (int i = 0; i < Shapes.Length; i++)
            {
                for (int j = 0; j < Shapes.Length; j++)
                {
                    if (i != j && Shapes[i].HasCollision == 1 && Shapes[j].HasCollision == 1)
                    {
                        //Console.WriteLine($"MRT1:{Matrises[i].Translation}::MRT2{Matrises[j].Translation}");
                        //attempt 3 works, had a little help from copilot since i wasn't finding any sort of ideas on google it uses AABB (axis-alligned Bounding Boxes)
                        if (Shapes[i].BoundingBox.Intersects(Shapes[j].BoundingBox) && _Frame_counter > 10)
                        {
                            Debugger.Log(1, "", $"{_Frame_counter}::Shape I:{Shapes[i].ShapeNum} collides with Shape:{Shapes[j].ShapeNum} {Environment.NewLine}");
                            if (Shapes[i].BoundingBox.IntersectsX(Shapes[j].BoundingBox))
                            {
                                InitiateBoing(Shapes[i], "X");
                                InitiateBoing(Shapes[j], "X");
                            }
                            if (Shapes[i].BoundingBox.IntersectsY(Shapes[j].BoundingBox))
                            {
                                InitiateBoing(Shapes[i], "Y");
                                InitiateBoing(Shapes[j], "Y");
                            }
                            if (Shapes[i].BoundingBox.IntersectsZ(Shapes[j].BoundingBox))
                            {
                                InitiateBoing(Shapes[i], "Z");
                                InitiateBoing(Shapes[j], "Z");
                            }

                        }
                        else
                        {
                            Debugger.Log(2, "", $"{_Frame_counter}::Shape I:{Shapes[i].ShapeNum} doesn't collide with Shape{Shapes[j].ShapeNum} {Environment.NewLine}");
                        }
                    }
                }
            }
        }
        private static void InitiateBoing(Shape Shape, string axis)
        {
            if (Shape.MomentumY <= 0.002f && Shape.MomentumY >= -0.002f && Shape.IsEffectedByGravity)
            {
                if (Shape.ClampedToFloor == 0 && Shape.MomentumY <= 0 && Shape.MomentumY >= -0.001f)
                {
                    Shape.MomentumY = 0.0f;
                    Shape.PosY = (float)(Math.Round(Shape.PosY) + (Shape.Size * 0.25) + 0.003f);
                    Shape.ClampedToFloor = 1;
                }
            }
            var _frame_count_last_boing = Shape.FrameTimeOfLastBoing;
            if (_Frame_counter >= _frame_count_last_boing + 5 || _Frame_counter == _frame_count_last_boing)
            {
                if (Shape.Player_moveable != 1)
                {
                    var _Old_momentum = Shape.Momentum;
                    Vector3 _Computed_new_momentum;
                    if (Shape.ClampedToFloor == 0)
                    {
                        _Computed_new_momentum = (Shape.Momentum * Shape.Restitution) * -1;
                    }
                    else
                    {
                        _Computed_new_momentum = new Vector3(0f, 0f, 0f);
                    }
                    switch (axis)
                    {
                        case "X":
                            Shape.MomentumX = _Computed_new_momentum.X * (float)0.5;
                            break;
                        case "Y":
                            Shape.MomentumY = _Computed_new_momentum.Y;
                            break;
                        case "Z":
                            Shape.MomentumZ = _Computed_new_momentum.Z * (float)0.5;
                            break;
                    }

                    Debugger.Log(1, "BOING", $"BOING CALLED ON SHAPE {Shape.ShapeNum}::Computed Differnce in BOING JUICE is {_Old_momentum - _Computed_new_momentum} {Environment.NewLine}");
                    Shape.FrameTimeOfLastBoing = _Frame_counter;
                }
                else
                {
                    Shape.PosY += _player_speed;
                }
            }
        }
        private static void ApplyGravity(Shape shape, double dt)
        {
            if (shape.ClampedToFloor == 0)
            {
                var _Y_speed_after_gravity = shape.Momentum.Y - (((shape.Gravity / 10f) * shape.Mass * dt) * 0.1f);
                if (Math.Abs(_Y_speed_after_gravity) <= (shape.TerminalVelocity / 4f) && _Frame_counter >= shape.FrameTimeOfLastBoing + 5)
                {
                    shape.MomentumY = (float)_Y_speed_after_gravity;
                    Debugger.Log(1, "", $"_Y_SPR::{_Y_speed_after_gravity}::SHAPE_NUM{shape.ShapeNum}{Environment.NewLine}");
                }
            }
        }
        /// <summary>
        /// Prints the PowerGL Logo
        /// </summary>
        public static void PrintFunnyLogo()
        {
            Console.WriteLine("                                                       ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(" _____                                 _____   _       ");
            Console.WriteLine("(_____)                   ____  _     (_____) (_)      ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("(_)__(_)___   _   _   _  (____)(_)__ (_)  ___ (_)      ");
            Console.WriteLine("(_____)(___) (_) ( ) (_)(_)_(_)(____)(_) (___)(_)      ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("(_)   (_)_(_)(_)_(_)_(_)(__)__ (_)   (_)___(_)(_)____  ");
            Console.WriteLine("(_)    (___)  (__) (__)  (____)(_)    (_____) (______) ");
            Console.ResetColor();
            Console.WriteLine();
        }
        private static void BuildPowerGLGUI()
        {
            ImGui.Text($"FPS:{FPS}");
            ImGui.SetWindowFontScale(1.5f);
            ImGui.Text($"<{ImGui.GetMousePos().X}-{ImGui.GetMousePos().Y}>");
            ImGui.EndMenu();
            if (ImGui.InputText("CMD", ref CMD_PROC, PROC_LEN, ImGuiInputTextFlags.EnterReturnsTrue))
            {
                GUI_CNS_TEXT += $"{ConsoleCommandHandler.ExecutePGLCommand(CMD_PROC)}{Environment.NewLine}";
                CMD_PROC = "";
            }
            ImGui.Text(GUI_CNS_TEXT);
            Thread.Sleep(1);
            if (CMD_PROC != "")
            {
                Debugger.Log(1, "", $"{CMD_PROC}{Environment.NewLine}");
            }
        }
        /// <summary>
        /// Goes around the the inablity to resize an array and will handle such such an action so that new shapes can be added
        /// </summary>
        /// <exception cref="InvalidDataException"></exception>
        private static void ExpandShapeArray()
        {
            int cur_len = Shapes.Length;
            Shape[] new_array = new Shape[cur_len + 1];
            for (int i = 0; i < Shapes.Length; i++)
            {
                new_array[i] = Shapes[i];
            }
            if (new_array.Length > Shapes.Length + 2)
            {
                EngineLogger.Log("FATAL ERROR ENGINE CRASH FROM OVER EXPANSION OF THE SHAPE ARRAY");
                throw new InvalidDataException("Array Got Oversized and will crash at .render() so we crash before the render crash");
            }
            Shapes = new_array;
            GlobalShapeCount = Shapes.Length;
        }
        /// <summary>
        /// The Public method to Modify Any Shape and select properties that won't cause the engine to crash from anywhere, Caution calling this for adding new shapes.
        /// </summary>
        /// <returns>
        /// -1 if theres no matches for property, -2 if there is a subfunction error, 0 for success.
        /// </returns>
        public static int ModifyShapeProperty(int ShapeNumber, string Property, float X, float Y, float Z, string? shader, string? frag, string? texture, float? Size)
        {//TODO me in the future, In our infinite design we forgor to include a string parameter for the texture path lmaoooo
            if (ShapeNumber > Shapes.Length - 1 && Property != "NEW")
            {
                EngineLogger.Log($"Engine called to modify a shape with number {ShapeNumber} but that is greater then the amount of shapes in the global state which is {GlobalShapeCount}");
                return 0;
            }
            //while techically NEW isn't a property of the PowerGL shape class in the area of changing the state of the engine shapes it lives here
            if (Property == "NEW")
            {
                try
                {
                    string _int_text = "";
                    if (texture == null)
                    {
                        _int_text = @$"{Application_Directory}\Randompictures\white.png";
                    }
                    else
                    {
                        _int_text = texture;
                    }
                    Shape new_shape = new Shape("cube", new Vector3(X, Y, Z), (uint)ShapeNumber, 0, 0, shader, frag, _int_text, Vector3.Zero, 1f, Vector3.One, Vector3.Zero, 0, Vector3.Zero, 0);
                    new_shape.Glcontext = Gl;
                    new_shape.Camera = Camera;
                    //TODO make this lookup the textures to save on compute by using already compiled textures to stop lower end stuttering ( we are not going to end up like unreal engine :|> )
                    new_shape.CompileShader();
                    if (_Name_to_texture_dict.ContainsKey(_int_text))
                    {
                        new_shape.CompiledTexture = _Name_to_texture_dict[_int_text];
                    }
                    else
                    {
                        new_shape.CompileTexture();
                        _Name_to_texture_dict.Add(_int_text, new_shape.CompiledTexture);
                    }
                    ExpandShapeArray();
                    Shapes[^1] = new_shape;
                    return 0;
                }
                catch (Exception ex)
                {
                    EngineLogger.Log($"//EXCEPTION// ATTEMPTING TO ADD NEW SHAPE TO GLOBAL STATE EXCEPTION {ex.Message} {Environment.NewLine}, STACKTRACE {ex.StackTrace}{Environment.NewLine}");
                    return -2;
                }
            }
            if (Property == "Position")
            {
                try
                {
                    Shapes[ShapeNumber].PosX = X;
                    Shapes[ShapeNumber].PosY = Y;
                    Shapes[ShapeNumber].PosZ = Z;
                    return 0;
                }
                catch (Exception ex)
                {
                    EngineLogger.Log($"//EXCEPTION// Modifting Postions EXCEPTION:{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                    return -2;
                }
            }
            if (Property == "Rotate")
            {
                try
                {
                    Shapes[ShapeNumber].RotX = X;
                    Shapes[ShapeNumber].RotY = Y;
                    Shapes[ShapeNumber].RotZ = Z;
                    return 0;
                }
                catch (Exception ex)
                {
                    EngineLogger.Log($"//EXCEPTION// Modifying Positions EXCEPTION:{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                    return -2;
                }
            }
            if (Property == "Scale")
            {
                try
                {
                    if (Size != null)
                    {
                        Shapes[ShapeNumber].Size = (float)Size;
                        return 0;
                    }
                    else
                    {
                        throw new Exception("Size Cannot be null");
                    }
                }
                catch (Exception ex)
                {
                    EngineLogger.Log($"//EXCEPTION// Modifying Scale EXCEPTION:{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                    return -2;
                }
            }
            if (Property == "Stretch")
            {
                try
                {
                    Shapes[ShapeNumber].StrX = X;
                    Shapes[ShapeNumber].StrY = Y;
                    Shapes[ShapeNumber].StrZ = Z;
                    return 0;
                }
                catch (Exception ex)
                {
                    EngineLogger.Log($"//EXCEPTION// Modifying Strech EXCEPTION:{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                    return -2;
                }
            }
            if (Property == "Shear")
            {
                Shapes[ShapeNumber].ShrX = X;
                Shapes[ShapeNumber].ShrY = Y;
                Shapes[ShapeNumber].ShrZ = Z; 
                return 0;
            }
            return -1;
        }
        public static void ApplyConfig()
        {
            //we just double check the config is not equal to what it currently is to save on things being executed
            if (!Config.Equals(Previous_config))
            {
                try
                {
                    Gl.Viewport(new Vector2D<int>(Config.WindowX, Config.WindowY));
                    Camera.AspectRatio = (float)Config.WindowX / Config.WindowY;
                    window.Title = Config.WindowTitle;
                    window.Size = new Vector2D<int>(Config.WindowX, Config.WindowY);
                    window.VSync = Config.IsVSyncEnabled;
                    Previous_config = Config;
                }
                catch (Exception ex)
                {
                    EngineLogger.Log($"//EXCEPTION// Failled to apply config change exception message {ex.Message}{Environment.NewLine} Config Rolled back to previous version");
                    Config = Previous_config;
                }
            }
        }
        public static void RevertConfig()
        {
            Config = Previous_config;
        }
        public static void UpdateRunningConfig(int NumberOfUpdates, string[] Properties, string[] Values)
        {
            for (int i = 0; i < Properties.Length; i++)
            {
                Config.UpdateConfiguration(Properties[i], Values[i]);
            }
        }

        public static void HandleEngineCrash(object sendder, UnhandledExceptionEventArgs crashevents)
        {
            Exception exception = crashevents.ExceptionObject as Exception ?? new Exception("ENGINE CRASH MEGA BAD"); 
            Console.WriteLine($"ENGINE CRASH {exception.Message}");
            EngineLogger.Log($"//CRITICAL FAILURE// ENIGINE CRASH EVENT EXCEPTION MESSAGE: {exception.Message}{Environment.NewLine} STACK TRACE FOR BUG REPORT {exception.StackTrace}");
            Console.ReadLine();
        }

        public static void ProcessWebSocket(HttpListenerContext context, Logger logger)
        {
            HttpListenerWebSocketContext WebSocketContext =  context.AcceptWebSocketAsync(null).Result;
            WebSocket WebSocket = WebSocketContext.WebSocket;

            while (WebSocket.State == WebSocketState.Open)
            {
                
                byte[] SocketBuffer = new byte[WebSocketBufferLength];
                WebSocketReceiveResult Result = WebSocket.ReceiveAsync(new ArraySegment<byte>(SocketBuffer),CancellationToken.None).Result;
                if (Result.MessageType == WebSocketMessageType.Text)
                {
                    if ((char)SocketBuffer[0] == '{') {
                        string JSON = (Encoding.UTF8.GetString(SocketBuffer));
                        HttpObject? Data = JsonSerializer.Deserialize<HttpObject>(JSON);
                        if (Data != null)
                        {
                            float? scale;
                            if (Data.Property == "Scale")
                            {
                                scale = Data.ValueX;
                            }
                            else
                            {
                                scale = null;
                            }
                            string Websock_Responsestring = string.Empty;
                            //the websock will only allow a single shape data to send to not overload the engine or the client
                            int modifystatus = -3;
                            try
                            {
                                modifystatus = ModifyShapeProperty((int)Data.ShapeNumber, Data.Property, Data.ValueX, Data.ValueY, Data.ValueZ, null, null, null, scale);
                            }
                            catch (Exception ex)
                            {
                                logger.Log($"WebSocket Encountered big oof error trying to modify shapes {ex.StackTrace}");
                                modifystatus = -3;
                            }
                            Websock_Responsestring = modifystatus switch
                            {
                                0 => "OK, Server Applied Changes Correclty",
                                -1 => "OK, Server got data but no valid property was given to the engine",
                                -2 => "OK, Server Got the data, but failed to apply correctly check engine log for details",
                                _ => "ERROR, Server failed to process request entirely"
                            };
                            WebSocket.SendAsync(Encoding.UTF8.GetBytes(Websock_Responsestring), WebSocketMessageType.Text, true, CancellationToken.None).Wait();
                        }
                    }
                    else
                    {
                        WebSocketBufferLength = (int)SocketBuffer[0];
                    }
                }
                else if (Result.MessageType == WebSocketMessageType.Close)
                {
                    WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,"Closing",CancellationToken.None).Wait();
                }
            }
        }
    }
}