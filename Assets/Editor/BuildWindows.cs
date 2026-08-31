using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class BuildWindows
{
    [MenuItem("Tools/Build Windows Player")]
    public static void MenuBuild()
    {
        PerformBuild();
    }

    public static void PerformBuild()
    {
        var projectRoot = Application.dataPath.Replace("/Assets", "");
        var buildDir = Path.Combine(projectRoot, "Builds", "Windows");
        Directory.CreateDirectory(buildDir);

        var scenes = new[]
        {
            "Assets/Scenes/TitleScreen.unity",
            "Assets/Scenes/MainGame.unity"
        };

        var options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = Path.Combine(buildDir, "DoubleEcho.exe"),
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        var report = BuildPipeline.BuildPlayer(options);
        if (report.summary.totalErrors > 0 || report.summary.result.ToString() != "Succeeded")
        {
            throw new Exception($"Windows build failed: {report.summary.result}\n{report.summary.totalErrors} errors\n{report.summary.totalWarnings} warnings");
        }

        Debug.Log($"Windows build succeeded: {report.summary.outputPath}");
    }
}
