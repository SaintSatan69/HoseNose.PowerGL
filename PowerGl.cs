using Silk.NET.OpenGL;
using System.Numerics;
using HoseRenderer.Exceptions;
using System.Text.Json.Serialization;
using HoseRenderer.ExtraUtils;
using System.Diagnostics;

namespace HoseRenderer
{
    namespace PowerGl
    {
        ///<summary>
        ///This Class Object is the object representing a singular shape/model in 3D space
        ///</summary>
        public class Shape
        {
            ///<summary>
            ///The Shape name, not super important but if you want to include it to keep tabs on it from powershell thats acceptable
            ///</summary>
            [JsonInclude]
            [JsonRequired]
            public string ShapeName { get; private set; } = "Cube";
            /// <summary>
            /// A Vector 3 (x,y,z) that stores the global position of the shape
            /// </summary>
            [JsonInclude]
            [JsonRequired]
            public Vector3 Position { get; set; } = new Vector3(0f, 0f, 0f);
            /// <summary>
            /// [float] The X component of the Postition needed for the deserialization of the position
            /// </summary>
            [JsonInclude]
            [JsonRequired]
            public float PosX { get; set; } = 0f;
            /// <summary>
            /// [float] The Y component of the Position needed for the deserialization of the position
            /// </summary>
            [JsonInclude]
            [JsonRequired]
            public float PosY { get; set; } = 0f;
            /// <summary>
            /// [float] The Z component of the Position needed for the deserialization of the position
            /// </summary>
            [JsonInclude]
            [JsonRequired]
            public float PosZ { get; set; } = 0f;
            /// <summary>
            /// [string] path to the image that the shape will use as a texture
            /// </summary>
            [JsonInclude]
            [JsonRequired]
            public string? TexturePath { get; set; }
            /// <summary>
            /// [string] path to the GLSL .vert shader that the shape will use
            /// </summary>
            [JsonInclude]
            [JsonRequired]
            public string ShaderVertPath { get; set; }
            /// <summary>
            /// [string] path to the GLSL .frag shader that the shape will use 
            /// </summary>
            [JsonInclude]
            [JsonRequired]
            public string FragmentPath { get; set; }
            /// <summary>
            /// [uint] a flag to tell the program that the shape emits light [NOT IMPLEMENTED FULLY]
            /// </summary>
            [JsonInclude]
            [JsonRequired]
            public uint LightEmitting { get; private set; } = 0;
            /// <summary>
            /// [boolean] a flag to tell the program that this is a model and needs to compile a whole mess of meshes and verticies
            /// </summary>
            [JsonInclude,JsonRequired]
            public Boolean IsModel { get; private set; } = false;
            /// <summary>
            /// [string] path to .model file if IsModel is true
            /// </summary>
            [JsonInclude]
            [JsonRequired]
            public string ModelPath { get; private set; } = (MainRenderer.Application_Directory + "\\Model\\cube.model" );
            /// <summary>
            /// [Model] Ignored by the json serializer, it holds the Model object containing the compiled model and its submeshes
            /// </summary>
            [JsonIgnore]
            public Model? Model { get; set; }
            /// <summary>
            /// [uint] the number of this specific shape VERY important, the program uses this number to know which of the many shapes to modify
            /// </summary>
            [JsonInclude]
            [JsonRequired]
            public uint ShapeNum { get; private set; }
            /// <summary>
            /// [uint] a needing to be reworked to IsReflective property that makes the program handle this shape having reflections in light
            /// </summary>
            [JsonInclude]
            [JsonRequired]
            public uint? Reflective { get; private set; } = 0;
            /// <summary>
            /// [string] path to the image of the diffusion map on the shape used in reflection  [not fully implemented]
            /// </summary>
            [JsonInclude]
            [JsonRequired]
            public string? DiffuseMapPath { get; private set; }
            /// <summary>
            /// [string[ path to the image of the specular map on the shape used in reflection [Not fully implemented]
            /// </summary>
            [JsonInclude]
            [JsonRequired]
            public string? SpecularMapPath { get; private set; }
            /// <summary>
            /// [uint] a flag to signal that shader compilation has been done for debugging
            /// </summary>
            public uint ShaderCompilationCompleted { get; private set; }
            /// <summary>
            /// [Shader] the compiled shader itself 
            /// </summary>
            public Shader? CompiledShader { get; private set; }
            /// <summary>
            /// [Texture] the shapes texture can be reused between shapes to save on compute power
            /// </summary>
            public Texture? CompiledTexture { get; set; }
            /// <summary>
            /// [Texture] if diffuse map is provided this is the compiled version of it
            /// </summary>
            private Texture? CompiledDiffuseMap { get; set; }
            /// <summary>
            /// [Texture] if specular map is provided this is the compiled version of it
            /// </summary>
            private Texture? CompiledSpecularMap { get; set; }
            /// <summary>
            /// a Vector 3 (x,y,z) of the shapes rotation in 3d space
            /// </summary>
            [JsonInclude]
            [JsonRequired]
            public Vector3? RotationVector { get; set; }
            /// <summary>
            /// [float] the X component of the rotation vector used for the deserialization 
            /// </summary>
            [JsonInclude]
            [JsonRequired]
            public float RotX { get; set; } = 0f;
            /// <summary>
            /// [float] the Y component of the rotation vector used for the deserialization
            /// </summary>
            [JsonInclude]
            [JsonRequired]
            public float RotY { get; set; } = 0f;
            /// <summary>
            /// [float] the Z component of the rotation vector used for the deserialization
            /// </summary>
            [JsonInclude]
            [JsonRequired]
            public float RotZ { get; set; } = 0f;
            /// <summary>
            /// [float] the size of the shape, it is equaly added to the stretch to make a vector3 used by the scalar matrix; defaults to 1
            /// </summary>
            public float Size { get; set; } = 1f;
            /// <summary>
            /// a vector3 (x,y,z) of the shapes strech in 3d space
            /// </summary>
            public Vector3 Stretch { get; set; } = new(1.0f, 1.0f, 1.0f);
            /// <summary>
            /// [float] the X component of the stretch vector used for deserialization
            /// </summary>
            [JsonInclude]
            [JsonRequired]
            public float StrX { get; set; } = 1f;
            /// <summary>
            /// [float] the Y component of the stretch vector used for deserialization
            /// </summary>
            [JsonInclude]
            [JsonRequired]
            public float StrY { get; set; } = 1f;
            /// <summary>
            /// [float] the Z component of the stretch vector used for deserialization
            /// </summary>
            [JsonInclude]
            [JsonRequired]
            public float StrZ { get; set; } = 1f;
            /// <summary>
            /// a vector3 (x,y,z) of the shapes Shearing [Z is broken avoid if possible]
            /// </summary>
            public Vector3 Shear { get; set; } = new(0f,0f,0f);
            /// <summary>
            /// [float] the X component of the shear vector used for deserialization
            /// </summary>
            [JsonInclude]
            [JsonRequired]
            public float ShrX { get; set; } = 0f;
            /// <summary>
            /// [float] the Y component of the shear vector used for deserialization
            /// </summary>
            [JsonInclude]
            [JsonRequired]
            public float ShrY { get; set; } = 0f;
            /// <summary>
            /// [float] the Z component of the shear vector used for deserialization
            /// </summary>
            [JsonInclude]
            [JsonRequired]
            public float ShrZ { get; set; } = 0f;
            /// <summary>
            /// [float] the amount of bounce a shape will experience less then 1 is losing speed per bounce, greater then 1 is gaining speed when bouncing
            /// </summary>
            [JsonInclude]
            [JsonRequired]
            public float Restitution { get; set; } = 1f;
            /// <summary>
            /// [float] how shiny the shape is when reflective [Not fully implemented]
            /// </summary>
            public float? Shininess { get; set; }
            /// <summary>
            /// [GL] BE CAREFUL MODIFYING THIS ITS THE OPENGL CONTEXT GENERATED BY THE MAIN THREAD AND HAS TO BE PASSED TO EACH OBJECT AT LOAD TIME SO THEY CAN COMPILE SHADERS AND TEXTURES ON THE GPU
            /// </summary>
            public GL? Glcontext { get; set; }
            /// <summary>
            /// [Camera] Unless you have your own camera don't modify this since its injected from the main thread at load time
            /// </summary>
            public Camera? Camera { get; set; }
            /// <summary>
            /// [uint] a uint flag that is used in the physics collision engine so that some shapes can and cannot have collision
            /// </summary>
            public uint HasCollision { get; set; } = 0;
            /// <summary>
            /// [matrix4x4] the matrix that has been applied to the shape at last render time after scales, transforms, rotations have been applied
            /// </summary>
            public Matrix4x4 ShapeMatrix { get; set; } = Matrix4x4.Identity;
            /// <summary>
            /// [int] a flag that signals to a shape that at render time some of its properties [position,rotation,scale] have been told to be modified and that it needs to apply them before the matrix math that dictates its state are run
            /// </summary>
            public int AwaitingDispatchFromNamedPipe { get; set; } = 0;
            /// <summary>
            /// [boolean] a flag that lets the engine know this object handles gravity should also be used with the HasCollision
            /// </summary>
            public Boolean IsEffectedByGravity { get; set; }
            /// <summary>
            /// a vector3 (x,y,z) that hold the Moment of the shape used mainly to debug the momentum mechanic
            /// </summary>
            public Vector3 Momentum { get; set; } = new(0.0f, 0.0f, 0.0f);
            /// <summary>
            /// [float] the X component of the momentum vector
            /// </summary>
            public float MomentumX { get; set; } = 0.0f;
            /// <summary>
            /// [float] the Y component of the momentum vector
            /// </summary>
            public float MomentumY { get; set; } = 0.0f;
            /// <summary>
            /// [float] the Z component of the momentum vector
            /// </summary>
            public float MomentumZ { get; set; } = 0.0f;
            /// <summary>
            /// [int] the int of the last time the shape successfully bounced off an object, used to make sure a bouncy doesn't happen within 10 frames of the last bounce so that it has time to get out of the collider of the other object without bouncing inside of it and never leaving
            /// </summary>
            public int FrameTimeOfLastBoing { get; set; }
            /// <summary>
            /// [BoundingBox] the shapes AABB (axis-alligned bounding box) for handling it collision
            /// </summary>
            public BoundingBox BoundingBox { get; set; }
            /// <summary>
            /// [float] the amount of gravity [still needs tweakes to get right]
            /// </summary>
            public float Gravity { get; set; } = 0.1f;
            /// <summary>
            /// [string] used by the engine to tell a shape what exactly it needs to modify as told by the powershell server
            /// </summary>
            public string? DISPATCH_TARGET_STRING { get; set; }
            /// <summary>
            /// [int] a flag to make the rending process spit out more verbos information to debug it
            /// </summary>
            public int Debug { get; set ; } = 0;

