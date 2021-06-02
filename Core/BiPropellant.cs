using System;
using System.Collections.Generic;
using System.Linq;

namespace Simulation.Engines.Liquid
{
    /// <summary>
    /// Manages thermodynamic data for a given mixture of fuel and oxidizer at various ratios.
    /// This class is not concerned with how that data is stored, and is runtime only.  It is initialized with data from some store
    /// </summary>
    [Serializable]
    public class BiPropellant
    {
        public Fuel fuel;
        public Oxidizer oxidizer;
        // ReSharper disable once IdentifierTypo
        public bool isHypergolic;

        public static BiPropellant CreateInstanceFromDataStore(IThermodynamicDataStore dataStore)
        {
            var biprop = new BiPropellant();

            biprop.engineTransients = new List<ThermodynamicData>();

            var availableRatios = dataStore.AvailableRatios;

            for (int ratioIndex = 0; ratioIndex < availableRatios.Length; ratioIndex++)
            {
                var currentRatio = availableRatios[ratioIndex];
                var dataForRatio = new ThermodynamicData();
                dataForRatio.ofRatio = currentRatio;
                dataForRatio.pressureData = new List<ChamberPressureDataPoint>();

                var pressures = dataStore.AvailableChamberPressuresForRatio(currentRatio);
                for (int pressureIndex = 0; pressureIndex < pressures.Length; pressureIndex++)
                {
                    float combustionTemperature;
                    float specificHeatRatio;
                    float molecularWeight;
                    dataStore.GetData(currentRatio, pressures[pressureIndex],
                        out combustionTemperature, out specificHeatRatio, out molecularWeight);
                    
                    var pressureData = new ChamberPressureDataPoint();
                    pressureData.chamberPressure = pressures[pressureIndex];
                    pressureData.combustionTemperature = combustionTemperature;
                    pressureData.specificHeatRatio = specificHeatRatio;
                    pressureData.molecularWeight = molecularWeight;
                    
                    dataForRatio.pressureData.Add(pressureData);
                }
                biprop.engineTransients.Add(dataForRatio);
            }
            biprop.SortData();
            return biprop;
        }

        /// <summary>
        /// For a given OF Ratio and Chamber Pressure, evaluate and determine thermodynamic data.
        /// OFRatio is snapped to nearest known ratio.
        /// Chamber pressure is interpolated to nearest pressure data for snapped ratio.
        /// </summary>
        /// <param name="ofRatio">Desired OF Ratio for data.  Will snap to nearest known ratio</param>
        /// <param name="chamberPressure">Desired pressure for data.  If not available, will be interpolated</param>
        /// <param name="combustionTemperature"></param>
        /// <param name="specificHeatRatio"></param>
        /// <param name="molecularWeight"></param>
        public void EvaluateRatioAndPressure(float ofRatio, float chamberPressure,
            out float combustionTemperature, out float specificHeatRatio, out float molecularWeight)
        {
            // First, snap O/F ratio to the nearest available
            var actualRatio = SnapRatio(ofRatio);

            // Return the data directly if we have it
            if (RatioDataContainsPressure(actualRatio, chamberPressure))
            {
                var pressureData = GetDataForRatioAndPressure(actualRatio, chamberPressure);
                combustionTemperature = pressureData.combustionTemperature;
                specificHeatRatio = pressureData.specificHeatRatio;
                molecularWeight = pressureData.molecularWeight;
                return;
            }
            // or interpolate if we don't
            {
                var ratioData = GetDataForRatio(actualRatio);
                var overIndex = ratioData.pressureData.FindIndex(pd => pd.chamberPressure > chamberPressure);
                if (overIndex == -1)
                {
                    // We didn't find any data with a pressure larger than we want, so simply return based on the last data we have
                    var pressureData = ratioData.pressureData.Last();
                    combustionTemperature = pressureData.combustionTemperature;
                    specificHeatRatio = pressureData.specificHeatRatio;
                    molecularWeight = pressureData.molecularWeight;
                    return;
                }
                else if (overIndex == 0)
                {
                    // The very first index is already over our pressure, so again, return that
                    var pressureData = ratioData.pressureData.First();
                    combustionTemperature = pressureData.combustionTemperature;
                    specificHeatRatio = pressureData.specificHeatRatio;
                    molecularWeight = pressureData.molecularWeight;
                    return;
                }
                else
                {
                    // interpolate between the key that is too high, and the previous which should be too low
                    var underIndex = overIndex - 1;
                    var lowPressureData = ratioData.pressureData[underIndex];
                    var highPressureData = ratioData.pressureData[overIndex];
                    // calculate ratio between key pressures
                    var ratio = chamberPressure / (highPressureData.chamberPressure - lowPressureData.chamberPressure);
                    var pressureData = LerpPressureData(lowPressureData, highPressureData, ratio);
                    combustionTemperature = pressureData.combustionTemperature;
                    specificHeatRatio = pressureData.specificHeatRatio;
                    molecularWeight = pressureData.molecularWeight;
                    return;
                }
            }
        }

