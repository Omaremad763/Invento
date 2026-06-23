using Application.DTOS;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Extentions;

public static class PaginationSetup
{
    public static async Task<PaginatedResult<T>> ToPaginatedListAsync<T>(
this IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();
        return new PaginatedResult<T>(items, count, pageNumber, pageSize);
    }
}