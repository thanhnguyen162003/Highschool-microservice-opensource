using Infrastructure.Contexts;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UserQuizProgressRepository(DocumentDbContext context) : BaseRepository<UserQuizProgress>(context), IUserQuizProgressRepository
    {
    }
}
