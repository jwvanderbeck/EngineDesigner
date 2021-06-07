// using Simulation.Engines.Liquid;

using System;
using System.Collections.Generic;
using System.Linq;
using Simulation.Engines.Liquid;
using UnityEngine;

namespace ksp
{
    public class EngineDesignerLE : PartModule
    {
        private const string k_engineInputsGroupName = "engineDesignerInputsGroup";
        private const string k_engineInputsGroupDisplayName = "Engine Designer - Inputs";

        [KSPField(isPersistant = true, 
             groupName = k_engineInputsGroupName, groupDisplayName = k_engineInputsGroupDisplayName,
             guiActiveEditor = true,
             guiName = "Fuel"), 
         UI_ChooseOption(suppressEditorShipModified = true)]
        public string fuelString = "Kerosene";
        
        [KSPField(isPersistant = true, 
             groupName = k_engineInputsGroupName, groupDisplayName = k_engineInputsGroupDisplayName,
             guiActiveEditor = true,
             guiName = "Oxidizer"), 
         UI_ChooseOption(suppressEditorShipModified = true)]
        public string oxidizerString = "LqdOxygen";
        
        [KSPField(isPersistant = true, 
            groupName = k_engineInputsGroupName, groupDisplayName = k_engineInputsGroupDisplayName,
            guiActiveEditor = true,
            guiName = "Chamber Pressure", guiFormat = "N2", guiUnits = "bar"), 
         UI_FloatRange(suppressEditorShipModified = true,
             minValue = 1, maxValue = 400, stepIncrement = 1)]
        public float chamberPressure= 10f;

        [KSPField(isPersistant = true, 
             groupName = k_engineInputsGroupName, groupDisplayName = k_engineInputsGroupDisplayName,
             guiActiveEditor = true,
             guiName = "O/F Ratio", guiFormat = "N2", guiUnits = ":1"), 
         UI_FloatRange(suppressEditorShipModified = true,
             minValue = 1, maxValue = 7, stepIncrement = 0.01f)]
        public float propellantRatio = 2.00f;

        [KSPField(isPersistant = true, 
             groupName = k_engineInputsGroupName, groupDisplayName = k_engineInputsGroupDisplayName,
             guiActiveEditor = true,
             guiName = "Nozzle Diam", guiFormat = "N2", guiUnits = "m"), 
         UI_FloatEdit(sigFigs = 2, suppressEditorShipModified = true,
             minValue = 0.1f, maxValue = 10f, incrementSmall = 0.1f, incrementLarge = 1)]
        public float nozzleDiameter = 1f;

        [KSPField(isPersistant = true,
             groupName = k_engineInputsGroupName, groupDisplayName = k_engineInputsGroupDisplayName,
             guiActiveEditor = true,
             guiName = "Expansion Ratio", guiFormat = "N1"),
         UI_FloatEdit(sigFigs = 1, suppressEditorShipModified = true,
             minValue = 2, maxValue = 50, incrementSmall = 0.1f, incrementLarge = 1)]
        public float expansionRatio = 7.46f;

        [KSPField(isPersistant = false, groupName = k_engineInputsGroupName, groupDisplayName = k_engineInputsGroupDisplayName, guiActiveEditor = true), UI_Toggle(scene = UI_Scene.Editor)]
        public bool guiApply;

        private const string k_engineTransientsGroupName = "engineDesignerTransientsGroup";
        private const string k_engineTransientsGroupDisplayName = "Engine Designer - Transients";
        
        [KSPField(groupName = k_engineTransientsGroupName, groupDisplayName = k_engineTransientsGroupDisplayName,
            guiActiveEditor = true,
            guiName = "Solved O/F Ratio", guiFormat = "N2", guiUnits = ":1")]
        public float actualPropellantRatio;
        [KSPField(groupName = k_engineTransientsGroupName, groupDisplayName = k_engineTransientsGroupDisplayName,
            guiActiveEditor = true,
            guiName = "Combustion Temperature", guiFormat = "N2", guiUnits = "K")]
        public float combustionTemperature;
        [KSPField(groupName = k_engineTransientsGroupName, groupDisplayName = k_engineTransientsGroupDisplayName,
            guiActiveEditor = true,
            guiName = "Specific Heat Ratio", guiFormat = "N2")]
        public float specificHeatRatio;
        [KSPField(groupName = k_engineTransientsGroupName, groupDisplayName = k_engineTransientsGroupDisplayName,
            guiActiveEditor = true,
            guiName = "Molecular Weight", guiFormat = "N2")]
        public float molecularWeight;
        [KSPField(groupName = k_engineTransientsGroupName, groupDisplayName = k_engineTransientsGroupDisplayName,
            guiActiveEditor = true,
            guiName = "Pressure Ratio", guiFormat = "N6")]
        public float pressureRatio;
        [KSPField(groupName = k_engineTransientsGroupName, groupDisplayName = k_engineTransientsGroupDisplayName,
            guiActiveEditor = true,
            guiName = "Mass Flow Rate", guiFormat = "N2")]
        public float massFlowRate;
        [KSPField(groupName = k_engineTransientsGroupName, groupDisplayName = k_engineTransientsGroupDisplayName,
            guiActiveEditor = true,
            guiName = "Exhaust Velocity", guiFormat = "N2", guiUnits = "m/s")]
        public float exhaustVelocity;
        
        private const string k_engineOutputsGroupName = "engineDesignerOutputsGroup";
        private const string k_engineOutputsGroupDisplayName = "Engine Designer - Outputs";

