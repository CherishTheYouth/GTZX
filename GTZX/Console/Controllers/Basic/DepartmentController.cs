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
    public class DepartmentController : Controller
    {
        readonly MyDbContext context = new MyDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetList()
        {
            RemoveUnusedDepartments();
            var list = context.GetSerializedDepartments();
            var result = GetDisplayList(list);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public void RemoveUnusedDepartments()
        {
            var temp = context.Departments.Where(x => x.ParentId.HasValue && !context.Departments.Any(y => y.Id == x.ParentId));
            if (temp.Any())
            {
                context.Departments.RemoveRange(temp);
                context.SaveChanges();
            }
        }

        public IList<object> GetDisplayList(IList<Department> departments)
        {
            var list = new List<object>();
            foreach (var department in departments)
            {
                var children = new List<object>();
                if (department.Children.Any())
                {
                    children.AddRange(GetDisplayList(department.Children));
                }
                list.Add(new
                {
                    key = department.Id,
                    title = department.Name,
                    id = department.Id,
                    icon = "icon-treenode-department",
                    children
                });
            }
            return list;
        }


        public ActionResult Modify(Guid? id, Guid? parentId)
        {
            var department = context.Departments.FirstOrDefault(x => x.Id == id) ?? new Department { ParentId = parentId };
            return View(department);
        }

        public ActionResult SaveDepartment(Department department)
        {
            try
            {
                this.ValidateModel();
                if (department.Id.Equals(Guid.Empty))
                {
                    department.Id = Guid.NewGuid();
                }
                if (!department.OrderNumber.HasValue)
                {
                    var maxOrderItem =
                        context.Departments.Where(x => x.ParentId == department.ParentId)
                            .OrderByDescending(x => x.OrderNumber)
                            .FirstOrDefault();
                    department.OrderNumber = maxOrderItem != null ? maxOrderItem.OrderNumber + 1 : 999;
                }
                context.Departments.AddOrUpdate(department);
                context.SaveChanges();

                return Json(new
                {
                    Result = true,
                    Data =
                        new
                        {
                            title = department.Name,
                            id = department.Id,
                            icon = "icon-treenode-department",
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

        public ActionResult RemoveDepartment(Guid? id)
        {
            try
            {
                var department = context.Departments.FirstOrDefault(x => x.Id == id);
                if (department == null) throw new Exception("要删除的菜单不存在，请重新加载页面");
                department.IsDelete = true;
                context.Departments.AddOrUpdate(department);
                context.SaveChanges();
                RemoveUnusedDepartments();

                return Json(new { Result = true });
            }
            catch (Exception exception)
            {
                return Json(new { Result = false, exception.Message });
            }
        }

        public ActionResult DragDepartment(Guid sourceId, Guid targetId, string hitMode)
        {
            try
            {
                var sourceDepartment = context.Departments.FirstOrDefault(x => x.Id == sourceId);
                if (sourceDepartment == null) throw new Exception("拖动的菜单已被删除，请重新加载页面");
                var targetDepartment = context.Departments.FirstOrDefault(x => x.Id == targetId);
                if (targetDepartment == null) throw new Exception("目标菜单已被删除，请重新加载页面");
                if (hitMode.Equals("over", StringComparison.CurrentCultureIgnoreCase))
                {
                    sourceDepartment.ParentId = targetId;
                    // 获取排序号
                    var maxOrderItem =
                        context.Departments.Where(x => x.ParentId == targetId)
                            .OrderByDescending(x => x.OrderNumber)
                            .FirstOrDefault();
                    sourceDepartment.OrderNumber = maxOrderItem == null ? 999 : maxOrderItem.OrderNumber + 1;
                }
                else
                {
                    sourceDepartment.ParentId = targetDepartment.ParentId;
                    if (hitMode.Equals("before", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var previousOrderItem =
                            context.Departments.Where(
                                x =>
                                    x.ParentId == targetDepartment.ParentId && x.OrderNumber <= targetDepartment.OrderNumber &&
                                    x.Id != targetId && x.Id != sourceId)
                                .OrderByDescending(x => x.OrderNumber).FirstOrDefault();
                        sourceDepartment.OrderNumber = previousOrderItem == null
                            ? targetDepartment.OrderNumber - 1
                            : targetDepartment.OrderNumber - (targetDepartment.OrderNumber - previousOrderItem.OrderNumber) / 2;
                    }
                    else
                    {
                        var nextOrderItem =
                            context.Departments.Where(
                                x =>
                                    x.ParentId == targetDepartment.ParentId && x.OrderNumber >= targetDepartment.OrderNumber &&
                                    x.Id != targetId && x.Id != sourceId)
                                .OrderBy(x => x.OrderNumber).FirstOrDefault();
                        sourceDepartment.OrderNumber = nextOrderItem == null
                            ? targetDepartment.OrderNumber + 1
                            : targetDepartment.OrderNumber + (nextOrderItem.OrderNumber - targetDepartment.OrderNumber) / 2;
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
        /// 通用的部门选择界面
        /// </summary>
        /// <param name="allowMulti">是否允许多选</param>
        /// <param name="selectedList">已选列表</param>
        /// <returns></returns>
        public ActionResult SelectDepartment(bool allowMulti, List<string> selectedList)
        {
            ViewBag.allowMulti = allowMulti;
            int allowMultiNum = 2;
            List<Guid> depIdList;
            // 跳转的Action通过TempData["selectedList"]传递已选项，否则接收到的selectedList参数会为空。
            if (selectedList == null || selectedList.Count == 0)
            {
                depIdList = (TempData["selectedList"] as List<Guid>) ?? new List<Guid>();
            }
            else
            {
                depIdList = selectedList.Select(Guid.Parse).ToList();
            }
            if (!allowMulti)
            {
                allowMultiNum = 1;
            }
            ViewBag.allowMulti = allowMulti;
            ViewBag.allowMultiNum = allowMultiNum;
            ViewBag.selectedList = JsonConvert.SerializeObject(depIdList);

            return View();
        }

        public ActionResult GetSelectList(string selectedList)
        {
            List<string> sel = JsonConvert.DeserializeObject<List<string>>(selectedList);
            List<Guid> selectedListGuid = sel.Select(Guid.Parse).ToList();
            var list = context.GetSerializedDepartments();
            var result = GetSelectDisplayList(list, selectedListGuid);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public IList<object> GetSelectDisplayList(IList<Department> departments, List<Guid> selectedList)
        {
            var list = new List<object>();
            foreach (var department in departments)
            {
                var children = new List<object>();
                if (department.Children.Any())
                {
                    children.AddRange(GetSelectDisplayList(department.Children, selectedList));
                }
                list.Add(new
                {
                    id = department.Id,
                    key = department.Id,
                    title = department.Name,
                    selected = selectedList.Contains(department.Id),
                    icon = "icon-treenode-department",
                    expanded = true,
                    children
                });
            }
            return list;
        }
    }
}
