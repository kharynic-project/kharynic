using System;
using System.IO;
using Kharynic.Engine.Unity;
using UnityEditor;

namespace Kharynic.Engine.Editor
{
    public class KharynicEditorMenu
    {
        [MenuItem("Kharynic/Save level")]
        public static void SaveLevel()
        {
            var world = WorldLoader.Write();
            var path = EditorUtility.SaveFilePanel
            (
                title: "Save level",
                directory: Kharynic.Engine.BuildInfo.LocalProjectPath + "/resources/levels",
                defaultName: "level",
                extension: "json"
            );
            if (string.IsNullOrEmpty(path))
                return;
            if (!path.StartsWith(Kharynic.Engine.BuildInfo.LocalProjectPath))
                throw new ArgumentException($"file access is limited to {Kharynic.Engine.BuildInfo.LocalProjectPath}");
            File.WriteAllText(path, world);
        }
        
        [MenuItem("Kharynic/Load level")]
        public static void LoadLevel()
        {
            var path = EditorUtility.OpenFilePanel
            (
                title: "Load level",
                directory: Kharynic.Engine.BuildInfo.LocalProjectPath + "/resources/levels",
                extension: "json"
            );
            if (string.IsNullOrEmpty(path))
                return;
            if (!path.StartsWith(Kharynic.Engine.BuildInfo.LocalProjectPath))
                throw new ArgumentException($"file access is limited to {Kharynic.Engine.BuildInfo.LocalProjectPath}");
            WorldLoader.Load(path.Substring(Kharynic.Engine.BuildInfo.LocalProjectPath.Length));
        }
    }
}