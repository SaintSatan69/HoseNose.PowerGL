using Silk.NET.OpenGL;
namespace HoseRenderer
{
    /// <summary>
    /// From the Silk.net tutorials and since it works im not touching it
    /// </summary>
    /// <typeparam name="TDataType"></typeparam>
    public class BufferObject<TDataType> : IDisposable
        where TDataType : unmanaged
    {
        private uint _handle;
        private BufferTargetARB _bufferType;
        private GL _gl;
        /// <summary>
        /// Something something a buffer that stores vertices and indicies thats used by the video buffer on the video card
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="data"></param>
        /// <param name="bufferType"></param>
        public unsafe BufferObject(GL gl, Span<TDataType> data, BufferTargetARB bufferType)
        {
            _gl = gl;
            _bufferType = bufferType;

            _handle = _gl.GenBuffer();

            Bind();

            fixed (void* d = data)
            {
                _gl.BufferData(bufferType, (nuint) (data.Length * sizeof(TDataType)), d, BufferUsageARB.StaticDraw);
            }
        }
        /// <summary>
        /// Tells OpenGL that this is the current buffer to read to handle some API calls
        /// </summary>
        public void Bind() { 
            _gl.BindBuffer(_bufferType, _handle);
        }
        /// <summary>
        /// Disposes of the buffer
        /// </summary>
        public void Dispose()
        {
            _gl.DeleteBuffer(_handle);
        }
    }
}
