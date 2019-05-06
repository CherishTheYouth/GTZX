using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using Modules;
using ORM;
using Newtonsoft.Json;

namespace Console.Controllers
{
    public class DicController : Controller
    {
        readonly MyDbContext context = new MyDbContext();

        public ActionResult Index(Dic? dic)
        {
            return View(dic);
        }

        public ActionResult GetList(Dic dic)
        {
            var list = context.GetSerializedDicItems(dic);
            var result = GetDisplayList(list);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public IList<object> GetDisplayList(IList<DicItem> dicItems)
        {
            var list = new List<object>();
            foreach (var dicItem in dicItems)
            {
                var children = new List<object>();
                if (dicItem.Children.Any())
                {
                    children.AddRange(GetDisplayList(dicItem.Children));
                }
                list.Add(new
                {
                    key = dicItem.Id,
                    title = dicItem.Name,
                    id = dicItem.Id,
                    remark = dicItem.Remark,
                    icon = "icon-treenode-dic",
                    children
                });
            }
            return list;
        }

        public ActionResult Modify(Guid? id, Guid? parentId, Dic? dic)
        {
            var dicItem = context.DicItems.Find(id);
            if (dicItem != null) return View(dicItem);
            if (parentId.HasValue)
            {
                var parentItem = context.DicItems.Find(parentId);
                if (parentItem != null)
                {
                    dicItem = new DicItem
                    {
                        ParentId = parentItem.Id,
                        Dic = parentItem.Dic
                    };
                    return View(dicItem);
                }
            }
            if (!dic.HasValue)
                return Content("字典是必选的。");
            dicItem = new DicItem
            {
                Dic = dic.Value
            };
            return View(dicItem);
        }

        public ActionResult SaveDicItem(DicItem dicItem)
        {
            try
            {
                this.ValidateModel();
                if (dicItem.Id.Equals(Guid.Empty))
                {
                    dicItem.Id = Guid.NewGuid();
                    dicItem.CreateTime = DateTime.Now;
                }
                if (!dicItem.OrderNumber.HasValue)
                {
                    var maxOrderItem =
                        context.DicItems.Where(x => x.ParentId == dicItem.ParentId && x.Dic == dicItem.Dic)
                            .OrderByDescending(x => x.OrderNumber)
                            .FirstOrDefault();
                    dicItem.OrderNumber = maxOrderItem != null ? maxOrderItem.OrderNumber + 1 : 999;
                }
                context.DicItems.AddOrUpdate(dicItem);
                context.SaveChanges();

                return Json(new
                {
                    Result = true,
                    Data =
                        new
                        {
                            key = dicItem.Id,
                            title = dicItem.Name,
                            id = dicItem.Id,
                            remark = dicItem.Remark,
                            icon = "icon-treenode-dic"
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

        public ActionResult RemoveDicItem(Guid id)
        {
            try
            {
                var dicItem = context.DicItems.FirstOrDefault(x => x.Id == id);
                if (dicItem == null || dicItem.IsDelete) throw new Exception("要删除的字典项不存在，请重新加载页面");
                dicItem.IsDelete = true;
                context.SaveChanges();

                return Json(new { Result = true });
            }
            catch (Exception exception)
            {
                return Json(new { Result = false, exception.Message });
            }
        }

        public ActionResult DragDicItem(Guid sourceId, Guid targetId, string hitMode)
        {
            try
            {
                var sourceDicItem = context.DicItems.FirstOrDefault(x => x.Id == sourceId);
                if (sourceDicItem == null) throw new Exception("拖动的字典项已被删除，请重新加载页面");
                var targetDicItem = context.DicItems.FirstOrDefault(x => x.Id == targetId);
                if (targetDicItem == null) throw new Exception("目标项已被删除，请重新加载页面");
                if (sourceDicItem.Dic != targetDicItem.Dic) throw new Exception("禁止拖动！");
                if (hitMode.Equals("over", StringComparison.CurrentCultureIgnoreCase))
                {
                    sourceDicItem.ParentId = targetId;
                    // 获取排序号
                    var maxOrderItem =
                        context.DicItems.Where(x => x.ParentId == targetId && x.Dic == sourceDicItem.Dic)
                            .OrderByDescending(x => x.OrderNumber)
                            .FirstOrDefault();
                    sourceDicItem.OrderNumber = maxOrderItem == null ? 999 : maxOrderItem.OrderNumber + 1;
                }
                else
                {
                    sourceDicItem.ParentId = targetDicItem.ParentId;
                    if (hitMode.Equals("before", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var previousOrderItem =
                            context.DicItems.Where(
                                x =>
                                    x.ParentId == targetDicItem.ParentId && x.OrderNumber <= targetDicItem.OrderNumber &&
                                    x.Id != targetId && x.Id != sourceId && x.Dic == sourceDicItem.Dic)
                                .OrderByDescending(x => x.OrderNumber).FirstOrDefault();
                        sourceDicItem.OrderNumber = previousOrderItem == null
                            ? targetDicItem.OrderNumber - 1
                            : targetDicItem.OrderNumber - (targetDicItem.OrderNumber - previousOrderItem.OrderNumber) / 2;
                    }
                    else
                    {
                        var nextOrderItem =
                            context.DicItems.Where(
                                x =>
                                    x.ParentId == targetDicItem.ParentId && x.OrderNumber >= targetDicItem.OrderNumber &&
                                    x.Id != targetId && x.Id != sourceId && x.Dic == sourceDicItem.Dic)
                                .OrderBy(x => x.OrderNumber).FirstOrDefault();
                        sourceDicItem.OrderNumber = nextOrderItem == null
                            ? targetDicItem.OrderNumber + 1
                            : targetDicItem.OrderNumber + (nextOrderItem.OrderNumber - targetDicItem.OrderNumber) / 2;
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


        /// <summary>
        /// 通用的数字选择界面
        /// </summary>
        /// <param name="allowMulti">是否允许多选</param>
        /// <param name="selectedList">已选列表</param>
        /// <param name="dic"></param>
        /// <returns></returns>
        public ActionResult SelectDic(bool allowMulti, List<string> selectedList,Dic dic)
        {
            ViewBag.allowMulti = allowMulti;
            int allowMultiNum = 2;
            List<Guid> dicIdList;
            // 跳转的Action通过TempData["selectedList"]传递已选项，否则接收到的selectedList参数会为空。
            if (selectedList == null || selectedList.Count == 0)
            {
                dicIdList = (TempData["selectedList"] as List<Guid>) ?? new List<Guid>();
            }
            else
            {
                dicIdList = selectedList.Select(Guid.Parse).ToList();
            }
            if (!allowMulti)
            {
                allowMultiNum = 1;
            }
            ViewBag.allowMulti = allowMulti;
            ViewBag.allowMultiNum = allowMultiNum;
            ViewBag.selectedList = JsonConvert.SerializeObject(dicIdList);
            ViewBag.dic = dic;
            return View();
        }

        public ActionResult GetSelectList(string selectedList,Dic dic)
        {
            List<string> sel = JsonConvert.DeserializeObject<List<string>>(selectedList);
            List<Guid> selectedListGuid = sel.Select(Guid.Parse).ToList();
            var list = context.GetSerializedDicItems(dic);
            var result = GetSelectDisplayList(list, selectedListGuid);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public IList<object> GetSelectDisplayList(IList<DicItem> dicItems, List<Guid> selectedList)
        {
            var list = new List<object>();
            foreach (var dicItem in dicItems)
            {
                var children = new List<object>();
                if (dicItem.Children.Any())
                {
                    children.AddRange(GetSelectDisplayList(dicItem.Children, selectedList));
                }
                list.Add(new
                {
                    id = dicItem.Id,
                    key = dicItem.Id,
                    title = dicItem.Name,
                    selected = selectedList.Contains(dicItem.Id),
                    icon = "icon-treenode-department",
                    expanded = true,
                    children
                });
            }
            return list;
        }
    }
}
