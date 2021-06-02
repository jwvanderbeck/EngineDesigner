namespace Simulation.Engines.Liquid
{
    public class Propellant
    {
        public string title;
        public float density;
        public float freezingPoint;
        public float meltingPoint;
        public float boilingPoint;
    }

    public class Oxidizer : Propellant { }

    public class Fuel : Propellant { }
}
