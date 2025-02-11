using Silk.NET.OpenGL;

namespace HoseRenderer
{
    /// <summary>
    /// the array of vertecies fead to OpenGL for rendering
    /// </summary>
    /// <typeparam name="TVertexType"></typeparam>
    /// <typeparam name="TIndexType"></typeparam>
    public class VertexArrayObject<TVertexType, TIndexType> : IDisposable
        where TVertexType : unmanaged
        where TIndexType : unmanaged
    {
        private uint _handle;
        private GL _gl;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="vbo"></param>
        /// <param name="ebo"></param>
        public VertexArrayObject(GL gl, BufferObject<TVertexType> vbo, BufferObject<TIndexType> ebo) { 
            _gl = gl;

            _handle = _gl.GenVertexArray();
            Bind();
            vbo.Bind();
            ebo.Bind();
        }
        /// <summary>
        /// Modifies attributes at the index (lookup the OpenGL docs)
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <param name="type"></param>
        /// <param name="vertexSize"></param>
        /// <param name="offSet"></param>
        public unsafe void VertexAttributePointer(uint index, int count, VertexAttribPointerType type, uint vertexSize, int offSet)
        {
            _gl.VertexAttribPointer(index,count, type,false , vertexSize * (uint) sizeof(TVertexType),(void*) (offSet * sizeof(TVertexType)));
            _gl.EnableVertexAttribArray(index);
        }
        /// <summary>
        /// Tells OpenGL to use this vertexArray currently
        /// </summary>
        public void Bind()
        {
            _gl.BindVertexArray( _handle );
        }
        /// <summary>
        /// Disposes of this vertex array
        /// </summary>
        public void Dispose() { 
            _gl.DeleteVertexArray(_handle);
        }
    }
}
