using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDIC_Tools.ComponentFuncs
{
    public class GeometeryTools
    {
        public static IList<Element> SuperGetElementBoundingBox(Element E, ExternalCommandData CB)
        {
            List<Document> documentList = new List<Document>();
            List<Element> elementBoundingBox = new List<Element>();

            foreach (Document document in CB.Application.Application.Documents)
            {
                documentList.Add(document);
            }

            documentList.Add(CB.Application.ActiveUIDocument.Document);

            foreach (Document document1 in documentList)
            {
                View activeView = CB.Application.ActiveUIDocument.Document.ActiveView;
                BoundingBoxXYZ boundingBoxXYZ = E.get_BoundingBox(activeView);
                Outline outline = new Outline(boundingBoxXYZ.Min, boundingBoxXYZ.Max);
                outline.Scale(1.2);

                FilteredElementCollector elementCollector = new FilteredElementCollector(document1).WhereElementIsNotElementType();
                elementCollector.WherePasses(new LogicalOrFilter(new List<ElementFilter>()
                {
                    new ElementCategoryFilter(BuiltInCategory.OST_Floors),
                    new ElementCategoryFilter(BuiltInCategory.OST_Walls),
                    new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming),
                    new ElementCategoryFilter(BuiltInCategory.OST_StructuralColumns),
                    new ElementCategoryFilter(BuiltInCategory.OST_StructuralFoundation),
                }));

                BoundingBoxIntersectsFilter intersectsFilter = new BoundingBoxIntersectsFilter(outline);
                elementCollector.WherePasses(intersectsFilter);
                elementBoundingBox.AddRange(elementCollector.ToElements() as List<Element>);
            }

            return elementBoundingBox;
        }

        public static IList<Solid> GetElementSolids(Element E)
        {
            List<Solid> elementSolids = new List<Solid>();

            foreach (GeometryObject geometryObject in E.get_Geometry(new Options()
            {
                ComputeReferences = false,
                DetailLevel = ViewDetailLevel.Fine
            }))
            {
                if (geometryObject is Solid)
                {
                    Solid solid = geometryObject as Solid;
                    if (solid.Volume != 0.0)
                    {
                        elementSolids.Add(solid);
                    }
                }
            }

            return elementSolids;
        }

        public static Element PlaceDirectShapeSpecial(Solid S, Document D, BuiltInCategory BC, string ObjName)
        {
            Solid solid = S;
            DirectShapeLibrary directShapeLibrary = DirectShapeLibrary.GetDirectShapeLibrary(D);
            DirectShapeType directShapeType = DirectShapeType.Create(D, ObjName, new ElementId(BC));
            directShapeType.SetShape((IList<GeometryObject>)new List<GeometryObject>()
      {
        (GeometryObject) solid
      });
            directShapeLibrary.AddDefinitionType(ObjName, ((Element)directShapeType).Id);
            DirectShape elementInstance = DirectShape.CreateElementInstance(D, ((Element)directShapeType).Id, ((Element)directShapeType).Category.Id, ObjName, Transform.Identity);
            elementInstance.SetTypeId(((Element)directShapeType).Id);
            elementInstance.ApplicationId = "Application id";
            elementInstance.ApplicationDataId = "Geometry object id";
            elementInstance.SetShape((IList<GeometryObject>)new GeometryObject[1]
            {
        (GeometryObject) solid
            });
            ((Element)directShapeType).Dispose();
            directShapeLibrary.Dispose();
            return (Element)elementInstance;
        }
    }
}
