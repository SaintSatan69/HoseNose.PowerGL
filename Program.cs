using Silk.NET.Input;
using Silk.NET.Windowing;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
//using Silk.NET.OpenGL.Extensions.ImGui;
using System.Numerics;
using HoseRenderer.PowerGl;
using HoseRenderer.NamedPipes;
using HoseRenderer.ExtraUtils;
using System.Diagnostics;
using System.Drawing;



namespace HoseRenderer
{
    public class MainRenderer
    {
#pragma warning disable CS8618 
        //OpenGL Things
        private static IWindow window;

        private static GL Gl;
        private static IKeyboard primaryKeyboard;

        private static VertexArrayObject<float, uint> VaoCube;
        private static BufferObject<float> Vbo;
        private static BufferObject<uint> Ebo;

        private static BufferObject<float> shapevbo;
        private static VertexArrayObject<float, uint> shapeVAO;
        private static Shader shader;
        private static Shader LightingShader;
        private static Shader LampShader;
        private static Vector3 LampPosition = new Vector3(1.2f, 1.0f, 2.0f);

        private static Texture DiffuseMap;
        private static Texture SpecularMap;
        private static Texture texture;

        private static Model model;
        private static Vector3 ModelPosition = new Vector3(2.0f,1.0f,3.0f);

        private static Camera Camera;

        private static Vector2 LastMousePosition;

        private static DateTime StartTime;

        private static PowerGLPipe Pipe;
        //debugging shape movements before attempting to complete the named pipe communion 
        private static float translationy = 1.0f;
        private static float rotationz = 1.0f;
        private static float scale = 1.0f;

        private static string[] FLAGS = new string[10];

        private static Dictionary<string,Texture> _Name_to_texture_dict = new();

        private static int _Frame_counter = 0;

        private static Shape[] Shapes = [];

        private static uint _player_1_controllerObject;
        private static uint _player_2_controllerObject;
        private static float _player_speed =  0.01f;

        private static readonly string _internal_app_dir = Directory.GetCurrentDirectory();
        public static string Application_Directory { get => _internal_app_dir;}

        //threading things
        private static Thread PipeThread;
        private static string _Pipe_THREAD_DATA = "";

        private static Thread FPS_Thread;
        private static uint FPS;

        //TODO make this thread pool spawn a thread for every 1000 shapes added so the computer doesn't melt trying to render 10k on a single thread and have the FPS be a nice 10 frames per milenium ( THIS IS VERY BUGGY)
        private static Thread[] ThreadPoolRenderer;

        //GUI things
        //private static ImGuiController guiController;
        private static int _Gui_Frame_state = 0;
        private static int _Last_frametime_GUI_change = 0;

#pragma warning restore CS8618 
        private static readonly float[] Vertices =
        {
            //X    Y      Z       Normals             U     V
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 1.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 1.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 0.0f, 1.0f,

            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f, 0.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f, 1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f, 1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f, 1.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f, 0.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f, 0.0f, 1.0f,

            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f, 0.0f, 1.0f,
            -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f, 1.0f, 1.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f, 1.0f, 0.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f, 1.0f, 0.0f,
            -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f, 0.0f, 0.0f,
            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f, 0.0f, 1.0f,

             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f, 0.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f, 1.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f, 1.0f, 0.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f, 1.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f, 0.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f, 0.0f, 1.0f,

            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f, 0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f, 1.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f, 1.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f, 1.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f, 0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f, 0.0f, 1.0f,

            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f, 0.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f, 1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f, 1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f, 1.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f, 0.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f, 0.0f, 1.0f
        };

