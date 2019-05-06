using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using Modules;
using ORM;
using Newtonsoft.Json;
using Helper.Extension;
using Modules.Biz;

namespace Console.Controllers.Biz
{
    public class RegulationSourceFileController : Controller
    {

        MyDbContext context = new MyDbContext();
        // GET: Default
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetList(int page, string fileName, string fileNo)
        {
            IQueryable<RegulationSourceFile> regulationFile = context.RegulationSourceFiles;
            //查询功能的筛选条件 文件名
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                regulationFile = regulationFile.Where(x => x.FileName.Contains(fileName));
            }
            //文件编号
            if (!string.IsNullOrWhiteSpace(fileNo))
            {
                regulationFile = regulationFile.Where(x => x.FileNo.Contains(fileNo));
            }
            regulationFile = regulationFile.OrderBy(x => x.UploadDate).Where(x => x.IsEnable == true);
            var count = regulationFile.Count();
            var list = regulationFile.ToPage(page, count).ToList();

            //    var employees = context.Employees.ToList();
            var result = list.Select(x => new
            {
                x.Id,
                x.FileName,
                x.FileNo,
                UploadDate = x.UploadDate.Format("yyyy-MM-dd")
                //(employees.FirstOrDefault(y => y.Id == x.Id) ?? new Employee()).FullName,
                //State = x.IsEnable ? "启用" : "禁用"
            });
            return Json(new { Count = count, Data = result });
        }


        /// <summary>
        /// 新增/修改
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult Modify(Guid? Id,Guid? parentId)
        {
            var data = context.RegulationSourceFiles.FirstOrDefault(x => x.Id == Id ) ?? new RegulationSourceFile() { ParentId = parentId};
        //    var item = context.ManageThings.Find(id) ?? new ManageThing();
            if (data.Id.Equals(Guid.Empty))
            {
                data.Id = Guid.NewGuid();
            }
            return View(data);       
        }

        public ActionResult Save(RegulationSourceFile data)
        {
            try
            {
                //   this.ValidateModel();
                var newData = context.RegulationSourceFiles.Find(data.Id) ?? new RegulationSourceFile();
                var changeInfoList = data.CompareDifference(newData, ModelState.Keys);
                newData.SetValues(data, ModelState.Keys);
                string logType = "";
                //if (newData.Id.Equals(Guid.Empty))
                //{
                //    logType = "Add";
                //    newData.Id = data.Id;
                //    newData.UploadDate = DateTime.Now;
                //    //    newData.IsDelete = false;
                //    //    newData.CreatorID = CacheUtil.LoginUser.Id;
                //}
                //   newData.IsPublish = data.IsPublish;
                //   newData.IsSign = data.IsSign;
                newData.UploadDate = DateTime.Now;
                newData.SourceFile = "政策文件txt";
                context.RegulationSourceFiles.AddOrUpdate(newData);

                //操作日志（添加、修改）
                if (logType == "Add")
                {
                    context.WriteLog(new Log
                    {
                        Content = "【政策文件原始文件管理】" + newData.FileName,
                        UserId = CacheUtil.LoginUser.Id,
                        TargetId = newData.Id,
                        Type = LogType.Add
                    });
                }
                else
                {
                    context.WriteLog(new Log
                    {
                        Content = "【政策文件原始文件管理】" + newData.FileName,
                        TargetId = newData.Id,
                        UserId = CacheUtil.LoginUser.Id,
                        Type = LogType.Modify
                    }, changeInfoList);
                }

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

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="targetId">所在记录id</param>
        /// <param name="group">组别</param>
        /// <returns></returns>
        public ActionResult SetUploadFile(Guid targetId, int group = 0)
        {
            string groupStr = UploadFileType.TypeString(group);
            IQueryable<UploadFileRelation> uploadFileRelationList = context.UploadFileRelations.AsQueryable();
            if (targetId != null && targetId != Guid.Empty)
            {
                uploadFileRelationList = uploadFileRelationList.Where(p => p.TargetId == targetId);
            }
            if (!string.IsNullOrWhiteSpace(groupStr))
            {
                uploadFileRelationList = uploadFileRelationList.Where(x => x.Group == groupStr);
            }
            IQueryable<UploadFile> data = context.UploadFiles.Where(x => uploadFileRelationList.Any(a => x.Id == a.UploadFileId));
            data = data.OrderByDescending(x => x.UploadTime);
            var list = data.ToList();
            var result = list.Select(x => new
            {
                x.Id,
                x.Name,
             //   x.Path,
                UploadTime = x.UploadTime.Format("yyyy-MM-dd"),
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShowDetail(Guid? id)
        {
            var data = context.RegulationSourceFiles.Find(id) ?? new RegulationSourceFile();
            return View(data);

        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Romove(Guid id)
        {
            try
            {
                var table = context.RegulationSourceFiles.Find(id);
                if (table == null) throw new Exception("要删除的项目不存在，请重新加载页面");
                table.IsEnable = false;
                context.RegulationSourceFiles.AddOrUpdate(table);
                //context.ManageThings.Remove(table);

                context.WriteLog(new Log
                {
                    Content = "【管理事务归档资料】" + table.FileName,
                    UserId = CacheUtil.LoginUser.Id,
                    TargetId = table.Id,
                    Type = LogType.Delete
                });

                context.SaveChanges();

                return Json(new { Result = true });
            }
            catch (Exception exception)
            {
                return Json(new { Result = false, exception.Message });
            }
        }

    }
}