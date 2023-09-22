using System.Collections.Generic;
using System.IO;
using Simulation.Engines.Liquid;
using System.Text.RegularExpressions;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;
using omg.Runtime;

namespace omg.Editor
{
    [ScriptedImporter(1, "cearun")]
    public class CEARUNImporter : ScriptedImporter
    {
        public PropellantAsset oxidizer;
        public PropellantAsset fuel;

        [SerializeField]
        public List<BiPropellantAsset.BiPropellantEngineTransient> engineTransients;

        private const string k_chamberPressureTag = @"P, BAR[ ]*(\d*\.?\d+)[ ]*(\d*\.?\d+)[ ]*(\d*\.?\d+)[ ]*(\d*\.?\d+)[ ]*(\d*\.?\d+)[ ]*(\d*\.?\d+)[ ]*(\d*\.?\d+)[ ]*(\d*\.?\d+)";
        private const string k_combustionTemperatureTag = @"T, K[ ]*(\d*\.?\d+)[ ]*(\d*\.?\d+)[ ]*(\d*\.?\d+)[ ]*(\d*\.?\d+)[ ]*(\d*\.?\d+)[ ]*(\d*\.?\d+)[ ]*(\d*\.?\d+)[ ]*(\d*\.?\d+)";
        private const string k_molecularWeightTag = @"M, \(1/n\)[ ]*(\d*\.?\d+)[ ]*(\d*\.?\d+)[ ]*(\d*\.?\d+)[ ]*(\d*\.?\d+)[ ]*(\d*\.?\d+)[ ]*(\d*\.?\d+)[ ]*(\d*\.?\d+)[ ]*(\d*\.?\d+)";
        private const string k_specificHeatRatioTag = @"GAMMAs[ ]*(\d*\.?\d+)[ ]*(\d*\.?\d+)[ ]*(\d*\.?\d+)[ ]*(\d*\.?\d+)[ ]*(\d*\.?\d+)[ ]*(\d*\.?\d+)[ ]*(\d*\.?\d+)[ ]*(\d*\.?\d+)";
        private const string k_ofRatioTag = @"O/F=[ ]*(?<ofRatio>\d*\.?\d+)";
        private const float barToPascal = 100000f;

        private Dictionary<float, BiPropellantAsset.BiPropellantEngineTransient> engineTransientsCache;
        
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var currentRatio = 0f;
            var nextRatio = 0f;
            var inBlock = false;
            var chamberPressures = new List<float>();
            var combustionTemperatures = new List<float>();
            var molecularWeights = new List<float>();
            var specificHeatRatios = new List<float>();
            
            var rxOFRatio = new Regex(k_ofRatioTag, RegexOptions.Compiled);
            var rxChamberPressure = new Regex(k_chamberPressureTag, RegexOptions.Compiled);
            var rxCombustionTemperature = new Regex(k_combustionTemperatureTag, RegexOptions.Compiled);
            var rxMolecularWeight = new Regex(k_molecularWeightTag, RegexOptions.Compiled);
            var rxSpecificHeatRatio = new Regex(k_specificHeatRatioTag, RegexOptions.Compiled);
            
            var data = File.ReadAllLines(ctx.assetPath);

            var lowestRatio = float.MaxValue;
            var highestRatio = float.MinValue;
            var lowestPressure = float.MaxValue;
            var highestPressure = float.MinValue;

            if (engineTransientsCache == null)
            {
                engineTransientsCache = new Dictionary<float, BiPropellantAsset.BiPropellantEngineTransient>();
            }
            
