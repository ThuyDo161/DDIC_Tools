using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DDIC_Tools.Command.FormWorkBeam;

namespace DDIC_Tools.ComponentFuncs
{
    internal class ElementCollect
    {
        private readonly UIDocument _uiDocument;
        private readonly Application _app;
        private readonly Document _parentDocument;
        private readonly View _activeView;

        internal ElementCollect(UIDocument uidoc)
        {
            this._uiDocument = uidoc;
            this._app = uidoc.Application.Application;
            this._parentDocument = uidoc.Document;
            this._activeView = this._parentDocument.ActiveView;
        }

        public List<Element> GetVisibleElements()
        {
            VisibleElementContext visibleElementContext = new VisibleElementContext(this._parentDocument);

            new CustomExporter(_parentDocument, visibleElementContext).Export(new List<ElementId>() { _activeView.Id });

            return visibleElementContext.Elements;
        }
    }
}
