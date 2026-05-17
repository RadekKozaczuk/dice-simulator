using Unity.Mathematics;
using Unity.Physics;

namespace GameLogic
{
    public class Utils
    {
        public static bool IsStopped(in PhysicsVelocity velocity, float epsilon = 0.01f)
        {
            float linearSpeedSq = math.lengthsq(velocity.Linear);
            float angularSpeedSq = math.lengthsq(velocity.Angular);

            return linearSpeedSq < epsilon * epsilon && angularSpeedSq < epsilon * epsilon;
        }
    }
}