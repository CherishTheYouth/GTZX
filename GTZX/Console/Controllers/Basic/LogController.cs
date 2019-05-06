using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Helper.Extension;
using Modules;
using ORM;
using System.ComponentModel;

namespace Console.Controllers
{
    public class LogController : Controller
    {
        readonly MyDbContext context = new MyDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetList(int page, string keyword, DateTime? startDate, DateTime? endDate)
        {
            IQueryable<Log> logs =
                    context.Logs;
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                logs = logs.Where(x => x.Content.Contains(keyword)||x.Detail.Contains(keyword));
            }
            if (startDate.HasValue)
            {
                var sDate = startDate.Value.Date;
                logs = logs.Where(x => x.CreateTime >= sDate);
            }
            if (endDate.HasValue)
            {
                var eDate = endDate.Value.Date.AddDays(1);
                logs = logs.Where(x => x.CreateTime < eDate);
            }
            logs = logs.OrderByDescending(x => x.CreateTime);
            var count = logs.Count();
            var list = logs.ToPage(page, count).ToList();
            var employees = context.Employees.ToList();

            var result = list.Select(x => new
            {
                x.Id,
                x.Content,
                UserName =
                    x.UserId.Equals(Guid.Empty)
                        ? "管理员"
                        : (employees.FirstOrDefault(y => y.Id == x.UserId) ?? new Employee()).FullName,
                x.CreateTime,
                Type = typeof(LogType).GetEnumDescription(x.Type.ToString())
            });

            return new DateFormatedJson
            {
                DateTimeFormat = "yyyy年MM月dd日 HH:mm:ss",
                Data = new { Count = count, Data = result }
            };
        }

        public ActionResult Detail(Guid id)
        {
            var log = context.Logs.Find(id);
            if (log == null) return Content("日志不存在！");
            return View(log);
        }
    }
}
