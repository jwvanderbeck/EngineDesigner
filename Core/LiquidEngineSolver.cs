using System;

namespace Simulation.Engines.Liquid
{

    [Serializable]
    public class LiquidEngineSolver
    {
        public BiPropellant propellantMixture;
        
        private float efficiencyFactor;

        // private BiPropellantAsset m_PropellantAsset;

        /// <summary>
        /// chamber pressure in pascals
        /// </summary>
        private float chamberPressure;
        
        /// <summary>
        /// O:F Ratio
        /// </summary>
        private float propellantRatio;
        
        /// <summary>
        /// Nozzle exit diameter in meters
        /// </summary>
        private float nozzleDiameter;

        /// <summary>
        /// Nozzle expansion ratio
        /// </summary>
        private float expansionRatio;

        // transients testing
        // F-1
        // Specific Heat 1.1507f
        // Molecular Weight 22.186f
        // Pressure Ratio 0.0079
        // Mixture Ratio 2.27
        //
        // RS-27
        // Combustion Temperature 3232.05
        // Specific Heat 1.1317
        // Molecular Weight 21.460
        // Pressure Ratio 0.0201
        // Mixture Ratio 2.245

        // Propellant data from LUT
        //
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

        // Solved and Derived Properties
        /// <summary>
        /// Sea level thrust in newtons
        /// </summary>
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

        

        LiquidEngineMetrics simEngine;

        public BiPropellant PropellantMixture
        {
            get { return propellantMixture; }
            set
            {
                propellantMixture = value;
                UpdatePropellant();
            }
        }

        public float EfficiencyFactor
        {
            get { return efficiencyFactor; }
            set
            {
                efficiencyFactor = value;
                UpdateDerivedProperties();
            }
        }

        public float ChamberPressure
        {
            get { return chamberPressure; }
            set
            {
                chamberPressure = value; 
                UpdateDerivedProperties();
            }
        }

        public float PropellantRatio
        {
            get { return propellantRatio; }
            set
            {
                propellantRatio = value; 
                UpdateDerivedProperties();
            }
        }

        public float NozzleDiameter
        {
            get { return nozzleDiameter; }
            set
            {
                nozzleDiameter = value; 
                UpdateDerivedProperties();
            }
        }

        public float ExpansionRatio
        {
            get { return expansionRatio; }
            set
            {
                expansionRatio = value; 
                UpdateDerivedProperties();
            }
        }

        public LiquidEngineSolver(BiPropellant propellantMixture)
        {
            this.propellantMixture = propellantMixture;
            Initialize();
        }

        public void Initialize()
        {
            if (simEngine == null)
            {
                simEngine = new LiquidEngineMetrics();
                simEngine.chamberPressure = ChamberPressure;
                simEngine.efficiencyFactor = EfficiencyFactor;
                simEngine.nozzleDiameter = NozzleDiameter;
                simEngine.expansionRatio = ExpansionRatio;
            }
        }

        void LoadTransientsForRatio(float ratio)
        {
            if (propellantMixture != null)
            {
                propellantMixture.EvaluateRatioAndPressure(ratio, chamberPressure,
                    out combustionTemperature, out specificHeatRatio, out molecularWeight);
            }
            if (simEngine != null)
            {
                simEngine.combustionTemperature = combustionTemperature;
                simEngine.specificHeatRatio = specificHeatRatio;
                simEngine.molecularWeight = molecularWeight;
                simEngine.expansionRatio = ExpansionRatio;
                pressureRatio = simEngine.SolvePressureRatio();
            }
        }

        public void UpdatePropellant()
        {
            LoadTransientsForRatio(PropellantRatio);
            UpdateDerivedProperties();
        }

        public void UpdateDerivedProperties()
        {
            if (simEngine == null) Initialize();
            
            // setup transients
            LoadTransientsForRatio(PropellantRatio);

            actualPropellantRatio = propellantMixture.SnapRatio(PropellantRatio);
            
            UpdateEfficiencyFactor();
            UpdateNozzleDiameter();
            UpdateExpansionRatio();
            UpdateChamberPressure();
            UpdateExhaustVelocity();
            UpdateMassFlowRate();
            UpdateThrust();

            seaLevelIsp = seaLevelThrust / (simEngine.MassFlowRate() * 9.807f);
            // seaLevelIsp *= efficiencyFactor;
            vacuumIsp = vacuumThrust / (simEngine.MassFlowRate() * 9.807f);
            // vacuumIsp *= efficiencyFactor;
        }

        void UpdateEfficiencyFactor()
        {
            if (simEngine == null) Initialize();
            simEngine.efficiencyFactor = EfficiencyFactor;
        }

        void UpdateExpansionRatio()
        {
            if (simEngine == null) Initialize();
            simEngine.expansionRatio = ExpansionRatio;
        }

        void UpdateThrust()
        {
            if (simEngine == null) Initialize();

            seaLevelThrust = simEngine.ThrustAtPressure(101325);// * efficiencyFactor;
            vacuumThrust = simEngine.ThrustAtPressure(0f);// * efficiencyFactor;
        }

        void UpdateMassFlowRate()
        {
            if (simEngine == null) Initialize();
            massFlowRate = simEngine.MassFlowRate();
        }

        void UpdateChamberPressure()
        {
            if (simEngine == null) Initialize();
            simEngine.chamberPressure = ChamberPressure;
        }

        void UpdateNozzleDiameter()
        {
            if (simEngine == null) Initialize();
            simEngine.nozzleDiameter = NozzleDiameter;
        }

        void UpdateExhaustVelocity()
        {
            if (simEngine == null) Initialize();
            exhaustVelocity = simEngine.IdealExhaustVelocity();
        }
    }
}
