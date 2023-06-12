using UnityEngine;
using Sora_Ults;

namespace Sora_Ults
{
    public class SO_Position_Handler
    {
        private ISecondOrderSystem selectedAlgorithm;
        private SecondOrderState stateX;
        private SecondOrderState stateY;
        private SecondOrderState stateZ;
        private Vector3 initialPosition;
        
        void SelectAlgorithm(SelectedAlgorithm Input)
        {
            switch (Input)
            { 
                case SelectedAlgorithm.ZeroPole:
                    selectedAlgorithm = new SO_Calc_ZeroPole();
                    break;
                case SelectedAlgorithm.StableForcedIterations:
                    selectedAlgorithm = new SO_Calc_StableForcedIterations();
                    break;
                case SelectedAlgorithm.Euler:
                    selectedAlgorithm = new SO_Calc_Euler();
                    break;
                case SelectedAlgorithm.Linear:
                    selectedAlgorithm = new SO_Calc_Linear();
                    break;
                case SelectedAlgorithm.EulerClampedK2:
                    selectedAlgorithm = new SO_Calc_EulerClampedK2();
                    break;
                case SelectedAlgorithm.EulerNoJitter:
                    selectedAlgorithm = new SO_Calc_EulerNoJitter();
                    break;
                case SelectedAlgorithm.None:
                    selectedAlgorithm = new SO_Calc_None();
                    break;
                default:
                    Debug.Log("pick plz");
                    selectedAlgorithm = new SO_Calc_None();
                    break;
            }
        }

        public void Initialize(float frequency, float damping, float responsiveness, float deltaTimeScale,
            Vector3 initialPosition, SelectedAlgorithm ASelector)
        {
            SelectAlgorithm(ASelector);

            stateX = new SecondOrderState
            {
                F = frequency,
                Z = damping,
                R = responsiveness,
                InitialValue = 0.0f,
                DeltaTime = deltaTimeScale,
                TargetValue = 1.0f,
            };
            stateY = new SecondOrderState
            {
                F = frequency,
                Z = damping,
                R = responsiveness,
                InitialValue = 0.0f,
                DeltaTime = deltaTimeScale,
                TargetValue = 1.0f,
            };
            stateZ = new SecondOrderState
            {
                F = frequency,
                Z = damping,
                R = responsiveness,
                InitialValue = 0.0f,
                DeltaTime = deltaTimeScale,
                TargetValue = 1.0f,
            };

            selectedAlgorithm.RecalculateConstants(ref stateX, stateX.InitialValue);
            selectedAlgorithm.RecalculateConstants(ref stateY, stateY.InitialValue);
            selectedAlgorithm.RecalculateConstants(ref stateZ, stateZ.InitialValue);
            
            //Debug.Log(initialPosition);
            this.initialPosition = initialPosition;
        }

        
        //The responsiveness just broken D: I dunno why, help plz
        public void UpdatePosition_2(Vector3 targetPosition, Transform transformToUpdate)
        {
            float deltaTime = Time.deltaTime * stateX.DeltaTime;

            Vector3 targetValue = targetPosition - initialPosition;
            Vector3 targetVelocity = targetValue / deltaTime;

            Vector3 updatedValue = new Vector3(
                selectedAlgorithm.UpdateStrategy(ref stateX, deltaTime, targetValue.x, targetVelocity.x),
                selectedAlgorithm.UpdateStrategy(ref stateY, deltaTime, targetValue.y, targetVelocity.y),
                selectedAlgorithm.UpdateStrategy(ref stateZ, deltaTime, targetValue.z, targetVelocity.z));

            transformToUpdate.localPosition = initialPosition + updatedValue;
        }

    }
}