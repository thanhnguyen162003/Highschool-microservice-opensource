using Application.Common.Interfaces.ClaimInterface;
using Application.Services.CacheService.Intefaces;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace Application.Common.UoW;

public class UnitOfWork : IUnitOfWork
{
    private IDbContextTransaction _transaction;
    private readonly IOrdinaryDistributedCache _distributedCache;

    private readonly IClaimInterface _claimInterface;

    public async Task RollbackTransactionAsync()
    {
        try
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
            }
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

}