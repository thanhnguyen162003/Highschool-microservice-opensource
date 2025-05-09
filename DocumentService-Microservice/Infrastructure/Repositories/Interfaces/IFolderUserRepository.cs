using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IFolderUserRepository : IRepository<FolderUser>
    {
        Task<IEnumerable<FolderUser>> GetMyFolder(Guid userId);
        Task<IEnumerable<FolderUser>> GetFolderUsers(Guid userId);
        Task<IEnumerable<FolderUser>> GetFolderUsers();
        Task<FolderUser?> GetById(Guid folderId);
        Task<FolderUser?> GetById(Guid folderId, Guid userId);
        Task<IEnumerable<FolderUser>> GetFolderByUserId(Guid userId);
        Task<IEnumerable<FolderUser>> GetFolders();
        Task<int> GetFoldersCount(CancellationToken cancellationToken = default);
    }
}
