using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DDIC_Tools.Data;
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
    public partial class FormCopyFilters : System.Windows.Forms.Form
    {
        private Document doc;
        public FormCopyFilters(Document document)
        {
            InitializeComponent();
            doc = document;
        }

        private void FormCopyFilters_Load(object sender, EventArgs e)
        {
            Autodesk.Revit.DB.View view = doc.ActiveView;

            IList<Element> views = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Views)
                .WhereElementIsNotElementType()
                .Cast<Element>()
                .ToList();

            if (view != null)
            {
                ICollection<ElementId> filters = view.GetFilters();

                if (filters.Count > 0)
                {
                    foreach (ElementId filter in filters)
                    {
                        Element element = doc.GetElement(filter);

                        if (element != null)
                        {
                            DataCopyFilter data = new DataCopyFilter();
                            data.Id = filter;
                            data.Name = element.Name;

                            cklsFilter.Items.Add(data);
                        }
                    }
                }

                foreach (Element elem in views)
                {
                    DataCopyFilter data = new DataCopyFilter();
                    data.Id = elem.Id;
                    data.Name = elem.Name;

                    if (elem.Id != doc.ActiveView.Id)
                    {
                        cklsView.Items.Add(data);
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
            try
            {
                IList<DataCopyFilter> filters = cklsFilter.CheckedItems.Cast<DataCopyFilter>().ToList();
                IList<DataCopyFilter> views = cklsView.CheckedItems.Cast<DataCopyFilter>().ToList();

                if (filters.Count > 0 && views.Count > 0)
                {
                    foreach (DataCopyFilter filter in filters)
                    {
                        OverrideGraphicSettings overrideGraphic = doc.ActiveView.GetFilterOverrides(filter.Id);
                        bool filterVisibility = doc.ActiveView.GetFilterVisibility(filter.Id);

                        foreach (DataCopyFilter view in views)
                        {
                            Autodesk.Revit.DB.View viewFilter = doc.GetElement(view.Id) as Autodesk.Revit.DB.View;

                            if (viewFilter != null)
                            {
                                using (Transaction trans = new Transaction(doc, "Copy filters"))
                                {
                                    trans.Start();

                                    if (!viewFilter.IsFilterApplied(filter.Id))
                                    {
                                        viewFilter.AddFilter(filter.Id);
                                    }

                                    viewFilter.SetFilterOverrides(filter.Id, overrideGraphic);
                                    viewFilter.SetFilterVisibility(filter.Id, filterVisibility);

                                    trans.Commit();
                                }
                            }
                        }

                    }
                    TaskDialog.Show("Thông báo", "Copy filters thành công!");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
