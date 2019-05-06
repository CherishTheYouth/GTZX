namespace ORM.Migrations
{
    using Modules;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<ORM.MyDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "ORM.MyDbContext";
        }

        /// <summary>
        /// 添加种子数据
        /// </summary>
        /// <param name="context"></param>
        protected override void Seed(ORM.MyDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            var pId = Guid.NewGuid();
            int orderNumber = 999;
            var sysMenuSet = new Menu() {
                Id = pId,
                ParentId = null,
                Name = "系统设置",
                Url = null,
                IconClass = null,
                OrderNumber = orderNumber
            };
            var menuSet = new Menu() {
                Id = Guid.NewGuid(),
                ParentId = pId,
                Name = "菜单设置",
                Url = "Menu/Index",
                IconClass = null,
                OrderNumber = orderNumber
            };
            var dicitemSet = new Menu()
            {
                Id = Guid.NewGuid(),
                ParentId = pId,
                Name = "字典设置",
                Url = "Dic/Index",
                IconClass = null,
                OrderNumber = orderNumber
            };
            var userSet = new Menu()
            {
                Id = Guid.NewGuid(),
                ParentId = pId,
                Name = "账户设置",
                Url = "User/Index",
                IconClass = null,
                OrderNumber = orderNumber
            };
            var empolyeeSet = new Menu()
            {
                Id = Guid.NewGuid(),
                ParentId = pId,
                Name = "用户管理",
                Url = "Employee/Index",
                IconClass = null,
                OrderNumber = orderNumber
            };
            var roleSet = new Menu()
            {
                Id = Guid.NewGuid(),
                ParentId = pId,
                Name = "角色管理",
                Url = "Role/Index",
                IconClass = null,
                OrderNumber = orderNumber
            };
            var departmentSet = new Menu()
            {
                Id = Guid.NewGuid(),
                ParentId = pId,
                Name = "部门管理",
                Url = "Department/Index",
                IconClass = null,
                OrderNumber = orderNumber
            };
            context.Menus.Add(sysMenuSet);
            context.Menus.Add(menuSet);
            context.Menus.Add(dicitemSet);
            context.Menus.Add(roleSet);
            context.Menus.Add(userSet);
            context.Menus.Add(empolyeeSet);
            context.Menus.Add(departmentSet);

            base.Seed(context);
        }
    }
}
