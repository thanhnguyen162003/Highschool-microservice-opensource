using Infrastructure.Constraints;
using Infrastructure.Contexts;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class DocumentFolderRepository(DocumentDbContext context) : BaseRepository<DocumentFolder>(context), IDocumentFolderRepository
    {
        public async Task<IEnumerable<DocumentFolder>> GetDocumentFolderByFolderId(Guid folderId)
        {
            return await _entities
                .Include(e => e.Folder)
                .Include(e => e.Document).ThenInclude(d => d.SubjectCurriculum)
                    .ThenInclude(s => s.Subject)
                    .ThenInclude(s => s.MasterSubject)
                .Include(e => e.Document).ThenInclude(d => d.SubjectCurriculum).ThenInclude(s => s.Curriculum)
                .Where(e => e.FolderId.Equals(folderId))
                .ToListAsync();
        }

        public async Task DeleteDocumentOnFolder(Guid folderId)
        {
            var documents = await _entities.Where(e => e.FolderId.Equals(folderId)).ToListAsync();

            _entities.RemoveRange(documents);

        }

        public async Task AddDocumentOnFolder(IEnumerable<Guid> documentIds, Guid folderId)
        {
            foreach (var documentId in documentIds)
            {
                var documentFolder = new DocumentFolder()
                {
                    Id = Guid.NewGuid(),
                    FolderId = folderId,
                    DocumentId = documentId,
                    CreatedAt = DateTime.UtcNow
                };

                await _entities.AddAsync(documentFolder);
            }
        }

        public async Task<Guid?> DeleteDocument(Guid documentId, Guid folderId)
        {
            var documentOnFolder = await _entities.FirstOrDefaultAsync(e => e.DocumentId.Equals(documentId) && e.FolderId.Equals(folderId));

            if (documentOnFolder != null)
            {
                _entities.Remove(documentOnFolder);

                return documentOnFolder.FolderId;
            }

            return null;
        }

        public async Task<IEnumerable<Guid>> NotExistOnFolder(IEnumerable<Guid> documentIds, Guid folderId)
        {
            var documents = await _entities
                .AsNoTracking()
                .Where(e => documentIds.Contains(e.DocumentId) && e.FolderId.Equals(folderId))
                .Select(e => e.DocumentId)
                .ToListAsync();

            return documentIds.Except(documents);

        }
    }
}
