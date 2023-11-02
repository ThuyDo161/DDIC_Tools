using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DDIC_Tools.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDIC_Tools.ComponentFuncs
{
    public class SupportFunctions
    {
        public static List<FormworkFace> FormworkCalculator(Face F, IList<Element> IntersectingElements, Element E)
        {
            Document document = E.Document;
            XYZ normal = F.ComputeNormal(new UV(0.5, 0.5));
            Face face1 = F;
            List<FormworkFace> formworkFaceList = new List<FormworkFace>();
            IList<CurveLoop> edgesAsCurveLoops = face1.GetEdgesAsCurveLoops();
            Solid solid1 = !(F is PlanarFace) ? ThickenCurvedSurface(F, 0.0328084) : GeometryCreationUtilities.CreateExtrusionGeometry(edgesAsCurveLoops, normal, 0.0328084);
            Solid solid2 = solid1;
            string str = "";
            if (IntersectingElements != null)
            {
                foreach (Element intersectingElement in (IEnumerable<Element>)IntersectingElements)
                {
                    foreach (Solid elementSolid in (IEnumerable<Solid>)GeometeryTools.GetElementSolids(intersectingElement))
                    {
                        if (elementSolid.Volume != 0.0)
                        {
                            try
                            {
                                BooleanOperationsUtils.ExecuteBooleanOperationModifyingOriginalSolid(solid1, elementSolid, (BooleanOperationsType)1);
                            }
                            catch (Exception ex)
                            {
                                str = "Error";
                            }
                        }
                    }
                }
            }
            foreach (Solid solid3 in !(str == "Error") ? (IEnumerable<Solid>)SolidUtils.SplitVolumes(solid1) : (IEnumerable<Solid>)SolidUtils.SplitVolumes(solid2))
            {
                FormworkFace formworkFace = new FormworkFace();
                formworkFace.Geometry = solid3;
                formworkFace.HostID = E.Id.ToString();
                formworkFace.ErrorID = str;
                formworkFace.HostElement = E;
                formworkFace.HostFace = F;
                formworkFace.Area = solid3.Volume / 0.0328084;
                formworkFace.ShapeKey = Math.Round(solid3.Volume, 1).ToString() + Math.Round(solid3.SurfaceArea, 1).ToString() + solid3.Faces.Size.ToString();
                XYZ xyz1 = F.ComputeNormal(new UV(0.5, 0.5)).Normalize();
                foreach (Face face2 in solid3.Faces)
                {
                    XYZ xyz2 = face2.ComputeNormal(new UV(0.5, 0.5)).Normalize();
                    if (xyz2.DotProduct(xyz1) < 0.0 && Math.Round(xyz2.AngleTo(xyz1), 2) == 3.14)
                    {
                        formworkFace.ModifiedFace = face2;
                        break;
                    }
                }
                formworkFaceList.Add(formworkFace);
            }
            return formworkFaceList;
        }

        public static Solid ThickenCurvedSurface(Face F, double Thickness)
        {
            try
            {
                Mesh mesh = F.Triangulate(0.5);
                List<Solid> SolidList = new List<Solid>();
                for (int index = 0; index < mesh.NumTriangles; ++index)
                {
                    MeshTriangle meshTriangle = mesh.get_Triangle(index);
                    XYZ xyz1 = meshTriangle.get_Vertex(0);
                    XYZ xyz2 = meshTriangle.get_Vertex(1);
                    XYZ xyz3 = meshTriangle.get_Vertex(2);
                    Line bound1 = Line.CreateBound(xyz1, xyz2);
                    Line bound2 = Line.CreateBound(xyz2, xyz3);
                    Line bound3 = Line.CreateBound(xyz3, xyz1);

                    XYZ vector1 = new XYZ(xyz2.X - xyz1.X, xyz2.Y - xyz1.Y, xyz2.Z - xyz1.Z);
                    XYZ vector2 = new XYZ(xyz3.X - xyz1.X, xyz3.Y - xyz1.Y, xyz3.Z - xyz1.Z);

                    XYZ xyz4 = new XYZ(vector1.Y * vector2.Z - vector1.Z * vector2.Y, vector1.Z * vector2.X - vector1.X * vector2.Z, vector1.X * vector2.Y - vector1.Y * vector2.X);
                    CurveLoop curveLoop = new CurveLoop();
                    curveLoop.Append(bound1);
                    curveLoop.Append(bound2);
                    curveLoop.Append(bound3);
                    Solid extrusionGeometry = GeometryCreationUtilities.CreateExtrusionGeometry(new List<CurveLoop>() { curveLoop }, xyz4, Thickness);
                    SolidList.Add(extrusionGeometry);
                }
                return UnionSolidList(SolidList);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static Solid UnionSolidList(List<Solid> SolidList)
        {
            Solid solid1 = null;
            foreach (Solid solid2 in SolidList)
            {
                if (solid2 != null && solid2.Faces.Size > 0)
                {
                    if (solid1 != null)
                    {

                        solid1 = BooleanOperationsUtils.ExecuteBooleanOperation(solid1, solid2, BooleanOperationsType.Union);
                    }
                    else
                    {
                        solid1 = solid2;
                    }
                }
            }
            return solid1;
        }

        public static Element FaceCreator(FormworkFace F, Document ActiveDoc)
        {
            Element element = null;
            try
            {
                Face modifiedFace = F.ModifiedFace;
                Element hostElement = F.HostElement;
                Solid geometry = F.Geometry;
                XYZ normal = modifiedFace.ComputeNormal(new UV(0.5, 0.5));

                BoundingBoxXYZ boundingBox = geometry.GetBoundingBox();
                double XX = boundingBox.Max.X - boundingBox.Min.X;
                double YY = boundingBox.Max.Y - boundingBox.Min.Y;
                double ZZ = boundingBox.Max.Z - boundingBox.Min.Z;

                if (normal.Z < 0.5 && normal.Z > -0.5)
                {
                    element = GeometeryTools.PlaceDirectShapeSpecial(geometry, ActiveDoc, BuiltInCategory.OST_GenericModel, "FormworkVertical");

                    if (Math.Abs(1 - (XX / 0.0328084)) < 0.001)
                    {
                        element.LookupParameter("Width").Set(YY);
                        element.LookupParameter("Height").Set(ZZ);
                    }
                    else if (Math.Abs(1 - (YY / 0.0328084)) < 0.001)
                    {
                        element.LookupParameter("Width").Set(XX);
                        element.LookupParameter("Height").Set(ZZ);
                    }
                }
                else
                {
                    element = GeometeryTools.PlaceDirectShapeSpecial(geometry, ActiveDoc, BuiltInCategory.OST_GenericModel, "FormworkHorizontal");

                    element.LookupParameter("Width").Set(XX);
                    element.LookupParameter("Height").Set(YY);
                }

                if (element.LookupParameter("SurfaceArea") != null)
                    element.LookupParameter("SurfaceArea").Set(F.Area);

                if (element.LookupParameter("HostID") != null)
                    element.LookupParameter("HostID").Set(F.HostID);


            }
            catch (Exception ex)
            {
                element.LookupParameter("Comments").Set("Error");
            }

            return element;
        }
    }
}
