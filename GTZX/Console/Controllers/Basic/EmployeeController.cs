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
    public class EmployeeController : Controller
    {
        readonly MyDbContext context = new MyDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetList(int page, string fullName)
        {
            IQueryable<Employee> employees =
                context.Employees.Where(x => !x.IsDelete);
            if (!string.IsNullOrWhiteSpace(fullName))
            {
                employees = employees.Where(x => x.FullName.Contains(fullName));
            }
            employees = employees.OrderBy(x => x.FullName);
            var count = employees.Count();
            var list = employees.ToPage(page, count).ToList();
            var departments = context.Departments.ToList();
            var result = list.Select(x => new
            {
                x.Id,
                DepartmentName = (departments.FirstOrDefault(y => y.Id == x.DepartmentId) ?? new Department()).Name,
                x.FullName,
                Gender = x.Gender ? "男" : "女",
                Birthday = x.Birthday.Format("yyyy-MM-dd"),
                x.IdCard,
            });
            return Json(new { Count = count, Data = result });
        }

        public ActionResult Modify(Guid? id)
        {
            var employee = context.Employees.Find(id) ?? new Employee();
            ViewBag.DepartmentName = (context.Departments.FirstOrDefault(y => y.Id == employee.DepartmentId) ?? new Department()).Name;
            return View(employee);
        }

        public ActionResult SelectDepartment(Guid? id)
        {
            var departmentIds = context.Employees.Where(x => x.Id == id).Select(x => x.DepartmentId).ToList();
            TempData["selectedList"] = departmentIds;
            return RedirectToAction("SelectDepartment", "Department",
                new { allowMulti = false });
        }

        public ActionResult SaveEmployee(Employee employee, List<string> listTag)
        {
            try
            {
                this.ValidateModel();

                var newEmployee = context.Employees.Find(employee.Id) ?? new Employee();
                newEmployee.SetValues(employee, ModelState.Keys);

                if (newEmployee.Id.Equals(Guid.Empty))
                {
                    newEmployee.Id = Guid.NewGuid();
                    newEmployee.CreateTime = DateTime.Now;
                }
                context.Employees.AddOrUpdate(newEmployee);

                listTag = listTag ?? new List<string>();
                var currentTags = context.EmployeeTags.Where(x => x.EmployeeId == newEmployee.Id);
                if (currentTags.Any())
                {
                    context.EmployeeTags.RemoveRange(currentTags);
                }
                if (listTag.Any())
                {
                    context.EmployeeTags.AddRange(listTag.Distinct().Select(x => new EmployeeTag
                    {
                        Id = Guid.NewGuid(),
                        TagId = Guid.Parse(x),
                        EmployeeId = newEmployee.Id
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

        public ActionResult RemoveEmployee(Guid? id)
        {
            try
            {
                var employee = context.Employees.Find(id);
                if (employee == null) throw new Exception("要删除的用户不存在，请重新加载页面");
                employee.IsDelete = true;
                context.Employees.AddOrUpdate(employee);
                var user = context.Users.Find(id);
                if (user != null)
                {
                    context.Users.Remove(user);
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
