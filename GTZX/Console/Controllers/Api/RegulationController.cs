using Common;
using Helper.Extension;
using Modules;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Console.Controllers.Api
{
    [RoutePrefix("api/Regulation")]
    public class RegulationController : ApiController
    {

        readonly MyDbContext context =  new MyDbContext();


        [HttpGet]
        [Route("GetRegulationList/{parentId?}")]
        public HttpResponseMessage GetRegulationList(Guid? parentId = null)
        {
            /*
            //    IQueryable<Regulation> regulation = context.Regulations.OrderBy(x => x.PublishDate).Where(x => x.IsEnable == true);
            var regulation = context.Regulations.OrderBy(x => x.PublishDate).Where(x => x.IsEnable == true);
            //var dicItemP = context.DicItems.FirstOrDefault(x => x.Name == "政策文件目录");
            //IQueryable<DicItem> dicItemS = context.DicItems.Where(s => s.ParentId == dicItemP.Id);

            if (parentId != null)
            {
                regulation = context.Regulations.Where(x => x.ParentId == parentId);
            }

            var count = regulation.Count();


            var list = regulation.ToPage(2, count).ToList();
            var result = list.Select(
                x => new
                {
                    x.Id,
                    x.RegulationName,
                    x.RegulationNo,
                    x.PublishDep,
                    PublishDate = x.PublishDate.Format("yyyy-MM-dd"),
                    x.ParentId,
                    DicItemName = context.DicItems.Where(y => y.Id == x.ParentId),
                }
            );
            return Json(
                    new {
                        Count = count,
                        Data = result
                    }            
                );
                */
            //样例
            var dictory = new Dictionary<string, List<object>>();
            //获取文件分组
            var grouplist = context.DicItems.Where(x => x.Dic==Dic.RegulationType&& !x.ParentId.HasValue).ToList();
            TraverseFile(grouplist, dictory);
           return new ServiceInvokeResult
            {
                Result = true,
                Data = dictory
           }.GetJsonMessage();
        }

        [HttpGet]
        [Route("ShowRegulation/{Guid?}")]
        public HttpResponseMessage ShowRegulation(Guid? id)
        {
            var data = context.Regulations.Find(id)??new Regulation();
            return new ServiceInvokeResult
            {
                Data = null
            }.GetJsonMessage();
        }

        /// <summary>
        /// 遍历政策文件
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="dictory"></param>
        private void TraverseFile(IList<DicItem> diclist, Dictionary<string, List<object>> dictory)
        {
            if(diclist==null || diclist.Count < 1)
            {
                return;
            }
            foreach(var dic in diclist)
            {
                var fileList = GetFileList(dic.Id);
                if (!dictory.ContainsKey(dic.Name))
                {
                    dictory.Add(dic.Name, fileList);
                }
                TraverseFile(dic.Children, dictory);
            }
        }

        /// <summary>
        /// 获取某个类型下的所有文件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private List<object> GetFileList(Guid id)
        {
            var list = context.Regulations.Where(x => x.ParentId == id && x.IsEnable).Select(x => new
            {
                x.Id,
                Name = x.RegulationName,
                No = x.RegulationNo,
                x.PublishDep,
                x.PublishDate
            }).ToList().Cast<object>().ToList();
            return list;
        }
    }
}
