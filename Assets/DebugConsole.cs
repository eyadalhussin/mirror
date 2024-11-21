using System.Collections.Generic;
using UnityEngine;

public class DebugConsoleScript : MonoBehaviour
{
    private struct LogMessage
    {
        public string message;
        public LogType type;

        public LogMessage(string message, LogType type)
        {
            this.message = message;
            this.type = type;
        }
    }

    // List to hold all log messages
    private List<LogMessage> logMessages = new List<LogMessage>();
    private Vector2 scrollPosition;

    // Background transparency color
    private Color backgroundColor = new Color(0, 0, 0, 0.4f);

    // Font style for the logs
    private GUIStyle logStyle;
    private GUIStyle errorStyle;

    // Scroll view toggle
    private bool showConsole = false;

    // Log handling
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Application.logMessageReceived += HandleLog;

        // Set up the log style
        logStyle = new GUIStyle();
        logStyle.fontSize = 14;
        logStyle.normal.textColor = Color.white;
        logStyle.wordWrap = true;

        // Set up the error style
        errorStyle = new GUIStyle();
        errorStyle.fontSize = 14;
        errorStyle.normal.textColor = Color.red;
        errorStyle.wordWrap = true;
    }

    private void Update()
    {
        // Toggle console visibility with the F1 key
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ToggleConsole();
        }
    }

    private void OnDestroy()
    {
        // Stop capturing logs when this object is destroyed
        Application.logMessageReceived -= HandleLog;
    }

    // Store log entries and stack traces
    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        string logEntry = $"[{type}] {logString}\n";
        if (type == LogType.Exception || type == LogType.Error)
        {
            logEntry += $"\n{stackTrace}"; // Append stack trace for exceptions and errors
        }

        logMessages.Add(new LogMessage(logEntry, type));

        // Optionally limit the number of logs stored to avoid memory issues
        if (logMessages.Count > 1000)
        {
            logMessages.RemoveAt(0); // Remove oldest log
        }
    }

    // Toggle the console visibility
    public void ToggleConsole()
    {
        showConsole = !showConsole;
    }

    private void OnGUI()
    {
        // Only show the console when toggled on
        if (!showConsole) return;

        // Set the GUI background color for the console
        Color originalColor = GUI.color;  // Save the original GUI color
        GUI.color = backgroundColor;

        // Draw the semi-transparent background box
        GUI.Box(new Rect(10, 10, Screen.width - 20, Screen.height / 2), "");

        // Restore the original color to avoid affecting other GUI elements
        GUI.color = originalColor;

        // Define the area where the text will be drawn
        GUILayout.BeginArea(new Rect(10, 10, Screen.width - 20, Screen.height / 2));

        // Create a scrollable view for the logs
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(Screen.width - 40), GUILayout.Height(Screen.height / 2 - 40));

        // Display all logs in the scroll view, use different styles based on log type
        foreach (var logMessage in logMessages)
        {
            if (logMessage.type == LogType.Error || logMessage.type == LogType.Exception || logMessage.type == LogType.Assert)
            {
                GUILayout.Label(logMessage.message, errorStyle); // Display error logs in red
            }
            else
            {
                GUILayout.Label(logMessage.message, logStyle); // Display normal logs in white
            }
        }

        GUILayout.EndScrollView();
        GUILayout.EndArea(); // Close the area block
    }
}
