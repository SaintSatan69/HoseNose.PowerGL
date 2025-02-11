using Silk.NET.Assimp;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace HoseRenderer
{
    /// <summary>
    /// From the Silk.Net Tutorials and works well so im not changing it unless needed
    /// </summary>
    public class Texture : IDisposable
    {
        private uint _handle;
        private GL _gl;
        /// <summary>
        /// The path to the image file used for the texture
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// the Type of texture defaults to TextureType.None
        /// </summary>
        public TextureType Type { get; set; }
        /// <summary>
        /// Builds the image Texture to use with the shape                
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="path"></param>
        /// <param name="type"></param>
        public unsafe Texture(GL gl, string path, TextureType type = TextureType.None)
        {
            _gl = gl;
            Path = path;
            Type = type;
            _handle = _gl.GenTexture();
            Bind();

            using (var img = Image.Load<Rgba32>(path)) {
                gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba8,(uint) img.Width, (uint)img.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, null);

                img.ProcessPixelRows(accessor =>
                {
                    for (int y = 0; y < accessor.Height; y++)
                    {
                        fixed (void* data = accessor.GetRowSpan(y))
                        {
                            gl.TexSubImage2D(TextureTarget.Texture2D, 0, 0, y, (uint)accessor.Width, 1, PixelFormat.Rgba, PixelType.UnsignedByte, data);
                        }
                    }
                });
            }
            SetParameters();
        }
        /// <summary>
        /// Builds a texture out of a byte span
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="data"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public unsafe Texture(GL gl, Span<byte> data, uint width, uint height) {
            _gl = gl;
            _handle = _gl.GenTexture();

            Bind();

            fixed (void* d = &data[0])
            {
                _gl.TexImage2D(TextureTarget.Texture2D,0,(int) InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte,d);
                SetParameters();
            }
        }
        private void SetParameters() {
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.ClampToEdge);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.ClampToEdge);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.LinearMipmapLinear);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel,0);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 8);
            _gl.GenerateMipmap(TextureTarget.Texture2D);
        }
        /// <summary>
        /// Tells OpenGL that this texture is being used at the specified texture spot on a specific shape
        /// </summary>
        /// <param name="textureslot"></param>
        public void Bind(TextureUnit textureslot = TextureUnit.Texture0)
        {
            _gl.ActiveTexture(textureslot);
            _gl.BindTexture(TextureTarget.Texture2D, _handle);
        }
        /// <summary>
        /// Disposes of the texture
        /// </summary>
        public void Dispose()
        {
            _gl.DeleteTexture(_handle);
        }
    }
}
