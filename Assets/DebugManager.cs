using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugManager
{
    public static bool DebugEnabled = true;  // Toggle this to enable/disable all debug logs

    // Optional: log levels (you can extend this with more control)
    public static bool LogInfo = true;
    public static bool LogWarnings = true;
    public static bool LogErrors = true;

    // Debug Log Methods
    public static void Log(object message)
    {
        if (DebugEnabled && LogInfo)
        {
            Debug.Log(message);
        }
    }

    public static void LogWarning(object message)
    {
        if (DebugEnabled && LogWarnings)
        {
            Debug.LogWarning(message);
        }
    }

    public static void LogError(object message)
    {
        if (DebugEnabled && LogErrors)
        {
            Debug.LogError(message);
        }
    }
}

