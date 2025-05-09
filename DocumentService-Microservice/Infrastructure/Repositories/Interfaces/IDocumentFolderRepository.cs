using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IDocumentFolderRepository : IRepository<DocumentFolder>
    {
        Task<IEnumerable<DocumentFolder>> GetDocumentFolderByFolderId(Guid folderId);
        Task DeleteDocumentOnFolder(Guid folderId);
        Task AddDocumentOnFolder(IEnumerable<Guid> documentIds, Guid folderId);
        Task<Guid?> DeleteDocument(Guid documentId, Guid folderId);
        Task<IEnumerable<Guid>> NotExistOnFolder(IEnumerable<Guid> documentIds, Guid folderId);
    }
}
