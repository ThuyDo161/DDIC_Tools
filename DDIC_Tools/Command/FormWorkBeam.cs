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
            UIDocument activeUiDocument = commandData.Application.ActiveUIDocument;
            Document document = activeUiDocument.Document;
            Application application = commandData.Application.Application;

            List<Exception> exceptionList = new List<Exception>();
            List<FormworkFace> formworkFaceList = new List<FormworkFace>();
            List<Element> elementList = new List<Element>();
            List<ElementId> elementIdList1 = new List<ElementId>();

            new FilteredElementCollector(document, (document.ActiveView).Id).WhereElementIsNotElementType();
            Dictionary<string, ElementContainer> dictionary = new Dictionary<string, ElementContainer>();

            try
            {
                foreach (IList<Element> source in (IEnumerable<List<Element>>)new ElementCollect(activeUiDocument).GetVisibleElements().GroupBy((x => x.Document.PathName)).Select((x => x.ToList())).ToList())
                {
                    List<ElementId> elementIdList2 = new List<ElementId>();
                    foreach (Element element in (IEnumerable<Element>)source)
                        elementIdList2.Add(element.Id);
                    LogicalOrFilter logicalOrFilter = new LogicalOrFilter(new List<ElementFilter>()
                      {
                        new ElementCategoryFilter((BuiltInCategory.OST_Floors)),
                        new ElementCategoryFilter((BuiltInCategory.OST_Walls)),
                        new ElementCategoryFilter((BuiltInCategory.OST_StructuralFraming)),
                        new ElementCategoryFilter((BuiltInCategory.OST_StructuralColumns)),
                        new ElementCategoryFilter((BuiltInCategory.OST_StructuralFoundation)),
                        new ElementCategoryFilter((BuiltInCategory.OST_Stairs))
                      });
                    FilteredElementCollector elementCollector = new FilteredElementCollector(source.FirstOrDefault<Element>().Document, elementIdList2);
                    elementCollector.WherePasses(logicalOrFilter);
                    foreach (Element element in (IEnumerable<Element>)elementCollector.ToElements())
                    {
                        ElementContainer elementContainer = new ElementContainer();
                        elementContainer.E = element;
                        elementContainer.ElementDoc = element.Document;
                        elementContainer.UniqueGuid = element.UniqueId;
                        dictionary.Add(element.UniqueId, elementContainer);
                    }
                }
                CategorySet C_Set = document.Application.Create.NewCategorySet();
                Category category1 = document.Settings.Categories.get_Item(BuiltInCategory.OST_Floors);
                Category category2 = document.Settings.Categories.get_Item(BuiltInCategory.OST_Walls);
                C_Set.Insert(category1);
                C_Set.Insert(category2);
                DataTools.AddSharedParameterToCategory(commandData, C_Set, "SurfaceArea", "DDIC_Tools", ParameterType.Area);
                DataTools.AddSharedParameterToCategory(commandData, C_Set, "HostID", "DDIC_Tools", ParameterType.Text, true);
                DataTools.AddSharedParameterToCategory(commandData, C_Set, "Width", "DDIC_Tools", ParameterType.Length, true);
                DataTools.AddSharedParameterToCategory(commandData, C_Set, "Height", "DDIC_Tools", ParameterType.Length, true);
            }
            catch (Exception ex)
            {
            }

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
                            Face F = (Face)null;
                            XYZ normal = face.ComputeNormal(new UV(0.5, 0.5));

                            switch (e.Category.Id.IntegerValue)
                            {
                                case -2001330:
                                    if (normal.Z < 0.98 && normal.Z > -0.98)
                                    {
                                        F = face;
                                        break;
                                    }
                                    break;
                                case -2001320:
                                    if (normal.Z < 0.98)
                                    {
                                        F = face;
                                        break;
                                    }
                                    break;
                                case -2000032:
                                    if (normal.Z < 0.98)
                                    {
                                        F = face;
                                        break;
                                    }
                                    break;
                                case -2000011:
                                    if (normal.Z < 0.98 && normal.Z > -0.98)
                                    {
                                        F = face;
                                        break;
                                    }
                                    PlanarFace planarFace2 = face as PlanarFace;
                                    if (planarFace2.Origin.Z != e.get_BoundingBox(document.ActiveView).Max.Z && planarFace2.Origin.Z != e.get_BoundingBox(document.ActiveView).Min.Z)
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

                            if ((e.Category.Id.IntegerValue == -2000011 || e.Category.Id.IntegerValue == -2001330))
                            {
                                List<ElementId> elementIdList3 = new List<ElementId>();
                                foreach (Element element in (IEnumerable<Element>)IntersectingElements)
                                    elementIdList3.Add(element.Id);
                                FilteredElementCollector elementCollector = new FilteredElementCollector(document, elementIdList3);
                                elementCollector.WherePasses(new LogicalOrFilter(new List<ElementFilter>()
                            {
                              new ElementCategoryFilter((BuiltInCategory.OST_Walls)),
                               new ElementCategoryFilter((BuiltInCategory.OST_StructuralColumns))
                            }));
                                IntersectingElements = (elementCollector.ToElements() as List<Element>);
                            }
                            try
                            {
                                formworkFaceList.AddRange(SupportFunctions.FormworkCalculator(F, IntersectingElements, e));
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    elementIdList1.Add(e.Id);
                }
            }

            using (Transaction transaction = new Transaction(document, "Formwork Element Created"))
            {
                transaction.Start();
                int count2 = formworkFaceList.Count;
                foreach (FormworkFace F in formworkFaceList)
                {
                    try
                    {
                        SupportFunctions.FaceCreator(F, document);
                    }
                    catch (Exception ex)
                    {
                        elementIdList1.Add(F.HostElement.Id);
                    }
                }
                transaction.Commit();
            }

            if (elementIdList1.Count == 0)
                TaskDialog.Show("FormworkTools", "Run completed Successfully");
            else
                TaskDialog.Show("FormworkTools", "Run completed Successfully");

            return Result.Succeeded;
        }
    }
}
