using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DDIC_Tools.ComponentFuncs;
using DDIC_Tools.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace DDIC_Tools.Command
{
    [Transaction(TransactionMode.Manual)]
    public class FormWorkBeam : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            List<FormworkFace> formworkFaceList = new List<FormworkFace>();
            List<Element> elementList = new List<Element>();
            List<ElementId> elementIds = new List<ElementId>();

            new FilteredElementCollector(doc, doc.ActiveView.Id).WhereElementIsNotElementType();
            Dictionary<string, ElementContainer> dictionary = new Dictionary<string, ElementContainer>();

            try
            {
                IEnumerable<IList<Element>> list = new ElementCollect(uidoc).GetVisibleElements().GroupBy(x => x.Document.PathName).Select(x => x.ToList()).ToList();
                foreach (IList<Element> source in list)
                {
                    List<ElementId> elementIds2 = new List<ElementId>();

                    foreach (Element element in (IEnumerable<Element>)source)
                    {
                        elementIds2.Add(element.Id);
                    }

                    LogicalOrFilter logicalOrFilter = new LogicalOrFilter(new List<ElementFilter>() {
                        new ElementCategoryFilter(BuiltInCategory.OST_Floors),
                        new ElementCategoryFilter(BuiltInCategory.OST_Walls),
                        new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming),
                        new ElementCategoryFilter(BuiltInCategory.OST_StructuralColumns),
                        new ElementCategoryFilter(BuiltInCategory.OST_Stairs),
                    });

                    FilteredElementCollector elementCollector = new FilteredElementCollector(source.FirstOrDefault().Document, elementIds2);
                    elementCollector.WherePasses(logicalOrFilter);

                    foreach (Element element1 in (IEnumerable<Element>)elementCollector.ToElements())
                    {
                        ElementContainer elementContainer = new ElementContainer();
                        elementContainer.E = element1;
                        elementContainer.ElementDoc = element1.Document;
                        elementContainer.UniqueGuid = element1.UniqueId;

                        dictionary.Add(element1.UniqueId, elementContainer);
                    }
                }

                CategorySet C_Set = doc.Application.Create.NewCategorySet();
                Category category1 = doc.Settings.Categories.get_Item(BuiltInCategory.OST_GenericModel);
                Category category2 = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Walls);

                C_Set.Insert(category1);
                C_Set.Insert(category2);

                DataTools.AddSharedParameterToCategory(commandData, C_Set, "SurfaceArea", "DDIC_Tools", ParameterType.Area);
                DataTools.AddSharedParameterToCategory(commandData, C_Set, "HostID", "DDIC_Tools", ParameterType.Text, true);
                DataTools.AddSharedParameterToCategory(commandData, C_Set, "Width", "DDIC_Tools", ParameterType.Length, true);
                DataTools.AddSharedParameterToCategory(commandData, C_Set, "Height", "DDIC_Tools", ParameterType.Length, true);
            }
            catch (Exception ex) { }

            int count1 = dictionary.Count;

            foreach (KeyValuePair<string, ElementContainer> keyValuePair in dictionary)
            {
                Element e = keyValuePair.Value.E;

                try
                {
                    IList<Element> IntersectingElements = GeometeryTools.SuperGetElementBoundingBox(e, commandData);
                    IList<Solid> elementSolids = GeometeryTools.GetElementSolids(e);

                    foreach (Element element in (IEnumerable<Element>)IntersectingElements)
                    {
                        if (!dictionary.ContainsKey(element.UniqueId))
                            IntersectingElements.Remove(element);
                    }

                    foreach (Solid solid in (IEnumerable<Solid>)elementSolids)
                    {
                        foreach (Face face in solid.Faces)
                        {
                            Face F = null;
                            XYZ normal = face.ComputeNormal(new UV(0.5, 0.5));

                            switch (e.Category.Id.IntegerValue)
                            {
                                case (int)BuiltInCategory.OST_StructuralColumns:
                                    if (normal.Z < 0.98 && normal.Z > -0.98)
                                    {
                                        F = face;
                                        break;
                                    }
                                    break;

                                case (int)BuiltInCategory.OST_StructuralFraming:
                                    if (normal.Z < 0.98)
                                    {
                                        F = face;
                                        break;
                                    }

                                    break;

                                case (int)BuiltInCategory.OST_Floors:
                                    if (normal.Z < 0.98)
                                    {
                                        F = face;
                                        break;
                                    }

                                    break;

                                case (int)BuiltInCategory.OST_Walls:
                                    if (normal.Z < 0.98 && normal.Z > -0.98)
                                    {
                                        F = face;
                                        break;
                                    }

                                    PlanarFace planarFace1 = face as PlanarFace;

                                    if (planarFace1.Origin.Z != e.get_BoundingBox(doc.ActiveView).Max.Z && planarFace1.Origin.Z != e.get_BoundingBox(doc.ActiveView).Min.Z)
                                    {
                                        F = face;
                                        break;
                                    }

                                    break;

                                default:
                                    if (normal.Z < 0.98)
                                    {
                                        F = face;
                                        break;
                                    }

                                    break;
                            }

                            if (e.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Walls || e.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns)
                            {
                                List<ElementId> elementIds3 = new List<ElementId>();

                                foreach (Element element in (IEnumerable<Element>)IntersectingElements)
                                {
                                    elementIds3.Add(element.Id);
                                }

                                FilteredElementCollector elementCollector = new FilteredElementCollector(doc, elementIds3);
                                elementCollector.WherePasses(new LogicalOrFilter(new List<ElementFilter>()
                                {
                                    new ElementCategoryFilter(BuiltInCategory.OST_Walls),
                                    new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming),
                                }));

                                IntersectingElements = elementCollector.ToElements() as List<Element>;
                            }

                            try
                            {
                                formworkFaceList.AddRange(SupportFunctions.FormworkCalculator(F, IntersectingElements, e));
                            }
                            catch (Exception ex) { }
                        }
                    }

                    using (Transaction tr = new Transaction(doc, "Formwork Element Created"))
                    {
                        tr.Start();

                        int coun2 = formworkFaceList.Count;

                        foreach (FormworkFace F in formworkFaceList)
                        {
                            try
                            {
                                SupportFunctions.FaceCreator(F, doc);
                            }
                            catch (Exception ex)
                            {
                                elementIds.Add(F.HostElement.Id);
                            }
                        }

                        tr.Commit();
                    }
                }
                catch
                {

                }
            }

            TaskDialog.Show("FormworkTools", "Run completed Successfully");

            return Result.Succeeded;
        }
    }
}
