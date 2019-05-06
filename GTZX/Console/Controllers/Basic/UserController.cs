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
    public class UserController : Controller
    {
        private readonly MyDbContext context = new MyDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetList(int page, string loginName)
        {
            IQueryable<User> users =
                context.Users;
            if (!string.IsNullOrWhiteSpace(loginName))
            {
                users = users.Where(x => x.LoginName.Contains(loginName));
            }
            users = users.OrderBy(x => x.CreateTime);
            var count = users.Count();
            var list = users.ToPage(page, count).ToList();

            var employees = context.Employees.ToList();
            var result = list.Select(x => new
            {
                x.Id,
                x.LoginName,
                (employees.FirstOrDefault(y => y.Id == x.Id) ?? new Employee()).FullName,
                State = x.IsEnable ? "启用" : "禁用"
            });

            return Json(new { Count = count, Data = result });
        }

        public ActionResult Modify(Guid? id)
        {
            var user = context.Users.Find(id) ?? new User();
            return View(user);
        }

        public ActionResult SaveUser(User user, List<string> listRole)
        {
            try
            {
                this.ValidateModel();
                var newUser = context.Users.Find(user.Id) ?? new User();
                var changeInfoList = user.CompareDifference(newUser, ModelState.Keys);

                newUser.SetValues(user, ModelState.Keys);

                if (newUser.Id.Equals(Guid.Empty))
                {
                    newUser.Id = Guid.NewGuid();
                }
                if (newUser.CreateTime.AddYears(1) < DateTime.Now)
                {
                    newUser.CreateTime = DateTime.Now;
                }

                // 验证登录名是否可用
                if (context.Users.Any(x => x.LoginName == newUser.LoginName && x.Id != newUser.Id))
                {
                    throw new Exception("登录名已被占用！");
                }
                // 验证雇员是否存在
                if (!context.Employees.Any(x => x.Id == newUser.Id))
                {
                    throw new Exception("无法创建登录帐号：请选择该登录帐号对应的人员。");
                }

                context.Users.AddOrUpdate(newUser);

                listRole = listRole ?? new List<string>();
                var currentRoles = context.UserRoles.Where(x => x.UserId == newUser.Id);
                if (currentRoles.Any())
                {
                    context.UserRoles.RemoveRange(currentRoles);
                }
                if (listRole.Any())
                {
                    context.UserRoles.AddRange(listRole.Distinct().Select(x => new UserRole
                    {
                        Id = Guid.NewGuid(),
                        RoleId = Guid.Parse(x),
                        UserId = newUser.Id
                    }));
                }

                context.WriteLog(new Log
                {
                    Content = "编辑用户",
                    TargetId = newUser.Id,
                    UserId = CacheUtil.LoginUser.Id
                }, changeInfoList);

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

        public ActionResult RomoveUser(Guid id)
        {
            try
            {
                var user = context.Users.Find(id);
                if (user == null) throw new Exception("要删除的用户不存在，请重新加载页面");
                context.Users.Remove(user);
                context.SaveChanges();

                return Json(new { Result = true });
            }
            catch (Exception exception)
            {
                return Json(new { Result = false, exception.Message });
            }
        }

        /// <summary>
        /// 通用的用户选择界面
        /// </summary>
        /// <param name="allowMulti">是否允许多选</param>
        /// <param name="selectedList">已选项</param>
        /// <param name="validateUserExist">是否依据登录帐号筛选，true表示只能选取有登录账号的用户，false表示只能选取没登录帐号的用户,null则不限。</param>
        /// <returns></returns>
        public ActionResult SelectUser(bool allowMulti, List<string> selectedList, bool? validateUserExist)
        {
            ViewBag.allowMulti = allowMulti;

            List<Guid> idList;
            // 跳转的Action通过TempData["selectedList"]传递已选项，否则接收到的selectedList参数会为空。
            if (selectedList == null || selectedList.Count == 0)
            {
                idList = (TempData["selectedList"] as List<Guid>) ?? new List<Guid>();
            }
            else
            {
                idList = selectedList.Select(Guid.Parse).ToList();
            }

            var allEmployees = context.Employees.ToList();
            var allUsers = context.Users.ToList();
            if (validateUserExist.HasValue)
            {
                allEmployees = validateUserExist.Value
                    ? allEmployees.Where(
                        x => allUsers.Any(y => y.Id.Equals(x.Id))).ToList()
                    : allEmployees.Where(
                        x => !allUsers.Any(y => y.Id.Equals(x.Id))).ToList();
            }
            ViewBag.allEmployees = allEmployees;
            return View(idList);
        }

        public ActionResult GetDepartmentUsers(Guid? departmentId, bool? validateUserExist)
        {
            var employees = new List<Employee>();
            var allDepartments = context.Departments.ToList();
            var allEmployees = context.Employees.ToList();
            var allUsers = context.Users.ToList();
            if (departmentId.HasValue)
            {
                var department = context.Departments.Find(departmentId.Value);
                if (department != null)
                {
                    FeatchDepartmentUsers(department, employees, validateUserExist, allDepartments, allEmployees,
                        allUsers);
                }
            }
            else
            {
                var departments = context.Departments.Where(x => !x.ParentId.HasValue).ToList();
                foreach (var department in departments)
                {
                    FeatchDepartmentUsers(department, employees, validateUserExist, allDepartments, allEmployees,
                        allUsers);
                }
            }
            return Json(employees, JsonRequestBehavior.AllowGet);
        }

        private void FeatchDepartmentUsers(Department department, List<Employee> employees,
            bool? validateUserExist, List<Department> allDepartments, List<Employee> allEmployees, List<User> allUsers)
        {
            if (validateUserExist.HasValue)
            {
                employees.AddRange(validateUserExist.Value
                    ? allEmployees.Where(
                        x => x.DepartmentId.Equals(department.Id) && allUsers.Any(y => y.Id.Equals(x.Id)))
                    : allEmployees.Where(
                        x => x.DepartmentId.Equals(department.Id) && !allUsers.Any(y => y.Id.Equals(x.Id))));
            }
            else
            {
                employees.AddRange(allEmployees.Where(x => x.DepartmentId.Equals(department.Id)));
            }
            var childrenDepartments = allDepartments.Where(x => x.ParentId == department.Id).ToList();
            if (!childrenDepartments.Any()) return;
            foreach (var childDepartment in childrenDepartments)
            {
                FeatchDepartmentUsers(childDepartment, employees, validateUserExist, allDepartments, allEmployees, allUsers);
            }
        }

        #region 注册用户
        [AuthEscape]
        public ActionResult Register()
        {
            return View();
        }
        #endregion
    }
}
