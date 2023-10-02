using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DDIC_Tools.ComponentFuncs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDIC_Tools.FormEventHandler
{
    public class CopyBlockCAD : IExternalEventHandler
    {
        private List<XYZ> points;
        private List<string> blocks;
        private string lstSelected;

        public CopyBlockCAD(List<XYZ> points, List<string> blocks, string lstSelected)
        {
            this.points = points;
            this.blocks = blocks;
            this.lstSelected = lstSelected;
        }

        public void Execute(UIApplication app)
        {
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = app.ActiveUIDocument.Document;

            Reference r = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);

            if (r != null)
            {
                LocationPoint point = doc.GetElement(r).Location as LocationPoint;
                XYZ rPoint = new XYZ(point.Point.X, point.Point.Y, point.Point.Z);

                XYZ t = CommonFunctions.ToPoint(rPoint);

                for (int i = 0; i < blocks.Count; i++)
                {
                    if (lstSelected.Contains(blocks[i]))
                    {
                        XYZ p = new XYZ((points[i].X - t.X) / 304.8, (points[i].Y - t.Y) / 304.8, 0);

                        using (Transaction trans = new Transaction(doc, "Copy Element"))
                        {
                            trans.Start();

                            ElementTransformUtils.CopyElement(doc, r.ElementId, p);

                            trans.Commit();
                        }
                    }
                }

                using (Transaction trans = new Transaction(doc, "Delete element"))
                {
                    trans.Start();

                    doc.Delete(r.ElementId);

                    trans.Commit();
                }

                TaskDialog.Show("Thông báo", "Copy đối tượng thành công!");
            }
        }

        public string GetName()
        {
            return "CopyBlockCAD";
        }
    }
}
