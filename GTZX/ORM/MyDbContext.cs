using System.Data.Entity;
using Modules;
using Modules.Biz;

namespace ORM
{
    public class MyDbContext : DbContext
    {
        public MyDbContext()
            : base(GetConnectionNameOrString()) { }

        private static string GetConnectionNameOrString()
        {
            // 这里可以设置ef的连接字符串（包括对加密的连接字符串进行解密）
            return "SqlServerConnectionString";
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // 如果使用的不是Oracle，禁用以下代码
            /*
            var defaultSchema =
                System.Configuration.ConfigurationManager.AppSettings["DefaultSchema"];
            if (string.IsNullOrEmpty(defaultSchema))
            {
                defaultSchema = "";
            }
            modelBuilder.HasDefaultSchema(defaultSchema);
            */
        }

        #region 系统表

        /// <summary>
        /// 部门
        /// </summary>
        public DbSet<Department> Departments { get; set; }

        /// <summary>
        /// 员工表
        /// </summary>
        public DbSet<Employee> Employees { get; set; }

        /// <summary>
        /// 功能
        /// </summary>
        public DbSet<Func> Funcs { get; set; }

        /// <summary>
        /// 菜单
        /// </summary>
        public DbSet<Menu> Menus { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public DbSet<Role> Roles { get; set; }

        /// <summary>
        /// 角色功能关系
        /// </summary>
        public DbSet<RoleFunc> RoleFuncs { get; set; }

        /// <summary>
        /// 角色菜单关系
        /// </summary>
        public DbSet<RoleMenu> RoleMenus { get; set; }

        /// <summary>
        /// 上传文件
        /// </summary>
        public DbSet<UploadFile> UploadFiles { get; set; }

        /// <summary>
        /// 上传文件关联
        /// </summary>
        public DbSet<UploadFileRelation> UploadFileRelations { get; set; }

        /// <summary>
        /// 登录用户
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// 用户角色关系
        /// </summary>
        public DbSet<UserRole> UserRoles { get; set; }

        /// <summary>
        /// 字典项
        /// </summary>
        public DbSet<DicItem> DicItems { get; set; }

        /// <summary>
        /// 日志
        /// </summary>
        public DbSet<Log> Logs { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public DbSet<Tag> Tags { get; set; }

        /// <summary>
        /// 人员标签
        /// </summary>
        public DbSet<EmployeeTag> EmployeeTags { get; set; }

        #endregion

        #region 业务表

        /// <summary>
        /// 政策法规
        /// </summary>
        public DbSet<Regulation> Regulations { get; set; }


        public DbSet<RegulationSourceFile> RegulationSourceFiles { get; set; }
        #endregion


    }
}
