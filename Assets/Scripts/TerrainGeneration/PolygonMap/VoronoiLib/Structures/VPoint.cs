namespace VoronoiLib.Structures
{
    public class VPoint
    {
        public double X { get; set; }
        public double Y { get; set; }

        internal VPoint(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}
