using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sora_Ults
{
    public abstract class ISecondOrderSystem
    {
        public virtual void RecalculateConstants(ref SecondOrderState state, float initialValue)
        {
            // Default implementation (None)
        }

        public virtual void OnNewValue(ref SecondOrderState state, float deltaTime, float targetValue, float targetVelocity)
        {
            // Default implementation (None)
        }
        
        internal static void DefaultConstantCalculation(ref SecondOrderState state, float initialValue)
        {
            state.K1 = state.Z / (Mathf.PI * state.F);

            float tauFrequency = (2 * Mathf.PI * state.F);

            state.K2 = 1 / (tauFrequency * tauFrequency);
            state.K3 = state.R / tauFrequency;

            state.MaximumTimeStep = 0.8f * (Mathf.Sqrt((4 * state.K2) + (state.K1 * state.K1)) - state.K1);

            state.PreviousTargetValue = initialValue;
            state.CurrentValue = initialValue;
            state.CurrentVelocity = default;
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
            state.CurrentVelocity += (deltaTime * (targetValue + (state.K3 * targetVelocity) - state.CurrentValue - (k1 * state.CurrentVelocity)) / k2);
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
        
        public float UpdateStrategy(ref SecondOrderState state, float deltaTime, float targetValue,
            float? targetVelocity)
        {
            float targetVelocityActual = UpdateVelocity(ref state, deltaTime, targetValue, targetVelocity);
            OnNewValue(ref state, deltaTime, targetValue, targetVelocityActual);
            return state.CurrentValue;
        }
    }
}