        public float SnapRatio(float ofRatio)
        {
            var ratioKey = 0f;
            var firstKey = engineTransients.First().ofRatio;
            var lastKey = engineTransients.Last().ofRatio;
            var ratio = (float)Math.Round(ofRatio, 2);
            if (ratio <= firstKey)
            {
                ratioKey = firstKey;
            }
            else if (ratio >= lastKey)
            {
                ratioKey = lastKey;
            }
            else
            {
                ratioKey = engineTransients.Last(k => k.ofRatio <= ratio).ofRatio;
            }
            return ratioKey;
        }

        public void AddData(float ofRatio, float chamberPressure,
            float combustionTemperature, float specificHeatRatio, float molecularWeight)
        {
            // if we already contain data on both the ratio and chamber pressure, then replace the existing data
            if (RatioDataContainsPressure(ofRatio, chamberPressure))
            {
                var pressureData = GetDataForRatioAndPressure(ofRatio, chamberPressure);
                pressureData.combustionTemperature = combustionTemperature;
                pressureData.specificHeatRatio = specificHeatRatio;
                pressureData.molecularWeight = molecularWeight;

                var ratioData = GetDataForRatio(ofRatio);
                var index = ratioData.pressureData.FindIndex(point => point.chamberPressure == chamberPressure);
                ratioData.pressureData.RemoveAt(index);
                ratioData.pressureData.Add(pressureData);

                return;
            }

            // If we have data for this ratio but not the pressure, add the pressure data to the ratio
            if (DataContainsRatio(ofRatio))
            {
                var pressureData = new ChamberPressureDataPoint();
                pressureData.chamberPressure = chamberPressure;
                pressureData.combustionTemperature = combustionTemperature;
                pressureData.specificHeatRatio = specificHeatRatio;
                pressureData.molecularWeight = molecularWeight;

                var ratioData = GetDataForRatio(ofRatio);
                ratioData.pressureData.Add(pressureData);

                return;
            }
            
            // no data on either ratio or pressure, so make a new entry
            {
                var pressureData = new ChamberPressureDataPoint();
                pressureData.chamberPressure = chamberPressure;
                pressureData.combustionTemperature = combustionTemperature;
                pressureData.specificHeatRatio = specificHeatRatio;
                pressureData.molecularWeight = molecularWeight;

                var ratioData = new ThermodynamicData();
                ratioData.ofRatio = ofRatio;
                ratioData.pressureData = new List<ChamberPressureDataPoint>();
                ratioData.pressureData.Add(pressureData);
            }

        }

        private void SortData()
        {
            for (int i = 0; i < engineTransients.Count; i++)
            {
                engineTransients[i].pressureData.Sort((d1, d2) => d1.chamberPressure.CompareTo(d2.chamberPressure));
            }
            engineTransients.Sort((t1, t2) => t1.ofRatio.CompareTo(t2.ofRatio));
        }
        
        private ChamberPressureDataPoint LerpPressureData(ChamberPressureDataPoint point1, ChamberPressureDataPoint point2, float by)
        {
            var pressureData = new ChamberPressureDataPoint();
            
            pressureData.combustionTemperature = point1.combustionTemperature * (1 - by) + point2.combustionTemperature * by;
            pressureData.specificHeatRatio = point1.specificHeatRatio * (1 - by) + point2.specificHeatRatio * by;
            pressureData.molecularWeight = point1.molecularWeight * (1 - by) + point2.molecularWeight * by;

            return pressureData;
        }


        private bool DataContainsRatio(float ratio)
        {
            
            for (int i = 0; i < engineTransients.Count; i++)
            {
                if (engineTransients[i].ofRatio == ratio) return true;
            }
            return false;
        }

        private ThermodynamicData GetDataForRatio(float ratio)
        {
            return engineTransients.FirstOrDefault(data => data.ofRatio == ratio);
        }

        private bool RatioDataContainsPressure(float ratio, float pressure)
        {
            if (!DataContainsRatio(ratio)) return false;
            var data = GetDataForRatio(ratio);

            for (int i = 0; i < data.pressureData.Count; i++)
            {
                if (data.pressureData[i].chamberPressure == pressure) return true;
            }

            return false;
        }

        private ChamberPressureDataPoint GetDataForRatioAndPressure(float ratio, float pressure)
        {
            var data = GetDataForRatio(ratio);

            return data.pressureData.FirstOrDefault(p => p.chamberPressure == pressure);
        }

        private List<ThermodynamicData> engineTransients;
        
        /// <summary>
        /// Contains thermodynamic data for various values of OF Ratio (1st) and Chamber Pressure (2nd)
        /// </summary>
        [Serializable]
        internal struct ThermodynamicData
        {
            // Each curve is based on Chamber Pressure on X, Value on Y
            internal float ofRatio;
            internal List<ChamberPressureDataPoint> pressureData;
        }

        [Serializable]
        internal struct ChamberPressureDataPoint
        {
            internal float chamberPressure; // Pascals
            internal float combustionTemperature; // Kelvin
            internal float specificHeatRatio;
            internal float molecularWeight;
        }
    }

    public interface IThermodynamicDataStore
    {
        float[] AvailableRatios { get; }
        float[] AvailableChamberPressuresForRatio(float ratio);
        
        void GetData(float ofRatio, float chamberPressure,
            out float combustionTemperature, out float specificHeatRatio, out float molecularWeight);
    }
}
