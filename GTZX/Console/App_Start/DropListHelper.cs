using Microsoft.SqlServer.Server;
using Modules;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Console.App_Start
{
    public class DropListHelper
    {
        public static SelectList ToList<T>(List<T> list, string valuefield, string textfield) where T : class, new()
        {
            T o = null;
            list.Insert(0, null);
            SelectList Data = new SelectList(list, valuefield, textfield);
            return Data;
        }

        public static SelectList ToNoNullList<T>(List<T> list, string valuefield, string textfield) where T : class, new()
        {
            T o = null;
            SelectList Data = new SelectList(list, valuefield, textfield);
            return Data;
        }
    }

    public static class GetSelectHelper {

        public static SelectList GetSelectDic(Dic dic, Guid? parentId)
        {
            MyDbContext context = new MyDbContext();
            //    List<DicItem> dicList = context.DicItems.Where(x => x.Dic == dic && x.ParentId == parentId && !x.IsDelete).OrderBy(p => p.OrderNumber).ToList();
            List<DicItem> dicList = context.DicItems.Where(x => x.Dic == dic && x.ParentId == parentId && !x.IsDelete).OrderBy(p => p.OrderNumber).ToList();
            if (dicList.Count == 0)
            {
                DicItem d = new DicItem();
                d.Id = Guid.Empty;
                d.Name = "请选择";
                dicList.Insert(0, d);
            }
            else
            {
                DicItem d = new DicItem();
                d.Id = Guid.Empty;
                d.Name = "请选择";
                dicList.Insert(0, d);
            }
            return DropListHelper.ToList(dicList, "Id", "Name");
        }

    }
}