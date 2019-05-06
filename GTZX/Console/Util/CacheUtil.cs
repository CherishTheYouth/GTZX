using System.Collections.Generic;
using System.Linq;
using Modules;
using ORM;

namespace Console
{
    public static class CacheUtil
    {
        private const string LoginUserKey = "CacheKey-LoginUserCacheKey";
        private const string SerializedLimitedMenusKey = "CacheKey-SerializedLimitedMenusKey";
        private const string AllFuncsKey = "CacheKey-AllFuncsKey";
        private const string LimitedFuncsKey = "CacheKey-LimitedFuncsKey";


        /// <summary>
        /// 获取或设置当前登录用户
        /// </summary>
        public static User LoginUser
        {
            get { return WebCache.GetCache(LoginUserKey) as User; }
            set { WebCache.SetCache(LoginUserKey, value); }
        }

        /// <summary>
        /// 获取用户是否登录的状态
        /// </summary>
        public static bool IsLogin
        {
            get { return LoginUser != null; }
        }

        /// <summary>
        /// 获取有权限的菜单
        /// </summary>
        private static IList<Menu> GetLimitedMenus()
        {
            var isAdmin = LoginUser.IsAdmin;
            using (var context = new MyDbContext())
            {
                IQueryable<Menu> menus = context.Menus;
                if (isAdmin) return menus.OrderByDescending(x => x.OrderNumber).ToList();
                var menuIds =
                    (from ur in context.UserRoles
                        join rm in context.RoleMenus on ur.RoleId equals rm.RoleId
                        where ur.UserId == LoginUser.Id
                        select rm.MenuId);
                menus = menus.Where(x => menuIds.Contains(x.Id));
                return menus.OrderByDescending(x => x.OrderNumber).ToList();
            }
        }

        private static IList<Menu> GetSerializedLimitedMenus()
        {
            var list = new List<Menu>();
            var limitedMenus = GetLimitedMenus();
            for (var i = limitedMenus.Count - 1; i >= 0; i--)
            {
                if (limitedMenus[i].ParentId.HasValue) continue;
                list.Add(limitedMenus[i]);
                limitedMenus.RemoveAt(i);
            }
            foreach (var item in list)
            {
                FeatchChildren(item, limitedMenus);
            }
            return list;
        }

        private static void FeatchChildren(Menu menu, IList<Menu> menus)
        {
            if (!menus.Any()) return;
            for (var i = menus.Count - 1; i >= 0; i--)
            {
                if (!menus[i].ParentId.Equals(menu.Id)) continue;
                menu.Children.Add(menus[i]);
                menus.RemoveAt(i);
            }
            if (!menu.Children.Any()) return;
            foreach (var child in menu.Children)
            {
                FeatchChildren(child, menus);
            }
        }

        /// <summary>
        /// 获取经过序列化的有权限的菜单
        /// </summary>
        public static IList<Menu> SerializedLimitedMenus
        {
            get
            {
                var serializedLimitedMenus = WebCache.GetCache(SerializedLimitedMenusKey) as IList<Menu>;
                if (serializedLimitedMenus != null) return serializedLimitedMenus;
                serializedLimitedMenus = GetSerializedLimitedMenus();
                WebCache.SetCache(SerializedLimitedMenusKey, serializedLimitedMenus);
                return serializedLimitedMenus;
            }
        }

        /// <summary>
        /// 获取拥有权限的菜单上的所有功能
        /// </summary>
        public static IList<Func> AllFuncs
        {
            get
            {
                var allFuncs = WebCache.GetCache(AllFuncsKey) as IList<Func>;
                if (allFuncs != null) return allFuncs;
                using (var context = new MyDbContext())
                {
                    allFuncs = context.Funcs.ToList().Where(x => !string.IsNullOrWhiteSpace(x.FuncCode)).ToList();
                    WebCache.SetCache(AllFuncsKey, allFuncs);
                }
                return allFuncs;
            }
        }

        /// <summary>
        /// 拥有权限的功能列表
        /// </summary>
        public static IList<Func> LimitedFuncs
        {
            get
            { 
                var limitedFuncs = WebCache.GetCache(LimitedFuncsKey) as IList<Func>;
                if (limitedFuncs != null) return limitedFuncs;
                var isAdmin = LoginUser.IsAdmin;
                using (var context = new MyDbContext())
                {
                    IQueryable<Func> funcs = context.Funcs;
                    if (!isAdmin)
                    {
                        var funcIds =
                            (from ur in context.UserRoles
                                join rm in context.RoleFuncs on ur.RoleId equals rm.RoleId
                                where ur.UserId == LoginUser.Id
                                select rm.FuncId);
                        funcs = funcs.Where(x => funcIds.Contains(x.Id));
                    }

                    limitedFuncs = funcs.ToList();
                }
                WebCache.SetCache(LimitedFuncsKey, limitedFuncs);
                return limitedFuncs;
            }
        }

        public static IList<Func> GetForbiddenFuncs()
        {
            return LoginUser == null
                ? AllFuncs
                : AllFuncs.Where(x => LimitedFuncs.All(y => y.Id != x.Id)).ToList();
        }
    }
}