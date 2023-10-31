using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDIC_Tools.Data
{
    public class FinishWallSetup
    {
        public WallType SelectedWallType { get; set; }

        public double BoardHeight { get; set; }

        public bool JoinWall { get; set; }

        public List<Room> SelectedRooms { get; set; }
    }
}
