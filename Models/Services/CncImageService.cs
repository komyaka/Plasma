using System;
using System.Collections.Generic;
using System.IO;
using Plazma.Models;
using Plazma.Models.NC;

namespace Plazma.Models.Services
{
    public class CncImageService
    {
        public bool TryResolveCncFilePath(string sourcePath, out string fullPath)
        {
            fullPath = string.Empty;
            if (string.IsNullOrWhiteSpace(sourcePath) || sourcePath.Contains(".."))
            {
                return false;
            }
            string basePath = Path.GetFullPath(Constants.CNCPath)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                + Path.DirectorySeparatorChar;
            string candidate = Path.GetFullPath(sourcePath);
            if (!candidate.StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            fullPath = candidate;
            return true;
        }

        public List<cnc.step> ReadPrimitives(string cncPath)
        {
            cnc cncFile = new cnc(cncPath);
            cncFile.AllPrimitives.Add(new cnc.step
            {
                textline = "",
                Xstart = cncFile.Sheet.minX,
                Ystart = cncFile.Sheet.minY,
                Xend = cncFile.Sheet.maxX,
                Yend = cncFile.Sheet.maxY
            });
            return cncFile.AllPrimitives;
        }

        public _SVGAnswer BuildSvg(string cncPath, bool addOffset)
        {
            cnc cncFile = new cnc(cncPath);
            cncFile.AllPrimitives.Add(new cnc.step
            {
                textline = "",
                Xstart = cncFile.Sheet.minX,
                Ystart = cncFile.Sheet.minY,
                Xend = cncFile.Sheet.maxX,
                Yend = cncFile.Sheet.maxY
            });

            _SVGAnswer answer = new _SVGAnswer
            {
                minX = (int)cncFile.Sheet.minX,
                minY = (int)cncFile.Sheet.minY,
                maxX = (int)cncFile.Sheet.maxX,
                maxY = (int)cncFile.Sheet.maxY
            };

            string svg = string.Empty;
            foreach (cnc.step s in cncFile.AllPrimitives)
            {
                switch (s.Comand)
                {
                    case cnc.comand.fastmove:
                        svg += " M" + s.Xend + " " + s.Yend;
                        break;
                    case cnc.comand.cuteline:
                        svg += " L" + s.Xend + " " + s.Yend;
                        break;
                    case cnc.comand.cutearc:
                        svg += " A" + s.Radius + " " + s.Radius + " 0 " + (s.SweepAngle < 180 ? "0 0 " : "1 0 ") + (addOffset ? s.Xend + 0.1 : s.Xend) + " " + (addOffset ? s.Yend + 0.1 : s.Yend);
                        break;
                    case cnc.comand.cutearc2:
                        svg += " A" + s.Radius + " " + s.Radius + " 0 " + (s.SweepAngle < 180 ? "0 1 " : "1 1 ") + (addOffset ? s.Xend + 0.1 : s.Xend) + " " + (addOffset ? s.Yend + 0.1 : s.Yend);
                        break;
                    default:
                        break;
                }
            }

            answer.path = svg;
            return answer;
        }
    }
}
