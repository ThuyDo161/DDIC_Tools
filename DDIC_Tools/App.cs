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

            a.CreateRibbonTab(tabName);

            var panel = a.CreateRibbonPanel(tabName, panelName);
            var panel1 = a.CreateRibbonPanel(tabName, panelName1);

            #region Button tạo bảng thống kê từ file txt
            //BitmapImage image = new BitmapImage(new Uri(@"C:\Users\Administrator\Documents\DDIC\DDIC_Tools\DDIC_Tools\Resources\Import.png"));
            BitmapImage image = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\Revit\Addins\2022\BIM_5D_Tools.bundle\Contents\Resources\Import.png"));
            var btnImport = new PushButtonData("btnImport", "Tạo schedule", Assembly.GetExecutingAssembly().Location,
                "DDIC_Tools.ImportSchedule");
            btnImport.ToolTip = "Tạo schedule từ file txt";
            btnImport.LargeImage = image;
            //btnImport.Image = image;

            var btn1 = panel.AddItem(btnImport) as PushButton;
            #endregion

            #region Button xuất bảng thống kê theo cây thư mục
            //BitmapImage imgExport = new BitmapImage(new Uri(@"C:\Users\Administrator\Documents\DDIC\DDIC_Tools\DDIC_Tools\Resources\Export.png"));
            BitmapImage imgExport = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\Revit\Addins\2022\BIM_5D_Tools.bundle\Contents\Resources\Export.png"));
            var btnExport = new PushButtonData("btnExport", "Xuất schedule", Assembly.GetExecutingAssembly().Location,
                "DDIC_Tools.ExportSchedule");
            btnExport.ToolTip = "Xuất schedule theo cây thư mục";
            btnExport.LargeImage = imgExport;
            //btnExport.Image = imgExport;

            var btn2 = panel.AddItem(btnExport) as PushButton;
            #endregion

            #region Tạo button chỉnh sửa tên column schedule
            //BitmapImage imgEdit = new BitmapImage(new Uri(@"C:\Users\Administrator\Documents\DDIC\DDIC_Tools\DDIC_Tools\Resources\Edit Heading.png"));
            BitmapImage imgEdit = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\Revit\Addins\2022\BIM_5D_Tools.bundle\Contents\Resources\Edit Heading.png"));
            var btnEdit = new PushButtonData("btnEdit", "Sửa tiêu đề", Assembly.GetExecutingAssembly().Location,
                "DDIC_Tools.Command.EditTitleSchedule");
            btnEdit.ToolTip = "Chỉnh sửa tiêu đề cột của bảng thống kê";
            btnEdit.LargeImage = imgEdit;
            //btnEdit.Image = imgEdit;

            var btn3 = panel.AddItem(btnEdit) as PushButton;
            #endregion

            #region Tạo button import ảnh vào revit
            BitmapImage imgEditHeading = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\Revit\Addins\2022\BIM_5D_Tools.bundle\Contents\Resources\Import Image.png"));
            //BitmapImage imgEditHeading = new BitmapImage(new Uri(@"C:\Users\Administrator\Documents\DDIC\DDIC_Tools\DDIC_Tools\Resources\Import Image.png"));
            var btnEditHeading = new PushButtonData("btnEditHeading", "Thêm ảnh", Assembly.GetExecutingAssembly().Location,
                "DDIC_Tools.Command.ImportFolderImage");
            btnEditHeading.ToolTip = "Thêm ảnh vào Revit từ folder";
            btnEditHeading.LargeImage = imgEditHeading;

            var btn4 = panel1.AddItem(btnEditHeading) as PushButton;
            #endregion

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
    }
}
