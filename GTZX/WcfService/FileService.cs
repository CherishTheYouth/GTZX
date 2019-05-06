using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.ServiceModel;
using Helper.Extension;
using WcfContract.ServiceContract;

namespace WcfService
{
    public class FileService : IFile
    {
        public bool DeleteFile(string path)
        {
            try
            {
                var fullName = GetFullName(path);

                var fileInfo = new FileInfo(fullName);
                fileInfo.Delete();
                return true;
            }
            catch (Exception exception)
            {
                throw ExceptionHandler.HandleException(exception);
            }
        }

        public byte[] DownloadFile(string path, long offset, long length)
        {
            try
            {
                var fullName = GetFullName(path);
                byte[] buffer;
                using (var fileStream = new FileStream(fullName, FileMode.Open, FileAccess.Read))
                {
                    var readLength = Math.Min(length, fileStream.Length - offset);
                    buffer = new byte[readLength];
                    fileStream.Seek(offset, SeekOrigin.Begin);
                    fileStream.Read(buffer, 0, Convert.ToInt32(readLength));
                }
                return buffer;
            }
            catch (Exception exception)
            {
                throw ExceptionHandler.HandleException(exception);
            }
        }

        public long UploadFile(string path, long offset, byte[] bufferBytes)
        {
            try
            {
                var fullName = GetFullName(path, false);

                var fileInfo = new FileInfo(fullName);
                if (!string.IsNullOrWhiteSpace(fileInfo.DirectoryName) && !Directory.Exists(fileInfo.DirectoryName))
                {
                    Directory.CreateDirectory(fileInfo.DirectoryName);
                }

                using (var fileStream = new FileStream(fullName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    fileStream.Seek(offset, SeekOrigin.Begin);
                    fileStream.Write(bufferBytes, 0, bufferBytes.Length);
                    fileStream.Flush();

                    return fileStream.Length;
                }
            }
            catch (Exception exception)
            {
                throw ExceptionHandler.HandleException(exception);
            }
        }

        public long GetUploadedLength(string path)
        {
            try
            {
                var fullName = GetFullName(path);
                var fileInfo = new FileInfo(fullName);
                return fileInfo.Length;
            }
            catch (Exception exception)
            {
                throw ExceptionHandler.HandleException(exception);
            }
        }

        public bool IsExist(string path)
        {
            try
            {
                var fullName = GetFullName(path, false);
                return File.Exists(fullName);
            }
            catch (Exception exception)
            {
                throw ExceptionHandler.HandleException(exception);
            }
        }

        public string GenerateShortcut(string path)
        {
            try
            {
                var fullName = GetFullName(path);
                if (!fullName.IsImage()) throw new FaultException("图片格式不正确！");

                var folder = path.Substring(0, path.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
                var fileName = path.Substring(path.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
                var shortcutName = "shortcut_" + fileName;
                var shortcutPath = folder + shortcutName;

                using (var originalImg = Image.FromFile(fullName))
                {
                    // 原始大小
                    var oWidth = originalImg.Width;
                    var oHeight = originalImg.Height;

                    const int maxSize = 200;

                    var rate = Math.Max(Math.Max(oWidth / maxSize, oHeight / maxSize), 1);

                    // 缩略图宽高均不超过200
                    var width = oWidth / rate;
                    var height = oHeight / rate;

                    using (var bitmap = new Bitmap(width, height))
                    {
                        using (var g = Graphics.FromImage(bitmap))
                        {
                            g.CompositingQuality = CompositingQuality.HighSpeed;
                            g.SmoothingMode = SmoothingMode.HighSpeed;
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.Clear(Color.Transparent);
                            g.DrawImage(originalImg, new Rectangle(0, 0, width, height),
                                new Rectangle(0, 0, oWidth, oHeight), GraphicsUnit.Pixel);
                        }

                        using (var fileStream = new FileStream(GetFullName(shortcutPath, false), FileMode.OpenOrCreate))
                        {
                            bitmap.Save(fileStream, ImageFormat.Jpeg);
                            fileStream.Flush();
                        }
                    }
                }
                return shortcutPath;
            }
            catch (Exception exception)
            {
                throw ExceptionHandler.HandleException(exception);
            }
        }

        private string GetFullName(string path, bool validateExist = true)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new FaultException("文件路径不可为空！");

            var folderName = ConfigurationManager.AppSettings["UploadFileFolder"];
            if (!Directory.Exists(Directory.GetDirectoryRoot(folderName)))
                throw new FaultException("上传服务配置异常！");

            var fullName = Path.Combine(folderName, path);
            if (validateExist && !File.Exists(fullName))
            {
                throw new FaultException("文件不存在！");
            }
            return fullName;
        }
    }
}
