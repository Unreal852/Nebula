using Nebula.Desktop.Contracts;
using Realms;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nebula.Desktop.Services;

public class RealmDatabaseService : IDatabaseService
{
    private readonly Realm   _realm;
    private readonly ILogger _logger;

    public RealmDatabaseService(ILogger logger)
    {
        _logger = logger.WithPrefix(nameof(RealmDatabaseService));
        var dbPath = Path.Combine(AppContext.BaseDirectory, "database", "db.realm");
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
        var realmConfig = new RealmConfiguration(dbPath);
#if DEBUG
        realmConfig.ShouldDeleteIfMigrationNeeded = true;
#endif
        _realm = Realm.GetInstance(realmConfig);
    }

    public Task EnterWriteTransaction(Action action, CancellationToken token = default)
    {
        return _realm.WriteAsync(action, token);
    }

    public Task AddAsync<T>(T realmObject) where T : IRealmObject
    {
        return _realm.WriteAsync(() => { _realm.Add(realmObject); });
    }

    public Task DeleteAsync<T>(T realmObject) where T : IRealmObject
    {
        return _realm.WriteAsync(() => { _realm.Remove(realmObject); });
    }

    public IQueryable<T> All<T>() where T : IRealmObject
    {
        return _realm.All<T>();
    }
}