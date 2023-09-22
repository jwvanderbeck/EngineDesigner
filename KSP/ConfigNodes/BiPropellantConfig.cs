using System;
using System.Collections.Generic;
using System.Linq;
using Simulation.Engines.Liquid.Interfaces;

namespace EngineDesignerKSP.ConfigNodes
{
    public class BiPropellantConfig : IThermodynamicDataStore
    {
        public string fuel;
        public string oxidizer;
        public string title;
        
        public struct BiPropellantEngineTransient
        {
            // Each curve is based on Chamber Pressure on X, Value on Y
            public float ofRatio;
            public FloatCurve combustionTemperature;
            public FloatCurve specificHeatRatio;
            public FloatCurve molecularWeight;
        }
        
        public List<BiPropellantEngineTransient> engineTransients;
        public List<float> sortedKeys;

        public BiPropellantConfig(ConfigNode node)
        {
            Load(node);
        }

        public BiPropellantConfig() { }

        public float[] AvailableRatios
        {
            get { return sortedKeys.ToArray(); }
        }

        public float[] AvailableChamberPressuresForRatio(float ratio)
        {
            float step = 100000f;
            var pressures = new List<float>();
            var transients = GetTransients(ratio);
            var startTime = transients.combustionTemperature.Curve[0].time;
            var stopTime = transients.combustionTemperature.Curve[transients.combustionTemperature.Curve.length - 1].time;
            for (var t = startTime; t <= stopTime; t+= step)
            {
                pressures.Add(t);
            }

            return pressures.ToArray();
        }

        public void GetData(float ofRatio, float chamberPressure, out float combustionTemperature, out float specificHeatRatio, out float molecularWeight)
        {
            var transients = GetTransients(ofRatio);
            combustionTemperature = transients.combustionTemperature.Evaluate(chamberPressure);
            specificHeatRatio = transients.specificHeatRatio.Evaluate(chamberPressure);
            molecularWeight = transients.molecularWeight.Evaluate(chamberPressure);
        }

        public void Load(ConfigNode node)
        {
            node.TryGetValue("fuel", ref fuel);
            node.TryGetValue("oxidizer", ref oxidizer);
            node.TryGetValue("name", ref title);

            sortedKeys = new List<float>();
            engineTransients = new List<BiPropellantEngineTransient>();

            foreach (var transientConfigNode in node.GetNodes("TransientData"))
            {
                var transientData = new BiPropellantEngineTransient();
                transientConfigNode.TryGetValue("ofRatio", ref transientData.ofRatio);
                sortedKeys.Add(transientData.ofRatio);
                
                if (transientConfigNode.HasNode("combustionTemperature"))
                {
                    transientData.combustionTemperature = new FloatCurve();
                    transientData.combustionTemperature.Load(transientConfigNode.GetNode("combustionTemperature"));
                }
                if (transientConfigNode.HasNode("specificHeatRatio"))
                {
                    transientData.specificHeatRatio = new FloatCurve();
                    transientData.specificHeatRatio.Load(transientConfigNode.GetNode("specificHeatRatio"));
                }
                if (transientConfigNode.HasNode("molecularWeight"))
                {
                    transientData.molecularWeight = new FloatCurve();
                    transientData.molecularWeight.Load(transientConfigNode.GetNode("molecularWeight"));
                }
                engineTransients.Add(transientData);
            }
            
            engineTransients.Sort((t1, t2) => t1.ofRatio.CompareTo(t2.ofRatio));
            sortedKeys.Sort();
        }

        public float SnapToKey(float atRatio)
        {
            var ratioKey = 0f;
            var firstKey = sortedKeys.First();
            var lastKey = sortedKeys.Last();
            var ratio = (float)Math.Round(atRatio, 2);
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
                ratioKey = sortedKeys.Last(k => k <= ratio);
            }
            
            return ratioKey;
        }

        public BiPropellantEngineTransient GetTransients(float atRatio)
        {
            var ratioKey = SnapToKey(atRatio);
            return engineTransients.First(transient => transient.ofRatio == ratioKey);
        }
    }
}