        [KSPField(groupName = k_engineOutputsGroupName, groupDisplayName = k_engineOutputsGroupDisplayName,
            guiActiveEditor = true,
            guiName = "Sea Level Thrust", guiFormat = "N2", guiUnits = "kN")]
        public float seaLevelThrust;
        [KSPField(groupName = k_engineOutputsGroupName, groupDisplayName = k_engineOutputsGroupDisplayName,
            guiActiveEditor = true,
            guiName = "Sea Level Isp", guiFormat = "N2", guiUnits = "s")]
        public float seaLevelIsp;
        [KSPField(groupName = k_engineOutputsGroupName, groupDisplayName = k_engineOutputsGroupDisplayName,
            guiActiveEditor = true,
            guiName = "Vacuum Thrust", guiFormat = "N2", guiUnits = "kN")]
        public float vacuumThrust;
        [KSPField(groupName = k_engineOutputsGroupName, groupDisplayName = k_engineOutputsGroupDisplayName,
            guiActiveEditor = true,
            guiName = "Vacuum Isp", guiFormat = "N2", guiUnits = "s")]
        public float vacuumIsp;
        
        private string[] fuelOptions;
        private string[] oxidizerOptions;

        private const float barToPascal = 100000f;
        private const float PascalToBar = 1 / 100000f;
        
        // private LiquidEngineSolver engineSolver;
        private List<BiPropellantConfig> bipropellants;
        private Propellant fuel;
        private Propellant oxidizer;

        private LiquidEngineSolver solverEngine;

        public override void OnAwake()
        {
            base.OnAwake();
            Debug.Log("[EngineDesignerLE] OnAwake()");
        }

        public override void OnStart(StartState state)
        {
            if (state != StartState.Editor) return;
            
            Debug.Log("[EngineDesignerLE] OnStart()");

            bipropellants = new List<BiPropellantConfig>();
            
            ConfigNode[] bipropConfigs = GameDatabase.Instance.GetConfigNodes("ENGINEDESIGNER_BIPROPELLANT");
            foreach (var config in bipropConfigs)
            {
                var bipropConfig = new BiPropellantConfig(config);
                bipropellants.Add(bipropConfig);
            }
            Debug.Log($"[EngineDesignerLE] Loaded {bipropellants.Count} biprop configs");
            
            Initialize();
        }

        private void Solve()
        {
            solverEngine.EfficiencyFactor = 1f;
            solverEngine.ChamberPressure = chamberPressure * barToPascal;
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

            seaLevelThrust = solverEngine.seaLevelThrust / 1000f;
            seaLevelIsp = solverEngine.seaLevelIsp;
            vacuumThrust = solverEngine.vacuumThrust / 1000f;
            vacuumIsp = solverEngine.vacuumIsp;
        }

        private void OnPropellantRatioChanged(BaseField arg1, object arg2)
        {
            Solve();
        }

        private void OnChamberPressureChanged(BaseField arg1, object arg2)
        {
            Solve();
        }

        private void OnNozzleDiameterChanged(BaseField arg1, object arg2)
        {
            Solve();
        }

        private void OnExpansionRatioChanged(BaseField arg1, object arg2)
        {
            Solve();
        }

        private void OnPropellantChanged(BaseField arg1, object arg2)
        {
            BiPropellantConfig biprop;
            var found = EngineDesignerKSP.Instance.TryGetBipropellant(fuelString, oxidizerString, out biprop);
            if (!found)
            {
                Debug.Log($"[ED] Error - Could not find biprop for {fuelString} & {oxidizerString}");
                return;
            }
            solverEngine.PropellantMixture = BiPropellant.CreateInstanceFromDataStore(biprop);
            Solve();
        }

        public void Initialize()
        {
            InitializeUI();
            // TEST CODE
            var biprop = BiPropellant.CreateInstanceFromDataStore(bipropellants[0]);
            solverEngine = new LiquidEngineSolver(biprop);
        }

        public void InitializeUI()
        {
            fuelOptions = new[] { "Kerosene" };
            oxidizerOptions = new[] { "LqdOxygen" };

            Fields[nameof(propellantRatio)].uiControlEditor.onFieldChanged += OnPropellantRatioChanged;
            Fields[nameof(chamberPressure)].uiControlEditor.onFieldChanged += OnChamberPressureChanged;
            Fields[nameof(nozzleDiameter)].uiControlEditor.onFieldChanged += OnNozzleDiameterChanged;
            Fields[nameof(expansionRatio)].uiControlEditor.onFieldChanged += OnExpansionRatioChanged;
            Fields[nameof(fuelString)].uiControlEditor.onFieldChanged += OnPropellantChanged;
            Fields[nameof(oxidizerString)].uiControlEditor.onFieldChanged += OnPropellantChanged;
            

            var fuelChooser = (UI_ChooseOption)GetWidget(this, nameof(fuelString));
            fuelChooser.options = fuelOptions;
            fuelChooser.display = fuelOptions;
            
            var oxidizerChooser = (UI_ChooseOption)GetWidget(this, nameof(oxidizerString));
            oxidizerChooser.options = oxidizerOptions;
            oxidizerChooser.display = oxidizerOptions;
        }
        
        private static UI_Control GetWidget(PartModule module, string fieldName)
        {
            if (!HighLogic.LoadedSceneIsFlight && !HighLogic.LoadedSceneIsEditor) return null;
            BaseField bf = module.Fields[fieldName];
            return HighLogic.LoadedSceneIsEditor ? bf.uiControlEditor : bf.uiControlFlight;
        }    
    }
}
