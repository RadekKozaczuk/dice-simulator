using Unity.Mathematics;
using Unity.Physics;

namespace GameLogic
{
    static class Utils
    {
        internal static bool IsStopped(in PhysicsVelocity velocity, float epsilon = 0.1f)
        {
            float linearSpeedSq = math.lengthsq(velocity.Linear);
            float angularSpeedSq = math.lengthsq(velocity.Angular);

            return linearSpeedSq < epsilon * epsilon && angularSpeedSq < epsilon * epsilon;
        }
    }
}