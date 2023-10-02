using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using DDIC_Tools.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDIC_Tools.FormEventHandler
{
    public class FinishWallHandler : IExternalEventHandler
    {
        private string txtHeight;

        private DataWallType lstWallType { get; set; }

        public FinishWallHandler(DataWallType selectedItem, string height)
        {
            lstWallType = selectedItem;
            txtHeight = height;
        }

        public void Execute(UIApplication app)
        {
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = app.ActiveUIDocument.Document;

            IList<Reference> r = uidoc.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element);

            if (r != null)
            {
                foreach (Reference reference in r)
                {
                    Element element = doc.GetElement(reference);

                    if (element.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Rooms))
                    {
                        ElementId id = reference.ElementId;

                        uidoc.Selection.SetElementIds(new List<ElementId> { id });

                        Room room = element as Room;

                        FinishWall(doc, room, element);
                    }
                    else
                    {
                        TaskDialog.Show("Thông báo", "Vui lòng chọn room!");
                    }
                }

                TaskDialog.Show("Thông báo", "Tạo lớp trát thành công!");
            }
        }

        public string GetName()
        {
            return "FinishWallHandler";
        }

        public void FinishWall(Document doc, Room room, Element element)
        {
            SpatialElementBoundaryOptions options = new SpatialElementBoundaryOptions();
            options.SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Finish;

            IList<IList<BoundarySegment>> boundaries = room.GetBoundarySegments(options);

            List<Curve> boundaryLines = new List<Curve>();

            List<Curve> lines = new List<Curve>();

            List<ElementId> wallIds = new List<ElementId>();

            double offsetDistance = 5 / 304.8;

            // Duyệt qua các đoạn đường biên và lưu chúng vào danh sách
            foreach (IList<BoundarySegment> boundary in boundaries)
            {
                foreach (BoundarySegment segment in boundary)
                {
                    Curve curve = segment.GetCurve();
                    boundaryLines.Add(curve);
                }
            }

            foreach (Curve curve in boundaryLines)
            {
                Curve offsetCurve = curve.CreateOffset(offsetDistance, new XYZ(0, 0, -1));

                lines.Add(offsetCurve);
            }

            using (Transaction trans = new Transaction(doc, "Create wall"))
            {
                trans.Start();

                double height = double.Parse(txtHeight) / 304.8;

                foreach (Curve curve in lines)
                {
                    Wall wall = Wall.Create(doc, curve, lstWallType.Id, element.LevelId, height, lstWallType.Width / 2, false, false);

                    wallIds.Add(wall.Id);
                }

                trans.Commit();
            }

            List<FamilyInstance> doors = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Doors)
                .OfClass(typeof(FamilyInstance))
                .Cast<FamilyInstance>()
                .ToList();

            if (doors.Count > 0)
            {
                foreach (FamilyInstance door in doors)
                {
                    ElementId hostId = door.Host.Id;
                    Element hostElement = doc.GetElement(hostId);

                    JoinWall(doc, wallIds, hostElement);
                }
            }

            List<FamilyInstance> windows = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Windows)
                .OfClass(typeof(FamilyInstance))
                .Cast<FamilyInstance>()
                .ToList();

            if (windows.Count > 0)
            {
                foreach (FamilyInstance window in windows)
                {
                    ElementId hostId = window.Host.Id;
                    Element hostElement = doc.GetElement(hostId);

                    JoinWall(doc, wallIds, hostElement);
                }
            }
        }

        public void JoinWall(Document doc, List<ElementId> wallIds, Element hostElement)
        {
            foreach (ElementId id in wallIds)
            {
                try
                {
                    Element ele = doc.GetElement(id);

                    using (Transaction trans = new Transaction(doc, "Join wall"))
                    {
                        trans.Start();

                        bool intersects = IsIntersect(ele, hostElement);

                        bool isJoined = JoinGeometryUtils.AreElementsJoined(doc, ele, hostElement);

                        if (intersects && !isJoined)
                        {
                            JoinGeometryUtils.JoinGeometry(doc, ele, hostElement);
                        }

                        trans.Commit();
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
        }

        public bool IsIntersect(Element ele, Element hostEle)
        {
            BoundingBoxXYZ boundingBoxXYZ = ele.get_BoundingBox(null);
            Outline outline = new Outline(boundingBoxXYZ.Min, boundingBoxXYZ.Max);

            BoundingBoxIntersectsFilter filter = new BoundingBoxIntersectsFilter(outline);

            return filter.PassesFilter(hostEle);
        }
    }
}
