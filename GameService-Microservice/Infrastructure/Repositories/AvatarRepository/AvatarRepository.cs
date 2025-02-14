using Domain.Entity;
using Domain.Enums;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Infrastructure.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AvatarRepository : SQLRepository<Avatar>, IAvatarRepository
    {
        public AvatarRepository(CoolketContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Avatar>> GetAll(AvatarSortBy sortBy, bool IsAscending)
        {
            return await _dbSet.Sort(sortBy.ToString(), IsAscending).ToListAsync();
        }

        public async Task UpdateBackground(string image, string type)
        {
            var avatars = await _dbSet.Where(a => a.Background == null && a.Type!.ToLower().Equals(type.ToLower())).ToListAsync();

            foreach (var avatar in avatars)
            {
                avatar.Background = image;
            }
        }

    }
}
