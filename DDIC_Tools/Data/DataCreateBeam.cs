using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDIC_Tools.Data
{
    public class DataCreateBeam
    {
        public string Name { get; set; }

        public ElementId Id { get; set; }

        public double Height { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
