using Domain.Entity;
using Domain.Enums;
using Domain.Models.Common;
using Infrastructure.Extensions;
using Infrastructure.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class KetRepository : SQLRepository<Ket>, IKetRepository
    {
        public KetRepository(DbContext context) : base(context)
        {
        }

        public async Task<Ket?> GetMyKet(Guid ketId, Guid userId)
        {
            return await _dbSet
                .Include(k => k.KetContents)
                .FirstOrDefaultAsync(k => k.Id.Equals(ketId) && k.CreatedBy.Equals(userId));
        }

        public PagedList<Ket> GetAll(KetSortBy sortBy, bool isAscending, string? search, Guid? userId)
        {
            var query = _dbSet
                .Include(k => k.Author)
                .AsQueryable();

            if(userId != null && userId != Guid.Empty)
            {
                query = query.Where(k => k.CreatedBy.Equals(userId));
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(k => k.Name!.Contains(search));
            }

            query = sortBy switch
            {
                KetSortBy.CreatedAt => isAscending ? query.OrderBy(k => k.CreatedAt) : query.OrderByDescending(k => k.CreatedAt),
                KetSortBy.Name => isAscending ? query.OrderBy(k => k.Name) : query.OrderByDescending(k => k.Name),
                _ => query.OrderBy(k => k.CreatedAt)
            };

            return new PagedList<Ket>(query);

        }

        public async Task<PagedList<Ket>> GetKetRecommend(IEnumerable<Ket> kets, int numberKet)
        {
            // Get list author from ket user play/host
            var listAuthor = await _dbSet
                .Include(k => k.Author)
                .Select(k => k.Author!.Id)
                .ToListAsync();

            // Get list ket by author
            var listKet = await _dbSet
                .Include(k => k.Author)
                .Except(kets)
                .Where(k => listAuthor.Contains(k.Author!.Id))
                .OrderByDescending(k => k.TotalPlay)
                .ToPagedListAsync(1, numberKet);

            // if not enough ket, get more ket from all ket
            var numberKetMore = numberKet - listKet.Count();
            if (numberKetMore > 0)
            {
                var listKetMore = await _dbSet
                    .Include(k => k.Author)
                    .Except(kets)
                    .OrderByDescending(k => k.TotalPlay)
                    .ToPagedListAsync(1, numberKetMore);

                listKet.Concat(listKetMore);
            }

            return listKet;
        }

        public async Task<PagedList<Ket>> GetNewKets(int numberKet)
        {
            return await _dbSet
                .Include(k => k.Author)
                .OrderByDescending(k => k.UpdatedAt ?? k.CreatedAt)
                .ToPagedListAsync(1, numberKet);
        }
    }
}
