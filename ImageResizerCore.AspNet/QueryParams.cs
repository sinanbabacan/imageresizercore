using System.Drawing;

namespace ImageResizerCore.AspNet
{
    public class QueryParams
    {
        public bool hasParams;
        public int w;
        public int h;
        public int quality; 
        public string format;
        public string mode;
        public string crop;

        public PointF CropTopLeft
        {
            get
            {
                string[] points = crop.ToString().Split(',');

                var x1 = float.Parse(points[0]);
                var y1 = float.Parse(points[1]);

                return new PointF(x1, y1);
            }
        }

        public PointF CropBottomRight
        {
            get
            {
                string[] points = crop.ToString().Split(',');

                var x2 = float.Parse(points[2]);
                var y2 = float.Parse(points[3]);

                return new PointF(x2, y2);
            }
        }

        public static string[] modes = new string[] { "pad", "crop" };
    }
}