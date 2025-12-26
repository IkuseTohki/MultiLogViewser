using System;
using System.IO;

namespace MultiLogViewer.Services
{
    public class ConfigPathResolver : IConfigPathResolver
    {
        private const string DefaultLogProfileName = "LogProfile.yaml";
        private const string AppSettingsFileName = "AppSettings.yaml";

        public string ResolveLogProfilePath(string[] args)
        {
            if (args != null && args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
            {
                return args[0];
            }

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DefaultLogProfileName);
        }

        public string GetAppSettingsPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppSettingsFileName);
        }
    }
}
