using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using DDIC_Tools.Data;
using DDIC_Tools.FormEventHandler;
using DDIC_Tools.ComponentFuncs;
using System;
using System.Collections.Generic;
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

        private FinishWallSetup FinishWallSetup = new FinishWallSetup();

        public FormFinishWall(UIDocument uidoc, Document doc)
        {
            InitializeComponent();
            this.uidoc = uidoc;
            this.doc = doc;
        }

        private void FormFinishWall_Load(object sender, EventArgs e)
        {
            IEnumerable<WallType> wallTypes = new FilteredElementCollector(doc).OfClass(typeof(WallType)).Select(elem => new
            {
                elem = elem,
                type = elem as WallType
            }).Where(p => p.type.Kind == 0).Select(p => p.type);

            lstWallType.DataSource = wallTypes.ToList();
            lstWallType.DisplayMember = "Name";
            lstWallType.SelectedItem = lstWallType.Items[lstWallType.Items.Count - 1];
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (ConvertValue.GetValueFromString(txtHeight.Text, doc.GetUnits()).HasValue)
            {
                this.FinishWallSetup.JoinWall = ckJoinWall.Checked;
                this.FinishWallSetup.BoardHeight = ConvertValue.GetValueFromString(txtHeight.Text, doc.GetUnits()).Value;
                if (lstWallType.SelectedItems == null) return;
                this.FinishWallSetup.SelectedWallType = lstWallType.SelectedItems[0] as WallType;

                if (rdAll.Checked)
                {
                    this.FinishWallSetup.SelectedRooms = SelectRooms().ToList();

                    try
                    {
                        CreateFinishWall();
                        this.Close();
                        TaskDialog.Show("Notification", "Create finish wall successfully!", TaskDialogCommonButtons.Close, TaskDialogResult.Close);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    _eventHandler = new FinishWallHandler(this.FinishWallSetup);
                    _externalEvent = ExternalEvent.Create(_eventHandler);

                    _externalEvent.Raise();

                    this.Close();
                }
            }
            else
            {
                TaskDialog.Show("Create skirting board", "Please enter a height value for the skirting board!", TaskDialogCommonButtons.Close, TaskDialogResult.Close);
                this.Activate();
            }
        }

        private IEnumerable<Room> SelectRooms()
        {
            IEnumerable<Room> source = null;

            source = new FilteredElementCollector(doc, doc.ActiveView.Id).OfClass(typeof(SpatialElement)).WhereElementIsNotElementType().Select(elem => new
            {
                elem = elem,
                room = elem as Room
            }).Select(p => p.room);

            return source;
        }

        public void CreateFinishWall()
        {
            Transaction tx = new Transaction(this.doc);
            tx.Start("Create skirting board");

            WallType newWallType = DuplicateWallType(this.FinishWallSetup.SelectedWallType, this.doc);
            Dictionary<ElementId, ElementId> walls = CreateWalls(this.doc, FinishWallSetup.SelectedRooms, this.FinishWallSetup.BoardHeight, newWallType);

            foreach (ElementId key in new List<ElementId>(walls.Keys))
            {
                if (this.doc.GetElement(key) == null)
                {
                    walls.Remove(key);
                }
            }

            Element.ChangeTypeId(this.doc, walls.Keys, FinishWallSetup.SelectedWallType.Id);
            if (this.FinishWallSetup.JoinWall)
            {
                JoinGeometry(this.doc, walls);
            }

            this.doc.Delete(newWallType.Id);

            tx.Commit();
        }

        public WallType DuplicateWallType(WallType wallType, Document doc)
        {
            WallType wallType1 = new FilteredElementCollector(doc).OfClass(typeof(WallType)).Select(elem => new
            {
                elem = elem,
                type = elem as WallType
            }).Where(p => p.type.Kind == 0).Select(p => p.type).Select(o => o.Name).ToList().Contains("newWallTypeName") ? wallType.Duplicate("newWallTypeName2") as WallType : wallType.Duplicate("newWallTypeName") as WallType;

            CompoundStructure compoundStructure = wallType1.GetCompoundStructure();
            IList<CompoundStructureLayer> layers = compoundStructure.GetLayers();
            int num1 = 0;

            foreach (CompoundStructureLayer compoundStructureLayer in layers)
            {
                double num2 = compoundStructureLayer.Width * 2.0;

                compoundStructure.SetLayerWidth(num1, num2);

                ++num1;
            }

            wallType1.SetCompoundStructure(compoundStructure);

            return wallType1;
        }

        public Dictionary<ElementId, ElementId> CreateWalls(Document doc, IEnumerable<Room> modelRooms, double height, WallType newWallType)
        {
            Dictionary<ElementId, ElementId> walls = new Dictionary<ElementId, ElementId>();

            foreach (Room modelRoom in modelRooms)
            {
                ElementId levelId = modelRoom.LevelId;
                IList<IList<BoundarySegment>> boundarySegments = modelRoom.GetBoundarySegments(new SpatialElementBoundaryOptions()
                {
                    SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Finish
                });

                if (boundarySegments != null)
                {
                    foreach (IList<BoundarySegment> boundarySegmentList in boundarySegments)
                    {
                        if (boundarySegmentList.Count != 0)
                        {
                            foreach (BoundarySegment boundarySegment in boundarySegmentList)
                            {
                                Element element = doc.GetElement(boundarySegment.ElementId);
                                if (element != null)
                                {
                                    Category category = doc.Settings.Categories.get_Item(BuiltInCategory.OST_RoomSeparationLines);

                                    if (element.Category.Id != category.Id)
                                    {
                                        Wall wall = Wall.Create(doc, boundarySegment.GetCurve(), newWallType.Id, levelId, height, 0.0, false, false);
                                        wall.get_Parameter(BuiltInParameter.WALL_KEY_REF_PARAM).Set(2);
                                        walls.Add(wall.Id, boundarySegment.ElementId);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return walls;
        }

        public void JoinGeometry(Document doc, Dictionary<ElementId, ElementId> wallDictionary)
        {
            foreach (ElementId key in wallDictionary.Keys)
            {
                if (doc.GetElement(key) is Wall elem)
                {
                    elem.get_Parameter(BuiltInParameter.WALL_KEY_REF_PARAM).Set(3);

                    if (doc.GetElement(wallDictionary[key]) is Wall element)
                    {
                        JoinGeometryUtils.JoinGeometry(doc, elem, element);
                    }
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
