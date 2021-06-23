namespace Nebula.Core.Update
{
    public class UpdateCheckResult
    {
        public UpdateCheckResult(bool updateAvailable, string currentVersion, string newVersion = "", string packageUrl = "")
        {
            UpdateAvailable = updateAvailable;
            CurrentVersion = currentVersion;
            PackageUrl = packageUrl;
            NewVersion = newVersion;
        }

        public bool   UpdateAvailable { get; }
        public string CurrentVersion  { get; }
        public string NewVersion      { get; }
        public string PackageUrl      { get; }
    }
}