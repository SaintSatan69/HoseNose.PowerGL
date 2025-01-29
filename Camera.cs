using System.Numerics;

namespace HoseRenderer
{
    public class Camera
    {
        public Vector3 Position { get; set; }
        public Vector3 Front { get; set; }
        public Vector3 Up { get; set; }
        public float AspectRatio { get; set; }
        public float Yaw {  get; set; }
        public float Pitch { get; set; }

        private float Zoom = 45f;

        public Camera(Vector3 position,Vector3 front,Vector3 up, float aspectRatio) { 
            Position = position;
            Front = front;
            Up = up;
            AspectRatio = aspectRatio;
        }
        public void ModifyZoom(float zoomamount) {
            Zoom = Math.Clamp(Zoom - zoomamount, 1.0f, 45f);
        }
        public void ModifyDirection(float xOffset, float yOffset) {
            Yaw += xOffset;
            Pitch -= yOffset;

            Pitch = Math.Clamp(Pitch, -89f, 89f);

            var cameraDirection = Vector3.Zero;
            cameraDirection.X = MathF.Cos(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
            cameraDirection.Y = MathF.Sin(MathHelper.DegreesToRadians(Pitch));
            cameraDirection.Z = MathF.Sin(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));

            Front = Vector3.Normalize(cameraDirection);
        }
        public Matrix4x4 GetViewMatrix()
        {
            return Matrix4x4.CreateLookAt(Position, Position + Front, Up);
        }
        public Matrix4x4 GetProjectionMatrix()
        {
            return Matrix4x4.CreatePerspectiveFieldOfView((MathHelper.DegreesToRadians(Zoom) * 2), AspectRatio, 0.1f, 1000.0f);
        }
    }
}
