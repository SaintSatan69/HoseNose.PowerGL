using System.IO;
using System.Numerics;
using Silk.NET.OpenGL;

namespace HoseRenderer
{
    public class Shader : IDisposable
    {
        private uint _handle;
        private GL _gl;

        public Shader(GL gl, string VertexPath, string FragmentPath) { 
            _gl = gl;

            uint vertex = LoadShader(ShaderType.VertexShader, VertexPath);
            uint fragment = LoadShader(ShaderType.FragmentShader, FragmentPath);
            _handle = _gl.CreateProgram();
            _gl.AttachShader(_handle, vertex);
            _gl.AttachShader(_handle, fragment);
            _gl.LinkProgram(_handle);
            _gl.GetProgram(_handle, GLEnum.LinkStatus, out var status);
            Console.WriteLine($"Open GL handle {_handle}, Vertexblob {vertex}, Fragment {fragment}, Open GL errors: {_gl.GetError()}, Status {status}");
            if(status == 0)
            {
                throw new Exception($"Program had a stronk with error: {_gl.GetProgramInfoLog(_handle)}");
            }
            _gl.DetachShader(_handle, vertex);
            _gl.DetachShader(_handle, fragment);
            _gl.DeleteShader(vertex);
            _gl.DeleteShader(fragment);

        }

        public void use()
        {
            _gl.UseProgram(_handle);
        }
        public void SetUniform(string name, int value) { 
            int location = _gl.GetUniformLocation(_handle, name);
            if (location == -1) {
                throw new Exception($"{name} uniform not found on shader.");
            }
            _gl.Uniform1(location, value);
        }
        public unsafe void SetUniform(string name, Matrix4x4 value) {
            int location = _gl.GetUniformLocation(_handle, name);

            if (location == -1) {
                throw new Exception($"{name} uniform not found on shader.");
            }
            _gl.UniformMatrix4(location,1,false, (float*) &value);
        }
        public void SetUniform(string name, float value)
        {
            int location = _gl.GetUniformLocation(_handle, name);
            if (location == -1)
            {
                throw new Exception($"{name} uniform not found on shader.");
            }
            _gl.Uniform1(location, value);
        }
        public void SetUniform(string name, Vector3 value) { 
            int location = _gl.GetUniformLocation (_handle, name);
            if (location == -1) {
                throw new Exception($"{name} uniform not found on shader");
            }
            _gl.Uniform3(location, value.X,value.Y,value.Z);
        }
        public void Dispose()
        {
            _gl.DeleteProgram(_handle);
        }
        private uint LoadShader(ShaderType type, string path)
        {
            string src = File.ReadAllText(path);
            uint handle = _gl.CreateShader(type);
            _gl.ShaderSource(handle, src);
            _gl.CompileShader(handle);
            string infolog = _gl.GetShaderInfoLog(handle);
            if (!String.IsNullOrWhiteSpace(infolog))
            {
                throw new Exception($"Error compiling shader of type {type}, failed with error {infolog}");
            }
            Console.WriteLine($"Handle int: {handle}, From File path: {path} GL errors {_gl.GetError()}");
            return handle;
        }
    }
}
