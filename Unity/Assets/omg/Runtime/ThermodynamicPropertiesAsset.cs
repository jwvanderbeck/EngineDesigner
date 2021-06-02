using System.Collections.Generic;
using Simulation.Engines.Liquid;
using UnityEngine;

namespace omg.Runtime
{
    [CreateAssetMenu(fileName = "ThermodynamicProperties", menuName = "Thermodynamic Properties", order = 0)]
    public class ThermodynamicPropertiesAsset : ScriptableObject
    {
        public PropellantAsset oxidizer;
        public PropellantAsset fuel;
        
        public List<BiPropellantAsset.BiPropellantEngineTransient> engineTransients;

        public void Initialize()
        {
            engineTransients.Sort((t1, t2) => t1.ofRatio.CompareTo(t2.ofRatio));
        }
    }
}
