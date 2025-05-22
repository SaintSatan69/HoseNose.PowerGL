using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public uint ShapeNumber { get; }
        /// <summary>
        /// The Property of the shape to retrive
        /// </summary>
        public string Property { get; }
        /// <summary>
        /// The X value of the property assuming its a vector property that doesn't serialize well
        /// </summary>
        public float ValueX { get; } = 0f;
        /// <summary>
        /// The Y value of the property assuming its a vector property that doesn't serialize well
        /// </summary>
        public float ValueY { get; } = 0f;
        /// <summary>
        /// The Z value of the property assuming its a vector property that doesn't serialize well
        /// </summary>
        public float ValueZ { get; } = 0f;
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
}
