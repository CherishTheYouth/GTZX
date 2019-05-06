using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Helper.Extension;
using Modules;
using Newtonsoft.Json;

namespace ORM
{
    /// <summary>
    /// 上下文扩展类，提供常用的数据库操作
    /// </summary>
    public static class ContextExtension
    {
        #region 上传文件相关

        /// <summary>
        /// 处理上传文件关系表
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="targetId">关联的对象Id</param>
        /// <param name="uploadFileIds">关联的上传文件Id列表</param>
        /// <param name="group">分组，默认为null</param>
        public static void HandleUploadFileRelation(this MyDbContext context, Guid targetId, IList<Guid> uploadFileIds, string group = null)
        {
            uploadFileIds = uploadFileIds ?? new Collection<Guid>();
            var currentItems = context.UploadFileRelations.Where(x => x.TargetId == targetId);
            if (!string.IsNullOrEmpty(group))
            {
                currentItems = currentItems.Where(x => x.Group == group);
            }
            if (currentItems.Any())
            {
                context.UploadFileRelations.RemoveRange(currentItems);
            }
            if (uploadFileIds.Any())
            {
                context.UploadFileRelations.AddRange(uploadFileIds.Distinct().Select(x => new UploadFileRelation
                {
                    Id = Guid.NewGuid(),
                    TargetId = targetId,
                    UploadFileId = x,
                    Group = group
                }));
            }
        }

        /// <summary>
        /// 处理上传文件关系表
        /// </summary>
        /// <param name="context"></param>
        /// <param name="targetId">关联的对象Id</param>
        /// <param name="uploadFileIds">关联的上传文件Id列表</param>
        /// <param name="group">分组，默认为null</param>
        public static void HandleUploadFileRelation(this MyDbContext context, Guid targetId, IList<string> uploadFileIds, string group = null)
        {
            uploadFileIds = uploadFileIds ?? new Collection<string>();
            context.HandleUploadFileRelation(targetId, uploadFileIds.Select(Guid.Parse).ToList(), group);
        }

        /// <summary>
        /// 根据关联的对象Id获取上传文件列表
        /// </summary>
        /// <param name="context"></param>
        /// <param name="targetId">关联的对象Id</param>
        /// <param name="group">分组，默认为null</param>
        /// <returns></returns>
        public static List<UploadFile> GetUploadFilesByTargetId(this MyDbContext context, Guid targetId, string group = null)
        {
            return (from ufr in context.UploadFileRelations
                    join uf in context.UploadFiles
                    on ufr.UploadFileId equals uf.Id
                    where ufr.TargetId == targetId && ufr.Group == @group
                    orderby uf.UploadTime
                    select uf).ToList();
        }

        #endregion

        #region 权限相关

        public static IList<Menu> GetSerializedMenus(this MyDbContext context)
        {
            var menus = context.Menus.OrderByDescending(x => x.OrderNumber).ToList();
            var list = new List<Menu>();
            for (var i = menus.Count - 1; i >= 0; i--)
            {
                if (menus[i].ParentId.HasValue) continue;
                list.Add(menus[i]);
                menus.RemoveAt(i);
            }
            foreach (var item in list)
            {
                FeatchMenuChildren(item, menus);
            }
            return list;
        }

