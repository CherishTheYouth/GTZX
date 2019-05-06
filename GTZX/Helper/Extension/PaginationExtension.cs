using System;
using System.Linq;

namespace Helper.Extension
{
    public static class PaginationExtension
    {
        /// <summary>
        /// 获取分页后的结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="currentPage">第几页</param>
        /// <param name="total">总记录数</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns></returns>
        public static IQueryable<T> ToPage<T>(this IQueryable<T> queryable, int currentPage, int total, int pageSize = 10)
        {
            var maxPage = total / pageSize;
            if (total % pageSize > 0)
            {
                maxPage++;
            }
            currentPage = Math.Min(currentPage, maxPage);
            return queryable.Skip(Math.Max(pageSize * (currentPage - 1), 0)).Take(pageSize);
        }
    }
}
