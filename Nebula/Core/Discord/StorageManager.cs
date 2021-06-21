using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Nebula.Core.Discord
{
    public partial class StorageManager
    {
        public IEnumerable<FileStat> Files()
        {
            int fileCount = Count();
            var files = new List<FileStat>();
            for (var i = 0; i < fileCount; i++) files.Add(StatAt(i));
            return files;
        }
    }
}