using System.Numerics;
namespace HoseRenderer
{
    /// <summary>
    /// From the Silk.net Docs was used in 2d land and not used anymore but kept just in case
    /// </summary>
    public class Transform
    {
        public Vector3 Postion {  get; set; } = new Vector3(0,0,0);

        public float Scale { get; set; } = 1f;

        public Quaternion Rotation { get; set; } = Quaternion.Identity;

        public Matrix4x4 ViewMatrix => Matrix4x4.Identity * Matrix4x4.CreateFromQuaternion(Rotation) * Matrix4x4.CreateScale(Scale) * Matrix4x4.CreateTranslation(Postion);
    }
}
