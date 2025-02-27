﻿
using System.Numerics;

namespace HoseRenderer
{
    /// <summary>
    /// From the Silk.Net Docs, Used by the Model Objects
    /// </summary>
    public struct Vertex
    {
        public Vector3 Postion;
        public Vector3 Normal;
        public Vector3 Tangent;
        public Vector2 TexCoords;
        public Vector3 Bittangent;

        public const int MAX_BONE_INFLUENCE = 4;
        public int[] BoneIds;
        public float[] Weights;
    }
}
