using System;
using System.Linq;
using System.Web.Mvc;
using Helper;
using Modules;
using ORM;

namespace Console.Controllers
{
    public class MainController : Controller
    {
        readonly MyDbContext context = new MyDbContext();
        private const string AdminLoginNameKey = "AdminLoginName";
        private const string AdminPasswordKey = "AdminPassword";

        public ActionResult Index()
        {
            return View();
        }

        [AuthEscape]
        public ActionResult Login()
        {
            WebCache.ClearCache();
            return View();
        }

        [AuthEscape]
        public ActionResult ValidateLogin(string userName, string password)
        {
            try
            {
                var adminLoginName = ConfigurationHelper.GetAppSetting(AdminLoginNameKey);
                var adminPassword = ConfigurationHelper.GetAppSetting(AdminPasswordKey);
                User user;
                if (userName.Equals(adminLoginName) && password.Equals(adminPassword))
                {
                    user = new User
                    {
                        LoginName = userName,
                        Password = password,
                        IsAdmin = true
                    };
                }
                else
                {
                    var employee = context.Employees.FirstOrDefault(x => x.FullName == userName);
                    user = employee == null ? context.Users.FirstOrDefault(x => x.LoginName == userName) : context.Users.Find(employee.Id);
                    if (user == null || !user.Password.Equals(password))
                        throw new Exception("用户名或密码错误！");
                    if (!user.IsEnable)
                        throw new Exception("用户已被禁止登录，更多信息请联系系统管理员！");
                }

                CacheUtil.LoginUser = user;
                context.WriteLog(new Log
                {
                    Content = "登录后台管理系统",
                    UserId = user.Id
                });
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

        public ActionResult FindMenu(Guid id)
        {
            return Json(CacheUtil.SerializedLimitedMenus.FirstOrDefault(x => x.Id == id), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        public ActionResult SaveNewPassword(string oldPassword, string newPassword)
        {
            try
            {
                if (CacheUtil.LoginUser.IsAdmin)
                {
                    throw new Exception("管理员账号不可修改密码！");
                }
                var user = context.Users.Find(CacheUtil.LoginUser.Id);
                if (user == null) throw new Exception("用户不存在！");
                if (!user.Password.Equals(oldPassword)) throw new Exception("旧密码不正确！");
                user.Password = newPassword;
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
