using System;

namespace Simulation.Engines.Liquid
{
    public class LiquidEngineMetrics
    {
        public const float pi = (float)Math.PI;
        public float efficiencyFactor = 1f;
        public float chamberPressure;            // Pa
        public float combustionTemperature;      // K                
        public float nozzleDiameter;             // Meters
        public float expansionRatio;            // *Area* of the nozzle exit divided by the area of the throat
        
        // transients
        public float specificHeatRatio;
        public float molecularWeight;
        public float pressureRatio;
        
        const float gasConstant = 8314.4621f; // metric, J/kmol-K
        

        public float ThrustAtPressure(float atmospherePressure)
        {
            SolvePressureRatio();
            float Ve = IdealExhaustVelocity();
            float mdot = MassFlowRate();
            float Ae = (pi * nozzleDiameter * nozzleDiameter) / 4f;
            float Pe = chamberPressure * pressureRatio;

            float thrust = mdot * Ve + (Pe - atmospherePressure) * Ae;

            return thrust;
        }    
        
        public float IdealExhaustVelocity()
        {
            SolvePressureRatio();
            float term1 = (2f * specificHeatRatio) / (specificHeatRatio - 1f);
            float term2 = (gasConstant * combustionTemperature) / molecularWeight;
            float term3Exp = (specificHeatRatio - 1f) / specificHeatRatio;
            float term3 = 1f - (float)Math.Pow(((chamberPressure * pressureRatio) / chamberPressure), term3Exp);

            float Ve = (float)Math.Sqrt(term1 * term2 * term3);

            return Ve * efficiencyFactor;
        }

        public float MassFlowRate()
        {
            float P0 = chamberPressure;
            float T0 = combustionTemperature;
            float gamma = specificHeatRatio;
            float R = gasConstant / molecularWeight;
            float Ae = (pi * nozzleDiameter * nozzleDiameter) / 4f;
            float AStar = Ae / expansionRatio;

            float term1 = (P0 * AStar) / (float)Math.Sqrt(T0);
            float term2 = (float)Math.Pow((gamma / R) * (float)Math.Pow(2f / (gamma + 1), (gamma + 1f) / (gamma - 1f)), 0.5f);

            return term1 * term2;
        }

        public float SolvePressureRatio()
        {
            var pgam = specificHeatRatio + 1f;
            var mgam = specificHeatRatio - 1f;
            var mach = MachNumber();
            var mach2 = mach * mach;
            var mach3 = mach2 - 1f;
            var factor = 1.0f + (0.5f * mgam * mach2);
            pressureRatio = (float)Math.Pow(1.0f / factor, specificHeatRatio / mgam);
            
            return pressureRatio;
        }
        
        /*
         * Original Isentropic calculations from NASA website: https://www.grc.nasa.gov/www/k-12/airplane/jsfiles/isentrop.js
            var pgam = parseFloat(gamma)+1;
            var mgam = parseFloat(gamma)-1;
            var mach2 = mach*mach;
            var mach3 = parseFloat(mach2)-1.0;
            var factor = 1.0 + (0.5 * mgam * mach2);
            var iselect = $("#inputvals").val();
            prat = Math.pow(1.0/factor, gamma/mgam);
            trat = 1.0/factor;
            drat = Math.pow(1.0/factor, 1.0/mgam);
            var factor2 = pgam / (2.0 * mgam);
            arat = mach * Math.pow(factor, -1*factor2) * Math.pow(pgam/2.0, factor2);
            arat = 1.0/arat;
            dynrat = gamma*mach2 * 0.5;
            wcor = getAir(mach);
            mu = (Math.asin(1.0/mach))/(3.14159/180);

            var nval1 = Math.sqrt((pgam)/(mgam));
            var nv2 = Math.sqrt((mgam*mach3)/pgam);
            var nval2 = Math.atan(nv2) * (180/3.14159);
            var nval3 = Math.atan(Math.sqrt(mach3))*(180/3.14159);
            nu = (nval1*nval2) - nval3;
        */

        float MachNumber()
        {
            var f1 = (specificHeatRatio + 1f) / (2.0f * (specificHeatRatio - 1f));
            var a0 = 2.0f;
            var m0 = 2.2f;
            var m1 = 0f;
            var fac = 0f;
            var a1 = 0f;
            var am = 0f;
            m1 = m0 + 0.05f;
            while(Math.Abs(expansionRatio - a0) > 0.0001)
            {
                fac = 1.0f + 0.5f * (specificHeatRatio - 1f) * m1 * m1;
                a1 = 1.0f / (m1 * (float)Math.Pow(fac, - 1f * f1) * (float)Math.Pow((specificHeatRatio + 1f) / 2.0f, f1));
                am = (a1 - a0) / (m1 - m0);
                a0 = a1;
                m0 = m1;
                m1 = m0 + ((expansionRatio - a0) / am);
            }
            var mach = m0;
            return mach;
        }
        
        /*
         * Original calculation for Mach number based on Area Ratio from NASA website: https://www.grc.nasa.gov/www/k-12/airplane/jsfiles/isentrop.js
                var getMachArat = function(){
                var f1 = (gamma+1)/(2.0*(gamma-1));
                var a0 = 2.0;
                var m0 = 2.2;
                var m1=0;
                var fac=0;
                var a1=0;
                var am = 0;
                var sub = document.getElementById('sonic').value;
                if(sub == 1){
                  m0 = 0.3;
                }
                m1 = m0 + 0.05;
                while(Math.abs(arat-a0) > 0.0001){
                  fac = 1.0 + 0.5*(gamma-1)*m1*m1;
                  a1 = 1.0/(m1 * Math.pow(fac, -1*f1) * Math.pow((gamma+1)/2.0, f1));
                  am = (a1-a0)/(m1-m0);
                  a0 = a1;
                  m0 = m1;
                  m1 = m0 + ((arat - a0)/am);
                }
                mach = m0;
                return mach;
              } 
        */
    }
}
