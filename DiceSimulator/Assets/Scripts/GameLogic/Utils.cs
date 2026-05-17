using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

namespace GameLogic
{
    static class Utils
    {
        internal static bool IsStopped(in PhysicsVelocity velocity, float epsilon = 0.2f)
        {
            Debug.LogError("Check performed");
            float linearSpeedSq = math.lengthsq(velocity.Linear);
            float angularSpeedSq = math.lengthsq(velocity.Angular);

            return linearSpeedSq < epsilon * epsilon && angularSpeedSq < epsilon * epsilon;
        }
    }
}