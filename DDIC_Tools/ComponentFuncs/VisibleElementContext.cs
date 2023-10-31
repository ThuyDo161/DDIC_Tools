using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDIC_Tools.ComponentFuncs
{
    internal class VisibleElementContext : IExportContext
    {
        public Document ParentDocument;
        public Document CurrentDocument;
        public List<Document> Documents;
        public List<Element> Elements;
        public List<ElementId> ElementIds;

        internal VisibleElementContext(Document doc)
        {
            this.ParentDocument = doc;
            this.CurrentDocument = doc;
            this.Documents = new List<Document>() { doc };
            this.Elements = new List<Element>();
            this.ElementIds = new List<ElementId>();
        }

        public void Finish() { }

        public bool IsCanceled() => false;

        public RenderNodeAction OnElementBegin(ElementId elementId)
        {
            this.ElementIds.Add(elementId);
            this.Elements.Add(this.CurrentDocument.GetElement(elementId));

            return RenderNodeAction.Proceed;
        }

        public void OnElementEnd(ElementId elementId) { }

        public RenderNodeAction OnFaceBegin(FaceNode node) => RenderNodeAction.Skip;
        public void OnFaceEnd(FaceNode node) { }

        public RenderNodeAction OnInstanceBegin(InstanceNode node) => RenderNodeAction.Skip;

        public void OnInstanceEnd(InstanceNode node) { }

        public void OnLight(LightNode node) { }

        public RenderNodeAction OnLinkBegin(LinkNode node)
        {
            Document document = node.GetDocument();
            this.Documents.Add(document);
            this.CurrentDocument = document;

            return RenderNodeAction.Proceed;
        }

        public void OnLinkEnd(LinkNode node) => this.CurrentDocument = this.ParentDocument;
        public void OnMaterial(MaterialNode node) { }

        public void OnPolymesh(PolymeshTopology node) { }
        public void OnRPC(RPCNode node) { }
        public RenderNodeAction OnViewBegin(ViewNode node) => RenderNodeAction.Proceed;

        public void OnViewEnd(ElementId elementId) { }
        public bool Start()
        {
            return true;
        }
    }
}
