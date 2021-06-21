using System.Collections.Generic;

namespace Nebula.Discord.SDK
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