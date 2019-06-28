using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor;

public class TreeNodeWindow : EditorWindow
{
    public static TreeNodeWindow mTreeWindow;
    public TreeNodeGraph data;
    private Vector2 mMousePosition;
    private bool mDrawingConnection = false;
    private TreeNode mSelectNode = null;

    private Rect mToolBarRect;

    private List<TreeNodeMenu> mNodeMenuList = new List<TreeNodeMenu>();

    public Action<TreeNodeGraph> onSave;
    public Func<TreeNodeGraph> onLoad;
    public Func<List<TreeNodeMenu>> onInitMenu;
    public Func<TreeNodeGraph> onDataChange;
    public Action<ITreeNode> onSelect;

    [MenuItem("Tools/Tree Node Editor")]
    private static void ShowEditor()
    {
        mTreeWindow = GetWindow<TreeNodeWindow>();
        mTreeWindow.titleContent = new GUIContent("NodeTree");
      
    }
    public static void SetData(TreeNodeGraph graph)
    {
        if(mTreeWindow== null)
        {
            ShowEditor();
        }
        if(mTreeWindow!=null)
        {
            mTreeWindow.data = graph;
        }
    }

    private void OnEnable()
    {
        hideFlags = HideFlags.HideAndDontSave;

        onLoad = EntityParamTool.OnLoad;
        onSave = EntityParamTool.OnSave;
        onInitMenu = EntityParamTool.OnInitMenu;
        onDataChange = EntityParamTool.onDataChange;

        if (onInitMenu != null)
        {
            mNodeMenuList = onInitMenu();
        }
       
        if (data == null)
            data = new TreeNodeGraph();
    
    }

    private void Update()
    {
        if (onDataChange != null)
        {
            var graph = onDataChange();
            if (graph != null && data != graph)
            {
                data = graph;
            }
        }
        Repaint();
    }

    private void OnGUI()
    {
        Event e = Event.current;
        mMousePosition = e.mousePosition;

        // create
        if (mTreeWindow == null)
            ShowEditor();

        //显示上下文菜单。
        if (e.button == 1)
        {
            if (e.type == EventType.MouseDown)
            {
                if (mDrawingConnection == false)
                {
                    var node = data.GetNode(mMousePosition);
                    OnSelectNode(node);

                    // 显示用于创建节点的上下文菜单（单击不在节点上）。
                    if (mSelectNode == null)
                    {
                        GenericMenu menuToAddQuest = new GenericMenu();

                        for (int i = 0; i < mNodeMenuList.Count; ++i)
                        {
                            menuToAddQuest.AddItem(new GUIContent("Add/"+mNodeMenuList[i].name), false, ContextNodeAddCallback, mNodeMenuList[i].type);
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

    private void OnSelectNode(TreeNode node)
    {
        mSelectNode = node;
        if (onSelect != null && mSelectNode!= null)
        {
            onSelect(mSelectNode.data);
        }
    }


    #region 显示
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
            data = new TreeNodeGraph();
        }
        if (GUILayout.Button("Load", EditorStyles.toolbarButton, new GUILayoutOption[]
        {
            GUILayout.Width(42f)
        }))
        {
            if(onLoad!=null)
            {
               data = onLoad();
            }
        }
        if (GUILayout.Button("Save", EditorStyles.toolbarButton, new GUILayoutOption[]
        {
             GUILayout.Width(42f)
        }))
        {
            if(onSave!=null)
            {
                onSave(data);
            }
           
        }
       
        GUILayout.FlexibleSpace();

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    
    #endregion
}