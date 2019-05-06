using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using Helper.Extension;
using Modules;
using Newtonsoft.Json;
using ORM;

namespace Console.Controllers
{
    public class RoleController : Controller
    {
        readonly MyDbContext context = new MyDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetList(int page, string name)
        {
            IQueryable<Role> roles =
                    context.Roles;
            if (!string.IsNullOrWhiteSpace(name))
            {
                roles = roles.Where(x => x.Name.Contains(name));
            }
            roles = roles.OrderBy(x => x.CreateTime);
            var count = roles.Count();
            var list = roles.ToPage(page, count).ToList();

            return Json(new { Count = count, Data = list });
        }

        public ActionResult GetMenuList(Guid roleId)
        {
            var selectedIds = new List<Guid>();
            selectedIds.AddRange(context.RoleMenus.Where(x => x.RoleId == roleId).Select(x => x.MenuId));
            var list = context.GetSerializedMenus();
            var result = GetDisplayMenuList(list, selectedIds);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public IList<object> GetDisplayMenuList(IList<Menu> menus, List<Guid> selectedIds)
        {
            var list = new List<object>();
            foreach (var menu in menus)
            {
                var children = new List<object>();
                if (menu.Children.Any())
                {
                    children.AddRange(GetDisplayMenuList(menu.Children, selectedIds));
                }
                list.Add(new
                {
                    key = menu.Id,
                    title = menu.Name,
                    id = menu.Id,
                    icon = "icon-treenode-menu",
                    children,
                    selected = selectedIds.Contains(menu.Id),
                    expanded = true
                });
            }
            return list;
        }

        public ActionResult GetFuncList(Guid roleId)
        {
            var selectedIds = new List<Guid>();
            selectedIds.AddRange(context.RoleFuncs.Where(x => x.RoleId == roleId).Select(x => x.FuncId));
            var list = context.GetSerializedFuncs();
            var result = GetDisplayFuncList(list, selectedIds);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public IList<object> GetDisplayFuncList(IList<Func> funcs, List<Guid> selectedIds)
        {
            var list = new List<object>();
            foreach (var func in funcs)
            {
                var children = new List<object>();
                if (func.Children.Any())
                {
                    children.AddRange(GetDisplayFuncList(func.Children, selectedIds));
                }
                list.Add(new
                {
                    key = func.Id,
                    title = func.Name,
                    id = func.Id,
                    icon = "icon-treenode-func",
                    children,
                    selected = selectedIds.Contains(func.Id),
                    expanded = true
                });
            }
            return list;
        }

        public ActionResult Modify(Guid? id)
        {
            var role = context.Roles.FirstOrDefault(x => x.Id == id) ?? new Role();
            return View(role);
        }

        public ActionResult SaveRole(Role role, string selectedMenuIds, string selectedFuncIds)
        {
            try
            {
                this.ValidateModel();
                var newRole = context.Roles.Find(role.Id) ?? new Role();
                newRole.SetValues(role, ModelState.Keys);

                if (newRole.Id.Equals(Guid.Empty))
                {
                    newRole.Id = Guid.NewGuid();
                    newRole.CreateTime = DateTime.Now;
                }
                context.Roles.AddOrUpdate(newRole);

                var menuIdArray = JsonConvert.DeserializeObject<List<Guid>>(selectedMenuIds);
                var funcIdArray = JsonConvert.DeserializeObject<List<Guid>>(selectedFuncIds);
                var currentMenus = context.RoleMenus.Where(x => x.RoleId == newRole.Id);
                if (currentMenus.Any())
                {
                    context.RoleMenus.RemoveRange(currentMenus);
                }
                if (menuIdArray.Any())
                {
                    context.RoleMenus.AddRange(menuIdArray.Select(x => new RoleMenu
                    {
                        Id = Guid.NewGuid(),
                        RoleId = newRole.Id,
                        MenuId = x
                    }));
                }

                var currentFuncs = context.RoleFuncs.Where(x => x.RoleId == newRole.Id);
                if (currentFuncs.Any())
                {
                    context.RoleFuncs.RemoveRange(currentFuncs);
                }
                if (funcIdArray.Any())
                {
                    context.RoleFuncs.AddRange(funcIdArray.Select(x => new RoleFunc
                    {
                        Id = Guid.NewGuid(),
                        RoleId = newRole.Id,
                        FuncId = x
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

        public ActionResult RemoveRole(Guid id)
        {
            try
            {
                var role = context.Roles.FirstOrDefault(x => x.Id == id);
                if (role == null) throw new Exception("要删除的角色不存在，请重新加载页面");
                context.Roles.Remove(role);
                context.SaveChanges();

                return Json(new { Result = true });
            }
            catch (Exception exception)
            {
                return Json(new { Result = false, exception.Message });
            }
        }

        public ActionResult SelectUser(Guid roleId)
        {
            var userIds = context.UserRoles.Where(x => x.RoleId == roleId).Select(x => x.UserId).ToList();
            TempData["selectedList"] = userIds;
            return RedirectToAction("SelectUser", "User",
                new { allowMulti = true, validateUserExist = true });
        }

        public ActionResult SetRoleUsers(Guid roleId, List<Guid> userIds)
        {
            try
            {
                userIds = userIds ?? new List<Guid>();

                var role = context.Roles.Find(roleId);
                if (role == null) throw new Exception("角色不存在，请刷新页面重试。");

                var currentUsers = context.UserRoles.Where(x => x.RoleId == roleId);
                if (currentUsers.Any())
                {
                    context.UserRoles.RemoveRange(currentUsers);
                }
                if (userIds.Any())
                {
                    context.UserRoles.AddRange(userIds.Select(x => new UserRole
                    {
                        Id = Guid.NewGuid(),
                        RoleId = roleId,
                        UserId = x
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
    }
}
