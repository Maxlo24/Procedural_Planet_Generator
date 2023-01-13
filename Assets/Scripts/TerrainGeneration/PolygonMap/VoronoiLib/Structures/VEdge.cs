namespace VoronoiLib.Structures
{
    public class VEdge
    {
        public VPoint Start { get; internal set; }
        public VPoint End { get; internal set; }
        public FortuneSite Left { get; set;  }
        public FortuneSite Right { get; set;  }
        public bool border { get; set; } = false;
        public double SlopeRise { get; }
        public double SlopeRun { get; }
        public double? Slope { get; }
        internal double? Intercept { get; }

        public VEdge Neighbor { get; internal set; }

        public void Reverse()
        {
            var temp = Start;
            Start = End;
            End = temp;

            var temp2 = Left;
            Right = Left;
            Left = temp2;
        }

        public VEdge(VPoint start, VPoint end)
        {
            Start = start;
            End = end;
        }

        internal VEdge(VPoint start, FortuneSite left, FortuneSite right)
        {
            Start = start;
            Left = left;
            Right = right;
            
            //for bounding box edges
            if (left == null || right == null)
                return;

            //from negative reciprocal of slope of line from left to right
            //ala m = (left.y -right.y / left.x - right.x)
            SlopeRise = left.X - right.X;
            SlopeRun = -(left.Y - right.Y);
            Intercept = null;

            if (SlopeRise.ApproxEqual(0) || SlopeRun.ApproxEqual(0)) return;
            Slope = SlopeRise/SlopeRun;
            Intercept = start.Y - Slope*start.X;
        }
    }
}
