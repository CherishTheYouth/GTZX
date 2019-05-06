using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using Modules;
using ORM;
using Newtonsoft.Json;
using Helper.Extension;

namespace Console.Controllers
{
    public class RegulationController : Controller
    {
        readonly MyDbContext context = new MyDbContext();

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取政策文件列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public ActionResult GetList(int page, string regulationName,string regulationNo,string publishDep,DateTime? publishDate)
        {
            IQueryable<Regulation> regulation = context.Regulations;

            //查询功能的筛选条件 文件名
            if (!string.IsNullOrWhiteSpace(regulationName))
            {
                regulation = regulation.Where(x => x.RegulationName.Contains(regulationName));
            }
            //文件编号
            if (!string.IsNullOrWhiteSpace(regulationNo))
            {
                regulation = regulation.Where(x => x.RegulationNo.Contains(regulationNo));
            }
            //颁布部门
            if (!string.IsNullOrWhiteSpace(publishDep))
            {
                regulation = regulation.Where(x => x.PublishDep.Contains(publishDep));
            }
            regulation = regulation.OrderBy(x => x.PublishDate).Where(x => x.IsEnable == true);
            var count = regulation.Count();
            var list = regulation.ToPage(page, count).ToList();

        //    var employees = context.Employees.ToList();
            var result = list.Select(x => new
            {
                x.Id,
                x.RegulationName,
                x.RegulationNo,
                x.PublishDep,
                PublishDate = x.PublishDate.Format("yyyy-MM-dd")
                //(employees.FirstOrDefault(y => y.Id == x.Id) ?? new Employee()).FullName,
                //State = x.IsEnable ? "启用" : "禁用"
            });
            return Json(new { Count = count, Data = result });
        }





        public ActionResult Modify(Guid? id, Guid? parentId)
        {
            var data = context.Regulations.FirstOrDefault(x => x.Id == id) ?? new Regulation() { ParentId = parentId };
            return View(data);
           
        }


        /// <summary>
        /// 测试用demo 与项目功能无关
        /// </summary>
        /// <returns></returns>
        public ActionResult UeDemo()
        {
            var data = new Regulation();
            return View(data);
        }

        /// <summary>
        /// 测试用demo 与项目功能无关
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fc"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult SaveUe(Regulation data,FormCollection fc)
        {
            var content = fc["editor"];
            return Content(content);
        }

        /// <summary>
        /// 保存政策文件
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fc"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Save(Regulation data, FormCollection fc) // 
        {
            try
            {
             //   this.ValidateModel();
                var content = fc["editor"]; //获取表单中元素名称为editor的内容
                var newData = context.Regulations.Find(data.Id) ?? new Regulation();
                var changeInfoList = data.CompareDifference(newData, ModelState.Keys);
                newData.SetValues(data, ModelState.Keys);

                string logType = "";
                if (newData.Id.Equals(Guid.Empty))
                {
                    logType = "Add";
                    newData.Id = Guid.NewGuid();
                    newData.PublishDate = DateTime.Now;
                //    newData.IsDelete = false;
                //    newData.CreatorID = CacheUtil.LoginUser.Id;
                }
                //   newData.IsPublish = data.IsPublish;
                //   newData.IsSign = data.IsSign;
                   newData.RegulationContent = content;
                context.Regulations.AddOrUpdate(newData);

                //操作日志（添加、修改）
                if (logType == "Add")
                {
                    context.WriteLog(new Log
                    {
                        Content = "【通知公告】" + newData.RegulationName,
                        UserId = CacheUtil.LoginUser.Id,
                        TargetId = newData.Id,
                        Type = LogType.Add
                    });
                }
                else
                {
                    context.WriteLog(new Log
                    {
                        Content = "【通知公告】" + newData.RegulationName,
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
        /// 删除文件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Remove(Guid id)
        {
            try
            {
                var data = context.Regulations.Find(id);
                if (data == null) throw new Exception("要删除的信息不存在，请重新加载页面");
                //context.Notices.Remove(data);
                data.IsEnable = false;
                context.Regulations.AddOrUpdate(data);
                //操作日志（添加、删除）
                context.WriteLog(new Log
                {
                    Content = "【政策维护】" + data.RegulationName,
                    UserId = CacheUtil.LoginUser.Id,
                    TargetId = data.Id,
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

        public ActionResult ShowRegulation(Guid? id) {
            var data = context.Regulations.Find(id) ?? new Regulation();
            ViewBag.RegulationName = data.RegulationName;
            return View(data);
        }

    }
}
