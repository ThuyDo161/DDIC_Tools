using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DDIC_Tools
{
    public partial class FormExportSchedule : System.Windows.Forms.Form
    {
        Document Doc;

        private int index = 0;

        private int groupboxWidth = 700;

        private int groupboxHeight = 70;

        private int comboboxWidth = 280;

        private int comboboxHeight = 110;

        private string ck = ".txt";

        private IList<Element> result = new List<Element>();
        public FormExportSchedule(Document doc)
        {
            InitializeComponent();
            Doc = doc;
        }

        private void FormExportSchedule_Load(object sender, EventArgs e)
        {
            IList<Element> schedules = new FilteredElementCollector(Doc)
                .OfCategory(BuiltInCategory.OST_Schedules)
                .ToElements();

            if (schedules.Count > 0)
            {
                Element sche1 = schedules[0];

                foreach (Parameter parameter in sche1.GetOrderedParameters())
                {
                    cbCategory.Items.Add(parameter.Definition.Name);
                }
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            IList<Element> elem = new List<Element>();

            IList<string> para = new List<string>();

            IList<string> error = new List<string>();

            string selectedPath = @"C:\";

            IList<Element> schedules = new FilteredElementCollector(Doc)
                .OfCategory(BuiltInCategory.OST_Schedules)
                .ToElements();

            string selected = cbResult.SelectedItem.ToString();

            string path = selectedPath + "\\" + selected;

            IEnumerable<GroupBox> listGroupBox = flowLayoutPanel.Controls.OfType<GroupBox>();
            IList<System.Windows.Forms.ComboBox> listCombobox = new List<System.Windows.Forms.ComboBox>();

            if (listGroupBox.Count() > 0)
            {
                foreach (GroupBox groupBox in listGroupBox)
                {
                    var list = groupBox.Controls.OfType<System.Windows.Forms.ComboBox>();
                    foreach (System.Windows.Forms.ComboBox cb in list)
                    {
                        listCombobox.Add(cb);
                    }
                }

                foreach (Element element in schedules)
                {
                    if (element.LookupParameter(cbCategory.SelectedItem.ToString()) != null)
                    {
                        string s = element.LookupParameter(cbCategory.SelectedItem.ToString()).AsValueString();

                        if (s != null)
                        {
                            if (cbResult.SelectedItem.ToString() == s)
                            {
                                elem.Add(element);
                            }
                        }
                    }
                }

                foreach (Element element in elem)
                {
                    string name = "";

                    foreach (System.Windows.Forms.ComboBox cb in listCombobox)
                    {
                        if (cb.Name.Contains("cbCategory"))
                        {
                            string i = cb.Name.Substring(cb.Name.Length - 1);

                            int number;

                            bool isNum = int.TryParse(i, out number);

                            if (isNum)
                            {
                                if (element.LookupParameter(cb.SelectedItem.ToString()) != null)
                                {
                                    string value = element.LookupParameter(cb.SelectedItem.ToString()).AsValueString();
                                    if (value != null)
                                    {
                                        name = name + value + "\\";
                                    }
                                }
                            }
                        }
                    }

                    if (!para.Contains(name))
                    {
                        para.Add(path + "\\" + name);
                    }
                }

                if (para.Count > 0)
                {
                    foreach (string ele in para)
                    {
                        for (int i = 0; i < ele.Length; i++)
                        {
                            if (ele[i] == '\\')
                            {
                                string ten = ele.Substring(0, i);
                                if (!System.IO.Directory.Exists(ten))
                                {
                                    System.IO.Directory.CreateDirectory(ten);
                                }
                            }
                        }
                    }
                }

                try
                {
                    foreach (Element element in elem)
                    {
                        string name = "";

                        foreach (System.Windows.Forms.ComboBox cb in listCombobox)
                        {
                            if (cb.Name.Contains("cbCategory"))
                            {
                                string i = cb.Name.Substring(cb.Name.Length - 1);

                                int number;

                                bool isNum = int.TryParse(i, out number);

                                if (isNum)
                                {
                                    if (element.LookupParameter(cb.SelectedItem.ToString()) != null)
                                    {
                                        string value = element.LookupParameter(cb.SelectedItem.ToString()).AsValueString();
                                        if (value != null)
                                        {
                                            name = name + "\\" + value;
                                        }
                                    }
                                }
                            }
                        }

                        ViewSchedule schedule = element as ViewSchedule;

                        ViewScheduleExportOptions options = new ViewScheduleExportOptions();
                        options.Title = false;
                        options.HeadersFootersBlanks = false;

                        string title = "";

                        try
                        {
                            string n = schedule.Name.Replace("<", "-")
                                .Replace(">", "-")
                                .Replace("\\", "-")
                                .Replace("*", "-")
                                .Replace("/", "-")
                                .Replace("|", "-")
                                .Replace(":", "-")
                                .Replace("?", "-");

                            if (n.Length > 150)
                            {
                                n = n.Substring(0, 120);
                            }

                            title = n;

                            schedule.Export(path + name, title + ck, options);
                        }
                        catch (Exception ex)
                        {
                            error.Add(title);
                            continue;
                        }
                    }

                    if (error.Count > 0)
                    {
                        string filePath = path + "\\" + "Các bảng thống kê chưa xuất";

                        File.WriteAllLines(filePath + ".txt", error);
                    }

                    TaskDialog.Show("Thông báo", "Xuất file thành công! Tệp của bạn nằm ở " + selectedPath);

                    string fileDictionary = Path.GetDirectoryName(path);
                    System.Diagnostics.Process.Start("explorer.exe", fileDictionary);
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("Thông báo", "Không tìm thấy file phù hợp!");
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddCombobox();
        }

        private void AddCombobox()
        {
            if (index < 3)
            {
                GroupBox groupBox = new GroupBox();

                groupBox.Name = "grbSelect" + (index + 1);
                groupBox.Location = new System.Drawing.Point(20, (index + 1) * (groupboxHeight + 20));
                groupBox.Size = new Size(groupboxWidth, groupboxHeight);

                System.Windows.Forms.ComboBox cb1 = new System.Windows.Forms.ComboBox();

                Button btnDel = new Button();

                Label label1 = new Label();

                label1.Text = "Chọn parameter " + (index + 1);

                cb1.Name = "cbCategory" + (index + 1);

                btnDel.Name = "btnDel" + (index + 1);
                btnDel.Text = "Xoá";
                btnDel.Click += Button_Click;

                label1.Location = new System.Drawing.Point(5, 10);

                cb1.Location = new System.Drawing.Point(5, 35);
                cb1.Size = new Size(comboboxWidth, comboboxHeight);

                btnDel.Location = new System.Drawing.Point(610, 25);
                btnDel.Size = new Size(70, 31);

                IList<Element> schedules = new FilteredElementCollector(Doc)
                    .OfCategory(BuiltInCategory.OST_Schedules)
                    .ToElements();
                Element sche1 = schedules[0];
                foreach (Parameter parameter in sche1.GetOrderedParameters())
                {
                    cb1.Items.Add(parameter.Definition.Name);
                }

                cb1.SelectedIndexChanged += comboboxSelectedChanged;

                groupBox.Controls.Add(label1);
                groupBox.Controls.Add(cb1);
                groupBox.Controls.Add(btnDel);

                flowLayoutPanel.Controls.Add(groupBox);

                index++;
            }
        }

        private void comboboxSelectedChanged(object sender, EventArgs e)
        {
            System.Windows.Forms.ComboBox currCombobox = (System.Windows.Forms.ComboBox)sender;

            string selected = currCombobox.SelectedItem.ToString();

            IList<string> listPara = new List<string>();

            IList<Element> schedules = new FilteredElementCollector(Doc)
                .OfCategory(BuiltInCategory.OST_Schedules)
                .ToElements();

            foreach (Element element in schedules)
            {
                if (element.LookupParameter(selected) != null)
                {
                    var ele = element.LookupParameter(selected).AsValueString();
                    if (ele != null && ele != "")
                    {
                        if (!listPara.Contains(ele))
                        {
                            listPara.Add(ele);
                        }
                    }
                }
            }
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
                }
            }
        }

        private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected = cbCategory.SelectedItem.ToString();

            IList<Element> schedules = new FilteredElementCollector(Doc)
                .OfCategory(BuiltInCategory.OST_Schedules)
                .ToElements();

            foreach (Element element in schedules)
            {
                if (element.LookupParameter(selected) != null)
                {
                    var ele = element.LookupParameter(selected).AsValueString();
                    if (ele != null && ele != "")
                    {
                        if (!cbResult.Items.Contains(ele))
                        {
                            cbResult.Items.Add(ele);
                        }
                    }
                }
            }
        }
    }
}
