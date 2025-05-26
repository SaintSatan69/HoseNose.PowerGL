using HoseRenderer.PowerGl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HoseRenderer
{
    /// <summary>
    /// The Class object that is the definition of the content between the server (engine) and the client (powershell), to keep a sane shema between the 2
    /// </summary>
    public class HttpObject
    {
        /// <summary>
        /// The ShapeNumber for which to retrive or modify
        /// </summary>
        [JsonRequired,JsonInclude]
        public uint ShapeNumber { get; set; }
        /// <summary>
        /// The Property of the shape to retrive
        /// </summary>
        [JsonRequired,JsonInclude]
        public string Property { get; set; } = "If You See This Serialization Has Failed";
        /// <summary>
        /// The X value of the property assuming its a vector property that doesn't serialize well
        /// </summary>
        [JsonRequired,JsonInclude]
        public float ValueX { get; set; } = 0f;
        /// <summary>
        /// The Y value of the property assuming its a vector property that doesn't serialize well
        /// </summary>
        [JsonRequired,JsonInclude]
        public float ValueY { get; set; } = 0f;
        /// <summary>
        /// The Z value of the property assuming its a vector property that doesn't serialize well
        /// </summary>
        [JsonRequired,JsonInclude]
        public float ValueZ { get; set; } = 0f;
        /// <summary>
        /// The .ctor for making a payload object for use in the HTTP API
        /// </summary>
        /// <param name="shapenumber"></param>
        /// <param name="property"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public HttpObject(uint shapenumber, string property, float x, float y, float z)
        {
            ShapeNumber = shapenumber;
            Property = property;
            ValueX = x;
            ValueY = y;
            ValueZ = z;
        }
        /// <summary>
        /// Paramless .ctor json serialization in use with the http API that can be exposed in the config
        /// </summary>
        public HttpObject() { }
    }
    /// <summary>
    /// For sending all the shapes and all their needed properties over JSON in a more compact form (NOT MENT FOR NORMAL USE JUST THE HTTP API)
    /// </summary>
    public class ShapeHttpObjectCollectionEntry
    {
        public string ShapeName{ get; set; }
        public uint ShapeNum { get; set; }
        public float PosX  { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }
        public float RotX{ get; set; }
        public float RotY{ get; set; }
        public float RotZ{ get; set; }
        public float StrX {  get; set; }
        public float StrY { get; set; }
        public float StrZ{ get; set; }
        public float ShrX { get; set; }
        public float ShrY { get; set; }
        public float ShrZ{ get; set; }
        public float MomentumX { get; set; }
        public float MomentumY { get; set; }
        public float MomentumZ { get; set; }
        public float Restitution {  get; set; }

        public ShapeHttpObjectCollectionEntry(Shape shape)
        {
            ShapeName = shape.ShapeName;
            ShapeNum = shape.ShapeNum;
            PosX = shape.PosX;
            PosY = shape.PosY;
            PosZ = shape.PosZ;
            RotX = shape.RotX;
            RotY = shape.RotY;
            RotZ = shape.RotZ;
            StrX = shape.StrX;
            StrY = shape.StrY;
            StrZ = shape.StrZ;
            ShrX = shape.ShrX;
            ShrY = shape.ShrY;
            ShrZ = shape.ShrZ;
            MomentumX = shape.MomentumX;
            MomentumY = shape.MomentumY;
            MomentumZ = shape.MomentumZ;
            Restitution = shape.Restitution;
        }
    }
}
