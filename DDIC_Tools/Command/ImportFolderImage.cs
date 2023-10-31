using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DDIC_Tools.Command
{
    [Transaction(TransactionMode.Manual)]
    public class ImportFolderImage : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            Document doc = uidoc.Document;

            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Chọn thư mục để ảnh";
                folderDialog.ShowNewFolderButton = false;

                DialogResult result = folderDialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    string folderPath = folderDialog.SelectedPath;

                    ImportImage(doc, folderPath);
                }
            }

            return Result.Succeeded;
        }

        #region xử lý thêm ảnh vào revit
        public void ImportImage(Document doc, string folder)
        {
            string[] jpgFile = System.IO.Directory.GetFiles(folder, "*.jpg");
            string[] pngFile = System.IO.Directory.GetFiles(folder, "*.png");

            string[] files = jpgFile.Concat(pngFile).ToArray();

            foreach (string file in files)
            {
                using (Transaction transaction = new Transaction(doc))
                {
                    transaction.Start("Link Image");

                    ImageTypeOptions options = new ImageTypeOptions(file, false, ImageTypeSource.Import);
                    ImageType imageType = ImageType.Create(doc, options);
                    ElementId imageId = imageType.Id;

                    ImageView view = ImageView.Create(doc, options);

                    ImagePlacementOptions placementOptions = new ImagePlacementOptions();
                    ImageInstance instance = ImageInstance.Create(doc, view, imageId, placementOptions);

                    Location location = instance.Location;

                    transaction.Commit();
                }
            }

            DeleteDuplicateImages(doc);

            TaskDialog.Show("Notification", "Import image to Revit successfully!");
        }
        #endregion xử lý thêm ảnh vào revit

        #region xoá file trùng path
        public void DeleteDuplicateImages(Document doc)
        {
            // Lấy danh sách tất cả các ảnh trong Revit
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ICollection<Element> images = collector.OfClass(typeof(ImageType)).ToElements();

            // Tạo một Dictionary để lưu trữ thông tin về đường dẫn file và element tương ứng
            Dictionary<string, Element> imagePaths = new Dictionary<string, Element>();

            // Duyệt qua danh sách ảnh và kiểm tra đường dẫn file
            foreach (Element image in images)
            {
                ImageType imageType = image as ImageType;
                if (imageType != null)
                {
                    string imagePath = imageType.Path;
                    if (!string.IsNullOrEmpty(imagePath))
                    {
                        // Kiểm tra xem đường dẫn file đã tồn tại trong Dictionary chưa
                        if (imagePaths.ContainsKey(imagePath))
                        {
                            using (Transaction transaction = new Transaction(doc))
                            {
                                transaction.Start("Delete Image");
                                // Nếu đã tồn tại, xóa ảnh hiện tại
                                doc.Delete(image.Id);
                                transaction.Commit();
                            }
                        }
                        else
                        {
                            // Nếu chưa tồn tại, thêm đường dẫn file và element vào Dictionary
                            imagePaths.Add(imagePath, image);
                        }
                    }
                }
            }
        }
        #endregion xoá file trùng path
    }
}
