using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor;
using System.Reflection;

public partial class TreeNodeWindow : EditorWindow
{

    public TreeNodeGraph data;
    private Vector2 mMousePosition;
    private bool mDrawingConnection = false;
    private TreeNode mSelectNode = null;

    private Rect mToolBarRect;

    private Dictionary<Type, string> mMenuDic = new Dictionary<Type, string>();

    public static T Open<T>(TreeNodeGraph graph) where T : TreeNodeWindow
    {
        T t = GetWindow<T>();
        t.titleContent = new GUIContent(typeof(T).Name);

        t.data = graph;

        return t;
    } 
   

    private void OnEnable()
    {
        hideFlags = HideFlags.HideAndDontSave;
        
        mMenuDic = GetMenu();
       
        if (data == null)
            data = new TreeNodeGraph();
    
    }

    protected void Update()
    {       
        Repaint();
    }

    private void OnGUI()
    {
        Event e = Event.current;
        mMousePosition = e.mousePosition;

        //显示上下文菜单。
        if (e.button == 1)
        {
            if (e.type == EventType.MouseDown)
            {
                if (mDrawingConnection == false)
                {
                    var node = data.GetNode(mMousePosition);

                    mSelectNode = node;

                    OnSelect(mSelectNode);

                    // 显示用于创建节点的上下文菜单（单击不在节点上）。
                    if (mSelectNode == null)
                    {
                        GenericMenu menuToAddQuest = new GenericMenu();
                        var it = mMenuDic.GetEnumerator();
                        while (it.MoveNext())
                        {
                            menuToAddQuest.AddItem(new GUIContent("Add/" + it.Current.Value), false,
                                ContextNodeAddCallback, it.Current.Key);
                        }
                       
                        menuToAddQuest.ShowAsContext();
                        e.Use();
                    }
                    // 显示用于管理节点的上下文菜单。
                    else
                    {
                        if (mSelectNode != null)
                        {
                            GenericMenu menuToControlQuest = new GenericMenu();

                            menuToControlQuest.AddItem(new GUIContent("Clone Node"), false, ContextNodeCloneCallback, mSelectNode.data);
                            menuToControlQuest.AddItem(new GUIContent("Delete Node"), false, ContextNodeDeleteCallback, null);

                            if (mSelectNode.parent != null)
                            {
                                menuToControlQuest.AddItem(new GUIContent("Delete Connection"), false, ContextNodeDeleteConnectionCallback, null);

                            }
                            else
                            {
                                menuToControlQuest.AddItem(new GUIContent("Make Connection"), false, ContextNodeAddConnectionCallback, null);
                            }


                            menuToControlQuest.ShowAsContext();
                            e.Use();
                        }
                    }
                }
                else
                {
                    TreeNode parentNode = data.GetNode(mMousePosition);
                   

                    if (parentNode != null && mSelectNode != null)
                    {
                        if (parentNode.data.ConnectableTo(mSelectNode.data))
                        {
                            mSelectNode.parent = parentNode;
                            parentNode.data.OnConnect(mSelectNode.data);
                        }
                    }
                    mSelectNode = null;
                    mDrawingConnection = false;
                }
            }
        }



        if (mDrawingConnection)
        {
            if (mSelectNode != null)
            {
                Color color = Color.green;
                var current = data.GetNode(mMousePosition);
                if (current != null && current.data.ConnectableTo(mSelectNode.data) == false)
                {
                    color = Color.red;
                }

                Rect rectFrom = mSelectNode.rect;

                Vector3 startPos = new Vector3(rectFrom.x, rectFrom.y + rectFrom.height / 2, 0);
                Vector3 endPos = mMousePosition;
                Vector3 startTan = startPos + Vector3.left * 50;
                Vector3 endTan = endPos + Vector3.left * 50;

                Handles.DrawBezier(startPos, endPos, startTan, endTan, color, null, 3);
            }
            else
            {
                mDrawingConnection = false;
            }
        }

        DrawEditor();

    }


    /// <summary>
    /// 添加节点。
    /// </summary>
    /// <param name="obj">Object.</param>
    private void ContextNodeAddCallback(object obj)
    {
        ITreeNode param = (ITreeNode)Activator.CreateInstance((Type)obj);
       
        TreeNode node = data.AddNode(param);
      
        node.rect = new Rect(mMousePosition.x, mMousePosition.y, TreeNode.WIDTH, TreeNode.HEIGHT);
    }
    /// <summary>
    /// Clone节点。
    /// </summary>
    /// <param name="obj">Object.</param>
    private void ContextNodeCloneCallback(object obj)
    {
        ITreeNode original = (ITreeNode)obj;
        if (original != null)
        {
            ITreeNode param = original.Clone(null);

            TreeNode node = data.AddNode(param);

            node.rect = new Rect(mMousePosition.x, mMousePosition.y, TreeNode.WIDTH, TreeNode.HEIGHT);
        }
    }

