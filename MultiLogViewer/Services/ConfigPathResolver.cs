using System;
using System.IO;
using System.Linq;

namespace MultiLogViewer.Services
{
    public class ConfigPathResolver : IConfigPathResolver
    {
        private const string DefaultConfigFileName = "config.yaml";

        public string ResolvePath(string[] args)
        {
            if (args != null && args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
            {
                return args[0];
            }

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DefaultConfigFileName);
        }
    }
}
