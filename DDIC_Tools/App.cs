#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#endregion

namespace DDIC_Tools
{
    public class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication a)
        {
            string tabName = "BIM 5D Tools";
            string panelName = "Schedule";
            string panelName1 = "Image";
            string panelName2 = "Wall";
            string panelName3 = "Views";
            string panelName4 = "Modify";
            string panelName5 = "Structure Framing";

            a.CreateRibbonTab(tabName);

            var panel = a.CreateRibbonPanel(tabName, panelName);
            var panel1 = a.CreateRibbonPanel(tabName, panelName1);
            var panel2 = a.CreateRibbonPanel(tabName, panelName2);
            var panel3 = a.CreateRibbonPanel(tabName, panelName3);
            var panel4 = a.CreateRibbonPanel(tabName, panelName4);
            var panel5 = a.CreateRibbonPanel(tabName, panelName5);

            #region Button tạo bảng thống kê từ file txt
            //BitmapImage image = new BitmapImage(new Uri(@"C:\Users\Administrator\Documents\DDIC\DDIC_Tools\DDIC_Tools\Resources\Import.png"));
            BitmapImage image = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\ApplicationPlugins\BIM_5D_Tools.bundle\Contents\Resources\Import.png"));
            var btnImport = new PushButtonData("btnImport", "Create schedule", Assembly.GetExecutingAssembly().Location,
                "DDIC_Tools.ImportSchedule");
            btnImport.ToolTip = "Create schedule from file txt";
            btnImport.LargeImage = image;
            //btnImport.Image = image;

            var btn1 = panel.AddItem(btnImport) as PushButton;
            #endregion

            #region Button xuất bảng thống kê theo cây thư mục
            //BitmapImage imgExport = new BitmapImage(new Uri(@"C:\Users\Administrator\Documents\DDIC\DDIC_Tools\DDIC_Tools\Resources\Export.png"));
            BitmapImage imgExport = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\ApplicationPlugins\BIM_5D_Tools.bundle\Contents\Resources\Export.png"));
            var btnExport = new PushButtonData("btnExport", "Export schedule", Assembly.GetExecutingAssembly().Location,
                "DDIC_Tools.ExportSchedule");
            btnExport.ToolTip = "Export schedule to directory tree";
            btnExport.LargeImage = imgExport;
            //btnExport.Image = imgExport;

            var btn2 = panel.AddItem(btnExport) as PushButton;
            #endregion

            #region Tạo button chỉnh sửa tên column schedule
            //BitmapImage imgEdit = new BitmapImage(new Uri(@"C:\Users\Administrator\Documents\DDIC\DDIC_Tools\DDIC_Tools\Resources\Edit Heading.png"));
            BitmapImage imgEdit = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\ApplicationPlugins\BIM_5D_Tools.bundle\Contents\Resources\Edit Heading.png"));
            var btnEdit = new PushButtonData("btnEdit", "Edit Title", Assembly.GetExecutingAssembly().Location,
                "DDIC_Tools.Command.EditTitleSchedule");
            btnEdit.ToolTip = "Edit the column headers of the schedule";
            btnEdit.LargeImage = imgEdit;
            //btnEdit.Image = imgEdit;

            var btn3 = panel.AddItem(btnEdit) as PushButton;
            #endregion

            #region Tạo button import ảnh vào revit
            BitmapImage imgEditHeading = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\ApplicationPlugins\BIM_5D_Tools.bundle\Contents\Resources\Import Image.png"));
            //BitmapImage imgEditHeading = new BitmapImage(new Uri(@"C:\Users\Administrator\Documents\DDIC\DDIC_Tools\DDIC_Tools\Resources\Import Image.png"));
            var btnEditHeading = new PushButtonData("btnEditHeading", "Image", Assembly.GetExecutingAssembly().Location,
                "DDIC_Tools.Command.ImportFolderImage");
            btnEditHeading.ToolTip = "Import image to Revit from folder";
            btnEditHeading.LargeImage = imgEditHeading;

            var btn4 = panel1.AddItem(btnEditHeading) as PushButton;
            #endregion

            #region Tạo lớp trát cho room
            BitmapImage imgFinishWall = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\ApplicationPlugins\BIM_5D_Tools.bundle\Contents\Resources\FinishWall.png"));
            var btnFinishWall = new PushButtonData("btnFinishWall", "Finish Wall", Assembly.GetExecutingAssembly().Location,
                "DDIC_Tools.Command.FinishWallCreate");
            btnFinishWall.ToolTip = "Create finish wall to rooms";
            btnFinishWall.LargeImage = imgFinishWall;

            var btn5 = panel2.AddItem(btnFinishWall) as PushButton;
            #endregion

            #region Copy filters views
            BitmapImage imgFilter = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\ApplicationPlugins\BIM_5D_Tools.bundle\Contents\Resources\CopyFilter.png"));
            var btnFilter = new PushButtonData("btnCopyFilter", "Copy Filters", Assembly.GetExecutingAssembly().Location,
                "DDIC_Tools.Command.CopyFilters");
            btnFilter.ToolTip = "Copy filters to views";
            btnFilter.LargeImage = imgFilter;

            var btn6 = panel3.AddItem(btnFilter) as PushButton;
            #endregion

            #region Copy element block
            BitmapImage imgChooseBlock = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\ApplicationPlugins\BIM_5D_Tools.bundle\Contents\Resources\CopyElement.png"));
            var btnCopyEleBlock = new PushButtonData("btnCopEleBlock", "Block CAD to Element", Assembly.GetExecutingAssembly().Location,
                "DDIC_Tools.Command.CopyElementCAD");
            btnCopyEleBlock.LargeImage = imgChooseBlock;
            btnCopyEleBlock.ToolTip = "Block CAD to Element";

            var btn7 = panel4.AddItem(btnCopyEleBlock) as PushButton;
            #endregion

            #region Tạo lanh tô
            BitmapImage imgCreateBeam = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\ApplicationPlugins\BIM_5D_Tools.bundle\Contents\Resources\Lanhto.png"));
            var btnCreateBeam = new PushButtonData("btnCreateBeam", "Create lintel", Assembly.GetExecutingAssembly().Location,
                "DDIC_Tools.Command.CreateBeam");
            btnCreateBeam.LargeImage = imgCreateBeam;
            btnCreateBeam.ToolTip = "Create lintel to door";

            var btn8 = panel5.AddItem(btnCreateBeam) as PushButton;
            #endregion

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
    }
}
