using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DDIC_Tools.ComponentFuncs;
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
using System.Windows.Media.Animation;

namespace DDIC_Tools.FormUI
{
    public partial class FormChooseBlock : System.Windows.Forms.Form
    {
        private Document doc;

        private Element eleBlock;

        private ExternalEvent _externalEvent;

        private CopyBlockCAD _eventHandler;

        List<XYZ> Points = new List<XYZ>();

        List<double> Rotations = new List<double>();

        List<string> Blocks = new List<string>();

        public FormChooseBlock(Document document, Element element)
        {
            InitializeComponent();
            doc = document;
            eleBlock = element;
        }

        private void FormChooseBlock_Load(object sender, EventArgs e)
        {
            try
            {
                GeometryElement geoElem = eleBlock.get_Geometry(new Options());

                if (geoElem != null)
                {
                    foreach (GeometryInstance geoObj in geoElem)
                    {

                        Transform transform = geoObj.Transform;
                        GeometryElement instance = geoObj.SymbolGeometry;

                        if (instance != null)
                        {
                            try
                            {
                                foreach (var item in instance)
                                {
                                    if (item is GeometryInstance inst)
                                    {
                                        XYZ point = transform.OfPoint(inst.Transform.Origin);
                                        Points.Add(CommonFunctions.ToPoint(point));

                                        double rotation = Math.Abs(UnitUtils.ConvertFromInternalUnits
                                            (inst.Transform.BasisX.AngleOnPlaneTo(XYZ.BasisX, XYZ.BasisZ),
                                            UnitTypeId.Degrees) - 360);

                                        if (Math.Round(rotation, 3) == 360)
                                        {
                                            rotation = 0;
                                        }

                                        Rotations.Add(Math.Round(rotation, 3));

                                        Blocks.Add(inst.Symbol.Name.Split(new string[] { ".dwg." }, StringSplitOptions.None).Last());
                                    }
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            if (Blocks.Count > 0)
            {
                foreach (string block in Blocks)
                {
                    if (!lstBlock.Items.Contains(block))
                    {
                        lstBlock.Items.Add(block);
                    }
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            _eventHandler = new CopyBlockCAD(Points, Blocks, lstBlock.SelectedItem.ToString());
            _externalEvent = ExternalEvent.Create(_eventHandler);

            _externalEvent.Raise();

            this.Close();
        }
    }
}
