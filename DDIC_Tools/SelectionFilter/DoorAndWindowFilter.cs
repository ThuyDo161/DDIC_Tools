using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDIC_Tools.SelectionFilter
{
    public class DoorAndWindowFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Doors
                || elem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Windows;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
