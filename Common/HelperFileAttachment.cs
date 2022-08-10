using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Drawing;

namespace Common
{
    public class HelperFileAttachment : Controller
    {
        /// <summary>
        /// Coppy file lên server
        /// </summary>
        /// <param name="fileAnhCreate"></param>
        /// <returns></returns>
        public string CoppyFileToServer(string base64, string fileName, string pathFolder)
        {
            string destFile = "";

            if (!Directory.Exists(pathFolder))
            {
                Directory.CreateDirectory(pathFolder);
            }
            var countFoleder = 0;
            var directory = System.IO.Directory.GetDirectories(pathFolder);
            if (directory != null)
                countFoleder = directory.Length + 1;
            var folder = Path.Combine(pathFolder + "\\", countFoleder.ToString());
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            try
            {
                destFile = Path.Combine(folder + "\\", fileName);
                if (fileName.ToLower().Contains(".jpg") || fileName.ToLower().Contains(".bmp") || fileName.ToLower().Contains(".png"))
                {
                    var bytes = Convert.FromBase64String(base64);
                    using (var imageFile = new FileStream(destFile, FileMode.Create))
                    {
                        imageFile.Write(bytes, 0, bytes.Length);
                        imageFile.Flush();
                    }
                }
                else //if(fileName.ToLower().Contains(".doc") || fileName.ToLower().Contains(".docx"))
                {
                    using (System.IO.FileStream stream = System.IO.File.Create(destFile))
                    {
                        var imageParts = base64.Split(',').ToList<string>();
                        System.Byte[] byteArray = System.Convert.FromBase64String(imageParts[imageParts.Count - 1]);
                        stream.Write(byteArray, 0, byteArray.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                destFile = "";
            }

            return destFile;
        }
        public string CoppyFile2Server(string base64, string fileName, string pathFolder, string pathFile)
        {
            string destFile = "";
            string returnData = string.Empty;

            if (!Directory.Exists(pathFolder))
            {
                Directory.CreateDirectory(pathFolder);
            }
            var countFoleder = 0;
            var directory = System.IO.Directory.GetDirectories(pathFolder);
            if (directory != null)
                countFoleder = directory.Length + 1;
            var folder = Path.Combine(pathFolder + "\\", countFoleder.ToString());
            var folder1 = pathFile + "/" + countFoleder.ToString();
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            try
            {
                destFile = Path.Combine(folder + "\\", fileName);
                returnData = folder1 + "/" + fileName;

                if (fileName.ToLower().Contains(".jpg") || fileName.ToLower().Contains(".bmp") || fileName.ToLower().Contains(".png"))
                {
                    var bytes = Convert.FromBase64String(base64);
                    using (var imageFile = new FileStream(destFile, FileMode.Create))
                    {
                        imageFile.Write(bytes, 0, bytes.Length);
                        imageFile.Flush();
                    }
                }
                else
                {
                    using (System.IO.FileStream stream = System.IO.File.Create(destFile))
                    {
                        var imageParts = base64.Split(',').ToList<string>();
                        System.Byte[] byteArray = System.Convert.FromBase64String(imageParts[imageParts.Count - 1]);
                        stream.Write(byteArray, 0, byteArray.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                destFile = "";
                returnData = string.Empty;
            }

            return returnData;
        }

        /// <summary>
        /// Xóa file trên s
        /// </summary>
        /// <param name="fileAnhCreate"></param>
        /// <returns></returns>
        public void DeleteFile(string pathRoot, string path)
        {
            try
            {


                var arrayPath = path.IndexOf("FileUpload");
                string pathGet = path.Substring(0, arrayPath) + "FileUpload";
                string patttt = path.Replace(pathGet, pathRoot);

                System.IO.File.Delete(path);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Chuyển file sang base64
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string ConvertFileToBase64(string path)
        {
            string base64String = "";
            var pathRoot = Server.MapPath("~/FileUpload");

            var arrayPath = path.IndexOf("FileUpload");
            string pathGet = path.Substring(0, arrayPath) + "FileUpload";
            string patttt = path.Replace(pathGet, pathRoot);

            try
            {
                if (!string.IsNullOrEmpty(path))
                {
                    using (Image image = Image.FromFile(path))
                    {
                        using (MemoryStream m = new MemoryStream())
                        {
                            image.Save(m, image.RawFormat);
                            byte[] imageBytes = m.ToArray();

                            // Convert byte[] to Base64 String
                            base64String = "data:image/jpeg;base64," + Convert.ToBase64String(imageBytes);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return base64String;
        }

        /// <summary>
        /// Lấy đường dẫn file server
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GetFileServer(string path)
        {
            var pathRoot = Server.MapPath("~/FileUpload");

            var arrayPath = path.IndexOf("FileUpload");
            string pathGet = path.Substring(0, arrayPath) + "FileUpload";
            string patttt = path.Replace(pathGet, pathRoot);

            return patttt;
        }
    }
}
