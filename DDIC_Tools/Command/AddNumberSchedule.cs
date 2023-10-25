using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDIC_Tools.Command
{
    [Transaction(TransactionMode.Manual)]
    public class AddNumberSchedule : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            ViewSchedule vs = doc.ActiveView as ViewSchedule;

            if (vs != null)
            {
                TableData tableData = vs.GetTableData();
                TableSectionData tableSectionData = tableData.GetSectionData(SectionType.Body);

                bool hasMarkField = false;

                ScheduleDefinition definition = vs.Definition;

                foreach (ScheduleFieldId fieldId in definition.GetFieldOrder())
                {
                    ScheduleField field = definition.GetField(fieldId);

                    if (field.GetName().Equals("Mark", StringComparison.OrdinalIgnoreCase))
                    {
                        hasMarkField = true;
                        break;
                    }
                }

                if (!hasMarkField)
                {
                    using (Transaction trans = new Transaction(doc, "Add Mark Field"))
                    {
                        trans.Start();

                        ElementId mark = new ElementId(BuiltInParameter.ALL_MODEL_MARK);
                        ScheduleField markField = definition.AddField(ScheduleFieldType.Instance, mark);

                        List<ScheduleFieldId> fieldIds = new List<ScheduleFieldId>();

                        // Lặp qua tất cả các cột hiện có và thêm FieldId vào danh sách
                        foreach (ScheduleFieldId field in definition.GetFieldOrder())
                        {
                            fieldIds.Add(field);
                        }

                        fieldIds.Remove(markField.FieldId);
                        fieldIds.Insert(0, markField.FieldId);

                        definition.SetFieldOrder(fieldIds);

                        trans.Commit();
                    }
                }

                int numberOfRow = tableSectionData.NumberOfRows;

                if (numberOfRow > 0)
                {
                    for (int r = 0; r < numberOfRow; r++)
                    {
                        List<Element> elementsOnRow = GettingTheElementsOnRow(doc, vs, r);

                        if (elementsOnRow != null)
                        {
                            using (Transaction tr = new Transaction(doc, "Setting Parameters"))
                            {
                                tr.Start();

                                foreach (Element e in elementsOnRow)
                                {
                                    if (e.get_Parameter(BuiltInParameter.ALL_MODEL_MARK) != null)
                                    {
                                        e.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set((r - 1).ToString());
                                    }
                                }

                                tr.Commit();
                            }
                        }
                    }

                    TaskDialog.Show("Thông báo", "Thêm số thứ tự thành công!");
                }

                return Result.Succeeded;
            }
            else
            {
                TaskDialog.Show("Lỗi", "Vui lòng mở một bảng thống kê!");
                return Result.Failed;
            }
        }

        private List<Element> GettingTheElementsOnRow(Document doc, ViewSchedule vs, int rowNumber)
        {
            TableData tableData = vs.GetTableData();

            TableSectionData tableSectionData = tableData.GetSectionData(SectionType.Body);

            List<ElementId> elemids = new FilteredElementCollector(doc, vs.Id).ToElementIds().ToList();

            List<Element> elementOnRow = new List<Element>();

            List<ElementId> remainingIds = null;

            using (Transaction tr = new Transaction(doc, "Getting IDs"))
            {
                tr.Start();

                using (SubTransaction st = new SubTransaction(doc))
                {
                    st.Start();
                    try
                    {
                        tableSectionData.RemoveRow(rowNumber);
                    }
                    catch
                    {
                        return null;
                    }
                    st.Commit();
                }

                remainingIds = new FilteredElementCollector(doc, vs.Id).ToElementIds().ToList();
                tr.RollBack();
            }

            foreach (ElementId eid in elemids)
            {
                if (remainingIds.Contains(eid)) continue;
                elementOnRow.Add(doc.GetElement(eid));
            }

            return elementOnRow;
        }
    }
}
