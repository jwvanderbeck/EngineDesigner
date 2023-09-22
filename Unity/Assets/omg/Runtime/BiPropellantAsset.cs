using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using omg.Runtime;
using Simulation.Engines.Liquid.Interfaces;
using UnityEditor;
using UnityEngine;

namespace Simulation.Engines.Liquid
{
    [CreateAssetMenu][Serializable]
    public class BiPropellantAsset : ScriptableObject, IThermodynamicDataStore
    {
        public PropellantAsset oxidizer;
        public PropellantAsset fuel;
        public bool hypergolic;

        public List<ThermodynamicPropertiesAsset> thermodynamicData;

        [Serializable]
        public struct BiPropellantEngineTransient
        {
            // Each curve is based on Chamber Pressure on X, Value on Y
            public float ofRatio;
            public AnimationCurve combustionTemperature;
            public AnimationCurve specificHeatRatio;
            public AnimationCurve molecularWeight;
        }
        
        [SerializeField, HideInInspector]
        public List<BiPropellantEngineTransient> engineTransients;

        private string datasetFile;

        [SerializeField]
        public List<float> sortedKeys;
        private float[] m_AvailableRatios;

        public float GetLowestAvailableRatio()
        {
            if (sortedKeys == null || sortedKeys.Count == 0) return 0f;
            
            sortedKeys.Sort();
            return sortedKeys[0];
        }

        public float GetHighestAvailableRatio()
        {
            if (sortedKeys == null || sortedKeys.Count == 0) return 0f;

            sortedKeys.Sort();
            return sortedKeys[sortedKeys.Count - 1];
        }

        public float GetLowestAvailableChamberPressure()
        {
            float lowest = float.MaxValue;
            foreach (var transient in engineTransients)
            {
                if (transient.combustionTemperature.keys[0].time < lowest)
                {
                    lowest = transient.combustionTemperature.keys[0].time;
                }
            }

            return lowest;
        }

        public float GetHighestAvailableChamberPressure()
        {
            float highest = float.MinValue;
            foreach (var transient in engineTransients)
            {
                var keys = transient.combustionTemperature.keys;
                if (transient.combustionTemperature.keys[keys.Length - 1].time > highest)
                {
                    highest = transient.combustionTemperature.keys[keys.Length - 1].time;
                }
            }

            return highest;
        }

        public void Initialize()
        {
            if (sortedKeys == null)
            {
                sortedKeys = new List<float>();
            }

            if (engineTransients == null)
            {
                engineTransients = new List<BiPropellantEngineTransient>();
            }
            
            sortedKeys.Clear();
            engineTransients.Clear();
            
            // Import all assigned thermodynamic data that matches our mixture
            foreach (var asset in thermodynamicData)
            {
                if (asset.fuel == fuel && asset.oxidizer == oxidizer)
                {
                    foreach (var transient in asset.engineTransients)
                    {
                        AddTransients(transient.ofRatio, transient);
                    }
                }
            }
        }
        
        public BiPropellantEngineTransient GetTransients(float atRatio)
        {
            var ratioKey = SnapToKey(atRatio);
            return engineTransients.First(transient => transient.ofRatio == ratioKey);
        }

        public void AddTransients(float atRatio, BiPropellantEngineTransient transientData)
        {
            if (sortedKeys.Contains(atRatio))
            {
                engineTransients.Remove(GetTransients(atRatio));
            }
            else
            {
                sortedKeys.Add(atRatio);
                sortedKeys.Sort();
            }
            engineTransients.Add(transientData);
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

        public void WriteDataToConfigNode()
        {
            Initialize();
            
            var fileText = new List<string>();
            // Header
            fileText.Add("ENGINEDESIGNER_BIPROPELLANT");
            fileText.Add("{");
            fileText.Add($"\tname = {name}");
            fileText.Add($"\tfuel = {fuel.title}");
            fileText.Add($"\toxidizer = {oxidizer.title}");
            fileText.Add("");
            
            // Transient Data Blocks
            foreach (var transient in engineTransients)
            {
                // header
                fileText.Add("\tTransientData");
                fileText.Add("\t{");
                
                // content
                fileText.Add($"\t\tofRatio = {transient.ofRatio}");

                // combustionTemperature
                fileText.Add($"\t\tcombustionTemperature");
                fileText.Add("\t\t{");
                foreach (var key in transient.combustionTemperature.keys)
                {
                    fileText.Add($"\t\t\tkey = {key.time} {key.value} {key.inTangent} {key.outTangent}");
                }
                fileText.Add("\t\t}");

                // specificHeatRatio
                fileText.Add($"\t\tspecificHeatRatio");
                fileText.Add("\t\t{");
                foreach (var key in transient.specificHeatRatio.keys)
                {
                    fileText.Add($"\t\t\tkey = {key.time} {key.value} {key.inTangent} {key.outTangent}");
                }
                fileText.Add("\t\t}");

                // molecularWeight
                fileText.Add($"\t\tmolecularWeight");
                fileText.Add("\t\t{");
                foreach (var key in transient.molecularWeight.keys)
                {
                    fileText.Add($"\t\t\tkey = {key.time} {key.value} {key.inTangent} {key.outTangent}");
                }
                fileText.Add("\t\t}");
                
                // footer
                fileText.Add("\t}");
            }
            // Footer
            fileText.Add("}");
            
            File.WriteAllLines($"Assets/Data/Configs/{name}.cfg", fileText);
        }

        [Serializable]
        public class BiPropellantDataset
        {
            public float ofRatio;
            public List<float> chamberPressures;
            public List<float> combustionTemperatures;
            public List<float> molecularWeights;
            public List<float> specificHeatRatios;
        }

        public float[] AvailableRatios
        {
            get { return sortedKeys.ToArray(); }
        }

        public float[] AvailableChamberPressuresForRatio(float ratio)
        {
            float step = 100000f;
            var pressures = new List<float>();
            var transients = GetTransients(ratio);
            var startTime = transients.combustionTemperature[0].time;
            var stopTime = transients.combustionTemperature[transients.combustionTemperature.length - 1].time;
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
    }
}
