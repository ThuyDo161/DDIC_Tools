using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DDIC_Tools
{
    public partial class FormImportSchedule : System.Windows.Forms.Form
    {
        Document Doc;

        string filePath;
        public FormImportSchedule(Document doc)
        {
            InitializeComponent();
            Doc = doc;
        }

        private void FormImportSchedule_Load(object sender, EventArgs e)
        {
            FilteredElementCollector collector
                = new FilteredElementCollector(Doc)
                .WhereElementIsNotElementType();
            collector.OfClass(typeof(SharedParameterElement));
            foreach (Element ele in collector)
            {
                SharedParameterElement param = ele as SharedParameterElement;
                Definition def = param.GetDefinition();

                cbCat1.Items.Add(def.Name);
            }
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Text files (*.txt)|*.txt";
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;

            DialogResult result = openFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                txtPath.Text = openFileDialog.FileName;
                filePath = openFileDialog.FileName;
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            if (filePath == "")
            {
                TaskDialog.Show("Notification", "Please select the file to import!");
            }
            else
            {
                ImportTxtFile(Doc, filePath);
            }
        }

        private void ImportTxtFile(Document doc, string path)
        {
            string[] content = System.IO.File.ReadAllLines(path);

            IList<string> names = new List<string>();

            IList<ViewSchedule> schedules = new List<ViewSchedule>();

            foreach (string line in content)
            {
                string[] values = line.Split('\t');

                for (int i = 0; i < values.Length; i++)
                {
                    if (i == 3)
                    {
                        using (Transaction trans = new Transaction(doc, "Create Schedule"))
                        {
                            trans.Start();

                            ViewSchedule schedule;

                            if (values[i] == "")
                            {
                                schedule = ViewSchedule.CreateSchedule(doc, new ElementId(BuiltInCategory.OST_GenericModel));
                            }
                            else
                            {
                                schedule = ViewSchedule.CreateSchedule(doc, new ElementId(int.Parse(values[i])));
                            }

                            schedules.Add(schedule);

                            trans.Commit();
                        }
                    }
                    else if (i == 4)
                    {
                        if (names.Contains(values[i] + " - " + values[5]))
                        {
                            string n = values[i] + " - " + values[5] + "(" + new Random().Next(0, 10000) + ")";
                            names.Add(n);
                        }
                        else
                        {
                            names.Add(values[i] + " - " + values[5]);
                        }
                    }
                }
            }

            for (int i = 0; i < schedules.Count; i++)
            {
                RenameSchedule(schedules[i], names[i]);

                string[] values = content[i].Split('\t');

                for (int j = 0; j < values.Length; j++)
                {
                    if (j == 0)
                    {
                        Parameter p = schedules[i].LookupParameter(cbCat1.SelectedItem.ToString());

                        if (p != null)
                        {
                            SetParameterSchedule(p, values[j]);
                        }
                    }
                    else if (j == 1)
                    {
                        Parameter p = schedules[i].LookupParameter(cbCat2.SelectedItem.ToString());

                        if (p != null)
                        {
                            SetParameterSchedule(p, values[j]);
                        }
                    }
                    else if (j == 2)
                    {
                        Parameter p = schedules[i].LookupParameter(cbCat3.SelectedItem.ToString());

                        if (p != null)
                        {
                            SetParameterSchedule(p, values[j]);
                        }
                    }
                }
            }

            TaskDialog.Show("Notification", "Import successfully!");
        }

        private void RenameSchedule(ViewSchedule schedule, string name)
        {
            try
            {
                using (Transaction trans = new Transaction(schedule.Document, "Rename Schedule"))
                {
                    trans.Start();

                    schedule.Name = name.Replace("\\", "-")
                        .Replace(">=", "≥")
                        .Replace("<=", "≤")
                        .Replace(":", "-")
                        .Replace("{", "-")
                        .Replace("}", "-")
                        .Replace("[", "-")
                        .Replace("]", "-")
                        .Replace("|", "-")
                        .Replace(";", "-")
                        .Replace("<", "nhỏ hơn")
                        .Replace(">", "lớn hơn")
                        .Replace("?", "-")
                        .Replace("`", "-")
                        .Replace("\"", "")
                        .Replace("~", "-");

                    trans.Commit();
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Lỗi", ex.Message);
            }
        }

        private void SetParameterSchedule(Parameter p, string value)
        {
            using (Transaction trans = new Transaction(Doc, "Set Parameter Schedule"))
            {
                trans.Start();

                p.Set(value);

                trans.Commit();
            }
        }

        private void cbCat1_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbCat2.Items.Clear();

            var items = cbCat1.Items.Cast<string>().ToList();

            items.Remove(cbCat1.SelectedItem.ToString());

            cbCat2.Items.AddRange(items.ToArray());
        }

        private void cbCat2_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbCat3.Items.Clear();

            var items = cbCat2.Items.Cast<string>().ToList();

            items.Remove(cbCat2.SelectedItem.ToString());

            cbCat3.Items.AddRange(items.ToArray());
        }
    }
}
