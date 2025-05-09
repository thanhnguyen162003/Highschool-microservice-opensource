using Infrastructure.Contexts;
using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories
{
    public class FlashcardTagRepository(DocumentDbContext context) : BaseRepository<FlashcardTag>(context), IFlashcardTagRepository
    {
    }
}
