using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDIC_Tools.Data
{
    public class FormworkFace
    {
        public Face HostFace;
        public Face ModifiedFace;
        public Element HostElement;
        public Solid Geometry;
        public string HostID;
        public string ErrorID;
        public string ShapeKey;
        public double Area;
        public double X;
        public double Y;
        public double Z;
    }
}
