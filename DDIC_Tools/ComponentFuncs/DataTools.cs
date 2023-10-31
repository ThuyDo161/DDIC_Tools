using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDIC_Tools.ComponentFuncs
{
    public class DataTools
    {
        public static bool AddSharedParameterToCategory(ExternalCommandData commandData, CategorySet C_Set, string ParameterName, string ParameterGroupName, ParameterType PType, bool Visible = true)
        {
            UIApplication uiapp = commandData.Application;
            Application app = uiapp.Application;
            Document document = commandData.Application.ActiveUIDocument.Document;

            try
            {
                DefinitionFile definitionFile = app.OpenSharedParameterFile();

                if (definitionFile != null)
                {
                    DefinitionGroups groups = definitionFile.Groups;
                    DefinitionGroup definitionGroup = groups.FirstOrDefault(x => x.Name == ParameterGroupName);

                    if (definitionGroup == null)
                    {
                        definitionGroup = groups.Create(ParameterGroupName);
                    }

                    Definition definition = definitionGroup.Definitions.FirstOrDefault(x => x.Name == ParameterName);

                    if (definition == null)
                    {
                        definition = definitionGroup.Definitions.Create(new ExternalDefinitionCreationOptions(ParameterName, PType)
                        {
                            Visible = Visible
                        });
                    }

                    using (Transaction tr = new Transaction(document, "Add Space Shared Parameters"))
                    {
                        tr.Start();

                        InstanceBinding instanceBinding = app.Create.NewInstanceBinding(C_Set);
                        uiapp.ActiveUIDocument.Document.ParameterBindings.Insert(definition, (Binding)instanceBinding, BuiltInParameterGroup.PG_DATA);

                        tr.Commit();
                        return true;
                    }

                }
                else
                {
                    TaskDialog.Show("Info", "Please Add Shared Parameter File to continue");
                    return false;
                }

            }
            catch
            {
                return false;
            }
        }
    }
}
