using System;

namespace Simulation.Engines.Liquid
{
    [Serializable]
    public class EngineTransients
    {
        public float ofRatio;    // Ratio of Oxidizer to Fuel that this transient data represents

        public float chamberPressure;
        public float combustionTemperature;
        public float molecularWeight;
        public float specificHeatRatio;
    }
}
