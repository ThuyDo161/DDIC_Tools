using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using DDIC_Tools.FormUI;
using DDIC_Tools.SelectionFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDIC_Tools.Command
{
    [Transaction(TransactionMode.Manual)]
    public class CopyElementCAD : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            ISelectionFilter filter = new CopyBlockFilter();

            Reference r = uidoc.Selection.PickObject(ObjectType.Element, filter);

            if (r != null )
            {
                Element element = doc.GetElement(r);

                FormChooseBlock form = new FormChooseBlock(doc, element);
                form.ShowDialog();
            }

            return Result.Succeeded;
        }
    }
}
