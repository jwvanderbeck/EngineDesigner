namespace Simulation.Engines.Liquid.Interfaces
{
    public interface IThermodynamicDataStore
    {
        float[] AvailableRatios { get; }
        float[] AvailableChamberPressuresForRatio(float ratio);
        
        void GetData(float ofRatio, float chamberPressure,
            out float combustionTemperature, out float specificHeatRatio, out float molecularWeight);
    }
}