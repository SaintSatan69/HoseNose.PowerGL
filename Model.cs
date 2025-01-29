using Silk.NET.Assimp;
using Silk.NET.OpenGL;
using System.Numerics;
using AssimpMesh = Silk.NET.Assimp.Mesh;
namespace HoseRenderer
{
    public class Model : IDisposable
    {
        public Model(GL gl, string path, bool gamma = false) {
            var assimp = Assimp.GetApi();
            Internalassimp = assimp;
            InternalGL = gl;
            LoadModel(path);
        }
        private readonly GL InternalGL;
        private Assimp Internalassimp;
        private List<Texture> InternalTexturesLoaded = new List<Texture>();
        public string Directory { get; protected set; } = string.Empty;
        public List<Mesh> Meshes { get; protected set; } = new List<Mesh>();
        private unsafe void LoadModel(string path)
        {
            var scene = Internalassimp.ImportFile(path, (uint)PostProcessSteps.Triangulate);

            if (scene == null || scene ->MFlags == Assimp.SceneFlagsIncomplete || scene ->MRootNode == null)
            {
                var error = Internalassimp.GetErrorStringS();
                throw new Exception(error);
            }

            Directory = path;

            ProcessNode(scene ->MRootNode,scene);
        }
        private unsafe void ProcessNode(Node* node, Scene* scene) { 
            for (var i = 0; i < node->MNumMeshes; i++)
            {
                var mesh = scene->MMeshes[node->MMeshes[i]];
                Meshes.Add(ProcessMesh(mesh, scene));
            }
            for (var i = 0;i < node->MNumChildren; i++)
            {
                ProcessNode(node->MChildren[i], scene);
            }
        }
        private unsafe Mesh ProcessMesh(AssimpMesh* mesh, Scene* scene) { 

            List<Vertex> vertices = new List<Vertex> ();
            List<uint> indices = new List<uint> ();
            List<Texture> textures = new List<Texture> ();

            for (uint i = 0; i < mesh->MNumVertices; i++) 
            { 
                Vertex vertex = new Vertex();
                vertex.BoneIds = new int[Vertex.MAX_BONE_INFLUENCE];
                vertex.Weights = new float[Vertex.MAX_BONE_INFLUENCE];

                vertex.Postion = mesh->MVertices[i];

                if (mesh->MNormals != null) { 
                    vertex.Normal = mesh->MNormals[i];
                }
                if (mesh->MTangents != null)
                {
                    vertex.Tangent = mesh->MTangents[i];
                }
                if (mesh->MBitangents != null)
                {
                    vertex.Bittangent = mesh->MBitangents[i];
                }
                if (mesh->MTextureCoords[0] != null)
                {
                    Vector3 texcoord3 = mesh->MTextureCoords[0][i];
                    vertex.TexCoords = new Vector2(texcoord3.X,texcoord3.Y);
                }
                vertices.Add(vertex);
            }

            for (uint i = 0;i < mesh->MNumFaces; i++)
            {
                Face face = mesh->MFaces[i];
                for (uint j = 0;j < face.MNumIndices; j++)
                {
                    indices.Add(face.MIndices[j]);
                }
            }

            Material* material = scene->MMaterials[mesh->MMaterialIndex];
            var diffuseMaps = LoadMaterialTexture(material, TextureType.Diffuse, "texture_diffuse");
            if (diffuseMaps.Any())
                textures.AddRange(diffuseMaps);
            var specularmaps = LoadMaterialTexture(material, TextureType.Specular, "texture_specular");
            if(specularmaps.Any())
                textures.AddRange(specularmaps);
            var normalmaps = LoadMaterialTexture(material, TextureType.Height, "texture_normal");
            if(normalmaps.Any())
                textures.AddRange(normalmaps);
            var heightmaps = LoadMaterialTexture(material, TextureType.Ambient, "texture_height");
            if(heightmaps.Any())
                textures.AddRange(heightmaps);


            var result = new Mesh(InternalGL,BuildVertices(vertices),BuildIndices(indices) ,textures);
            return result;
        }
        private unsafe List<Texture> LoadMaterialTexture(Material* mat, TextureType type, string typeName) {
            var texturecount = Internalassimp.GetMaterialTextureCount(mat, type);
            List<Texture> textures = new List<Texture>();
            for (uint i = 0; i < texturecount; i++) {
                AssimpString path;
                Internalassimp.GetMaterialTexture(mat, type, i, &path, null, null, null, null, null, null);
                bool skip = false;
                for (int j = 0; j < InternalTexturesLoaded.Count; j++) {
                    if (InternalTexturesLoaded[j].Path == path) { 
                        textures.Add(InternalTexturesLoaded[j]);
                        skip = true;
                        break;
                    }
                }
                if (!skip) {
                    var texture = new Texture(InternalGL, Directory, type);
                    texture.Path = path;
                    textures.Add(texture);
                    InternalTexturesLoaded.Add(texture);
                }
            }
            return textures;
        }
        private float[] BuildVertices(List<Vertex> vertexcollection)
        {
            var vertices = new List<float>();
            foreach (var vertex in vertexcollection)
            {
                vertices.Add(vertex.Postion.X);
                vertices.Add(vertex.Postion.Y);
                vertices.Add(vertex.Postion.Z);
                vertices.Add(vertex.TexCoords.X);
                vertices.Add(vertex.TexCoords.Y);
            }
            return vertices.ToArray();
        }
        private uint[] BuildIndices(List<uint> indices) { 
            return indices.ToArray();
        }
        public void Dispose() {
            foreach (var mesh in Meshes) { 
                mesh.Dispose();
            }
            InternalTexturesLoaded = null;
        }
    }
}
