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

namespace DDIC_Tools.FormUI
{
    public partial class FormEditTitleSchedule : System.Windows.Forms.Form
    {
        Document Doc;

        private int index = 0;

        private int col = 1;

        private int groupboxWidth = 700;

        private int groupboxHeight = 95;

        private int textBoxWidth = 590;

        private int textBoxHeight = 40;

        public FormEditTitleSchedule(Document doc)
        {
            InitializeComponent();
            Doc = doc;
        }

        private void FormEditTitleSchedule_Load(object sender, EventArgs e)
        {

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddTextBox();
        }

        private void AddTextBox()
        {
            GroupBox groupBox = new GroupBox();

            groupBox.Name = "grbSelect" + (index + 1);
            groupBox.Location = new System.Drawing.Point(20, (index + 1) * (groupboxHeight + 20));
            groupBox.Size = new Size(groupboxWidth, groupboxHeight);

            Button btnDel = new Button();
            btnDel.Name = "btnDel" + (index + 1);
            btnDel.Text = "Delete";
            btnDel.Location = new System.Drawing.Point(610, 30);
            btnDel.Size = new Size(70, 31);
            btnDel.Click += Button_Click;

            Label label1 = new Label();
            label1.Text = "Enter the column name " + (col + 1);
            label1.Location = new System.Drawing.Point(5, 10);

            System.Windows.Forms.TextBox textBox = new System.Windows.Forms.TextBox();
            textBox.Location = new System.Drawing.Point(5, 35);
            textBox.Name = "txtColumn" + (index + 1);
            textBox.Multiline = true;
            textBox.Size = new Size(textBoxWidth, textBoxHeight);

            groupBox.Controls.Add(label1);
            groupBox.Controls.Add(btnDel);
            groupBox.Controls.Add(textBox);

            flowLayoutPanel.Controls.Add(groupBox);

            index++;
            col++;
        }

        private void Button_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Button btn = (System.Windows.Forms.Button)sender;
            string i = btn.Name.Substring(btn.Name.Length - 1);
            int number;

            if (int.TryParse(i, out number))
            {
                string groupName = "grbSelect" + i;
                GroupBox groupBox = flowLayoutPanel.Controls.OfType<GroupBox>().First(x => x.Name == groupName);
                if (groupBox != null)
                {
                    flowLayoutPanel.Controls.Remove(groupBox);
                    index--;
                    col--;
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            // Lấy danh sách các textbox
            IEnumerable<GroupBox> listGroupBox = flowLayoutPanel.Controls.OfType<GroupBox>();
            IList<System.Windows.Forms.TextBox> textBoxes = new List<System.Windows.Forms.TextBox>();
            int number;

            if (listGroupBox.Count() > 0)
            {
                foreach (GroupBox groupBox in listGroupBox)
                {
                    var list = groupBox.Controls.OfType<System.Windows.Forms.TextBox>();

                    foreach (System.Windows.Forms.TextBox textBox in list)
                    {
                        textBoxes.Add(textBox);
                    }
                }
            }

            if (textBoxes.Count() > 0)
            {
                IList<Element> schedules = new FilteredElementCollector(Doc)
                .OfCategory(BuiltInCategory.OST_Schedules)
                .ToElements();

                if (schedules.Count() > 0)
                {
                    try
                    {
                        foreach (Element element in schedules)
                        {
                            ViewSchedule schedule = element as ViewSchedule;
                            ScheduleDefinition definition = schedule.Definition;

                            IList<ScheduleFieldId> fieldIds = definition.GetFieldOrder();
                            IList<ScheduleField> fields = new List<ScheduleField>();

                            if (fieldIds.Count > 0)
                            {
                                foreach (var id in fieldIds)
                                {
                                    ScheduleField field = definition.GetField(id);

                                    fields.Add(field);
                                }
                            }

                            if (fields.Count() > 0)
                            {
                                for (int i = 0; i < textBoxes.Count; i++)
                                {

                                    using (Transaction trans = new Transaction(Doc, "Edit title header schedule"))
                                    {
                                        trans.Start();

                                        fields[i].ColumnHeading = textBoxes[i].Text;

                                        trans.Commit();
                                    }
                                }

                            }
                        }

                        TaskDialog.Show("Notification", "Edit title header of the schedule successfully!");
                    }
                    catch (Exception ex)
                    {
                        TaskDialog.Show("Notification", ex.Message);
                    }
                }
            }
        }
    }
}