            private Vector3 _Size_as_Vec3 {  get; set; }
            private float _BB_MIN_X { get; set; }
            private float _BB_MIN_Y { get; set; }
            private float _BB_MIN_Z { get; set; }
            private float _BB_MAX_X { get; set; }
            private float _BB_MAX_Y { get; set; }
            private float _BB_MAX_Z { get; set; }
            /// <summary>
            /// [double] the maximum speed the object is allowed to travel based on its mass and gravity to make sure it doesn't teleport out of existence by falling a large distance
            /// </summary>
            public double TerminalVelocity { get; private set; }
            /// <summary>
            /// [float] the mass of the object which modifies its terminal velocity
            /// </summary>
            public float Mass { get; private set; } = 1f;
            /// <summary>
            /// [uint] if the shape current momentum is low and it bounces onto the floor it will clamp so that its not just stuck forever going up and down
            /// </summary>
            public uint ClampedToFloor { get; set; } = 0;
            /// <summary>
            /// [uint] a flag to state that this object is directly controlled by a player or dev without it coming from the powershell server
            /// </summary>
            [JsonInclude]
            [JsonRequired]
            public uint Player_moveable { get; set; }
            /// <summary>
            /// [uint] states which player is controlling it [so far only there are 1,2]
            /// </summary>
            [JsonInclude]
            [JsonRequired]
            public uint Player_scheme { get; set; }
            /// <summary>
            /// [float] the max distance an object will be rendered to save on computer resources
            /// </summary>
            public float Culling_Distance { get; set; } = 200f;

