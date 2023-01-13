using System.Collections.Generic;

namespace VoronoiLib.Structures
{
    public enum ZoneType
    {
        Land,
        Water,
        Coast
    }
    
    public class FortuneSite
    {
        public double X { get; }
        public double Y { get; }

        public bool border { get; set; } = false;

        public List<VEdge> Cell { get; private set; }

        public List<FortuneSite> Neighbors { get; private set; }

        public FortuneSite(double x, double y)
        {
            X = x;
            Y = y;
            Cell = new List<VEdge>();
            Neighbors = new List<FortuneSite>();
        }
    }
}