    private void ContextNodeDeleteConnectionCallback(object obj)
    {
        if (mSelectNode != null)
        {
            if(mSelectNode.parent!=null)
            {
                mSelectNode.parent.data.OnDisconnect(mSelectNode.data);
            }
            mSelectNode.parent = null;
        }
    }
    /// <summary>
    /// 节点的上下文菜单。
    /// </summary>
    /// <param name="controlActionType">Control action type.</param>
    private void ContextNodeDeleteCallback(object obj)
    {
        if (mSelectNode != null)
        {
            data.RemoveNode(mSelectNode);
           
        }
    }

    /// <summary>
    /// 删除已删除节点中的传出/输入边缘。
    /// </summary>
    /// <param name="nodeGuid"></param>
    private void ContextNodeAddConnectionCallback(object obj)
    {
        if (mSelectNode != null && mSelectNode.parent == null)
        {
            mDrawingConnection = true;
        }
    }




    #region 显示
    static GUIStyle _windowStyle;
    static GUIStyle WindowStyle
    {
        get
        {
            if (_windowStyle == null)
            {
                _windowStyle = new GUIStyle(GUI.skin.window);
            }
            return _windowStyle;
        }
    }
    /// <summary>
    /// 显示编辑器的主要组件
    /// </summary>
    private void DrawEditor()
    {
        DrawToolbar();
        BeginWindows();
        if(data == null)
        {
            data = new TreeNodeGraph();
        }
        for (int i = 0; i < data.nodes.Count; i++)
        {
            //data.nodes[i].rect = GUILayout.Window(i,data.nodes[i].rect, DrawNode, data.nodes[i].id + "-" + data.nodes[i].name, WindowStyle);

            data.nodes[i].rect = GUI.Window(i, data.nodes[i].rect, DrawNode, data.nodes[i].id + "-" + data.nodes[i].name );

            DrawConnection(data.nodes[i]);
        }
        EndWindows();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nodeFrom">Node from.</param>
    /// <param name="nodeTo">Node to.</param>
    private void DrawConnection(TreeNode node)
    {
        var parent = node.parent;
        if (parent != null)
        {
            Rect rectFrom = parent.rect;
            Rect rectTo = node.rect;

            Vector3 startPos = new Vector3(rectFrom.x + rectFrom.width, rectFrom.y + rectFrom.height / 2, 0);
            Vector3 endPos = new Vector3(rectTo.x, rectTo.y + rectTo.height / 2, 0);
            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;

            Handles.DrawBezier(startPos, endPos, startTan, endTan, node.data.GetConnectionColor(), null, 3);
        }
    }

    /// <summary>
    /// Отображение окна ноды с указанным id.
    /// </summary>
    /// <param name="id"></param>
    private void DrawNode(int i)
    {
        if(data == null)
        {
            return;
        }

        if (i >= 0 && i < data.nodes.Count)
        {
            data.nodes[i].DrawWindow();
            GUI.DragWindow();
        }
    }

    //绘制工具栏
    private void DrawToolbar()
    {
        mToolBarRect = new Rect(0, 0, Screen.width, 20);
        GUILayout.BeginArea(mToolBarRect, EditorStyles.toolbar);
        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
        if (GUILayout.Button("New", EditorStyles.toolbarButton, new GUILayoutOption[]
        {
            GUILayout.Width(42f)
        }))
        {
            data = OnNew();
        }
        if (GUILayout.Button("Load", EditorStyles.toolbarButton, new GUILayoutOption[]
        {
            GUILayout.Width(42f)
        }))
        {
            var graph =  OnLoad();
            if (graph != null)
            {
                data = graph;
            }
        }
        if (GUILayout.Button("Save", EditorStyles.toolbarButton, new GUILayoutOption[]
        {
             GUILayout.Width(42f)
        }))
        {
            OnSave(data);
        }
       
        GUILayout.FlexibleSpace();

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }


    #endregion

    #region Get Menu

    public static Dictionary<Type, string> GetMenu()
    {
        Dictionary<Type, string> menus = new Dictionary<Type, string>();
        List<string> assemblies = new List<string>
        {
            "Assembly-CSharp",
            "Assembly-CSharp-firstpass",
            "Assembly-UnityScript",
            "Assembly-UnityScript-firstpass"
        };
        for (int i = 0; i < assemblies.Count; i++)
        {
            Assembly assembly = null;
            try
            {
                assembly = Assembly.Load(assemblies[i]);
            }
            catch (Exception)
            {
                assembly = null;
            }
            if (assembly != null)
            {
                Type[] types = assembly.GetTypes();
                for (int j = 0; j < types.Length; j++)
                {
                    var type = types[j];
                    if (!type.IsAbstract)
                    {
                        if (typeof(ITreeNode).IsAssignableFrom(type) && menus.ContainsKey(type) == false)
                        {
                            var menu = type.GetCustomAttribute<TreeNodeMenuAttribute>();
                            if (menu != null && string.IsNullOrEmpty(menu.menu) == false)
                            {
                                menus.Add(type, menu.menu);
                            }
                        }

                    }
                }
            }
        }
        return menus;
    }

    #endregion

    protected virtual void OnSave(TreeNodeGraph graph)
    {

    }

    protected virtual TreeNodeGraph OnLoad()
    {
        return null;
    }

    protected virtual void OnSelect(TreeNode node)
    {
        mSelectNode = node;
    }

    protected virtual TreeNodeGraph OnNew()
    {
        return  new TreeNodeGraph();
    }
}