            private bool IgnoreTimeUniform = false;
            /// <summary>
            /// The Base contructor of a shape this is the one that should be used over the parameterless one
            /// </summary>
            /// <param name="shapename">The human name for this shape</param>
            /// <param name="position">The shapes position in 3d space</param>
            /// <param name="shapenum">The rngines internal name for the shape</param>
            /// <param name="reflective"></param>
            /// <param name="glowy"></param>
            /// <param name="shaderpath">The path to the .vert vertex shader</param>
            /// <param name="fragmentpath">The path to the .frag fragment shader</param>
            /// <param name="texturepath">The path to the picture used as the texture</param>
            /// <param name="rotationvector">The rotation of the shape in 3d space</param>
            /// <param name="size">The size of the shape</param>
            /// <param name="stretch">The stretch of the shape in 3d space</param>
            /// <param name="shear">The shear of the shape in 3d space</param>
            /// <param name="Collision">Whether or not the shape can collide with other shapes that support collision</param>
            /// <param name="momentum">The shapes inital momentum from engine start</param>
            /// <param name="Restitution">The factor of bouncing when a shape collides with another</param>
            /// <param name="iseffectedbygravity">Whether or not the shape is effected by gravity down</param>
            /// <param name="player_control">Whether or not the shape can be controlled by a control scheme</param>
            /// <param name="player_num">The 'Player' number which is what control scheme the shape is controlled by</param>
            /// <param name="ismodel">Should always be true but whether or not the shape is actually a model</param>
            /// <param name="modelpath">The path to the .obj or .model file that holds all the vertexs needed to make a model</param>
            /// <param name="CullDistance">Distance From Camera Before Shape Isn't rendered</param>
            public Shape(string shapename, Vector3 position, uint shapenum, uint reflective, uint glowy, string? shaderpath, string? fragmentpath, string? texturepath, Vector3 rotationvector, float size, Vector3 stretch, Vector3 shear, uint Collision,Vector3 momentum,float Restitution,Boolean iseffectedbygravity = false, uint player_control = 0, uint player_num = 1, Boolean ismodel = false,string modelpath = "",float CullDistance = 200)
            {
                ShapeName = shapename;
                Position = position;
                ShapeNum = shapenum;
                Size = size;
                HasCollision = Collision;
                Reflective = reflective;
                LightEmitting = glowy;
                RotationVector = rotationvector;
                TexturePath = texturepath;
                if (shaderpath == null)
                {
                    ShaderVertPath = ".\\Shaders\\shader.vert";
                }
                else
                {
                    ShaderVertPath = shaderpath;
                }
                if (fragmentpath == null)
                {
                    if (reflective == 0) {
                        FragmentPath = ".\\Shaders\\shader.frag";
                    }
                    else
                    {
                        FragmentPath = ".\\Shaders\\lighting.frag";
                    }
                }
                else
                {
                    FragmentPath = fragmentpath;
                }
                PosX = position.X; 
                PosY = position.Y; 
                PosZ = position.Z;
                RotX = rotationvector.X;
                RotY = rotationvector.Y;
                RotZ = rotationvector.Z;
                Stretch = stretch;
                StrX = stretch.X;
                StrY = stretch.Y;
                StrZ = stretch.Z;
                Shear = shear;
                ShrX = shear.X;
                ShrY = shear.Y;
                ShrZ = shear.Z;
                Momentum = momentum;
                MomentumX = momentum.X;
                MomentumY = momentum.Y;
                MomentumZ = momentum.Z;
                Restitution = Restitution;
                TerminalVelocity = Math.Sqrt((2 * Mass * Gravity));
                IsEffectedByGravity = iseffectedbygravity;
                IsModel = ismodel;
                ModelPath = modelpath;
                Culling_Distance = CullDistance;

                Player_moveable = player_control;
                Player_scheme = player_num;
            }
            /// <summary>
            /// DO NOT USE THIS IS JUST HERE FOR THE JSON SERIALIZATION PROCESS WHICH REQUIRES A PARAMETERLESS CTOR TO CONVERT THE RAW BYTES BACK INTO AN OBJECT
            /// </summary>
            public Shape() { }
            /// <summary>
            /// the method that needs to be called after giving the object the GL context this tells openGL to compile the shader
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            public void CompileShader()
            {
                Console.WriteLine($"running shader compilation on shape num {this.ShapeNum}");
                if (this.Glcontext == null)
                {
                    throw new InvalidOperationException("Cannot compile shaders when there is no OPENGL context for which to bind to.");
                }
                try
                {
                    this.CompiledShader = new Shader(this.Glcontext, this.ShaderVertPath, this.FragmentPath);
                    this.ShaderCompilationCompleted = 1;
                    Console.WriteLine($"Shader Compilation Complete on shape num {this.ShapeNum}");
                    MainRenderer.EngineLogger.Log($"Shader Compilation on Shape {this.ShapeNum}, VertexShader={this.ShaderVertPath}, FragShader={this.FragmentPath} Completed Successfully");
                }
                catch (Exception ex) 
                {
                    MainRenderer.EngineLogger.Log($"Shader Compilation on Shape {this.ShapeNum}, VertexShader={this.ShaderVertPath}, FragShader={this.FragmentPath} FAILED Exception:{ex.Message}");
                    throw new InvalidOperationException($"See Error Log at ${MainRenderer.EngineLogger.Log_Directory}");
                }
            }
            /// <summary>
            /// Compiles the specifed texture using the shapes OpenGL context
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            public void CompileTexture()
            {
                if (this.TexturePath == null || this.TexturePath == "")
                {
                    this.CompiledTexture = null;
                    return;
                }
                else
                {

                    if (this.Glcontext == null)
                    {
                        throw new InvalidOperationException("Cannot compile the textures where their is no OPENGL context for which to bind to.");
                    }
                    else
                    {
                        this.CompiledTexture = new Texture(this.Glcontext, this.TexturePath);
                        Console.WriteLine($"Texture Compilation Complete on shape num {this.ShapeNum}");
                        MainRenderer.EngineLogger.Log($"Texture Compilation of texture {this.TexturePath} Successful");
                    }
                }
            }
            /// <summary>
            /// [not fully implemented] will compile the shapes diffusing and specular maps
            /// </summary>
            public void CompileDiffusion()
            {
                if (this.Glcontext != null) {
                    if (this.Reflective == 1)
                    {
                        if (this.DiffuseMapPath == null)
                        {
                            Console.WriteLine($"No diffusion map provided on shape:{this.ShapeName} of which is shape {this.ShapeNum}, it is marked as reflective so falling back to default");
                            this.DiffuseMapPath = ".\\randompictures\\silkBoxed.png";

                        }
                        if (this.SpecularMapPath == null)
                        {
                            Console.WriteLine($"No specular map provided on shape:{this.ShapeName} of which is shape {this.ShapeNum}, it is marked as reflective so falling back to default");
                            this.SpecularMapPath = ".\\randompictures\\silkSpecular.png";
                        }
                        this.CompiledDiffuseMap = new Texture(this.Glcontext, this.DiffuseMapPath);
                        this.CompiledSpecularMap = new Texture(this.Glcontext, this.SpecularMapPath);
                    }
                    else
                    {
                        return;
                    }
                }
            }
            /// <summary>
            /// if the shape is a model this is compile out the models meshes using the OpenGL context [WARNING MODELS ARE CURRENTLY BUGGY AND DO NOT ASSEMBLE CORRECTLY]
            /// </summary>
            public void CompileModelMesh()
            {
                if (this.Glcontext != null) {
                    this.Model = new Model(this.Glcontext, this.ModelPath);
                    MainRenderer.EngineLogger.Log($"Model {this.ModelPath} Has Completed Building");
                }
                else
                {
                    return ;
                }
            }
            //TODO MAKE THE MATH THAT HANDLES OBJECTS IN RELATION TO EACHOTHER USING A GLOBAL LIGHT ARRAY SO THAT YOU CAN DO THE MATRIX MATH AS TO HOW TO MAKE THING BRIGHT
            /// <summary>
            /// the important method, this handles everything a shape needs to showup on the screen. From receiving a message from powershell to modify a property with the DIRECTIVES, to the matrix math that needs to be applied to the identity matrix to position,rotate, and scale the shape
            /// </summary>
            /// <exception cref="AnomalousObjectException"></exception>
            public void Render()
            {
                int Print_debug_msg = 0;
                if (this.Debug == 1)
                {
                    Print_debug_msg = 1;
                }
                //NONE OF THIS IS WORKING I DONT KNOW WHY IT JUST ISN'T RENDERING THE CUBE WHAT AM I NOT UNDERSTANDING IN MY OWN MESS ( IT WAS WORKING IM JUST DUMB AND NEVER APPLIED ANY THING TO THE MODEL WITH A MATRIX, ONCE I DID THE HECKING THING WORK)
                string dbgmessage = "RENDER CALL ON OBJECT" + this.ShapeNum;
                DebugMessages.PrintDebugMSG(dbgmessage, Print_debug_msg);

                this.GlueShape();
                string[] DISPATCH_STRING_EXPLODED;
                string DISPATCH_OPERATION_DIRECTIVE = "";
                string DISPATCH_VALUE_OF_MODIFICATION = "";
                
                if (this.DISPATCH_TARGET_STRING != "" && this.DISPATCH_TARGET_STRING != null)
                {

                    DISPATCH_STRING_EXPLODED = this.DISPATCH_TARGET_STRING.Split(",");
                    if (DISPATCH_STRING_EXPLODED.Length == 3) {
                        DISPATCH_OPERATION_DIRECTIVE = DISPATCH_STRING_EXPLODED[1];
                        DISPATCH_VALUE_OF_MODIFICATION = DISPATCH_STRING_EXPLODED[2];
                    }
                    else
                    {
                        this.DISPATCH_TARGET_STRING = null;
                        this.AwaitingDispatchFromNamedPipe = 0;
                    }
                }

                if (this.CompiledShader == null)
                {
                    throw new AnomalousObjectException("I dont know how you got here but you shouldn't have but you managed to try and render a shape without a compiled property of: ", "CompiledShader");
                }
                if (DISPATCH_OPERATION_DIRECTIVE != "")
                {
                    Debugger.Log(1, "IPC_OBJECT_DIRECTIVES", $"RECIEVED_DIRECTIVE:{DISPATCH_OPERATION_DIRECTIVE}::VAL_MODIFCATION{DISPATCH_VALUE_OF_MODIFICATION}{Environment.NewLine} ");
                    switch (DISPATCH_OPERATION_DIRECTIVE)
                    {//TODO MAKE THE DIRECTIVE JUST CHANGE THE OBJECTS SIZE AS OTHERWISE IT WILL LOOK LIKE ITS RUBBER BANDING AFTER THE MAGIX MATRIX SETS IT BACK TO ITS PARAMS
                        case "SCALE":
                            try
                            {
                                this.Size = float.Parse(DISPATCH_VALUE_OF_MODIFICATION);
                            }
                            catch { }
                            break;
                        case "TRANSFORM": //CREATE THE VECTOR MATH PLEASE THERE WON't BE ANY MOVEMENTS IF IT DOESN't KNOW WHAT ITS DOING
                            if (!DISPATCH_VALUE_OF_MODIFICATION.Contains(':'))
                            {
                                Console.WriteLine("there will be no movement on this shape, Pipe didn't bring the : for seperating the vector componets correctly");
                                break;
                            }
                            var vecparts = DISPATCH_VALUE_OF_MODIFICATION.Split(':');
                            float posX;
                            float posY;
                            float posZ;
                            try
                            {
                                posX = float.Parse(vecparts[0]);
                            }
                            catch
                            {
                                posX = this.PosX;
                            }
                            try
                            {
                                posY = float.Parse(vecparts[1]);
                            }
                            catch
                            {
                                posY = this.PosY;
                            }
                            try
                            {
                                posZ = float.Parse(vecparts[2]);
                            }
                            catch
                            {
                                posZ = this.PosZ;
                            }
                            this.PosX = posX;
                            this.PosY = posY;
                            this.PosZ = posZ;
                            break;
                        case "ROTATE":
                            if (!DISPATCH_VALUE_OF_MODIFICATION.Contains(':'))
                            {
                                Console.WriteLine("there will be no rotation on this shape, Pipe didn't bring the : for seperating the vector componets correctly");
                                break;
                            }
                            var rotparts = DISPATCH_VALUE_OF_MODIFICATION.Split(':');
                            float rotX;
                            float rotY;
                            float rotZ;
                            try
                            {
                                rotX = float.Parse(rotparts[0]);
                            }
                            catch
                            {
                                rotX = this.RotX;
                            }
                            try
                            {
                                rotY = float.Parse(rotparts[1]);
                            }
                            catch
                            {
                                rotY = this.RotY;
                            }
                            try
                            {
                                rotZ = float.Parse(rotparts[2]);
                            }
                            catch
                            {
                                rotZ = this.RotZ;
                            }
                            this.RotX = rotX;
                            this.RotY = rotY;
                            this.RotZ = rotZ;

                            break;
                        case "STRETCH":
                            if (!DISPATCH_VALUE_OF_MODIFICATION.Contains(':'))
                            {
                                Console.WriteLine("there will be no Stretch on this shape, Pipe didn't bring the : for seperating the vector componets correctly");
                                break;
                            }
                            var StretchParts = DISPATCH_VALUE_OF_MODIFICATION.Split(":");
                            try
                            {
                                this.StrX = Convert.ToSingle(StretchParts[0]);
                            }
                            catch { }
                            try
                            {
                                this.StrY = Convert.ToSingle(StretchParts[1]);
                            }
                            catch { }
                            try
                            {
                                this.StrZ = Convert.ToSingle(StretchParts[2]);
                            }
                            catch { }
                            break;
                        default:
                            Console.WriteLine("DISPATCH_OPERATION_DIRECTIVE was called yet no object action supplied?");
                            break;
                    }
                    this.DISPATCH_TARGET_STRING = "";
                    this.AwaitingDispatchFromNamedPipe = 0;
                }
                var camera = this.Camera;
                //this is object culling when you get too far, going to add object property for setting the culling space on each object
                if (!(this.PosX < camera.Position.X + this.Culling_Distance && this.PosX > camera.Position.X - this.Culling_Distance) || !(this.PosY < camera.Position.Y + this.Culling_Distance && this.PosY > camera.Position.Y - this.Culling_Distance ) || !(this.PosZ < camera.Position.Z + this.Culling_Distance && this.PosZ > camera.Position.Z - this.Culling_Distance) || MainRenderer.IsGUICalled)
                {
                    return;
                }
                if (this.Glcontext == null)
                {
                    MainRenderer.EngineLogger.Log("CRITICAL FAILURE OF ENGINE GL IS NULL AT RENDER CALL WHICH CANNOT HAPPEN");
                    throw new AnomalousObjectException("You done royally fucked up to get here, you've managed to get past the check of having an OPENGL context on compiling and rending, we obviously can't draw the triangles if theres no place to put the fucking things");
                }
                var shader = this.CompiledShader;
                var texture = this.CompiledTexture;
                var GL = this.Glcontext;
                shader.use();
                if (texture != null)
                {
                    DebugMessages.PrintDebugMSG($"Attempting to bind Texture {this.TexturePath}",Print_debug_msg);
                    texture.Bind();
                }
                if (this.Reflective == 1)
                {
                    var diffusemap = this.CompiledDiffuseMap;
                    var specularmap = this.CompiledSpecularMap;
                    diffusemap.Bind(TextureUnit.Texture0);
                    specularmap.Bind(TextureUnit.Texture1);
                    shader.SetUniform("material.diffuse", 0);
                    shader.SetUniform("material.specular", 1);
                    shader.SetUniform("material.shininess", (float)this.Shininess);
                    var diffuseColor = new Vector3(0.5f);
                    var ambientColor = diffuseColor * new Vector3(0.2f);
                    shader.SetUniform("light.ambient", ambientColor);
                    shader.SetUniform("light.diffuse", diffuseColor);
                    shader.SetUniform("light.specular", new Vector3(1.0f, 1.0f, 1.0f));
                    shader.SetUniform("viewPos", camera.Position);
                }
                
                //only took 2 weeks to find this bug |:(
                DebugMessages.PrintDebugMSG("Applied magic to cube to make it actually show up",Print_debug_msg);
                var Magic_Matrix = Matrix4x4.Identity;
                Vector3 _Scaller_stretch = new(this.Size + this.StrX,this.Size + this.StrY,this.Size + this.StrZ);
                if (_Scaller_stretch != Vector3.One)
                {
                    Magic_Matrix *= Matrix4x4.CreateScale(_Scaller_stretch);
                }
                //this is the old code incase what im doing breaks anything and ctrl + z doesn't save me
                //if (this.Size != 1.0f)
                //{
                //    Magic_Matrix *= Matrix4x4.CreateScale(this.Size);
                //}
                //TODO: REWRITE THE SCALLER TO INSTEAD OF ATTEMPTING TO USE STRETCH DIRECTLY ON 1:1 2:2 or 3:3 SINCE THAT HAS BUGGY BEHAVOUR TO USE THE STRETCH AND SCALE I HAVE TO JUST MAKE A VEC3 OF 1 SCALE TO SCALE THE OBJECT ON EACH AXIS ( I pray this works or so help me god)
                //stretching is in-dev expect explosions ( works decently well this is starting to for some reason be easier and easier per thing i add to this)
                //if (this.StrX != 1.0f)
                //{
                //    Magic_Matrix[1,1] *= this.StrX;
                //}
                //if (this.StrY != 1.0f)
                //{
                //    Magic_Matrix[2,2] *= this.StrY;
                //}
                //if (this.StrZ != 1.0f)
                //{
                //    Magic_Matrix[3,3] *= this.StrZ;
                //}
                if (this.ShrX != 0.0f)
                {
                    Magic_Matrix[1,2] += this.ShrX;
                }
                if(this.ShrY != 0.0f) 
                { 
                    Magic_Matrix[2,1] += this.ShrY; 
                }
                //BEWARE TRYING TO SHEAR ON Z IT JUST MOVES THE CUBE INSTEAD OF SHEARING CORRECLTY IDK WHY THE MAXTRIX POS IS FOR ZX SHEARING
                if(this.ShrZ != 0.0f)
                {
                    Magic_Matrix[1,3] += this.ShrZ;
                }
                if (this.RotX != 0.0f)
                {
                    Magic_Matrix *= Matrix4x4.CreateRotationX(MathHelper.DegreesToRadians(this.RotX));
                }
                if (this.RotY != 0f )
                {
                    Magic_Matrix *= Matrix4x4.CreateRotationY(MathHelper.DegreesToRadians(this.RotY));
                }
                if (this.RotZ != 0f )
                {
                    Magic_Matrix *= Matrix4x4.CreateRotationZ(MathHelper.DegreesToRadians(this.RotZ));
                }
                if (this.Position != Vector3.Zero)
                {
                    Magic_Matrix *= Matrix4x4.CreateTranslation(this.Position);
                }
                if (this.IsModel)
                {
                    try
                    {
                        shader.SetUniform("uTexture0", 3);
                    }
                    catch
                    {
                        Debugger.Log(1,"Shaders","Shader Doesn't have a vertex+fragment pair that  implements uTexture0 correctly");
                        MainRenderer.EngineLogger.Log($"//WARN// the shape {this.ShapeName}-{this.ShapeNum} shader is missing the uniform uTexture0, this maybe be ignoreable if the model looks weird then it is probably this causing errors!!");
                    }
                    try
                    {
                        //This can fail, which is fine as its only used it things want to use it
                        if (!this.IgnoreTimeUniform) {
                            shader.SetUniform("time", (float)DateTime.Now.Ticks);
                        }
                    }
                    catch
                    {
                        this.IgnoreTimeUniform = true;
                    }
                    //TODO Make this avoid rendering verticies who are far away from the CULL distance to try and help the compute resources on the GPU
                    foreach (var mesh in this.Model.Meshes)
                    {
                        mesh.Bind();
                        shader.use();
                        texture.Bind();
                        try
                        {
                            shader.SetUniform("uTexture0", 0);
                        }
                        catch { }
                        try {
                            //This can fail, which is fine as its only used it things want to use it
                            if (!this.IgnoreTimeUniform) {
                                shader.SetUniform("time", (float)DateTime.Now.Ticks);
                            }
                        } catch { }
                        shader.SetUniform("uModel", Magic_Matrix);
                        shader.SetUniform("uView", camera.GetViewMatrix());
                        shader.SetUniform("uProjection", camera.GetProjectionMatrix());

                        GL.DrawArrays(PrimitiveType.Triangles,0,(uint)mesh.Verticies.Length);
                    }
                }
                else
                {
                    shader.SetUniform("uModel", Magic_Matrix);
                    shader.SetUniform("uView", camera.GetViewMatrix());
                    shader.SetUniform("uProjection", camera.GetProjectionMatrix());
                }
                this.ShapeMatrix = Magic_Matrix;
                if (this.Momentum != Vector3.Zero)
                {
                    this.PosX += this.MomentumX;
                    this.PosY += this.MomentumY;
                    this.PosZ += this.MomentumZ;
                }
                //this.Size += 1f;
                


                //if (GL == null) {
                //    MainRenderer.EngineLogger.Log("CRITICAL FAILURE OF ENGINE GL IS NULL AT RENDER CALL WHICH CANNOT HAPPEN");
                //    throw new AnomalousObjectException("You done royally fucked up to get here, you've managed to get past the check of having an OPENGL context on compiling and rending, we obviously can't draw the triangles if theres no place to put the fucking things");
                //} else {
                //    if (!this.IsModel) {
                //        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
                //    }
                //    DebugMessages.PrintDebugMSG("OPENGL is drawing the triangles",Print_debug_msg);
                //}
            }
            /// <summary>
            /// a method to make sure all Vector3 are updated to keep a seamless state if somewhere else needs to use this (like .render() that needs it for matrix math) and build/update the BB with the shape state
            /// </summary>
            public void GlueShape()
            {
                this.Position = new Vector3(this.PosX,this.PosY,this.PosZ);
                this.RotationVector = new Vector3(this.RotX,this.RotY,this.RotZ);
                this.Stretch = new Vector3(this.StrX,this.StrY,this.StrZ);
                this.Shear = new Vector3(this.ShrX,this.ShrY,this.ShrZ);
                this.Momentum = new Vector3(this.MomentumX,this.MomentumY,this.MomentumZ);
                if (this.BoundingBox != null) {
                    this.BoundingBox.Update(this.Position, this.Size, this.Stretch);
                }
                else
                {
                    this.BuildBoundingBox();
                }
                if (this.IsEffectedByGravity && this.Mass > 0 && this.Gravity > 0f && this.TerminalVelocity == 0)
                {
                    this.TerminalVelocity = Math.Sqrt((2 * this.Mass * this.Gravity));
                }
            }
            private void BuildBoundingBox()
            {
                this._BB_MIN_X = PosX - Size - StrX;
                this._BB_MIN_Y = PosY - Size - StrY;
                this._BB_MIN_Z = PosZ - Size - StrZ;
                this._BB_MAX_X = PosX + Size + StrX;
                this._BB_MAX_Y = PosY + Size + StrY;
                this._BB_MAX_Z = PosZ + Size + StrZ;
                this._Size_as_Vec3 = Vector3.One * Size;
                this.BoundingBox = new(new Vector3(_BB_MIN_X, _BB_MIN_Y, _BB_MIN_Z), new Vector3(_BB_MAX_X, _BB_MAX_Y, _BB_MAX_Z));
            }
            /// <summary>
            /// Modifies the shapes position based on player input
            /// </summary>
            /// <param name="speed"></param>
            /// <param name="axis"></param>
            public void UserLiveControl(float speed, string axis)
            {
                switch (axis)
                {
                    case "X":
                        this.PosX = speed / 10;
                    break;
                    case "Y":
                        this.PosY = speed / 10;
                    break;
                    case "Z":
                        this.PosZ = speed / 10;
                    break;
                }
            }
        }
        /// <summary>
        /// the AABB class is the bounding box of shapes
        /// </summary>
        public class BoundingBox
        {
            /// <summary>
            /// [Vector3] the minimum (starting point of the BB)
            /// </summary>
            public Vector3 Min { get; set; }
            /// <summary>
            /// [Vector3] the maximum (ending point of the BB)
            /// </summary>
            public Vector3 Max { get; set; }
            /// <summary>
            /// Builds the AABB bounding box
            /// </summary>
            /// <param name="min"></param>
            /// <param name="max"></param>
            public BoundingBox(Vector3 min, Vector3 max)
            {
                Min = min; Max = max;
            }
            /// <summary>
            /// Each frame the BB gets updated incase the shape moves by updating the min-max in accorance to stretch
            /// </summary>
            /// <param name="Position"></param>
            /// <param name="Size"></param>
            /// <param name="stretch"></param>
            public void Update(Vector3 Position, float Size, Vector3 stretch)
            {
                Vector3 adjustedscale = new(Size + stretch.X, Size + stretch.Y, Size + stretch.Z);
                Min = Position - adjustedscale / 2;
                Max = Position + adjustedscale / 2;
            }
            /// <summary>
            /// Detects if a shape collides with another shape in any axis
            /// </summary>
            /// <param name="Other"></param>
            /// <returns></returns>
            public Boolean Intersects(BoundingBox Other)
            {
                return (Min.X <= Other.Max.X && Max.X >= Other.Min.X) && 
                       (Min.Y <= Other.Max.Y && Max.Y >= Other.Min.Y) && 
                       (Min.Z <= Other.Max.Z && Max.Z >= Other.Min.Z);
            }
            /// <summary>
            /// Detects in that intersection was on the X axis so it can bounce off it that way
            /// </summary>
            /// <param name="Other"></param>
            /// <returns></returns>
            public Boolean IntersectsX(BoundingBox Other)
            {
                return Min.X <= Other.Max.X && Max.X >= Other.Min.X;
            }
            /// <summary>
            /// Detects in that intersection was on the Y axis so it can bounce off it that way
            /// </summary>
            /// <param name="Other"></param>
            /// <returns></returns>
            public Boolean IntersectsY(BoundingBox Other) 
            {
                return Min.Y <= Other.Max.Y && Max.Y >= Other.Min.Y;
            }
            /// <summary>
            /// Detects in that intersection was on the Z axis so it can bounce off it that way
            /// </summary>
            /// <param name="Other"></param>
            /// <returns></returns>
            public Boolean IntersectsZ(BoundingBox Other) { 
                return Min.Z <= Other.Max.Z && Max.Z >= Other.Min.Z;
            }
        }
    }
}
