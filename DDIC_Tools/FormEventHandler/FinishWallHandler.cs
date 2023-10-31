using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using DDIC_Tools.Data;
using DDIC_Tools.SelectionFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DDIC_Tools.FormEventHandler
{
    public class FinishWallHandler : IExternalEventHandler
    {
        private FinishWallSetup FinishWallSetup;
        public FinishWallHandler(FinishWallSetup finishWallSetup)
        {
            FinishWallSetup = finishWallSetup;
        }

        public void Execute(UIApplication app)
        {
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = app.ActiveUIDocument.Document;

            this.FinishWallSetup.SelectedRooms = SelectRooms(uidoc, doc).ToList();

            try
            {
                CreateFinishWall(doc);
                TaskDialog.Show("Notification", "Create finish wall successfully!", TaskDialogCommonButtons.Close, TaskDialogResult.Close);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public string GetName()
        {
            return "FinishWallHandler";
        }

        private IEnumerable<Room> SelectRooms(UIDocument uidoc, Document doc)
        {
            ICollection<ElementId> elementIds = uidoc.Selection.GetElementIds();

            IEnumerable<Room> source = null;
            IList<Room> roomList = new List<Room>();

            if (elementIds.Count != 0)
            {
                source = new FilteredElementCollector(doc, elementIds).OfClass(typeof(SpatialElement)).Select(elem => new
                {
                    elem = elem,
                    room = elem as Room
                }).Select(_param1 => _param1.room);
                roomList = source.ToList();
            }

            if (roomList.Count == 0)
            {
                foreach (Reference r in uidoc.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element, new RoomSelectionFilter()))
                {
                    roomList.Add(doc.GetElement(r) as Room);
                }

                source = roomList;
            }

            return source;
        }

        private void CreateFinishWall(Document doc)
        {
            Transaction tx = new Transaction(doc);
            tx.Start("Create skirting board");

            WallType newWallType = DuplicateWallType(this.FinishWallSetup.SelectedWallType, doc);
            Dictionary<ElementId, ElementId> walls = CreateWalls(doc, FinishWallSetup.SelectedRooms, this.FinishWallSetup.BoardHeight, newWallType);

            foreach (ElementId key in new List<ElementId>(walls.Keys))
            {
                if (doc.GetElement(key) == null)
                {
                    walls.Remove(key);
                }
            }

            Element.ChangeTypeId(doc, walls.Keys, FinishWallSetup.SelectedWallType.Id);
            if (this.FinishWallSetup.JoinWall)
            {
                JoinGeometry(doc, walls);
            }

            doc.Delete(newWallType.Id);

            tx.Commit();
        }

        private WallType DuplicateWallType(WallType wallType, Document doc)
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

        private Dictionary<ElementId, ElementId> CreateWalls(Document doc, IEnumerable<Room> modelRooms, double height, WallType newWallType)
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

        private void JoinGeometry(Document doc, Dictionary<ElementId, ElementId> wallDictionary)
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
    }
}
