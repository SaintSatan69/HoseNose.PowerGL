

namespace HoseRenderer
{
    /// <summary>
    /// Does Math that the computer wants such as rotations needing to be in radian and I prefer degrees
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// coverts the [float] degress into it radian amount
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns>a float of the radian value of the degrees called</returns>
        public static float DegreesToRadians(float degrees) { 
            return (float) ((Math.PI / 180f) * degrees);
        }
    }
}