        private static void FeatchMenuChildren(Menu menu, IList<Menu> menus)
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
                FeatchMenuChildren(child, menus);
            }
        }

        public static IList<Func> GetSerializedFuncs(this MyDbContext context)
        {
            var funcs = context.Funcs.OrderByDescending(x => x.OrderNumber).ToList();
            var list = new List<Func>();
            for (var i = funcs.Count - 1; i >= 0; i--)
            {
                if (funcs[i].ParentId.HasValue) continue;
                list.Add(funcs[i]);
                funcs.RemoveAt(i);
            }
            foreach (var item in list)
            {
                FeatchFuncChildren(item, funcs);
            }
            return list;
        }

        private static void FeatchFuncChildren(Func func, IList<Func> funcs)
        {
            if (!funcs.Any()) return;
            for (var i = funcs.Count - 1; i >= 0; i--)
            {
                if (!funcs[i].ParentId.Equals(func.Id)) continue;
                func.Children.Add(funcs[i]);
                funcs.RemoveAt(i);
            }
            if (!func.Children.Any()) return;
            foreach (var child in func.Children)
            {
                FeatchFuncChildren(child, funcs);
            }
        }

        #endregion

        #region 部门

        public static IList<Department> GetSerializedDepartments(this MyDbContext context, bool isDelete = false)
        {
            var departments = !isDelete ? context.Departments.Where(x => !x.IsDelete).OrderByDescending(x => x.OrderNumber).ToList() : context.Departments.Where(x => x.IsDelete == false).OrderByDescending(x => x.OrderNumber).ToList();

            var list = new List<Department>();
            for (var i = departments.Count - 1; i >= 0; i--)
            {
                if (departments[i].ParentId.HasValue) continue;
                list.Add(departments[i]);
                departments.RemoveAt(i);
            }
            foreach (var item in list)
            {
                FeatchDepartmentChildren(item, departments);
            }
            return list;
        }

        private static void FeatchDepartmentChildren(Department department, IList<Department> departments)
        {
            if (!departments.Any()) return;
            for (var i = departments.Count - 1; i >= 0; i--)
            {
                if (!departments[i].ParentId.Equals(department.Id)) continue;
                department.Children.Add(departments[i]);
                departments.RemoveAt(i);
            }
            if (!department.Children.Any()) return;
            foreach (var child in department.Children)
            {
                FeatchDepartmentChildren(child, departments);
            }
        }


        public static List<Department> GetChildrenDepartmentsByUserId(this MyDbContext context, Guid userId)
        {
            var employee = context.Employees.Find(userId);
            return employee == null ? new List<Department>() : context.GetChildrenDepartments(employee.DepartmentId);
        }

        public static List<Department> GetChildrenDepartments(this MyDbContext context, Guid id, bool includingSelf = true)
        {
            var list = new List<Department>();
            var department = context.Departments.Find(id);
            if (department != null)
            {
                if(includingSelf) list.Add(department);
                var departments = context.Departments.OrderBy(x => x.OrderNumber).ToList();
                FeatchChildrenDepatments(list, departments, department);
            }
            return list;
        }

        private static void FeatchChildrenDepatments(List<Department> list, List<Department> departments,
            Department department)
        {
            var childrenList = departments.Where(x => x.ParentId == department.Id).ToList();
            if (!childrenList.Any()) return;
            list.AddRange(childrenList);
            foreach (var child in childrenList)
            {
                FeatchChildrenDepatments(list, departments, child);
            }
        }

        #endregion

        #region 字典

        public static IList<DicItem> GetSerializedDicItems(this MyDbContext context, Dic dic)
        {
            var dicItems = context.DicItems.Where(x => x.Dic == dic && !x.IsDelete).OrderByDescending(x => x.OrderNumber).ToList();

            var list = new List<DicItem>();
            for (var i = dicItems.Count - 1; i >= 0; i--)
            {
                if (dicItems[i].ParentId.HasValue) continue;
                list.Add(dicItems[i]);
                dicItems.RemoveAt(i);
            }
            foreach (var item in list)
            {
                FeatchDicItemChildren(item, dicItems);
            }
            return list;
        }

        private static void FeatchDicItemChildren(DicItem dicItem, IList<DicItem> dicItems)
        {
            if (!dicItems.Any()) return;
            for (var i = dicItems.Count - 1; i >= 0; i--)
            {
                if (!dicItems[i].ParentId.Equals(dicItem.Id)) continue;
                dicItem.Children.Add(dicItems[i]);
                dicItems.RemoveAt(i);
            }
            if (!dicItem.Children.Any()) return;
            foreach (var child in dicItem.Children)
            {
                FeatchDicItemChildren(child, dicItems);
            }
        }


        public static List<DicItem> GetChildrenDicItems(this MyDbContext context, Guid id, bool includingSelf = true)
        {
            var list = new List<DicItem>();
            var dicItem = context.DicItems.Find(id);
            if (dicItem != null)
            {
                if (includingSelf) list.Add(dicItem);
                var items = context.DicItems.Where(x => x.Dic == dicItem.Dic).OrderBy(x => x.OrderNumber).ToList();
                FeatchChildrenDicItems(list, items, dicItem);
            }
            return list;
        }

        private static void FeatchChildrenDicItems(List<DicItem> list, List<DicItem> dicItems,
            DicItem dicItem)
        {
            var childrenList = dicItems.Where(x => x.ParentId == dicItem.Id).ToList();
            if (!childrenList.Any()) return;
            list.AddRange(childrenList);
            foreach (var child in childrenList)
            {
                FeatchChildrenDicItems(list, dicItems, child);
            }
        }
        #endregion

        #region 日志

        public static void WriteLog(this MyDbContext context, Log log)
        {
            log.Id = Guid.NewGuid();
            log.CreateTime = DateTime.Now;
            context.Logs.Add(log);
        }

        public static void WriteLog(this MyDbContext context, Log log, ICollection<ObjectChangeInfo> objectChangeInfos)
        {
            objectChangeInfos = objectChangeInfos ?? new List<ObjectChangeInfo>();
            log.Id = Guid.NewGuid();
            log.CreateTime = DateTime.Now;
            log.Detail = JsonConvert.SerializeObject(objectChangeInfos);
            context.Logs.Add(log);
        }

        #endregion
    }
}
