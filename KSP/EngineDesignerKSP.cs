using System.Collections.Generic;
using UnityEngine;

namespace ksp
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class EngineDesignerKSP : MonoBehaviour
    {
        public EngineDesignerKSP instance;
        
        private List<ConfigNode> bipropellantConfigNodes;
        private List<BiPropellantConfig> bipropellants;

        public void ModuleManagerPostLoad()
        {
            Debug.Log("[EngineDesignerKSP] ModuleManagerPostLoad");
        }
    }
}