            foreach (var s in data)
            {
                MatchCollection matches = rxOFRatio.Matches(s);
                if (matches.Count > 0)
                {
                    if (float.TryParse(matches[0].Groups["ofRatio"].Value, out nextRatio))
                    {
                        if (nextRatio < lowestRatio) lowestRatio = nextRatio;
                        if (nextRatio > highestRatio) highestRatio = nextRatio;
                        
                        if (inBlock  && nextRatio != currentRatio)
                        {
                            // Save data from last block, and get ready for next
                            SaveBlock(currentRatio, chamberPressures, combustionTemperatures, molecularWeights, specificHeatRatios);
                        }
                        inBlock = true;
                        currentRatio = nextRatio;
                    }
                }

                if (!inBlock) continue;
                
                matches = rxChamberPressure.Matches(s);
                if (matches.Count > 0)
                {
                    for (int i = 1; i < matches[0].Groups.Count; i++)
                    {
                        if (!float.TryParse(matches[0].Groups[i].Value, out var pressure)) continue;
                        pressure *= barToPascal;
                        
                        if (pressure > 0)
                        {
                            chamberPressures.Add(pressure);
                            if (pressure < lowestPressure) lowestPressure = pressure;
                            if (pressure > highestPressure) highestPressure = pressure;
                        }
                    }
                }

                matches = rxCombustionTemperature.Matches(s);
                if (matches.Count > 0)
                {
                    for (int i = 1; i < matches[0].Groups.Count; i++)
                    {
                        float temperature;
                        if (!float.TryParse(matches[0].Groups[i].Value, out temperature)) continue;
                        
                        if (temperature > 0)
                        {
                            combustionTemperatures.Add(temperature);
                        }
                    }
                }
                
                matches = rxMolecularWeight.Matches(s);
                if (matches.Count > 0)
                {
                    for (int i = 1; i < matches[0].Groups.Count; i++)
                    {
                        float molecularWeight;
                        if (!float.TryParse(matches[0].Groups[i].Value, out molecularWeight)) continue;
                        
                        if (molecularWeight > 0)
                        {
                            molecularWeights.Add(molecularWeight);
                        }
                    }
                }
                
                matches = rxSpecificHeatRatio.Matches(s);
                if (matches.Count > 0)
                {
                    for (int i = 1; i < matches[0].Groups.Count; i++)
                    {
                        float gamma;
                        if (!float.TryParse(matches[0].Groups[i].Value, out gamma)) continue;
                        
                        if (gamma > 0)
                        {
                            specificHeatRatios.Add(gamma);
                        }
                    }
                }
            }
            // save any final data
            SaveBlock(currentRatio, chamberPressures, combustionTemperatures, molecularWeights, specificHeatRatios);
            
            // create final list from Dict
            engineTransients = new List<BiPropellantAsset.BiPropellantEngineTransient>(engineTransientsCache.Count);
            foreach (var transient in engineTransientsCache)
            {
                engineTransients.Add(transient.Value);
            }

            var asset = ScriptableObject.CreateInstance<ThermodynamicPropertiesAsset>();
            asset.engineTransients = engineTransients;
            asset.fuel = fuel;
            asset.oxidizer = oxidizer;
            asset.Initialize();
            ctx.AddObjectToAsset("thermo", asset);
            ctx.SetMainObject(asset);
        }

        private void SaveBlock(float currentRatio, List<float> chamberPressures, List<float> combustionTemperatures, List<float> molecularWeights, List<float> specificHeatRatios)
        {
            var combustionTemperatureCurve = new AnimationCurve();
            var molecularWeightCurve = new AnimationCurve();
            var specificHeatCurve = new AnimationCurve();
            for (int i = 0; i < chamberPressures.Count; i++)
            {
                combustionTemperatureCurve.AddKey(chamberPressures[i], combustionTemperatures[i]);
                molecularWeightCurve.AddKey(chamberPressures[i], molecularWeights[i]);
                specificHeatCurve.AddKey(chamberPressures[i], specificHeatRatios[i]);
            }

            var transient = new BiPropellantAsset.BiPropellantEngineTransient();
            transient.combustionTemperature = combustionTemperatureCurve;
            transient.molecularWeight = molecularWeightCurve;
            transient.specificHeatRatio = specificHeatCurve;
            transient.ofRatio = currentRatio;
                            
            if (!engineTransientsCache.ContainsKey(currentRatio))
            {
                                
                engineTransientsCache.Add(currentRatio, transient);
            }
            else
            {
                engineTransientsCache[currentRatio] = transient;
            }
            chamberPressures.Clear();
            combustionTemperatures.Clear();
            molecularWeights.Clear();
            specificHeatRatios.Clear();
        }
    }
}
