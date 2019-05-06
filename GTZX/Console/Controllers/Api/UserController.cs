using Helper;
using Modules;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace Console.Controllers.Api
{

    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {

        readonly MyDbContext context = new MyDbContext();
        private const string AdminLoginNameKey = "AdminLoginName";
        private const string AdminPasswordKey = "AdminPassword";

        //登录校验
        [HttpGet]
        [Route("GetUser/{userName}/{password}")]
        public IHttpActionResult ValidateLogin(string userName, string password)
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
                    Content = "登录手机app",
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


        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}