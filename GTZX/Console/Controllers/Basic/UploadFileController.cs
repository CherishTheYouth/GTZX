using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Modules;
using Newtonsoft.Json;
using ORM;
using WcfContract.ServiceContract;
using WcfContract.Wcf;
using Helper.Extension;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using Helper;
using ICSharpCode.SharpZipLib.BZip2;
using System.Web.ApplicationServices;

namespace Console.Controllers
{
    public class UploadFileController : Controller
    {
        private readonly MyDbContext context = new MyDbContext();
        private readonly IFile fileService = new ServiceProxy<IFile>().CreateProxy();
        /// <summary>
        /// 分段上传/下载量
        /// </summary>
        private const long BlockSize = 1048576;

        /// <summary>
        /// 图片上传界面
        /// </summary>G:\proj-hbch\trunk\HBCH\WcfContract\Wcf\ServiceProxy.cs
        /// <param name="extension">文件扩展名，如jpg,png,gif,rar,zip,doc等</param>
        /// <param name="title">扩展名描述</param>
        /// <returns></returns>
        public ActionResult Index(string extension = "*", string title = "所有文件")
        {
            ViewBag.Extension = extension;
            ViewBag.Title = title;
            return View();
        }

        [HttpPost]
        public string Upload()
        {
            var id = Guid.NewGuid();
            var name = string.Empty;

            // 循环文件
            foreach (string key in Request.Files.Keys)
            {
                var file = Request.Files[key];
                if (file == null) continue;
                name = Request.Form["filename"];

                // 文件上传后的路径
                var path = Path.Combine(id.ToString(), name);

                var stream = file.InputStream;
                long offset = 0;
                stream.Position = offset;
                while (offset < stream.Length)
                {
                    // 分段上传
                    var buffer = new byte[Math.Min(BlockSize, (stream.Length - offset))];
                    var i = stream.Read(buffer, 0, buffer.Length);
                    if (i <= 0) break;
                    offset = fileService.UploadFile(path, offset, buffer);
                }

                var uploadFile = new UploadFile
                {
                    Id = id,
                    Name = name,
                    FileSize = file.InputStream.Length,
                    Path = path,
                    UploadTime = DateTime.Now
                };
                context.UploadFiles.Add(uploadFile);
                context.SaveChanges();
            }
            return JsonConvert.SerializeObject(new { Id = id, Name = name });
        }




        public ActionResult UploadFileList(Guid targetId, string group)
        {
            ViewBag.targetId = targetId;
            ViewBag.group = group;
            return View();
        }

        public ActionResult GetUploadFileList(int page, Guid targetId, string group)
        {
            IQueryable<UploadFileRelation> uploadFileRelations = context.UploadFileRelations.Where(x => x.TargetId == targetId && x.Group==group);
            IQueryable<UploadFile> data = context.UploadFiles.Where(x => uploadFileRelations.Any(a=>x.Id==a.UploadFileId));
            data = data.OrderByDescending(x => x.UploadTime);
            var count = data.Count();
            var list = data.ToPage(page, count).ToList();
            var result = list.Select(x => new
            {
                x.Id,
                x.Name,
                x.Path,
                UploadTime = x.UploadTime.Format("yyyy-MM-dd"),
            });
            return Json(new { Count = count, Data = result });
        }

        public ActionResult SetUploadFileRelation(Guid targetId, string group, List<Guid> idList)
        {
            try
            {
                idList = idList ?? new List<Guid>();
                context.HandleUploadFileRelation(targetId, idList, group);
                context.SaveChanges();

                return Json(new
                {
                    Result = true
                });
            }
            catch (Exception exception)
            {
                return
                    Json(
                        new
                        {
                            Result = false,
                            exception.Message
                        });
            }
        }

        public ActionResult Romove(Guid id)
        {
            try
            {
                var data = context.UploadFileRelations.Where(x => x.UploadFileId == id);
                if (data == null) throw new Exception("要删除的数据不存在，请重新加载页面");
                if (data.Any())
                {
                    context.UploadFileRelations.RemoveRange(data);
                }
                context.SaveChanges();

                return Json(new { Result = true });
            }
            catch (Exception exception)
            {
                return Json(new { Result = false, exception.Message });
            }
        }

        public ActionResult Download(Guid id, bool isShortcut = false)
        {
            var uploadFile = context.UploadFiles.FirstOrDefault(x => x.Id.Equals(id));
            if (uploadFile == null) return null;

            string contentType;
            var fileName = string.Empty;
            contentType = "application/octet-stream";
            fileName = HttpUtility.UrlEncode(uploadFile.Name, Encoding.UTF8);
            using (var stream = new MemoryStream())
            {
                if (isShortcut)
                {
                    // 缩略图直接一次性下载显示
                    var buffer =
                            fileService.DownloadFile(
                                !string.IsNullOrWhiteSpace(uploadFile.ShortcutPath)
                                    ? uploadFile.ShortcutPath
                                    : uploadFile.Path, 0, int.MaxValue);
                    stream.Write(buffer, Convert.ToInt32(stream.Length), buffer.Length);
                }
                else
                {
                    var buffer = fileService.DownloadFile(uploadFile.Path, 0, uploadFile.FileSize);
                    stream.Write(buffer, 0, buffer.Length);
                }
                return File(stream.GetBuffer(), contentType, fileName);
            }
        }
    }
}
