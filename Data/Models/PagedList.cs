using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZestMonitor.Api.Data.Entities;

namespace ZestMonitor.Api.Data.Models
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public PagedList() { }
        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            this.CurrentPage = pageNumber;
            this.TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.PageSize = pageSize;
            this.TotalCount = count;
            this.AddRange(items);
        }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedList<T>(items, count, pageNumber, pageSize);
        }

        public static PagedList<T> Create(List<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var items = source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToList();

            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}