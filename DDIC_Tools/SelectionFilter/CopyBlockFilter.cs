using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDIC_Tools.SelectionFilter
{
    public class CopyBlockFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem is ImportInstance importInstance && importInstance.IsLinked;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
