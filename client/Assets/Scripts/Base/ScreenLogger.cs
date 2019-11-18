﻿#define ENABLE_SCREEN_LOG
using UnityEngine;
using System.Collections.Generic;

public class ScreenLogger : MonoBehaviour
{

#if ENABLE_SCREEN_LOG
    struct Log
    {
        public string message;
        public string stackTrace;
        public LogType type;
    }

    #region Inspector Settings

    /// <summary>
    /// The hotkey to show and hide the console window.
    /// </summary>
    public KeyCode toggleKey = KeyCode.F1;

    /// <summary>
    /// Whether to open the window by shaking the device (mobile-only).
    /// </summary>
    public bool shakeToOpen = true;

    /// <summary>
    /// The (squared) acceleration above which the window should open.
    /// </summary>
    public float shakeAcceleration = 3f;

    /// <summary>
    /// Whether to only keep a certain number of logs.
    ///
    /// Setting this can be helpful if memory usage is a concern.
    /// </summary>
    public bool restrictLogCount = false;

    /// <summary>
    /// Number of logs to keep before removing old ones.
    /// </summary>
    public int maxLogs = 1000;

    #endregion

    private readonly List<Log> logs = new List<Log>();
    private Vector2 scrollPosition;
    private bool visible;
    private bool collapse;

    // Visual elements:
    static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>
    {
        { LogType.Assert, Color.white },
        { LogType.Error, Color.red },
        { LogType.Exception, Color.red },
        { LogType.Log, Color.white },
        { LogType.Warning, Color.yellow },
    };

    const string windowTitle = "Console";
    const int margin = 20;
    static readonly GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console.");
    static readonly GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");

    readonly Rect titleBarRect = new Rect(0, 0, 10000, 20);
    Rect windowRect = new Rect(margin, margin, Screen.width - (margin * 2), Screen.height - (margin * 2));

    void Awake()
    {
        //Debug.LogError("test 1");
        //Debug.Log("test 2");
        //visible = true;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            visible = !visible;

        if (shakeToOpen && Input.acceleration.sqrMagnitude > shakeAcceleration)
            visible = true;
    }

    void OnGUI()
    {
        //if (!visible) return;
        if (visible) windowRect = GUILayout.Window(123456, windowRect, DrawConsoleWindow, windowTitle);
        int w = Screen.width / 5;
        GUILayout.BeginArea(new Rect(Screen.width / 2 - w / 2, 0, w, Screen.height / 10));
        GUILayoutOption o1 = GUILayout.Height(40);
        GUILayoutOption o2 = GUILayout.Width(100);
        GUILayoutOption[] oo = { o1, o2 };
        if (GUILayout.Button("Show/Hide", oo)) visible = !visible;
        GUILayout.EndArea();
    }

    /// <summary>
    /// Displays a window that lists the recorded logs.
    /// </summary>
    /// <param name="windowID">Window ID.</param>
    void DrawConsoleWindow(int windowID)
    {
        DrawLogsList();
        DrawToolbar();

        // Allow the window to be dragged by its title bar.
        GUI.DragWindow(titleBarRect);
    }

    /// <summary>
    /// Displays a scrollable list of logs.
    /// </summary>
    void DrawLogsList()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        // Iterate through the recorded logs.
        for (var i = 0; i < logs.Count; i++)
        {
            var log = logs[i];

            // Combine identical messages if collapse option is chosen.
            if (collapse && i > 0)
            {
                var previousMessage = logs[i - 1].message;

                if (log.message == previousMessage)
                {
                    continue;
                }
            }

            GUI.contentColor = logTypeColors[log.type];
            GUILayout.Label(log.message);
        }

        GUILayout.EndScrollView();

        // Ensure GUI colour is reset before drawing other components.
        GUI.contentColor = Color.white;
    }

    /// <summary>
    /// Displays options for filtering and changing the logs list.
    /// </summary>
    void DrawToolbar()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button(clearLabel))
        {
            logs.Clear();
        }

        collapse = GUILayout.Toggle(collapse, collapseLabel, GUILayout.ExpandWidth(false));

        GUILayout.EndHorizontal();
    }

    /// <summary>
    /// Records a log from the log callback.
    /// </summary>
    /// <param name="message">Message.</param>
    /// <param name="stackTrace">Trace of where the message came from.</param>
    /// <param name="type">Type of message (error, exception, warning, assert).</param>
    void HandleLog(string message, string stackTrace, LogType type)
    {
        logs.Add(new Log
        {
            message = message,
            stackTrace = stackTrace,
            type = type,
        });

        TrimExcessLogs();
    }

    /// <summary>
    /// Removes old logs that exceed the maximum number allowed.
    /// </summary>
    void TrimExcessLogs()
    {
        if (!restrictLogCount)
        {
            return;
        }

        var amountToRemove = Mathf.Max(logs.Count - maxLogs, 0);

        if (amountToRemove == 0)
        {
            return;
        }

        logs.RemoveRange(0, amountToRemove);
    }
#endif
}