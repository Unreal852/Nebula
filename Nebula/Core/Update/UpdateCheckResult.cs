namespace Nebula.Core.Update
{
    public class UpdateCheckResult
    {
        public UpdateCheckResult(bool updateAvailable, string currentVersion, string newVersion = "", string newVersionFull = "", string packageUrl = "")
        {
            UpdateAvailable = updateAvailable;
            CurrentVersion = currentVersion;
            NewVersionFull = newVersionFull;
            PackageUrl = packageUrl;
            NewVersion = newVersion;
        }

        public bool   UpdateAvailable { get; }
        public string CurrentVersion  { get; }
        public string NewVersion      { get; }
        public string NewVersionFull  { get; }
        public string PackageUrl      { get; }
    }
}