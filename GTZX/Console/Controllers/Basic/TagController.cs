using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using Modules;
using ORM;

namespace Console.Controllers
{
    public class TagController : Controller
    {
        readonly MyDbContext context = new MyDbContext();

        public ActionResult Index(TagType type = TagType.Default)
        {
            return View(type);
        }

        public ActionResult GetList(TagType type)
        {
            var list = context.Tags.Where(x => x.Type == type).OrderBy(x => x.OrderNumber).ToList();
            var result = list.Select(x => new
            {
                key = x.Id,
                title = x.Name,
                id = x.Id,
                icon = "icon-treenode-tag",
                remark = x.Remark
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Modify(Guid? id, TagType? type)
        {
            var tag = context.Tags.FirstOrDefault(x => x.Id == id) ?? new Tag { Type = type ?? TagType.Default };
            return View(tag);
        }

        public ActionResult SaveTag(Tag tag)
        {
            try
            {
                this.ValidateModel();
                if (tag.Id.Equals(Guid.Empty))
                {
                    tag.Id = Guid.NewGuid();
                }
                if (!tag.OrderNumber.HasValue)
                {
                    var maxOrderItem =
                        context.Tags
                            .OrderByDescending(x => x.OrderNumber)
                            .FirstOrDefault();
                    tag.OrderNumber = maxOrderItem != null ? maxOrderItem.OrderNumber + 1 : 999;
                }
                context.Tags.AddOrUpdate(tag);
                context.SaveChanges();

                return Json(new
                {
                    Result = true,
                    Data =
                        new
                        {
                            key = tag.Id,
                            title = tag.Name,
                            id = tag.Id,
                            icon = "icon-treenode-tag",
                            remark = tag.Remark
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

        public ActionResult RemoveTag(Guid id)
        {
            try
            {
                var tag = context.Tags.FirstOrDefault(x => x.Id == id);
                if (tag == null) throw new Exception("要删除的项不存在，请重新加载页面");
                context.Tags.Remove(tag);
                context.SaveChanges();

                return Json(new { Result = true });
            }
            catch (Exception exception)
            {
                return Json(new { Result = false, exception.Message });
            }
        }

        public ActionResult DragTag(Guid sourceId, Guid targetId, string hitMode)
        {
            try
            {
                var sourceTag = context.Tags.FirstOrDefault(x => x.Id == sourceId);
                if (sourceTag == null) throw new Exception("拖动的菜单已被删除，请重新加载页面");
                var targetTag = context.Tags.FirstOrDefault(x => x.Id == targetId);
                if (targetTag == null) throw new Exception("目标菜单已被删除，请重新加载页面");
                if (hitMode.Equals("before", StringComparison.CurrentCultureIgnoreCase))
                {
                    var previousOrderItem =
                        context.Tags.Where(
                            x => x.OrderNumber <= targetTag.OrderNumber &&
                                x.Id != targetId && x.Id != sourceId && x.Type == targetTag.Type)
                            .OrderByDescending(x => x.OrderNumber).FirstOrDefault();
                    sourceTag.OrderNumber = previousOrderItem == null
                        ? targetTag.OrderNumber - 1
                        : targetTag.OrderNumber - (targetTag.OrderNumber - previousOrderItem.OrderNumber) / 2;
                }
                else
                {
                    var nextOrderItem =
                        context.Tags.Where(
                            x => x.OrderNumber >= targetTag.OrderNumber &&
                                x.Id != targetId && x.Id != sourceId && x.Type == targetTag.Type)
                            .OrderBy(x => x.OrderNumber).FirstOrDefault();
                    sourceTag.OrderNumber = nextOrderItem == null
                        ? targetTag.OrderNumber + 1
                        : targetTag.OrderNumber + (nextOrderItem.OrderNumber - targetTag.OrderNumber) / 2;
                }
                context.SaveChanges();

                return Json(new { Result = true });
            }
            catch (Exception exception)
            {
                return Json(new { Result = false, exception.Message });
            }
        }

        public ActionResult SelectUser(Guid tagId)
        {
            var employeeIds = context.EmployeeTags.Where(x => x.TagId == tagId).Select(x => x.EmployeeId).ToList();
            TempData["selectedList"] = employeeIds;
            return RedirectToAction("SelectUser", "User",
                new { allowMulti = true });
        }

        public ActionResult SetTagEmployees(Guid tagId, List<Guid> employeeIds)
        {
            try
            {
                employeeIds = employeeIds ?? new List<Guid>();

                var tag = context.Tags.Find(tagId);
                if (tag == null) throw new Exception("标签不存在，请刷新页面重试。");

                var currentEmployees = context.EmployeeTags.Where(x => x.TagId == tagId);
                if (currentEmployees.Any())
                {
                    context.EmployeeTags.RemoveRange(currentEmployees);
                }
                if (employeeIds.Any())
                {
                    context.EmployeeTags.AddRange(employeeIds.Select(x => new EmployeeTag
                    {
                        Id = Guid.NewGuid(),
                        TagId = tagId,
                        EmployeeId = x
                    }));
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

        public ActionResult SelectTag(bool allowMulti, List<string> selectedList, TagType type = TagType.Default)
        {
            ViewBag.allowMulti = allowMulti;
            ViewBag.Type = type;

            List<Guid> idList;
            // 跳转的Action通过TempData["selectedList"]传递已选项，否则接收到的selectedList参数会为空。
            if (selectedList == null || selectedList.Count == 0)
            {
                idList = TempData["selectedList"] as List<Guid> ?? new List<Guid>();
            }
            else
            {
                idList = selectedList.Select(Guid.Parse).ToList();
            }
            return View(idList);
        }
    }
}
