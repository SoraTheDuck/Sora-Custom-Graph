using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sora_Ults;

namespace Sora_Ults
{
    public class SO_Calc
    {
        public void RecalculateConstants(out float W, out float D, out float K1, out float K2, out float K3, float Z,
            float F, float R)
        {
            W = 2 * Mathf.PI * F;
            D = W * Mathf.Sqrt(Mathf.Abs((Z * Z) - 1));
            K1 = Z / (Mathf.PI * F);
            K2 = 1 / (W * W);
            K3 = R * Z / W;
        }

        protected void OnNewValue(ref SecondOrderState state, float deltaTime, float targetValue, float targetVelocity)
        {
            // values to prevent jitter & instability
            // k1Stable is only calculated if usePoleZeroMatching is enabled.
            float k1Stable;
            float k2Stable;

            bool assumeK1Stable = state.W * deltaTime < state.Z;

            if (assumeK1Stable)
            {
                // clamp k2 to guarantee stability without jitter
                k1Stable = state.K1;
                k2Stable = Mathf.Max(state.K2, (deltaTime * deltaTime / 2) + (deltaTime * state.K1 / 2),
                    deltaTime * state.K1);
            }
            else
            {
                // use pole matching when the system is very fast
                float t1 = Mathf.Exp(-state.Z * state.W * deltaTime);
                float alpha = 2 * t1 * (state.Z <= 1
                    ? Mathf.Cos(deltaTime * state.D)
                    : (float)System.Math.Cosh(deltaTime * state.D));
                float beta = t1 * t1;
                float t2 = deltaTime / (1 + beta - alpha);
                k1Stable = (1 - beta) * t2;
                k2Stable = deltaTime * t2;
            }

            Integrate(ref state, deltaTime, targetValue, targetVelocity, k1Stable, k2Stable);
        }

        protected static void Integrate(ref SecondOrderState state,
            float deltaTime,
            float targetValue,
            float targetVelocity,
            float? k1Override = null,
            float? k2Override = null
        )
        {
            float k1 = k1Override ?? state.K1;
            float k2 = k2Override ?? state.K2;

            // integrate position by velocity
            state.CurrentValue += (deltaTime * state.CurrentVelocity);

            // integrate velocity by acceleration
            state.CurrentVelocity += (deltaTime * (targetValue + (state.K3 * targetVelocity) - state.CurrentValue -
                                                   (k1 * state.CurrentVelocity)) / k2);
        }

        #region targetVelocity update
        public float UpdateStrategy(ref SecondOrderState state, float deltaTime, float targetValue,
            float? targetVelocity)
        {
            float targetVelocityActual = UpdateVelocity(ref state, deltaTime, targetValue, targetVelocity);
            OnNewValue(ref state, deltaTime, targetValue, targetVelocityActual);
            return state.CurrentValue;
        }

        private static float UpdateVelocity(ref SecondOrderState state,
            float deltaTime,
            float targetValue,
            float? targetVelocityOrNull
        )
        {
            float estimatedVelocity = (targetValue - state.PreviousTargetValue) / deltaTime;
            float xd = targetVelocityOrNull.GetValueOrDefault(estimatedVelocity);
            state.PreviousTargetValue = targetValue;
            return xd;
        }

        #endregion
    }
}
