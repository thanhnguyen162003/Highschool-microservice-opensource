using Domain.QueriesFilter;
using Infrastructure.Contexts;
using Infrastructure.Repositories.Interfaces;
using System.Linq;

namespace Infrastructure.Repositories
{
    public class DocumentRepository(DocumentDbContext context) : BaseRepository<Document>(context), IDocumentRepository
    {
        public async Task<Document> GetDocumentsByIds(Guid documentId, CancellationToken cancellationToken = default)
        {
            return (await _entities.AsNoTracking()
                .Where(document => document.Id == documentId)
                .Include(x=>x.SubjectCurriculum)
                .ThenInclude(s=>s.Subject)
                .Include(c=>c.SubjectCurriculum)
                .ThenInclude(c=>c.Curriculum)
                .AsSplitQuery()
                .FirstOrDefaultAsync(cancellationToken))!;
        }

        public async Task<List<Document>> GetRelatedDocumentsByDocumentId(Guid documentId, CancellationToken cancellationToken = default)
        {
            // Find the document with the provided documentId
            var document = await _entities.AsNoTracking()
                .Where(x => x.Id == documentId)
                .FirstOrDefaultAsync(cancellationToken);

            // If the document doesn't exist, return an empty list
            if (document == null || document.SubjectCurriculumId == null)
            {
                return new List<Document>();
            }

            // Get the SubjectCurriculumId of the document
            var subjectCurriculumId = document.SubjectCurriculumId;
            var documentYear = document.DocumentYear;
            var semester = document.Semester;
            var schoolId = document.SchoolId;

            // Find related documents with the same SubjectCurriculumId, excluding the original document
            var relatedDocuments = await _entities.AsNoTracking()
                .Where(x =>
                        x.SubjectCurriculumId == subjectCurriculumId && // Matching SubjectCurriculumId
                        x.Id != documentId &&                          // Excluding the original document
                        x.DeletedAt == null &&                       
                        x.DocumentYear == documentYear &&              // Matching DocumentYear
                        x.Semester == semester &&                      // Matching Semester
                        x.SchoolId == schoolId                         // Matching SchoolId
                )
                .OrderByDescending(x => x.View)  // Sort by most viewed
                .ThenBy(x => x.Like)             // Then sort by likes
                .Take(5)
                .ToListAsync(cancellationToken);
            
            // If no related documents were found, use fallback criteria
            if (!relatedDocuments.Any())
            {
                // Fallback: get documents with minimal criteria, ordered by views and likes
                relatedDocuments = await _entities.AsNoTracking()
                    .Where(x =>
                        x.Id != documentId &&
                        x.DeletedAt == null
                    )
                    .OrderByDescending(x => x.View)
                    .ThenBy(x => x.Like)
                    .Take(5)
                    .ToListAsync(cancellationToken);
            }
            return relatedDocuments;
        }


        public async Task<IEnumerable<Document>> GetDocuments()
		{
            return await _entities
                .Include(y => y.SubjectCurriculum)
                .ThenInclude(x => x.Subject)
                .Include(c=>c.SubjectCurriculum)
                .ThenInclude(c=>c.Curriculum)
                .AsSplitQuery()
                .AsNoTracking()
                .ToListAsync();
        }

		public async Task<List<Document>> GetDocumentBySubjectIdFilter(List<Guid> subjectIds)
        {
            var documents = await _entities
                .AsNoTracking()
                .Where(x => x.SubjectCurriculumId != null && subjectIds.Contains(x.SubjectCurriculum.SubjectId)) 
                .Include(y=>y.SubjectCurriculum)
                .ThenInclude(s=>s.Subject)
                .Include(c=>c.SubjectCurriculum)
                .ThenInclude(c=>c.Curriculum)
                .AsSplitQuery()
                .Take(8)
                .ToListAsync(); 
            return documents;
        }

        public async Task<List<Document>> GetDocumentPlaceholder()
        {
            var documents = await _entities
                .AsNoTracking()
                .OrderBy(x=>x.View)
                .Include(y=>y.SubjectCurriculum)
                .ThenInclude(s=>s.Subject)
                .ThenInclude(c=>c.Category)
                .Take(8)
                .ToListAsync(); 
            return documents;
        }

        public async Task<List<Document>> GetRelatedDocuments(Guid subjectId, CancellationToken cancellationToken = default)
        {
            var documents = await _entities
                .AsNoTracking()
                .Where(x => x.SubjectCurriculumId != null &&  x.SubjectCurriculum.SubjectId.Equals(subjectId))
                .Take(3)
                .ToListAsync(cancellationToken);
            return documents;
        }

