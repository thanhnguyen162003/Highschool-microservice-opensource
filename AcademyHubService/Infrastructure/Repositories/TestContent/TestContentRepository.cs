using Domain.Entity;
using Infrastructure.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.Json;

namespace Infrastructure.Repositories
{
    public class TestContentRepository(DbContext context) : SqlRepository<TestContent>(context), ITestContentRepository
    {
        private readonly DbContext _context = context;

        public async Task CreateTestContent(List<TestContent> test)
        {
            await _dbSet.AddRangeAsync(test);
        }
        public async Task<List<TestContent>> GetByAssignmentId(Guid assignmentId)
        {
            return await _dbSet.AsNoTracking().Where(x => x.Assignmentid.Equals(assignmentId)).ToListAsync();
        }
        public async Task Delete(IEnumerable<TestContent> testContents)
        {
            _dbSet.RemoveRange(testContents);

            await Task.CompletedTask;
        }

        public async Task<Dictionary<Guid, string>> SubmitTest(List<TestContent> test, Guid assignmentId)
        {
            var testIds = test.Select(t => t.Id).ToList();
            var testAnswers = test.Select(_ => _.Answers).ToList();

            var testContent = await _dbSet.AsNoTracking().Where(x => x.Assignmentid.Equals(assignmentId)).ToListAsync();
        
            var existingEntities = testContent
                .Where(x => testIds.Contains(x.Id)).ToList();

            var notExistingEntities = testContent
                .Where(x => !testIds.Contains(x.Id)).ToList();

            Dictionary<Guid, string> wrongAnswer = new Dictionary<Guid, string>();

            foreach (var entity in notExistingEntities)
            {
                wrongAnswer[entity.Id] = entity.Answers; 
            }

            var differentEntities = testContent
                .Where(tc =>
                    existingEntities.Any(ee =>
                        ee.Id == tc.Id && !AreObjectsEqualExceptId(ee, tc)
                    )
                ).ToList();

            if (testAnswers.Count != existingEntities.Count) return null;

            for (int i=0; i < testAnswers.Count; i++)
            {
                var correctAnswer = DeserializeAnswers(existingEntities[i].Answers);
                if (!testAnswers[i].Equals(correctAnswer[existingEntities[i].CorrectAnswer.Value]))
                {
                    wrongAnswer[existingEntities[i].Id] = testAnswers[i];
                }
            }

           
            return wrongAnswer;
        }
        private static List<string>? DeserializeAnswers(string? answersJson)
        {
            if (string.IsNullOrEmpty(answersJson)) return null; 

            try
            {
                return JsonSerializer.Deserialize<List<string>>(answersJson);
            }
            catch (JsonException ex) 
            {
                Console.WriteLine($"Error deserializing answers: {ex.Message}");
                return null;
            }
        }
        bool AreObjectsEqualExceptId(object obj1, object obj2)
        {
            if (obj1 == null || obj2 == null) return false;
            if (obj1.GetType() != obj2.GetType()) return false;

            foreach (var prop in obj1.GetType().GetProperties())
            {
                if (prop.Name == "Id") continue; // Skip Id

                var val1 = prop.GetValue(obj1);
                var val2 = prop.GetValue(obj2);

                if (val1 == null && val2 == null) continue;
                if (val1 == null || val2 == null || !val1.Equals(val2))
                    return false;
            }

            return true;
        }

    }

}
