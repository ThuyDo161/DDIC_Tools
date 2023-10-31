using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDIC_Tools.ComponentFuncs
{
    public class ConvertValue
    {
        public static double? GetValueFromString(string text, Units units)
        {
            string str = text;
            double num;

            return UnitFormatUtils.TryParse(units, SpecTypeId.Length, str, out num) ? new double?(num) : new double?();
        }
    }
}