        public async Task<List<string>> GetDocumentIdsAsString(CancellationToken cancellationToken = default)
        {
            return await _entities
                .AsNoTracking()
                .OrderByDescending(x => x.View)
                .Select(s => s.Id.ToString()).Take(2) // Convert Guid to string
                .ToListAsync(cancellationToken);
        }

        public async Task<(List<Document> Documents, int TotalCount)> GetDocumentAdvanceFilter(DocumentAdvanceQueryFilter queryFilter, CancellationToken cancellationToken = default)
        {
            IQueryable<Document> query = _entities
                .Include(d => d.School)
                .ThenInclude(d => d.Province)
                .Include(d => d.SubjectCurriculum)
                .ThenInclude(d => d.Subject)
                .ThenInclude(d => d.Category)
                .Include(d => d.SubjectCurriculum)
                .ThenInclude(d => d.Curriculum).AsSplitQuery();

            if (!string.IsNullOrEmpty(queryFilter.Search))
            {
                query = query.Where(d => d.DocumentName.Contains(queryFilter.Search) || 
                                         (d.DocumentDescription != null && d.DocumentDescription.Contains(queryFilter.Search)));
            }

            if (queryFilter.SchoolId.HasValue)
            {
                query = query.Where(d => d.SchoolId != null && d.School.Id == queryFilter.SchoolId);
            }
            if (queryFilter.SubjectIds != null && queryFilter.SubjectIds.Any())
            {
                query = query.Where(d => d.SubjectCurriculumId != null &&  queryFilter.SubjectIds.Contains(d.SubjectCurriculum.SubjectId));
            }
            //if (queryFilter.SubjectCurriculumId.HasValue)
            //{
            //    query = query.Where(d => d.SubjectCurriculumId == queryFilter.SubjectCurriculumId);
            //}
            if (queryFilter.ProvinceId.HasValue)
            {
                query = query.Where(d => d.SchoolId != null && d.School.ProvinceId == queryFilter.ProvinceId);
            }
            if (queryFilter.CategoryIds != null && queryFilter.CategoryIds.Any())
            {
                query = query.Where(d => d.SubjectCurriculumId != null && d.SubjectCurriculum!.Subject!.CategoryId != null && queryFilter.CategoryIds.Contains(d.SubjectCurriculum.Subject!.CategoryId.Value));
            }

            if (queryFilter.CurriculumIds != null && queryFilter.CurriculumIds.Any())
            {
                query = query.Where(d => d.SubjectCurriculumId != null && queryFilter.CurriculumIds.Contains(d.SubjectCurriculum.CurriculumId));
            }

            if (queryFilter.DocumentYear.HasValue)
            {
                query = query.Where(d => d.DocumentYear == queryFilter.DocumentYear);
            }

            if (queryFilter.Semester.HasValue)
            {
                query = query.Where(d => d.Semester == queryFilter.Semester);
            }

            if (queryFilter.SortPopular.HasValue)
            {
                query = queryFilter.SortPopular.Value ? query.OrderByDescending(d => d.View) : query.OrderBy(d => d.View); 
            }
            else
            {
                query = query.OrderByDescending(x => x.CreatedAt);
            }
            int totalCount = await query.CountAsync(cancellationToken);
            if (queryFilter.PageSize > 0)
            {
                query = query
                    .Skip((queryFilter.PageNumber - 1) * queryFilter.PageSize)
                    .Take(queryFilter.PageSize);
            }
            var pagination = await query.ToListAsync(cancellationToken);
            return (pagination, totalCount);
        } 
        public async Task<List<string>> GetDocumentBySubjectId(List<Guid> subjectIds, CancellationToken cancellationToken = default)
        {
            var documents = await _entities
                .AsNoTracking()
                .Where(x => x.SubjectCurriculumId != null && subjectIds.Contains(x.SubjectCurriculumId.Value))
                .Select(x => x.Id.ToString())
                .ToListAsync();
            return documents;
        }

        public async Task<List<Document>> GetDocumentByIds(IEnumerable<string> documentIds, CancellationToken cancellationToken = default)
        {
            if (documentIds == null || !documentIds.Any())
            {
                return new List<Document>();
            }

            List<Guid> guids = documentIds.Select(guid => Guid.Parse(guid)).ToList();

            return await context.Documents.AsNoTracking()
                .Where(x => guids.Contains(x.Id))
                .Select(x => new Document { Id = x.Id, DocumentName = x.DocumentName, Slug = x.Slug })
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Guid>> ExceptExistDocuments(IEnumerable<Guid> documentIds)
        {
            var documents = await _entities
                .AsNoTracking()
                .Where(x => documentIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync();

            var result = documentIds.Except(documents).ToList();

            return result;
        }
    }
}
