using System;
using UnityEngine;
public interface ITreeNode
{
    string name { get; set; }
    Rect rect { get; set; }
    void OnDraw(ref Rect rect);
    bool ConnectableTo(ITreeNode node);
    void OnConnect(ITreeNode node);
    void OnDisconnect(ITreeNode node);
    ITreeNode Clone(ITreeNode node);
    Action<ITreeNode> OnAddChild { get; set; }
    Color GetConnectionColor();
}

