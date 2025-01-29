using Silk.NET.OpenGL;
using System.Collections.Generic;
namespace HoseRenderer
{
    public class Mesh : IDisposable
    {
        public Mesh(GL gl, float[] vertices, uint[] indices, List<Texture> textures)
        {
            GL = gl;
            Verticies = vertices;
            Indices = indices;
            Textures = textures;
            SetupMesh();
        }
        public float[] Verticies { get; private set; }
        public uint[] Indices { get; private set; }
        public IReadOnlyList<Texture> Textures { get; private set; }
        public VertexArrayObject<float, uint> VAO {  get; set; }
        public BufferObject<float> VBO { get; set; }
        public BufferObject<uint> EBO { get; set; }
        public GL GL { get; private set; }
        public unsafe void SetupMesh()
        {
            EBO = new BufferObject<uint>(GL, Indices, BufferTargetARB.ElementArrayBuffer);
            VBO = new BufferObject<float>(GL,Verticies, BufferTargetARB.ArrayBuffer);
            VAO = new VertexArrayObject<float, uint>(GL, VBO, EBO);
            VAO.VertexAttributePointer(0,3,VertexAttribPointerType.Float,5,0);
            VAO.VertexAttributePointer(1,2,VertexAttribPointerType.Float,5,3);
        }
        public void Bind()
        {
            VAO.Bind();
        }
        public void Dispose() {
            Textures = null;
            VAO.Dispose();
            VBO.Dispose();
            EBO.Dispose();
        }
    }
}
