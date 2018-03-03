using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Kharynic.Engine.Scripting;

namespace Kharynic.Engine
{
    // format compliant with semver.org
    public class Version
    {
        public int Major { get; private set; }
        public int Minor { get; private set; }
        public int Build { get; private set; }
        public string Metadata { get; private set; }
        
        private static readonly string FilePath =
            GeneratorUtils.GetSourceFilePath(typeof(Version), "txt", isUnityAsset: false, isGenerated: false);

        private Version(){}

        private static Version ReadLast()
        {
            if (!File.Exists(FilePath))
            {
                Debug.Log($"WARNING: {FilePath} not found - resetting version number");
                Write(new Version());
            }
            var file = GeneratorUtils.ReadFile(FilePath).Split('.', '+').ToArray();
            return new Version
            {
                Major = int.Parse(file[0]),
                Minor = int.Parse(file[1]),
                Build = int.Parse(file[2]),
                Metadata = file.Length > 3 ? file[3] : null
            };
        }
        
        private static void Write(Version version)
        {
            GeneratorUtils.WriteFile(version.ToString(), FilePath);
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Build}{(Metadata != null ? $"+{Metadata}" : "")}";
        }

        private static string GetBuildHash()
        {
            return Guid.NewGuid().GetHashCode().ToString("x8").Substring(0, 3);
        }

        public static Version GetForPreview()
        {
            var lastVersion = ReadLast();
            lastVersion.Build++;
            lastVersion.Metadata = $"{GetBuildHash()}.preview";
            return lastVersion;
        }

        public static Version IncrementAndGet()
        {
            var lastVersion = ReadLast();
            lastVersion.Build++;
            lastVersion.Metadata = GetBuildHash();
            Write(lastVersion);
            return lastVersion;
        }
    }
}