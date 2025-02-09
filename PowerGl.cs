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
        //<summary>
        //This Class Object is the object representing a singular shape/modle in 3D space
        //</summary>
        public class Shape
        {
            //<summary>
            //The Shape name, not super important but if you want to include it to keep tabs on it from powershell thats acceptable
            //</summary>
            [JsonInclude]
            [JsonRequired]
            public string ShapeName { get; private set; } = "Cube";
            [JsonInclude]
            [JsonRequired]
            public Vector3 Position { get; set; } = new Vector3(0f, 0f, 0f);
            [JsonInclude]
            [JsonRequired]
            public float PosX { get; set; } = 0f;
            [JsonInclude]
            [JsonRequired]
            public float PosY { get; set; } = 0f;
            [JsonInclude]
            [JsonRequired]
            public float PosZ { get; set; } = 0f;
            [JsonInclude]
            [JsonRequired]
            public string? TexturePath { get; set; }
            [JsonInclude]
            [JsonRequired]
            public string ShaderVertPath { get; set; }
            [JsonInclude]
            [JsonRequired]
            public string FragementPath { get; set; }
            [JsonInclude]
            [JsonRequired]
            public uint LightEmitting { get; private set; } = 0;
            [JsonInclude,JsonRequired]
            public Boolean IsModel { get; private set; } = false;
            [JsonInclude]
            [JsonRequired]
            public string ModelPath { get; private set; } = (MainRenderer.Application_Directory + "\\Model\\cube.model" );
            [JsonIgnore]
            public Model? Model { get; private set; }
            [JsonInclude]
            [JsonRequired]
            public uint ShapeNum { get; private set; }
            [JsonInclude]
            [JsonRequired]
            public uint? Reflective { get; private set; } = 0;
            [JsonInclude]
            [JsonRequired]
            public string? DiffuseMapPath { get; private set; }
            [JsonInclude]
            [JsonRequired]
            public string? SpecularMapPath { get; private set; }
            public uint ShaderCompilationCompleted { get; private set; }
            public Shader? CompiledShader { get; private set; }
            public Texture? CompiledTexture { get; set; }
            private Texture? CompiledDiffuseMap { get; set; }
            private Texture? CompiledSpecularMap { get; set; }
            [JsonInclude]
            [JsonRequired]
            public Vector3? RotationVector { get; set; }
            [JsonInclude]
            [JsonRequired]
            public float RotX { get; set; } = 0f;
            [JsonInclude]
            [JsonRequired]
            public float RotY { get; set; } = 0f;
            [JsonInclude]
            [JsonRequired]
            public float RotZ { get; set; } = 0f;
            public float Size { get; set; } = 1f;
            public Vector3 Stretch { get; set; } = new(1.0f, 1.0f, 1.0f);
            [JsonInclude]
            [JsonRequired]
            public float StrX { get; set; } = 1f;
            [JsonInclude]
            [JsonRequired]
            public float StrY { get; set; } = 1f;
            [JsonInclude]
            [JsonRequired]
            public float StrZ { get; set; } = 1f;
            public Vector3 Shear { get; set; } = new(0f,0f,0f);
            [JsonInclude]
            [JsonRequired]
            public float ShrX { get; set; } = 0f;
            [JsonInclude]
            [JsonRequired]
            public float ShrY { get; set; } = 0f;
            [JsonInclude]
            [JsonRequired]
            public float ShrZ { get; set; } = 0f;
            [JsonInclude]
            [JsonRequired]
            public float BoingFactor { get; set; } = 1f;
            public float? Shininess { get; set; }
            public GL? Glcontext { get; set; }
            public Camera? Camera { get; set; }
            public uint HasCollison { get; set; } = 0;
            public Matrix4x4 ShapeMatrix { get; set; } = Matrix4x4.Identity;
            public int AwaitingDispatchFromNamedPipe { get; set; } = 0;
            public Boolean IsEffectedByGravity { get; set; }
            public Vector3 Momentum { get; set; } = new(0.0f, 0.0f, 0.0f);
            public float MomentumX { get; set; } = 0.0f;
            public float MomentumY { get; set; } = 0.0f;
            public float MomentumZ { get; set; } = 0.0f;
            public int FrameTimeOfLastBoing { get; set; }
            public BoundingBox BoundingBox { get; set; }
            public float Gravity { get; set; } = 0.1f;
            public string? DISPATCH_TARGET_STRING { get; set; }
            public int Debug { get; set ; } = 0;

            private Vector3 _Size_as_Vec3 {  get; set; }
            private float _BB_MIN_X { get; set; }
            private float _BB_MIN_Y { get; set; }
            private float _BB_MIN_Z { get; set; }
            private float _BB_MAX_X { get; set; }
            private float _BB_MAX_Y { get; set; }
            private float _BB_MAX_Z { get; set; }

            public double TerminalVelocity { get; private set; }
            public float Mass { get; private set; } = 1f;
            public uint ClampedToFloor { get; set; } = 0;
            [JsonInclude]
            [JsonRequired]
            public uint Player_moveable { get; set; }
            [JsonInclude]
            [JsonRequired]
            public uint Player_scheme { get; set; }

            public float Culling_Distance { get; set; } = 200f;
            public Shape(string shapename, Vector3 position, uint shapenum, uint reflective, uint glowy, string? shaderpath, string? fragmentpath, string? texturepath, Vector3 rotationvector, float size, Vector3 stretch, Vector3 shear, uint Collison,Vector3 momentum,float boingfactor,Boolean iseffectedbygravity = false, uint player_control = 0, uint player_num = 1, Boolean ismodel = false,string modelpath = "")
            {
                Console.WriteLine("Creating new instance of Shape object");
                ShapeName = shapename;
                Position = position;
                ShapeNum = shapenum;
                Size = size;
                HasCollison = Collison;
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
                        FragementPath = ".\\Shaders\\shader.frag";
                    }
                    else
                    {
                        FragementPath = ".\\Shaders\\lighting.frag";
                    }
                }
                else
                {
                    FragementPath = fragmentpath;
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
                BoingFactor = boingfactor;
                TerminalVelocity = Math.Sqrt((2 * Mass * Gravity));
                IsEffectedByGravity = iseffectedbygravity;
                IsModel = ismodel;
                ModelPath = modelpath;


                Player_moveable = player_control;
                Player_scheme = player_num;
            }
            public Shape() { }
            public void CompileShader()
            {
                Console.WriteLine($"running shader compilation on shape num {this.ShapeNum}");
                if (this.Glcontext == null)
                {
                    throw new InvalidOperationException("Cannot compile shaders when there is no OPENGL context for which to bind to.");
                }
                this.CompiledShader = new Shader(this.Glcontext, this.ShaderVertPath, this.FragementPath);
                this.ShaderCompilationCompleted = 1;
                Console.WriteLine($"Shader Compilation Complete on shape num {this.ShapeNum}");
            }
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
                    }
                }
            }
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
            public void CompileModelMesh()
            {
                if (this.Glcontext != null) {
                    this.Model = new Model(this.Glcontext, this.ModelPath);
                }
                else
                {
                    return ;
                }
            }
            //TODO MAKE THE MATH THAT HANDLES OBJECTS IN RELATION TO EACHOTHER USING A GLOBAL LIGHT ARRAY SO THAT YOU CAN DO THE MATRIX MATH AS TO HOW TO MAKE THING BRIGHT
            //<summary>
            //Contains all the things that need to happen to the shape to get it to rendering on the screen.
            //</summary>
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
                        default:
                            Console.WriteLine("DISPATCH_OPERATION_DIRECTIVE was called yet no object action supplied?");
                            break;
                    }
                }
                var camera = this.Camera;
                //this is object culling when you get too far, going to add object property for setting the culling space on each object
                if (!(this.PosX < camera.Position.X + this.Culling_Distance && this.PosX > camera.Position.X - this.Culling_Distance) || !(this.PosY < camera.Position.Y + this.Culling_Distance && this.PosY > camera.Position.Y - this.Culling_Distance ) || !(this.PosZ < camera.Position.Z + this.Culling_Distance && this.PosZ > camera.Position.Z - this.Culling_Distance))
                {
                    return;
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
                    shader.SetUniform("uTexture0", 3);
                    foreach (var mesh in this.Model.Meshes)
                    {
                        mesh.Bind();
                        shader.use();
                        texture.Bind();
                        shader.SetUniform("uTexture0", 0);
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
                


                if (GL == null) {
                    throw new AnomalousObjectException("You done royally fucked up to get here, you've managed to get past the check of having an OPENGL context on compiling and rending, we obviously can't draw the triangles if theres no place to put the fucking things");
                } else {
                    if (!this.IsModel) {
                        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
                    }
                    DebugMessages.PrintDebugMSG("OPENGL is drawing the triangles",Print_debug_msg);
                    this.DISPATCH_TARGET_STRING = "";
                    this.AwaitingDispatchFromNamedPipe = 0;
                }
            }
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
        public class BoundingBox
        {
            public Vector3 Min { get; set; }
            public Vector3 Max { get; set; }
            public BoundingBox(Vector3 min, Vector3 max)
            {
                Min = min; Max = max;
            }
            public void Update(Vector3 Position, float Size, Vector3 stretch)
            {
                Vector3 adjustedscale = new(Size + stretch.X, Size + stretch.Y, Size + stretch.Z);
                Min = Position - adjustedscale / 2;
                Max = Position + adjustedscale / 2;
            }
            public Boolean Intersects(BoundingBox Other)
            {
                return (Min.X <= Other.Max.X && Max.X >= Other.Min.X) && 
                       (Min.Y <= Other.Max.Y && Max.Y >= Other.Min.Y) && 
                       (Min.Z <= Other.Max.Z && Max.Z >= Other.Min.Z);
            }
            public Boolean IntersectsX(BoundingBox Other)
            {
                return Min.X <= Other.Min.X && Max.X >= Other.Max.X;
            }
            public Boolean IntersectsY(BoundingBox Other) 
            {
                return Min.Y <= Other.Min.Y && Max.Y >= Other.Max.Y;
            }
            public Boolean IntersectsZ(BoundingBox Other) { 
                return Min.Z <= Other.Min.Z && Max.Z >= Other.Max.Z;
            }
        }
    }
}
