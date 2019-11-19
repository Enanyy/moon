#define ENABLE_SCREEN_LOG
using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class ScreenLogger : MonoBehaviour
{
    private static ScreenLogger mInstance;
    
    public static void SetActive(bool active)
    {
        if(active)
        {
            if (mInstance == null)
            {
                mInstance = FindObjectOfType<ScreenLogger>();
                if (mInstance == null)
                {
                    GameObject go = new GameObject(typeof(ScreenLogger).Name);
                    mInstance = go.AddComponent<ScreenLogger>();
                }
            }
            mInstance.gameObject.SetActive(active);
        }
        else
        {
            if(mInstance)
            {
                mInstance.gameObject.SetActive(active);
            }
        }
    }

#if ENABLE_SCREEN_LOG
    struct Log
    {
        public string message;
        public string stackTrace;
        public LogType type;
        public string time;
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

    public bool showStackTrace = false;

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
    const int margin = 0;
    static readonly GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console.");
    static readonly GUIContent stackTraceLabel = new GUIContent("Show Stack Trace", " Show stack trace of logs.");
    static readonly GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");
    static readonly GUIContent closeLabel = new GUIContent("Close", "Hide the console.");
    static readonly GUIContent saveLabel = new GUIContent("Save", "Save logs to file.");

    readonly Rect titleBarRect = new Rect(0, 0, 10000, 20);
   
    void Awake()
    {
        //Debug.LogError("test 1");
        //Debug.Log("test 2");
        //visible = true;
        DontDestroyOnLoad(gameObject);
        mInstance = this;
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
        if (visible)
        {
            GUILayout.Window(123456, new Rect(margin, margin, Screen.width - (margin * 2), Screen.height - (margin * 2)), DrawConsoleWindow, windowTitle);
        }
        else
        {
            int w = Screen.width / 5;
            GUILayout.BeginArea(new Rect(Screen.width / 2 - w / 2, 0, w, Screen.height / 10));
            GUILayoutOption o1 = GUILayout.Height(40);
            GUILayoutOption o2 = GUILayout.Width(100);
            GUILayoutOption[] oo = { o1, o2 };
            if (GUILayout.Button("Show", oo)) visible = true;
            GUILayout.EndArea();
        }
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
            // Save GUI colour before change it.

            Color color = GUI.skin.label.normal.textColor;
            GUI.skin.label.normal.textColor = logTypeColors[log.type];
            if (showStackTrace)
            {
                GUILayout.Label(string.Format("[{0} {1}] {2}\n{3}", log.type.ToString(), log.time, log.message, log.stackTrace));
            }
            else
            {
                GUILayout.Label(string.Format("[{0} {1}] {2}", log.type.ToString(), log.time, log.message));
            }
            // Ensure GUI colour is reset before drawing other components.
            GUI.skin.label.normal.textColor = color;
        }

        GUILayout.EndScrollView();

    }

    /// <summary>
    /// Displays options for filtering and changing the logs list.
    /// </summary>
    void DrawToolbar()
    {
        GUILayout.BeginHorizontal();

        if(GUILayout.Button(saveLabel))
        {
            Save();
        }

        if (GUILayout.Button(clearLabel))
        {
            logs.Clear();
        }
        
        if (GUILayout.Button(closeLabel))
        {
            visible = false;
        }

        showStackTrace = GUILayout.Toggle(showStackTrace, stackTraceLabel, GUILayout.ExpandWidth(false));
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
            time = System.DateTime.Now.ToString("HH:mm:ss"),
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

    void Save()
    {
#if UNITY_EDITOR
        string file = string.Format("{0}/log/log_{1}.txt", Application.dataPath, System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss"));
#else
        string file = string.Format("{0}/log/log_{1}.txt", Application.persistentDataPath, System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss"));
#endif

        StringBuilder builder = new StringBuilder();
        for(int i = 0; i < logs.Count; ++i)
        {
            var log = logs[i];
            builder.AppendFormat("[{0} {1}] {2}\n{3}\n\n", log.type.ToString(), log.time, log.message, log.stackTrace);
        }

        FileEx.SaveFile(file, builder.ToString());

    }
#endif
}
