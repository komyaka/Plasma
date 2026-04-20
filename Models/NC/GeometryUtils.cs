using System;
using System.Globalization;

namespace Plazma.Models.NC
{
    public static class GeometryUtils
    {
        public static float GetValueFromLine(string symbol, ref string line)
        {
            if (line.IndexOf(symbol) < 0)
            {
                return 0;
            }

            string value = line.Substring(line.IndexOf(symbol) + 1);
            line = line.Substring(0, line.IndexOf(symbol));
            return (float)Convert.ToDouble(value, CultureInfo.InvariantCulture);
        }

        public static float Angle(float x0, float y0, float x1, float y1)
        {
            float dx = x1 - x0;
            float dy = y1 - y0;
            float length = (float)Math.Sqrt(dx * dx + dy * dy);
            if (dy < 0)
            {
                return (float)(90 - (Math.Asin(-dx / length)) * 180 / Math.PI);
            }

            return (float)(360 - ((Math.Asin(dx / length)) * 180 / Math.PI + 90));
        }
    }
}
