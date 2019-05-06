using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using Modules;
using ORM;

namespace Console.Controllers
{
    public class MenuController : Controller
    {
        readonly MyDbContext context = new MyDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetList()
        {
            RemoveUnusedMenus();
            var list = context.GetSerializedMenus();
            var result = GetDisplayList(list);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public void RemoveUnusedMenus()
        {
            var temp = context.Menus.Where(x => x.ParentId.HasValue && !context.Menus.Any(y => y.Id == x.ParentId));
            if (temp.Any())
            {
                context.Menus.RemoveRange(temp);
                context.SaveChanges();
            }
        }

        public IList<object> GetDisplayList(IList<Menu> menus)
        {
            var list = new List<object>();
            foreach (var menu in menus)
            {
                var children = new List<object>();
                if (menu.Children.Any())
                {
                    children.AddRange(GetDisplayList(menu.Children));
                }
                list.Add(new
                {
                    key = menu.Id,
                    title = menu.Name,
                    id = menu.Id,
                    icon = "icon-treenode-menu",
                    menuIcon = menu.IconClass,
                    url = menu.Url,
                    children
                });
            }
            return list;
        }

        public ActionResult Modify(Guid? id, Guid? parentId)
        {
            var menu = context.Menus.FirstOrDefault(x => x.Id == id) ?? new Menu { ParentId = parentId };
            return View(menu);
        }

        public ActionResult SaveMenu(Menu menu)
        {
            try
            {
                this.ValidateModel();
                if (menu.Id.Equals(Guid.Empty))
                {
                    menu.Id = Guid.NewGuid();
                }
                if (!menu.OrderNumber.HasValue)
                {
                    var maxOrderItem =
                        context.Menus.Where(x => x.ParentId == menu.ParentId)
                            .OrderByDescending(x => x.OrderNumber)
                            .FirstOrDefault();
                    menu.OrderNumber = maxOrderItem != null ? maxOrderItem.OrderNumber + 1 : 999;
                }
                context.Menus.AddOrUpdate(menu);
                context.SaveChanges();

                return Json(new
                {
                    Result = true,
                    Data =
                        new
                        {
                            title = menu.Name,
                            id = menu.Id,
                            icon = "icon-treenode-menu",
                            menuIcon = menu.IconClass,
                            url = menu.Url
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

        public ActionResult RemoveMenu(Guid id)
        {
            try
            {
                var menu = context.Menus.FirstOrDefault(x => x.Id == id);
                if (menu == null) throw new Exception("要删除的菜单不存在，请重新加载页面");
                context.Menus.Remove(menu);
                context.SaveChanges();
                RemoveUnusedMenus();

                return Json(new { Result = true });
            }
            catch (Exception exception)
            {
                return Json(new { Result = false, exception.Message });
            }
        }

        public ActionResult DragMenu(Guid sourceId, Guid targetId, string hitMode)
        {
            try
            {
                var sourceMenu = context.Menus.FirstOrDefault(x => x.Id == sourceId);
                if (sourceMenu == null) throw new Exception("拖动的菜单已被删除，请重新加载页面");
                var targetMenu = context.Menus.FirstOrDefault(x => x.Id == targetId);
                if (targetMenu == null) throw new Exception("目标菜单已被删除，请重新加载页面");
                if (hitMode.Equals("over", StringComparison.CurrentCultureIgnoreCase))
                {
                    sourceMenu.ParentId = targetId;
                    // 获取排序号
                    var maxOrderItem =
                        context.Menus.Where(x => x.ParentId == targetId)
                            .OrderByDescending(x => x.OrderNumber)
                            .FirstOrDefault();
                    sourceMenu.OrderNumber = maxOrderItem == null ? 999 : maxOrderItem.OrderNumber + 1;
                }
                else
                {
                    sourceMenu.ParentId = targetMenu.ParentId;
                    if (hitMode.Equals("before", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var previousOrderItem =
                            context.Menus.Where(
                                x =>
                                    x.ParentId == targetMenu.ParentId && x.OrderNumber <= targetMenu.OrderNumber &&
                                    x.Id != targetId && x.Id != sourceId)
                                .OrderByDescending(x => x.OrderNumber).FirstOrDefault();
                        sourceMenu.OrderNumber = previousOrderItem == null
                            ? targetMenu.OrderNumber - 1
                            : targetMenu.OrderNumber - (targetMenu.OrderNumber - previousOrderItem.OrderNumber) / 2;
                    }
                    else
                    {
                        var nextOrderItem =
                            context.Menus.Where(
                                x =>
                                    x.ParentId == targetMenu.ParentId && x.OrderNumber >= targetMenu.OrderNumber &&
                                    x.Id != targetId && x.Id != sourceId)
                                .OrderBy(x => x.OrderNumber).FirstOrDefault();
                        sourceMenu.OrderNumber = nextOrderItem == null
                            ? targetMenu.OrderNumber + 1
                            : targetMenu.OrderNumber + (nextOrderItem.OrderNumber - targetMenu.OrderNumber) / 2;
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

        public ActionResult Empty()
        {
            return View();
        }

        public ActionResult Forbidden()
        {
            return View();
        }
    }
}
