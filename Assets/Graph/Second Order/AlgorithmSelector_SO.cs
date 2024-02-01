using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Sora_Ults
{
    public enum SelectedAlgorithm {
        ZeroPole,
        StableForcedIterations,
        Euler,
        None
    }
    public class AlgorithmSelector_SO
    {
        public SelectedAlgorithm selectedAlgorithm;
    }
}