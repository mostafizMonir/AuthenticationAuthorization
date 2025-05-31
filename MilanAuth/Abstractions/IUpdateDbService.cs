namespace MilanAuth.Abstractions;

public interface IUpdateDbService<T> where T : class, IEntity
{
    Task<bool> UpdateModel(T entity);
} 
