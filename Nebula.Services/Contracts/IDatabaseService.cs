using Realms;

namespace Nebula.Services.Contracts;

public interface IDatabaseService
{
    Task          EnterWriteTransaction(Action action, CancellationToken token = default);
    Task          AddAsync<T>(T realmObject) where T : IRealmObject;
    Task          DeleteAsync<T>(T realmObject) where T : IRealmObject;
    IQueryable<T> All<T>() where T : IRealmObject;
}