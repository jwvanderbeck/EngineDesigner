using System;
using UnityEngine;

namespace Simulation.Engines.Liquid
{
    [CreateAssetMenu][Serializable]
    public class PropellantAsset : ScriptableObject
    {
        [Serializable]
        public enum PropellantType
        {
            Oxidizer,
            Fuel
        }
        
        public string title;
        public PropellantType propellantType;
        public float density;
        public float freezingPoint;
        public float meltingPoint;
        public float boilingPoint;
    }
}
