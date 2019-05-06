using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using Helper.Extension;
using Modules;
using ORM;

namespace Console.Controllers
{
    public class FuncController : Controller
    {
        readonly MyDbContext context = new MyDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetList()
        {
            RemoveUnusedFuncs();
            var list = context.GetSerializedFuncs();
            var result = GetDisplayList(list);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public void RemoveUnusedFuncs()
        {
            var temp = context.Funcs.Where(x => x.ParentId.HasValue && !context.Funcs.Any(y => y.Id == x.ParentId));
            if (temp.Any())
            {
                context.Funcs.RemoveRange(temp);
                context.SaveChanges();
            }
        }

        public IList<object> GetDisplayList(IList<Func> funcs)
        {
            var list = new List<object>();
            foreach (var func in funcs)
            {
                var children = new List<object>();
                if (func.Children.Any())
                {
                    children.AddRange(GetDisplayList(func.Children));
                }
                list.Add(new
                {
                    key = func.Id,
                    title = func.Name,
                    id = func.Id,
                    icon = "icon-treenode-func",
                    code = func.FuncCode,
                    remark = func.Remark,
                    children
                });
            }
            return list;
        }

        public ActionResult Modify(Guid? id, Guid? parentId)
        {
            var func = context.Funcs.FirstOrDefault(x => x.Id == id) ?? new Func { ParentId = parentId };
            return View(func);
        }

        public ActionResult SaveFunc(Func func)
        {
            try
            {
                this.ValidateModel();

                var newFunc = context.Funcs.Find(func.Id) ?? new Func();
                newFunc.SetValues(func, ModelState.Keys);
                if (newFunc.Id.Equals(Guid.Empty))
                {
                    newFunc.Id = Guid.NewGuid();
                }
                if (!string.IsNullOrWhiteSpace(newFunc.FuncCode) &&
                    context.Funcs.Any(x => x.FuncCode == newFunc.FuncCode && x.Id != newFunc.Id))
                {
                    throw new Exception("该功能编码已被使用！");
                }
                if (!newFunc.OrderNumber.HasValue)
                {
                    var maxOrderItem =
                        context.Funcs.Where(x => x.ParentId == newFunc.ParentId)
                            .OrderByDescending(x => x.OrderNumber)
                            .FirstOrDefault();
                    newFunc.OrderNumber = maxOrderItem != null ? maxOrderItem.OrderNumber + 1 : 999;
                }
                context.Funcs.AddOrUpdate(newFunc);
                context.SaveChanges();

                return Json(new
                {
                    Result = true,
                    Data =
                        new
                        {
                            title = newFunc.Name,
                            id = newFunc.Id,
                            icon = "icon-treenode-func",
                            code = newFunc.FuncCode,
                            remark = newFunc.Remark
                        }
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

        public ActionResult RemoveFunc(Guid id)
        {
            try
            {
                var func = context.Funcs.FirstOrDefault(x => x.Id == id);
                if (func == null) throw new Exception("要删除的功能不存在，请重新加载页面");
                context.Funcs.Remove(func);
                context.SaveChanges();
                RemoveUnusedFuncs();

                return Json(new { Result = true });
            }
            catch (Exception exception)
            {
                return Json(new { Result = false, exception.Message });
            }
        }

        public ActionResult DragFunc(Guid sourceId, Guid targetId, string hitMode)
        {
            try
            {
                var sourceFunc = context.Funcs.FirstOrDefault(x => x.Id == sourceId);
                if (sourceFunc == null) throw new Exception("拖动的功能已被删除，请重新加载页面");
                var targetFunc = context.Funcs.FirstOrDefault(x => x.Id == targetId);
                if (targetFunc == null) throw new Exception("目标功能已被删除，请重新加载页面");
                if (hitMode.Equals("over", StringComparison.CurrentCultureIgnoreCase))
                {
                    sourceFunc.ParentId = targetId;
                    // 获取排序号
                    var maxOrderItem =
                        context.Funcs.Where(x => x.ParentId == targetId)
                            .OrderByDescending(x => x.OrderNumber)
                            .FirstOrDefault();
                    sourceFunc.OrderNumber = maxOrderItem == null ? 999 : maxOrderItem.OrderNumber + 1;
                }
                else
                {
                    sourceFunc.ParentId = targetFunc.ParentId;
                    if (hitMode.Equals("before", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var previousOrderItem =
                            context.Funcs.Where(
                                x =>
                                    x.ParentId == targetFunc.ParentId && x.OrderNumber <= targetFunc.OrderNumber &&
                                    x.Id != targetId && x.Id != sourceId)
                                .OrderByDescending(x => x.OrderNumber).FirstOrDefault();
                        sourceFunc.OrderNumber = previousOrderItem == null
                            ? targetFunc.OrderNumber - 1
                            : targetFunc.OrderNumber - (targetFunc.OrderNumber - previousOrderItem.OrderNumber) / 2;
                    }
                    else
                    {
                        var nextOrderItem =
                            context.Funcs.Where(
                                x =>
                                    x.ParentId == targetFunc.ParentId && x.OrderNumber >= targetFunc.OrderNumber &&
                                    x.Id != targetId && x.Id != sourceId)
                                .OrderBy(x => x.OrderNumber).FirstOrDefault();
                        sourceFunc.OrderNumber = nextOrderItem == null
                            ? targetFunc.OrderNumber + 1
                            : targetFunc.OrderNumber + (nextOrderItem.OrderNumber - targetFunc.OrderNumber) / 2;
                    }
                }
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
