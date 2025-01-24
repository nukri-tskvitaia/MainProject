using MvcProject.DTO;
using MvcProject.Models;

namespace MvcProject.Data.Repositories;

public interface ITransactionRepository
{
    public Task<bool> CreateAsync(Transaction data);
    public Task<IEnumerable<TransactionModel>> GetAllAsync();
}
