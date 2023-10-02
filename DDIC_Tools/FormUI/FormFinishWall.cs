using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using DDIC_Tools.Data;
using DDIC_Tools.FormEventHandler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DDIC_Tools.FormUI
{
    public partial class FormFinishWall : System.Windows.Forms.Form
    {
        private ExternalEvent _externalEvent;

        private FinishWallHandler _eventHandler;

        private Document doc;

        private UIDocument uidoc;

        public FormFinishWall(UIDocument uidoc, Document doc)
        {
            InitializeComponent();
            this.uidoc = uidoc;
            this.doc = doc;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (rdAll.Checked == true)
            {
                IList<Element> elements = new FilteredElementCollector(doc)
                     .OfCategory(BuiltInCategory.OST_Rooms)
                     .WhereElementIsNotElementType()
                     .Cast<Element>()
                     .ToList();

                if (elements.Count > 0)
                {
                    foreach (Element element in elements)
                    {
                        ElementId id = element.Id;

                        uidoc.Selection.SetElementIds(new List<ElementId> { id });

                        Room room = element as Room;

                        FinishWall(doc, room, element);
                    }

                    TaskDialog.Show("Thông báo", "Tạo lớp trát thành công!");

                    this.Close();
                }
            }
            else
            {
                DataWallType selectedData = (DataWallType)lstWallType.SelectedItem;
                string height = txtHeight.Text;

                _eventHandler = new FinishWallHandler(selectedData, height);
                _externalEvent = ExternalEvent.Create(_eventHandler);

                _externalEvent.Raise();

                this.Close();
            }
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

                double height = double.Parse(txtHeight.Text) / 304.8;

                DataWallType selectedData = (DataWallType)lstWallType.SelectedItem;

                foreach (Curve curve in lines)
                {
                    Wall wall = Wall.Create(doc, curve, selectedData.Id, element.LevelId, height, selectedData.Width / 2, false, false);

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

                    if (door.FromRoom != null)
                    {
                        if (door.FromRoom.Id == room.Id)
                        {
                            JoinWall(doc, wallIds, hostElement);
                        }
                    }
                    else if (door.Room != null)
                    {
                        if (door.Room.Id == room.Id)
                        {
                            JoinWall(doc, wallIds, hostElement);
                        }
                    }
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

                    if (window.FromRoom != null)
                    {
                        if (window.FromRoom.Id == room.Id)
                        {
                            JoinWall(doc, wallIds, hostElement);
                        }
                    }
                    else if (window.Room != null)
                    {
                        if (window.Room.Id == room.Id)
                        {
                            JoinWall(doc, wallIds, hostElement);
                        }
                    }
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

                        if (intersects == true)
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormFinishWall_Load(object sender, EventArgs e)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc)
                .OfClass(typeof(WallType));

            foreach (WallType wallType in collector)
            {
                DataWallType data = new DataWallType();
                data.Name = wallType.Name;
                data.Id = wallType.Id;
                data.Width = wallType.Width;

                lstWallType.Items.Add(data);
            }

            lstWallType.SelectedIndex = lstWallType.Items.Count - 1;
        }
    }
}
