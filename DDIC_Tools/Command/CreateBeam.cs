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
    public class CreateBeam : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            ISelectionFilter filter = new DoorAndWindowFilter();

            IList<Reference> references = uidoc.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element, filter);

            if (references != null)
            {
                IList<Element> list = new List<Element>();

                foreach (Reference r in references)
                {
                    list.Add(doc.GetElement(r));
                }

                if (list.Count > 0)
                {
                    FormCreateBeam form = new FormCreateBeam(doc, list);
                    form.ShowDialog();
                }
            }

            return Result.Succeeded;
        }
    }
}
