using System.Threading.Tasks;
using HandyControl.Controls;
using SQLite;

namespace Nebula.Core.Database
{
    public static class DatabaseExtensions
    {
        public static async Task<int> InsertOrUpdate(this SQLiteAsyncConnection conn, object obj)
        {
            return await conn.InsertOrReplaceAsync(obj);
            return await conn.UpdateAsync(obj);
        }
    }
}