        private static readonly uint[] Indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        public static void Main(string[] args)
        {
            if (args.Length > 0) {
                if (args[0] == "pipe_enable") {
                    FLAGS[0] = "IPC_NAMED_PIPE_ENABLE";
                    Console.WriteLine("FLAG IPC_NAMED_PIPE_ENABLE has been enabled");
                }
                if (args[0] != "render_extracube")
                {
                    FLAGS[1] = "";
                }
                else
                {
                    FLAGS[1] = "RENDER_DEFAULT_CUBES_REGARDLESS";
                }
                if (args[0] == "debug_msg_enable")
                {
                    FLAGS[2] = "DEBUG";

                }
                if (args[0] == "DEV_BUGLAND")
                {
                    FLAGS[0] = "IPC_NAMED_PIPE_ENABLE";
                    FLAGS[2] = "DEBUG";
                    FLAGS[3] = "PHYSICS";
                    //FLAGS[4] = "MULTITHREADED_RENDERER";
                    Console.WriteLine("AWAITING DEBUGGER");
                    while (!Debugger.IsAttached)
                    {
                        Thread.Sleep(100);
                    }
                }
            }
            //make sure to comment this when not debugging the cubes
            //FLAGS[0] = "IPC_NAMED_PIPE_ENABLE";
            //FLAGS[1] = "RENDER_DEFAULT_CUBES_REGARDLESS";
            //FLAGS[2] = "DEBUG";
            //end of Debugging flags to tinker from the source code

            PrintFunnyLogo();

            WindowOptions options = WindowOptions.Default;
            options.Size = new Vector2D<int>(1920, 1080);
            options.Title = "HoseNose.PowerGL";
            options.Position = new Vector2D<int>(0,30);
            
            Console.WriteLine("HoseNose.PowerGL has been loaded");
            Console.WriteLine($"{options.API.API.ToString()}{options.Size}");
            var Platforms = Window.Platforms.Count;
            Console.WriteLine($"Num of platforms: {Platforms}");
            window = Window.Create(options);
            //why must techonology not like powershell :( (we are cheating by having powershell just do the control of shapes engine, the shape properties and when this program is launched leaving
            //this to do the rendering calls to OpenGL through Silk.Net)
            Console.WriteLine($"{window.API.API} {window.WindowState}");
            
            //SharedFileIPC.InitalizeFileIPC();
            //this is for debugging the rendering engine not showing my shapes :( (Fixed that still leaving all this here for main engine debugging)
            //Shape[] shape = new Shape[2];
            //shape[0] = new Shape("Cube", new Vector3(2f, 3f, 3f), 1, 0, 0, null, null, null, null, null, ".\\randompictures\\silk.png", Vector3.Zero,1.0f, new Vector3(0.0f,0.0f,0.0f));
            //shape[1] = new Shape("Cube", new Vector3(1f, 1f, 0f), 2, 0, 0, null, null, null, null, null, ".\\randompictures\\PDQ_wallpaper.png", new Vector3(0.0f,0.0f,0.0f),2.0f, new Vector3(0.5f,0.0f,0.0f));
            //SharedFileIPC.WriteFileIPC(shape);

            if (FLAGS[0] == "IPC_NAMED_PIPE_ENABLE") {
                //VERY LIKELY CHANCE WE GET A RACE CONDITION BECAUSE BOTH PROGRAMS ARE SINGLE THREADED FOR ALL THEIR LOGIC BESIDES IN LIBRARIES AND OTHER THINGS SO WE CAN JUST SLEEP THE APPLICATION ONCE
                //IT STARTS SO POWERSHELL CAN CONSTRUCT THE NAMED PIPE AND AWAIT THE RENDERER TO CONNECT (THIS WORKS CORRECTLY NAMED PIPES HOWER ARE SYNCHORNOUS SO THE NAMED PIPE WILL DEADLOCK THE 
                //APPLICATION ATTEMPTING TO READ THE BUFFER)
                Thread.Sleep(2000);
                Pipe = SharedFileIPC.AttachToOrchastratorNamedPipe();
                PipeThread = new Thread(() => {
                    Thread.CurrentThread.IsBackground = true;
                    while (true)
                    {
                        _Pipe_THREAD_DATA = Pipe.ReadString();
                    }
                });
                PipeThread.Name = "PS_IPC_THREAD";
                PipeThread.Start();
            }
            
            window.Load += OnLoad;
            window.Render += OnRender;
            window.FramebufferResize += OnFramebufferResize;
            window.Update += OnUpdate;
            window.Closing += OnClose;
            window.Run();
            window.Dispose();
            SharedFileIPC.UninitalizeFileIPC();
        }
        public static void LoadObjects() {
            try
            {
                Shapes = SharedFileIPC.ReadShapeIPC();
                Console.WriteLine($"SUCCESS MAYBE OBJECT [0] is {Shapes[0].ShapeName}");
            }
            catch
            {
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
            for (int i = 0; i < input.Mice.Count; i++) {
                input.Mice[i].Cursor.CursorMode = CursorMode.Raw;
                input.Mice[i].MouseMove += OnMouseMove;
                input.Mice[i].Scroll += OnMouseWheel;
            }
            Gl = GL.GetApi(window);
            LoadObjects();
            Console.WriteLine($"Shapes array from LOAD_OBJECTS:{Shapes.Length}");
            //guiController = new(Gl,window,input);

            Ebo = new BufferObject<uint>(Gl, Indices, BufferTargetARB.ElementArrayBuffer);
            Vbo = new BufferObject<float>(Gl, Vertices, BufferTargetARB.ArrayBuffer);
            VaoCube = new VertexArrayObject<float, uint>(Gl, Vbo, Ebo);
            if (Shapes.Length == 0 || FLAGS[1] == "RENDER_DEFAULT_CUBES_REGARDLESS") {
                DebugMessages.PrintDebugMSG("Rending Default Cubes");

                VaoCube.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 8, 0);
                VaoCube.VertexAttributePointer(1, 3, VertexAttribPointerType.Float, 8, 3);
                VaoCube.VertexAttributePointer(2, 2, VertexAttribPointerType.Float, 8, 6);

                LightingShader = new Shader(Gl, ".\\Shaders\\shader.vert", ".\\Shaders\\lighting.frag");

                LampShader = new Shader(Gl, ".\\Shaders\\shader.vert", ".\\Shaders\\shader.frag");

                DiffuseMap = new Texture(Gl, ".\\randompictures\\silkBoxed.png");
                SpecularMap = new Texture(Gl, ".\\randompictures\\silkSpecular.png");

                shader = new Shader(Gl, ".\\Shaders\\Model.vert", ".\\Shaders\\Model.frag");
                texture = new Texture(Gl, ".\\Randompictures\\silk.png");
                model = new Model(Gl, ".\\Model\\cube.model");
            }
            var size = window.FramebufferSize;
            Camera = new Camera(new Vector3(-6.0f,0.0f,6.0f), Vector3.UnitX, Vector3.UnitY, (float)size.X / size.Y);
            if (Shapes.Length != 0)
            {
                //shapevertfloats = new();
                for (int i = 0; i < Shapes.Length; i++)
                {
                    Shapes[i].Glcontext = Gl;
                    Shapes[i].Camera = Camera;
                    Shapes[i].CompileShader();
                    if (Shapes[i].TexturePath != "" && Shapes[i].TexturePath != null) {
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
                            _Name_to_texture_dict.Add(_compiled_texture_path,_compiled_texture_texture);
                        }
                    }
                    if (Shapes[i].IsModel)
                    {
                        Shapes[i].CompileModelMesh();
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
                for (uint i = 0; i < Shapes.Length; i++) {
                    if (FLAGS[1]=="" || FLAGS[1] == null) {
                        VaoCube.VertexAttributePointer(i, 3, VertexAttribPointerType.Float, 8, 3 * (int)i);
                        if (FLAGS[2] == "DEBUG")
                        {
                            DebugMessages.PrintDebugMSG("Using the RAW vertexattribpointer if this is set to render the default cubes that is bad");
                        }
                        
                    }
                    else
                    {
                        VaoCube.VertexAttributePointer((i + 3), 3, VertexAttribPointerType.Float, 8, 3 * (int)(i + 3));
                        if (FLAGS[2] == "DEBUG")
                        {
                            DebugMessages.PrintDebugMSG($"Using the vertextattribpointer in the context of rendering default cubes. I offset for math is {i + 3}");
                        }
                    }
                }
            }
            if (FLAGS[2] == "DEBUG") {
                foreach (var _texture_name_mapper in _Name_to_texture_dict)
                {
                    Console.WriteLine(_texture_name_mapper.Key);
                }
            }

            FPS_Thread = new Thread(() =>
            {
                
                while (true) {
                    Thread.CurrentThread.Name = "FPS_THREAD";
                    Thread.CurrentThread.IsBackground = true;
                    uint _frame_counter_start = (uint)_Frame_counter;
                    Thread.Sleep(1000);
                    uint _frame_counter_end = (uint)_Frame_counter;
                    FPS = _frame_counter_end - _frame_counter_start;
                    Debugger.Log(1,"THREADING",$"FPS::{FPS}{Environment.NewLine}");
                }
            });
            FPS_Thread.Start();
            if (FLAGS[4] == "MULTITHREADED_RENDERER") {
                int _num_renderer_threads;
                int _num_Shapes = Shapes.Length;
                int _last_thread_shaperange;
                if (_num_Shapes > 0)
                {
                    if (_num_Shapes % 1000 == 0)
                    {
                        _num_renderer_threads = _num_Shapes / 1000;
                        _last_thread_shaperange = 0;
                    }
                    else
                    {
                        _num_renderer_threads = ((int)(_num_Shapes / 1000)) + 1;
                        _last_thread_shaperange = _num_Shapes % 1000;
                    }
                    ThreadPoolRenderer = new Thread[_num_renderer_threads];
                    for (int i = 0; i < _num_renderer_threads; i++)
                    {
                        int _min_shape_num = 1000 * i;
                        int _max_shape_num;
                        if (_last_thread_shaperange > 0) {
                            _max_shape_num = _last_thread_shaperange;
                        }
                        else
                        {
                            _max_shape_num = ((1000 * i) - 1) + 1000;
                        }
                        //what in the world is this abominiation
                        ThreadPoolRenderer[i] = new Thread(() =>
                        {
                            Thread.CurrentThread.IsBackground = true;
                            string _threadfullname = $"RENDERER_THREAD::{i}::{_min_shape_num}->{_max_shape_num}";
                            Thread.CurrentThread.Name = _threadfullname;
                            int _internal_threaded_min = _min_shape_num;
                            int _internal_threaded_max = _max_shape_num;
                            while (true) {
                                for (int i = _internal_threaded_min; i < _internal_threaded_max; i++) {
                                    Shapes[i].Render();
                                }
                            }
                        });
                    }
                }
            }
        }

        private static unsafe void OnRender(double dt)
        {
            _Frame_counter++;
            Gl.Enable(EnableCap.DepthTest);
            Gl.Clear((uint) (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
            VaoCube.Bind();
            if (FLAGS[1] == "RENDER_DEFAULT_CUBES_REGARDLESS" || Shapes.Length == 0)
            {
                RenderLitCube();

                RenderLampCube();

                RenderModel();
            }
            if (FLAGS[3] == "PHYSICS")
            {
                
                ComputeWhetherOrNotThereIsAShapeInsideAShapeThatSupportsCollions(Shapes);
                for (int i = 0; i < Shapes.Length; i++)
                {
                    if (Shapes[i].IsEffectedByGravity) {
                        ApplyGravity(Shapes[i],dt);
                    }
                }
            }
            //guiController.Update((float)dt);
            //Debugger.Log(4,"INTERNAL_GUI",$"GUI_TYPE{_Gui_Frame_state}");

            //ImGuiNET.ImGui.ShowDemoWindow();
            //BuildGUI();
            //if (_Gui_Frame_state == 1) {
            //    try
            //    {
            //        guiController.Render();
            //    }
            //    catch { }
            //}



            if (FLAGS[0] == "IPC_NAMED_PIPE_ENABLE") {
                if (_Pipe_THREAD_DATA == "DEBUG:PIPETALKED")
                {
                    Debugger.Log(1,"IPC",$"PIPE COMMUNION SUCCESSFUL{Environment.NewLine}");
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
                    }catch(Exception e){
                        DebugMessages.PrintDebugMSG($"Error read pipe contents with exception {e.StackTrace}");
                        Debugger.Log(1,"IPC_PIPE","Chances are you just sent a string that doesn't contain a number and angered the .parse");
                    }
                    Pipecontents = "";
                    _Pipe_THREAD_DATA = ""; 
                }
                
            }
            if (FLAGS[4] != "MULTITHREADED_RENDERER")
            {
                for (int i = 0; i < Shapes.Length; i++)
                {
                    Shapes[i].Render();
                }
            }
            else
            {
                for (int i = 0; i < ThreadPoolRenderer.Length; i++)
                {
                    //prepare to have to make a global var that blocks execution from main thread to sub thread in the instance that join cannot escape a while(true) loop
                    var cur_thread = ThreadPoolRenderer[i];
                    if (cur_thread.ThreadState == System.Threading.ThreadState.Unstarted)
                    {
                        cur_thread.Start();
                    }
                    if (cur_thread.ThreadState == System.Threading.ThreadState.WaitSleepJoin)
                    {
                        cur_thread.Interrupt();
                    }
                    cur_thread.Join();
                }
            }
        }
        private static unsafe void RenderLitCube()
        {
            LightingShader.use();
            DiffuseMap.Bind(TextureUnit.Texture0);
            SpecularMap.Bind(TextureUnit.Texture1);
            var megamatix = Matrix4x4.Identity;
            megamatix *= Matrix4x4.CreateRotationY(MathHelper.DegreesToRadians(25f));
            megamatix *= Matrix4x4.CreateTranslation(new Vector3(1.0f, translationy, 1.0f));
            megamatix *= Matrix4x4.CreateRotationZ(MathHelper.DegreesToRadians(rotationz));
            megamatix *= Matrix4x4.CreateScale(scale);
            LightingShader.SetUniform("uModel", megamatix);
            LightingShader.SetUniform("uView", Camera.GetViewMatrix());
            LightingShader.SetUniform("uProjection", Camera.GetProjectionMatrix());
            LightingShader.SetUniform("material.diffuse", 0);
            LightingShader.SetUniform("material.specular", 1);
            LightingShader.SetUniform("material.shininess", 32.0f);
            LightingShader.SetUniform("viewPos", Camera.Position);

            var diffuseColor = new Vector3(0.5f);
            var ambientColor = diffuseColor * new Vector3(0.2f);

            LightingShader.SetUniform("light.ambient", ambientColor);
            LightingShader.SetUniform("light.diffuse", diffuseColor);
            LightingShader.SetUniform("light.specular", new Vector3(1.0f, 1.0f, 1.0f));
            LightingShader.SetUniform("light.position", LampPosition);
            Gl.DrawArrays(PrimitiveType.Triangles, 0, 36);
            if (translationy > 10f)
            {
                translationy = 0f;
            }
            else
            {
                translationy += (0.1f / ( 2 * scale));
            }
            if (scale > 50f)
            {
                scale = 1f;
            }
            else
            {
                scale += 0.1f;
            }
            rotationz += (1f / ( 2 * scale));
        }
        private static unsafe void RenderLampCube() {
            LampShader.use();
            var lampMatrix = Matrix4x4.Identity;
            lampMatrix *= Matrix4x4.CreateScale(0.2f);
            lampMatrix *= Matrix4x4.CreateTranslation(LampPosition);

            LampShader.SetUniform("uModel", lampMatrix);
            LampShader.SetUniform("uView", Camera.GetViewMatrix());
            LampShader.SetUniform("uProjection", Camera.GetProjectionMatrix());

            Gl.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }
        private static unsafe void RenderModel()
        {
            texture.Bind();
            shader.use();
            shader.SetUniform("uTexture0", 3);

            var difference = (float)(window.Time * 100);

            var size = window.FramebufferSize;

            var ModelMatrix = Matrix4x4.Identity;
            ModelMatrix *= Matrix4x4.CreateScale(0.2f);
            ModelMatrix *= Matrix4x4.CreateTranslation(ModelPosition);

            foreach (var mesh in model.Meshes)
            {
                mesh.Bind();
                shader.use();
                texture.Bind();
                shader.SetUniform("uTexture0",0);
                shader.SetUniform("uModel", ModelMatrix);
                shader.SetUniform("uView",Camera.GetViewMatrix());
                shader.SetUniform("uProjection",Camera.GetProjectionMatrix());

                Gl.DrawArrays(PrimitiveType.Triangles,0,(uint)mesh.Verticies.Length);
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
                else {
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
            //if (primaryKeyboard.IsKeyPressed(Key.Tab))
            //{
            //    if (_Frame_counter >= _Last_frametime_GUI_change + 10) {
            //        if (_Gui_Frame_state < 2) {
            //            _Gui_Frame_state++;
            //            _Last_frametime_GUI_change = _Frame_counter;
            //        }
            //        else
            //        {
            //            _Gui_Frame_state = 0;
            //        }
            //    }
            //}
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
                Shapes[_player_1_controllerObject].PosZ += _player_speed;
            }
            if (primaryKeyboard.IsKeyPressed(Key.H))
            {
                Shapes[_player_1_controllerObject].PosZ -= _player_speed;
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
            Camera.ModifyZoom(scrollWheel.Y);
        }
        private static void OnClose()
        {
            if (Shapes.Length == 0) {
                Vbo.Dispose();
                
                VaoCube.Dispose();
                LightingShader.Dispose();
                model.Dispose();
                shader.Dispose();
          
               texture.Dispose();
            }
            else
            {
                //shapevbo.Dispose();
                VaoCube.Dispose();
            }
            Ebo.Dispose();
        }
        private static void KeyDown(IKeyboard arg1, Key arg2, int arg3)
        {
            if (arg2 == Key.Escape)
            {
                window.Close();
            }
        }
        private static void ComputeWhetherOrNotThereIsAShapeInsideAShapeThatSupportsCollions(Shape[] Shapes)
        {
            for (int i = 0; i < Shapes.Length; i++) {
                for (int j = 0; j < Shapes.Length; j++)
                {
                    if (i != j && Shapes[i].HasCollison == 1 && Shapes[j].HasCollison == 1)
                    {
                        //Console.WriteLine($"MRT1:{Matrises[i].Translation}::MRT2{Matrises[j].Translation}");
                        //attempt 3 works, had a little help from copilot since i wasn't finding any sort of ideas on google it uses AABB (axis-alligned Bounding Boxes)
                        if (Shapes[i].BoundingBox.Intersects(Shapes[j].BoundingBox))
                        {
                            Debugger.Log(1,"",$"{_Frame_counter}::Shape I:{Shapes[i].ShapeNum} collides with Shape:{Shapes[j].ShapeNum} {Environment.NewLine}");
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
                            Debugger.Log(2,"",$"{_Frame_counter}::Shape I:{Shapes[i].ShapeNum} doesn't collide with Shape{Shapes[j].ShapeNum} {Environment.NewLine}");
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
            if (_Frame_counter >= _frame_count_last_boing + 5)
            {
                if (Shape.Player_moveable != 1) {
                    var _Old_momentum = Shape.Momentum;
                    Vector3 _Computed_new_momentum;
                    if (Shape.ClampedToFloor == 0)
                    {
                        _Computed_new_momentum = (Shape.Momentum * Shape.BoingFactor) * -1;
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
            if (shape.ClampedToFloor == 0) {
                var _Y_speed_after_gravity = shape.Momentum.Y - (((shape.Gravity / 10f) * shape.Mass * dt) * 0.1f);
                if (Math.Abs(_Y_speed_after_gravity) <= (shape.TerminalVelocity / 4f) && _Frame_counter >= shape.FrameTimeOfLastBoing + 5)
                {
                    shape.MomentumY = (float)_Y_speed_after_gravity;
                    Debugger.Log(1, "", $"_Y_SPR::{_Y_speed_after_gravity}::SHAPE_NUM{shape.ShapeNum}{Environment.NewLine}");
                }
            }
        }
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
        //private static void BuildGUI()
        //{
        //    ImGuiNET.ImGui.ShowDemoWindow();
        //}
    }
}