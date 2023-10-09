using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.UI;
using DDIC_Tools.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DDIC_Tools.FormUI
{
    public partial class FormCreateBeam : System.Windows.Forms.Form
    {
        private Document doc;

        private IList<Element> elements;

        public FormCreateBeam(Document document, IList<Element> listElements)
        {
            InitializeComponent();
            doc = document;
            this.elements = listElements;
        }

        private void FormCreateBeam_Load(object sender, EventArgs e)
        {
            cklstBeamType.SelectionMode = SelectionMode.One;

            FilteredElementCollector collector = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilySymbol))
                .OfCategory(BuiltInCategory.OST_StructuralFraming);

            foreach (FamilySymbol symbol in collector)
            {
                DataCreateBeam data = new DataCreateBeam();
                data.Name = symbol.Name + " - " + symbol.FamilyName;
                data.Id = symbol.Id;

                IList<Parameter> parameters = symbol.GetParameters("h");
                if (parameters.Count > 0)
                {
                    Parameter parameter = parameters.First();
                    data.Height = parameter.AsDouble();
                }

                cklstBeamType.Items.Add(data);
            }

            cklstBeamType.SelectedIndex = 0;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cklstBeamType.CheckedItems.Count == 0)
            {
                MessageBox.Show("Bạn phải chọn kiểu lanh tô!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                foreach (Element element in this.elements)
                {
                    Transaction trans = new Transaction(doc, "Create Beam");

                    trans.Start();

                    CreatBeamEle(element);

                    trans.Commit();
                }

                TaskDialog.Show("Thông báo", "Tạo lanh tô thành công!");
                this.Close();
            }
        }

        private void CreatBeamEle(Element element)
        {
            // Gán element được chọn sang kiểu dữ liệu
            FamilyInstance instance = element as FamilyInstance;

            // Lấy tường chứa element được chọn
            Element wall = instance.Host;

            // Lấy location của wall
            LocationCurve wallCurve = wall.Location as LocationCurve;

            // Lấy location của element được chọn
            LocationPoint locationPoint = element.Location as LocationPoint;

            XYZ pointEle = new XYZ(locationPoint.Point.X, locationPoint.Point.Y, 0);

            Parameter parameter = instance.Symbol.get_Parameter(BuiltInParameter.DOOR_WIDTH);
            double width = parameter.AsDouble() * 304.8;

            Plane plane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, pointEle);
            SketchPlane sketchPlane = SketchPlane.Create(doc, plane);

            double widthPlus = double.Parse(txtWidth.Text);

            Arc circle = Arc.Create(plane, (width / 2 + widthPlus) / 304.8, 0, 2 * Math.PI);

            SetComparisonResult result = wallCurve.Curve.Intersect(circle);

            List<XYZ> points = new List<XYZ>();
            if (result == SetComparisonResult.Overlap)
            {
                IntersectionResultArray intersectionResults;
                wallCurve.Curve.Intersect(circle, out intersectionResults);

                foreach (IntersectionResult intersectionResult in intersectionResults)
                {
                    points.Add(intersectionResult.XYZPoint);
                }
            }

            Level level = doc.ActiveView.GenLevel;

            DataCreateBeam data = (DataCreateBeam)cklstBeamType.SelectedItem;

            FamilySymbol beamSymbol = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_StructuralFraming)
                .OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>()
                .FirstOrDefault(x => x.Id == data.Id);

            double heightDoor = element.get_Parameter(BuiltInParameter.INSTANCE_HEAD_HEIGHT_PARAM).AsDouble() * 304.8;
            double heightBeam = data.Height * 304.8;

            double zoffset = (heightDoor + heightBeam) / 304.8;

            if (points.Count > 0)
            {
                Line line = Line.CreateBound(points[0], points[1]);

                Curve curve = line as Curve;
                FamilyInstance beam = doc.Create.NewFamilyInstance(curve, beamSymbol, level, Autodesk.Revit.DB.Structure.StructuralType.Beam);
                Parameter pBeam = beam.get_Parameter(BuiltInParameter.Z_OFFSET_VALUE);
                pBeam.Set(zoffset);

                if (ckJoin.Checked == true)
                {
                    JoinGeometryUtils.JoinGeometry(doc, beam, wall);
                }
            }
        }

        private void cklstBeamType_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
            {
                for (int i = 0; i < cklstBeamType.Items.Count; i++)
                {
                    if (i != e.Index)
                    {
                        cklstBeamType.SetItemChecked(i, false);
                    }
                }
            }
        }
    }
}
