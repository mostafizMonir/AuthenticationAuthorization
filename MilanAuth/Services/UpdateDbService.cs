using MilanAuth.Abstractions;
using MilanAuth.Data;
using System.Threading.Tasks;

namespace MilanAuth.Services;

public class UpdateDbService<T> : IUpdateDbService<T> where T : class, IEntity
{
    private readonly Repository<T> _repository;

    public UpdateDbService(Repository<T> repository)
    {
        _repository = repository;
    }

    public async Task<bool> UpdateModel(T entity)
    {
        await _repository.UpdateAsync(entity);
        return true;
    }
}
