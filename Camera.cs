using System.Numerics;

namespace HoseRenderer
{
    /// <summary>
    /// the camera class makes cameras what did you expect a puppy???
    /// </summary>
    public class Camera
    {
        /// <summary>
        /// The vec3 of the cameras position in 3d space
        /// </summary>
        public Vector3 Position { get; set; }
        /// <summary>
        /// the vec3 that stores the 3d space position of the front of the camera
        /// </summary>
        public Vector3 Front { get; set; }
        /// <summary>
        /// the vece that stores the up direction of the camera commonly just (0,1,0)
        /// </summary>
        public Vector3 Up { get; set; }
        /// <summary>
        /// the aspectratio of the camera usually unless the stars alligned should be 16:9
        /// </summary>
        public float AspectRatio { get; set; }
        /// <summary>
        /// the yaw of the camera to be modified by the mouse moving
        /// </summary>
        public float Yaw {  get; set; }
        /// <summary>
        /// the pitch of the camera to be modified by the mouse moving
        /// </summary>
        public float Pitch { get; set; }

        private float Zoom = 45f;

        /// <summary>
        /// camera contructor makes cameras
        /// </summary>
        /// <param name="position"></param>
        /// <param name="front"></param>
        /// <param name="up"></param>
        /// <param name="aspectRatio"></param>
        public Camera(Vector3 position,Vector3 front,Vector3 up, float aspectRatio) { 
            Position = position;
            Front = front;
            Up = up;
            AspectRatio = aspectRatio;
        }
        /// <summary>
        /// Modifies the camera zoom level from the mouse wheel
        /// </summary>
        /// <param name="zoomamount"></param>
        public void ModifyZoom(float zoomamount) {
            Zoom = Math.Clamp(Zoom - zoomamount, 1.0f, 45f);
        }
        /// <summary>
        /// moves the camera in 3d space when the WASD buttons are useda
        /// </summary>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
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
        /// <summary>
        /// gets the view of the camera as a 4x4 matrix from the pos, pos + front, and up
        /// </summary>
        /// <returns></returns>
        public Matrix4x4 GetViewMatrix()
        {
            return Matrix4x4.CreateLookAt(Position, Position + Front, Up);
        }
        /// <summary>
        /// gets the projection matrix for used on the rendering calls for lighing
        /// </summary>
        /// <returns></returns>
        public Matrix4x4 GetProjectionMatrix()
        {
            return Matrix4x4.CreatePerspectiveFieldOfView((MathHelper.DegreesToRadians(Zoom) * 2), AspectRatio, 0.1f, 1000.0f);
        }
    }
}
