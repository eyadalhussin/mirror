using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildScript
{
    private static string path = "C:/Users/eyada/Desktop/MasterserverDev";

    [MenuItem("Build/Build All Prod + Dev")]
    [Obsolete]
    public static void BuildAllProdDev()
    {
        EnvironmentSetting.prod = false;
        path = "C:/Users/eyada/Desktop/MasterserverDev";
        BuildWindowsServer();
        BuildLinuxServer();
        BuildWindowsClient();
        EnvironmentSetting.prod = true;
        path = "C:/Users/eyada/Desktop/MasterserverProd";
        BuildWindowsServer();
        BuildLinuxServer();
        BuildWindowsClient();
    }


    [MenuItem("Build/Build All Prod + Dev + WebGL")]
    [Obsolete]
    public static void BuildAllProdDevWebGL()
    {
        EnvironmentSetting.prod = false;
        path = "C:/Users/eyada/Desktop/MasterserverDev";
        BuildWindowsServer();
        BuildLinuxServer();
        BuildWindowsClient();
        BuildWebGL();
        EnvironmentSetting.prod = true;
        path = "C:/Users/eyada/Desktop/MasterserverProd";
        BuildWindowsServer();
        BuildLinuxServer();
        BuildWindowsClient();
        BuildWebGL();
    }

    [MenuItem("Build/Build All Dev")]
    [Obsolete]
    public static void BuildAllDev()
    {
        EnvironmentSetting.prod = false;
        path = "C:/Users/eyada/Desktop/MasterserverDev";
        BuildLinuxServer();
        BuildWindowsServer();
        BuildWindowsClient();
    }

    [MenuItem("Build/Build All Prod")]
    [Obsolete]
    public static void BuildAllProd()
    {
        EnvironmentSetting.prod = true;
        path = "C:/Users/eyada/Desktop/MasterserverProd";
        BuildWindowsServer();
        BuildLinuxServer();
        BuildWindowsClient();
    }

    [MenuItem("Build/Build All Dev + WebGL")]
    [Obsolete]
    public static void BuildAllDevWebGL()
    {
        EnvironmentSetting.prod = false;
        path = "C:/Users/eyada/Desktop/MasterserverDev";
        BuildWindowsServer();
        BuildLinuxServer();
        BuildWindowsClient();
        BuildWebGL();
    }

    [MenuItem("Build/Build All Prod + WebGL")]
    [Obsolete]
    public static void BuildAllProdWebGL()
    {
        EnvironmentSetting.prod = true;
        path = "C:/Users/eyada/Desktop/MasterserverProd";
        BuildWindowsServer();
        BuildLinuxServer();
        BuildWindowsClient();
        BuildWebGL();
    }

    [MenuItem("Build/Build Server (Windows)")]
    [Obsolete]
    public static void BuildWindowsServer()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/Scenes/Offline.unity", "Assets/Scenes/Room.unity", "Assets/Scenes/Game.unity" };
        buildPlayerOptions.locationPathName = $"{path}/Mirror/Windows/Server/Server.exe";
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.CompressWithLz4HC | BuildOptions.EnableHeadlessMode;

        Console.WriteLine("Building Server (Windows)...");
        BuildPipeline.BuildPlayer(buildPlayerOptions);
        Console.WriteLine("Built Server (Windows).");
    }

    [MenuItem("Build/Build Server (Linux)")]
    [Obsolete]
    public static void BuildLinuxServer()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/Scenes/Offline.unity", "Assets/Scenes/Room.unity", "Assets/Scenes/Game.unity" };
        buildPlayerOptions.locationPathName = $"{path}/Mirror/Linux/Server/Server.x86_64";
        buildPlayerOptions.target = BuildTarget.StandaloneLinux64;
        buildPlayerOptions.options = BuildOptions.CompressWithLz4HC | BuildOptions.EnableHeadlessMode;

        Console.WriteLine("Building Server (Linux)...");
        BuildPipeline.BuildPlayer(buildPlayerOptions);
        Console.WriteLine("Built Server (Linux).");
    }


    [MenuItem("Build/Build Client (Windows)")]
    public static void BuildWindowsClient()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/Scenes/Offline.unity", "Assets/Scenes/Room.unity", "Assets/Scenes/Game.unity" };
        buildPlayerOptions.locationPathName = $"{path}/Mirror/Windows/Client/Client.exe";
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.CompressWithLz4HC;

        Console.WriteLine("Building Client (Windows)...");
        BuildPipeline.BuildPlayer(buildPlayerOptions);
        Console.WriteLine("Built Client (Windows).");
    }

    [MenuItem("Build/Build WebGL")]
    public static void BuildWebGL()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/Scenes/Offline.unity", "Assets/Scenes/Room.unity", "Assets/Scenes/Game.unity" };
        buildPlayerOptions.locationPathName = $"{path}/Mirror/WebGL";
        buildPlayerOptions.target = BuildTarget.WebGL;
        buildPlayerOptions.options = BuildOptions.None;

        Console.WriteLine("Building WebGL");
        BuildPipeline.BuildPlayer(buildPlayerOptions);
        Console.WriteLine("Built WebGL");
    }
}