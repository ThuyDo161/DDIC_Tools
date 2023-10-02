using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDIC_Tools.ComponentFuncs
{
    public static class CommonFunctions
    {
        public static XYZ ToPoint(XYZ point)
        {
            double x = UnitUtils.ConvertFromInternalUnits(point.X, UnitTypeId.Millimeters);
            double y = UnitUtils.ConvertFromInternalUnits(point.Y, UnitTypeId.Millimeters);

            XYZ result = new XYZ(x, y, 0);

            return result;
        }
    }
}
