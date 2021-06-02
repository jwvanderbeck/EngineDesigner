using System;
using Simulation.Engines.Liquid;
using UnityEngine;

namespace omg.Unity.Runtime
{
    [ExecuteInEditMode]
    public class LiquidEngine : MonoBehaviour
    {
        // Inputs
        public BiPropellantAsset propellant;
        public float propellantRatio;
        public float chamberPressure;
        public float nozzleDiameter;
        public float expansionRatio;
        
        // Derived
        /// <summary>
        /// The actual O:F ratio used in look up.  This is stepped and may be slightly different from given O:F ratio
        /// </summary>
        public float actualPropellantRatio;
        
        /// <summary>
        /// Combustion temp in kelvins
        /// </summary>
        public float combustionTemperature;
        public float specificHeatRatio;
        public float molecularWeight;
        public float pressureRatio;
        public float massFlowRate;
        public float exhaustVelocity;
        
        // Solved
        public float seaLevelThrust;
        /// <summary>
        /// Vacuum thrust in newtons
        /// </summary>
        public float vacuumThrust;

        /// <summary>
        /// Sea level ISP in seconds
        /// </summary>
        public float seaLevelIsp;
        
        /// <summary>
        /// Vacuum ISP in seconds
        /// </summary>
        public float vacuumIsp;
        

        private LiquidEngineSolver solverEngine;

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (propellant != null)
            {
                var prop = BiPropellant.CreateInstanceFromDataStore(propellant);
                solverEngine = new LiquidEngineSolver(prop);
            }
        }

        private void OnValidate()
        {
            UpdateSolver();
        }

        public void UpdateSolver()
        {
            solverEngine.EfficiencyFactor = 1f;
            solverEngine.ChamberPressure = chamberPressure;
            solverEngine.NozzleDiameter = nozzleDiameter;
            solverEngine.ExpansionRatio = expansionRatio;
            solverEngine.PropellantRatio = propellantRatio;
            solverEngine.UpdateDerivedProperties();

            actualPropellantRatio = solverEngine.actualPropellantRatio;
            combustionTemperature = solverEngine.combustionTemperature;
            specificHeatRatio = solverEngine.specificHeatRatio;
            molecularWeight = solverEngine.molecularWeight;
            pressureRatio = solverEngine.pressureRatio;
            massFlowRate = solverEngine.massFlowRate;
            exhaustVelocity = solverEngine.exhaustVelocity;

            seaLevelThrust = solverEngine.seaLevelThrust;
            seaLevelIsp = solverEngine.seaLevelIsp;
            vacuumThrust = solverEngine.vacuumThrust;
            vacuumIsp = solverEngine.vacuumIsp;
        }

        public void UpdatePropellant()
        {
            if (propellant != null)
            {
                var prop = BiPropellant.CreateInstanceFromDataStore(propellant);
                solverEngine.PropellantMixture = prop;
            }
        }
    }
}
